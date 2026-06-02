using CoffinTech.SaveData;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Signals;
using Il2CppVampireSurvivors.Spells;
using Il2CppZenject;
using MelonLoader;

namespace ContestCharacters.spells;
public class SpellDirecterWillNotLikeThis
{
    private PlayerOptions? _playerOptions;

    private SignalBus? _signalBus;

    private SpellsManager? _spellsManager;
    internal bool _activated;

    public void Init(PlayerOptions player, SignalBus signalBus, SpellsManager spellsManager)
    {
        _playerOptions = player;
        _signalBus = signalBus;
        _spellsManager = spellsManager;
    }
    
    public void Start()
    {
        //_spellsManager.AddSpell(this);
        SoundManager.StopMusic(BgmType.BGM_Secret);
        _playerOptions.Config.SelectedStage = StageType.STAGEX;
        _playerOptions.Config.SelectedCharacter = ModOptionsData.CustomCharacter("ContestCharacterZeta").Value;
        if (MultiplayerManager.Instance.IsMultiplayer)
        {
            var localPlayerSlots = MultiplayerManager.Instance.GetLocalPlayerSlots();
            for (int i = 0; i < localPlayerSlots.Count; i++) 
            { 
                localPlayerSlots[i].SelectedCharacter = ModOptionsData.CustomCharacter("ContestCharacterZeta").Value;
            }
        }
        _activated = true;
        _signalBus.Fire<UISignals.LaunchGameplaySignal>();
        
    }
    
    public void Activate()
    {
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterARG6Unlock").Value);
        GM.Core.Player.CurrentCharacterData.moveSpeed += 1.25f;
        GM.Core.Player.Magnet.RefreshSize();
    }

    public void Deactivate()
    {
    }
}