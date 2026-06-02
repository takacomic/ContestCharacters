using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class DuoUnlock
{
    private static bool _unlocked;
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterDuoUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterMortisUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterLuigiUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterBaronUnlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.COOP) return;
        if (__instance._characterType != ModOptionsData.CustomCharacter("ContestCharacterMortis").Value) return;
        
        var i = 0;
        if (_unlocked) return;
        foreach (var item in GM.Core.PlayerOptions.Config.RunPickups)
        {
            if (item != ItemType.ROAST) continue;
            i++;
        }
        if (i < 50) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterDuoUnlock").Value)) return;
        _unlocked = true;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterLuigiMortis").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}