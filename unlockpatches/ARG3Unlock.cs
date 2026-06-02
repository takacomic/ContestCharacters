using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(RecapPage))]
public class ARG3Unlock
{
    [HarmonyPatch(nameof(RecapPage.OnShowStart))]
    [HarmonyPostfix]
    public static void Postfix(RecapPage __instance)
    {
        if (GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG3Unlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG2Unlock").Value)) return;
        if (__instance._playerOptions.Config.SelectedStage != StageType.MOLISE) return;
        if (Mathf.Floor(GM.Core.SurvivedSeconds) / 60f != 5f) return;
        
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterARG3Unlock").Value);
        
        SoundManager.PlaySound(SfxType.ThingFound, new SoundManager.SoundConfig
        {
            Volume = new Il2CppSystem.Nullable<float>(1),
            Detune = -1000f,
            Rate = 0.5f
        });
    }
}