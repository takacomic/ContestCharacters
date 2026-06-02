using System.Runtime.InteropServices;
using CoffinTech.Extensions;
using Il2Cpp;
using Il2CppCoherence;
using Il2CppInterop.Runtime;
using Il2CppVampireSurvivors.App.Objects;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.Phaser;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Objects.Props;
using Il2CppVampireSurvivors.Objects.Stages;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(Background1))]
public class BetaUnlock
{
    private static MapToken? _mapToken = null;
    private static Destructible? _destructible = null;
    private static bool setup;
    private static bool killed;
    private static Action<IntPtr> _baseMyMethod;
    [HarmonyPatch(nameof(Background1.Create))]
    [HarmonyPostfix]
    private static void CreatePostfix(Background1 __instance)
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterBetaUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterRubriccoUnlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.FOREST) return;
        setup = true;
        _mapToken = null;
        _destructible = null;
    }

    [HarmonyPatch(nameof(Background1.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdatePostfix(GameManager __instance)
    {
        if (!setup) return;
        if (killed) return;
        if (GM.Core.Player.WeaponsManager.ActiveEquipment.ToList().Count(e => e._equipmentType == WeaponType.WHIP) != 1) return;
        if (_destructible == null)
        {
            
            _destructible = GM.Core.Stage.MakeDestructible(PropType.FOSCARI_SEAL_1, new Vector2(207.4719f, -103.7576f));
            _destructible.IgnoreForcedMovement = true;
            if (_mapToken == null)
            {
                _mapToken = new MapToken();
                GM.Core.MapTokens.Add(_mapToken);
            }
            _mapToken.x = _destructible.position.x;
            _mapToken.y = _destructible.position.y;
            _destructible.Cast<ArcadeSprite>().setFrame(SpriteManager.GetSprite("beta_seal.png"));
        }
    }
    
    [HarmonyPatch(typeof(PropFoscariSeal1), nameof(PropFoscariSeal1.GetDamaged))]
    [HarmonyPrefix]
    private static void GetDamagedPrefix(PropFoscariSeal1 __instance, float value, HitVfxType showHitVFX, float knockbackMul, WeaponType damageType, bool hasKnockback)
    {
        if (!setup) return;
        
        __instance._hp -= value;
        if (__instance._hp <= 0f)
        {
            if (GM.Core.IsStageHost)
            {
                __instance._isDead = true;
                if (GM.Core.IsOnlineMultiplayer)
                {
                    __instance._coherenceSync.SendCommand(new Action<long>(__instance.DestroySeal), MessageTarget.All, OnlineStageManager.Instance.GetStartingOnlineClientFrame());
                }
                else
                {
                    __instance.OnDestroyed();
                }
            }
        }
        else
        {
            __instance.OnGetDamaged(showHitVFX);
        }
    }
    
    [HarmonyPatch(typeof(PropFoscariSeal1), nameof(PropFoscariSeal1.OnDestroyed))]
    [HarmonyPrefix]
    private static bool OnDestroyedPrefix(PropFoscariSeal1 __instance)
    {
        if (!setup) return true;
        
        __instance._gameManager.SignalBus.Fire(new GameplaySignals.DestructibleDestroyed
        {
            destructible = _destructible
        });
        
        __instance._blinkTimer?.Cancel();
        __instance._isDead = true;
        if (__instance._gameManager.Stage.HasLights && __instance._light != null)
        {
            __instance._light.enabled = false;
            __instance._light = null;
            __instance._gameManager.ReturnLight(__instance);
        }
        if (__instance.body != null)
        {
            __instance.body._enable = false;
        }
        PhysicsManager.Instance._destructiblesGroup.remove(__instance);
        __instance.Release();
        
        _destructible = null;
        killed = true;
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterBetaUnlock").Value);
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterBeta").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
        return false;
    }
}