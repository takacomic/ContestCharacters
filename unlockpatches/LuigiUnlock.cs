using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class LuigiUnlock
{
    private static bool _unlocked;
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterLuigiUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterPiumaUnlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.COOP) return;
        
        var i = 0;
        if (_unlocked) return;
        foreach (var item in GM.Core.PlayerOptions.Config.RunPickups)
        {
            if (item != ItemType.ROAST) continue;
            i++;
        }
        if (i < 50) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterLuigiUnlock").Value)) return;
        _unlocked = true;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterLuigi").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}