using CoffinTech.Extensions;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class RubriccoUnlock
{

    private static bool foundTurbo;
    
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterRubriccoUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterEnzoUnlock").Value)) return;
        if (__instance.CharacterType != CharacterType.PEPPINO) return;
        if (!GM.Core.PlayerOptions.Config.RunWeapons.Contains(WeaponType.CANDYBOX2)) return;
        //if (!foundTurbo) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterRubriccoUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterRubricco").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
    
    //[HarmonyPatch(typeof(OpenTreasurePage), nameof(OpenTreasurePage.OnShowStart))]
}