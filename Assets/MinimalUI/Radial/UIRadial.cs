using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityTools.EditorTools;

namespace MinimalUI {
    [System.Serializable] public class UIRadialParameters : UIBaseParameters<UIRadialParameters> {
        public float panelSize = 3;
        public UIRadialSelectableParameters selectableParams;
        public UIPanelParameters panelParams;
        public float hintsSpace = .05f;
        public ActionHintsPanelParameters controlHintsParams;

        public override void CopyFrom(UIRadialParameters other) {
            panelSize = other.panelSize;
            hintsSpace = other.hintsSpace;

            UIBaseParameters.CopyParameters(ref selectableParams, other.selectableParams);
            UIBaseParameters.CopyParameters(ref panelParams, other.panelParams);
            UIBaseParameters.CopyParameters(ref controlHintsParams, other.controlHintsParams);

        }

    }
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UIRadialParameters))]
    class UIRadialParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "panelSize", false);
            
            DrawProp(ref position, property, "selectableParams", true);
            DrawProp(ref position, property, "panelParams", true);
            DrawProp(ref position, property, "hintsSpace", false);
            DrawProp(ref position, property, "controlHintsParams", true);
        }
        static readonly string[] subClassNames = new string[] { "selectableParams", "panelParams", "controlHintsParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 2; }
    }
    #endif

    [ExecuteInEditMode] public class UIRadial : UISelectableHolder
    {
        // protected override bool IsPopup() { return false; }

        public UIRadialParameters parameters;

        public override void SetMainText(string text) {
        
        }
        
        
        public override void UpdateElementLayout(){//bool firstBuild) {
            
            rectTransform.sizeDelta = Vector2.one * parameters.panelSize;
        
            float sliceAngle = 360.0f / Mathf.Max(showElements.Count, 1);
            float radialAmount = 1f / Mathf.Max(showElements.Count, 1);
            
            Quaternion radialAngleRotation = Quaternion.Euler(0,0, sliceAngle*.5f);


            float textHeight = 0;
            for (int i = 0; i < showElements.Count; i++) {
                float elementAngle = -i * sliceAngle;

                UISelectable element = showElements[i];

                UIBaseParameters.CopyParameters(ref (element as UIRadialSelectable).parameters, parameters.selectableParams);
            

                element.transform.localRotation = Quaternion.Euler(0,0, elementAngle);

                element.image.mainGraphic.fillAmount = radialAmount;
                element.image.mainGraphic.rectTransform.localRotation = radialAngleRotation;

                element.uiText.transform.localRotation = Quaternion.Euler(0,0, -elementAngle);
                
                if (elementAngle == 0 || elementAngle == -180) {
                    element.uiText.SetAnchor(TextAnchor.MiddleCenter, false, Vector2.zero);
                }
                else {
                    element.uiText.SetAnchor(elementAngle < -180f ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft, false, Vector2.zero);
                }

                // Vector2 elementTextSize = 
                element.UpdateElementLayout();//firstBuild);
                if (i == 0)
                    textHeight = (element as UIRadialSelectable).textHeight; //elementTextSize.y;
                
            }

            UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParams);
            
            panel.UpdateElementLayout();//firstBuild);

            float h = textHeight;//allElements.Count == 0 ? 0 : allElements[0].uiText.getHeight;
            actionHintsPanel.transform.localPosition = new Vector3(0, -(parameters.panelSize * .5f + (h) + parameters.hintsSpace), 0);
            UIBaseParameters.CopyParameters(ref actionHintsPanel.parameters, parameters.controlHintsParams);

            actionHintsPanel.UpdateElementLayout();//firstBuild);

            // return Vector2.zero;
        }
            
        
        protected override UISelectable ElementPrefab () {
            return settings.radialButton;
        }

        public override void OnComponentEnable () {
            base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();       
            currentSelected = -1;   
        }

        // protected override void OnDisable () {
        //     base.OnDisable();
        //     currentSelected = -1;
        // }

        protected override 
        void Update () {
            base.Update ();
            if (Application.isPlaying) {
                if (isActive)
                    SetSelection(UIInput.mouseAxis);
            }
        }

        int currentSelected = -1;

        public void SetSelection(Vector2 selection) {
            // Debug.Log(selection);

            int lastSelected = currentSelected;
            currentSelected = -1;

            if (selection != Vector2.zero && showElements.Count > 0) {
                currentSelected = 0;
                if (showElements.Count > 1) {
                    float sliceAngle = 360.0f / showElements.Count;
                    float a = (Mathf.Atan2(selection.x, selection.y) * Mathf.Rad2Deg) + (sliceAngle * .5f);
                    if (a < 0) a = a + 360.0f;
                    currentSelected = (int)(a / sliceAngle);
                }
            }

            if (lastSelected != currentSelected) {
                UIManager.SetSelection(currentSelected != -1 ? showElements[currentSelected].gameObject : null);
            }
        }
    }

    

    #if UNITY_EDITOR
    [CustomEditor(typeof(UIRadial))] public class UIRadialEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Internal.SelectableDebug.DrawDebugger(target as UIRadial);
        }
    }
    #endif
}
