using CoffinTech.SaveData;
using ContestCharacters.spells;
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Spells;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(SpellsManager))]
public static class SpellsManagerPatches
{
    private static SecretType _secretType = SecretType.CastThiefSpell;
    internal static SpellDirecterWillNotLikeThis _spell;
    [HarmonyPatch(nameof(SpellsManager.StartSpell))]
    [HarmonyPostfix]
    public static void Postfix(SpellsManager __instance, SecretType secretType)
    {
        if (ModOptionsData.CustomSecret("ContestCharacterARG6Unlock").Value != secretType) return;
                
        var spell = new SpellDirecterWillNotLikeThis();
        spell.Init(__instance._playerOptions, __instance._signalBus, __instance);
        spell.Start();
        _secretType = ModOptionsData.CustomSecret("ContestCharacterARG6Unlock").Value;
        _spell = spell;
    }
    
    [HarmonyPatch(nameof(SpellsManager.ActivateSpells))]
    [HarmonyPostfix]
    public static void ActivateSpellsPostfix()
    {
        if (ModOptionsData.CustomSecret("ContestCharacterARG6Unlock").Value != _secretType) return;
        
        _spell.Activate();
    }
}