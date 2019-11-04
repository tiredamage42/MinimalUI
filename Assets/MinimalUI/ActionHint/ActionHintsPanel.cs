using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityTools;
using UnityTools.EditorTools;
namespace MinimalUI {

    [System.Serializable] public class ActionHintsPanelParameters : UIBaseParameters<ActionHintsPanelParameters> {
        public bool usePanel = true;
        public UIPanelParameters panelParams;
        public float panelWidth = 3;
        public bool autoExpand = true;
        public UITextParameters mainTextParams;
        public Vector2 hintsSpacing = new Vector2(.25f, .05f);
        public int maxHintColumns = 2;

        public ActionHintParameters hintParams;
        [Tooltip("X, Y, Btwn Txt & Hint")] public Vector3 buffers = new Vector3(0.25f,0.05f,0.1f);

        public override void CopyFrom(ActionHintsPanelParameters other) {
            usePanel = other.usePanel;
            buffers = other.buffers;
            maxHintColumns = other.maxHintColumns;
            hintsSpacing = other.hintsSpacing;
            autoExpand = other.autoExpand;
            panelWidth = other.panelWidth;
            
            UIBaseParameters.CopyParameters(ref panelParams, other.panelParams);
            UIBaseParameters.CopyParameters(ref mainTextParams, other.mainTextParams);
            UIBaseParameters.CopyParameters(ref hintParams, other.hintParams);
        }        
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ActionHintsPanelParameters))] class ActionHintsPanelParametersDrawer : Internal.ParametersDrawer {

        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "usePanel", false);
            if (property.FindPropertyRelative("usePanel").boolValue) {
                DrawProp(ref position, property, "panelWidth", false);
                DrawProp(ref position, property, "autoExpand", false);
                DrawProp(ref position, property, "panelParams", true);
            }
            DrawProp(ref position, property, "mainTextParams", true);
            DrawProp(ref position, property, "hintsSpacing", false);
            DrawProp(ref position, property, "maxHintColumns", false);
            DrawProp(ref position, property, "buffers", false);
            DrawProp(ref position, property, "hintParams", true);
        }

        static readonly string[] subClassNames1 = new string[] { "panelParams", "hintParams", "mainTextParams" };
        static readonly string[] subClassNames2 = new string[] { "hintParams", "mainTextParams" };
        
        protected override string[] SubClassNames (SerializedProperty prop) { return prop.FindPropertyRelative("usePanel").boolValue ? subClassNames1 : subClassNames2; }
        protected override int SinglePropsCount(SerializedProperty prop) { return prop.FindPropertyRelative("usePanel").boolValue ? 6 : 4; }
    }
    #endif

    [ExecuteInEditMode] public class ActionHintsPanel : UIObject
    {

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
            RemoveAllHintElements();
        }


        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     textUI.OnComponentClose();

        //     RemoveAllHintElements();
        // }
        UIText _textUI;
        public UIText textUI {
            get {
                if (_textUI == null) _textUI = GetComponentInChildren<UIText>();
                return _textUI;
            }
        }        
        // public UIPanel panelUI;

        public ActionHintsPanelParameters parameters;
        
        // [HideInInspector] 
        public List<ActionHint> allHints = new List<ActionHint>();

        
        /*
            ALL HINTS SHOULD BE 1 UNIT IN HEIGHT 
        */

        const float hintImgSize = 1;
        const float halfImgSize = hintImgSize * .5f;

        // public float GetHintWidth (ActionHint hint) {
        //     return (hint.text.text.preferredWidth * parameters.hintsTextScale + (hintImgSize + parameters.hintsTextBuffer)) * parameters.hintsScale;
        // }


        // public float getHeight { get { return rectTransform.sizeDelta.y; } }

        public override void UpdateElementLayout(){//bool firstBuild) {

            bool emptyText = textUI.textEmpty;// string.IsNullOrEmpty(textUI.text.text);

            float betweenTextAndHints = emptyText ? 0 : parameters.buffers.z;
            float allHintsWidth = 0;

            float lastWidth = 0;

            int hintRows = 1;

            int c = 0;
            bool countLastWidth = false;

            float[] hintWidths = new float[allHints.Count];

            int maxColumns = Mathf.Min( parameters.maxHintColumns, allHints.Count);

            for (int i = 0; i < allHints.Count; i++) {
                // if (allHints[i] == null) {
                //     Debug.LogError(transform.parent.name);
                // }

                allHints[i].onTopOfPanel = parameters.usePanel;

                UIBaseParameters.CopyParameters(ref allHints[i].parameters, parameters.hintParams);


                // Vector2 hintSize = 
                allHints[i].UpdateElementLayout();//firstBuild);
                float hintWidth = allHints[i].getWidth;



                hintWidths[i] = hintWidth;

                if (hintRows == 1)
                    allHintsWidth += hintWidth + (c < maxColumns - 1 ? parameters.hintsSpacing.x : 0);    
                    // allHintsWidth += hintWidth + (c != parameters.maxHintColumns - 1 ? parameters.hintsSpacing.x : 0);    
                    
                    


                if (countLastWidth) 
                    lastWidth += hintWidth + (i != allHints.Count - 1 ? parameters.hintsSpacing.x : 0);    

                if (i < allHints.Count - 1) {

                    c++;
                    if (c >= maxColumns) {
                        hintRows ++;
                        c = 0;
                        if (i + maxColumns >= allHints.Count) {
                            countLastWidth = true;
                        }
                    }
                }
            }


            
            UIBaseParameters.CopyParameters(ref textUI.parameters, parameters.mainTextParams);

            Vector2 textSize = Vector2.zero;
            if (!emptyText) {
                textUI.useOutline = !parameters.usePanel;
                textUI.UpdateElementLayout();//firstBuild);
                textSize = new Vector2(textUI.getWidth, textUI.getHeight);
                // textUI.transform.localScale = Vector3.one * parameters.textScale;
            }
            
            float myTextWidth = emptyText ? 0 : textSize.x;
            float myTextHeight = emptyText ? 0 : textSize.y;
            
            float maxInsidesWith = Mathf.Max(allHintsWidth, myTextWidth);

            Vector2 panelSize = Vector2.zero;

            if (parameters.usePanel) {
                panelSize.x = parameters.autoExpand || (maxInsidesWith > parameters.panelWidth) ? maxInsidesWith + parameters.buffers.x : parameters.panelWidth;
            }
            panelSize.y = myTextHeight + (parameters.hintParams.imgScale * hintRows + (parameters.hintsSpacing.y * (hintRows - 1))) + betweenTextAndHints + parameters.buffers.y*2;

            rectTransform.sizeDelta = panelSize;
            

            float pivotOffset = (1.0f - rectTransform.pivot.y) * panelSize.y;
            
            // draw text
            if (!emptyText) {
                textUI.transform.localPosition = new Vector3(0, ((myTextHeight*-.5f)-parameters.buffers.y) + pivotOffset, 0);   
                // textUI.UpdateElementLayout();
                // textUI.transform.localScale = Vector3.one * parameters.textScale;
            }
            
            // draw hints
            float xStart = allHintsWidth * -.5f;
            float hintY = -(myTextHeight + (parameters.hintParams.imgScale * .5f) + parameters.buffers.y + betweenTextAndHints) + pivotOffset;
            // Debug.Log("Hint Y: " + hintY);
            // Debug.Log(transform.parent.name + "Rect pivot: " + rectTransform.pivot);

            c = 0;
            bool centerLast = false;
            int lastCount = 0;
            // int row = 0;
            for (int i = 0; i < allHints.Count; i++) {
                ActionHint hint = allHints[i];
                float width = hintWidths[i];// hint.getWidth;
                float w = width;
                
                // hint.SetPivotAndAnchor(new Vector2(.5f, .5f));
                hint.SetPivotAndAnchor(new Vector2(.5f, rectTransform.pivot.y));

                // hint.transform.localPosition = new Vector3(xStart + (w * (centerLast ? 1 : .5f)), hintY, 0);
                // hint.rectTransform.anchoredPosition = new Vector2(xStart + (w * (centerLast ? (lastCount) : .5f)), hintY);
                hint.rectTransform.anchoredPosition = new Vector2(xStart + (w * (.5f)), hintY);
                
                // hint.UpdateElementLayout();
                xStart += w + parameters.hintsSpacing.x;

                c++;
                if (c >= maxColumns) {
                    c = 0;
                    xStart = allHintsWidth * -.5f;
                    hintY -= parameters.hintParams.imgScale + parameters.hintsSpacing.y;// * .5f;
                    // row++;
                    if (i + maxColumns >= allHints.Count) {
                        lastCount = (allHints.Count - 1) - i;
                        centerLast = lastCount < maxColumns;
                        lastCount = maxColumns - lastCount;

                        xStart = lastWidth * -.5f;
                    }

                }
            }

            

            if (panel.gameObject.activeSelf != parameters.usePanel)
                panel.gameObject.SetActive(parameters.usePanel);
            
            if (parameters.usePanel){
                UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParams);
                panel.UpdateElementLayout();//firstBuild);
            }


            // return rectTransform.sizeDelta;
        }

        // void OnDisable () {
        //     RemoveAllHintElements();
        // }

        public void RemoveAllHintElements () {
            for (int i = 0; i < allHints.Count; i++) {
                if (Application.isPlaying) {
                    // allHints[i].OnComponentClose();
                    allHints[i].gameObject.SetActive(false);
                    allHints[i].transform.SetParent(UIMainCanvas.instance.rectTransform);
                }
                else {
                    if (allHints[i]!= null) DestroyImmediate(allHints[i].gameObject);
                }
            }
            allHints.Clear();
        }
        
        static PrefabPool<ActionHint> pool = new PrefabPool<ActionHint>();

        public void AddHintElements (List<int> actionHints, List<string> hintNames) {

            if (settings.actionHintsInitializer == null) {
                Debug.LogError("ActionHintInitializer instance not found...");
                return;
            }
            for (int i = 0; i < actionHints.Count; i++) {
                int action = actionHints[i];

                ActionHint prefab = settings.actionHintsInitializer.GetPrefabForAction(action);
                
                if (prefab == null)
                    continue;

                ActionHint hintInstance = pool.GetAvailable(prefab, null, false, null);
                
                settings.actionHintsInitializer.OnActionHintInitialized(hintInstance, action);

                AddHintElement(hintInstance, hintNames[i], i == actionHints.Count - 1);
            }
        }

        public void AddHintElement (ActionHint hint, string txt, bool updateLayouts) {
            if (!allHints.Contains(hint)) {
                hint.transform.SetParent(transform, Vector3.zero, Quaternion.identity, Vector3.one);
                hint.hintNameText.SetText ( txt );
                allHints.Add(hint);
                hint.gameObject.SetActive(true);
                if (updateLayouts) 
                    UpdateElementLayout();//true);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ActionHintsPanel))] public class ActionHintsPanelEditor : Editor {
        string debugMessage = "Some Debug Text To Show\nIn the ActionHints Popup Window";
        int buttons = 3;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUITools.Space(2);
            EditorGUILayout.LabelField("Debug:", GUITools.boldLabel);
            debugMessage = EditorGUILayout.TextArea(debugMessage);
            buttons = EditorGUILayout.IntField("Buttons Count", buttons);

            if (GUILayout.Button("Start Debug")) {
                ActionHintsPanel panel = target as ActionHintsPanel;
                panel.textUI.SetText(debugMessage);


                panel.RemoveAllHintElements();
                
                ActionHint actionHint = UnityTools.GameSettingsSystem.GameSettings.GetSettings<UISettings>().debugActionHintUI;
                for (int i = 0; i < buttons; i++) panel.AddHintElement(Instantiate(actionHint), "Buttons " + i, false);
                panel.UpdateElementLayout();//true);
                EditorUtility.SetDirty(panel);
            }

            if (GUILayout.Button("End Debug")) {
                ActionHintsPanel panel = target as ActionHintsPanel;
                panel.RemoveAllHintElements();
                panel.UpdateElementLayout();//true);
                EditorUtility.SetDirty(panel);
            }
        }
    }
    #endif
}
