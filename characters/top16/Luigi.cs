using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
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
        if (_characterController._cachedTransform.localScale.x < 4f)
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



public sealed class LuigiStats : BaseCharacterData
{
    public LuigiStats()
    {
        CharName = "Head Chef";
        SurName = "Luigi";
        TextureName = "chef_luigi_walk";
        SpriteName = "chef_luigi_walk_01.png";
        Description = "Gets stronger (and larger) the more he eats";
        StartingWeapon = WeaponType.HELLFIRE;
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