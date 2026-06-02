using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class RatUnlock
{
    private static bool _unlocked;
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterRatUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterBaronUnlock").Value)) return;
        if (__instance._characterType != CharacterType.BATSBATSBATS) return;
        if (_unlocked) return;
        
        if (!GM.Core.PlayerOptions.Config.RunWeapons.Contains(WeaponType.GATTI)) return;
        if (!GM.Core.PlayerOptions.Config.RunWeapons.Contains(WeaponType.GATTI_COUNTER)) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterRatUnlock").Value)) return;
        _unlocked = true;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterRat").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}