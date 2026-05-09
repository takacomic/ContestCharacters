using System.Text.Json.Serialization;
using CoffinTech.Utils;
using Newtonsoft.Json.Linq;
using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;

namespace ContestCharacters.characters;

public abstract class BaseCharacterData
{
    [JsonProperty("level")]
    public virtual int Level { get; set; } = 1;
    [JsonProperty("startingWeapon")]
    public virtual WeaponType StartingWeapon { get; set; } = WeaponType.VOID;
    [JsonProperty("charName")]
    public virtual string CharName { get; set; } = "Default";
    [JsonProperty("surName")]
    public virtual string SurName { get; set; } = "Name";
    [JsonProperty("spriteName")]
    public virtual string SpriteName { get; set; } = "Null";
    [JsonProperty("textureName")]
    public virtual string TextureName { get; set; } = "Null";
    [JsonProperty("walkingFrames")]
    public virtual int WalkingFrames { get; set; } = 4;
    /*[JsonProperty("skins")]
    public virtual JArray Skins { get; set; } = new();*/
    [JsonProperty("currentSkin")]
    public virtual string CurrentSkin { get; set; } = "DEFAULT";
    [JsonProperty("description")]
    public virtual string Description { get; set; } = "Default Description";
    [JsonProperty("isBought")]
    public virtual bool IsBought { get; set; } = true;
    [JsonProperty("price")]
    public virtual int Price { get; set; } = 10;
    
    [JsonProperty("maxHp")]
    public virtual int MaxHp { get; set; } = 100;
    [JsonProperty("armor")]
    public virtual float Armor { get; set; }
    [JsonProperty("regen")]
    public virtual float Regen { get; set; }
    [JsonProperty("moveSpeed")]
    public virtual float MoveSpeed { get; set; } = 1;
    [JsonProperty("power")]
    public virtual float Power { get; set; } = 1;
    [JsonProperty("area")]
    public virtual float Area { get; set; } = 1;
    [JsonProperty("speed")]
    public virtual float Speed { get; set; } = 1;
    [JsonProperty("duration")]
    public virtual float Duration { get; set; } = 1;
    [JsonProperty("amount")]
    public virtual float Amount { get; set; }
    [JsonProperty("growth")]
    public virtual float Growth { get; set; } = 1;
    [JsonProperty("luck")]
    public virtual float Luck { get; set; } = 1;
    [JsonProperty("cooldown")]
    public virtual float Cooldown { get; set; } = 1;
    [JsonProperty("shields")]
    public virtual float Shields { get; set; }
    [JsonProperty("greed")]
    public virtual float Greed { get; set; } = 1;
    [JsonProperty("revivals")]
    public virtual float Revivals { get; set; }
    [JsonProperty("rerolls")]
    public virtual float Rerolls { get; set; }
    [JsonProperty("skips")]
    public virtual float Skips { get; set; }
    [JsonProperty("magnet")]
    public virtual float Magnet { get; set; }
    [JsonProperty("curse")]
    public virtual float Curse { get; set; } = 1;
    [JsonProperty("banish")]
    public virtual float Banish { get; set; }
    [JsonProperty("shroud")]
    public virtual float Shroud { get; set; }
    [JsonProperty("charm")]
    public virtual int Charm { get; set; }
    [JsonProperty("defang")]
    public virtual float Defang { get; set; }
    [JsonProperty("thorns")]
    public virtual float Thorns { get; set; }
    [JsonProperty("invulTimeBonus")]
    public virtual float InvulTimeBonus { get; set; }
    [JsonProperty("fever")]
    public virtual float Fever { get; set; }
    [JsonProperty("recycle")]
    public virtual float Recycle { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public JObject Level20 = new JObject(
        new JProperty("growth", 1),
        new JProperty("level", 20)
    );
    [Newtonsoft.Json.JsonIgnore]
    public JObject Level21 = new JObject(
        new JProperty("growth", -1),
        new JProperty("level", 21)
    );
    [Newtonsoft.Json.JsonIgnore]
    public JObject Level40 = new JObject(
        new JProperty("growth", 1),
        new JProperty("level", 40)
    );
    [Newtonsoft.Json.JsonIgnore]
    public JObject Level41 = new JObject(
        new JProperty("growth", -1),
        new JProperty("level", 41)
    );

    [Newtonsoft.Json.JsonIgnore] 
    public JObject DefaultSkin = new JObject(
        
    );
    
    public virtual string JsonText()
    {
        return "";
    }
}