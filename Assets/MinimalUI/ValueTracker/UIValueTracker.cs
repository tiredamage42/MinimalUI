using UnityEngine;
using UnityEngine.UI;


using UnityEditor;
using UnityTools.EditorTools;
using UnityTools;
namespace MinimalUI {

    /* 
        slider for hud visuals (eg health bar) 
    */
    [System.Serializable] public class UIValueTrackerParameters : UIBaseParameters<UIValueTrackerParameters> {
        public float fillBorderOffset = .02f;
        public bool isVertical;
        public bool flipTextAndValue;
        public Vector2 trackerSize = new Vector2(1, .1f);
        public UITextParameters textParameters;
        [Range(-1,1)] public float textOffset = .05f;
        public UIPanelParameters panelParameters;

        public override void CopyFrom(UIValueTrackerParameters other) {
            
            textOffset = other.textOffset;
            trackerSize = other.trackerSize;
            flipTextAndValue = other.flipTextAndValue;
            isVertical = other.isVertical;
            fillBorderOffset = other.fillBorderOffset;
        
            UIBaseParameters.CopyParameters(ref textParameters, other.textParameters);
            UIBaseParameters.CopyParameters(ref panelParameters, other.panelParameters);
            
        }
    }
        
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIValueTrackerParameters))] class UIValueTrackerParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "fillBorderOffset", false);
            
            DrawProp(ref position, property, "isVertical", false);
            DrawProp(ref position, property, "flipTextAndValue", false);
            DrawProp(ref position, property, "trackerSize", false);
            DrawProp(ref position, property, "textParameters", true);
            DrawProp(ref position, property, "textOffset", false);   
            DrawProp(ref position, property, "panelParameters", true);
        }
        static readonly string[] subClassNames = new string[] { "textParameters", "panelParameters" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 5; }
    }
    [CustomEditor(typeof(UIValueTracker))] public class UIValueTrackerEditor : Editor {
        static float value = .5f;
        static string text = "HP";
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GUITools.Space(2);
            EditorGUILayout.LabelField("Debug:", GUITools.boldLabel);
            value = EditorGUILayout.Slider(value, 0, 1);
            text = EditorGUILayout.TextField(text);

            if (GUILayout.Button("Start Debug")) {
                (target as UIValueTracker).SetValue(value, UIColorScheme.Normal);
                (target as UIValueTracker).text.SetText(text);
                (target as UIValueTracker).UpdateElementLayout();//true, true);

                EditorUtility.SetDirty(target);   
            }
        }
    }
    #endif

    [ExecuteInEditMode] public class UIValueTracker : UIObject
    {
        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     text.OnComponentClose();
        //     fillImage.OnComponentClose();
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }


        public UIValueTrackerParameters parameters;
        
        UIText _valueText;
        public UIText text { get { return gameObject.GetComponentInChildrenIfNull<UIText>(ref _valueText, true); } }
        UIImage _fillImage;
        UIImage fillImage{
            get {
                if (_fillImage == null) _fillImage = panel.rectTransform.GetChild(1).GetComponent<UIImage>();
                return _fillImage;
            }
        }
        
        public void SetValue (float val, UIColorScheme scheme) {
            fillImage.mainGraphic.fillAmount = val;
            fillImage.SetColorScheme(scheme, false);
            // panel.SetColorScheme(scheme, scheme);
        }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {

            
            UIBaseParameters.CopyParameters(ref text.parameters, parameters.textParameters);
            // Vector2 textSize = 
            text.UpdateElementLayout();//firstBuild, !parameters.flipTextAndValue);
            
            // TextAnchor anchor = TextAnchor.LowerCenter;



            text.SetAnchor(rectTransform.pivot);

            float trackerAnchorX = rectTransform.anchorMin.x;
            float trackerAnchorY = rectTransform.anchorMin.y;
            
            // if (trackerAnchorX == 0) {
            //     if      (trackerAnchorY == 0.0f) anchor = TextAnchor.LowerLeft;
            //     else if (trackerAnchorY == 0.5f) anchor = TextAnchor.MiddleLeft;
            //     else if (trackerAnchorY == 1.0f) anchor = TextAnchor.UpperLeft;
            // }
            // else if (trackerAnchorX == .5f) {
            //     if      (trackerAnchorY == 0.0f) anchor = TextAnchor.LowerCenter;
            //     else if (trackerAnchorY == 0.5f) anchor = TextAnchor.MiddleCenter;
            //     else if (trackerAnchorY == 1.0f) anchor = TextAnchor.UpperCenter;
            // }
            // else if (trackerAnchorX == 1) {
            //     if      (trackerAnchorY == 0.0f) anchor = TextAnchor.LowerRight;
            //     else if (trackerAnchorY == 0.5f) anchor = TextAnchor.MiddleRight;
            //     else if (trackerAnchorY == 1.0f) anchor = TextAnchor.UpperRight;
            // }
            
            // text.SetAnchor( anchor, true, Vector2.zero );    
            
            Vector2 trackerSize = parameters.isVertical ? new Vector2(parameters.trackerSize.y, parameters.trackerSize.x) : parameters.trackerSize;
            
            panel.rectTransform.sizeDelta = trackerSize;

            panel.SetPivotAndAnchor(rectTransform.anchorMin);
            
            RectTransform a = parameters.flipTextAndValue ? panel.rectTransform : text.rectTransform;
            RectTransform b = parameters.flipTextAndValue ? text.rectTransform : panel.rectTransform;

            a.anchoredPosition = Vector2.zero;

            if ((!parameters.isVertical && trackerAnchorX != .5f) || trackerAnchorY == .5f) {
                float aWidth = parameters.flipTextAndValue ? trackerSize.x : text.getWidth;
                b.anchoredPosition = new Vector2((aWidth + parameters.textOffset) * (trackerAnchorX == 1 ? -1 : 1), 0);
            }
            else {
                float aHeight = parameters.flipTextAndValue ? trackerSize.y : text.getHeight;
                b.anchoredPosition = new Vector2(0, (aHeight + parameters.textOffset) * (trackerAnchorY == 1 ? -1 : 1));
            }

            UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParameters);
            panel.UpdateElementLayout();//firstBuild);


            fillImage.rectTransform.sizeDelta = new Vector2(trackerSize.x - parameters.fillBorderOffset * 2, trackerSize.y - parameters.fillBorderOffset * 2);
            fillImage.rectTransform.anchoredPosition = new Vector2(parameters.fillBorderOffset, -parameters.fillBorderOffset);
            
            fillImage.UpdateElementLayout();//firstBuild, true);
            fillImage.mainGraphic.type = Image.Type.Filled;
            fillImage.mainGraphic.fillMethod = !parameters.isVertical ? Image.FillMethod.Horizontal : Image.FillMethod.Vertical;
            // return Vector2.zero;
        }
        
    }
}
