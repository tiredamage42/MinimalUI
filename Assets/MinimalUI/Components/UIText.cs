using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
namespace MinimalUI {

    [System.Serializable] public class UITextParameters : UIBaseParameters<UITextParameters> {
        public float scale = .0005f;
        public FontStyle fontStyle = FontStyle.Normal;
        public HorizontalWrapMode horizontalWrap = HorizontalWrapMode.Overflow;
        public float lineSpace = 1f;
        public bool alignByGeometry = true;

            
        public override void CopyFrom(UITextParameters other) {
            scale = other.scale;
            fontStyle = other.fontStyle;
            horizontalWrap = other.horizontalWrap;
            lineSpace = other.lineSpace;
            alignByGeometry = other.alignByGeometry;
        }
    }
    
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UITextParameters))]
    class UITextParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "scale", false);
            DrawProp(ref position, property, "fontStyle", false);
            DrawProp(ref position, property, "horizontalWrap", false);
            DrawProp(ref position, property, "lineSpace", false);
            DrawProp(ref position, property, "alignByGeometry", false);   
        }
        protected override string[] SubClassNames (SerializedProperty prop) { return null; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 5; }
    }
    #endif
    /*
        used to keep text in ui consistent
    */
    [RequireComponent(typeof(Text))] 
    [ExecuteInEditMode] public class UIText : UIGraphic<Text>
    {
        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }

        public float sizeMultiplier = 1;
        public UITextParameters parameters;
        public Text text { get { return mainGraphic; } }

        public bool textEmpty { get { return string.IsNullOrEmpty(text.text); } }// !text.enabled; } }
        
        public 
        float getHeight { get { return text.preferredHeight * parameters.scale; } }
        public 
        float getWidth { get { return text.preferredWidth * parameters.scale; } }
        
        public void SetText (string text) {
            if (this.text.text != text) this.text.text = text;
        
            if (string.IsNullOrEmpty(text)) {
                if (this.text.enabled)
                    this.text.enabled = false;
            }
            else {
                if (!this.text.enabled) this.text.enabled = true;
            }
        }

        public void SetAnchor(Vector2 anchor) {
            TextAnchor textAnchor = TextAnchor.LowerCenter;
            if (anchor.x == 0) {
                if      (anchor.y == 0.0f) textAnchor = TextAnchor.LowerLeft;
                else if (anchor.y == 0.5f) textAnchor = TextAnchor.MiddleLeft;
                else if (anchor.y == 1.0f) textAnchor = TextAnchor.UpperLeft;
            }
            else if (anchor.x == .5f) {
                if      (anchor.y == 0.0f) textAnchor = TextAnchor.LowerCenter;
                else if (anchor.y == 0.5f) textAnchor = TextAnchor.MiddleCenter;
                else if (anchor.y == 1.0f) textAnchor = TextAnchor.UpperCenter;
            }
            else if (anchor.x == 1) {
                if      (anchor.y == 0.0f) textAnchor = TextAnchor.LowerRight;
                else if (anchor.y == 0.5f) textAnchor = TextAnchor.MiddleRight;
                else if (anchor.y == 1.0f) textAnchor = TextAnchor.UpperRight;
            }

            if (text.alignment != textAnchor)
                text.alignment = textAnchor;
            SetPivotAndAnchor(anchor);
        }

        public void SetAnchor(TextAnchor textAnchor, bool adjustAnchor, Vector2 offsets) {
            if (text.alignment != textAnchor)
                text.alignment = textAnchor;

            if (adjustAnchor) AdjustAnchorSet(offsets);
        }

        void AdjustAnchorSet (Vector2 offsets){
            
            bool isMiddle = text.alignment == TextAnchor.MiddleCenter || text.alignment == TextAnchor.MiddleRight || text.alignment == TextAnchor.MiddleLeft;
            bool isLower = text.alignment == TextAnchor.LowerCenter || text.alignment == TextAnchor.LowerRight || text.alignment == TextAnchor.LowerLeft;
            bool isUpper = text.alignment == TextAnchor.UpperCenter || text.alignment == TextAnchor.UpperRight || text.alignment == TextAnchor.UpperLeft;
            
            bool isCenter = text.alignment == TextAnchor.MiddleCenter || text.alignment == TextAnchor.LowerCenter || text.alignment == TextAnchor.UpperCenter;
            bool isRight = text.alignment == TextAnchor.MiddleRight || text.alignment == TextAnchor.LowerRight || text.alignment == TextAnchor.UpperRight;
            bool isLeft = text.alignment == TextAnchor.MiddleLeft || text.alignment == TextAnchor.LowerLeft || text.alignment == TextAnchor.UpperLeft;

            Vector2 anchorPivot = Vector2.zero;

            if (isMiddle) anchorPivot.y = .5f;
            else if (isLower) anchorPivot.y = offsets.y;
            else if (isUpper) anchorPivot.y = 1 - offsets.y;

            if (isCenter) anchorPivot.x = .5f;
            else if (isRight) anchorPivot.x = 1 - offsets.x;
            else if (isLeft) anchorPivot.x = offsets.x;

            SetPivotAndAnchor(anchorPivot);
        }
            

        protected override Vector2 OutlineEffect() { return settings.outlineEffect; }
        protected override Vector2 ShadowEffect() { return settings.shadowEffect; }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            base.UpdateElementLayout();//firstBuild, needsSize);

            if (text.raycastTarget)
                text.raycastTarget = false;
            
            if (text.verticalOverflow != VerticalWrapMode.Overflow)
                text.verticalOverflow = VerticalWrapMode.Overflow;
            
            if (text.font != settings.font)
                text.font = settings.font;
            
            if (text.fontSize != settings.defaultFontSize)
                text.fontSize = settings.defaultFontSize;
            
            if (text.resizeTextForBestFit)
                text.resizeTextForBestFit = false;
            
            if (text.supportRichText)
                text.supportRichText = false;

            
            if (parameters == null)
                return;// Vector2.zero;

            if (text.alignByGeometry != parameters.alignByGeometry)
                text.alignByGeometry = parameters.alignByGeometry;
            
            if (text.fontStyle != parameters.fontStyle)
                text.fontStyle = parameters.fontStyle;
            
            if (text.lineSpacing != parameters.lineSpace)
                text.lineSpacing = parameters.lineSpace;
            
            if (text.horizontalOverflow != parameters.horizontalWrap)
                text.horizontalOverflow = parameters.horizontalWrap;


            float s = parameters.scale * sizeMultiplier;
            if (transform.localScale.x != s)
                transform.localScale = Vector3.one * s;

            // if (needsSize)
            //     return new Vector2(text.preferredWidth * parameters.scale, text.preferredHeight * parameters.scale);

            // return Vector2.zero;
        }
    }
}

