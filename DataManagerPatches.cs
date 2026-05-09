using HarmonyLib;
using Il2CppSystem.Reflection;
using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;
using CoffinTech.SaveData;
using CoffinTech.Utils;
using ContestCharacters.characters;
using ContestCharacters.characters.top16;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Projectiles;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters;
    internal static class DataManagerPatches
    {
        internal static Dictionary<string, CharacterType> IdToType = new();
        internal static readonly JsonSerializerSettings SerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
        
        [HarmonyPatch(typeof(DataManager))]
        static class DataManagerPatch
        {
            [HarmonyPatch(nameof(DataManager.LoadBaseJObjects))]
            [HarmonyPostfix]
            static void LoadBaseJObjects_Postfix(DataManager __instance, object[] __args, MethodBase __originalMethod)
            {
                SpriteRegister();
                CharacterRegister(__instance);
            }
        }

        static void SpriteRegister()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] assets =
            {
                "chef_luigi_walk.png", "Enzo_Brigante_walk.png", "Gourdtellio_Crowlaguard_walk.png",
                "Mortis_Surmanski_walk.png", "Piuma_Ferro_walk.png", "Roller_Brawlerweed_walk.png",
                "Sir_Bone_walk.png", "specimen_40_walk.png", "baron_husker_walk.png", "Rubricco_Puzzorelio_walk.png",
                "Usui_Yukimi_walk.png", "Ashnard_Brenen_walk.png", "beta_walk.png", "zeta_walk.png",
                "Slein_walk.png", "Guillotina_Ravera_walk.png"
            };

            foreach (var asset in assets)
            {
                Texture2D texture = SpriteImporter.LoadTextureFromAssembly(assembly, "ContestCharacters.resources",asset);
                SpriteImporter.SpriteStrip(texture, asset.Split('.').First(), 4);
            }
        }

        private static void CharacterRegister(DataManager __instance)
        {
            CharacterRegister<GourdtellioController, GourdtellioStats>(__instance, "ContestCharacterGourd");
            CharacterRegister<RollerBrawlerweedController, RollerBrawlerweedStats>(__instance, "ContestCharacterRoller");
            CharacterRegister<AshnardController, AshnardStats>(__instance, "ContestCharacterAshnard");
            CharacterRegister<BaronController, BaronStats>(__instance, "ContestCharacterBaron");
            CharacterRegister<BetaController, BetaStats>(__instance, "ContestCharacterBeta");
            CharacterRegister<ZetaController, ZetaStats>(__instance, "ContestCharacterZeta");
            CharacterRegister<LuigiController, LuigiStats>(__instance, "ContestCharacterLuigi");
            CharacterRegister<EnzoController, EnzoStats>(__instance, "ContestCharacterEnzo");
            CharacterRegister<MortisController, MortisStats>(__instance, "ContestCharacterMortis");
            CharacterRegister<PiumaController, PiumaStats>(__instance, "ContestCharacterPiuma");
            CharacterRegister<RubriccoController, RubriccoStats>(__instance, "ContestCharacterRubricco");
            CharacterRegister<SirBoneController, SirBoneStats>(__instance, "ContestCharacterSirBone");
            CharacterRegister<UsuiController, UsuiStats>(__instance, "ContestCharacterUsui");
            CharacterRegister<GuillotinaController, GuillotinaStats>(__instance, "ContestCharacterGuillotina");
            CharacterRegister<SpecimenController, SpecimenStats>(__instance, "ContestCharacterSpecimen");
            CharacterRegister<SleinController, SleinStats>(__instance, "ContestCharacterSlein");
        }

        private static void CharacterRegister<TController, TStats>(DataManager manager, string characterId)
            where TController : ModCharacterController, new()
            where TStats : BaseCharacterData, new()
        {
            var characterType =
                ModCharacterControllerRegistry.Register(ModCharacterController.GetInstance<TController>());
            ModOptionsData.SetCharacterId(characterType, characterId);
            
            var json = new TStats().JsonText();
            var jArray = JArray.Parse(json);

            manager._allCharactersJson.Add(characterType.ToString(), jArray);
            IdToType.Add(characterId, characterType);
        }
    }
