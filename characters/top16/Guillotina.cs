using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContestCharacters.characters.top16;

public class GuillotinaController : ModCharacterController
{
    
}

public sealed class GuillotinaStats : BaseCharacterData
{
    public GuillotinaStats()
    {
        CharName = "Guillotina";
        SurName = "Ravera";
        TextureName = "Guillotina_Ravera_walk";
        SpriteName = "Guillotina_Ravera_walk_01.png";
        Description = "Goth anti-facist lady with the power of guillotines.";
        StartingWeapon = WeaponType.SONG;
        Luck += 0.2f;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        obj.Add("onEveryLevelUp", new JObject(
            new JProperty("area", 0.05f)
        ));
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}