using HarmonyLib;
using Il2CppI2.Loc;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(SecretsPage))]
public class SecretsPagePatches
{
    static List<string> _secrets = new() { "ContestCharacterZetaUnlock", "ContestCharacterUsuiUnlock", "ContestCharacterAshnardUnlock", "ContestCharacterPiumaUnlock", 
        "ContestCharacterLuigiUnlock", "ContestCharacterGuillotinaUnlock", "ContestCharacterMortisUnlock", "ContestCharacterSirBoneUnlock", "ContestCharacterEnzoUnlock",
        "ContestCharacterRubriccoUnlock", "ContestCharacterBetaUnlock", "ContestCharacterSleinUnlock", "ContestCharacterSpecimenUnlock", "ContestCharacterRollerUnlock",
        "ContestCharacterGourdtellioUnlock", "ContestCharacterBaronUnlock"
    };
    [HarmonyPatch(nameof(SecretsPage.Start))]
    [HarmonyPostfix]
    public static void Postfix(SecretsPage __instance)
    {
        GridLayoutGroup obj = __instance.gameObject.GetComponentInChildren<GridLayoutGroup>();
        
        obj.cellSize = new Vector2(78, 78);
        
        if (__instance._characters.Contains('2')) return;
        
        __instance._characters.Add('2');
        __instance._characters.Add('3');
        __instance._characters.Add('4');
        __instance._characters.Add('5');
        __instance._characters.Add('6');
        __instance._characters.Add('7');
        __instance._characters.Add('8');
        __instance._characters.Add('9');
        __instance._characters.Add('0');
        __instance._characters.Add('?');
    }

    [HarmonyPatch(nameof(SecretsPage.OnShowStart))]
    [HarmonyPrefix]
    public static void OnShowStart(SecretsPage __instance, GameObject g)
    {
        __instance._secrets = __instance._data.AllSecrets;
        bool flag = __instance._playerOptions.Config.HasCollectedItem(ModOptionsData.CustomItem("ContestCharacterARG1").Value);
        bool flag2 = __instance._playerOptions.Config.HasCollectedItem(ModOptionsData.CustomItem("ContestCharacterARG2").Value);
        bool flag3 = __instance._playerOptions.Config.HasCollectedItem(ModOptionsData.CustomItem("ContestCharacterARG3").Value);
        bool flag4 = __instance._playerOptions.Config.HasCollectedItem(ModOptionsData.CustomItem("ContestCharacterARG4").Value);
        bool flag5 = __instance._playerOptions.Config.HasCollectedItem(ModOptionsData.CustomItem("ContestCharacterARG5").Value);
        bool flag6 = __instance._playerOptions.Config.HasCollectedItem(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value);
        bool flag7 = __instance._playerOptions.Config.UnlockedCharacters.Contains(ModOptionsData.CustomCharacter("ContestCharacterBaron").Value);
        bool flag8 = __instance._playerOptions.Config.UnlockedCharacters.Contains(ModOptionsData.CustomCharacter("ContestCharacterPiuma").Value);
        bool flag9 = __instance._playerOptions.Config.UnlockedCharacters.Contains(ModOptionsData.CustomCharacter("ContestCharacterVinzon").Value);
        if (flag)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterARG2Unlock").Value].hidden = false;
        }
        if (flag2)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterARG3Unlock").Value].hidden = false;
        }
        if (flag3)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterARG4Unlock").Value].hidden = false;
        }
        if (flag4)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterARG5Unlock").Value].hidden = false;
        }
        if (flag5)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterARG6Unlock").Value].hidden = false;
        }
        if (flag6)
        {
            foreach (var secret in _secrets)
                __instance._secrets[ModOptionsData.CustomSecret(secret).Value].hidden = false;
        }
        if (flag7)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterDuoUnlock").Value].hidden = false;
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterRatUnlock").Value].hidden = false;
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterVinzonUnlock").Value].hidden = false;
        }
        if (flag8)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterPiumaSkinUnlock").Value].hidden = false;
        }
        if (flag9)
        {
            __instance._secrets[ModOptionsData.CustomSecret("ContestCharacterVinzonSkinUnlock").Value].hidden = false;
        }
    }

    [HarmonyPatch(nameof(SecretsPage.SetInfoPanel))]
    [HarmonyPrefix]
    public static bool SetInfoPanel(SecretsPage __instance, SecretData data, SecretType type, SecretItemUI item)
    {
        if (!ModOptionsData.IsCustomSecret(type))  return true;
        if(!ModOptionsData.TryGetCustomSecret(null, type, out var secret) ) return true;
        if (!ModOptionsData.TryGetCustomItem(secret.Key.Replace("Unlock", ""), null, out var itemType)) return true;
        if (!item._hasAchieved) return true;
        var itemData = __instance._data.AllItems?[itemType.Value];
        __instance._CharacterRewardIcon.sprite = SpriteManager.GetSprite(itemData.frameName, itemData.texture);
        __instance._OtherRewardIcon.sprite = SpriteManager.GetSprite(itemData.frameName, itemData.texture);
        __instance._Unlocks.text = LocalizationManager.GetTranslation("lang/genericPopup_unlocked") + " " + itemData.name;
        
        return false;
    }
}