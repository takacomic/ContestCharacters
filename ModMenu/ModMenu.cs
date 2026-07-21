using CoffinTech.SaveData;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using MelonLoader;
using SurvivorModMenu.ModMenu;

namespace ContestCharacters.ModMenu;

public static class ModMenu
{

    public static void RegisterModMenu() =>
        ModMenuRegistry.Register(
        id: "ContestCharacters",
        displayName: "Contest Characters",
        build: BuildModMenu,
        sortOrder: 0);

    internal static DataManager _dataManager;
    private static void ResetCharacters()
    {
        /*foreach (var kvp in ContestCharactersMod.CharacterIdToType)
        {
            _dataManager._playerOptions.Config.UnlockedCharacters.Remove(kvp.Value);
            _dataManager._playerOptions.Config.BoughtCharacters.Remove(kvp.Value);
            _dataManager._playerOptions.Config.OpenedCoffins.Remove(kvp.Value);
        }*/
        _dataManager._playerOptions.Save();
    }
    
    private static void ResetAll()
    {
        _dataManager._playerOptions.Config.CollectedItems.Remove(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value);
        /*foreach (var kvp in ContestCharactersMod.CharacterIdToType)
        {
            _dataManager._playerOptions.Config.UnlockedCharacters.Remove(kvp.Value);
            _dataManager._playerOptions.Config.OpenedCoffins.Remove(kvp.Value);
            _dataManager._playerOptions.Config.BoughtCharacters.Remove(kvp.Value);
        }*/
        _dataManager._playerOptions.Save();
    }
    
    private static void ResetDisk()
    {
        _dataManager._playerOptions.Config.CollectedItems.Remove(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value);
        _dataManager._playerOptions.Save();
    }
    
    private static bool CheckSecret(string secret)
    {
        return _dataManager._playerOptions.Config.Secrets.Contains(ModOptionsData
            .CustomSecret(secret).Value);
    }

    private static void UpdateSecret(bool value, string secret)
    {
        if (value)
        {
            _dataManager._playerOptions.Config.Secrets.Add(ModOptionsData
                .CustomSecret(secret).Value);
            secret = secret.Replace("Unlock", "");
            _dataManager._playerOptions.Config.UnlockedCharacters.Add(ModOptionsData.CustomCharacter(secret).Value);
            _dataManager._playerOptions.Config.BoughtCharacters.Add(ModOptionsData.CustomCharacter(secret).Value);
        }
        else
        {
            _dataManager._playerOptions.Config.Secrets.Remove(ModOptionsData
                .CustomSecret(secret).Value);
            secret = secret.Replace("Unlock", "");
            _dataManager._playerOptions.Config.UnlockedCharacters.Remove(ModOptionsData.CustomCharacter(secret).Value);
            _dataManager._playerOptions.Config.OpenedCoffins.Remove(ModOptionsData.CustomCharacter(secret).Value);
            _dataManager._playerOptions.Config.BoughtCharacters.Remove(ModOptionsData.CustomCharacter(secret).Value);
        }
        
        _dataManager._playerOptions.Save();
    }

    private static void BuildModMenu(ModMenuBuilder builder)
    {
        builder.AddButton("Reset Characters", ResetCharacters);
        builder.AddButton("Reset Disk", ResetDisk);
        builder.AddButton("Reset All", ResetAll);
        builder.AddLabel("Secrets");
        builder.AddToggle("Zeta", () => CheckSecret("ContestCharacterZetaUnlock"),
            value => UpdateSecret(value, "ContestCharacterZetaUnlock"));
        builder.AddToggle("Usui", () => CheckSecret("ContestCharacterUsuiUnlock"),
            value => UpdateSecret(value, "ContestCharacterUsuiUnlock"));
        builder.AddToggle("Ashnard", () => CheckSecret("ContestCharacterAshnardUnlock"),
            value => UpdateSecret(value, "ContestCharacterAshnardUnlock"));
        builder.AddToggle("Piuma", () => CheckSecret("ContestCharacterPiumaUnlock"),
            value => UpdateSecret(value, "ContestCharacterPiumaUnlock"));
        builder.AddToggle("Luigi", () => CheckSecret("ContestCharacterLuigiUnlock"),
            value => UpdateSecret(value, "ContestCharacterLuigiUnlock"));
        builder.AddToggle("Guillotina", () => CheckSecret("ContestCharacterGuillotinaUnlock"),
            value => UpdateSecret(value, "ContestCharacterGuillotinaUnlock"));
        builder.AddToggle("Mortis", () => CheckSecret("ContestCharacterMortisUnlock"),
            value => UpdateSecret(value, "ContestCharacterMortisUnlock"));
        builder.AddToggle("SirBone", () => CheckSecret("ContestCharacterSirBoneUnlock"),
            value => UpdateSecret(value, "ContestCharacterSirBoneUnlock"));
        builder.AddToggle("Enzo", () => CheckSecret("ContestCharacterEnzoUnlock"),
            value => UpdateSecret(value, "ContestCharacterEnzoUnlock"));
        builder.AddToggle("Rubricco", () => CheckSecret("ContestCharacterRubriccoUnlock"),
            value => UpdateSecret(value, "ContestCharacterRubriccoUnlock"));
        builder.AddToggle("Beta", () => CheckSecret("ContestCharacterBetaUnlock"),
            value => UpdateSecret(value, "ContestCharacterBetaUnlock"));
        builder.AddToggle("Slein", () => CheckSecret("ContestCharacterSleinUnlock"),
            value => UpdateSecret(value, "ContestCharacterSleinUnlock"));
        builder.AddToggle("Specimen", () => CheckSecret("ContestCharacterSpecimenUnlock"),
            value => UpdateSecret(value, "ContestCharacterSpecimenUnlock"));
        builder.AddToggle("Roller", () => CheckSecret("ContestCharacterRollerUnlock"),
            value => UpdateSecret(value, "ContestCharacterRollerUnlock"));
        builder.AddToggle("Gourdtellio", () => CheckSecret("ContestCharacterGourdtellioUnlock"),
            value => UpdateSecret(value, "ContestCharacterGourdtellioUnlock"));
        builder.AddToggle("Baron", () => CheckSecret("ContestCharacterBaronUnlock"),
            value => UpdateSecret(value, "ContestCharacterBaronUnlock"));
    }
}