using ContestCharacters;
using MelonLoader;

[assembly: MelonInfo(typeof(ContestCharactersMod), ModInfo.Name, ModInfo.Version, ModInfo.Author, ModInfo.Download)]
[assembly: MelonGame("poncle", "Vampire Survivors")]

namespace ContestCharacters;

internal static class ModInfo
{
    public const string Name = "ContestCharacters";
    public const string Author = "Takacomic";
    public const string Version = "0.0.1";
    public const string Download = "https://github.com/takacomic/.../latest";
}

public class ContestCharactersMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("Started");
    }
}