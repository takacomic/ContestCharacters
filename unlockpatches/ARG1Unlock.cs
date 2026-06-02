using Il2CppDG.Tweening;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Tools;
using UnityEngine;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(CharacterController))]
public class ARG1Unlock
{
    private static Transform _spritesRootTransform;
    
    [HarmonyPatch(nameof(CharacterController.OnUpdate))]
    [HarmonyPostfix]
    public static void Postfix(CharacterController __instance)
    {
        if (GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterARG1Unlock").Value)) return;
        if (GM.Core.Stage.StageType != StageType.TOWER) return;
        if (!GM.Core.PlayerOptions.Config.RunWeapons.Contains(WeaponType.FOLLOWER_KNIFE2)) return;
        
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterARG1Unlock").Value);
        
        
        _spritesRootTransform = new GameObject("ShardTransform").transform;
        _spritesRootTransform.position = new Vector2(GM.Core.Stage._mainCamera.transform.position.x, GM.Core.Stage._mainCamera.transform.position.y);
        _spritesRootTransform.SetParent(GM.Core.Stage._mainCamera.transform, worldPositionStays: true);
        
        float num = MathF.PI * 2f;
        Vector3 position = GM.Core.Stage._mainCamera.transform.position;
        SpriteRenderer s = __instance.AddSprite(position.x, position.y, new Vector2(0.5f, 0f), "disk_shard1", "disk_shard1.png");
        s.enabled = false;
        s.SetDepth(2000);
        s.SetParent(_spritesRootTransform.transform);
        Vector3 localPosition = s.transform.localPosition;
        localPosition.x += Mathf.Cos(0.5f + num * 0f) * 0.59999996f;
        localPosition.y -= Mathf.Sin(0.5f + num * 0f) * 0.59999996f;
        s.transform.DOLocalMove(localPosition, 0.5f).SetEase(Ease.InOutSine).SetDelay((800f + 100f * 0f) * 0.001f)
            .OnStart(new Action(delegate
            {
                s.enabled = true;
                SoundManager.PlaySound(SfxType.ThingFound, new SoundManager.SoundConfig
                {
                    Volume = new Il2CppSystem.Nullable<float>(1),
                    Detune = -1000f,
                    Rate = 0.5f
                });
            }))
            .OnComplete(new Action(delegate
            {
                s.transform.DOScale(new Vector3(0f, 10f, 1f), 0.5f).SetEase(Ease.InOutSine).SetDelay((1100f + 0f) * 0.001f)
                    .OnComplete(new Action(delegate
                    {
                        s.enabled = false;
                    }));
            })).SetGameId();
    }
}