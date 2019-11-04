using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools;
namespace MinimalUI {
    public class UIFourPointCrosshair : UICrosshair
    {
// public override void OnComponentClose() {
//             // base.OnComponentClose();
//         for (int i =0 ; i< images.Length; i++) {
//             images[i].OnComponentClose();
//         }

// }

public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }
        public Vector2 partSize = new Vector2(.1f, .5f);
        UIImage[] _imgs;
        UIImage[] images { get { return gameObject.GetComponentsInChildrenIfNull<UIImage>(ref _imgs, true); } }
        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {    

            Vector2 partSizeReversed = new Vector2(partSize.y, partSize.x);

            // top bottom left right
            for (int i = 0; i < images.Length; i++) {
                
                images[i].SetPivotAndAnchor(new Vector2( 
                    i < 2 ? .5f : (i == 2 ? 1 : 0), 
                    i > 1 ? .5f : (i == 0 ? 0 : 1) 
                ));
                
                Vector2 s = i < 2 ? partSize : partSizeReversed;
                if (images[i].rectTransform.sizeDelta != s)
                    images[i].rectTransform.sizeDelta = s;

                images[i].UpdateElementLayout();//firstBuild, needsSize);
            }
            // return Vector2.zero;
        }
        public override void UpdateCrosshair (float spread) {
            float halfSpread = spread * .5f;
            // top bottom left right
            for (int i = 0; i < images.Length; i++) {
                Vector2 a = new Vector2(
                    i < 2 ? 0 : (i == 3 ? halfSpread : -halfSpread), 
                    i > 1 ? 0 : (i == 0 ? halfSpread : -halfSpread)
                );
                if (images[i].rectTransform.anchoredPosition != a)
                    images[i].rectTransform.anchoredPosition = a;

                // images[i].rectTransform.anchoredPosition = new Vector2(
                //     i < 2 ? 0 : (i == 3 ? halfSpread : -halfSpread), 
                //     i > 1 ? 0 : (i == 0 ? halfSpread : -halfSpread)
                // );
            }
        }
    }
}

