using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class BaronController : ModCharacterController
{
    private static CharacterController? _characterController;
    public override void AfterFullInit(CharacterController instance)
    {
        instance._gameManager.ArcanaManager.ActiveArcanas.Add(ArcanaType.D03_BREAD_ANATHEMA);
        instance._gameManager.ArcanaManager.TriggerArcana(ArcanaType.D03_BREAD_ANATHEMA);
        _characterController = instance;
        
        var prefixes = new List<MethodInfo>();
        var postfixes = new List<MethodInfo>();
        prefixes.Add(typeof(Roast).GetMethod("GetTaken"));
        prefixes.Add(typeof(BaronHarmony).GetMethod("RoastGetTaken"));
        prefixes.Add(typeof(Pickup_TP_WallChicken).GetMethod("GetTaken"));
        prefixes.Add(typeof(BaronHarmony).GetMethod("WallChickenGetTaken"));
        prefixes.Add(typeof(Gem).GetMethod("GetTaken"));
        prefixes.Add(typeof(BaronHarmony).GetMethod("GemGetTaken"));
        prefixes.Add(typeof(Pickup).GetMethod("SetFrame"));
        prefixes.Add(typeof(BaronHarmony).GetMethod("PickupSetFrame"));
        postfixes.Add(typeof(EnemyController).GetMethod("Die"));
        postfixes.Add(typeof(BaronHarmony).GetMethod("EnemyControllerDie"));
        HarmonyPatching.Patch(typeof(BaronHarmony), prefixes, postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(BaronHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    public override void LevelUp(CharacterController instance)
    {
        if(instance.Level % 6 != 0) return;
        
        foreach (var equip in instance._weaponsManager.HiddenEquipment)
        {
            if(equip._equipmentType != WeaponType.FOLLOWER_KNIFE1 || equip.Level == 8) continue;

            equip.LevelUp();
            break;
        }
    }
    
    private static class BaronHarmony
    {
        private static bool _gemRecovery = false;
        
        // Prefixes
        public static void PickupSetFrame(Pickup __instance, ref string spriteName)
        {
            if (__instance._targetPlayer == null) return;
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return;
            var luckyNum = 2 * (1 + __instance._targetPlayer.PLuck() / 10);
            if (Random.Range(0, 100) <= luckyNum)
                spriteName = "corn";
        }
        
        public static bool RoastGetTaken(Roast __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return true;
            if (__instance.SpriteName == "corn")
                __instance._gameManager.AddPlayerXp(__instance._gameManager.LevelUpFactory.XpRequiredToLevelUp - _characterController._xp, XPMultiplierMode.IgnoreAll);
            
            __instance.AddToRunPickups();
            __instance.SetHasSeenItem();
            SoundManager.PlaySound(SfxType.Roast, new SoundManager.SoundConfig
            {
                Volume = new Il2CppSystem.Nullable<float>(0.6f),
                Rate = 1f
            }, 500f, 4);
            __instance._gameManager.ArcanaManager.OnFoodPickedUp(__instance._targetPlayer, __instance.PickupType, __instance.Value);
            __instance._taken = true;
            PhysicsManager.Instance._pickupGroup.remove(__instance);
            PhysicsManager.Instance._goToPlayerPickupGroup.remove(__instance);
            __instance._doOnlineDespawn = true;
            __instance.Despawn();
            __instance.TrackItemPickup();
            __instance.PickupCallback?.Invoke(__instance);
            
            return false;
        }
        
        public static bool GemGetTaken(Gem __instance)
        {
            if (__instance._targetPlayer == null) return true;
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return true;
            if (__instance._gameManager._arcanaManager.PewPew) return true;

            if (_characterController.MaxHp() <= _characterController._currentHp) return true;
            
            var recover = __instance.Value * (0.2f + (_characterController.PGrowth() / 10));
            _characterController.RecoverHp(recover, true, true);
            return false;
        }
    
        public static bool WallChickenGetTaken(Pickup_TP_WallChicken __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return true;
            return false;
        }
        
        // Postfixes
        public static void EnemyControllerDie(EnemyController __instance)
        {
            if (_characterController == null) return;
            var luckyNum = 2 * (1 + _characterController?.PLuck() / 10);
            if (Random.Range(0, 100) >= luckyNum) return;
            
            var item = GM.Core.MakePickup(__instance.CurrentPos, ItemType.ROAST);
            item.SetFrame("corn");
        }
    }
}

public sealed class BaronStats : BaseCharacterData
{
    public BaronStats()
    {
        CharName = "Baron";
        SurName = "Husker";
        TextureName = "baron_husker_walk";
        SpriteName = "baron_husker_walk_01.png";
        Description = "A former maize magnate corrupted by corn. He seeks to turn all land into his personal crop.";
        StartingWeapon = WeaponType.SILF2;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        obj["hiddenWeapons"] = new JArray("FOLLOWER_KNIFE1");
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}