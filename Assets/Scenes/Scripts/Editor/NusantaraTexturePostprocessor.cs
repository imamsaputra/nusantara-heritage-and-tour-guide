using UnityEditor;

/// Otomatis set semua PNG di folder Sprites sebagai Sprite (2D and UI) tanpa kompresi.
public class NusantaraTexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("NusantaraSplash/Sprites")) return;
        var ti = (TextureImporter)assetImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.alphaIsTransparency = true;
        ti.mipmapEnabled = false;
        ti.filterMode = UnityEngine.FilterMode.Bilinear;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.maxTextureSize = 2048;
    }
}
