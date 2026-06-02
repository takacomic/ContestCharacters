using CoffinTech.Extensions;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(MerchantUIPage))]
public class ARG5Unlock
{
    [HarmonyPatch(nameof(MerchantUIPage.Populate))]
    [HarmonyPostfix]
    private static void Postfix(MerchantUIPage __instance)
    {
        if (GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG5Unlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG4Unlock").Value)) return;
        if (GM.Core.MerchantInventory != MerchantInventoryType.CUSTOM) return;
        if (GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantCharacter != CharacterType.MARIASOFIA) return;
        __instance.AddItem(ModOptionsData.CustomItem("ContestCharacterARG5").Value, __instance._shopFactory.AvailableItems.Count);
    }
    
    [HarmonyPatch(nameof(MerchantUIPage.ProcessItemPurchase))]
    [HarmonyPostfix]
    public static void ProcessItemPostfix(MerchantUIPage __instance, ItemType t, ShopItemUI item, RectTransform sender)
    {
        if (!ModOptionsData.TryGetCustomItem(null, t, out var customItem)) return;
        if (!customItem.Key.Equals("ContestCharacterARG5")) return;
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterARG5Unlock").Value);
    }
    
    [HarmonyPatch(nameof(MerchantUIPage.AddItem))]
    [HarmonyPostfix]
    public static void AddItemPostfix(MerchantUIPage __instance, ItemType t, int index)
    {
        if (!ModOptionsData.TryGetCustomItem(null, t, out var customItem)) return;
        if (!customItem.Key.Equals("ContestCharacterARG5")) return;
        if (!__instance._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG5Unlock").Value)) return;
        if (!__instance._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterARG5").Value)) return;
        __instance._spawned.ToList().Last().GetComponent<ShopItemUI>().SoldOut();
    }
}