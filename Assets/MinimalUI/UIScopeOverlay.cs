// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityTools;
namespace MinimalUI {

    public class UIScopeOverlay : UIComponent
    {

        void BuildFillersIfNull () {
            if (transform.childCount == 0) {
                for (int i = 0; i < 2; i++) {
                    GameObject newFill = new GameObject("ScopeFill");
                    RectTransform t = newFill.AddComponent<RectTransform>();
                    t.SetParent(transform, Vector3.zero, Quaternion.identity, Vector3.one);
                    StretchRectTopBottom(t);
                    Image image = newFill.AddComponent<Image>();
                    image.color = Color.black;
                    image.raycastTarget = false;
                }
            }
        }

        public void ShowScope(Sprite scopeSprite, float fadeInTime) {
            BuildFillersIfNull();
            mainImage.sprite = scopeSprite;
            // if (!gameObject.activeSelf) {
                UIManager.ShowUIComponent(this, fadeInTime, 0, 0);
            // }
            // else {
                UpdateElementLayout();//true, true);
                // FadeIn(fadeInTime, 0, 0);
            // }
        }
        

        Image[] _allImages;
        Image[] allImages { get { return gameObject.GetComponentsInChildrenIfNull<Image>(ref _allImages, true); } }
        Image mainImage { get { return allImages[0]; } }

        // public override void OnComponentClose() {

            
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }
        



        void StretchRectTopBottom (RectTransform t) {
            
            Vector2 a = new Vector2(.5f, .5f);
            Vector2 b = new Vector2(.5f, 0);
            Vector2 c = new Vector2(.5f, 1);

            if (t.pivot != a)
                t.pivot = a;
            if (t.anchorMin != b)
                t.anchorMin = b;
            if (t.anchorMax != c)
                t.anchorMax = c;


            // t.pivot = new Vector2(.5f, .5f);
            // t.anchorMin = new Vector2(.5f, 0);
            // t.anchorMax = new Vector2(.5f, 1);
        }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            if (Application.isPlaying)
                BuildFillersIfNull();

            StretchRectTopBottom(rectTransform);

            if (mainImage.raycastTarget)
                mainImage.raycastTarget = false;
            
            Vector2 canvasSize = UIMainCanvas.instance.rectTransform.sizeDelta;
            
            float ratio = (float)mainImage.sprite.texture.width / (float)mainImage.sprite.texture.height;
            
            float canvasWidth = canvasSize.x;
            float imgWidth = canvasSize.y * ratio;

            Vector2 targetSize = new Vector2( imgWidth, 0 );

            if (rectTransform.sizeDelta != targetSize)
                rectTransform.sizeDelta = targetSize;


            float leftover = canvasWidth - imgWidth;
            if (leftover < 0)
                leftover = 0;

            // if (leftover > 0) {
                leftover *= .5f;

                float anchoredX = (imgWidth * .5f + leftover * .5f);
                Vector2 fillSize = new Vector2(leftover, 0);
                    
                for (int i = 1; i < allImages.Length; i++) {
                    Image img = allImages[i];
                    // if (!img.gameObject.activeSelf)
                    //     img.gameObject.SetActive(true);
                    
                    if (img.rectTransform.sizeDelta != fillSize)
                        img.rectTransform.sizeDelta = fillSize;
                    
                    Vector2 a = new Vector2(i == 1 ? -anchoredX : anchoredX, 0);
                    if (img.rectTransform.anchoredPosition != a)
                        img.rectTransform.anchoredPosition = a;
                }
            // }
            // else {
            //     for (int i = 1; i < allImages.Length; i++) {
            //         Image img = allImages[i];
            //         if (img.gameObject.activeSelf)
            //             img.gameObject.SetActive(false);
                    
            //     }
            // }
            // return Vector2.zero;
        }
        
    }
}
