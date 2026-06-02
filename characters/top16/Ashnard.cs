using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2Cpp;
using Newtonsoft.Json.Linq;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.TimerSystem;
using Il2CppVampireSurvivors.Interfaces;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Timer = Il2CppVampireSurvivors.Framework.TimerSystem.Timer;

namespace ContestCharacters.characters.top16;

public class AshnardController : ModCharacterController
{
    private Timer? retal;
    private static CharacterController? _characterController;
    private static List<Projectile> _projectiles;
    private static List<WeaponType> _weapons = new () { WeaponType.KNIFE, WeaponType.THOUSAND, WeaponType.FLASHARROW2 };
    private static bool _retalRecovery;
    private static EnemyController _enemyController;
    private static bool _retaled;
    public override void AfterFullInit(CharacterController instance)
    {
        retal = null;
        _projectiles = new List<Projectile>();
        _retalRecovery = false;
        _characterController = instance;
        var postfixes = new List<MethodInfo>
        {
            typeof(EnemyController).GetMethod("GetDamaged"),
            typeof(AshnardHarmony).GetMethod("EnemyControllerGetDamaged"),
            typeof(Weapon).GetMethod("DealDamageRetaliation"),
            typeof(AshnardHarmony).GetMethod("WeaponDealDamageRetaliation")
        };
        HarmonyPatching.Patch(typeof(AshnardHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(AshnardHarmony));
        _projectiles.Clear();
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    public override void EnemyOnPlayerOverlap(EnemyController instance, CharacterController player)
    {
        if (retal == null)
        {
            retal = Timers.Register(2f, new Action(delegate { retal = null;
                _retaled = false;
            }), new Action<float>(KnifeRetal));
        }
    }

    private void KnifeRetal(float time)
    {
        if (time > 1f) return;
        if (_retaled) return;
        if (_characterController == null) return;
        foreach ( var weapon in _weapons)
        {
            var weapon2 = _characterController._weaponsManager.GetWeaponByType(weapon);
            if (weapon2 == null) continue;
            var pos = _characterController._weaponsManager.GetWeaponByType(weapon).PlayerPos;
            var transform = _characterController._weaponsManager.GetWeaponByType(weapon)._targetTransform;
            _projectiles.Add(_characterController._weaponsManager.GetWeaponByType(weapon)
                .FireOneProjectile(pos, 0, transform));
            if (_characterController.PAmount() <= 1f) return;
            for (var i = 1; i < _characterController.PAmount(); i++)
            {
                pos.x += 0.03f;
                _projectiles.Add(_characterController._weaponsManager.GetWeaponByType(weapon)
                    .FireOneProjectile(pos, i, transform));
            }
        }

        _retaled = true;
    }
    
    private static class AshnardHarmony
    {
        public static void WeaponDealDamageRetaliation(Weapon __instance, IDamageable other)
        {
            if (_characterController == null) return;
            if (__instance.Owner != _characterController) return;
            var tenHp = _characterController.MaxHp() * 0.05f;
            _characterController?.RecoverHp(tenHp, true, true);
        }
        
        public static void EnemyControllerGetDamaged(EnemyController __instance, float value, HitVfxType showHitVfx, float damageKb, WeaponType damageType, bool hasKb)
        {
            List<Projectile> projectile2 = new();
            if (_characterController == null) return;
            foreach (var projectile in _projectiles)
            {
                foreach (var enemy in projectile._objectsHit)
                {
                    var en = enemy.TryCast<EnemyController>();
                    var tenHp = _characterController.MaxHp() * 0.05f;
                
                    _characterController?.RecoverHp(tenHp, true, true);
                    projectile2.Add(projectile);
                    break;
                }
                
            }

            projectile2.ForEach(p =>
            {
                _projectiles.Remove(p);
            });
        }
    }
}

public sealed class AshnardStats : BaseCharacterData
{
    public AshnardStats()
    {
        CharName = "Ashnard";
        SurName = "Brenen";
        TextureName = "Ashnard_Brenen_walk";
        SpriteName = "Ashnard_Brenen_walk_01.png";
        PortraitName = "p_ashnard.png";
        Description = "A contract hunter from a foreign land far to the south. Heals 5% of his max HP on retaliation. Retaliates with Knife and Thousand Edge";
        StartingWeapon = WeaponType.KNIFE;
        Power += 0.25f;
        Amount += 1f;
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