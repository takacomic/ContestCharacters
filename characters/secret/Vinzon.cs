using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.DLC;
using Il2CppVampireSurvivors.Objects.Characters;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.secret;

public class VinzonController : ModCharacterController
{
    private List<WeaponType> _weapons = new()
    {
        WeaponType.MAGIC_MISSILE,
        WeaponType.FIREBALL,
        WeaponType.LIGHTNING,
        WeaponType.MISSPELL,
        WeaponType.ICELANCE,
        WeaponType.FOLLOWER_KNIFE1
    };
    private Dictionary<WeaponType, WeaponType> _evoWeapons = new()
    {
        { WeaponType.MAGIC_MISSILE, WeaponType.HOLY_MISSILE },
        { WeaponType.FIREBALL, WeaponType.HELLFIRE},
        { WeaponType.LIGHTNING, WeaponType.LOOP },
        { WeaponType.MISSPELL, WeaponType.MISSPELL2 },
        { WeaponType.ICELANCE, WeaponType.ICELANCE },
        { WeaponType.FOLLOWER_KNIFE1, WeaponType.FOLLOWER_KNIFE2 }
    };
    private Dictionary<WeaponType, WeaponType> _tpWeapons = new()
    {
        { WeaponType.MAGIC_MISSILE, WeaponType.TP_SUMMON_SPIRIT2 },
        { WeaponType.FIREBALL, WeaponType.TP_AURABLAST_WEAPON2},
        { WeaponType.LIGHTNING, WeaponType.TP_SWORD_BROTHERS2 },
        { WeaponType.MISSPELL, WeaponType.MISSPELL2 },
        { WeaponType.ICELANCE, WeaponType.ICELANCE },
        { WeaponType.FOLLOWER_KNIFE1, WeaponType.FOLLOWER_KNIFE2 }
    };
    
    private List<ArcanaType> _arcanas = new()
    {
        ArcanaType.T14_JEWELS,
        ArcanaType.T19_FIRE,
        ArcanaType.T02_TWILIGHT,
        ArcanaType.T05_CRASH,
        ArcanaType.T12_OUT_OF_TIME,
        ArcanaType.T17_PAINTING
    };
    private List<ArcanaType> _darcanas = new()
    {
        ArcanaType.D01_SAPPHIRE_MIST,
        ArcanaType.D03_BREAD_ANATHEMA,
        ArcanaType.D05_PALE_DIAMOND,
        ArcanaType.D06_BOLERO,
        ArcanaType.D10_FROM_THE_FUTURE,
        ArcanaType.D12_CRYSTAL_CRIES,
        ArcanaType.D13_MAD_MOON,
        ArcanaType.D18_VICTORIAN_HORROR,
        ArcanaType.D21_JETBLACK
    };
    
    private Dictionary<ArcanaType, ArcanaType> _evoArcanas = new()
    {
        //{ ArcanaType.T14_JEWELS, },
        //{ ArcanaType.T19_FIRE, },
        //{ ArcanaType.T02_TWILIGHT, },
        { ArcanaType.T05_CRASH, ArcanaType.D05_PALE_DIAMOND},
        { ArcanaType.T12_OUT_OF_TIME, ArcanaType.D12_CRYSTAL_CRIES},
        //{ ArcanaType.T17_PAINTING, }
    };
    
    private List<WeaponType> _skinUnlockWeapons = new()
    {
        WeaponType.HOLYWATER,
        WeaponType.BORA,
        WeaponType.TP_HYDROSTORM,
        WeaponType.TP_HYDROSTORM2,
        WeaponType.LIGHTNING,
        WeaponType.LOOP,
        WeaponType.SILF,
        WeaponType.SILF2,
        WeaponType.SILF3,
        WeaponType.ROBBA,
        WeaponType.SANTAJAVELIN,
        WeaponType.SANTAJAVELIN2,
        WeaponType.EX_MAGISTONE1,
        WeaponType.EX_MAGISTONE2,
        WeaponType.FLASHARROW2,
        WeaponType.PRISMATICMISS,
        WeaponType.PRISMATICMISS2,
        WeaponType.C1_GARBA1,
        WeaponType.C1_GARBA2,
        WeaponType.TP_EARTH1,
        WeaponType.TP_EARTH2,
        WeaponType.TP_PENDULUM_WEAPON,
        WeaponType.TP_CLOCKTOWER_WEAPON,
        WeaponType.TP_ELEVATOR_WEAPON,
        WeaponType.EME_AXE1,
        WeaponType.EME_AXE2
    };
    
