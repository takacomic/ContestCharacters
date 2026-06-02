using CoffinTech.SaveData;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class AshnardUnlock
{
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterAshnardUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterUsuiUnlock").Value)) return;
        if (GM.Core.Player._characterType != CharacterType.NEO) return;
        
        if (GM.Core.SurvivedSeconds < 1800) return;
        if (!GM.Core.ArcanaManager.ActiveArcanas.Contains(ArcanaType.D05_PALE_DIAMOND)) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterAshnardUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterAshnard").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
}