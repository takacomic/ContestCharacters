using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Stage;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class BetaController : ModCharacterController
{
    private static CharacterController _characterController;

    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        var postfixes = new List<MethodInfo>();
        postfixes.Add(typeof(GameManager).GetMethod("MakeTreasure"));
        postfixes.Add(typeof(BetaHarmony).GetMethod("GameManagerMakeTreasure"));
        HarmonyPatching.Patch(typeof(BetaHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(BetaHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private static class BetaHarmony
    {
        public static void GameManagerMakeTreasure( GameManager __instance, Vector2 pos, Treasure treasure, bool isRemote, ref TreasureChest __result)
        {
            if (__instance.PlayerOne != _characterController) return;
            if (treasure.hasRandoms) return;
             if (!treasure.prizeTypes.Contains(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EVOLUTION)))
            {
                treasure.prizeTypes.Clear();
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EVOLUTION));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                __result.SetData(ItemType.TREASURE, treasure);
                __result.SetWithEvo();
            }
            else if (!treasure.hasArcana)
            {
                treasure.prizeTypes.Clear();
                treasure.hasArcana = true;
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EVO_ARCANA));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                treasure.prizeTypes.Add(new Il2CppSystem.Nullable<PrizeType>(PrizeType.EXISTING_ANY));
                __result.SetData(ItemType.TREASURE, treasure);
                __result.SetArcana(true);
                
            }
             
            var x = Random.Range(0, 20);
            if (x > 3) return;
            treasure.prizeTypes.Clear();
            __instance.MakePickup(pos, ItemType.SV_DRAFT1);
            PickupManager.RemovePickup(__result);
        }
    }
}

public sealed class BetaStats : BaseCharacterData
{
    public BetaStats()
    {
        CharName = "Beta";
        SurName = "";
        TextureName = "beta_walk";
        SpriteName = "beta_walk_01.png";
        PortraitName = "p_beta.png";
        Description = "A gilded shadow beast, continues to keep secrets.";
        StartingWeapon = WeaponType.MANNAGGIA;
        Power += 0.1f;
        Speed -= 0.1f;
        Duration += 0.1f;
        Area -= 0.2f;
        Growth += 0.15f;
        Amount += 1f;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        jArray.Add(obj);
        obj.Add("hiddenWeapons", new JArray("FOLLOWER_KNIFE1"));
        obj.Add("onEveryLevelUp", new JObject(
            new JProperty("growth", 0.01f)
        ));
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}