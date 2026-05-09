using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class RubriccoController : ModCharacterController
{
    private static bool standing;
    private bool cooldown;
    private bool speed;
    private static CharacterController? _characterController;

    public override void AfterFullInit(CharacterController instance)
    {
        standing = false;
        cooldown = false;
        speed = false;
        _characterController = instance;
        var prefixes = new List<MethodInfo>
        {
            typeof(Weapon).GetMethod("Fire", new Type[] {typeof(bool)}),
            typeof(RubriccoHarmony).GetMethod("WeaponFire")
        };
        HarmonyPatching.Patch(typeof(RubriccoHarmony), prefixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(RubriccoHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    public override void OnUpdate(CharacterController instance)
    {
        if (instance.Walked != 0f)
        {
            if (standing) return;
            standing = true;
            if (!cooldown)
            {
                _characterController._playerStats.Cooldown._val -= 0.1f;
                cooldown = true;
            }

            if (!speed) return;
            
            _characterController._playerStats.Speed._val -= 0.1f;
            speed = false;
        }
        else
        {
            if (!standing) return;
            standing = false;
            if (cooldown)
            {
                _characterController._playerStats.Cooldown._val += 0.1f;
                cooldown = false;
            }
            if (!speed)
            {
                _characterController._playerStats.Speed._val += 0.1f;
                speed = true;
            }
        }
    }
    
    public static class RubriccoHarmony
    {
        public static bool WeaponFire(Weapon __instance, bool skipTriggers)
        {
            if (standing) return true;
            if (_characterController.WeaponsManager.ActiveEquipment.Count <= 1) return true;
            return Random.Range(1 , 101) > 5;
        }
    }
}

public sealed class RubriccoStats : BaseCharacterData
{
    public RubriccoStats()
    {
        CharName = "Rubricco";
        SurName = "Puzzorelio";
        TextureName = "Rubricco_Puzzorelio_walk";
        SpriteName = "Rubricco_Puzzorelio_walk_01.png";
        Description = "Sometimes it pays to focus, others to just run like crazy as long as you have an idea of what you are doing";
        StartingWeapon = WeaponType.DIAMOND;
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