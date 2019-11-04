using UnityEngine;
using UnityEngine.UI;

using UnityTools;

using UnityEditor;
using UnityTools.EditorTools;
/*
    keep the color schemes of UIComponents consistent
*/
namespace MinimalUI {

    [ExecuteInEditMode] public abstract class UIGraphic : UIComponent
    {


        static Material _graphicMaterial = null;
        static Material graphicMaterial {
            get {
                if ((object)_graphicMaterial == null) {
                    _graphicMaterial = new Material(Shader.Find("MinimalUI"));
                    _graphicMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _graphicMaterial;
            }
        }

        public bool useOutline = true;
        public bool useShadow = true;

        Outline _outline; 
        protected Outline outline { get { return gameObject.GetOrAddComponent<Outline>(ref _outline, true); } }
        Shadow _shadow; 
        protected Shadow shadow { get { 
            if (_shadow == null || _shadow == outline) {
                Shadow[] allshadows = gameObject.GetComponents<Shadow>(true);
                for (int i = 0; i < allshadows.Length; i++) {
                    if (allshadows[i] != outline) {
                        _shadow = allshadows[i];
                        break;
                    }
                }
                if (_shadow == null) _shadow = gameObject.AddComponent<Shadow>();
            }
            return _shadow;
        } }
        
        protected abstract Vector2 OutlineEffect ();
        protected abstract Vector2 ShadowEffect ();
        
        public UIColorScheme colorScheme;
        public bool useDark, overrideColor;

        protected abstract Graphic MainGraphic () ;

        void UpdateColors () {
            if (isActive)
                _UpdateColors();
        }
        void _UpdateColors () {
            if (!overrideColor) {
                Color32 c = GetUIColor( colorScheme, useDark );
                if (MainGraphic().color != c)
                    MainGraphic().color = c;
            }


            if (outline.enabled != useOutline)
                outline.enabled = useOutline;
            // if (firstBuild) {
            // }
            if (useOutline) {

                if (outline.effectDistance != OutlineEffect())
                    outline.effectDistance = OutlineEffect();

                Color32 oppositeColor = GetUIColor(colorScheme, !useDark );
                if (outline.effectColor != oppositeColor)
                    outline.effectColor = oppositeColor;
            }

            // if (firstBuild) {

                if (shadow.enabled != useShadow)
                    shadow.enabled = useShadow;
            // }
            if (useShadow) {


                
                if (shadow.effectDistance != ShadowEffect())
                    shadow.effectDistance = ShadowEffect();
                // shadow.effectDistance = ShadowEffect();
                // shadow.effectColor = settings.shadowEffectColor;
                if (shadow.effectColor != settings.shadowEffectColor)
                    shadow.effectColor = settings.shadowEffectColor;
            }
            
        }
        public override void UpdateElementLayout (){//bool firstBuild, bool needsSize) {
            
            if (MainGraphic().material != graphicMaterial)
                MainGraphic().material = graphicMaterial;

            _UpdateColors();
            // return Vector2.zero;
        }
            
            
        // protected Color32 SetColorAlpha (Color32 c, byte newAlpha) {
        //     c.a = newAlpha;
        //     return c;
        // }
        // TODO: use canvas group
        // public virtual void SetAlphaMultiplier (float multiplier) {
        //     MainGraphic().color = SetColorAlpha(MainGraphic().color, (byte)(GetUIColor( colorScheme, useDark ).a * multiplier));
        //     if (useOutline) outline.effectColor = SetColorAlpha(outline.effectColor, (byte)(GetUIColor( colorScheme, !useDark ).a * multiplier));
        //     if (useShadow) shadow.effectColor = SetColorAlpha(shadow.effectColor, (byte)(settings.shadowEffectColor.a * multiplier));
        // }



        public void SetColorScheme (UIColorScheme colorScheme, bool useDark){//, bool updateLayout) {
            this.useDark = useDark;
            this.colorScheme = colorScheme;
            // if (updateLayout) UpdateElementLayout();
            _UpdateColors();
        }


        

        // System.Action onColorChange = () => UpdateColors(true);

        protected virtual void Awake () {
            UIManager.onUIColorChange += UpdateColors;
        }

        protected virtual void OnDestroy () {
            UIManager.onUIColorChange -= UpdateColors;        
        }
        
        // protected override void OnEnable() {
        //     base.OnEnable();
        //     UIManager.onUIColorChange += UpdateColors;
        // }

        // protected virtual void OnDisable () {
        //     UIManager.onUIColorChange -= UpdateColors;        
        // }
    }
    
    [ExecuteInEditMode] public abstract class UIGraphic<T> : UIGraphic where T : Graphic
    {
        T _mainGraphic; 
        public T mainGraphic { get { return gameObject.GetOrAddComponent<T>(ref _mainGraphic, true); } }
        protected override Graphic MainGraphic() { return mainGraphic; }
    }
}
