using Il2CppVampireSurvivors.Framework;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(GoldFeverController))]
public class EnzoUnlock
{
    [HarmonyPatch(nameof(GoldFeverController.OnTick))]
    [HarmonyPostfix]
    public static void UnlockEnzo(GoldFeverController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterEnzoUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSirBoneUnlock").Value)) return;
        if (!__instance._isActive) return;
        if (!GM.Core.HasGfBonus) return;
        
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterEnzoUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterEnzo").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}