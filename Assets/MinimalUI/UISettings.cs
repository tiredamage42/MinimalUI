using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools.GameSettingsSystem;
using UnityTools.EditorTools;
using UnityTools;
using UnityEditor;
using System.Linq;
using UnityEngine.Video;

namespace MinimalUI {

    [CreateAssetMenu(menuName="Minimal UI/Internal/UI Settings", fileName="UISettings")]
    public class UISettings : GameSettingsObject
    {
        [Header("Intro Videos")]
        public VideoClip mainMenuBackgroundVideo;
        public VideoClip introVideo;


        public ActionHintInitializer actionHintsInitializer;

        [Header("Prefabs")]
        public UIMessageElement messageElement;
        public UIIconTableElement iconTableElement;
        public UISelectable rectButton, radialButton;

        [Header("Default Actions")]
        [Action] public int cancelAction;
        [Action] public int submitAction;
        [NeatArray] public NeatIntArray verticalAxes;
        [NeatArray] public NeatIntArray horizontalAxes;
        
        [Header("Text")]
        public Font font;
        public int defaultFontSize = 300;
        public Vector2 outlineEffect = new Vector2(10, 10);
        public Vector2 shadowEffect = new Vector2(10, -10);


        // [Header("Graphics")]
        // public Vector2 imageOutlineEffect = new Vector2(.02f, .02f);
        // public Vector2 imageShadowEffect = new Vector2(.04f, -.04f);
        
        [Header("Colors")]

        public Vector3 rgbDarkenMultiplier = new Vector3(.33f,.33f,.33f);
        [Range(0, 255)] public int darkenAlpha = 250;


        public Color32 shadowEffectColor = Color.black;
        public Color32 blackColor = Color.black;
        public Color32 whiteColor = Color.white;
        public Color32 mainLightColor = Color.green;//, mainDarkColor = Color.green;
        public Color32 warningLightColor = Color.yellow;//, warningDarkColor = Color.yellow;
        public Color32 invalidLightColor = Color.red;//, invalidDarkColor = Color.red;
        public ActionHint debugActionHintUI;
        public List<Sprite> customSprites = new List<Sprite>();

    }


    public class MinimalUISpriteAttribute : PropertyAttribute { }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(MinimalUISpriteAttribute))]
    public class MinimalUISpriteAttributeDrawer : PropertyDrawer
    {
        static UISettings _settings;
        static UISettings settings {
            get {
                if (_settings == null) _settings = GameSettings.GetSettings<UISettings>();
                return _settings;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            if (settings != null) {
                int selected = property.intValue;// -1;
                
                int c = settings.customSprites.Count;

                // Object o = property.objectReferenceValue;

                string[] allNames = new string[c];
                for (int i = 0; i < c; i++) {
                    Sprite sprite = settings.customSprites[i];
                    allNames[i] = sprite.name.Contains("@") ? sprite.name.Split('@')[0] : sprite.name;
                    // if (sprite == o) selected = i;
                }

                // position.width *= .5f;

                selected = EditorGUI.Popup(position, label.text, selected, allNames);


                // if (selected != -1) {

                    if (selected != property.intValue) {
                        property.intValue = selected;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    // property.objectReferenceValue = settings.customSprites[selected];
                // }

                // position.x += position.width;
                // EditorGUI.PropertyField(position, property, GUITools.noContent, true);// label, true);
            }
            else {
                EditorGUI.PropertyField(position, property, label, true);
            }
            
            EditorGUI.EndProperty();
        }
    }

#endif
}
