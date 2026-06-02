using CoffinTech.SaveData;
using ContestCharacters.characters.top16;
using HarmonyLib;
using Il2CppRewiredConsts;
using Il2CppVampireSurvivors;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Signals;
using Il2CppVampireSurvivors.UI;
using MelonLoader;
using UnityEngine.UI;
using Action = System.Action;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(ItemFoundPage))]
public static class ItemFoundPagePatches
{
    internal static bool _diskFound;
    [HarmonyPatch(nameof(ItemFoundPage.ReceiveItem))]
    [HarmonyPostfix]
    private static void ReceiveItemPostfix(ItemFoundPage __instance)
    {
        if (__instance._item != ModOptionsData.CustomItem("ContestCharacterContestDisk").Value) return;
        
        __instance._GetButton.gameObject.SetActive(value: false);
        __instance._YellowSign.DoClaps( new Action( delegate
        {
            if (GM.Core.IsOnlineMultiplayer) return;
            AppWarningState.HasShown = false;
            WarningPage.Corrupt = true;
        }));
        HarmonyPatching.UnPatch(typeof(ZetaController.ZetaHarmony));
        _diskFound = true;
    }
    
    [HarmonyPatch(nameof(ItemFoundPage.CacheItem))]
    [HarmonyPrefix]
    private static bool CacheItemPrefix(ItemFoundPage __instance, GameplaySignals.PlayerPickedUpNewItemSignal sig)
    {
        if (!ModOptionsData.IsCustomRelic(sig.Item)) return true;
        __instance._playerWhoFoundIt = sig.Character;
        if (sig.Character == null)
        {
            __instance._playerWhoFoundIt = GM.Core.PlayerOne;
        }
        __instance._discarded = false;
        __instance._item = ItemType.VOID;
        __instance._itemData = null;
        __instance._weapon = WeaponType.VOID;
        __instance._weaponData = null;
        __instance._item = sig.Item;
        __instance._itemData = __instance._dataManager.AllItems[sig.Item];
        __instance._GetButton.GetComponent<Button>().interactable = false;
        __instance.SetRelicDisplay();
        __instance._canDiscard = false;
        return false;
    }
}