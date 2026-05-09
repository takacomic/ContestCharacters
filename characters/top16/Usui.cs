using System.Reflection;
using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class UsuiController : ModCharacterController
{
    private static CharacterController? _characterController;
    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        var postfixes = new List<MethodInfo>
        {
            typeof(CharacterController).GetMethod("RecoverHp"),
            typeof(UsuiHarmony).GetMethod("CharacterControllerRecoverHp")
        };
        HarmonyPatching.Patch(typeof(UsuiHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(UsuiHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private static class UsuiHarmony
    {
        public static void CharacterControllerRecoverHp(CharacterController __instance, float value, bool showRecovery, bool mulByRegen)
        {
            if (__instance._characterType != _characterController?._characterType) return;
            var currentHp = __instance._currentHp;
            value *= (mulByRegen ? (1f + __instance.PRegen()) : 1f);
            if (GM.Core.ArcanaManager.ActiveArcanas.Contains(ArcanaType.T06_SARABANDE))
            {
                value *= 2f;
            }

            currentHp += value;
            if (!(currentHp > __instance.MaxHp())) return;
            if (Random.Range(1, 101) <= 10)
                GM.Core.StopTimeForMilliseconds(5000f);
        }
    }
}

public sealed class UsuiStats : BaseCharacterData
{
    public UsuiStats()
    {
        CharName = "Usui";
        SurName = "Yukimi";
        TextureName = "Usui_Yukimi_walk";
        SpriteName = "Usui_Yukimi_walk_01.png";
        Description = "Kitsune far from home.";
        StartingWeapon = WeaponType.SANTAJAVELIN;
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