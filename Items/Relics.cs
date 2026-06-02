using Il2CppI2.Loc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ContestCharacters.Items;

public class RelicDisk
{
    
    [JsonProperty("contentGroup")] 
    public const string ContentGroup = "BASE";
    [JsonProperty("name")]
    public const string Name = "Contest Disk";
    [JsonProperty("description")]
    public const string Description = "Allows access to the Contest characters.";
    [JsonProperty("tips")]
    public const string Tips = "-";
    [JsonProperty("achievementTips")]
    public const string AchievementTips = "-";
    [JsonProperty("texture")]
    public const string Texture = "contest_disk";
    [JsonProperty("frameName")]
    public const string FrameName = "CONTEST_DISK_01.png";
    [JsonProperty("pickedupAmount")]
    public const int PickedupAmount = 0;
    [JsonProperty("rarity")]
    public const int Rarity = 0;
    [JsonProperty("unlocksAt")]
    public const int UnlocksAt = 1000000;
    [JsonProperty("inTreasures")]
    public const bool InTreasures = false;
    [JsonProperty("hidden")]
    public const bool Hidden = false;
    [JsonProperty("value")]
    public const int Value = 0;
    [JsonProperty("isRelic")]
    public const bool IsRelic = true;
    
    
    public string JsonText()
    {
        var obj = JObject.FromObject(this);
        return JsonConvert.SerializeObject(obj);
    }
    
    public TextAsset Text()
    {
        JObject obj = new() { { "100000", JObject.Parse(JsonText()) } };
        return new TextAsset(JsonConvert.SerializeObject(obj));
    }
    
    public void SetLanguageData(string contentName, LanguageSourceData languageData)
    {
        var prefix = "itemLang/{" + contentName + "}";
        var nameLoc = languageData.AddTerm(prefix + "name");
        nameLoc.SetTranslation(0, Name);
        var descLoc = languageData.AddTerm(prefix + "description");
        descLoc.SetTranslation(0, Description);
        var tipsLoc = languageData.AddTerm(prefix + "tips");
        tipsLoc.SetTranslation(0, Tips);
    }
}