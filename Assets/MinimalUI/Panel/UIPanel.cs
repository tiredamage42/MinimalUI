using UnityEngine;

using UnityEditor;
using UnityTools.EditorTools;
using UnityTools;
namespace MinimalUI {

    [System.Serializable] public class UIPanelParameters : UIBaseParameters<UIPanelParameters> {
        public bool useBackground = true;
        public float borderSize = 1;
        [MinimalUISprite] public int borderSprite;

        public override void CopyFrom(UIPanelParameters other) {
            useBackground = other.useBackground;
            borderSize = other.borderSize;
            borderSprite = other.borderSprite;
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIPanelParameters))] class UIPanelParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "useBackground", false);
            DrawProp(ref position, property, "borderSize", false);
            DrawProp(ref position, property, "borderSprite", false);
        }
        protected override string[] SubClassNames (SerializedProperty prop) { return null; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 3; }
    }
    #endif

    [ExecuteInEditMode] public class UIPanel : UIComponent
    {
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }

        public UIColorScheme colorScheme;
        public UIColorScheme backgroundColorScheme;

        public void SetColorScheme (UIColorScheme colorScheme, UIColorScheme backgroundColorScheme) {
            this.colorScheme = colorScheme;
            this.backgroundColorScheme = backgroundColorScheme;
            UpdateElementLayout();//false);
        }

        public UIPanelParameters parameters;

        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     for (int i = 0; i < images.Length; i++)
        //         images[i].OnComponentClose();
        // }
        UIImage[] _images;
        UIImage[] images { get { return gameObject.GetComponentsInChildrenIfNull<UIImage>(ref _images, true); } }
        
        public override void UpdateElementLayout (){//bool firstBuild) {
            images[0].mainGraphic.enabled = parameters.useBackground;

            if (parameters.useBackground) {

                // images[0].colorScheme = parameters.backgroundColorScheme;
                images[0].colorScheme = backgroundColorScheme;
                
                images[0].useOutline = false;
                images[0].useShadow = false;
                images[0].useDark = true;
                
            }

            // images[1].colorScheme = parameters.colorScheme;
            images[1].colorScheme = colorScheme;
            
            images[1].imageScale = parameters.borderSize;
            images[1].mySprite = parameters.borderSprite;
            
            for (int i = 0; i < images.Length; i++) 
                images[i].UpdateElementLayout();//firstBuild);


            // return rectTransform.sizeDelta;
        }
    }
}
