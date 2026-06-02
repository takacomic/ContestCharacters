using System.Reflection;
using CoffinTech.SaveData;
using CoffinTech.Utils;
using ContestCharacters.characters.top16;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects.Algorithm;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.secret;

public class LuigiMortisController : ModCharacterController
{
    private static CharacterController? _characterController;
    private int _followerNum;
    private int _followerTotal;
    private bool _spawnFollowerNextFrame;
    
    public override void AfterFullInit(CharacterController instance)
    {
        _spawnFollowerNextFrame = true;
        
        _characterController = instance;
        //Mortis
        if (instance._skinType != SkinType.DEFAULT2)
        {
            _followerNum = 0;
            _followerTotal = 0;

            instance._signalBus.Subscribe<GameplaySignals.EnemyKilledImmediateSignal>(
                new Action(MakeEnemyFollower));
        }
        else
        {
            //Luigi
            var postfixes = new List<MethodInfo>
            {
                typeof(Roast).GetMethod("GetTaken"),
                typeof(DuoHarmony).GetMethod("RoastGetTaken"),
                typeof(Pickup_TP_WallChicken).GetMethod("GetTaken"),
                typeof(DuoHarmony).GetMethod("WallChickenGetTaken")
            };
            HarmonyPatching.Patch(typeof(DuoHarmony), postfix: postfixes);
            instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
        }
    }
    public override void OnUpdate(CharacterController instance)
    {
        if (!_spawnFollowerNextFrame) return;
        _spawnFollowerNextFrame = false;
        CharacterController characterController;
        if (instance._skinType != SkinType.DEFAULT2)
        {
            characterController = GM.Core.AddFollower(ModOptionsData.CustomCharacter("ContestCharacterLuigi").Value, instance, AIType.Defensive, manualLevelups: true);
        }
        else
        {
            characterController = GM.Core.AddFollower(ModOptionsData.CustomCharacter("ContestCharacterMortis").Value, instance, AIType.Defensive, manualLevelups: true);
        }
        
        
        if (characterController == null) return;
        characterController.TrackedByCamera = true;
        characterController.SetPermanentInvulnerability(on: true);
        characterController.CountsAsMainCharacterForRevivals = false;
        characterController.MaxWeaponCount = instance.MaxWeaponCount;
        characterController.HealthBar.SetScale(0.00125f);
    }

    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(DuoHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    private static void FoodBuffs()
    {
        var x = _characterController.transform.localScale.x;
        if (_characterController._cachedTransform.localScale.x < 6f)
            _characterController.setScale( x *= 1.02f, new Il2CppSystem.Nullable<float>());
        _characterController._playerStats.Power._val += 0.01f;
        _characterController._playerStats.Area._val += 0.01f;
        _characterController._playerStats.MaxHp._val += 2;
        _characterController._playerStats.MoveSpeed._val -= 0.02f;
        _characterController._playerStats.Speed._val -= 0.01f;
    }

    private void MakeEnemyFollower()
    {
        if (_characterController?._gameManager.GetLatestKilledEnemyThatCanBeFollower() == null) return;
        if (_characterController._gameManager.GetNumAliveEnemyFollowers(_characterController) >= 8) return;
        if (Random.Range(1, 81) > (10 * _characterController.PCurse())) return;
            
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
    
    private static class DuoHarmony
    {
        public static void RoastGetTaken(Roast __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return;
            FoodBuffs();
        }
    
        public static void WallChickenGetTaken(Pickup_TP_WallChicken __instance)
        {
            if (__instance._targetPlayer._characterType != _characterController?._characterType) return;
            FoodBuffs();
        }
    }
}

public sealed class LuigiMortisStats : BaseCharacterData
{
    public LuigiMortisStats()
    {
        CharName = "Mortis & Luigi";
        TextureName = "Mortis_Surmanski_walk_duo";
        SpriteName = "Mortis_Surmanski_walk_01.png";
        PortraitName = "p_mortis_luigi.png";
        Skins = new JArray();
        WalkFrameRate = 8;
        WalkingFrames = 4;
        JObject skin = new JObject
        {
            ["name"] = "Luigi & Mortis",
            ["suffix"] = "(Luigi)",
            ["charSelTexture"] = "luigi_duo_portrait",
            ["charSelFrame"] = "luigi_duo_portrait_01.png",
            ["textureName"] = "Mortis_Surmanski_walk_duo",
            ["spriteName"] = "chef_luigi_walk_01.png",
            ["description"] = "Gets stronger (and larger) the more he eats",
            ["startingWeapon"] = "HELLFIRE",
            ["walkingFrames"] = 4,
            ["walkFrameRate"] = 8,
            ["unlocked"] = true,
            ["skinType"] = "DEFAULT2"
        };
        JObject skin2 = new JObject()
        {
            ["name"] = "Mortis & Luigi",
            ["suffix"] = "(Mortis)",
            ["charSelTexture"] = "mortis_duo_portrait",
            ["charSelFrame"] = "mortis_duo_portrait_01.png",
            ["textureName"] = "Mortis_Surmanski_walk_duo",
            ["spriteName"] = "Mortis_Surmanski_walk_01.png",
            ["description"] = "Can bring dead monsters back to life. Every 60 revived monsters increases a random permanent stat.",
            ["startingWeapon"] = "HOLYBOOK",
            ["walkingFrames"] = 4,
            ["walkFrameRate"] = 8,
            ["maxHp"] = -40,
            ["moveSpeed"] = 0.15f,
            ["power"] = -0.25f,
            ["cooldown"] = -0.15f,
            ["unlocked"] = true,
            ["skinType"] = "DEFAULT"
        };
        Skins.Add(skin2);
        Skins.Add(skin);
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