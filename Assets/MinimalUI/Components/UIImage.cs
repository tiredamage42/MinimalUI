using UnityEngine;
using UnityEngine.UI;
using UnityTools;
namespace MinimalUI {

    /* used for keeping ui image color consistent */
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode] public class UIImage : UIGraphic<Image> { 
        
        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }
        public Vector2 outlineEffect = new Vector2(.01f, .01f);
        public Vector2 shadowEffect = new Vector2(.01f, -.01f);

        protected override Vector2 OutlineEffect() { return outlineEffect; }
        protected override Vector2 ShadowEffect() { return shadowEffect; }
        
        public bool useReadySprite = true;
        [MinimalUISprite] public int mySprite;
        public bool isScaled;
        public bool parentScaled;
        public float imageScale = 1;
        StretchToFit stretchToFit;

        protected override void Awake() {
            base.Awake();
            stretchToFit = GetComponent<StretchToFit>();
            
        }


        // string mainGraphicName;
        // protected override void OnEnable() {
        //     stretchToFit = GetComponent<StretchToFit>();
        //     base.OnEnable();
        // }


        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            base.UpdateElementLayout();//firstBuild, needsSize);

            if (mainGraphic.raycastTarget)
                mainGraphic.raycastTarget = false;
            
            if (useReadySprite) {
                
                if (mainGraphic.fillCenter)
                    mainGraphic.fillCenter = true;

                Sprite s = settings.customSprites[ mySprite ];
                if ((object)mainGraphic.sprite != s) {
                    
                    mainGraphic.sprite = s;
                    // mainGraphicName = s.name;
                    
                    if (mainGraphic.type != Image.Type.Filled) {
                        if (s != null) {
                            Image.Type type = s.name.Contains("@") ? Image.Type.Sliced :Image.Type.Simple;
                            if (mainGraphic.type != type)
                                mainGraphic.type = type;
                            // mainGraphic.type = s.name.Contains("@") ? Image.Type.Sliced :Image.Type.Simple;
                        }
                    }
                // if (string.IsNullOrEmpty(mainGraphicName)) {
                //     mainGraphicName = s.name;
                // }

                }
            }
            if (isScaled) {
                float s = (imageScale * (parentScaled ? (1f/transform.parent.localScale.x) : 1));
                if (transform.localScale.x != s)
                    transform.localScale = Vector3.one * s;
            }
            if ((object)stretchToFit != null) 
                stretchToFit.UpdateElementLayout( );//firstBuild, needsSize );

            // return Vector2.zero;
        }        
    }
}
