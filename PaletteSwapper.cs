using Il2CppVampireSurvivors.Graphics;
using MelonLoader;
using UnityEngine;

namespace ContestCharacters;

//Mostly AI gen'd
public static class PaletteSwapper
{
    internal static Material _baseMaterial;
    internal static Shader _shader;
    internal static Il2CppAssetBundle _bundle;
    private static bool _initialized = false;

    public static void Initialize()
    {
        if (_initialized) return;

        if (_bundle == null)
        {
            MelonLogger.Error("Bundle failed to load.");
            return;
        }

        _shader = _bundle.LoadAsset<Shader>("assets/shaders/paletteluminance.shader");
        if (_shader == null)
        {
            MelonLogger.Error("Shader is null after LoadAsset.");
            return;
        }

        _baseMaterial = new Material(_shader);
        if (_baseMaterial == null)
        {
            MelonLogger.Error("Material creation failed.");
            return;
        }

        _initialized = true;
    }
    
    public static void ApplySwap(SpriteAnimation sa, SpriteRenderer sr, Color[] swapColors)
    {
        if (sr?.sprite == null) return;

        Il2CppSystem.Collections.Generic.List<Sprite> swapped = GenerateSwappedSpriteSheetCPU(sa._animations["walk"]._frames, swapColors, sr.sprite.texture);
        if (swapped == null) return;

        // Restore the original material first so the sprite renders normally
        sr.sharedMaterial = new Material(Shader.Find("Shader Graphs/BaseSpriteShader"));

        // Just swap the sprite asset — no material change needed
        sa._animations["uwalk"]._frames = swapped;
        sr.sprite = sa._animations["walk"]._frames[0];

    }

    public static Il2CppSystem.Collections.Generic.List<Sprite> GenerateSwappedSpriteSheetCPU(Il2CppSystem.Collections.Generic.List<Sprite> originalSprites, Color[] swapColors, Texture2D source)
{
    if (originalSprites == null || originalSprites._size == 0) return null;
    
    source = MakeReadable(source);

    // All sprites share the same source texture

    // Check texture is readable
    try { source.GetPixel(0, 0); }
    catch
    {
        MelonLogger.Error("Texture is not readable — cannot do CPU swap.");
        return null;
    }

    Il2CppSystem.Collections.Generic.List<Sprite> results = new Il2CppSystem.Collections.Generic.List<Sprite>(originalSprites._size);

    for (int s = 0; s < originalSprites._size; s++)
    {
        Sprite original = originalSprites[s];
        Rect rect = original.textureRect;

        // Grab just this sprite's pixels from the sheet
        Color[] pixels = source.GetPixels(
            (int)rect.x,
            (int)rect.y,
            (int)rect.width,
            (int)rect.height
        );

        // Apply luminance swap
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a < 0.01f) continue;

            float lum = pixels[i].r * 0.299f + pixels[i].g * 0.587f + pixels[i].b * 0.114f;
            int index = Mathf.Clamp(Mathf.RoundToInt(lum * (swapColors.Length - 1)), 0, swapColors.Length - 1);
            pixels[i] = new Color(swapColors[index].r, swapColors[index].g, swapColors[index].b, pixels[i].a);
        }

        // Each sprite gets its own texture
        Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        result.filterMode = FilterMode.Point;
        result.wrapMode = TextureWrapMode.Clamp;
        result.SetPixels(pixels);
        result.Apply();

        results.Add(Sprite.Create(
            result,
            new Rect(0, 0, rect.width, rect.height),
            new Vector2(
                original.pivot.x / rect.width,
                original.pivot.y / rect.height
            ),
            original.pixelsPerUnit
        ));

    }

    return results;
}
    
    private static Texture2D MakeReadable(Texture2D source)
    {
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Point;

        Graphics.Blit(source, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readable.filterMode = FilterMode.Point;
        readable.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        readable.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        return readable;
    }

    /// <summary>
    /// Bake the luminance swap into a new Sprite instead of a live material.
    /// </summary>

    private static Texture2D ColorsToTexture(Color[] colors)
    {
        Texture2D tex = new Texture2D(colors.Length, 1, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }
}