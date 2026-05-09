using CoffinTech.Utils;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Signals;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class MortisController : ModCharacterController
{
    private CharacterController? _characterController;
    private int _followerNum;
    private int _followerTotal;

    public override void AfterFullInit(CharacterController instance)
    {
        _characterController = instance;
        _followerNum = 0;
        _followerTotal = 0;
        
        instance._signalBus.Subscribe<GameplaySignals.EnemyKilledImmediateSignal>(
            new Action(MakeEnemyFollower));
    }

    private void MakeEnemyFollower()
    {
        if (_characterController?._gameManager.GetLatestKilledEnemyThatCanBeFollower() == null) return;
        if (_characterController._gameManager.GetNumAliveEnemyFollowers(_characterController) >= 5) return;
        if (Random.Range(1, 101) > (10 * _characterController.PCurse())) return;
            
        var enemyFollower = _characterController._gameManager.AddLastEnemyFollower(_characterController);
            
        if (enemyFollower == null) return;
        if (enemyFollower.HasSetName) return;
            
        var enemyData = enemyFollower.CurrentCharacterData;
            
        enemyData.charName = $"{enemyData.charName} {_followerNum}";
        enemyFollower.HasSetName = true;
        _followerNum++;
        _followerTotal++;

        if (_followerTotal % 60 == 0)
            GainStat();
    }
    
    private void GainStat()
    {
        var random = UnityEngine.Random.Range(1, 16);
        
        switch (random)
        {
            case 1:
                _characterController.PlayerStats.Amount += 0.1f;
                break;
            case 2:
                _characterController.PlayerStats.Area += 0.1f;
                break;
            case 3:
                _characterController.PlayerStats.Armor += 0.1f;
                break;
            case 4:
                _characterController.PlayerStats.Cooldown -= 0.05f;
                break;
            case 5:
                _characterController.PlayerStats.Duration += 0.1f;
                break;
            case 6:
                _characterController.PlayerStats.Greed += 0.1f;
                break;
            case 7:
                _characterController.PlayerStats.Growth += 0.1f;
                break;
            case 8:
                _characterController.PlayerStats.Luck += 0.1f;
                break;
            case 9:
                _characterController.PlayerStats.Magnet += 0.1f;
                break;
            case 10:
                _characterController.PlayerStats.MaxHp += 5f;
                break;
            case 11:
                _characterController.PlayerStats.MoveSpeed += 0.1f;
                break;
            case 12:
                _characterController.PlayerStats.Power += 0.1f;
                break;
            case 13:
                _characterController.PlayerStats.Regen += 0.1f;
                break;
            case 14:
                _characterController.PlayerStats.Revivals += 0.1f;
                break;
            case 15:
                _characterController.PlayerStats.Speed += 0.1f;
                break;
        }
    }
}

public sealed class MortisStats : BaseCharacterData
{
    public MortisStats()
    {
        CharName = "Mortis";
        SurName = "Surmanski";
        TextureName = "Mortis_Surmanski_walk";
        SpriteName = "Mortis_Surmanski_walk_01.png";
        Description = "Can bring dead monsters back to life. Every 60 revived monsters increases a random permanent stat.";
        StartingWeapon = WeaponType.HOLYBOOK;
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