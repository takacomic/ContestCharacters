using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Stages;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(Background2))]
public class VinzonUnlock
{
    private static List<WeaponType> _weapons = new ()
    {
        WeaponType.HOLY_MISSILE,
        WeaponType.TP_SUMMON_SPIRIT2,
        WeaponType.HELLFIRE,
        WeaponType.TP_AURABLAST_WEAPON2,
        WeaponType.LOOP,
        WeaponType.TP_SWORD_BROTHERS2,
        WeaponType.MISSPELL2,
        WeaponType.ICELANCE2,
        WeaponType.FOLLOWER_KNIFE2
    };
    
    [HarmonyPatch(nameof(Background2.HandleTricksterDefeat))]
    [HarmonyPostfix]
    public static void HandleTricksterDefeat()
    {
        var weaponCount = _weapons.Count(weaponType => GM.Core._playerOptions.Config.RunWeapons.Contains(weaponType));
        
        if (weaponCount < 3) return;
        
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterVinzonUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterVinzon").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}