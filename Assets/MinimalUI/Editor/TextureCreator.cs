using UnityEngine;

using UnityEditor;
using UnityTools.EditorTools;
using System.IO;

namespace MinimalUI
{

    public static class UISpritesUtils {
        public static string directory = "Assets/MinimalUI/Icons/";

        // public static Texture2D SaveTexture (string name, int width, int height, Color32[] colors) {
        //     Texture2D tex = new Texture2D(width, height);
        //     tex.SetPixels32(colors);        
        //     tex.Apply(false); 

        //     // AssetDatabase.CreateAsset(tex, directory + name + ".png");

        //     string fileName = directory + name + ".png";

        //     File.WriteAllBytes(fileName, tex.EncodeToPNG()); 
        //     AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate);

        //     return AssetDatabase.LoadAssetAtPath<Texture2D>(fileName);

        //     // return tex;
        // }
    }

    public class TextureInverter : ScriptableWizard
    {
        public bool invertColors;
        public bool invertAlpha;
        public Texture2D[] textures;
    
        [MenuItem("GameObject/Minimal UI/Internal/Texture Invert")]
        static void CreateWizard() {
            ScriptableWizard.DisplayWizard<TextureInverter>("Invert Texture").SetSize(256, 256).CenterWindow();
        }

        void InvertTexture (Texture2D t) {
            Color32[] colors = t.GetPixels32();
            for (int i = 0; i < colors.Length; i++) {

                Color32 c = colors[i];

                colors[i] = new Color32(
                    (byte)(invertColors ? 255 - c.r : c.r),
                    (byte)(invertColors ? 255 - c.g : c.g),
                    (byte)(invertColors ? 255 - c.b : c.b),
                    (byte)(invertAlpha  ? 255 - c.a : c.a)
                );
            }
            SavePNG(t.name + "_inverted", t.width, t.height, colors);
        }

        void SavePNG (string name, int width, int height, Color32[] colors) {
            Texture2D tex = new Texture2D(width, height);
            tex.SetPixels32(colors);        
            tex.Apply(false); 
            string fileName = UISpritesUtils.directory + name + ".png";
            File.WriteAllBytes(fileName, tex.EncodeToPNG()); 
            AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        void OnWizardCreate () {
            if (!invertColors && !invertAlpha) {
                Debug.LogWarning("No Inverts Specified...");
                return;
            }
            for (int i =0 ; i < textures.Length; i++) InvertTexture(textures[i]);
            AssetDatabase.Refresh();
        }
    }
}