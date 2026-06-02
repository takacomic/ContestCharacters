using Il2CppDG.Tweening;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Geom;
using Il2CppVampireSurvivors.Framework.Phaser;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Stages;
using Il2CppVampireSurvivors.Tools;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(Background2))]
public class GuillotinaUnlock
{
    private static Circle _heartCircle;
    private static PhaserSprite _heartSprite;
    private static bool heartActive;
    
    [HarmonyPatch(nameof(Background2.Create))]
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterGuillotinaUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterLuigiUnlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.LIBRARY) return;
        if (GM.Core.Player.CharacterType == CharacterType.GIOVANNA)
        {
            heartActive = true;
            //_checkHeartDistance = true;
            _heartCircle = new Circle(137.44f, -10.56f, 0.32f);
            _heartSprite = PhaserWorld.Instance.AddPhaserSprite(new Vector2(137.44f, -10.56f), "items", "HeartRuby");
            _heartSprite.setDepth(-1997);
            _heartSprite.transform.DOScale(2f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutExpo)
                .SetGameId();
        }
    }

    [HarmonyPatch(nameof(Background2.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdatePostfix(Background2 __instance)
    {
        if (!heartActive) return;
        foreach (CharacterController allPlayer in GM.Core.AllPlayers)
        {
            Vector3 position = allPlayer.transform.position;
            if (!_heartCircle.Contains(position)) continue;
            //_checkHeartDistance = false;
            if (GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterGuillotinaUnlock").Value))
            {
                InGameUnlocks.InGameSecretCharacterUnlock(ModOptionsData.CustomCharacter("ContestCharacterGuillotina").Value);
            }
            _heartSprite.Rend.DOFade(0f, 2.5f).SetGameId();
        }
    }
}