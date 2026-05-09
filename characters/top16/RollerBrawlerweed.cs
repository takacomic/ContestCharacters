using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContestCharacters.characters.top16;

public class RollerBrawlerweedController : ModCharacterController
{
    private float size;
    private float damage;

    public override void AfterFullInit(CharacterController instance)
    {
        size = 1f;
        damage = 0.8f;
    }

    public override void EnemyOnPlayerOverlap(EnemyController instance, CharacterController player)
    {
        var dmg = damage * (player.PArmor() < 1 ? 1 : player.PArmor()) * player._cachedTransform.localScale.x;
        instance.GetDamaged(dmg);
        if (dmg < instance._hp) return;
        if (player._cachedTransform.localScale.x < 4f)
            Grow(player);
        else
            GrowDamage();
    }

    private void Grow(CharacterController player)
    {
        player.setScale(player._cachedTransform.localScale.x + 0.0002f, new Il2CppSystem.Nullable<float>());
    }
    private void GrowDamage()
    {
        damage += 0.0001f;
    }
}

public class RollerBrawlerweedStats : BaseCharacterData
{
    public RollerBrawlerweedStats()
    {
        CharName = "Roller";
        SurName = "Brawlerweed";
        TextureName = "Roller_Brawlerweed_walk";
        SpriteName = "Roller_Brawlerweed_walk_01.png";
        Description = "Deals contact damage. Grows and increases contact damage when defeating enemies with contact damage.";
        StartingWeapon = WeaponType.LAUREL;
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