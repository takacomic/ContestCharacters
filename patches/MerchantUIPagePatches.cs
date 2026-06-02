using CoffinTech.SaveData;
using HarmonyLib;
using Il2CppVampireSurvivors;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.UI;
using UnityEngine;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(MerchantUIPage))]
public static class MerchantUIPagePatches
{
    [HarmonyPatch(nameof(MerchantUIPage.ProcessItemPurchase))]
    public static void Postfix(MerchantUIPage __instance, ItemType t, ShopItemUI item, RectTransform sender)
    {
        if (!ModOptionsData.TryGetCustomItem(null, t, out var customItem)) return;
        if (!__instance._playerOptions.Config.CollectedItems.Contains(customItem.Value));
        {
            __instance._playerOptions.Config.CollectedItems.Add(customItem.Value);
        }
        __instance._playerOptions.Config.RunPickups.Add(customItem.Value);
        item.SoldOut();
    }
}