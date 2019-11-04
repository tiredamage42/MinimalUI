using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityTools;
namespace MinimalUI {
    [ExecuteInEditMode] public class FilledCircleSegment : UIComponent
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
        
        [Range(1, 90)] public float maxFill = 45;
        [Range(0,1)] public float currentFill = 1;
        public void SetFill (float fill) {
            currentFill = Mathf.Clamp01(fill);
            UpdateElementLayout();//false, true);
        }

        Image img;
        Image image { get { return gameObject.GetComponentIfNull<Image>(ref img, true); } }

        public override void UpdateElementLayout (){//bool firstBuild, bool needsSize) {
            
            // if (firstBuild){

            // }   

            if (image.type != Image.Type.Filled)
                image.type = Image.Type.Filled;
            
            if (image.fillMethod != Image.FillMethod.Radial360)
                image.fillMethod = Image.FillMethod.Radial360;
            

            float f = (maxFill / 360f) * currentFill;
            if (image.fillAmount != f)
                image.fillAmount = f;
                
            transform.localRotation = Quaternion.Euler(0, 0, maxFill * currentFill * .5f);
            
            // return Vector2.zero;
        }


        // protected override 
        void Update() {
            // base.Update();
            if (!Application.isPlaying) {
                SetFill ( currentFill );
            }
        }



        
    }
}
