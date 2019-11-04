using UnityEngine;

namespace MinimalUI {
    [ExecuteInEditMode] public class StretchToFit : UIComponent
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
        RectTransform _transformToFit;
        RectTransform transformToFit {
            get {
                if (_transformToFit == null && transform.parent != null) _transformToFit = transform.parent.GetComponent<RectTransform>();
                return _transformToFit;
            }
        }

        public override void UpdateElementLayout (){//bool firstBuild, bool needsSize) {
            if ((object)transformToFit != null) {
                Vector2 sizeDelta = transformToFit.sizeDelta * (1f/transform.localScale.x);
                if (rectTransform.sizeDelta != sizeDelta)
                    rectTransform.sizeDelta = sizeDelta;
                
                if (rectTransform.anchoredPosition != Vector2.zero)
                    rectTransform.anchoredPosition = Vector2.zero;
            }
            // return Vector2.zero;
        }
    }
}
