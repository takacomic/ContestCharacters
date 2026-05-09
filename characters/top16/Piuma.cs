using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
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

    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        amount = 0;
        var postfixes = new List<MethodInfo>
        {
            typeof(EquipmentManager).GetMethod("AddEquipment"),
            typeof(PiumaHarmony).GetMethod("EquipmentManagerAddEquipment"),
        };
        HarmonyPatching.Patch(typeof(PiumaHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(PiumaHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private static class PiumaHarmony
    {
        public static void EquipmentManagerAddEquipment(EquipmentManager __instance, Equipment item)
        {
            if (_characterController == null) return;
            _characterController._playerStats.Amount._val -= amount;
            var num = 0;
            foreach (var equip in _characterController.WeaponsManager.ActiveEquipment)
            {
                if (equip is Weapon weapon && !weapon.CurrentWeaponData.isPowerUp)
                {
                    num++;
                }
            }
            amount = Mathf.Max(0, 6 - num);

            _characterController._playerStats.Amount._val += amount;
        }
    }
}

public sealed class PiumaStats : BaseCharacterData
{
    public PiumaStats()
    {
        CharName = "Piuma";
        SurName = "Ferro";
        TextureName = "Piuma_Ferro_walk";
        SpriteName = "Piuma_Ferro_walk_01.png";
        Description = "Starts with +1 level";
        StartingWeapon = WeaponType.VOID;
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