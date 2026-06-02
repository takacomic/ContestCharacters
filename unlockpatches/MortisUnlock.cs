using CoffinTech.SaveData;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Stages;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(BackgroundRash))]
public class MortisUnlock
{
    private static EnemyController? _enemyController = null;
    private static int _minute = 0;
    
    [HarmonyPatch(nameof(BackgroundRash.CheckMinute))]
    [HarmonyPostfix]
    private static void CreatePostfix(BackgroundRash __instance, int minute)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterMortisUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterGuillotinaUnlock").Value)) return;
        _minute = minute;
    }

    [HarmonyPatch(nameof(BackgroundRash.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdatePostfix(BackgroundRash __instance)
    {
        if (_minute < 15) return;
        if (_enemyController == null)
        {
            foreach (var enemy in GM.Core.Enemies.children)
            {
                var enemyController = enemy.Cast<EnemyController>();
                if (enemyController == null) continue;
                if (enemyController._enemyType == EnemyType.BOSS_XLDEATH)
                    _enemyController = enemyController;
                break;
            }
        }
        else
        {
            if (!_enemyController.IsDead) return;
            if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterMortisUnlock").Value)) return;
            var characterType = ModOptionsData.CustomCharacter("ContestCharacterMortis").Value;
            InGameUnlocks.InGameSecretCharacterUnlock(characterType);
        }
    }
}