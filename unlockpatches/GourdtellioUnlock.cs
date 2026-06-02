using Il2CppVampireSurvivors.Objects.Characters;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class GourdtellioUnlock
{
    public static bool _gourdtellioUnlockReady;
    [HarmonyPatch(nameof(CharacterController.AfterFullInitialization))]
    [HarmonyPrefix]
    private static void Prefix(CharacterController __instance)
    {
        _gourdtellioUnlockReady = false;
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterGourdtellioUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterRollerUnlock").Value)) return;
        _gourdtellioUnlockReady = true;
    }
}