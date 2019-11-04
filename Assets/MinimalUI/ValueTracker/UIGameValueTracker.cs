using UnityEngine;

using UnityTools;
using UnityTools.EditorTools;

namespace MinimalUI {

    [RequireComponent(typeof(UIValueTracker))]
    public class UIGameValueTracker : UIComponent
    {
        // public override void OnComponentClose() {
        //     uiObject.OnComponentClose();
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }


        UIValueTracker _uiObject;
        UIValueTracker uiObject { get { return gameObject.GetComponentIfNull<UIValueTracker>( ref _uiObject, true ); } }
        
        [Header("Invalid/Warning Thresholds")]
        [NeatArray(2)] public NeatFloatArray colorSchemeThresholds;
        
        GameValue gameValue;

        bool initialized;
        public void SetGameValue (GameValue gameValue) {
            initialized = false;
            if (this.gameValue != null) {
                this.gameValue.RemoveChangeListener(UpdateUIObject);
            }

            this.gameValue = gameValue;
            InitializeWithGameValue();
        }

        public void UntrackGameValue () {
            if (gameValue != null) {
                gameValue.RemoveChangeListener(UpdateUIObject);
                initialized = false;
            }            
        }

        void InitializeWithGameValue () {
            if (gameValue != null) {
                if (!initialized) {
                    uiObject.text.SetText(gameValue.name);
                    gameValue.AddChangeListener(UpdateUIObject);
                    UpdateUIObject(0, gameValue.GetValue(), gameValue.GetMinValue(), gameValue.GetMaxValue());
                    initialized = true;    
                }
            }
        }
        
        // protected override void OnEnable () {
        //     base.OnEnable();
            // InitializeWithGameValue();    
        // }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // return Vector2.zero;
        }
            
        // void OnDisable () {
            // if (this.gameValue != null) {
            //     this.gameValue.RemoveChangeListener(UpdateUIObject);
            //     initialized = false;
            // }            
        // }

        public event System.Action<float, float, float, float> onGameValueChange;

        void UpdateUIObject (float delta, float newValue, float min, float max) {
            if (onGameValueChange != null) {
                onGameValueChange(delta, newValue, min, max);
            }

            UIColorScheme scheme = UIColorScheme.Normal;
            
            for (int i = 0; i < colorSchemeThresholds.Length; i++) {
                if (newValue <= colorSchemeThresholds[i]) {
                    scheme = i == 0 ? UIColorScheme.Invalid : UIColorScheme.Warning;
                    break;
                }
            }

            if (uiObject != null) uiObject.SetValue(Mathf.InverseLerp(min, max, newValue), scheme);
        }
    }
}
