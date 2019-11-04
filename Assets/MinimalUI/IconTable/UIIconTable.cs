// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTools;
using System.Linq;
using UnityEditor;

namespace MinimalUI {
    [System.Serializable] public class UIIconTableParameters : UIBaseParameters<UIIconTableParameters> {
        public UIIconTableElementParameters iconParameters;
        public int maxColumns = 3;
        public float spacing = .1f;
        public override void CopyFrom(UIIconTableParameters other) {
            UIBaseParameters.CopyParameters(ref iconParameters, other.iconParameters);
            maxColumns = other.maxColumns;
            spacing = other.spacing;
        }
    }
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIIconTableParameters))]
    class UIIconTableParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "maxColumns", false);
            DrawProp(ref position, property, "spacing", false);
            DrawProp(ref position, property, "iconParameters", true);
        }
        static readonly string[] subClassNames = new string[] { "iconParameters" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 2; }
    }
    #endif

    public class UIIconTable : UIComponent
    {
        static UIIconTable _instance;
        public static UIIconTable instance { get { return Singleton.GetInstance<UIIconTable>(ref _instance, true); } }

        static PrefabPool<UIIconTableElement> pool = new PrefabPool<UIIconTableElement>();
        public UIIconTableElement AddIcon (Sprite iconSprite) {
            UIIconTableElement newElement = pool.GetAvailable(settings.iconTableElement, null, false, null);
            newElement.SetIconSprite(iconSprite);
            newElement.transform.SetParent(transform, Vector3.zero, Quaternion.identity, Vector3.one);
            
            newElement.gameObject.SetActive(true);
            allIcons.Add(newElement);
            UpdateElementLayout();//true);
            return newElement;
        }
        public UIIconTableElement RemoveIcon (UIIconTableElement element) {
            allIcons.Remove(element);
            element.gameObject.SetActive(false);
            element.transform.SetParent(UIMainCanvas.instance.transform);
            UpdateElementLayout();//true);
            return null;
        }
        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     for (int i =0 ; i < allIcons.Count; i++)
        //         allIcons[i].OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }


        public UIIconTableParameters parameters;
        List<UIIconTableElement> allIcons = new List<UIIconTableElement>();

        float ResetCoord (float pivot, int rowCols, float middleMult) {
            float iconSize = parameters.iconParameters.iconSize + parameters.spacing;
            if      (pivot == 1.0f) return 0; 
            else if (pivot == 0.5f) return middleMult * rowCols * .5f * iconSize + -middleMult * iconSize * .5f;
            else if (pivot == 0.0f) return 0; 
            return 0;
        }

        public override void UpdateElementLayout(){//bool firstBuild) {
            
            if (!Application.isPlaying) {
                allIcons = GetComponentsInChildren<UIIconTableElement>().ToList();
            }

            int c = allIcons.Count;
            int cDiv = c / parameters.maxColumns;
            int rows = c % parameters.maxColumns == 0 ? cDiv : cDiv + 1;
            Vector2 pivot = rectTransform.pivot;

            float iconSize = parameters.iconParameters.iconSize + parameters.spacing;

            float x = ResetCoord ( pivot.x, parameters.maxColumns, -1);
            float y = ResetCoord ( pivot.y, rows, 1);

            int column = 0;

            for (int i = 0; i < c; i++) {
                allIcons[i].rectTransform.pivot = rectTransform.pivot;
                allIcons[i].rectTransform.anchorMax = rectTransform.anchorMax;
                allIcons[i].rectTransform.anchorMin = rectTransform.anchorMin;
                
                UIBaseParameters.CopyParameters(ref allIcons[i].parameters, parameters.iconParameters);
            
                allIcons[i].UpdateElementLayout();//firstBuild);

                allIcons[i].rectTransform.anchoredPosition = new Vector2(x, y);
                
                column++;

                if      (pivot.x == 1.0f) x -= iconSize; 
                else if (pivot.x == 0.5f) x += iconSize;
                else if (pivot.x == 0.0f) x += iconSize; 
                

                if (column >= parameters.maxColumns) {
                    column = 0;

                    x = ResetCoord ( pivot.x, parameters.maxColumns, -1);

                    if      (pivot.y == 1.0f) y -= iconSize; 
                    else if (pivot.y == 0.5f) y -= iconSize;
                    else if (pivot.y == 0.0f) y += iconSize; 
                }
            }
            // return Vector2.zero;
        }
    }
}
