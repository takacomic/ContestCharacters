using CoffinTech.Extensions;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.DLC;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Stages;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(MerchantUIPage))]
public class ARG5Unlock
{
    
    [HarmonyPatch(nameof(MerchantUIPage.Populate))]
    [HarmonyPrefix]
    public static void Prefix(MerchantUIPage __instance)
    {
        if (GM.Core.MerchantInventory != MerchantInventoryType.CUSTOM) return;
        if (GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantCharacter != CharacterType.MARIASOFIA) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG1Unlock").Value))
        {
            if (GM.Core.PlayerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
            if (GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantInventoryItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
            GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantInventoryItems.Add(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value);
            return;
        }
        if (GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG6Unlock").Value))
        {
            if (GM.Core.PlayerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
            if (GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantInventoryItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
            GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantInventoryItems.Add(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value);
            return;
        }
        if (GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG5Unlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG4Unlock").Value)) return;
        if (GM.Core.SurvivedSeconds < 900) return;
        if (GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantInventoryItems.Contains(ModOptionsData.CustomItem("ContestCharacterARG5").Value)) return;
        GM.Core.CurrentCustomMerchant.CustomMerchantData.MerchantInventoryItems.Add(ModOptionsData.CustomItem("ContestCharacterARG5").Value);
        
    }
    
    [HarmonyPatch(nameof(MerchantUIPage.ProcessItemPurchase))]
    [HarmonyPostfix]
    public static void ProcessItemPostfix(MerchantUIPage __instance, ItemType t, ShopItemUI item, RectTransform sender)
    {
        if (!ModOptionsData.TryGetCustomItem(null, t, out var customItem)) return;
        if (!customItem.Key.Equals("ContestCharacterARG5")) return;
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterARG5Unlock").Value);
    }
    
    static List<string> _shopFactoryAvailableItems = new() {"ContestCharacterARG5", "ContestCharacterContestDisk"};
    
    [HarmonyPatch(nameof(MerchantUIPage.AddItem))]
    [HarmonyPostfix]
    public static void AddItemPostfix(MerchantUIPage __instance, ItemType t, int index)
    {
        if (!ModOptionsData.TryGetCustomItem(null, t, out var customItem)) return;
        if (!_shopFactoryAvailableItems.Contains(customItem.Key)) return;
        if (customItem.Key.Equals("ContestCharacterARG5"))
        {
            if (!__instance._playerOptions.Config.Secrets.Contains(ModOptionsData
                    .CustomSecret("ContestCharacterARG5Unlock").Value)) return;
            if (!__instance._playerOptions.Config.CollectedItems.Contains(ModOptionsData
                    .CustomItem("ContestCharacterARG5").Value)) return;
            __instance._spawned.ToList().Last().GetComponent<ShopItemUI>().SoldOut();
        }
        if (customItem.Key.Equals("ContestCharacterContestDisk"))
        {
            if (__instance._playerOptions.Config.CollectedItems.Contains(ModOptionsData
                    .CustomItem("ContestCharacterContestDisk").Value))
            __instance._spawned.ToList().Last().GetComponent<ShopItemUI>().SoldOut();
        }
    }
    
    [HarmonyPatch(typeof(BackgroundWater), nameof(BackgroundWater.Create))]
    [HarmonyPostfix]
    public static void CreatePostfix(BackgroundWater __instance)
    {
        if (GM.Core.Stage.StageType != StageType.SINKING) return;
        if (DlcSystem.LoadedDlc.ContainsKey(DlcType.Emeralds)) return;
        if (DlcSystem.LoadedDlc.ContainsKey(DlcType.Lemon)) return;
        PickupCustomMerchant merc = GM.Core.MakeStagePickup( new Vector2(-30.8f, -10.08f), ItemType.CUSTOM_MERCHANT).TryCast<PickupCustomMerchant>();
        merc.SetInventoryData(GM.Core._dataManager.AllCustomMerchantsData[CharacterType.MARIASOFIA]);
    }
}