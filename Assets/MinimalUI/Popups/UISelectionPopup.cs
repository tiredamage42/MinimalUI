using UnityEngine;
using UnityEngine.UI;

using UnityTools;
using UnityEditor;
using UnityTools.EditorTools;

namespace MinimalUI {

    [System.Serializable] public class UISelectionPopupParameters : UIBaseParameters<UISelectionPopupParameters> {
        public float maxTextWidth = 2.5f;
        public UITextParameters textParams;
        public float panelWidth = 3;
        public UIPanelParameters panelParameters;
        public float buttonsSpace = .00875f;
        public UIRectSelectableParameters buttonParameters;
        public float hintsSpace = .05f;
        public ActionHintsPanelParameters controlHintsParams;
        
        public override void CopyFrom(UISelectionPopupParameters other) {
            UIBaseParameters.CopyParameters(ref textParams, other.textParams);
            UIBaseParameters.CopyParameters(ref panelParameters, other.panelParameters);
            UIBaseParameters.CopyParameters(ref buttonParameters, other.buttonParameters);
            UIBaseParameters.CopyParameters(ref controlHintsParams, other.controlHintsParams);

            maxTextWidth = other.maxTextWidth;
            panelWidth = other.panelWidth;
            buttonsSpace = other.buttonsSpace;
            hintsSpace = other.hintsSpace;
        
        }
    }

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UISelectionPopupParameters))]
    class UISelectionPopupParametersDrawer : Internal.ParametersDrawer {

        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "maxTextWidth", false);
            DrawProp(ref position, property, "textParams", true);
            DrawProp(ref position, property, "panelWidth", false);
            DrawProp(ref position, property, "panelParameters", true);
            DrawProp(ref position, property, "buttonsSpace", false);
            DrawProp(ref position, property, "buttonParameters", true);
            DrawProp(ref position, property, "hintsSpace", false);
            DrawProp(ref position, property, "controlHintsParams", true);
        }

        static readonly string[] subClassNames = new string[] { "textParams", "controlHintsParams", "panelParameters", "buttonParameters" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 4; }   
    }

    #endif


    [ExecuteInEditMode] public class UISelectionPopup : UISelectableHolder
    {

        public override void OnComponentEnable () {
            base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();          
        }


        // static UISelectionPopup _instance;
        // public static UISelectionPopup instance { get { return Singleton.GetInstance<UISelectionPopup>(ref _instance, true); } }
        void Awake () {
            if (Application.isPlaying) {
            //     if (SetUIComponentInstance<UISelectionPopup>(ref _instance, this)) {
                    // UIManager.HideUI(this);
                    // UIManager.HideUIComponent(this, 0);
                // }
            }
        }

        // protected override void OnEnable() {
        //     if (SetUIComponentInstance<UISelectionPopup>(ref _instance, this)) {
        //         base.OnEnable();
        //         UIManager.HideUI(this);
        //     }
        // }
        // protected override bool IsPopup() { return true; }
        protected override UISelectable ElementPrefab () { return settings.rectButton; }
        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     text.OnComponentClose();
        // }
        UIText _uiText;
        public UIText text { get { return gameObject.GetComponentInChildrenIfNull<UIText>(ref _uiText, true); } }

        public override void SetMainText(string text) {
            this.text.SetText(text);
        }
        
        public UISelectionPopupParameters parameters;
        
        static readonly Vector2 defaultAnchor = new Vector2(.5f, .5f);

        public override void UpdateElementLayout(){//bool firstBuild) {

            // float fullButtonSize = 0;
            float singleButtonSize = 0;
            for (int i =0 ; i< showElements.Count; i++) {
                showElements[i].rectTransform.pivot = defaultAnchor;

                UIBaseParameters.CopyParameters(ref (showElements[i] as UIRectSelectable).parameters, parameters.buttonParameters);
                showElements[i].UpdateElementLayout();//firstBuild);  
                
                // fullButtonSize += showElements[i].rectTransform.sizeDelta.y;
                if (singleButtonSize == 0) singleButtonSize = showElements[i].rectTransform.sizeDelta.y;
            }





            // float singleButtonSize = parameters.buttonParameters.elementsSize.y;
        
            // float fullButtonSize = parameters.buttonParameters.elementsSize.y + parameters.buttonsSpace;
            float fullButtonSize = singleButtonSize + parameters.buttonsSpace;
            
            float x2ButtonPad = singleButtonSize * .5f;// parameters.buttonParameters.elementsSize.y * .5f;

            UIBaseParameters.CopyParameters(ref text.parameters, parameters.textParams);
            // Vector2 textSize = 
            text.UpdateElementLayout();//firstBuild);
            float textHeight = text.getHeight;

            
            
            // needs to be before height check for text...
            text.rectTransform.sizeDelta = new Vector2(parameters.maxTextWidth/parameters.textParams.scale, 1);
            
            float textSizeAll = textHeight + x2ButtonPad * 3;
            
            
            float fullMessageBoxSize = textSizeAll + (showElements.Count * fullButtonSize) + singleButtonSize;

            float pivotOffset = (1.0f - rectTransform.pivot.y) * fullMessageBoxSize;
            
            Vector2 fullPanel = new Vector2(parameters.panelWidth, fullMessageBoxSize);
            rectTransform.sizeDelta = fullPanel;
            

            text.rectTransform.localPosition = new Vector3(0, -x2ButtonPad*2 + pivotOffset, 0);

            for (int i =0 ; i< showElements.Count; i++) {
                // showElements[i].rectTransform.pivot = defaultAnchor;

                showElements[i].transform.localPosition = new Vector3(0, -(textSizeAll + singleButtonSize + (fullButtonSize * i)) + pivotOffset, 0);
                
                // UIBaseParameters.CopyParameters(ref (showElements[i] as UIRectSelectable).parameters, parameters.buttonParameters);

            
                // showElements[i].UpdateElementLayout();//firstBuild);  
            }

            UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParameters);
            panel.UpdateElementLayout();//firstBuild);

            actionHintsPanel.transform.localPosition = new Vector3(0, -(fullMessageBoxSize + parameters.hintsSpace) + pivotOffset, 0);
            UIBaseParameters.CopyParameters(ref actionHintsPanel.parameters, parameters.controlHintsParams);
            
            actionHintsPanel.UpdateElementLayout();//firstBuild);
            // return Vector2.zero;
        }


    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(UISelectionPopup))] public class UISelectionPopupEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Internal.SelectableDebug.DrawDebugger(target as UISelectionPopup);
        }
    }
    #endif
}