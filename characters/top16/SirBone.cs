using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ContestCharacters.characters.top16;

public class SirBoneController : ModCharacterController
{
    private static CharacterController? _characterController;

    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        var prefixes = new List<MethodInfo>
        {
            typeof(GameManager).GetMethod("MakePickup"),
            typeof(SirBoneHarmony).GetMethod("GameManagerMakePickup")
        };
        HarmonyPatching.Patch(typeof(SirBoneHarmony), prefixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(SirBoneHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private static class SirBoneHarmony
    {
        public static void GameManagerMakePickup(GameManager __instance, Vector2 pos,
            ItemType itemType,
            WeaponType weaponType,
            float value,
            ItemType relicType,
            bool shouldCallValidatePickups,
            bool isRemote,
            bool onlineSynchronization)
        {
            if (__instance.Player != _characterController) return;
            if (itemType is ItemType.ROAST or ItemType.TP_WALL_CHICKEN)
            {
                itemType = ItemType.COINBAG2;
            }
        }
    }
}

public sealed class SirBoneStats : BaseCharacterData
{
    public SirBoneStats()
    {
        CharName = "Sir";
        SurName = "Bone";
        TextureName = "Sir_Bone_walk";
        SpriteName = "Sir_Bone_walk_01.png";
        Description = "+1 armor, Gains +5% speed and curse every 10 levels max +40%. Healing items are turned into coins.";
        StartingWeapon = WeaponType.BONE;
        Armor += 1;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        jArray.Add(obj);
        jArray.Add(new JObject(
            new JProperty("level", 10),
            new JProperty("speed", 0.05),
            new JProperty("curse", 0.05)));
        Level20.Add("speed", 0.05);
        Level20.Add("curse", 0.05);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(new JObject(
            new JProperty("level", 30),
            new JProperty("speed", 0.05),
            new JProperty("curse", 0.05)));
        Level40.Add("speed", 0.05);
        Level40.Add("curse", 0.05);
        jArray.Add(Level40);
        jArray.Add(Level41);
        jArray.Add(new JObject(
            new JProperty("level", 50),
            new JProperty("speed", 0.05),
            new JProperty("curse", 0.05)));
        jArray.Add(new JObject(
            new JProperty("level", 60),
            new JProperty("speed", 0.05),
            new JProperty("curse", 0.05)));
        jArray.Add(new JObject(
            new JProperty("level", 70),
            new JProperty("speed", 0.05),
            new JProperty("curse", 0.05)));
        jArray.Add(new JObject(
            new JProperty("level", 80),
            new JProperty("speed", 0.05),
            new JProperty("curse", 0.05)));
        return JsonConvert.SerializeObject(jArray);
    }
}