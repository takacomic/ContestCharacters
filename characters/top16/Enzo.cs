using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContestCharacters.characters.top16;

public class EnzoController : ModCharacterController
{
    private float greedIncrease;
    public override void AfterFullInit(CharacterController instance)
    {
        greedIncrease = 0;
        instance._spriteAnimation._currentAnimation._fps = 4;
    }

    public override void LevelUp(CharacterController instance)
    {
        instance._playerStats.Greed._val -= greedIncrease;
        if (instance.Level % 4 == 0)
        {
            instance._playerStats.MoveSpeed._val += 0.01f;
            instance._playerStats.Speed._val += 0.01f;
        }
        greedIncrease = instance._playerStats.MoveSpeed._val + instance._playerStats.Speed._val;
        instance._playerStats.Greed._val += greedIncrease;
    }
}

public sealed class EnzoStats : BaseCharacterData
{
    public EnzoStats()
    {
        CharName = "Enzo";
        SurName = "Brigante";
        TextureName = "Enzo_Brigante_walk";
        SpriteName = "Enzo_Brigante_walk_01.png";
        Description = "Speed brings greed";
        StartingWeapon = WeaponType.VESPERS;
        
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}