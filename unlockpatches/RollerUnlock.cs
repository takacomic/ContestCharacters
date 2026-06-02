using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Interfaces;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Characters.Enemies;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.UI;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(EnemySketamari))]
public class RollerUnlock
{
    /*private static int _rollerCount;
    private static bool ready;
    private static List<CharacterType> characters = new () {CharacterType.EX_CHULAREH, CharacterType.EX_GIOCARE};
    [HarmonyPatch(nameof(CharacterController.AfterFullInitialization))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        ready = false;
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterRollerUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSpecimenUnlock").Value)) return;
        if (!characters.Contains(__instance.CharacterType)) return;
        ready = true;
        _rollerCount = 0;
    }
    
    [HarmonyPatch(typeof(LevelUpPage), nameof(LevelUpPage.ResetLevelUpViewsAfterReRoll))]
    [HarmonyPostfix]
    public static void Postfix(LevelUpPage __instance)
    {
        _rollerCount++;
        if (_rollerCount < 25 || !ready) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterRollerUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterRoller").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }*/
    
    private static bool _ready;
    private static bool _retalDeath;
    private static float _retalDamage;
    private static EnemySketamari? _sketamari;
    [HarmonyPatch(nameof(EnemySketamari.InitEnemy))]
    [HarmonyPostfix]
    public static void Postfix(EnemySketamari __instance)
    {
        _ready = false;
        _retalDeath = false;
        _sketamari = null;
        _retalDamage = 0;
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterRollerUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSpecimenUnlock").Value)) return;

        _ready = true;
        _sketamari = __instance;
    }
    
    [HarmonyPatch(typeof(Weapon), nameof(Weapon.DealDamageRetaliation))]
    [HarmonyPostfix]
    private static void DealDamageRetaliationPostfix(Weapon __instance, IDamageable other)
    {
        float num = ((__instance.Owner.PArmor() > 0f) ? (1f + __instance.Owner.PArmor() * 0.1f) : 1f);
        float damage = __instance.PPower() * num;
        _retalDamage = damage;
    }

    [HarmonyPatch(typeof(EnemyController), nameof(EnemyController.GetDamaged))]
    [HarmonyPrefix]
    private static void GetDamagedPrefix(EnemyController __instance, float value, HitVfxType showHitVfx, float damageKb, WeaponType damageType, bool hasKb)
    {
        if (!_ready) return;
        if (__instance.TryCast<EnemySketamari>() != _sketamari) return;
        if (value == _retalDamage) return;
        if (__instance._hp - value <= 0)
            _retalDeath = true;
    }

    [HarmonyPatch(nameof(EnemySketamari.Die))]
    [HarmonyPostfix]
    private static void DiePostfix(EnemySketamari __instance)
    {
        if (!_ready) return;
        if (!_retalDeath) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterRollerUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterRoller").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}