using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
using System;

namespace MinimalUI {
    public class UIDirectionIndicator : UIObject
    {
        static PrefabPool<UIDirectionalIcon> iconPool = new PrefabPool<UIDirectionalIcon>();
        List<UIDirectionalIcon> icons = new List<UIDirectionalIcon>();


        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     for (int i = 0; i < icons.Count; i++) {
        //         icons[i].OnComponentClose();
        //     }
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }

        public UIDirectionalIcon AddDirectionalIcon ( UIDirectionalIcon prefab, Func<Vector3> getWorldPosition ) {
            
            UIDirectionalIcon icon = iconPool.GetAvailable(prefab, null, false, null);
            icon.SetWorldPositionGetter(getWorldPosition);
            icons.Add(icon);
            icon.transform.SetParent(transform, Vector3.zero, Quaternion.identity, Vector3.one);        
            // icon.UpdateElementLayout();
            icon.gameObject.SetActive(true);
            UpdateElementLayout();//true);
            return icon;
        }

        public UIDirectionalIcon RemoveDirectionalIcon (UIDirectionalIcon icon) {
            if (icons.Contains(icon)) {
                icons.Remove(icon);
                icon.gameObject.SetActive(false);   
                icon.transform.SetParent(UIMainCanvas.instance.transform);
            }
            return null;
        }

        // static UIDirectionIndicator _instance;
        // public static UIDirectionIndicator instance { get { return Singleton.GetInstance<UIDirectionIndicator>(ref _instance, true); } }
        
        public override void UpdateElementLayout(){//bool firstBuild) {
            if (Application.isPlaying) {
                for (int i = 0; i < icons.Count; i++) {
                    icons[i].UpdateElementLayout();//firstBuild);
                }
            } 
            // return Vector2.zero;  
        }

        // protected override 
        void Update() {
            // base.Update();

            if (!Application.isPlaying)
                return;

            if (!isActive)
                return;

            // if (UIMainCanvas.instance == null)
            //     return;

            Camera playerCamera = GameManager.playerCamera;
            if (playerCamera == null)
                return;

            Vector3 playerCameraFwd = playerCamera.transform.forward;
            playerCameraFwd.y = 0;

            Vector3 playerCameraPos = playerCamera.transform.position;

            Vector3 playerCameraRight = playerCamera.transform.right;
            playerCameraRight.y = 0;


            
            Vector2 fullSize = ((UIMainCanvas.instance.rectTransform.sizeDelta) / transform.localScale.x);// * .5f;
            Vector2 halfSizeFull = fullSize * .5f;

            // Vector2 clampedSize = ((canvasRect.sizeDelta - Vector2.one * markerParameters.iconSize * transform.localScale.x * 2) / transform.localScale.x);// * .5f;
            // Vector2 halfSizeClamped = clampedSize * .5f;

            for (int i = 0; i < icons.Count; i++) {
                UIDirectionalIcon m = icons[i];
                Vector3 worldPos = m.worldPosition;

                // if (m.useScreenPos) {

                    Vector3 screenPos = playerCamera.WorldToViewportPoint(worldPos);


                    if (screenPos.z < 0)
                    {
                        screenPos.x = 1f - screenPos.x;                    
                        screenPos.y = 0;
                    }
                    else {
                        if (m.keepForward) {
                            screenPos.y = 1;
                        }
                        screenPos.y = Mathf.Clamp01(screenPos.y);
                    }

                    Vector2 pos = new Vector2(
                        screenPos.x * fullSize.x - halfSizeFull.x, 
                        screenPos.y * fullSize.y - halfSizeFull.y
                    );
                    
                    float targetAngle = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;        
                    m.transform.localRotation = Quaternion.Euler(0, 0, -targetAngle);

                // }
                // else {
                //     Vector3 dir = worldPos - playerCameraPos;
                //     dir.y = 0;

                //     float angle = Vector3.Angle(playerCameraFwd, dir);

                //     if (Vector3.Angle(dir, playerCameraRight) < 90) {
                //         angle = -angle;
                //     }

                //     m.transform.localRotation = Quaternion.Euler(0, 0, angle);
                // }


                    if (m.mainGraphic != null) {

                        // m.mainGraphic.transform.localPosition = Vector3.up * m.iconDistance;
                        m.mainGraphic.rectTransform.anchoredPosition = new Vector2(0, m.iconDistance * halfSizeFull.y);
                    }



            }
        }
    }
}
