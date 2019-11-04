using UnityEngine;
using UnityEditor;
using UnityTools.EditorTools;
using UnityTools;
namespace MinimalUI {

    [System.Serializable] public class UITextPanelParameters : UIBaseParameters<UITextPanelParameters> {
        public Vector2 rectSize = new Vector2 (4, 4);
        public Vector2 textOffsets = new Vector2(.1f,.1f);

        public float titleOffset = .1f;
        public UITextParameters titleTextParams;
        public TextAnchor textAlignment = TextAnchor.UpperLeft;
        public UITextParameters textParams;
        public UIPanelParameters panelParams;

        public override void CopyFrom(UITextPanelParameters other) {
            rectSize = other.rectSize;
            titleOffset = other.titleOffset;
            textOffsets = other.textOffsets;
            textAlignment = other.textAlignment;

            UIBaseParameters.CopyParameters(ref titleTextParams, other.titleTextParams);
            
            UIBaseParameters.CopyParameters(ref textParams, other.textParams);
            UIBaseParameters.CopyParameters(ref panelParams, other.panelParams);

        }
    }
    
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UITextPanelParameters))]
    class UITextPanelParametersDrawer : Internal.ParametersDrawer {

        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "rectSize", false);
            DrawProp(ref position, property, "textAlignment", false);
            DrawProp(ref position, property, "textOffsets", false);
            
            DrawProp(ref position, property, "titleOffset", false);
            
            DrawProp(ref position, property, "titleTextParams", true);
            
            DrawProp(ref position, property, "textParams", true);
            DrawProp(ref position, property, "panelParams", true);

        }

        static readonly string[] subClassNames = new string[] { "panelParams", "textParams", "titleTextParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 4; }
        
    }
    #endif


    [ExecuteInEditMode] public class UITextPanel : UIComponent
    {
        // public override void OnComponentClose() {
        //     // base.OnComponentClose();


        //     uiPanel.OnComponentClose();
        //     for (int i =0 ; i < texts.Length; i++)
        //         texts[i].OnComponentClose();
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }

        public bool overrideHeight;
        public UITextPanelParameters parameters;
        UIPanel _uiPanel;
        UIPanel uiPanel { get { return gameObject.GetComponentInChildrenIfNull<UIPanel>(ref _uiPanel, true); } }


        UIText[] _texts;
        UIText[] texts { get { return gameObject.GetComponentsInChildrenIfNull<UIText>(ref _texts, true); } }
        
        UIText mainText { get { return texts[0]; } }
        UIText titleText { get { return texts[1]; } }

        public void SetTexts (string title, string main, bool updateLayout) {
            titleText.SetText(title);
            mainText.SetText(main);
            if (updateLayout) UpdateElementLayout();//false, true);
        }
        
        float heightOverride;
        public void SetHeightOverride (float heightOverride) {
            this.heightOverride = heightOverride;
        }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            UIBaseParameters.CopyParameters(ref mainText.parameters, parameters.textParams);
            
            mainText.useOutline = !parameters.panelParams.useBackground;
            
            // Vector2 mainTextSize = 
            mainText.UpdateElementLayout();//firstBuild, true);
            

            rectTransform.sizeDelta = new Vector2(
                parameters.textParams.horizontalWrap == HorizontalWrapMode.Overflow ? Mathf.Max(parameters.rectSize.x, mainText.getWidth + parameters.textOffsets.x * 2) : parameters.rectSize.x, 
                Mathf.Max(overrideHeight ? heightOverride : parameters.rectSize.y, mainText.getHeight + parameters.textOffsets.y * 2)
            );

            if (!titleText.textEmpty){// string.IsNullOrEmpty(titleText.text.text)) {
                UIBaseParameters.CopyParameters(ref titleText.parameters, parameters.titleTextParams);

                // Vector2 titleTextSize = 
                titleText.UpdateElementLayout();//firstBuild, true);
                float titleHeight = titleText.getHeight;

                titleText.SetAnchor(rectTransform.pivot);
                float pivotOffset = (1.0f - rectTransform.pivot.y) * rectTransform.sizeDelta.y;
                
                titleText.rectTransform.anchoredPosition = new Vector2(0, parameters.titleOffset + titleHeight * (rectTransform.pivot.y) + pivotOffset);
            }

            mainText.SetAnchor( parameters.textAlignment, false, Vector2.zero);
            mainText.rectTransform.sizeDelta = (rectTransform.sizeDelta - parameters.textOffsets * 2) / parameters.textParams.scale;
 
            
            UIBaseParameters.CopyParameters(ref uiPanel.parameters, parameters.panelParams);
            uiPanel.UpdateElementLayout();//firstBuild);

            // return rectTransform.sizeDelta;
        }
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(UITextPanel))] 
    public class UITextPanelEditor : Editor {
        string title = "Text Panel Title";
        string debugMessage = "Some Debug Text To Show\nIn the Selection Popup Window";
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUITools.Space(2);
            EditorGUILayout.LabelField("Debug:", GUITools.boldLabel);
            title = EditorGUILayout.TextField(title);
            debugMessage = EditorGUILayout.TextArea(debugMessage);
        
            if (GUILayout.Button("Start Debug")) {
                UITextPanel panel = target as UITextPanel;
                panel.SetTexts(title, debugMessage, true);
                EditorUtility.SetDirty(panel);
            }
        }
    }
    #endif
}
