using UnityEngine;
using UnityEditor;

namespace MutCommon
{
    public class SpriteFolderAssetProcessor : AssetPostprocessor
    {
        [SerializeField]
        public string Keyword = "Sprite";

        bool IsSprite => assetPath.ToLowerInvariant().IndexOf(Keyword.ToLowerInvariant()) != -1;
        void OnPreprocessTexture() {
            var ti = (TextureImporter)assetImporter;
            if (!IsSprite) return;
            Debug.Log("processing as sprite" + assetPath);
            ti.textureType = TextureImporterType.Sprite;
            ti.alphaIsTransparency = true;
            ti.isReadable = true;
            // ti.spriteBorder = Vector4.zero;
        }

        void OnPostProcessSprites(Texture2D texture, Sprite[] sprites) {
            Debug.Log("Sprites: " + sprites.Length);
        }

        void OnPostProcessTexture(Texture2D texture) {
            Debug.Log("Texture2D: (" + texture.width + "x" + texture.height + ")");
        }
    }
}