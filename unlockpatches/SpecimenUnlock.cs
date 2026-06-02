using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Stages;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(BackgroundLaborratory))]
public class SpecimenUnlock
{
    private static TilingTileset.TeleporterData teleport;
    private static float _totalTime;
    private static PickupCoffin pickupCoffin;

    [HarmonyPatch(nameof(BackgroundLaborratory.Create))]
    [HarmonyPostfix]
    private static void CreatePostfix(Background6 __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSpecimenUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSleinUnlock").Value)) return;

        var characterType = ModOptionsData.CustomCharacter("ContestCharacterSpecimen").Value;

        Vector2 spawnPos = new Vector2 { x = 20.64f, y = -63.9f };
        pickupCoffin = GM.Core.MakeStagePickup(spawnPos, ItemType.COFFIN).Cast<PickupCoffin>();
        if (!(pickupCoffin == null))
        {
            pickupCoffin.SetChar(characterType);
            pickupCoffin.OnGotTaken = new Action(delegate
            {
                GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterSpecimenUnlock").Value);
                InGameUnlocks.InGameSecretCharacterUnlock(characterType);
            });
        }
    }
}