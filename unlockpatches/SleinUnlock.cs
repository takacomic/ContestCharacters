using CoffinTech.Extensions;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Stages;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(BackgroundDevilRoom))]
public class SleinUnlock
{
    private static List<WeaponType> _weaponTypes = new () { WeaponType.PENTAGRAM, WeaponType.SIRE, WeaponType.TP_SPIRITTORNADO2 };
    [HarmonyPatch(nameof(BackgroundDevilRoom.AdvanceLevel))]
    [HarmonyPostfix]
    public static void Postfix(BackgroundDevilRoom __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSleinUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterBetaUnlock").Value)) return;
        if (!GM.Core.PlayerOne.WeaponsManager.ActiveEquipment.ToList().Any(e => _weaponTypes.Contains(e._equipmentType))) return;
        
        if (__instance.currentLevel != 15) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterSleinUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterSlein").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}