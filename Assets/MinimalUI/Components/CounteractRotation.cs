// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace MinimalUI {
    [ExecuteInEditMode] public class CounteractRotation : UIComponent
    {
        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }
        public Transform antiRotate;
        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // return Vector2.zero;
        }
        // protected override 
        void Update() {
            // base.Update();
            if (antiRotate != null) {
                if (isActive) {
                    transform.localRotation = Quaternion.Euler(0, 0, -antiRotate.rotation.eulerAngles.z);
                }
            }
        }
    }
}
