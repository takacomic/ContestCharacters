using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Phaser;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Objects.Pools;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using Il2CppVampireSurvivors.Tools;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class BaronController : ModCharacterController
{
    private static CharacterController? _characterController;
    private static int _cornConsumed;
    private static int _hiddenSilf;
    public override void AfterFullInit(CharacterController instance)
    {
        instance._gameManager.ArcanaManager.ActiveArcanas.Add(ArcanaType.D03_BREAD_ANATHEMA);
        instance._gameManager.ArcanaManager.TriggerArcana(ArcanaType.D03_BREAD_ANATHEMA);
        _characterController = instance;
        _cornConsumed = 0;
        _hiddenSilf = 0;
        
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
        prefixes.Add(typeof(BattiliaProjectile).GetMethod("InitProjectile"));
        prefixes.Add(typeof(BaronHarmony).GetMethod("BattiliaProjectileInitProjectile"));
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
        if (instance._level % 6 != 0) return;
        var hiddens = instance.WeaponsManager.HiddenEquipment;
        foreach (var hidden in hiddens)
        {
            if (!hidden.TryCast<Silf2Weapon>()) return;
            if (hidden.Level >= 8) continue;
            hidden.LevelUp();
        }
    }

    private static void DoRandomCorn(CharacterController instance)
    {
        _cornConsumed++;
        var x = Random.Range(1, 9);
        if (_cornConsumed % 100 == 0 && _hiddenSilf < 3)
        {
            x = 9;
            _hiddenSilf++;
        }

        switch (x)
        {
            case 1:
                instance._gameManager.MakeAndActivatePickup(ItemType.VACUUM, instance);
                break;
            case 2:
                instance._gameManager.MakeAndActivatePickup(ItemType.ROSARY, instance);
                break;
            case 3:
                instance._gameManager.MakeAndActivatePickup(ItemType.NFT, instance);
                break;
            case 4:
                instance._gameManager.MakeAndActivatePickup(ItemType.GILDED, instance);
                break;
            case 5:
                instance._gameManager.MakeAndActivatePickup(ItemType.COINBAGMAX, instance);
                break;
            case 6: 
                instance._gameManager.AddPlayerXp(instance._gameManager.LevelUpFactory.XpRequiredToLevelUp - instance._xp, XPMultiplierMode.IgnoreAll);
                break;
            case 7:
                var stats = new ModifierStats
                {
                    Power = 0.02f,
                    Speed = 0.02f,
                    Duration = 0.02f,
                    Area = 0.02f
                };
                instance.PlayerStatsUpgrade(stats);
                break;
            case 8:
                stats = new ModifierStats
                {
                    Growth = 0.005f
                };
                instance.PlayerStatsUpgrade(stats);
                break;
            case 9:
                instance._gameManager.AddHiddenWeapon(WeaponType.SILF2, instance, true);
                break;
        }
    }
    
    private static class BaronHarmony
    {
        private static bool _gemRecovery = false;
        
        // Prefixes
        public static void PickupSetFrame(Pickup __instance, ref string spriteName)
        {
            /*if (__instance._targetPlayer == null) return;
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return;
            if (Random.Range(0, 150) <= 2)
                spriteName = "corn";*/
        }
        
        public static bool RoastGetTaken(Roast __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return true;
            if (__instance.SpriteName == "corn")
                DoRandomCorn(_characterController);
            
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
            
            var recover = __instance.Value * (0.2f + _characterController.PGrowth());
            _characterController.RecoverHp(recover, true, true);
            PhysicsManager.Instance._pickupGroup.remove(__instance);
            PhysicsManager.Instance._goToPlayerPickupGroup.remove(__instance);
            __instance._doOnlineDespawn = true;
            __instance.Despawn();
            __instance.TrackItemPickup();
            __instance.PickupCallback?.Invoke(__instance);
            return false;
        }
    
        public static bool WallChickenGetTaken(Pickup_TP_WallChicken __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return true;
            return false;
        }

        public static void BattiliaProjectileInitProjectile(BattiliaProjectile __instance, BulletPool pool, Weapon weapon, int index)
        {
            if (weapon.Owner == null) return;
            if (weapon.Owner._characterType != _characterController?._characterType) return;
            __instance._batSprite = PhaserWorld.Instance.AddPhaserSprite(__instance.position, "pakoebony.png", "pakoebony_01");
            __instance._shadowSprite = PhaserWorld.Instance.AddPhaserSprite(__instance.position, "pakoebony.png", "pakoebony_01");
            Il2CppSystem.Collections.Generic.List<Sprite> animation =
                new Il2CppSystem.Collections.Generic.List<Sprite>(2);
            animation.Add(SpriteManager.GetSprite("pakoebony_01", "pakoebony.png"));
            animation.Add(SpriteManager.GetSprite("pakoebony_02", "pakoebony.png"));
            __instance._batSprite.Anim.AddAnimation("idle1", animation, 6, shouldLoop: true);
            __instance._batSprite.Anim.AddAnimation("idle2", animation, 6, shouldLoop: true);
            __instance._batSprite.Anim.AddAnimation("idle3", animation, 6, shouldLoop: true);
            __instance._batSprite.Anim.AddAnimation("idle4", animation, 6, shouldLoop: true);
            
        }
        
        // Postfixes
        public static void EnemyControllerDie(EnemyController __instance)
        {
            if (_characterController == null) return;
            if (Random.Range(0, 150) >= 2) return;
            
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
        PortraitName = "p_husker.png";
        Description = "A former maize magnate corrupted by corn. He seeks to turn all land into his personal crop. Has a hidden Pako Battiliar. Gains 1 hidden Ebony Wings every 100 corn (Max 3). Can not heal from food based items. Consuming corn triggers a random effect";
        MaxHp = 66;
        Regen -= 0.5f;
        MoveSpeed -= 0.2f;
        Duration += 0.1f;
        Area += 0.1f;
        Greed += 0.2f;
        Curse += 0.2f;
        Banish += 66;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        obj["hiddenWeapons"] = new JArray("BATTILIA");
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}