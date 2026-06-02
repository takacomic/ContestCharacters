using CoffinTech.SaveData;
using HarmonyLib;
using Il2CppDG.Tweening;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework;
using Il2CppVampireSurvivors.Framework.Particles;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Items;
using Il2CppVampireSurvivors.Objects.Pickups;
using Il2CppVampireSurvivors.Objects.Stages;
using Il2CppVampireSurvivors.Tools;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters.patches;

[HarmonyPatch(typeof(BackgroundX))]
public static class BackgroundXPatches
{
    [HarmonyPatch(nameof(BackgroundX.Create))]
    [HarmonyPrefix]
    private static void CreatePrefix(BackgroundX __instance)
    {
        if (!GM.Core.PlayerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value) && GM.Core.PlayerOptions.Config.CollectedItems.Contains(ItemType.RELIC_YELLOW) && SpellsManagerPatches._spell._activated)
        {
            var pickup = GM.Core.MakeStagePickup(new Vector2(76.6544f, -7f), ItemType.RELIC, WeaponType.VOID, 1f,
                ModOptionsData.CustomItem("ContestCharacterContestDisk").Value).Cast<PickupRelic>();
            OnDiskFound(pickup, __instance);
        }
    }

    [HarmonyPatch(nameof(BackgroundX.RemovePowers))]
    [HarmonyPostfix]
    private static void RemovePowersPostfix(BackgroundX __instance)
    {
        if (!SpellsManagerPatches._spell._activated) return;
        foreach (var player in GM.Core.AllPlayers)
        {
            player._playerStats.MoveSpeed.SetValue(1.5f);
            player._playerStats.MaxHp.SetValue(200);
            player._playerStats.Armor.SetValue(3);
            player.SetHealthToMax();
        }
        
        var list = new List<string>() { "disk_shard1", "disk_shard2", "disk_shard3", "disk_shard4", "disk_shard5", "disk_shard6" };
        
        float num = MathF.PI * 2f / (float)list.Count;
        Vector3 position = __instance._mainCamera.transform.position;
        for (int i = 0; i < list.Count; i++)
        {
            SpriteRenderer s = __instance.AddSprite(position.x, position.y, new Vector2(0.5f, 0f), list[i], list[i] + ".png");
            s.enabled = false;
            s.SetDepth(2000);
            s.SetParent(__instance._spritesRootTransform);
            Vector3 localPosition = s.transform.localPosition;
            localPosition.x += Mathf.Cos(0.5f + num * (float)i) * 0.59999996f;
            localPosition.y -= Mathf.Sin(0.5f + num * (float)i) * 0.59999996f;
            int index = i;
            s.transform.DOLocalMove(localPosition, 0.5f).SetEase(Ease.InOutSine).SetDelay((3800f + 100f * (float)i) * 0.001f)
                .OnStart(new Action(delegate
                {
                    s.enabled = true;
                    SoundManager.PlaySound(SfxType.Haha, new SoundManager.SoundConfig
                    {
                        Volume = new Il2CppSystem.Nullable<float>(0.9f),
                        Rate = 1f
                    }, 10000f, 1);
                }))
                .OnComplete(new Action(delegate
                {
                    s.transform.DOScale(new Vector3(0f, 10f, 1f), 0.5f).SetEase(Ease.InOutSine).SetDelay((1100f + (float)index) * 0.001f)
                        .OnComplete(new Action(delegate
                        {
                            s.enabled = false;
                        }));
                }))
                .SetGameId();
        }
    }

    [HarmonyPatch(nameof(BackgroundX.SetupCharacterAnimation))]
    [HarmonyPostfix]
    private static void SetupCharacterAnimationPostfix(BackgroundX __instance, CharacterController character)
    {
        if (!SpellsManagerPatches._spell._activated) return;
        SpriteRenderer sr = character._spriteRenderer;
        if (sr == null) return;

        // AI
        // Dark to light, left to right — these map to the sprite's luminance range
        Color[] undeadPalette = new Color[]
        {
            new Color(0.00f, 0.00f, 0.00f), // keep black outlines
            new Color(0.05f, 0.05f, 0.08f), // near black
            new Color(0.08f, 0.05f, 0.12f), // darkest purple
            new Color(0.15f, 0.05f, 0.20f), // deep purple shadow
            new Color(0.15f, 0.18f, 0.12f), // dark rot green
            new Color(0.20f, 0.12f, 0.25f), // dark purple mid
            new Color(0.18f, 0.25f, 0.12f), // mid rot green
            new Color(0.35f, 0.15f, 0.40f), // purple mid
            new Color(0.20f, 0.38f, 0.15f), // sickly green mid
            new Color(0.40f, 0.30f, 0.45f), // muted purple
            new Color(0.25f, 0.45f, 0.20f), // green mid
            new Color(0.45f, 0.45f, 0.40f), // desaturated grey-purple
            new Color(0.50f, 0.45f, 0.30f), // pale bone
            new Color(0.35f, 0.55f, 0.25f), // bright rot green
            new Color(0.60f, 0.58f, 0.50f), // light bone grey
            new Color(0.55f, 0.65f, 0.35f), // light green highlight
            new Color(0.65f, 0.65f, 0.40f), // pale sickly yellow
            new Color(0.70f, 0.70f, 0.55f), // light bone highlight
            new Color(0.80f, 0.82f, 0.70f), // near white with green tint
            new Color(0.88f, 0.90f, 0.85f), // white with slight green tint
        };
        PaletteSwapper.Initialize();
        PaletteSwapper.ApplySwap(character._spriteAnimation, sr, undeadPalette);
        SpellsManagerPatches._spell._activated = false;
    }

    private static void OnDiskFound(PickupRelic found, BackgroundX instance)
    {
        var frame = new Il2CppSystem.Collections.Generic.List<string>();
        frame.Add("CCC_01.png");
        frame.Add("CCC_02.png");
        frame.Add("CCC_03.png");
        frame.Add("CCC_04.png");
        frame.Add("CCC_05.png");
        frame.Add("CCC_06.png");
        frame.Add("CCC_07.png");
        frame.Add("CCC_08.png");
        frame.Add("CCC_09.png");
        frame.Add("CCC_10.png");
        var frame2 = new Il2CppSystem.Collections.Generic.List<string>();
        frame2.Add("CCC_11.png");
        frame2.Add("CCC_12.png");
        frame2.Add("CCC_13.png");
        frame2.Add("CCC_14.png");
        frame2.Add("CCC_15.png");
        frame2.Add("CCC_16.png");
        frame2.Add("CCC_17.png");
        frame2.Add("CCC_18.png");
        frame2.Add("CCC_19.png");
        frame2.Add("CCC_20.png");
        
        Vector3 position = found.transform.position;
        GameObject obj = new GameObject("YellowPxfEmitter");
        obj.transform.position = position;
        GameObject gameObject = obj;
        gameObject.transform.SetParent(found.transform, worldPositionStays: true);
        instance._particleEmitterManager = gameObject.AddComponent<ParticleEmitterManager>();
        ParticleSystemConfig config = new ParticleSystemConfig("CCC")
        {
            _frame = frame,
            _x = new ParticleSystem.MinMaxCurve(position.x - 0.64f, position.x + 0.64f),
            _y = position.y - instance._camBounds.size.y - 0.32f,
            _lifespan = 6000f,
            _speedY = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(-100f, -300f)),
            _alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(0.9f, 0.8f)),
            _scaleY = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 2f)),
            _quantity = new Il2CppSystem.Nullable<int>(1)
        };
        instance._particleEmitterManager.CreateEmitter(config, null, "ContestPfxEmitter1");
        ParticleSystemConfig config2 = new ParticleSystemConfig("CCC")
        {
            _frame = frame2,
            _x = new ParticleSystem.MinMaxCurve(position.x - 0.64f, position.x + 0.64f),
            _y = position.y - instance._camBounds.size.y - 0.32f,
            _lifespan = 7000f,
            _speedY = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(-100f, -300f)),
            _alpha = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 0f)),
            _scaleY = new Il2CppSystem.Nullable<ParticleSystem.MinMaxCurve>(new ParticleSystem.MinMaxCurve(1f, 2f)),
            _quantity = new Il2CppSystem.Nullable<int>(1),
            _frequency = new Il2CppSystem.Nullable<float>(200f),
            _blendMode = new Il2CppSystem.Nullable<BlendMode>(BlendMode.Add)
        };
        instance._particleEmitterManager.CreateEmitter(config2, null, "ContestPfxEmitter2");
        instance._particleEmitterManager.SetDepth(-1000);
        instance._particleEmitterManager.StartAllEmitters();
    }
}