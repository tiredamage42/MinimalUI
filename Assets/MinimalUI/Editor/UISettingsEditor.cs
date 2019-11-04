using UnityEngine;
using UnityTools.EditorTools;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MinimalUI {

    [CustomEditor(typeof(UISettings))]
    public class UISettingsEditor : UnityEditor.Editor
    {

        static int border = 2;
        static int corner = 8;
        static int rectRes = 32;
        static int circleRes = 512;

        static float alphaSteepness = 10;
        static float baseMiddleAlpha = .75f;
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUITools.Space(3);
            EditorGUILayout.LabelField("Create Sprites:", GUITools.boldLabel);

            border = EditorGUILayout.IntField("Border Pixels", border);
            corner = EditorGUILayout.IntField("Corner Pixels", corner);
            rectRes = EditorGUILayout.IntField("Rect Res", rectRes);
        
            GUITools.Space(1);
            EditorGUILayout.LabelField("Circles:", GUITools.boldLabel);
            circleRes = EditorGUILayout.IntField("Circle Res", circleRes);
            alphaSteepness = EditorGUILayout.Slider("Alpha Steepness", alphaSteepness, 1, 10);
            baseMiddleAlpha = EditorGUILayout.Slider("Base Middle Alpha", baseMiddleAlpha, 0, 1);
            
            if (GUILayout.Button("Create Sprites")) {
                CreateSpriteTextures(target as UISettings, circleRes, rectRes, corner, border);
            }
        }

        static string GetFileName (string name, int l, int t, int r, int b) {
            return string.Format("{0}@{1}_{2}_{3}_{4}", name, l, t, r, b);
        }

        static string SaveTexture (Texture2D orig, string name, Color32[] colors) {
            orig.SetPixels32(colors);        
            orig.Apply(false); 
            string fileName =  UISpritesUtils.directory + name + ".png";
            File.WriteAllBytes(fileName, orig.EncodeToPNG()); 
            AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate);
            AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate);
            return fileName;
        }
        static Sprite SpriteFromTexture(string texture) {
            Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath( texture ).OfType<Sprite>().ToArray();
            return (sprites.Length > 0) ? sprites[0] : null;
        }

        public static void CreateSpriteTextures (UISettings settings, int circleRes, int rectRes, int corner, int border) {
            if (settings == null) return;


            settings.customSprites.Clear();

            List<string> textures = new List<string>();

            Color32 white = new Color32 (255, 255, 255, 255);
            Color32 clear = new Color32 (0, 0, 0, 0);

            Texture2D smallRectText = new Texture2D(2, 2);
            Texture2D circleTex = new Texture2D(circleRes, circleRes);
            Texture2D rectTex = new Texture2D(rectRes, rectRes);

            textures.Add(SaveTexture(smallRectText, "Rect", new Color32[4] { white, white, white, white }));

            Color32[] circleColors = new Color32[circleRes * circleRes];
            
            textures.Add(SaveTexture(circleTex, "Circle" + circleRes, CreateCircleOutline(circleRes, 0, white, clear, circleColors)));
            
            int circleBorder = 2;
            while (circleBorder <= (circleRes/4) ) {
                textures.Add(SaveTexture(circleTex, "CircleOutline" + circleRes + "_Border" + circleBorder, CreateCircleOutline(circleRes, circleBorder, white, clear, circleColors)));
                circleBorder *= 2;
            }
            
            Color32[] colors = new Color32[rectRes * rectRes];
            /*  
                ---   ---
                |       |
                
                |       |
                ---   ---
            */  
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_Corners", corner+1, corner+1, corner+1, corner+1), CreateCorners ( rectRes, corner, border, white, clear, colors)));
            /*  
                ---------
                |       |
                
                |       |
                ---------
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_TB_Corner", border + 1, corner+1, border + 1, corner+1), CreateTopBottomRectWCorner ( rectRes, corner, border, white, clear, colors)));
            /*  
                ---   ---
                |       |
                |       |
                |       |
                ---   ---
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_LR_Corner", corner+1, border+1, corner+1, border+1), CreateLeftRightRectWCorner ( rectRes, corner, border, white, clear, colors)));
            /*  
                -       -
                |       |
                |       |
                |       |
                -       -
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_LR", border + 1, 0, border + 1, 0), CreateLeftRightRect ( rectRes, border, white, clear, colors)));
            /*  
                ---------
                
                
                
                ---------
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_TB", 0, border + 1, 0, border + 1), CreateTopBottomRect ( rectRes, border, white, clear, colors)));
            /*  
                ---------
                |       |
                |       |
                |       |
                ---------
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_Outline", border + 1, border + 1, border + 1, border + 1), CreateOutlineRect ( rectRes, border, white, clear, colors)));
        
        
            // rectangle that will fit into the outline rects made above, with border width space
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_Fill", border * 2 + 1, border * 2 + 1, border * 2 + 1, border * 2 + 1), CreateFillRectangleWithSpace ( rectRes, border, white, clear, colors)));

            /*  
                
                |       
                |       
                |       
                ---------
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_Diag", border + 1, border + 1, border + 1, border + 1), CreateDiagonal ( rectRes, border, white, clear, colors)));

            /*  
                |       |
                |       |
                |       |
                ---------
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_NoTop", border + 1, border + 1, border + 1, border + 1), CreateNoTop ( rectRes, border, white, clear, colors)));

            /*  
                
                |       
                |       
                |       
                ---------
            */
            textures.Add(SaveTexture(rectTex, GetFileName("Rect_DiagCorner", corner + 1, border + 1, border + 1, corner + 1), CreateDiagonalCorner ( rectRes, border, corner, white, clear, colors)));




            circleBorder = 2;
            while (circleBorder <= (circleRes/4) ) {
                textures.Add(SaveTexture(circleTex, "CircleOutlineFilled" + circleRes + "_Border" + circleBorder, CreateCircleOutlineFilled(circleRes, circleBorder, white, clear, alphaSteepness, baseMiddleAlpha, circleColors)));
                circleBorder *= 2;
            }
            
            circleBorder = 2;
            while (circleBorder <= (circleRes/4) ) {
                textures.Add(SaveTexture(circleTex, "CircleOutlineParts" + circleRes + "_Border" + circleBorder, CreateCircleOutlineParts(circleRes, circleBorder, white, clear, circleColors)));
                circleBorder *= 2;
            }
            
            


            
            settings.customSprites.Clear();
            for (int i = 0; i < textures.Count; i++) settings.customSprites.Add (SpriteFromTexture(textures[i]));
            EditorUtility.SetDirty(settings);
                
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        static Color32[] CreateCircleOutline (int res, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            float halfRes = res * .5f;
            Vector2 maskCenter = new Vector2(halfRes, halfRes);
            float thresholdOuter = halfRes * halfRes;
            
            float thresholdInner = borderWidth == 0 ? 0 : (halfRes - borderWidth) * (halfRes - borderWidth);
            
            for(int y = 0; y < res; ++y){
                for(int x = 0; x < res; ++x){
                    float dist = Vector2.SqrMagnitude(maskCenter - new Vector2(x, y));
                    colors[x + y * res] = (dist <= thresholdOuter && dist >= thresholdInner) ? white : clear;
                }
            }
            return colors;
        }
        static Color32[] CreateCircleOutlineParts (int res, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            float halfRes = res * .5f;
            Vector2 maskCenter = new Vector2(halfRes, halfRes);
            float thresholdOuter = halfRes * halfRes;
            
            float thresholdInner = borderWidth == 0 ? 0 : (halfRes - borderWidth) * (halfRes - borderWidth);
            
            for(int y = 0; y < res; ++y){
                for(int x = 0; x < res; ++x){
                    Vector2 dir = new Vector2(x, y) - maskCenter;
                    float dist = Vector2.SqrMagnitude(dir);
                    

                    float angle = Vector2.Angle(Vector2.up, dir);
                    

                    colors[x + y * res] = (dist <= thresholdOuter && dist >= thresholdInner && (angle >= 45f && angle <= 135)) ? white : clear;
                }
            }
            return colors;
        }

        static Color32[] CreateCircleOutlineFilled (int res, int borderWidth, Color32 white, Color32 clear, float alphaSteepness, float baseMiddleAlpha, Color32[] colors) {
            float halfRes = res * .5f;
            Vector2 maskCenter = new Vector2(halfRes, halfRes);
            float thresholdOuter = halfRes * halfRes;
            
            float thresholdInner = borderWidth == 0 ? 0 : (halfRes - borderWidth) * (halfRes - borderWidth);
            
            for(int y = 0; y < res; ++y){
                for(int x = 0; x < res; ++x){
                    float dist = Vector2.SqrMagnitude(maskCenter - new Vector2(x, y));
                    bool isBorder = (dist <= thresholdOuter && dist >= thresholdInner);

                    float alpha = dist > thresholdOuter ? 0 : Mathf.Pow(dist / thresholdInner, alphaSteepness) * baseMiddleAlpha * 255;
                    colors[x + y * res] = isBorder ? white : new Color32(255,255,255,(byte)alpha);
                }
            }
            return colors;
        }


        static Color32[] CreateCorners (int res, int corner, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateCorners ( res, corner, corner, borderWidth, white, clear, colors);
        }
        
        static Color32[] CreateTopBottomRectWCorner (int res, int corner, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateSidesRectWCorner (false, res, corner, borderWidth, white, clear, colors);
        }
        
        static Color32[] CreateLeftRightRectWCorner (int res, int corner, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateSidesRectWCorner (true, res, corner, borderWidth, white, clear, colors);
        }
        
        static Color32[] CreateLeftRightRect (int res, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateSidesRect(true, res, borderWidth, white, clear, colors);
        }
        
        static Color32[] CreateTopBottomRect (int res, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateSidesRect(false, res, borderWidth, white, clear, colors);
        }
       
        static Color32[] CreateOutlineRect (int res, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateFilledRect ( res, borderWidth, clear, white, colors );
        }
        static Color32[] CreateFillRectangleWithSpace (int res, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateFilledRect ( res, borderWidth * 2, white, clear, colors);
        }
        static Color32[] CreateSidesRectWCorner (bool useX, int res, int corner, int borderWidth, Color32 white, Color32 clear, Color32[] colors) {
            return CreateCorners(res, useX ? corner : res * 2,  useX ? res * 2 : corner, borderWidth, white, clear, colors);
        }
        static Color32[] CreateFilledRect (int res, int borderWidth, Color32 insideColor, Color32 borderColor, Color32[] colors) {
            return CreateCorners ( res, res * 2, res * 2, borderWidth, borderColor, insideColor, colors);
        }
        static Color32[] CreateSidesRect (bool useX, int res, int borderWidth, Color32 borderColor, Color32 insideColor, Color32[] colors) {            
            return CreateCorners(res, !useX ? res * 2 : 0, !useX ? 0 : res * 2, borderWidth, borderColor, insideColor, colors);
        }
        static Color32[] CreateCorners (int res, int cornerX, int cornerY, int border, Color32 borderColor, Color32 insideColor, Color32[] colors) {
            for (int y = 0; y < res; y++) {
                for (int x = 0; x < res; x++) {
                    int r = res - 1;
                    bool isBorder = ((y < cornerY || y > r - cornerY) && (x < border || x > r - border)) || ((x < cornerX || x > r - cornerX) && (y < border || y > r - border));
                    colors[x + y * res] = isBorder ? borderColor : insideColor;
                }
            }
            return colors;
        }

        static Color32[] CreateNoTop (int res, int border, Color32 borderColor, Color32 insideColor, Color32[] colors) {
            for (int y = 0; y < res; y++) {
                for (int x = 0; x < res; x++) {
                    int r = res - 1;
                    bool isBorder = ( (x < border || x > r - border)) || ( (y < border ));
                    colors[x + y * res] = isBorder ? borderColor : insideColor;
                }
            }
            return colors;
        }


        static Color32[] CreateDiagonal (int res, int border, Color32 borderColor, Color32 insideColor, Color32[] colors) {
            for (int y = 0; y < res; y++) {
                for (int x = 0; x < res; x++) {
                    bool isBorder = ((y < border) || (x < border));
                    colors[x + y * res] = isBorder ? borderColor : insideColor;
                }
            }
            return colors;
        }
        static Color32[] CreateDiagonalCorner (int res, int border, int corner, Color32 borderColor, Color32 insideColor, Color32[] colors) {
            for (int y = 0; y < res; y++) {
                for (int x = 0; x < res; x++) {
                    bool isBorder = ((y < border && x < corner) || (x < border && y < corner ));
                    colors[x + y * res] = isBorder ? borderColor : insideColor;
                }
            }
            return colors;
        }
    }

    public class TexturePostProcessor : AssetPostprocessor
    {
        // void OnPreprocessTexture()
        void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter importer = assetImporter as TextureImporter;
            
            if (!importer.assetPath.Contains(UISpritesUtils.directory)) 
                return;

            if (importer.textureType == TextureImporterType.Sprite)
                return;
            
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;

            if (!importer.assetPath.Contains("@")) 
                return;

            string parameters = importer.assetPath.Split('@')[1];
            
            if (!parameters.Contains("_")) 
                return;
            
            parameters = parameters.Split('.')[0];
            
            string[] border = parameters.Split('_');
            
            if (border.Length < 4) 
                return;
            
            importer.spriteBorder = new Vector4( 
                int.Parse(border[0]), 
                int.Parse(border[1]), 
                int.Parse(border[2]), 
                int.Parse(border[3]) 
            );
        }
    }
}
