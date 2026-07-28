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
    public const string Name = "Contest Characters";
    public const string Author = "Takacomic";
    public const string Version = "1.1.2";
    public const string Download = "https://github.com/takacomic/ContestCharacters/releases";
}

public class ContestCharactersMod : MelonMod
{
    internal static JObject CharacterData = new();
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
        JObject jObject2 = new();
        
        List<object> dlcStuffs = new();
        dlcStuffs.Add("Contest Characters");
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
            CharacterRegister<ZetaController>("ContestCharacterZeta");
            CharacterRegister<UsuiController>("ContestCharacterUsui");
            CharacterRegister<AshnardController>("ContestCharacterAshnard");
            CharacterRegister<PiumaController>("ContestCharacterPiuma");
            CharacterRegister<LuigiController>("ContestCharacterLuigi");
            CharacterRegister<GuillotinaController>("ContestCharacterGuillotina");
            CharacterRegister<MortisController>("ContestCharacterMortis");
            CharacterRegister<SirBoneController>("ContestCharacterSirBone");
            CharacterRegister<EnzoController>("ContestCharacterEnzo");
            CharacterRegister<RubriccoController>("ContestCharacterRubricco");
            CharacterRegister<BetaController>("ContestCharacterBeta");
            CharacterRegister<SleinController>("ContestCharacterSlein");
            CharacterRegister<SpecimenController>("ContestCharacterSpecimen");
            CharacterRegister<RollerBrawlerweedController>("ContestCharacterRoller");
            CharacterRegister<GourdtellioController>("ContestCharacterGourdtellio");
            CharacterRegister<BaronController>("ContestCharacterBaron");
            
            CharacterRegister<LuigiMortisController>("ContestCharacterLuigiMortis");
            CharacterRegister<RatController>("ContestCharacterRat");
            CharacterRegister<VinzonController>("ContestCharacterVinzon");
        }

        private static void CharacterRegister<TController>(string characterId)
            where TController : ModCharacterController, new()
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