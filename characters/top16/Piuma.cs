using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ContestCharacters.characters.top16;

public class PiumaController : ModCharacterController
{
    private static CharacterController? _characterController;
    private static int amount;
    private static int amount2;

    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        amount = 6;
        amount2 = 6;
        var postfixes = new List<MethodInfo>
        {
            typeof(WeaponsFacade).GetMethod("AddWeapon"),
            typeof(PiumaHarmony).GetMethod("WeaponsFacadeAddWeapon"),
            typeof(AccessoriesFacade).GetMethod("AddAccessory"),
            typeof(PiumaHarmony).GetMethod("AccessoriesFacadeAddAccessory"),
        };
        HarmonyPatching.Patch(typeof(PiumaHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
        
        _characterController._playerStats.Amount._val += amount;
        if (_characterController._skinType != SkinType.DEFAULT2) return;
        _characterController._playerStats.Amount._val += amount2;
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(PiumaHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private static class PiumaHarmony
    {
        public static void WeaponsFacadeAddWeapon(WeaponsFacade __instance, WeaponType weaponType, CharacterController character, bool removeFromStore)
        {
            if (_characterController == null) return;
            if (_characterController != character) return;
            
            _characterController._playerStats.Amount._val -= amount;
            amount = Mathf.Max(0, 6 - _characterController.WeaponsManager.ActiveEquipment.Count);
            _characterController._playerStats.Amount._val += amount;
        }
        
        public static void AccessoriesFacadeAddAccessory(AccessoriesFacade __instance, WeaponType accessoryType, CharacterController characterController, bool removeFromStore)
        {
            if (_characterController == null) return;
            if (_characterController != characterController) return;
            if (_characterController._skinType != SkinType.DEFAULT2) return;
            
            _characterController._playerStats.Amount._val -= amount2;
            amount2 = Mathf.Max(0, 6 - _characterController.AccessoriesManager.ActiveEquipment.Count);
            _characterController._playerStats.Amount._val += amount2;
        }
    }
}

public sealed class PiumaStats : BaseCharacterData
{
    public PiumaStats()
    {
        CharName = "Piuma";
        SurName = "Ferro";
        TextureName = "Piuma_Ferro_walk_new";
        SpriteName = "piuma_legacy_walk_01.png";
        PortraitName = "p_piuma.png";
        Description = "Starts with +1 level. Gains +1 Amount for every unused weapon slot.";
        Skins = new JArray();
        WalkFrameRate = 8;
        WalkingFrames = 4;
        StartingWeapon = WeaponType.VOID;
        MaxHp = 60;
        Regen += 0.02f;
        MoveSpeed -= 0.01f;
        Revivals = 1;
        Luck += 0.1f;
        Armor += 5;
        
        JObject skin = new JObject
        {
            ["name"] = "Weaponized",
            ["suffix"] = "(Weaponized)",
            ["textureName"] = "Piuma_Ferro_walk_new",
            ["spriteName"] = "Piuma_Ferro_walk_01.png",
            ["description"] = "Starts with +1 level. Gains +1 Amount for every unused weapon and accessory slot. Has a hidden No Future and Vandalier",
            ["hiddenWeapons"] = new JArray("SILF3", "ROCHER"),
            ["walkingFrames"] = 4,
            ["walkFrameRate"] = 8,
            ["unlocked"] = true,
            ["skinType"] = "DEFAULT2"
        };
        JObject skin2 = new JObject()
        {
            ["name"] = "Default",
            ["textureName"] = "Piuma_Ferro_walk_new",
            ["spriteName"] = "piuma_legacy_walk_01.png",
            ["walkingFrames"] = 4,
            ["walkFrameRate"] = 8,
            ["unlocked"] = true,
            ["skinType"] = "DEFAULT"
        };
        
        Skins.Add(skin2);
        Skins.Add(skin);
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        obj.Add("exLevels", 1);
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}