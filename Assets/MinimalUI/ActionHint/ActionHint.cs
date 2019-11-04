using UnityEngine;
using UnityTools;
using UnityTools.EditorTools;
using UnityEditor;
namespace MinimalUI {

    [System.Serializable] public class ActionHintParameters : UIBaseParameters<ActionHintParameters> {
        
        public float imgScale = .2f;
        public float hintsTextBuffer = 0;
        public UITextParameters hintTextParams;

        public override void CopyFrom(ActionHintParameters other) {
            imgScale = other.imgScale;
            hintsTextBuffer = other.hintsTextBuffer;

            UIBaseParameters.CopyParameters(ref hintTextParams, other.hintTextParams);
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ActionHintParameters))] class ActionHintParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "imgScale", false);
            DrawProp(ref position, property, "hintsTextBuffer", false);
            DrawProp(ref position, property, "hintTextParams", true);   
        }
        static readonly string[] subClassNames = new string[] { "hintTextParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 2; }
    }
    #endif


    public class ActionHint : UIComponent
    {

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }


        // public override void OnComponentClose() {
        //     // base.OnComponentClose();

        //     for (int i = 0; i < allTexts.Length; i++)
        //         allTexts[i].OnComponentClose();
            
        // }


        public RectTransform imgTransform;
        public ActionHintParameters parameters;

        UIText[] _allTexts;
        UIText[] allTexts { get { return gameObject.GetComponentsInChildrenIfNull<UIText>(ref _allTexts, true); } }

        public UIText keyboardHintText { get { return allTexts[0]; } }
        public UIText hintNameText { get { return allTexts[allTexts.Length - 1]; } }
        public bool isKeyboardHint { get { return keyboardHintText != hintNameText; } }

        const float hintImgSize = 1;
        const float halfImgSize = hintImgSize * .5f;

        // public float getWidth {
        //     get  {
        //         // set text params to be sure to get proper text width
        //         UIBaseParameters.CopyParameters(ref hintNameText.parameters, parameters.hintTextParams);

        //         if (isKeyboardHint) {
        //             UIBaseParameters.CopyParameters(ref keyboardHintText.parameters, parameters.hintTextParams);
        //             return (hintNameText.getWidth + (keyboardHintText.getWidth + parameters.hintsTextBuffer));
        //         }
            
        //         return (hintNameText.getWidth + (hintImgSize * parameters.imgScale + parameters.hintsTextBuffer));
        //     }
        // }


        public float getWidth { get { return calculatedWidth; } }
        float calculatedWidth;

        [HideInInspector] public bool onTopOfPanel;

        public override void UpdateElementLayout(){//bool firstBuild) {

            hintNameText.useOutline = !onTopOfPanel;

            // set text params to be sure to get proper text width
            UIBaseParameters.CopyParameters(ref hintNameText.parameters, parameters.hintTextParams);
            // Vector2 hintNameSize = 
            hintNameText.UpdateElementLayout();//firstBuild);

            float hintNameWidth = hintNameText.getWidth;

            //float keyboardHintWidth = 0;
            // Vector2 keyboardHintSize = Vector2.zero;
            
            float offsetWidth = 0;
            float width = 0;
            if (isKeyboardHint) {

                keyboardHintText.useOutline = !onTopOfPanel;
            
                UIBaseParameters.CopyParameters(ref keyboardHintText.parameters, parameters.hintTextParams);
                // keyboardHintSize = 
                keyboardHintText.UpdateElementLayout();//firstBuild);

                float keyboardHintWidth = keyboardHintText.getWidth;
                offsetWidth = keyboardHintWidth;
                width = (keyboardHintWidth + hintNameWidth + parameters.hintsTextBuffer);
            }
            else {
                offsetWidth = hintImgSize * parameters.imgScale;
                width = (hintNameWidth + (offsetWidth) + parameters.hintsTextBuffer);
            }
            
            
            // draw hints
            
            // float width = getWidth;

            // float localXStart = width * -.5f;// (hintNameWidth + (hintImgSize * parameters.imgScale + parameters.hintsTextBuffer)) * -.5f;

            // if (isKeyboardHint) {
            //     localXStart = (hintNameWidth + (keyboardHintWidth + parameters.hintsTextBuffer)) * -.5f;
            // }
            
            rectTransform.sizeDelta = new Vector2(width, 0);
            
            // float offsetWidth = 0;
            if (isKeyboardHint) {

                keyboardHintText.SetAnchor(TextAnchor.MiddleLeft, true, Vector2.zero);

                keyboardHintText.rectTransform.anchoredPosition = new Vector2(0, 0);// + (keyboardHintWidth * .5f), 0);
                
                // keyboardHintText.rectTransform.localPosition = new Vector3(localXStart + (keyboardHintWidth * .5f), 0, 0);
                
                // keyboardHintText.UpdateElementLayout();

                // offsetWidth = keyboardHintText.getWidth;
            }
            else {

                imgTransform.pivot = new Vector2(0, .5f);
                imgTransform.anchorMin = new Vector2(0, .5f);
                imgTransform.anchorMax = new Vector2(0, .5f);
                
                // imgTransform.anchoredPosition = new Vector2(localXStart, 0);// + halfImgSize * parameters.imgScale, 0);
                imgTransform.anchoredPosition = new Vector2(0, 0);// + halfImgSize * parameters.imgScale, 0);
                
                // imgTransform.localPosition = new Vector3(localXStart + halfImgSize * parameters.imgScale, 0, 0);
                
                imgTransform.localScale = Vector3.one * parameters.imgScale;
                // offsetWidth = hintImgSize * parameters.imgScale;
            }


            hintNameText.SetAnchor(TextAnchor.MiddleLeft, true, Vector2.zero);
            // hintNameText.transform.localPosition = new Vector3(localXStart + offsetWidth + parameters.hintsTextBuffer, 0, 0);
            
            // hintNameText.rectTransform.anchoredPosition = new Vector2(localXStart + offsetWidth + parameters.hintsTextBuffer, 0);
            hintNameText.rectTransform.anchoredPosition = new Vector2( offsetWidth + parameters.hintsTextBuffer, 0);
                
            // hintNameText.UpdateElementLayout();


            // return new Vector2(width, 0);

            calculatedWidth = width;


        }
    }
}
