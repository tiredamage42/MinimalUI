using UnityEngine;
using UnityEngine.UI;

using UnityTools;
using UnityTools.EditorTools;
using UnityEditor;
namespace MinimalUI
{
    [System.Serializable] public class UIMessageElementParameters : UIBaseParameters<UIMessageElementParameters> {
        public float messageHeight = 1f;
        public UITextParameters textParams;
        public Vector2 textMargin = Vector2.zero;
        public UIPanelParameters panelParams;

        public float flairIndent = .125f;
        public float flairSize = .2f;
        

        public override void CopyFrom(UIMessageElementParameters other) {
            flairIndent = other.flairIndent;
            flairSize = other.flairSize;
            messageHeight= other.messageHeight;
            textMargin = other.textMargin;
            UIBaseParameters.CopyParameters(ref textParams, other.textParams);
            UIBaseParameters.CopyParameters(ref panelParams, other.panelParams);

        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIMessageElementParameters))] class UIMessageElementParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "messageHeight", false);
            DrawProp(ref position, property, "textParams", true);
            DrawProp(ref position, property, "textMargin", false);

            DrawProp(ref position, property, "panelParams", true);

            DrawProp(ref position, property, "flairIndent", false);
            DrawProp(ref position, property, "flairSize", false);
            
        }
        static readonly string[] subClassNames = new string[] { "textParams", "panelParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 4; }
    }
    #endif

    public class UIMessageElement : UIComponent {
        public UIMessageElementParameters parameters;

        public UIImage flair;

        public void EnableFlair(bool enabled) {
            flair.gameObject.SetActive(enabled);
        }

        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     flair.OnComponentClose();
        //     text.OnComponentClose();

        //     panel.OnComponentClose();

        //     for (int i =0 ; i < graphics.Length; i++)
        //         graphics[i].OnComponentClose();
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }

        

        
        UIText _text;
        public UIText text { get { return gameObject.GetComponentInChildrenIfNull<UIText>(ref _text, true); } }
        
        UIGraphic[] _graphics;
        public UIGraphic[] graphics { get { return gameObject.GetComponentsInChildrenIfNull<UIGraphic>(ref _graphics, true); } }

        // CanvasGroup _canvasGroup;
        // CanvasGroup canvasGroup { get { return gameObject.GetComponentIfNull<CanvasGroup>(ref _canvasGroup, true); } }
        
        UIPanel _panel;
        UIPanel panel { get { return gameObject.GetComponentInChildrenIfNull<UIPanel>(ref _panel, true); } }
         
        bool inExit;
        float alpha, timer, duration, fadeIn, fadeOut;
        public bool isAvailable { get { return !gameObject.activeSelf; } }

        public void DisableMessage () {
            gameObject.SetActive(false);
        }

        public override void UpdateElementLayout(){//bool firstBuild) {
            UIBaseParameters.CopyParameters(ref text.parameters, parameters.textParams);
            text.SetAnchor(TextAnchor.MiddleLeft, true, Vector2.zero);
            // Vector2 textSize = 
            text.UpdateElementLayout();//firstBuild);
            float textWidth = text.getWidth;


            float elementWidth = textWidth + (parameters.flairIndent * 2 + parameters.flairSize) * 2;
        
            rectTransform.sizeDelta = new Vector2(elementWidth, elementHeight);
            
            // text.rectTransform.anchoredPosition = new Vector2(parameters.textMargin.x, 0);
            text.rectTransform.anchoredPosition = new Vector2((parameters.flairIndent * 2 + parameters.flairSize), 0);
            
            // for (int x = 0; x < flairs.Length; x++) {
                
                if (flair.gameObject.activeSelf) {
                    flair.rectTransform.sizeDelta = Vector2.one * parameters.flairSize;
                    

                    // float offset = rectTransform.pivot.x == 0 ? parameters.elementsSize.x * .5f : (rectTransform.pivot.x == 1 ? -(parameters.elementsSize.x * .5f) : 0);
                    // flairs[x].transform.localPosition = new Vector3((parameters.elementsSize.x * .5f - parameters.flairIndent) * (x == 0 ? -1 : 1), 0, 0);
                    flair.rectTransform.anchoredPosition = new Vector2(
                        // ((parameters.elementsSize.x * .5f - parameters.flairIndent) * (x == 0 ? -1 : 1)) + (offset), 
                        ((parameters.flairIndent)),// * (x == 0 ? 1 : -1)),//+ (offset), 

                        
                    0);

                    flair.mainGraphic.raycastTarget = false;
                    
                    // flairs[x].SetColorScheme(UIColorScheme.Normal, selected);
                    flair.UpdateElementLayout();//firstBuild);
                }
                
            // }

            
            UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParams);
            panel.UpdateElementLayout();//firstBuild);


            // return rectTransform.sizeDelta;


        }

        /*
        public override void UpdateElementLayout() {
            
            UIBaseParameters.CopyParameters(ref uiText.parameters, parameters.textParams);
            uiText.SetAnchor( parameters.textAlignment, true, Vector2.zero );
            
            uiText.UpdateElementLayout();
            uiText.rectTransform.sizeDelta = new Vector2(parameters.elementsSize.x - (parameters.flairIndent * 2 + parameters.flairSize) * 2, parameters.elementsSize.y) / uiText.rectTransform.localScale.x;
            uiText.rectTransform.anchoredPosition = new Vector2((parameters.flairIndent * 2 + parameters.flairSize), 0);
            
            
            rectTransform.sizeDelta = new Vector2( parameters.elementsSize.x, uiText.getHeight + parameters.elementsSize.y * 2);// Mathf.Max(parameters.elementsSize.y, uiText.getHeight + parameters.yBuffer * 2));
            
            uiText.SetColorScheme(UIColorScheme.Normal, selected);
            uiText.useOutline = !selected;



            
        }
        
         */





        // public float elementWidth { get { return text.getWidth + (parameters.flairIndent * 2 + parameters.flairSize) * 2; } } // parameters.textMargin.x * 2; } }
        public float elementHeight { get { return parameters.messageHeight; } }

        

















        
        public void ShowMessage (float duration, float fadeIn, float fadeOut, UIColorScheme scheme){
            
            alpha = 0;
            timer = 0;
            inExit = false;

            
            this.duration = duration;
            this.fadeIn = fadeIn;
            this.fadeOut = fadeOut;

            for (int i = 0; i < graphics.Length; i++) 
                graphics[i].SetColorScheme(scheme, graphics[i].useDark);
            
            canvasGroup.alpha = alpha;
            
        }

        public bool UpdateElement (float deltaTime) {
            if (!inExit) {
                if (alpha != 1) {
                    alpha += deltaTime / fadeIn;
                    if (alpha > 1) alpha = 1;
                }
            }
            else {
                if (alpha != 0) {
                    alpha -= deltaTime / fadeOut;
                    if (alpha < 0) alpha = 0;
                }
            }
            if (alpha == 1) {
                timer += deltaTime;
                if (timer >= duration) inExit = true;
            }

            canvasGroup.alpha = alpha;
            
            
            if (inExit && alpha == 0) {
                gameObject.SetActive(false);
                return true;
            }
            return false;
        }
    }
}