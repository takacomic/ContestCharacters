using System.Reflection;
using CoffinTech.Utils;
using HarmonyLib;
using MelonLoader;

namespace ContestCharacters;

public static class HarmonyPatching
{
    private static HarmonyLib.Harmony _harmonyInstance = Melon<ContestCharactersMod>.Instance.HarmonyInstance;
    private static Dictionary<Type, List<MethodInfo>> _methodsDictionary = new ();

    internal static void Patch(Type type, List<MethodInfo>? prefix = null, List<MethodInfo>? postfix = null)
    {
        if (prefix == null && postfix != null)
        {
            if (!_methodsDictionary.TryAdd(type, postfix)) return;
            for (var i = 0; i < postfix.Count; i += 2)
            {
                _harmonyInstance.Patch(postfix[i], postfix: new HarmonyMethod(type, postfix[i + 1].Name));
            }
        }
        else if (prefix != null && postfix == null)
        {
            if (!_methodsDictionary.TryAdd(type, prefix)) return;
            for (var i = 0; i < prefix.Count; i += 2)
            {
                _harmonyInstance.Patch(prefix[i], prefix: new HarmonyMethod(type, prefix[i + 1].Name));
            }
        }
        else if (prefix != null && postfix != null)
        {
            if (!_methodsDictionary.TryAdd(type, prefix)) return;
            for (var i = 0; i < prefix.Count; i += 2)
            {
                MelonLogger.Msg($"Patching {prefix[i]}: with {prefix[i + 1]}");
                _harmonyInstance.Patch(prefix[i], prefix: new HarmonyMethod(type, prefix[i + 1].Name));
            }
            for (var i = 0; i < postfix.Count; i += 2)
            {
                MelonLogger.Msg($"Patching {postfix[i]}: with {postfix[i + 1]}");
                _harmonyInstance.Patch(postfix[i], postfix: new HarmonyMethod(type, postfix[i + 1].Name));
            }

            _methodsDictionary.Remove(type);
            prefix.AddRange(postfix);
            if (!_methodsDictionary.TryAdd(type, prefix)) return;
        }
    }

    internal static void UnPatch(Type type)
    {
        var methods = _methodsDictionary[type];
        for (var i = 0; i < methods.Count; i+=2)
        {
            _harmonyInstance.Unpatch(methods[i], methods[i+1]);
        }
        _methodsDictionary.Remove(type);
    }
}