    private WeaponType _weapon;
    private ArcanaType _arcana;
    private bool _set;
    private int _skinUnlockCount;
 
    public override void AfterFullInit(CharacterController instance)
    {
        if (instance._maxWeaponCount > 3)
            instance._maxWeaponCount = 3;

        switch (instance._skinType)
        {
            case SkinType.DEFAULT:
            {
                var i = Random.RandomRangeInt(0, 6);
                _weapon = _weapons[i];
                _arcana = _arcanas[i];
                break;
            }
            case SkinType.CROCI_NOCROSS:
                _weapon = _weapons[0];
                _arcana = _arcanas[0];
                break;
            case SkinType.DEFAULT2:
                _weapon = _weapons[1];
                _arcana = _arcanas[1];
                break;
            case SkinType.EMPTY:
                _weapon = _weapons[2];
                _arcana = _arcanas[2];
                break;
            case SkinType.ADVENTURE1:
                _weapon = _weapons[3];
                _arcana = _arcanas[3];
                break;
            case SkinType.FINO_DARK:
                _weapon = _weapons[4];
                _arcana = _arcanas[4];
                break;
            case SkinType.FINO_EYE:
                _weapon = _weapons[5];
                _arcana = _arcanas[5];
                break;
        }

        GM.Core.AddHiddenWeapon(_weapon, instance);
        GM.Core.ArcanaManager.ActiveArcanas.Add(_arcana);
        GM.Core.ArcanaManager.TriggerArcana(_arcana);
        
    }

    public override void OnUpdate(CharacterController instance)
    {
        if (GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterVinzonSkinUnlock").Value)) return;
        SkinUnlock();
    }

    public override void LevelUp(CharacterController instance)
    {
        if (instance.Level != 100) return;

        var weapon = _evoWeapons[_weapon];
        if (DlcSystem.LoadedDlc.ContainsKey(DlcType.ThosePeople))
            weapon = _tpWeapons[_weapon];
        GM.Core.ArcanaManager.ActiveArcanas.Remove(_arcana);
        var arcana = _evoArcanas.TryGetValue(_arcana, out var arcanaT) ? arcanaT : _darcanas[Random.RandomRangeInt(0, 9)];
        
        GM.Core.ArcanaManager.ActiveArcanas.Add(arcana);
        GM.Core.ArcanaManager.TriggerArcana(arcana);
        GM.Core.AddHiddenWeapon(weapon, instance);
        GM.Core.RemoveHiddenWeaponFromPlayer(_weapon, instance);
    }

    private void SkinUnlock()
    {
        var enemyCount = GM.Core._playerOptions.Config.RunEnemies;
        if (!_set)
        {
            foreach (var weapon in _skinUnlockWeapons)
            {
                if (!GM.Core._playerOptions.Config.RunWeapons.Contains(weapon)) continue;
                _set = true;
                _skinUnlockCount = enemyCount;
                break;
            }
            return;
        }
        
        if (enemyCount - _skinUnlockCount < 10000) return;
        if (!GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterVinzonSkinUnlock").Value)) return;
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterVinzon").Value;
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.DEFAULT2);
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.CROCI_NOCROSS);
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.EMPTY);
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.ADVENTURE1);
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.FINO_DARK);
        InGameUnlocks.InGameSecretCharacterSkinUnlock(characterType, SkinType.FINO_EYE);
    }
}