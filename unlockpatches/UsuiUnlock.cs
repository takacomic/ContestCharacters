using CoffinTech.SaveData;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Stages;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(BackgroundWhiteout))]
public class UsuiUnlock
{
    private static float _totalTime;
    private static PickupCoffin pickupCoffin;

    [HarmonyPatch(nameof(BackgroundWhiteout.Create))]
    [HarmonyPostfix]
    private static void CreatePostfix(BackgroundWhiteout __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        ModOptionsData.TryGetCustomCharacter("ContestCharacterUsui", null, out var character);
        var characterType = character.Value;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterUsuiUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterZetaUnlock").Value)) return;
        
        Vector2 spawnPos = new Vector2 { x = -354.1103f, y = 397.0696f };
        pickupCoffin = GM.Core.MakeStagePickup(spawnPos, ItemType.COFFIN).Cast<PickupCoffin>();
        if (pickupCoffin == null) return;
        pickupCoffin.SetChar(characterType);
        pickupCoffin.SetEnemySpawnType(EnemyType.PATROL_WITCH, 50);
        pickupCoffin.SpawnAngle = 360f * (MathF.PI / 180f);
        pickupCoffin.OnGotTaken = new Action(delegate
        {
            GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterUsuiUnlock").Value);
            InGameUnlocks.InGameSecretCharacterUnlock(characterType);
        });
    }
}