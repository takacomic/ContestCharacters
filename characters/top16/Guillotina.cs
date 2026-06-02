using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.TimerSystem;
using Il2CppVampireSurvivors.Objects.Characters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Timer = Il2CppVampireSurvivors.Framework.TimerSystem.Timer;

namespace ContestCharacters.characters.top16;

public class GuillotinaController : ModCharacterController
{
    private Timer? _timer;
    public override void AfterFullInit(CharacterController instance)
    {
        
    }

    public override void EnemyOnPlayerOverlap(EnemyController instance, CharacterController player)
    {
        if (instance.EnemyType.ToString().Contains("BOSS") && player._skinType == SkinType.DEFAULT) return;
        if (_timer == null)
        {
            _timer = Timers.Register(15, new Action(OnComplete));
            foreach (var phaserGameObject in player._gameManager.Enemies.children)
            {
                var enemy = phaserGameObject.TryCast<EnemyController>();
                if (enemy == null) continue;
                if (enemy.EnemyType.ToString().Contains("BOSS")) continue;
                if (enemy.IsDead) continue;
                var x = Math.Abs(enemy.transform.position.x - player.transform.position.x);
                var y = Math.Abs(enemy.transform.position.y - player.transform.position.y);
                
                if (x > 0.3 && y > 0.3) continue;
                enemy.Kill();
            }
        }
    }
    
    private void OnComplete()
    {
        _timer = null;
    }
}

public sealed class GuillotinaStats : BaseCharacterData
{
    public GuillotinaStats()
    {
        CharName = "Guillotina";
        SurName = "Ravera";
        TextureName = "Guillotina_Ravera_walk";
        SpriteName = "Guillotina_Ravera_walk_01.png";
        PortraitName = "p_guillotina.png";
        Description = "Goth anti-facist lady with the power of guillotines. Gains 2.5% area and 0.5% speed every level. Can instant kill non boss enemies in a short range on enemy touch (15 second cooldown).";
        StartingWeapon = WeaponType.SONG;
        Luck += 0.2f;
        MaxHp -= 10;
        MoveSpeed += 0.1f;
        Power += 0.3f;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        obj.Add("onEveryLevelUp", new JObject(
            new JProperty("area", 0.025f),
            new JProperty("speed", 0.005f)
        ));
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}