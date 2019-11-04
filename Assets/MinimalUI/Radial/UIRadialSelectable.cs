using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools;
using UnityEditor;
using UnityTools.EditorTools;

using UnityEngine.UI;

namespace MinimalUI {
    [System.Serializable] public class UIRadialSelectableParameters : UIBaseParameters<UIRadialSelectableParameters> {
        public UITextParameters textParams;
        public float textOffset = 0; 
        [Range(0,1)] public float selectionSizeOffset = .1f;       

        public override void CopyFrom(UIRadialSelectableParameters other) {
            textOffset = other.textOffset;
            selectionSizeOffset = other.selectionSizeOffset;
            UIBaseParameters.CopyParameters(ref textParams, other.textParams);
        }

    }

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UIRadialSelectableParameters))]
    class UIRadialSelectableParametersDrawer : Internal.ParametersDrawer {

        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "textParams", true);
            DrawProp(ref position, property, "textOffset", false);
            DrawProp(ref position, property, "selectionSizeOffset", false);
        }
        static readonly string[] subClassNames = new string[] { "textParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 2; }
        
        
    }

    #endif


    [ExecuteInEditMode] public class UIRadialSelectable : UISelectable {

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

        public UIRadialSelectableParameters parameters;


        // UIImage _image;
        // UIImage image { get { return gameObject.GetComponentInChildrenIfNull<UIImage>(ref _image, true); } }
        // StretchToFit _stretch;
        // StretchToFit stretch { get { return gameObject.GetComponentIfNull<StretchToFit>(ref _stretch); } }
        
        RectTransform _transformToFit;
        RectTransform transformToFit {
            get {
                if (_transformToFit == null && transform.parent != null) _transformToFit = transform.parent.GetComponent<RectTransform>();
                return _transformToFit;
            }
        }

        public override void UpdateElementLayout(){//bool firstBuild) {
            // base.UpdateElementLayout();

            if (transformToFit != null) {
                rectTransform.sizeDelta = transformToFit.sizeDelta + Vector2.one * parameters.selectionSizeOffset;
            }

            image.overrideColor = true;
            image.UpdateElementLayout();//firstBuild);
            image.mainGraphic.type = Image.Type.Filled;
            
            base.UpdateElementLayout();//firstBuild);
            

            UIBaseParameters.CopyParameters(ref uiText.parameters, parameters.textParams);

            // uiText.UpdateElementLayout();
            
            
            // Vector2 textSize = 
            uiText.UpdateElementLayout();//firstBuild);

            calculatedTextHeight = uiText.getHeight;
            
            uiText.transform.localPosition = new Vector3(0, parameters.textOffset + rectTransform.sizeDelta.x * .5f + textHeight * .5f, 0);
            // return textSize;
            // return Vector2.zero;




                
        }

        public float textHeight { get { return calculatedTextHeight; } }
        float calculatedTextHeight;
    }
}

