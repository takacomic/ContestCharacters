using System.Reflection;
using CoffinTech.Extensions;
using ContestCharacters.components;
using Il2Cpp;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.Geom;
using Il2CppVampireSurvivors.Framework.Particles;
using Il2CppVampireSurvivors.Framework.TimerSystem;
using Il2CppVampireSurvivors.Interfaces;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Pools;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using UnityEngine;
using Timer = Il2CppVampireSurvivors.Framework.TimerSystem.Timer;

namespace ContestCharacters.characters.secret;

public class RatController : ModCharacterController
{
    private static CharacterController? _characterController;
    private static List<WeaponType> weapons = new List<WeaponType> {
	    WeaponType.BORA, WeaponType.HOLYWATER
    };
    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        instance._hasAnyCriticalHPSkill = true;
        instance._onCriticalHP = new Action(CritHealth);
        
        var prefixes = new List<MethodInfo>
        {
	        typeof(HolyWaterProjectile).GetMethod("InitProjectile"),
	        typeof(RatHarmony).GetMethod("HolyWaterProjectileInitProjectile"),
	        typeof(BoraProjectile).GetMethod("InitProjectile"),
	        typeof(RatHarmony).GetMethod("BoraProjectileInitProjectile"),
	        typeof(EnemyController).GetMethod("GetDamaged"),
	        typeof(RatHarmony).GetMethod("EnemyControllerGetDamaged"),
        };
        var postfixes = new List<MethodInfo>
        {
	        typeof(Weapon).GetMethod("OnBulletOverlapsEnemy"),
	        typeof(RatHarmony).GetMethod("WeaponOnBulletOverlapsEnemy"),
        };
        HarmonyPatching.Patch(typeof(RatHarmony),prefixes, postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private void CritHealth()
    {
	    if (_characterController.WeaponsManager.ActiveEquipment.ToList().Find(w => w.Type == WeaponType.BORA))
	    {
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).EnableAdept();
		    Timers.Register(10, new Action(delegate
		    {
			    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).IsAdept = false;
			    _characterController.WeaponsManager.GetWeaponByType(WeaponType.BORA).Interval = _characterController
				    .WeaponsManager.GetWeaponByType(WeaponType.BORA)._currentWeaponData.interval * 2f;
		    }));
	    }
	    if (_characterController.WeaponsManager.ActiveEquipment.ToList().Find(w => w.Type == WeaponType.HOLYWATER))
	    {
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).Fire();
		    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).EnableAdept();
		    Timers.Register(10, new Action(delegate
		    {
			    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).IsAdept = false;
			    _characterController.WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER).Interval = _characterController
				    .WeaponsManager.GetWeaponByType(WeaponType.HOLYWATER)._currentWeaponData.interval * 2f;
		    }));
	    }
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(RatHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private static class RatHarmony
    {
        private static bool _ready = false;
        private static List<WeaponType> weapons = new List<WeaponType> {
	        WeaponType.BORA, WeaponType.HOLYWATER
        };
        
        public static void EnemyControllerGetDamaged(EnemyController __instance, float value, HitVfxType showHitVfx, float damageKb, WeaponType damageType, bool hasKb)
        {
	        if (__instance.Hp - value > 0) return;
	        var poison = __instance.gameObject.GetComponent<PoisonComponent>();
	        if (poison != null) poison.Remove();
        }
        
        public static void WeaponOnBulletOverlapsEnemy(Weapon __instance, CallbackContext context, ArcadeColliderType second, ArcadeColliderType first)
        {
	        if (__instance.Owner != _characterController) return;
	        if (!weapons.Contains(__instance.Type)) return;
	        EnemyController component = first.gameObject.GetComponent<EnemyController>();
	        if (component == null) return;
	        if (component.IsDead) return;
	        
	        //if (!second.gameObject.GetComponent<Projectile>().HasAlreadyHitObject(component.Cast<IDamageable>())) return;
	        
	        if (component.gameObject.GetComponent<PoisonComponent>() != null) return;

	        component.gameObject.AddComponent<PoisonComponent>().Initialize(component, _characterController.PCurse());
        }
        
        public static void HolyWaterProjectileInitProjectile(HolyWaterProjectile __instance)
		{
			__instance._GroundFx.color = new UnityEngine.Color(0.0f, 1.0f, 0.0f, 0.2f);
			
			__instance._explosionCircle = new Circle(0f, 0f, 8f);
			Il2CppSystem.Collections.Generic.List<string> projectiles = new ();
			projectiles.Add("ProjectileGreen1");
			projectiles.Add("ProjectileFlameGreen");
			ParticleSystemConfig config = new ParticleSystemConfig("vfx")
			{
				_frame = projectiles,
				_speed = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 1f)),
				_quantity = new Il2CppSystem.Nullable<int>(1),
				_rotate = new ParticleSystem.MinMaxCurve(90f, 90f),
				_lifespan = 600f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 1f)),
				_emitZone = new EmitZone
				{
					_type = EmitZoneType.Random,
					_source = __instance._explosionCircle
				},
				_on = false
			};
			__instance._pfx1 = ParticleSystemGenerator.GenerateParticleSystem(config, __instance._cachedTransform, "PfxEmitter1");
			ParticleSystemConfig config2 = new ParticleSystemConfig("vfx")
			{
				_frame = projectiles,
				_speed = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 1f)),
				_quantity = new Il2CppSystem.Nullable<int>(1),
				_rotate = new ParticleSystem.MinMaxCurve(90f, 90f),
				_lifespan = 600f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.2f, 0f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 0.5f)),
				_blendMode = new Il2CppSystem.Nullable<BlendMode>(BlendMode.Add),
				_emitZone = new EmitZone
				{
					_type = EmitZoneType.Random,
					_source = __instance._explosionCircle
				},
				_on = false
			};
			__instance._pfx2 = ParticleSystemGenerator.GenerateParticleSystem(config2, __instance._cachedTransform, "PfxEmitter2");
			Il2CppSystem.Collections.Generic.List<string> projectile1 = new ();
			projectile1.Add("HitGreen2");
			Il2CppSystem.Collections.Generic.List<string> projectile2 = new ();
			projectile2.Add("HitGreen1");
			ParticleSystemConfig config3 = new ParticleSystemConfig("vfx")
			{
				_frame = projectile1,
				_quantity = new Il2CppSystem.Nullable<int>(1),
				_rotate = new ParticleSystem.MinMaxCurve(0f, 360f),
				_lifespan = 150f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0.5f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 1f)),
				_on = false
			};
			__instance._explosionPfx1 = ParticleSystemGenerator.GenerateParticleSystem(config3, __instance._cachedTransform, "ExplosionPfx1");
			ParticleSystemConfig config4 = new ParticleSystemConfig("vfx")
			{
				_frame = projectile2,
				_quantity = new Il2CppSystem.Nullable<int>(3),
				_rotate = new ParticleSystem.MinMaxCurve(0f, 360f),
				_lifespan = 150f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0.5f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 1f)),
				_on = false
			};
			__instance._explosionPfx2 = ParticleSystemGenerator.GenerateParticleSystem(config4, __instance._cachedTransform, "ExplosionPfx2");
		}
        
        public static void BoraProjectileInitProjectile(BoraProjectile __instance)
		{
			__instance._GroundFx._spriteRenderer.color = new UnityEngine.Color(0.0f, 1.0f, 0.0f, 0.2f);
			
			__instance._explosionCircle = new Circle(0f, 0f, 8f);
			Il2CppSystem.Collections.Generic.List<string> projectiles = new ();
			projectiles.Add("ProjectileGreen1");
			projectiles.Add("ProjectileFlameGreen");
			ParticleSystemConfig config = new ParticleSystemConfig("vfx")
			{
				_frame = projectiles,
				_speed = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 1f)),
				_quantity = new Il2CppSystem.Nullable<int>(1),
				_rotate = new ParticleSystem.MinMaxCurve(90f, 90f),
				_lifespan = 600f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 1f)),
				_emitZone = new EmitZone
				{
					_type = EmitZoneType.Random,
					_source = __instance._explosionCircle
				},
				_on = false
			};
			__instance._pfxEmitter1 = ParticleSystemGenerator.GenerateParticleSystem(config, __instance._cachedTransform, "PfxEmitter1");
			ParticleSystemConfig config2 = new ParticleSystemConfig("vfx")
			{
				_frame = projectiles,
				_speed = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 1f)),
				_quantity = new Il2CppSystem.Nullable<int>(1),
				_rotate = new ParticleSystem.MinMaxCurve(90f, 90f),
				_lifespan = 600f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.2f, 0f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 0.5f)),
				_blendMode = new Il2CppSystem.Nullable<BlendMode>(BlendMode.Add),
				_emitZone = new EmitZone
				{
					_type = EmitZoneType.Random,
					_source = __instance._explosionCircle
				},
				_on = false
			};
			__instance._pfxEmitter1 = ParticleSystemGenerator.GenerateParticleSystem(config2, __instance._cachedTransform, "PfxEmitter2");
			Il2CppSystem.Collections.Generic.List<string> projectile1 = new ();
			projectile1.Add("HitGreen2");
			Il2CppSystem.Collections.Generic.List<string> projectile2 = new ();
			projectile2.Add("HitGreen1");
			var pfxSystems = __instance._pfxEmitterExplosionManager._particleSystems.ToList();
			foreach (var pfx in pfxSystems)
			{
				__instance._pfxEmitterExplosionManager.RemoveEmitter(pfx);
			}
			__instance._pfxEmitterExplosionManager.CreateEmitter(new ParticleSystemConfig("vfx")
			{
				_frame = projectile1,
				_quantity = new Il2CppSystem.Nullable<int>(1),
				_rotate = new ParticleSystem.MinMaxCurve(0f, 360f),
				_lifespan = 150f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0.5f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 1f)),
				_on = false
			});
			__instance._pfxEmitterExplosionManager.CreateEmitter(new ParticleSystemConfig("vfx")
			{
				_frame = projectile2,
				_quantity = new Il2CppSystem.Nullable<int>(3),
				_rotate = new ParticleSystem.MinMaxCurve(0f, 360f),
				_lifespan = 150f,
				_alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0.5f)),
				_scale = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.25f, 1f)),
				_on = false
			});
		}
    }
}