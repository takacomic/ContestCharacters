using System.Reflection;
using CoffinTech.Utils;
using Il2Cpp;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Interfaces;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

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
            typeof(SleinHarmony).GetMethod("GameManagerRosaryDamage"),
            typeof(SireProjectile).GetMethod("EraseEnemies"),
            typeof(SleinHarmony).GetMethod("SireProjectileEraseEnemies"),
            typeof(SireProjectile).GetMethod("EraseRandomEnemy"),
            typeof(SleinHarmony).GetMethod("SireProjectileEraseRandomEnemy"),
            typeof(TP_SpiritTornado2_Projectile).GetMethod("EraseEnemies"),
            typeof(SleinHarmony).GetMethod("TP_SpiritTornado2_ProjectileEraseEnemies"),
            typeof(TP_SpiritTornado2_Projectile).GetMethod("EraseRandomEnemies"),
            typeof(SleinHarmony).GetMethod("TP_SpiritTornado2_ProjectileEraseRandomEnemies"),
            typeof(LEM_Inferno2_Weapon).GetMethod("DoCoinRosary"),
            typeof(SleinHarmony).GetMethod("LEM_Inferno2_WeaponDoCoinRosary"),
            typeof(Report2Weapon).GetMethod("EraseEnemyType"),
            typeof(SleinHarmony).GetMethod("Report2WeaponEraseEnemyType"),
            typeof(SantaJavelin2Weapon).GetMethod("WSPDamage"),
            typeof(SleinHarmony).GetMethod("SantaJavelin2WeaponWSPDamage"),
            typeof(TP_PowerOfLire_Projectile).GetMethod("TransformEnemies"),
            typeof(SleinHarmony).GetMethod("TP_PowerOfLire_ProjectileTransformEnemies"),
            typeof(TP_Holy2_WeaponSupport).GetMethod("RosaryDamage"),
            typeof(SleinHarmony).GetMethod("TP_Holy2_WeaponSupportRosaryDamage"),
            typeof(EME_WaveWeapon).GetMethod("RaptureDamage"),
            typeof(SleinHarmony).GetMethod("EME_WaveWeaponRaptureDamage"),
            typeof(WorldEaterVFX).GetMethod("DoSoulSteal"),
            typeof(SleinHarmony).GetMethod("WorldEaterVFXDoSoulSteal"),
            
        };

        var postfixes = new List<MethodInfo>
        {
            typeof(TP_SonicWhip1_Weapon).GetMethod("OnBulletOverlapsEnemy"),
            typeof(SleinHarmony).GetMethod("TP_SonicWhip1_WeaponOnBulletOverlapsEnemy"),
            typeof(TP_SonicWhip1_Weapon).GetMethod("IsInstaKill"),
            typeof(SleinHarmony).GetMethod("TP_SonicWhip1_WeaponIsInstaKill"),
            typeof(TP_Gun2_Weapon).GetMethod("ShowBigDamage"),
            typeof(SleinHarmony).GetMethod("TP_Gun2_WeaponShowBigDamage"),
            typeof(TP_Frog2_TongueProjectile).GetMethod("MorphTargetEnemy"),
            typeof(SleinHarmony).GetMethod("TP_Frog2_TongueProjectileMorphTargetEnemy"),
        };
        HarmonyPatching.Patch(typeof(SleinHarmony), prefixes, postfixes);
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
        
        public static void SireProjectileEraseEnemies(SireProjectile __instance)
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
        
        public static void SireProjectileEraseRandomEnemy(SireProjectile __instance, SfxType sfx, int index, int detune, float offset)
        {
            if (_characterController == null) return;
            EnemyController enemyController = __instance._weapon.GameMan.Stage.FindClosestEnemy(__instance._playerCachedTransform.position, excludeDead: true);
            var resRosary = enemyController.ResRosary;
            if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
            {
                _characterController._playerStats.Power._val += 0.001f;
            }
        }
        
        public static void TP_SpiritTornado2_ProjectileEraseEnemies(TP_SpiritTornado2_Projectile __instance, bool makeHearts)
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
        
        public static void TP_SpiritTornado2_ProjectileEraseRandomEnemies(TP_SpiritTornado2_Projectile __instance, SfxType sfx, int index, int detune, float offset, bool scaleVenus)
        {
            if (_characterController == null) return;
            var closestEnemiesSorted = GM.Core.Stage.GetClosestEnemiesSorted(__instance._weapon.Owner.position.ToVector3(), excludeDead: true);
            foreach (var enemyController in closestEnemiesSorted)
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
        
        public static void LEM_Inferno2_WeaponDoCoinRosary(LEM_Inferno2_Weapon __instance)
        {
            if (_characterController == null) return;

            foreach (var enemyController in __instance.GetAllEnemiesOnScreen())
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue && !(resRosary.GetValueOrDefault() > 0f) && !enemyController.IsBoss)
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
        
        public static void TP_PowerOfLire_ProjectileTransformEnemies(TP_PowerOfLire_Projectile __instance, bool erase)
        {
            if (_characterController == null) return;

            foreach (var enemyController in __instance._weapon.GameMan.Stage.GetAllEnemiesInScreenBounds())
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue && !(resRosary.GetValueOrDefault() > 0f) && !enemyController.IsBoss)
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
        
        public static void Report2WeaponEraseEnemyType(Report2Weapon __instance, EnemyType type)
        {
            if (_characterController == null) return;

            foreach (var enemyController in GM.Core.Stage.SpawnedEnemies)
            {
                if(enemyController != null && enemyController.EnemyType == type)
                    _characterController._playerStats.Power._val += 0.001f;
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
        
        public static void SantaJavelin2WeaponWSPDamage(SantaJavelin2Weapon __instance)
        {
            if (_characterController == null) return;

            foreach (var enemyController in GM.Core.Stage.SpawnedEnemies)
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
        
        public static void TP_Holy2_WeaponSupportRosaryDamage(SantaJavelin2Weapon __instance)
        {
            if (_characterController == null) return;

            foreach (var enemyController in GM.Core.Stage.SpawnedEnemies)
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
        
        public static void EME_WaveWeaponRaptureDamage(EME_WaveWeapon __instance, EnemyController enemy, bool risky = true)
        {
            if (_characterController == null) return;

            var resRosary = enemy.ResRosary;
            if ((!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f)) && !enemy._hasATreasure)
            {
                _characterController._playerStats.Power._val += 0.001f;
            }
        }
        
        public static void TP_Frog2_TongueProjectileMorphTargetEnemy(TP_Frog2_TongueProjectile __instance)
        {
            if (_characterController == null) return;

            _characterController._playerStats.Power._val += 0.001f;
        }
        
        public static void WorldEaterVFXDoSoulSteal(WorldEaterVFX __instance, bool isCursed)
        {
            if (_characterController == null) return;

            foreach (var enemyController in GM.Core.Stage.GetAllEnemiesInScreenBounds())
            {
                var resRosary = enemyController.ResRosary;
                if (!resRosary.HasValue || !(resRosary.GetValueOrDefault() > 0f))
                {
                    _characterController._playerStats.Power._val += 0.001f;
                }
            }
        }
        
        
        private static bool _isInstaKill = false;
        
        public static void TP_SonicWhip1_WeaponOnBulletOverlapsEnemy(TP_SonicWhip1_Weapon __instance, CallbackContext context, ArcadeColliderType second, ArcadeColliderType first)
        {
            if (_characterController == null) return;

            var component = first.gameObject.GetComponent<EnemyController>();
            if (second.gameObject.GetComponent<Projectile>().HasAlreadyHitObject(component.Cast<IDamageable>())) return;
            if (!_isInstaKill || component.ResRosary.Value != 0f) return;
            
            _characterController._playerStats.Power._val += 0.001f;
            _isInstaKill = false;
        }
        
        public static void TP_SonicWhip1_WeaponIsInstaKill(TP_SonicWhip1_Weapon __instance, ref bool __result)
        {
            if (_characterController == null) return;

            _isInstaKill = __result;
        }
        
        public static void TP_Gun2_WeaponShowBigDamage(TP_Gun2_Weapon __instance, float value, Vector3 position)
        {
            if (_characterController == null) return;

            _characterController._playerStats.Power._val += 0.001f;
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
        PortraitName = "p_slein.png";
        Description = "Loves pentagrams. Gains +0.1% Might for every banished enemy.";
        StartingWeapon = WeaponType.PENTAGRAM;
        MaxHp = 666;
        Amount = 5;
        Magnet += 1.5f;
        Curse += 0.5f;
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