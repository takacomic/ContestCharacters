using System.Runtime.CompilerServices;
using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.TimerSystem;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Timer = Il2CppVampireSurvivors.Framework.TimerSystem.Timer;

namespace ContestCharacters.characters.top16;

public class GourdtellioController : ModCharacterController
{
    private Timer? _standing;
    private float _tempBonus;
    private float _removalBonus;
    private float _tempTime = 0.5f;
    private PlayerModifierStats _playerModifierStats;

    public override void AfterFullInit(CharacterController instance)
    {
        _playerModifierStats = instance._playerStats;
    }

    public override void OnStop(CharacterController instance)
    {
        if(_standing != null && instance.Walked == 0f) return;
        _standing = Timers.Register(7500, null, new Action<float>(StatBonus));
    }

    public override void OnUpdate(CharacterController instance)
    {
        if(instance.Walked == 0f) return;
        if(_standing == null) return;
        
        _standing?.Cancel();
        _standing = null;
        StatReset(instance);
    }

    private void StatBonus(float time)
    {
        if (time < _tempTime || _tempTime > 7.6) return;
        
        _tempTime += 0.5f;
        _tempBonus += 0.0025f;
        _removalBonus += _tempBonus;
        _playerModifierStats._Area_k__BackingField._val += _tempBonus;
        _playerModifierStats._Curse_k__BackingField._val += _tempBonus;
    }

    private void StatReset(CharacterController instance)
    {
        _playerModifierStats._Area_k__BackingField._val -= _removalBonus;
        _playerModifierStats._Curse_k__BackingField._val -= _removalBonus;
        _tempBonus = 0f;
        _removalBonus = 0f;
        _tempTime = 0.5f;
    }
}

public sealed class GourdtellioStats : BaseCharacterData
{
    public GourdtellioStats()
    {
        CharName = "Gourdtellio";
        SurName = "Crowlaguard";
        TextureName = "Gourdtellio_Crowlaguard_walk";
        SpriteName = "Gourdtellio_Crowlaguard_walk_01.png";
        PortraitName = "p_gourdtellio.png";
        Description = "A lowly scarecrow who is outstanding in his field. When standing, gains +30% Aura and Curse over time, resets when moving.";
        StartingWeapon = WeaponType.SILF2;
        Curse += 0.1f;
        Area += 0.1f;
        MoveSpeed -= 0.25f;
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