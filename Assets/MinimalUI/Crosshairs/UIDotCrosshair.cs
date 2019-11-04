// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityTools;
namespace MinimalUI {
    public class UIDotCrosshair : UICrosshair
    {
        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     image.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }
        UIImage _img;
        UIImage image { get { return gameObject.GetComponentIfNull<UIImage>(ref _img, true); } }
        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {    
            // return 
            image.UpdateElementLayout();//firstBuild, needsSize);
        }
        public override void UpdateCrosshair (float spread) {

        }
    }
}
