using CoffinTech.Patches;
using CoffinTech.SaveData;
using CoffinTech.Utils;
using ContestCharacters;
using ContestCharacters.characters;
using ContestCharacters.characters.secret;
using ContestCharacters.characters.top16;
using ContestCharacters.components;
using ContestCharacters.Items;
using Il2CppI2.Loc;
using Il2CppInterop.Runtime.Injection;
using Newtonsoft.Json.Linq;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.DLC;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[assembly: MelonInfo(typeof(ContestCharactersMod), ModInfo.Name, ModInfo.Version, ModInfo.Author, ModInfo.Download)]
[assembly: MelonGame("poncle", "Vampire Survivors")]
[assembly: MelonOptionalDependencies("SurvivorModMenu")]

namespace ContestCharacters;

internal static class ModInfo
{
    public const string Name = "GalloTower?";
    public const string Author = "???";
    public const string Version = "1.0.0";
    public const string Download = "?";
}

public class ContestCharactersMod : MelonMod
{
    internal static JObject CharacterData = new();
    internal static Dictionary<string, CharacterType> CharacterIdToType = new();
    internal static Il2CppAssetBundle bundle;
    public static bool OtherPluginPresent { get; private set; }

    public override void OnInitializeMelon()
    {
        ClassInjector.RegisterTypeInIl2Cpp<PoisonComponent>();
        JObject jObject = new();
        TextAsset items = new();
        TextAsset secrets = new();
        TextAsset characters = new();
        using (Stream? stream = System.Reflection.Assembly.GetExecutingAssembly()
                   .GetManifestResourceStream("ContestCharacters.resources.contestbundle"))
        {
            if (stream == null) return;
            MemoryStream memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);
            Il2CppSystem.IO.MemoryStream il2MemoryStream = new(memoryStream.ToArray());
            bundle = Il2CppAssetBundleManager.LoadFromStream(il2MemoryStream);
            items = bundle.LoadAsset<TextAsset>("items.json");
            secrets = bundle.LoadAsset<TextAsset>("secrets.json");
            characters = bundle.LoadAsset<TextAsset>("characters.json");
        }
        CharacterRegister();
        //CharacterIdToType.Add("ContestCharacterRat", (CharacterType)100017);
        //CharacterIdToType.Add("ContestCharacterNyx", (CharacterType)100018);
        ModOptionsData.CustomSecret("DirecterWillNotLikeThis");
        JObject jObject2 = new();
        
        List<object> dlcStuffs = new();
        dlcStuffs.Add("???");
        dlcStuffs.Add("1.0.000");
        dlcStuffs.Add(PopulateDataSettings(items, secrets, characters));
        DlcPatches.AddDlc((DlcType)100000, dlcStuffs);
#if DEBUG
        OtherPluginPresent = RegisteredMelons
            .Any(m => m.Info.Name == "SurvivorModMenu");
        if (OtherPluginPresent)
        {
            ModMenu.ModMenu.RegisterModMenu();
        }
#endif
    }
    
    
    private static void CharacterRegister()
        {
            CharacterRegister<ZetaController, ZetaStats>("ContestCharacterZeta");
            CharacterRegister<UsuiController, UsuiStats>("ContestCharacterUsui");
            CharacterRegister<AshnardController, AshnardStats>("ContestCharacterAshnard");
            CharacterRegister<PiumaController, PiumaStats>("ContestCharacterPiuma");
            CharacterRegister<LuigiController, LuigiStats>("ContestCharacterLuigi");
            CharacterRegister<GuillotinaController, GuillotinaStats>("ContestCharacterGuillotina");
            CharacterRegister<MortisController, MortisStats>("ContestCharacterMortis");
            CharacterRegister<SirBoneController, SirBoneStats>("ContestCharacterSirBone");
            CharacterRegister<EnzoController, EnzoStats>("ContestCharacterEnzo");
            CharacterRegister<RubriccoController, RubriccoStats>("ContestCharacterRubricco");
            CharacterRegister<BetaController, BetaStats>("ContestCharacterBeta");
            CharacterRegister<SleinController, SleinStats>("ContestCharacterSlein");
            CharacterRegister<SpecimenController, SpecimenStats>("ContestCharacterSpecimen");
            CharacterRegister<RollerBrawlerweedController, RollerBrawlerweedStats>("ContestCharacterRoller");
            CharacterRegister<GourdtellioController, GourdtellioStats>("ContestCharacterGourdtellio");
            CharacterRegister<BaronController, BaronStats>("ContestCharacterBaron");
            
            CharacterRegister<LuigiMortisController, LuigiMortisStats>("ContestCharacterLuigiMortis");
            CharacterRegister<RatController, LuigiMortisStats>("ContestCharacterRat");
        }

        private static void CharacterRegister<TController, TStats>(string characterId)
            where TController : ModCharacterController, new()
            where TStats : BaseCharacterData, new()
        {
            ModCharacterControllerRegistry.Register(ModCharacterController.GetInstance<TController>(), characterId);
            
            //var json = new TStats().JsonText();
            //var jArray = JArray.Parse(json);
            //CharacterData.Add(characterType.ToString(), jArray);

            //manager._allCharactersJson.Add(characterType.ToString(), jArray);
            //CharacterIdToType.Add(characterId, characterType);
        }
        
        
    
        private static DataManagerSettings PopulateDataSettings(TextAsset items, TextAsset secrets, TextAsset characters)
        {
            var settings = new DataManagerSettings
            {
                _CharacterDataJsonAsset = characters,
                _ItemDataJsonAsset = items,
                _SecretsDataJsonAsset = secrets
            };
            
            return settings;
        }
}