using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Graphics;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Pools;
using Il2CppVampireSurvivors.Objects.Projectiles;
using Il2CppVampireSurvivors.Objects.Weapons;
using Il2CppVampireSurvivors.Signals;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ContestCharacters.characters.top16;

public class SpecimenController : ModCharacterController
{
    private static string projName = "";
    private static string textName = "";
    private static bool yeet;
    private static int frameSet;
    private static float cooldown;
    private const float cooldownSetTime = (1f / 16f);
    private static CharacterController _characterController;
    public override void AfterFullInit(CharacterController instance)
    {
        projName = "";
        textName = "";
        yeet = false;
        cooldown = 0;
        frameSet = 0;
        _characterController = instance;
        var postfixes = new List<MethodInfo>();
        postfixes.Add(typeof(GemCannonProjectile).GetMethod("InitProjectile"));
        postfixes.Add(typeof(SpecimenHarmony).GetMethod("GemCannonProjectileInitProjectile"));
        instance._spriteAnimation.AddAnimation("throw", SpriteManager.GetAnimationFrames("specimen_throw_", 1, 4, new Vector2(0.5f, 0f), "specimen_throw", 2, instance.RespectAnimationXPivots), 16, false);
        HarmonyPatching.Patch(typeof(SpecimenHarmony), postfix: postfixes);
        instance._signalBus.Subscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }
    
    private void ExitToRecap()
    {
        HarmonyPatching.UnPatch(typeof(SpecimenHarmony));
        _characterController._signalBus.TryUnsubscribe<GameplaySignals.ResetGameSessionSignal>(new Action(ExitToRecap));
    }

    public override void OnUpdate(CharacterController instance)
    {
        if (instance.SpriteAnimation.CurrentAnim != "throw") return;
        cooldown += PauseSystem.DeltaTime;
        if (cooldown >= cooldownSetTime)
        {
            if (frameSet < 4)
            {
                instance.SpriteAnimation.ApplySpriteFrame(instance.SpriteAnimation.GetCurrentAnimation()
                    ._frames[frameSet++]);
            }
            else
            {
                frameSet = 0;
                instance.SpriteAnimation.Play("walk");
            }
            cooldown = 0;
        }
    }

    public override void EnemyOnPlayerOverlap(EnemyController instance, CharacterController player)
    {
        if (Random.RandomRange(1, 101) > (4)) return;
        if (instance.EnemyType.ToString().Contains("BOSS") && player._skinType == SkinType.DEFAULT) return;
        projName = instance._spriteRenderer.sprite.name;
        textName = instance._spriteRenderer.sprite.texture.name;
        var gemCannonWeapon = player.WeaponsManager.GetWeaponByType(WeaponType.WINDOW2, searchHidden: true).TryCast<GemCannonWeapon>();
        if (gemCannonWeapon != null)
        {
            gemCannonWeapon.GemValue = instance.CurrentHealth() * player.PGrowth();
            gemCannonWeapon.GemFrame = projName;
            gemCannonWeapon.Fire();
            if (instance.SpriteAnimation.CurrentAnim != "throw")
            {
                player._spriteAnimation.Play("throw");
            }
            yeet = true;
        }

        instance.Despawn();
    }

    private static class SpecimenHarmony
    {
        public static void GemCannonProjectileInitProjectile(GemCannonProjectile __instance, BulletPool pool, Weapon weapon, int index)
        {
            if(yeet)
            {
                Sprite sprite = SpriteManager.GetSprite(projName, textName);
                __instance.setFrame(sprite);
                yeet = false;
            }
        }
    }
}

public sealed class SpecimenStats : BaseCharacterData
{
    public SpecimenStats()
    {
        CharName = "Specimen";
        SurName = "40";
        TextureName = "specimen_40_walk";
        SpriteName = "specimen_40_walk_01.png";
        PortraitName = "p_specimen.png";
        Description = "Occasionally absorbs and throws enemies";
        StartingWeapon = WeaponType.LAUREL;
        MoveSpeed -= 0.3f;
        Area += 0.3f;
        Growth += 0.5f;
        Regen += 0.1f;
    }

    public override string JsonText()
    {
        var jArray = new JArray();
        var obj = JObject.FromObject(this);
        obj.Add("hiddenWeapons", new JArray("WINDOW2"));
        jArray.Add(obj);
        jArray.Add(Level20);
        jArray.Add(Level21);
        jArray.Add(Level40);
        jArray.Add(Level41);
        return JsonConvert.SerializeObject(jArray);
    }
}