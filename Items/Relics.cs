using Newtonsoft.Json;

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
    [JsonProperty("collectionFrame")]
    public const string CollectionFrame = "frameB_lemon.png";
    [JsonProperty("texture")]
    public const string Texture = "CommunityContest";
    [JsonProperty("frameName")]
    public const string FrameName = "CONTEST_DISK.png";
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
    public const int Value = 50000;
    [JsonProperty("isRelic")]
    public const bool IsRelic = true;
    [JsonProperty("sealable")]
    public const bool Sealable = false;
}