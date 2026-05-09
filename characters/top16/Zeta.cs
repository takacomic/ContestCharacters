using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Pickups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Action = System.Action;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class ZetaController : ModCharacterController
{
    private CharacterController? _characterController;
    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        
        List<WeaponType> evos = new();
        foreach (var specialWeapon in instance._gameManager.LevelUpFactory.SpecialWeapons)
        {
            var data = instance._dataManager.GetConvertedWeapons()[specialWeapon][0];
            if (!data.isEvolution) continue;
            
            evos.Add(specialWeapon);
        }

        foreach (var evo in evos)
        {
            instance._gameManager.LevelUpFactory.SpecialWeapons.Remove(evo);
            instance._gameManager._levelUpFactory.WeaponStore.AddLast(evo);
        }
        
        instance._gameManager._levelUpFactory.CalculateWeights(instance);
        evos.Clear();

        instance._onCriticalHP = new Action(RandomPickup);
    }

    private void RandomPickup()
    {
        if (_characterController == null) return;
        var pickup = new List<ItemType>()
        {
            ItemType.GEM, ItemType.ROAST, ItemType.COIN, ItemType.COINBAG1, ItemType.COINBAG2, ItemType.ROSARY, ItemType.NFT, ItemType.SORBETTO, ItemType.OROLOGION,
            ItemType.VACUUM, ItemType.CLOVER, ItemType.GILDED, ItemType.GOLDFINGER, ItemType.RELIC_GOLDENEGG, ItemType.FB_RAPIDFIRE, ItemType.FB_BARRIER, ItemType.FB_GRENADE,
            ItemType.TP_WALL_CHICKEN, ItemType.TP_HEART_REFRESH, ItemType.TP_KARMA_COIN, ItemType.TP_MIRROR_OF_TRUTH, ItemType.TP_NEUTRON_BOMB, ItemType.BONUS_CURSEDSOUL,
            ItemType.BONUS_FROZENSOUL, ItemType.STATIC_GOLDPILE, ItemType.LITTLEHEART, ItemType.SV_DRAFT1
        };

        var rand = Random.Range(0, 27);
        GM.Core.MakePickup(_characterController.CurrentPos ,pickup[rand]);
    }
}

public sealed class ZetaStats : BaseCharacterData
{
    public ZetaStats()
    {
        CharName = "Zeta";
        SurName = "Settetails";
        TextureName = "zeta_walk";
        SpriteName = "zeta_walk_01.png";
        Description = "Can find evolutions in levelups. Triggers a random pickup when health reaches crit level. Gains +1 revival every 10 levels (max +6)";
        StartingWeapon = WeaponType.VOID;
        MaxHp -= 50;
        MoveSpeed += 0.3f;
        Area += 0.3f;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        jArray.Add(obj);
        obj.Add("hiddenWeapons", new JArray("SILF_COUNTER", "SILF2_COUNTER"));
        obj.Add("onEveryLevelUp", new JObject(
            new JProperty("power", 0.005f),
            new JProperty("speed", 0.005f),
            new JProperty("duration", 0.005f),
            new JProperty("area", 0.005f)
            ));
        jArray.Add(new JObject(
            new JProperty("level", 10),
            new JProperty("revivals", 1)));
        Level20.Add("revivals", 1);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(new JObject(
            new JProperty("level", 30),
            new JProperty("revivals", 1)));
        Level40.Add("revivals", 1);
        jArray.Add(Level40);
        jArray.Add(Level41);
        jArray.Add(new JObject(
            new JProperty("level", 50),
            new JProperty("revivals", 1)));
        jArray.Add(new JObject(
            new JProperty("level", 60),
            new JProperty("revivals", 1)));
        return JsonConvert.SerializeObject(jArray);
    }
}