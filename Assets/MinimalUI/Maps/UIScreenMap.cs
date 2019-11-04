

using UnityEngine;

using UnityTools;

namespace MinimalUI {
    public class UIScreenMap : UIComponent
    {
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            baseMapParameters.OnComponentEnable();
            

        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            baseMapParameters.OnComponentDisable();
        }

        

        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     baseMapParameters.OnComponentClose();
        // }
        
        static UIScreenMap _instance;
        public static UIScreenMap instance { get { return Singleton.GetInstance<UIScreenMap>(ref _instance, true); } }   
        
        public UIBaseMap baseMapParameters;


        void Awake () {
            baseMapParameters.OnEnable(transform, OnMarkerAdd, null);
        }
            
        // protected override void OnEnable() {
        //     baseMapParameters.OnEnable(transform, OnMarkerAdd, null);
        //     base.OnEnable();
        // }

        public override void UpdateElementLayout(){//bool firstBuild) {
            // return 
            baseMapParameters.UpdateElementLayout();//firstBuild);
            
        }

        // protected override 
        // void OnMarkerRemove(MapMarker marker) {
            
        // }
        // protected override 
        void OnMarkerAdd (MapMarker marker) {
            marker.ShowMessage(false);
            marker.ShowDirectionals(false);
            marker.ShowDistance(true);
            marker.showRotation = false;
        }

        // protected override 
        void Update() {
            // base.Update();

            if (!Application.isPlaying)
                return;
            
            if (!GameManager.playerExists)
                return;

            // if (GameManager.isInMainMenuScene)
            //     return;


            baseMapParameters.UpdateMapMarkers(GameManager.playerActor.GetPosition());


            Camera playerCamera = GameManager.playerCamera;
            if (playerCamera == null)
                return;

            if (!isActive)
                return;

            if (UIMainCanvas.instance == null)
                return;

            float borderOffset = baseMapParameters.markerParameters.iconSize * transform.localScale.x * 2;

            Vector2 fullSize = ((UIMainCanvas.instance.rectTransform.sizeDelta - Vector2.one * borderOffset) / transform.localScale.x);
            Vector2 halfSize = fullSize * .5f;

            for (int i = 0; i < baseMapParameters.mapMarkers.Count; i++) {
                MapMarker m = baseMapParameters.mapMarkers[i];
                Vector3 worldPos = m.GetMarkerPosition();

                Vector3 screenPos = playerCamera.WorldToViewportPoint(worldPos);

                if (screenPos.z < 0) {
                    screenPos.x = 1f - screenPos.x;
                    screenPos.y = 0;
                }
                else {
                    screenPos.y = Mathf.Clamp01(screenPos.y);
                }

                Vector2 pos = new Vector2(
                    screenPos.x * fullSize.x - halfSize.x, 
                    screenPos.y * fullSize.y - halfSize.y 
                );

                /*
                    \  |
                    x\a|   panel radius
                      \|

                    cos (a) = panel radius / x
                    cos (a) * x = panel radius
                    x = panelRadius / cos(a)

                    max angle before switching hypotenuse calculation to use width:
                    
                    panel x radius
                    _____
                     \  |
                      \a|   panel h radius
                       \|

                    a = maxAngle
                    tan (maxAngle) = (panel x radius) / (panel y radius)
                */
                float maxAngle = Mathf.Atan(halfSize.x / halfSize.y) * Mathf.Rad2Deg;

                float adjacentSize = halfSize.y;                
                float angle = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
                angle = angle < 0 ? -angle : angle;
                angle = angle > (180 - maxAngle) ? 180 - angle : angle;

                if (angle >= maxAngle) {
                    adjacentSize = halfSize.x;
                    angle = angle > 90 ? (angle - 90) : (90 - angle);
                }

                float hypotenuse = adjacentSize / Mathf.Cos(angle * Mathf.Deg2Rad);
                
                
                /*
                     \  |
                    x \a| <-panel radius
                       \|

                    cos (a) = panel radius / x      ->
                    cos (a) * x = panel radius      ->                    
                    x = panelRadius / cos(a)
                */


                // float angle = Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
                // angle = angle < 0 ? -angle : angle;
                // angle = angle > 135 ? angle - 90 : angle;
                

                // float adjacentSize = halfSize.y;                
                // if (angle >= 45) {
                //     angle = angle > 90 ? (angle - 90) : (90 - angle);
                //     adjacentSize = halfSize.x;
                // }

                // float hypotenuse = (adjacentSize / Mathf.Cos(angle * Mathf.Deg2Rad));
                        
                pos = Vector2.ClampMagnitude(pos, hypotenuse);
                m.rectTransform.anchoredPosition = pos;
            }
        }
    }
}
