using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Algorithm;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Signals;
using Il2CppZenject;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContestCharacters.characters.top16;

public class LuigiController : ModCharacterController
{
    private static CharacterController? _characterController;
    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        var postfixes = new List<MethodInfo>
        {
            typeof(Roast).GetMethod("GetTaken"),
            typeof(LuigiHarmony).GetMethod("RoastGetTaken"),
            typeof(Pickup_TP_WallChicken).GetMethod("GetTaken"),
            typeof(LuigiHarmony).GetMethod("WallChickenGetTaken")
        };
        HarmonyPatching.Patch(typeof(LuigiHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(LuigiHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private static void FoodBuffs()
    {
        var x = _characterController.transform.localScale.x;
        if (_characterController._cachedTransform.localScale.x < 6f)
            _characterController.setScale( x *= 1.02f, new Il2CppSystem.Nullable<float>());
        _characterController._playerStats.Power._val += 0.01f;
        _characterController._playerStats.Area._val += 0.01f;
        _characterController._playerStats.MaxHp._val += 2;
        _characterController._playerStats.MoveSpeed._val -= 0.02f;
        _characterController._playerStats.Speed._val -= 0.01f;
    }
    
    private static class LuigiHarmony
    {
        public static void RoastGetTaken(Roast __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return;
            FoodBuffs();
        }
    
        public static void WallChickenGetTaken(Pickup_TP_WallChicken __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return;
            FoodBuffs();
        }
    }
}



public class LuigiStats : BaseCharacterData
{
    public LuigiStats()
    {
        Prefix = "Head Chef";
        CharName = "Luigi";
        SurName = "";
        TextureName = "Mortis_Surmanski_walk_duo";
        SpriteName = "chef_luigi_walk_01.png";
        PortraitName = "p_luigi.png";
        Description = "Gets stronger (and larger) the more he eats. Gains +1% Might and Area, +2 Max Hp, -2% Move Speed, and -1% Projectile Speed on when consuming food.";
        StartingWeapon = WeaponType.HELLFIRE;
        MoveSpeed += 0.05f;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}