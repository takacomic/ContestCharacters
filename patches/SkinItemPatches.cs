using Il2CppVampireSurvivors.UI;
using MelonLoader;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(SkinItem))]
public class SkinItemPatches
{
    [HarmonyPatch(nameof(SkinItem.RefreshUnlockState))]
    [HarmonyPrefix]
    public static void Prefix(SkinItem __instance)
    {
        if (!__instance.IsSkinAlwaysHidden() || !ModOptionsData.TryGetCustomCharacter(null, __instance.CharacterType, out var customCharacter)) return;
        if (!customCharacter.Key.Equals("ContestCharacterPiuma")) return;
        if (__instance._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret(customCharacter.Key + "SkinUnlock").Value))
        {
            __instance._skinData.alwaysHidden = false;
        }
    }
}