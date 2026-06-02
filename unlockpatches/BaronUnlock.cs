using CoffinTech.Extensions;
using Il2Cpp;
using Il2CppDG.Tweening;
using Il2CppRewiredConsts;
using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Framework.Actions;
using Il2CppVampireSurvivors.Framework.Geom;
using Il2CppVampireSurvivors.Framework.PhaserTweens;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Stages;
using UnityEngine;
using Il2CppVampireSurvivors.Framework.Particles;
using MelonLoader;

namespace ContestCharacters.unlockpatches;

[HarmonyPatch(typeof(BackgroundGreenAcres))]
public class BaronUnlock
{
    private static Circle? _fixedCircle;
    private static float _elapsedTime;
    private static float _elapsedTime2;
    
    private static SpriteRenderer _groundFx;

    private static Il2CppSystem.Collections.Generic.List<Transform> _group;

    private static Il2CppSystem.Collections.Generic.List<Transform> _group2;
    
    private static Il2CppSystem.Collections.Generic.List<Transform> _group3;

    private static Il2CppSystem.Collections.Generic.List<Transform> _group4;

    private static Transform _group1Parent;

    private static Transform _group2Parent;

    private static Transform _group3Parent;

    private static Transform _group4Parent;
    
    [HarmonyPatch(nameof(BackgroundGreenAcres.Create))]
    [HarmonyPostfix]
    private static void BaronUnlockPostfix(BackgroundGreenAcres __instance)
    {
        _elapsedTime = 0f;
        _elapsedTime2 = 0f;
        if (!GM.Core._playerOptions.Config.CollectedItems.Contains(ModOptionsData.CustomItem("ContestCharacterContestDisk").Value)) return;
        if (GM.Core._playerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterBaronUnlock").Value)) return;
        if (!GM.Core.PlayerOptions.Config.Secrets.Contains(ModOptionsData.CustomSecret("ContestCharacterGourdtellioUnlock").Value)) return;
        if (GM.Core.PlayerOne.CharacterType != ModOptionsData.CustomCharacter("ContestCharacterGourdtellio").Value) return;
        
        float y = 0f;
        float x = 204.79999f;
        _fixedCircle = new Circle(x, y, 3f);
        AddGroundFx(x, y, __instance);
        AddFlowers(x, y, __instance);
    }

    [HarmonyPatch(nameof(BackgroundGreenAcres.OnUpdate))]
    [HarmonyPostfix]
    private static void OnUpdatePostfix(BackgroundGreenAcres __instance)
    {
        if (_fixedCircle == null) return;
        if (!_fixedCircle.Contains(GM.Core.PlayerOne.transform.position)) return;
        if (GM.Core.Player.WeaponsManager.ActiveEquipment.ToList().Count(e => e._equipmentType == WeaponType.SILF3) != 1)
        {
	        var i = 0;
	        if (GM.Core.Player.WeaponsManager.ActiveEquipment.ToList().Any(e => e._equipmentType == WeaponType.SILF))
	        {
		        i++;
	        }
	        if (GM.Core.Player.WeaponsManager.ActiveEquipment.ToList().Any(e => e._equipmentType == WeaponType.SILF2))
	        {
		        i++;
	        }
	        if (i < 2)
	        {
		        return;
	        }
        }
	    _elapsedTime += PauseSystem.DeltaTime;
	    _elapsedTime2 += PauseSystem.DeltaTime;

        if (!(_elapsedTime >= 10f) || GM.Core.PlayerOptions.Config.UnlockedCharacters.Contains(ModOptionsData.CustomCharacter("ContestCharacterBaron").Value)) return;
        GM.Core.PlayerOptions.UnlockSecret(ModOptionsData.CustomSecret("ContestCharacterBaronUnlock").Value);
        var characterType = ModOptionsData.CustomCharacter("ContestCharacterBaron").Value;
        InGameUnlocks.InGameSecretCharacterUnlock(characterType);
        UnityEngine.Object.Destroy(_groundFx.gameObject);
        _groundFx = null;
        UnityEngine.Object.Destroy(_group1Parent.gameObject);
        _group1Parent = null;
        _group = null;
        UnityEngine.Object.Destroy(_group2Parent.gameObject);
        _group2Parent = null;
        _group2 = null;
        UnityEngine.Object.Destroy(_group3Parent.gameObject);
        _group3Parent = null;
        _group3 = null;
        UnityEngine.Object.Destroy(_group4Parent.gameObject);
        _group4Parent = null;
        _group4 = null;
    }
    
    private static void AddGroundFx(float x, float y, BackgroundGreenAcres __instance)
		{
			_groundFx = __instance.AddSprite(x, y, null, "UnityCircle");
				_groundFx.SetTint(65280u).SetAlpha(0.1f)
				.SetVisible(visible: true)
				.SetBlendMode(BlendMode.Add)
				.SetScale(400f)
				.SetName("GroundFx");
		}

		 static void AddFlowers(float x, float y, BackgroundGreenAcres __instance)
		{
			Circle circle = new Circle(x, y, 2f);
			Circle circle2 = new Circle(x, y, 1.7f);
			Circle circle3 = new Circle(x, y, 1.3f);
			Circle circle4 = new Circle(x, y, 1f);
			_group = new Il2CppSystem.Collections.Generic.List<Transform>();
			_group2 = new Il2CppSystem.Collections.Generic.List<Transform>();
			_group3 = new Il2CppSystem.Collections.Generic.List<Transform>();
			_group4 = new Il2CppSystem.Collections.Generic.List<Transform>();
			_group1Parent = new GameObject("Group1Parent").transform;
			_group1Parent.SetParent(__instance.transform);
			_group2Parent = new GameObject("Group2Parent").transform;
			_group2Parent.SetParent(__instance.transform);
			_group3Parent = new GameObject("Group3Parent").transform;
			_group3Parent.SetParent(__instance.transform);
			_group4Parent = new GameObject("Group4Parent").transform;
			_group4Parent.SetParent(__instance.transform);
			for (int j = 0; j < 36; j++)
			{
				SpriteRenderer spriteRenderer = _group1Parent.gameObject.AddSprite(x, y, "cornplant", "cornplant");
				_group.Add(spriteRenderer.transform);
				SpriteRenderer spriteRenderer2 = _group2Parent.gameObject.AddSprite(x, y, "cornplant", "cornplant");
				_group2.Add(spriteRenderer2.transform);
				SpriteRenderer spriteRenderer3 = _group3Parent.gameObject.AddSprite(x, y, "cornplant", "cornplant");
				_group3.Add(spriteRenderer3.transform);
				SpriteRenderer spriteRenderer4 = _group4Parent.gameObject.AddSprite(x, y, "cornplant", "cornplant");
				_group4.Add(spriteRenderer4.transform);
			}
			Actions.PlaceOnCircle(_group, circle, new Il2CppSystem.Nullable<float>(0), new Il2CppSystem.Nullable<float>(360));
			Actions.PlaceOnCircle(_group2, circle2, new Il2CppSystem.Nullable<float>(5), new Il2CppSystem.Nullable<float>(365));
			Actions.PlaceOnCircle(_group3, circle3, new Il2CppSystem.Nullable<float>(10), new Il2CppSystem.Nullable<float>(370));
			Actions.PlaceOnCircle(_group4, circle4, new Il2CppSystem.Nullable<float>(15), new Il2CppSystem.Nullable<float>(375));
		}
}