using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
using UnityEditor;

namespace MinimalUI {
    [System.Serializable] public class UITextGroupParameters : UIBaseParameters<UITextGroupParameters> {
        public float spaceOffset;
        public UITextParameters textParameters;
        public bool switchDirection;

        public override void CopyFrom(UITextGroupParameters other) {
            spaceOffset = other.spaceOffset;
            switchDirection = other.switchDirection;
            UIBaseParameters.CopyParameters(ref textParameters, other.textParameters);
        }
    }
    
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UITextGroupParameters))]
    class UITextGroupParametersDrawer : Internal.ParametersDrawer {

        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "spaceOffset", false);
            DrawProp(ref position, property, "switchDirection", false);
            DrawProp(ref position, property, "textParameters", true);
            
        }

        static readonly string[] subClassNames = new string[] { "textParameters" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 2; }   
    }

    #endif
    public class UITextGroup : UIComponent
    {

        public UITextGroupParameters parameters;

        UIText[] _allTexts;
        public UIText[] allTexts { get { return gameObject.GetComponentsInChildrenIfNull<UIText>(ref _allTexts, true); } }

        // public override void OnComponentClose() {
        //     for (int i = 0; i < allTexts.Length; i++) {
        //         allTexts[i].OnComponentClose();
        //     }
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }
        

        public override void UpdateElementLayout(){//bool firstBuild) {
            
            Vector2 pivot = rectTransform.pivot;
            float offsetSoFar = 0;
            for (int i = 0; i < allTexts.Length; i++) {
                UIText t = allTexts[parameters.switchDirection ? (allTexts.Length - 1) - i : i];
                UIBaseParameters.CopyParameters(ref t.parameters, parameters.textParameters);
                t.SetAnchor(pivot);
                
                // Vector2 tSize = 
                t.UpdateElementLayout();//firstBuild);
                t.rectTransform.anchoredPosition = new Vector2(pivot.x == 1 ? -offsetSoFar : offsetSoFar, 0);
                
                if (i < allTexts.Length - 1){
                    float tWidth = t.getWidth;
                    offsetSoFar += tWidth + parameters.spaceOffset;
                }
            }
            // return Vector2.zero;
        }
    }
}
