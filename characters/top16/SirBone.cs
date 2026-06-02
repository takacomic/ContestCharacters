using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Characters.Enemies;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
            typeof(SirBoneHarmony).GetMethod("GameManagerMakePickup"),
            typeof(StageEventManager).GetMethod("fnChicken"),
            typeof(SirBoneHarmony).GetMethod("StageEventManagerfnChicken"),
            typeof(EnemyRoastable_HorizontalRight).GetMethod("GetDamaged"),
            typeof(SirBoneHarmony).GetMethod("EnemyRoastable_HorizontalRightGetDamaged"),
            typeof(EnemyRoastable).GetMethod("GetDamaged"),
            typeof(SirBoneHarmony).GetMethod("EnemyRoastableGetDamaged"),
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
            ref ItemType itemType,
            WeaponType weaponType,
            float value,
            ItemType relicType,
            bool shouldCallValidatePickups,
            bool isRemote,
            bool onlineSynchronization)
        {
            if (_characterController == null) return;
            if (itemType == ItemType.ROAST || itemType == ItemType.TP_WALL_CHICKEN)
            {
                itemType = ItemType.COINBAG2;
            }
        }
        
        public static void StageEventManagerfnChicken(StageEventManager __instance)
        {
            if (_characterController == null) return;
            var screenCenter = GM.Core.scene.renderer.screenCenter;
            for (var i = 0; i < 12; i++)
            {
                var x = screenCenter.x + Mathf.Sin(MathF.PI / 6f * (float)i) * 1.75f;
                var y = screenCenter.y + Mathf.Cos(MathF.PI / 6f * (float)i) * 1.75f;
                GM.Core.MakePickup(new Vector2(x, y), ItemType.COINBAG2);
            }
            GM.Core.GizmoManager.ShowHighlightAt(__instance.PlayerPos.x, __instance.PlayerPos.y);
        }
        
        public static void EnemyRoastable_HorizontalRightGetDamaged(EnemyRoastable_HorizontalRight __instance, float value, HitVfxType showHitVfx, float damageKb, WeaponType damageType, bool hasKb)
        {
            if (_characterController == null) return;
            __instance.itemChance = -1f;
            bool flag = Array.IndexOf(EnemyController.FireDamageTypes, damageType) > -1;
            if (flag && Random.value < 0.2f)
            {
                Vector3 vector = __instance.transform.position;
                vector.y += 0.12f;
                __instance._gameManager.MakePickup(vector, ItemType.COINBAG2);
            }
        }
        public static void EnemyRoastableGetDamaged(EnemyRoastable __instance, float value, HitVfxType showHitVfx, float damageKb, WeaponType damageType, bool hasKb)
        {
            if (_characterController == null) return;
            __instance.itemChance = -1f;
            bool flag = Array.IndexOf(EnemyController.FireDamageTypes, damageType) > -1;
            if (flag && Random.value < 0.2f)
            {
                Vector3 vector = __instance.transform.position;
                vector.y += 0.12f;
                __instance._gameManager.MakePickup(vector, ItemType.COINBAG2);
            }
        }
    }
}

public sealed class SirBoneStats : BaseCharacterData
{
    public SirBoneStats()
    {
        CharName = "Sir Bone";
        SurName = "";
        TextureName = "Sir_Bone_walk";
        SpriteName = "Sir_Bone_walk_01.png";
        PortraitName = "p_sirbone.png";
        Description = "+1 armor and +50% Greed, Gains +5% speed and curse every 10 levels max +40%. Healing items are turned into coins.";
        StartingWeapon = WeaponType.BONE;
        Armor += 1;
        Greed += 0.5f;
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