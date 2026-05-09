using System.Reflection;
using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContestCharacters.characters.top16;

public class SleinController : ModCharacterController
{
    private static CharacterController? _characterController;
    public override void AfterFullInit(CharacterController instance)
    {
        instance._gameManager.ArcanaManager.ActiveArcanas.Add(ArcanaType.T21_BLOODY);
        instance._gameManager.ArcanaManager.TriggerArcana(ArcanaType.T21_BLOODY);
        _characterController = instance;
        var prefixes = new List<MethodInfo>
        {
            typeof(PentagramProjectile).GetMethod("EraseEnemies"),
            typeof(SleinHarmony).GetMethod("PentagramProjectileEraseEnemies"),
            typeof(GameManager).GetMethod("RosaryDamage"),
            typeof(SleinHarmony).GetMethod("GameManagerRosaryDamage")
        };
        HarmonyPatching.Patch(typeof(SleinHarmony), prefixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(SleinHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private static class SleinHarmony
    {
        public static void PentagramProjectileEraseEnemies(PentagramProjectile __instance, bool erase)
        {
            if (_characterController == null) return;
            
            foreach (var enemyController in __instance._weapon.GameMan.Stage.GetAllEnemiesInScreenBounds())
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
    
        public static void GameManagerRosaryDamage(GameManager __instance, bool showVfx, float volume, WeaponType damageType, bool setDark)
        {
            if (_characterController == null) return;

            foreach (var enemyController in __instance._stage.SpawnedEnemies)
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
    }
}

public sealed class SleinStats : BaseCharacterData
{
    public SleinStats()
    {
        CharName = "Slein";
        SurName = "the Great Demon King";
        TextureName = "Slein_walk";
        SpriteName = "Slein_walk_01.png";
        Description = "Loves pentagrams.";
        StartingWeapon = WeaponType.PENTAGRAM;
        
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