using UnityEngine;

using UnityEditor;
using UnityTools.EditorTools;
using UnityTools;
namespace MinimalUI {
    [System.Serializable] public class UIRectSelectableParameters : UIBaseParameters<UIRectSelectableParameters> {
        
        public bool useTextHeight;
        [Tooltip("Width / Y Buffer")] public Vector2 elementsSize = new Vector2(.3f, .175f);
        public TextAnchor textAlignment = TextAnchor.MiddleLeft;
        public UITextParameters textParams;
        public float flairIndent = .125f;
        public float flairSize = .2f;
        [MinimalUISprite] public int unselectedSprite;




        public override void CopyFrom(UIRectSelectableParameters other) {
            useTextHeight = other.useTextHeight;
            elementsSize = other.elementsSize;
            unselectedSprite = other.unselectedSprite;
            textAlignment = other.textAlignment;
            flairIndent = other.flairIndent;
            flairSize = other.flairSize;
            UIBaseParameters.CopyParameters(ref textParams, other.textParams);
                
        }
    }

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UIRectSelectableParameters))]
    class UIRectSelectableParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "elementsSize", false);
            DrawProp(ref position, property, "useTextHeight", false);
            DrawProp(ref position, property, "unselectedSprite", false);
            DrawProp(ref position, property, "textAlignment", false);
            DrawProp(ref position, property, "textParams", true);
            DrawProp(ref position, property, "flairIndent", false);
            DrawProp(ref position, property, "flairSize", false);
        }
        static readonly string[] subClassNames = new string[] { "textParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 6; }  
    }
    #endif

    [ExecuteInEditMode] public class UIRectSelectable : UISelectable {

        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     image.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();          
        }

        public UIRectSelectableParameters parameters;

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            base.UpdateElementLayout();//firstBuild, needsSize);

            int selectedSprite = selected ? 0 : parameters.unselectedSprite;
            image.mySprite = selectedSprite;

            image.useOutline = false;
            image.useShadow = false;

            image.UpdateElementLayout();//firstBuild, false);

            UIBaseParameters.CopyParameters(ref uiText.parameters, parameters.textParams);
            uiText.useOutline = false;
            
            // if (uiText.text.alignment != parameters.textAlignment)
            uiText.SetAnchor( parameters.textAlignment, true, Vector2.zero );
            
            float flairOffset = parameters.flairIndent * 2 + parameters.flairSize;
            uiText.rectTransform.sizeDelta = new Vector2((parameters.elementsSize.x - flairOffset * 2) / parameters.textParams.scale, 0);
            
            float p = uiText.rectTransform.pivot.x;
            float o = p == 1 ? -flairOffset : (p == 0 ? flairOffset : 0);
            uiText.rectTransform.anchoredPosition = new Vector2(o, 0);
            

            // uiText.useOutline = !selected;
            uiText.useShadow = !selected;
            uiText.SetColorScheme(UIColorScheme.Normal, selected);
            // Vector2 textRect = 
            uiText.UpdateElementLayout();//firstBuild, true);

            if (selected) uiText.text.fontStyle = FontStyle.Bold;

            
            if (parameters.useTextHeight) {
                float textHeight = uiText.getHeight;
                rectTransform.sizeDelta = new Vector2( parameters.elementsSize.x, textHeight + parameters.elementsSize.y * 2);
            }
            else {
                rectTransform.sizeDelta = parameters.elementsSize;
            }
            
            Vector2 flairSize = Vector2.one * parameters.flairSize;
            for (int x = 0; x < flairs.Length; x++) {
                UIImage flair = flairs[x];
                if (!flair.gameObject.activeSelf) continue;

                flair.rectTransform.sizeDelta = flairSize;
                flair.rectTransform.anchoredPosition = new Vector2(x == 0 ? parameters.flairIndent : -parameters.flairIndent, 0);
                flair.mainGraphic.raycastTarget = false;
                flair.SetColorScheme(UIColorScheme.Normal, selected);
                flair.UpdateElementLayout();//firstBuild, true);
            }

            // return rectTransform.sizeDelta;
        }
    }
}
