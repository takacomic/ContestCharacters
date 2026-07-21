using CoffinTech.SaveData;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(SecretItemUI))]
public class SecretItemUIPatches
{
    [HarmonyPatch(nameof(SecretItemUI.GetRewardSprite))]
    [HarmonyPostfix]
    public static void Postfix(SecretItemUI __instance, SecretData bad, ref Sprite __result)
    {
        if(!ModOptionsData.TryGetCustomSecret(null, __instance._type, out var secret) ) return;
        //il2cpp nullables suck rn
        //Use custom secret class to get the character reward
        string? text = null;
        switch (secret.Key)
        {
            case "ContestCharacterPiumaSkinUnlock":
                __result = SpriteManager.GetSprite("p_weaponized.png", "Contest_face_portraits");
                return;
            case "ContestCharacterVinzonSkinUnlock":
                __result = SpriteManager.GetSprite("p_vinzon_alt.png", "Contest_face_portraits");
                return;
            case "ContestCharacterDuoUnlock":
                __result = SpriteManager.GetSprite("p_mortis_luigi.png", "Contest_face_portraits");
                return;
        }

        if (ModOptionsData.TryGetCustomCharacter(secret.Key.Replace("Unlock", ""), null, out var characterType))
                text = __instance._dataManager.GetConvertedCharacterData()[characterType.Value][0].portraitName;
        var characterReward = ((text == null) ? null : SpriteManager.GetSprite(text, "Contest_face_portraits"));
        if (characterReward == null)return;
        __result = characterReward;
    }
}