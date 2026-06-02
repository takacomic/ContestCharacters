using CoffinTech.SaveData;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Characters.Enemies;
using Il2CppVampireSurvivors.Objects.Stages;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class SirBoneUnlock
{
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSirBoneUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterMortisUnlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.BONEZONE) return;
        if (!GM.Core.PlayerOptions.Config.RunWeapons.Contains(WeaponType.BONE2)) return;
        
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterSirBoneUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterSirBone").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
    
    //Sadge
    /*private static bool _empowered;
    private static float lastHp = -1;
    [HarmonyPatch(nameof(EnemySketamari.InitEnemy))]
    [HarmonyPostfix]
    public static void Postfix(EnemySketamari __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterSirBoneUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterMortisUnlock").Value)) return;

        __instance._maxHp = 35000;
        __instance._hp = 35000;
        __instance._defaultSpeed = 500;
        __instance.Speed = 500;
        _empowered = true;
        
    }

    [HarmonyPatch(nameof(EnemySketamari.Die))]
    [HarmonyPostfix]
    private static void DiePostfix(EnemySketamari __instance)
    {
        if (!_empowered) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterSirBoneUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterSirBone").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }*/
}