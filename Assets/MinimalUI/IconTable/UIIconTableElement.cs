using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;

using UnityEditor;

namespace MinimalUI {

    [System.Serializable] public class UIIconTableElementParameters : UIBaseParameters<UIIconTableElementParameters> {
        public float iconSize = 1;
        public override void CopyFrom(UIIconTableElementParameters other) {
            iconSize = other.iconSize;
        }
    }
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIIconTableElementParameters))]
    class UIIconTableElementParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "iconSize", false);
            
        }
        static readonly string[] subClassNames = null;//new string[] { "iconParameters" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 1; }
    }
    #endif

    public class UIIconTableElement : UIComponent
    {
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }

        public UIIconTableElementParameters parameters;
        UIImage[] _allImages;
        UIImage[] allImages { get { return gameObject.GetComponentsInChildrenIfNull<UIImage>(ref _allImages, true); } }
        UIImage iconImage { get { return allImages[1]; } }
// public override void OnComponentClose() {
//             // base.OnComponentClose();
//             for (int i = 0; i < allImages.Length; i++)
//                 allImages[i].OnComponentClose();
// }

        public void SetIconColorScheme (UIColorScheme scheme) {
            iconImage.SetColorScheme(scheme, false);
        }

        public void SetIconSprite (Sprite sprite) {
            iconImage.useReadySprite = false;
            iconImage.mainGraphic.sprite = sprite;
        }

        public override void UpdateElementLayout(){//bool firstBuild) {
            allImages[0].rectTransform.sizeDelta = Vector2.one * parameters.iconSize;
            iconImage.useReadySprite = false;
            for (int i = 0; i < allImages.Length; i++) {
                allImages[i].UpdateElementLayout();//firstBuild);
            }
            // return Vector2.zero;
        }
    }
}
