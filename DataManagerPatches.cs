using HarmonyLib;
using Il2CppSystem.Reflection;
using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;
using CoffinTech.SaveData;
using CoffinTech.Utils;
using ContestCharacters.characters;
using ContestCharacters.characters.secret;
using ContestCharacters.characters.top16;
using ContestCharacters.Items;
using Il2CppI2.Loc;
using Il2CppNewtonsoft.Json.Linq;
using Il2CppVampireSurvivors.App.Data;
using Il2CppVampireSurvivors.Framework.DLC;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Projectiles;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters;
    internal static class DataManagerPatches
    {
        
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
                ModMenu.ModMenu._dataManager = __instance;
                SpriteRegister();
                
                var obby2 = new JObject();
                obby2.Add("textureName", "beta_seal");
                obby2.Add("frameName", "beta_seal.png");
                obby2.Add("destroyedAmount", 0);
                obby2.Add("maxHp", 50);
                obby2.Add("destructibleType", "100000");
                
                __instance._allPropsJson.Add("100000", obby2);
            }
        }

        static void SpriteRegister()
        {
            Sprite[] sprites = ContestCharactersMod.bundle.LoadAll<Sprite>();
            foreach (var sprite in sprites)
            {
                SpriteManager.RegisterSprite(sprite);
            }
            PaletteSwapper._bundle = ContestCharactersMod.bundle;
        }

        
    }
