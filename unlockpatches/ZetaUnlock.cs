using CoffinTech.SaveData;
using CoffinTech.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Objects.Stages;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(Background6))]
public static class ZetaUnlock
{
    private static TilingTileset.TeleporterData teleport;
    private static float _totalTime;
    private static PickupCoffin pickupCoffin;
    private static PickupTeleporter pickupTeleporter;
    private static PickupTeleporter pickupTeleporter2;
    
    [HarmonyPatch(nameof(Background6.Create))]
    [HarmonyPostfix]
    private static void CreatePostfix(Background6 __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterZetaUnlock").Value)) return;
        if (GM.Core.Player._characterType != CharacterType.AVATAR) return;
        
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterZeta").Value;
        
        Vector2 spawnPos = new Vector2 { x = -10.4134f, y = -5.8222f };
        pickupCoffin = GM.Core.MakeStagePickup(spawnPos, ItemType.COFFIN).Cast<PickupCoffin>();
        if (!(pickupCoffin == null))
        {
            pickupCoffin.SetChar(characterType);
            pickupCoffin.OnGotTaken = new Action(delegate
            {
                GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterZetaUnlock").Value);
                InGameUnlocks.InGameSecretCharacterUnlock(characterType);
            });
        }

        teleport = new();
        teleport.TeleportKey = "ZetaTeleporter";
        teleport.A = new Vector2 { x = -6.5757f, y = -5.3f };
        teleport.B = new Vector2 { x = 6.2727f, y = -5.3f };
        
        pickupTeleporter = GM.Core.MakePickup(teleport.A, ItemType.TELEPORTER).Cast<PickupTeleporter>();
        pickupTeleporter2 = GM.Core.MakePickup(teleport.B, ItemType.TELEPORTER).Cast<PickupTeleporter>();
        pickupTeleporter.SetTeleportKey(teleport.TeleportKey);
        pickupTeleporter2.SetTeleportKey(teleport.TeleportKey);
        pickupTeleporter.LinkTo(pickupTeleporter2);
        pickupTeleporter2.LinkTo(pickupTeleporter);
    }

    [HarmonyPatch(nameof(Background6.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdatePostfix(Background6 __instance)
    {
        if (pickupTeleporter == null) return;
        _totalTime += PauseSystem.DeltaTime;
        if (_totalTime > 0.040000003f)
        {
            _totalTime = 0f;
            GM.Core.SpawnPickupEffectsParticles(teleport.A);
            GM.Core.SpawnPickupEffectsParticles(teleport.B);
        }
    }

    [HarmonyPatch(nameof(Background6.SwapDirecters))]
    [HarmonyPostfix]
    private static void SwapDirectersPostfix(Background6 __instance)
    {
        
        if (pickupTeleporter == null) return;
        pickupTeleporter.SetTeleportKey("null");
        pickupTeleporter.CanTeleport = false;
        pickupTeleporter2.CanTeleport = false;
        pickupCoffin.Despawn();
        pickupCoffin.RemoveCursor();
        pickupTeleporter.ActuallyDespawn();
        pickupTeleporter2.ActuallyDespawn();
        PickupManager.ReturnPickup(pickupCoffin);
        PickupManager.ReturnPickup(pickupTeleporter);
        PickupManager.ReturnPickup(pickupTeleporter2);
        teleport = null;
        pickupTeleporter = null;
        pickupTeleporter2 = null;
        
    }
}