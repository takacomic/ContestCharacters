using Il2CppVampireSurvivors.UI;
using MelonLoader;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(CharacterItem))]
public class CharacterItemPatches
{
    [HarmonyPatch(nameof(CharacterItem.RefreshUnlockState))]
    [HarmonyPrefix]
    public static void Prefix(CharacterItem __instance)
    {
        if (!__instance.IsCharacterAlwaysHidden() || !ModOptionsData.TryGetCustomCharacter(null, __instance.CharacterType, out var customCharacter)) return;
        var key = customCharacter.Key;
        if (customCharacter.Key.Equals("ContestCharacterLuigiMortis")) key = "ContestCharacterDuo";
        if (__instance._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret(key + "Unlock").Value))
        {
            __instance._characterData.alwaysHidden = false;
        }
    }
    
    /*[HarmonyPatch(nameof(CharacterItem.HasCharacterRequirementsOrUnlock))]
    [HarmonyPostfix]
    public static void HasCharacterRequirementsOrUnlockPostfix(CharacterItem __instance, bool __result)
    {
        MelonLogger.Msg("Custom Character: " + __instance.CharacterType + " : Unlock: " + __instance.HasCharacterRequirements());
        if (ModOptionsData.TryGetCustomCharacter(null, __instance.CharacterType, out var customCharacter))
        {
            MelonLogger.Msg("Custom Character: " + customCharacter.Key + " : Unlock: " + __instance.HasCharacterRequirements());
            
            __result = __instance.HasCharacterRequirements();
        }

    }*/
}