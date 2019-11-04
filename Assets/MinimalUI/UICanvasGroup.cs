// using System.Collections;
// using System.Collections.Generic;

using UnityEngine;
using UnityTools;

namespace MinimalUI {

    [RequireComponent(typeof(CanvasGroup))]
    // [RequireComponent(typeof(Canvas))]
    
    public class UICanvasGroup : UIComponent
    {

        // Canvas _canvas;
        // Canvas canvas { get { return gameObject.GetComponentIfNull<Canvas>(ref _canvas, true); } }

        // public override void OnComponentClose() {
        //     canvas.enabled = false;    
        // }
        // public override void OnComponentOpen () {
        //     canvas.enabled = true;
        // } 
        public override void UpdateElementLayout (){//bool firstBuild, bool needsSize) {
            // return Vector2.zero;
        }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }









    }
}
