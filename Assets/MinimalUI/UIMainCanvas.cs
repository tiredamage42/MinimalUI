using UnityEngine;
using UnityTools;
namespace MinimalUI {
    public class UIMainCanvas : UIComponent
    {
        static UIMainCanvas _instance;
        public static UIMainCanvas instance { get { return Singleton.GetInstance<UIMainCanvas>(ref _instance, true); } }

        // Canvas _canvas;
        // public Canvas canvas { get { return gameObject.GetComponentIfNull<Canvas>(ref _canvas, true); } }
        
        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // return Vector2.zero;
        }
        // public override void OnComponentClose() {
        
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }

    }
}
