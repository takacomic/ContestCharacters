using CoffinTech.SaveData;
using CoffinTech.Utils;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Stages;
using MelonLoader;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class PiumaUnlock
{
    private static bool _unlocked;
    static List<CharacterType> characters = new () {CharacterType.PEPPINO, CharacterType.TP_KEREMET};
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterPiumaUnlock").Value))
        {
            SkinUnlock(__instance);
            return;
        }
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterAshnardUnlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.CARLOCART) return;
        if (!characters.Contains(__instance.CharacterType) ) return;
        if (GM.Core.SurvivedSeconds < 900) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterPiumaUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterPiuma").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
    }
    
    public static void SkinUnlock(CharacterController __instance)
    {
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterPiumaSkinUnlock").Value)) return;
        var flag = false;
        var flag2 = false;
        var flag3 = false;
        var flag4 = false;
        foreach (var character in GM.Core.AllPlayers)
        {
            switch (character.CharacterType)
            {
                case CharacterType.GYORUNTIN:
                    flag = true;
                    break;
                case CharacterType.EXDASH:
                    flag2 = true;
                    break;
                case CharacterType.PANINI:
                    flag3 = true;
                    break;
                case CharacterType.SMITH:
                    flag4 = true;
                    break;
            }
        }
        if (!flag && !flag2 && !flag3 && !flag4) return;
        if (__instance.Level < 200) return;
        if (GM.Core.SurvivedSeconds > 1800) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterPiumaSkinUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterPiuma").Value;
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.DEFAULT2);
    }
}