using CoffinTech.Utils;
using ContestCharacters.unlockpatches;
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
    private float scale = 5.5f;
    private bool _unlocked;

    public override void AfterFullInit(CharacterController instance)
    {
        size = 1f;
        damage = 0.8f;
        if (GourdtellioUnlock._gourdtellioUnlockReady) scale = 15f;
    }

    public override void EnemyOnPlayerOverlap(EnemyController instance, CharacterController player)
    {
        var dmg = damage * (player.PArmor() < 1 ? 1 : player.PArmor()) * player._cachedTransform.localScale.x;
        instance.GetDamaged(dmg);
        if (dmg < instance._hp) return;
        
        if (player._cachedTransform.localScale.x < scale)
            Grow(player);
        else
            GrowDamage();
    }

    private void Grow(CharacterController player)
    {
        player.setScale(player._cachedTransform.localScale.x + 0.005f, new Il2CppSystem.Nullable<float>());
        if (!(player._cachedTransform.localScale.x > 10.1) || !GourdtellioUnlock._gourdtellioUnlockReady || _unlocked) return;
        
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterGourdtellioUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterGourdtellio").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
        _unlocked = true;
    }
    private void GrowDamage()
    {
        damage += 0.002f;
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
        PortraitName = "p_roller.png";
        Description = "Deals contact damage. Grows and increases contact damage when defeating enemies with contact damage.";
        StartingWeapon = WeaponType.LAUREL;
        MaxHp += 50;
        Armor += 3;
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