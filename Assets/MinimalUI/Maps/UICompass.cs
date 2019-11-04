using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;

namespace MinimalUI {
    public class UICompass : UIObject// UIBaseMap
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
        //     base.OnComponentClose();
        //     baseMapParameters.OnComponentClose();

        //     for (int i = 0; i < imgs.Length; i++)
        //         imgs[0].OnComponentClose();
        // }
        

        public UIBaseMap baseMapParameters;


        void Awake () {
            if (Application.isPlaying) baseMapParameters.OnEnable(transform, OnMarkerAdd, null);
        }
        // protected override void OnEnable() {
        //     baseMapParameters.OnEnable(transform, OnMarkerAdd, null);
        //     base.OnEnable();
        // }


        static UICompass _instance;
        public static UICompass instance { get { return Singleton.GetInstance<UICompass>(ref _instance, true); } }   
        // protected override void OnMarkerRemove(MapMarker marker) {
            
        // }
        // protected override
        void OnMarkerAdd (MapMarker marker) {
            marker.ShowMessage(false);
            marker.ShowDistance(false);
            marker.ShowDirectionals(!UIMaps.RegisteredObjectIsPlayer( marker.uIRegisteredObject ));
            marker.showRotation = false;
        }

        public Vector2 size = new Vector2(3, .125f);
        public float centerWidth = .05f;

        UIImage background { get { return imgs[0]; } }
        UIImage centerReference { get { return imgs[1]; } }

        UIImage[] _imgs;
        UIImage[] imgs { get { return gameObject.GetComponentsInChildrenIfNull<UIImage>(ref _imgs, true); } }



        public override void UpdateElementLayout(){//bool firstBuild) {
            baseMapParameters.UpdateElementLayout();//firstBuild);

            rectTransform.sizeDelta = size;
            background.UpdateElementLayout();//firstBuild);
            centerReference.rectTransform.sizeDelta = new Vector2(centerWidth, size.y + .05f);
            // return 
            centerReference.UpdateElementLayout();//firstBuild);
        }


        [Tooltip("Distance To Start Damping Angle (To Avoid Close Object Jumping Around The Compass")]
        public float distanceBuffer = 5;
        public float distanceBufferSteepness = 5;
        public float angleRange = 75;
        [Tooltip("Degrees Above Angle Range Where The Object Stays On The Compass")]
        public float outOfAngleBuffer = 15;

        // protected override 
        void Update() {
            // base.Update();

            if (!Application.isPlaying)
                return;

            if (!GameManager.playerExists)
                return;

            if (!isActive)
                return;

            Actor playerActor = GameManager.playerActor;
            // if (playerActor == null)
            //     return;

            Vector3 playerPos = playerActor.GetPosition();
            baseMapParameters.UpdateMapMarkers(playerPos);




            Vector3 playerFwd = playerActor.GetForward();
            playerFwd.y = 0;
            Vector3 playerRight = Vector3.Cross(playerFwd, Vector3.up);

            // set position of icons based on angle with camera
            // Transform playerTransform = UIManager.GetMainCamera().transform;
            // Vector3 playerPosition = playerTransform.position;
            // Vector3 playerFwd = playerTransform.forward;
            // Vector3 playerRight = playerTransform.right;
            // playerFwd.y = 0;
            // playerRight.y = 0;

            float halfSize = size.x * .5f;

            float maxAngle = angleRange + outOfAngleBuffer;


            for (int i = 0; i < baseMapParameters.mapMarkers.Count; i++) {
                MapMarker m = baseMapParameters.mapMarkers[i];

                Vector3 dir = m.GetMarkerPosition() - playerPos;

                float distance = dir.magnitude;

                // angle goes to zero the closer the object is, so close markers dont jump around compass
                float mult = Mathf.Pow(distance / distanceBuffer, distanceBufferSteepness);
                
                if (mult > 1)
                    mult = 1;

                dir.y = 0;

                float angleWFwd = Vector3.Angle(playerFwd, dir) * mult;

                bool markerEnabled = ( angleWFwd <= maxAngle );

                m.EnableMapMarker(markerEnabled);

                if (markerEnabled) {

                    float t = angleWFwd / angleRange;
                    if (t > 1)
                        t = 1;

                    bool toRight = Vector3.Angle(playerRight, dir) < 90;

                    m.rectTransform.anchoredPosition = new Vector2( t * (toRight ? halfSize : -halfSize), 0 );
                }
            }
        }
    }
}
