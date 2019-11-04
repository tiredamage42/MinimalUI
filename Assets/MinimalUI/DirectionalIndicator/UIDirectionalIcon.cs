using UnityEngine;
using System;

namespace MinimalUI {
    public class UIDirectionalIcon : UIComponent
    {
        public float iconDistance;   
        public bool keepForward;

        public bool sizeRelativeToCanvas;
        public float iconSizeMultiplier = 1;
        public UIGraphic mainGraphic;

        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     mainGraphic.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }

        public Vector3 worldPosition { get { return positionGetter(); } }
        Func<Vector3> positionGetter;
        public void SetWorldPositionGetter (Func<Vector3> positionGetter) {
            this.positionGetter = positionGetter;
        }
        public override void UpdateElementLayout(){//bool firstBuild) {
            if (mainGraphic != null) {
                if (sizeRelativeToCanvas) {

                    if (transform.parent != null) {
                        float h = UIMainCanvas.instance.rectTransform.sizeDelta.y / transform.parent.localScale.x;
                        mainGraphic.rectTransform.sizeDelta = Vector2.one * iconSizeMultiplier * h;
                    }
                }
                else {
                    mainGraphic.rectTransform.sizeDelta = Vector2.one * iconSizeMultiplier;
                }
                mainGraphic.UpdateElementLayout();//firstBuild);
            }     
            // return Vector2.zero;       
        }
    }
}