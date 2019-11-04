

using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityTools.EditorTools;
using UnityTools;
namespace MinimalUI {
    /*
        rectangular panel of selectable element buttons....
    */
    [System.Serializable] public class UIPageParameters : UIBaseParameters<UIPageParameters> {
        public bool layoutSisterElements;
        // public bool overrideTextPanelHeight;
        public float titleOffset;
        public UITextParameters titleParams;
        public UIPanelParameters panelParameters;
        public UIRectSelectableParameters buttonParams;

        public float hintSpace = .1f;
        public ActionHintsPanelParameters hintPanelParams;

        public override void CopyFrom(UIPageParameters other) {
            titleOffset = other.titleOffset;
            hintSpace = other.hintSpace;
            // overrideTextPanelHeight = other.overrideTextPanelHeight;
            layoutSisterElements = other.layoutSisterElements;

            UIBaseParameters.CopyParameters(ref titleParams, other.titleParams);
            UIBaseParameters.CopyParameters(ref panelParameters, other.panelParameters);
            UIBaseParameters.CopyParameters(ref buttonParams, other.buttonParams);
            UIBaseParameters.CopyParameters(ref hintPanelParams, other.hintPanelParams);

        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIPageParameters))]
    class UIPageParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "titleOffset", false);
            // DrawProp(ref position, property, "overrideTextPanelHeight", false);
            DrawProp(ref position, property, "layoutSisterElements", false);

            
            DrawProp(ref position, property, "titleParams", true);
            DrawProp(ref position, property, "panelParameters", true);
            DrawProp(ref position, property, "buttonParams", true);
            DrawProp(ref position, property, "hintSpace", false);
            
            DrawProp(ref position, property, "hintPanelParams", true);
        }
        static readonly string[] subClassNames = new string[] { "titleParams", "panelParameters", "buttonParams", "hintPanelParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 4; }
    }
    #endif


    [ExecuteInEditMode] public class UIPage : UISelectableHolder
    {

        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     pageTitle.OnComponentClose();
        // }

        public override void OnComponentEnable () {
            base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();          
        }



        // public bool overrideTextPanelHeight;
        // protected override bool IsPopup() { return false; }
        public UIPageParameters parameters;
        protected override UISelectable ElementPrefab () { return settings.rectButton; }
      
        UIText _pageTitle;
        UIText pageTitle { get { return gameObject.GetComponentInChildrenIfNull<UIText>(ref _pageTitle, true); } }

        public override void SetMainText(string text) {
            SetTitle(text);
        }
        public void SetTitle (string title) {
            pageTitle.SetText(title);
        }

        public override void UpdateElementLayout(){//bool firstBuild) {

            float pageWidth = parameters.buttonParams.elementsSize.x;
            
            float halfPageWidth = pageWidth * .5f;
            
            float elementYBuffer = parameters.buttonParams.elementsSize.y;

            float myHeight = 0;

            Vector2 buttonPivot = rectTransform.pivot;
            buttonPivot.y = .5f;

            for (int i = 0; i < showElements.Count; i++) {
                // if (shownElements[i].baseObject.activeSelf) {
                    UIBaseParameters.CopyParameters(ref (showElements[i] as UIRectSelectable).parameters, parameters.buttonParams);                    
                    showElements[i].SetPivotAndAnchor(buttonPivot);
                    // Vector2 elementSize = 
                    showElements[i].UpdateElementLayout();//firstBuild);         
                    myHeight += showElements[i].rectTransform.sizeDelta.y;
                // }
            }


            if (!parameters.buttonParams.useTextHeight)
                myHeight = Mathf.Max(elementYBuffer, showElements.Count * elementYBuffer);
            else 
                myHeight = Mathf.Max(elementYBuffer * 4, myHeight);

            rectTransform.sizeDelta = new Vector2(pageWidth, myHeight);

            
            float pivotOffset = (1.0f - rectTransform.pivot.y) * rectTransform.sizeDelta.y;
            
            UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParameters);
            
            panel.UpdateElementLayout();//firstBuild);

            
            UIBaseParameters.CopyParameters(ref pageTitle.parameters, parameters.titleParams);
            pageTitle.SetAnchor(rectTransform.pivot);
            // Vector2 pageTitleRect = 
            pageTitle.UpdateElementLayout();//firstBuild);
            float pageTitleHeight = pageTitle.getHeight;

            pageTitle.rectTransform.anchoredPosition = new Vector2(0, parameters.titleOffset + pageTitleHeight * (rectTransform.pivot.y) + pivotOffset);

            float y = rectTransform.sizeDelta.y * .5f;

            int c = 0;
            for (int i = 0; i < showElements.Count; i++) {
                // if (!shownElements[i].gameObject.activeSelf) continue;      
                float elementHeight = showElements[i].rectTransform.sizeDelta.y;      
                if (c == 0) y -= elementHeight * .5f;
                showElements[i].rectTransform.anchoredPosition = new Vector2(0, y);
                y -= elementHeight;
                c++;
            }


            // Vector2 hintsPanelSize = Vector2.zero;

            float hintsPanelHeight = 0;
            if (actionHintsPanel != null) {
                UIBaseParameters.CopyParameters(ref actionHintsPanel.parameters, parameters.hintPanelParams);   
                // hintsPanelSize = 
                actionHintsPanel.UpdateElementLayout();//firstBuild);
                hintsPanelHeight = actionHintsPanel.rectTransform.sizeDelta.y;
            }
            
            if (parameters.layoutSisterElements) {

                // float textPanelHeight = 0;

                float textPanelSize = 0;
                // Vector2 textPanelSize = Vector2.zero;
                if (textPanel != null) {

                
                    float fullWidth = pageWidth + .1f + textPanel.parameters.rectSize.x;

                    float rX = fullWidth * -.5f + halfPageWidth;
                    float tX = fullWidth * .5f - textPanel.parameters.rectSize.x * .5f;

                    rectTransform.anchoredPosition = new Vector2(rX, 0);
                    
                    textPanel.rectTransform.anchoredPosition = new Vector2(tX, myHeight * .5f);
                    
                    // if (parameters.overrideTextPanelHeight) {
                    textPanel.overrideHeight = true;
                    float textPanelHeight = myHeight - (actionHintsPanel != null ? (hintsPanelHeight + parameters.hintSpace) : 0);
                    textPanel.SetHeightOverride(textPanelHeight);
                    textPanel.UpdateElementLayout();//firstBuild);
                    textPanelSize = textPanel.rectTransform.sizeDelta.y;
                
                }

                if (actionHintsPanel != null) {
                    if (textPanel != null) {
                        actionHintsPanel.rectTransform.anchoredPosition = new Vector2(
                            (textPanel.parameters.rectSize.x * .5f) + (halfPageWidth) + .1f, 
                            -(textPanelSize + parameters.hintSpace)
                        );
                    }
                    else {
                        actionHintsPanel.rectTransform.anchoredPosition = new Vector2(0, -(myHeight + parameters.hintSpace));
                    }
                }
            }
            // return Vector2.zero;
        }
    }

    

    #if UNITY_EDITOR
    [CustomEditor(typeof(UIPage))] public class UIPageEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            Internal.SelectableDebug.DrawDebugger(target as UIPage);
        }
    }
    #endif
}
