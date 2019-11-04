using UnityEngine;
using UnityEngine.UI;
using UnityTools;
using UnityEditor;
using UnityTools.EditorTools;

namespace MinimalUI {
    [System.Serializable] public class UISliderPopupParameters : UIBaseParameters<UISliderPopupParameters> {
        public Vector3 elementSpacings = new Vector3(.075f, .05f, .1f);
        public float panelWidth = 2;
        public UIPanelParameters panelParameters;
        public UITextParameters titleTextParams;
        public UITextParameters amountTextParams;
        public float sliderBGScale = 1;
        public Vector2 sliderHeadSize = new Vector2( .35f, .45f);
        public Vector2 sliderBGSize = new Vector2( 2, .1f );
        public float hintsSpace = .05f;
        public ActionHintsPanelParameters controlHintsParams;

        public override void CopyFrom(UISliderPopupParameters other) {
            elementSpacings = other.elementSpacings;
            panelWidth = other.panelWidth;
            sliderBGScale = other.sliderBGScale;
            sliderHeadSize = other.sliderHeadSize;
            sliderBGSize = other.sliderBGSize;
            hintsSpace = other.hintsSpace;

            UIBaseParameters.CopyParameters(ref panelParameters, other.panelParameters);
            UIBaseParameters.CopyParameters(ref titleTextParams, other.titleTextParams);
            UIBaseParameters.CopyParameters(ref amountTextParams, other.amountTextParams);
            UIBaseParameters.CopyParameters(ref controlHintsParams, other.controlHintsParams);            
        }
    }

    
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(UISliderPopupParameters))]
    class UISliderPopupParametersDrawer : Internal.ParametersDrawer {

        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "elementSpacings", false);
            DrawProp(ref position, property, "panelWidth", false);
            DrawProp(ref position, property, "panelParameters", true);
            DrawProp(ref position, property, "titleTextParams", true);
            DrawProp(ref position, property, "amountTextParams", true);
            DrawProp(ref position, property, "sliderBGScale", false);
            DrawProp(ref position, property, "sliderHeadSize", false);
            DrawProp(ref position, property, "sliderBGSize", false);
            DrawProp(ref position, property, "hintsSpace", false);            
            DrawProp(ref position, property, "controlHintsParams", true);
        }

        static readonly string[] subClassNames = new string[] { "controlHintsParams", "panelParameters", "titleTextParams", "amountTextParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 6; }
    }
    #endif


    [ExecuteInEditMode] public class UISliderPopup : UIWithInput
    {

        public override void OnComponentEnable () {
            base.OnComponentEnable();

            slider.interactable = true;
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();     
            slider.interactable = false;     
        }

        // protected override bool IsPopup() { return true; }
        public override void SetSelectablesActive(bool active) { }
        
        public override bool CurrentSelectedIsOurs (GameObject currentSelected) { 
            return slider.gameObject == currentSelected;
        }

        // static UISliderPopup _instance;
        // public static UISliderPopup instance { get { return Singleton.GetInstance<UISliderPopup>(ref _instance, true); } }

        void Awake () {
            if (Application.isPlaying) {
            //     if (SetUIComponentInstance<UISliderPopup>(ref _instance, this)) {
                    // UIManager.HideUI(this);

                    // UIManager.HideUIComponent(this, 0);
                // }

                slider.onValueChanged.AddListener(OnValueChanged);
            }
        }
        // protected override void OnEnable () {
        //     if (Application.isPlaying) slider.onValueChanged.AddListener(OnValueChanged);
        //     base.OnEnable();
        // }


        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     backgroundImage.OnComponentClose();
        //     for (int i = 0; i < texts.Length; i++)
        //         texts[i].OnComponentClose();
        // }

        UIImage _backgroundImage;
        UIImage backgroundImage { get { return slider.gameObject.GetComponentInChildrenIfNull<UIImage>(ref _backgroundImage, true); } }

        UIText[] _texts;
        UIText[] texts { get { return gameObject.GetComponentsInChildrenIfNull<UIText>(ref _texts, true); } }

        UIText titleText { get{ return texts[0]; } }
        UIText amountText { get{ return texts[1]; } }
            

        RectTransform _sliderRect;
        RectTransform sliderRect { get { return slider.gameObject.GetComponentIfNull<RectTransform>(ref _sliderRect, true); } }
        Slider _slider;
        public Slider slider { get { return gameObject.GetComponentInChildrenIfNull<Slider>(ref _slider, true); } }
            
        public UISliderPopupParameters parameters;
    
        public float sliderValue { get { return slider.value; } }
        public void SetTitle (string txt) {
            titleText.SetText(txt);//, -1);
        }

        public override void UpdateElementLayout(){//bool firstBuild) {
            UIBaseParameters.CopyParameters(ref titleText.parameters, parameters.titleTextParams);
            UIBaseParameters.CopyParameters(ref amountText.parameters, parameters.amountTextParams);
            // Vector2 titleSize = 
            titleText.UpdateElementLayout();//firstBuild);
            // Vector2 amountSize = 
            amountText.UpdateElementLayout();//firstBuild);

            float titleTextYRect = titleText.getHeight;
            float amountTextYRect = amountText.getHeight;


            float sliderHeight = slider.handleRect.sizeDelta.y * parameters.sliderHeadSize.y * sliderRect.localScale.x * 2;
            float fullMessageBoxSize = titleTextYRect + amountTextYRect + sliderHeight + parameters.elementSpacings.x + parameters.elementSpacings.y + (parameters.elementSpacings.z*2);

            float pivotOffset = (1.0f - rectTransform.pivot.y) * fullMessageBoxSize;
            
            rectTransform.sizeDelta = new Vector2(parameters.panelWidth, fullMessageBoxSize);
            
            //title
            titleText.rectTransform.localPosition = new Vector3(0,-(titleTextYRect*.5f + parameters.elementSpacings.z) + pivotOffset, 0);
            
            //amount
            float drawn = (titleTextYRect + parameters.elementSpacings.z) + amountTextYRect*.5f + parameters.elementSpacings.x;
            amountText.rectTransform.localPosition = new Vector3(0,-(drawn) + pivotOffset, 0);            
            
            backgroundImage.imageScale = parameters.sliderBGScale;
            
            //slider
            slider.handleRect.localScale = new Vector3(parameters.sliderHeadSize.x, parameters.sliderHeadSize.y, 1);
            sliderRect.sizeDelta = parameters.sliderBGSize / sliderRect.localScale.x;
            
            backgroundImage.UpdateElementLayout();//firstBuild);

            sliderRect.localPosition = new Vector3(0, -(drawn + sliderHeight + parameters.elementSpacings.y) + pivotOffset, 0);
            
            //controller hints
            actionHintsPanel.transform.localPosition = new Vector3(0, -(fullMessageBoxSize + parameters.hintsSpace) + pivotOffset, 0);
            UIBaseParameters.CopyParameters(ref actionHintsPanel.parameters, parameters.controlHintsParams);
            actionHintsPanel.UpdateElementLayout();//firstBuild);

            UIBaseParameters.CopyParameters(ref panel.parameters, parameters.panelParameters);
            
            panel.UpdateElementLayout();//firstBuild);     
            // return Vector2.zero;  
        }

        protected override 
        void Update()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying) 
                OnValueChanged(sliderValue);    
            #endif

            if (Application.isPlaying) {

                if (isActive) {

                    if (UIManager.CurrentSelected() != slider.gameObject) 
                        UIManager.SetSelection(slider.gameObject);

                    if (UIInput.GetHorizontalAxis() != 0)
                    {
                        // if (Time.unscaledTime - lastTimeUntouched > timeForSpeedUp) {
                        //     speedUpValue = Time.unscaledTime - lastTimeUntouched * speedUpFactor;
                        // }
                    }
                    else {
                        lastTimeUntouched = Time.unscaledTime;
                        // speedUpValue = 0f;
                    }
                }
            }
            base.Update();
        }

        public float timeForSpeedUp = 2;
        public float speedUpFactor = 2;
        public float lastTimeUntouched;
        // public float speedUpValue;
        
        bool extraSpeedUp;
        public void OnValueChanged (float value) {

            // amountText.SetText( "<b>" + value.ToString() + "</b>");//, -1 );
            amountText.SetText( value.ToString() );//, -1 );
            
            if (Application.isPlaying) {

                BroadcastSelectEvent(null, new object[] { value });
                if (!extraSpeedUp) {

                    if (Time.unscaledTime - lastTimeUntouched > timeForSpeedUp) {
                        float speedUpValue = (Time.unscaledTime - lastTimeUntouched) * speedUpFactor;
                        extraSpeedUp = true;
                        slider.value += speedUpValue * UIInput.GetHorizontalAxis();
                    }
                    // if (speedUpValue != 0 ) {
                    // }
                }
                extraSpeedUp = false;
            }

        }

        
        // protected override void OnDisable () {
        //     base.OnDisable();
        //     if (Application.isPlaying) slider.onValueChanged.RemoveListener(OnValueChanged);
        // }
    }
}
