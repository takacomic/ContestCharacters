using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2Cpp;
using Newtonsoft.Json.Linq;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.TimerSystem;
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
    private static CharacterType? _characterType = null;
    private static CharacterController? _characterController;
    private static List<Projectile> _projectiles;
    private static bool _retalRecovery;
    private static EnemyController _enemyController;
    public override void AfterFullInit(CharacterController instance)
    {
        retal = null;
        _projectiles = new List<Projectile>();
        _retalRecovery = false;
        _characterType = instance._characterType;
        _characterController = instance;
        var postfixes = new List<MethodInfo>
        {
            typeof(Weapon).GetMethod("StandardCritical"),
            typeof(AshnardHarmony).GetMethod("WeaponStandardCritical"),
            typeof(EnemyController).GetMethod("GetDamaged"),
            typeof(AshnardHarmony).GetMethod("EnemyControllerGetDamaged")
        };
        HarmonyPatching.Patch(typeof(AshnardHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(AshnardHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    public override void EnemyOnPlayerOverlap(EnemyController instance, CharacterController player)
    {
        if (retal is { IsCompleted: true })
            retal = Timers.Register(1f, null ,new Action<float>(KnifeRetal));
    }

    private void KnifeRetal(float time)
    {
        var pos = _characterController._weaponsManager.GetWeaponByType(WeaponType.KNIFE).PlayerPos;
        var transform = _characterController._weaponsManager.GetWeaponByType(WeaponType.KNIFE)._targetTransform;
        _projectiles.Add(_characterController._weaponsManager.GetWeaponByType(WeaponType.KNIFE).FireOneProjectile(pos, 0, transform));
        if (_characterController.PAmount() <= 1f) return;
        for (var i = 1; i < _characterController.PAmount(); i++)
        {
            _projectiles.Add(_characterController._weaponsManager.GetWeaponByType(WeaponType.KNIFE).FireOneProjectile(pos, i, transform));
        }
    }
    
    private static class AshnardHarmony
    {
        public static void WeaponStandardCritical(Weapon __instance, ArcadeColliderType second, ArcadeColliderType first)
        {
            if (__instance.Owner._characterType != _characterController?._characterType) return;
            
            if (__instance.Type != WeaponType.KNIFE) return;
            
            var projectile = second.TryCast<Projectile>();
            var enemy = first.TryCast<EnemyController>();
            if (projectile == null) return;
            if (enemy == null) return;
                
            if (!_projectiles.Contains(projectile)) return;
            _retalRecovery = true;
            _enemyController = enemy;
        }
    
        public static void EnemyControllerGetDamaged(EnemyController __instance, float value, HitVfxType showHitVfx, float damageKb, WeaponType damageType, bool hasKb)
        {
            if (_characterController == null) return;
            if (!_retalRecovery || _enemyController != __instance) return;
            var tenHp = _characterController.MaxHp() * 0.1f;
            var tenDmg = value * 0.1f;
                
            _characterController?.RecoverHp(tenHp < tenDmg ? tenHp : tenDmg, true, true);
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
        Description = "A contract hunter from a foreign land far to the south. Ashnard Brenen has been contracted to travel here in search of one thing: Vampires";
        StartingWeapon = WeaponType.KNIFE;
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