
#if UNITY_EDITOR
using UnityEngine;
using UnityTools.EditorTools;
using UnityEditor;

namespace MinimalUI.Internal {
    public abstract class ParametersDrawer : PropertyDrawer {

        protected void DrawProp (ref Rect position, SerializedProperty property, string name, bool isClass) {
            SerializedProperty p = property.FindPropertyRelative(name);
            EditorGUI.PropertyField(position, p, true);
            position.y += isClass ? EditorGUI.GetPropertyHeight(p, true) : GUITools.singleLineHeight;
        }

        protected abstract void DrawParams (Rect position, SerializedProperty property);
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            
            property.isExpanded = GUITools.DrawToggleButton(property.isExpanded, new GUIContent(property.isExpanded ? "V" : ">"), position.x, position.y, GUITools.blue, GUITools.white);           
            position.x += GUITools.iconButtonWidth;
            position.width -= GUITools.iconButtonWidth;
            EditorGUI.LabelField(position, label, GUITools.boldLabel);
            
            if (property.isExpanded) {
                position.y += GUITools.singleLineHeight;
                DrawParams(position, property);
            }
            EditorGUI.EndProperty();
        }
        protected abstract int SinglePropsCount (SerializedProperty prop);
        protected abstract string[] SubClassNames (SerializedProperty prop);

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float h = GUITools.singleLineHeight;
            if (property.isExpanded) {
                string[] classNames = SubClassNames (property);
                if (classNames != null) {
                    for (int i = 0; i < classNames.Length; i++) {
                        h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(classNames[i]), true);
                    }
                }
                h += GUITools.singleLineHeight * SinglePropsCount(property);
            }
            return h;
        }
    }

    public class SelectableDebug {

        static string debugMessage = "Some Debug Text To Show";
        static int buttons = 3;

        public static void DrawDebugger(UISelectableHolder selectableHolder) {
            GUITools.Space(2);
            EditorGUILayout.LabelField("Debug:", GUITools.boldLabel);
            debugMessage = EditorGUILayout.TextArea(debugMessage);
            buttons = EditorGUILayout.IntField("Buttons Count", buttons);

            if (GUILayout.Button("Start Debug")) {
                selectableHolder.RemoveAllElements();
                
                int randomSelected = Random.Range(0, buttons);
                selectableHolder.SetMainText(debugMessage);
                for (int i = 0; i < buttons; i++) {
                    // UISelectable s = selectableHolder.AddNewElement( "ButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtonsButtons" + i, false);
                    UISelectable s = selectableHolder.AddNewElement( "Buttons " + i, false);
                    
                    s.selected = i == randomSelected;
                }
                UpdateElementLayoutAndSetDirty(selectableHolder);
            }
            if (GUILayout.Button("End Debug")) {
                selectableHolder.RemoveAllElements();
                UpdateElementLayoutAndSetDirty(selectableHolder);
            }
        }
        static void UpdateElementLayoutAndSetDirty (UISelectableHolder selectableHolder) {
            selectableHolder.UpdateElementLayout();//true);
            EditorUtility.SetDirty(selectableHolder);
        }
    }
}
#endif
