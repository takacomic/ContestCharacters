using HarmonyLib;
using Il2CppVampireSurvivors.Objects;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(YellowSignManager))]
public static class YellowSignManagerPatches
{
    [HarmonyPatch(nameof(YellowSignManager.DoClaps))]
    [HarmonyPrefix]
    private static bool UnlockWeaponsPrefix(YellowSignManager __instance)
    {
        return !ItemFoundPagePatches._diskFound;
    }
}