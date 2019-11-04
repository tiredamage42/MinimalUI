
using UnityEngine;
using UnityEngine.UI;
using UnityTools;


namespace MinimalUI {
    // [ExecuteInEditMode] 
    public class UIMiniMap : UIObject
    {

        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     baseMapParameters.OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            baseMapParameters.OnComponentEnable();
            

        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            baseMapParameters.OnComponentDisable();
        }

        
        
        static UIMiniMap _instance;
        public static UIMiniMap instance { get { return Singleton.GetInstance<UIMiniMap>(ref _instance, true); } }   
        
        public UIBaseMap baseMapParameters;
        // protected override void OnEnable() {
        //     // if (Application.isPlaying) 
        //     baseMapParameters.OnEnable(transform, OnMarkerAdd, null);
        //     base.OnEnable();
        // }
        void Awake () {
            // if (Application.isPlaying) {
                // if (SetUIComponentInstance<UIMiniMap>(ref _instance, this)) {
                    baseMapParameters.OnEnable(transform, OnMarkerAdd, null);
                    // UIManager.HideUI(this);
                // }
            // }
        }


        
        // protected override void OnMarkerRemove(MapMarker marker) {
            
        // }
        // protected override 
        void OnMarkerAdd (MapMarker marker) {
            marker.ShowMessage(false);
            marker.ShowDistance(false);

            marker.ShowDirectionals(!UIMaps.RegisteredObjectIsPlayer( marker.uIRegisteredObject ));

            marker.showRotation = marker.showRotation && !UIMaps.RegisteredObjectIsPlayer( marker.uIRegisteredObject );
        }


        public UIPanelParameters panelParameters;

        public Vector2 size = new Vector2(2, 2);

        RawImage _mapImage;
        RawImage mapImage { get { return gameObject.GetComponentInChildrenIfNull<RawImage>(ref _mapImage, true); } }


        public override void UpdateElementLayout(){//bool firstBuild) {
            baseMapParameters.UpdateElementLayout();//firstBuild);

            panel.rectTransform.sizeDelta = size;
            UIBaseParameters.CopyParameters(ref panel.parameters, panelParameters);

            
            panel.UpdateElementLayout();//firstBuild);

            mapImage.rectTransform.sizeDelta = panel.rectTransform.sizeDelta;

            // return Vector2.zero;
        }


        const string miniMapLayer = "MiniMap";

        Camera BuildMinimapCamera () {
            Transform mapCameraParent = new GameObject("MinimapCameraRoot").transform;
            Camera camera = new GameObject("MinimapCamera").AddComponent<Camera>();
            camera.transform.SetParent(mapCameraParent, Vector3.zero, Quaternion.Euler(90,0,0), Vector3.one);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.cullingMask = 1 << LayerMask.NameToLayer(miniMapLayer);   
            camera.orthographic = true;
            camera.nearClipPlane = 1f;
            camera.farClipPlane = 100;

            camera.useOcclusionCulling = true;
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.allowDynamicResolution = false;

            cameraRenderTexture = RenderTexture.GetTemporary(
                256, 256, 
                depthBuffer: 0, 
                RenderTextureFormat.ARGB32, 
                RenderTextureReadWrite.sRGB, 
                antiAliasing: 1, 
                RenderTextureMemoryless.Color, 
                VRTextureUsage.None, 
                useDynamicScale: false
            );
            camera.targetTexture = cameraRenderTexture;
            mapImage.texture = cameraRenderTexture;

            mapImage.color = GetUIColor(UIColorScheme.Normal, false);

            return camera;
        }
        RenderTexture cameraRenderTexture;

        void OnDestroy () {
            if (cameraRenderTexture != null) {
                RenderTexture.ReleaseTemporary(cameraRenderTexture);
            }
        }

            
        Camera _uiCamera;
        Camera uiCamera {
            get {
                if (_uiCamera == null) _uiCamera = BuildMinimapCamera();
                return _uiCamera;
            }
        }


        [Range(1, 25)] public float cameraArea = 10;

        // protected override 
        void Update() {
            // base.Update();

            if (!Application.isPlaying) //{
                return;
            // }

            // if (GameManager.isInMainMenuScene)
            //     return;

            if (!GameManager.playerExists)
                return;
            
            if (!isActive)
                return;

            // Camera playerCamera = GameManager.playerCamera;
            // if (playerCamera == null)
            //     return;


            Actor playerActor = GameManager.playerActor;
            // if (playerActor == null)
            //     return;

            Vector3 playerPos = playerActor.GetPosition();

            baseMapParameters.UpdateMapMarkers(playerPos);


            Vector3 playerFwd = playerActor.GetForward();
            playerFwd.y = 0;

            // Transform t = UIManager.GetMainCamera().transform;

            // if (t != null) {
                // Vector3 playerPos = t.position;
                // Vector3 playerFwd = t.forward;
                // playerFwd.y = 0;

                float playerAngle = Vector3.Angle(playerFwd, Vector3.forward);
                if (Vector3.Angle(playerFwd, Vector3.right) > 90) {
                    playerAngle *= -1;
                }
                                
                Transform parent = uiCamera.transform.parent;
                parent.position = playerPos + Vector3.up * uiCamera.farClipPlane * .5f;
                parent.rotation = Quaternion.LookRotation(playerFwd);

                float cameraSize = cameraArea * .5f;
                uiCamera.orthographicSize = cameraSize;
                uiCamera.Render();

                Vector2 halfSize = size * .5f;

                for (int i = 0; i < baseMapParameters.mapMarkers.Count; i++) {
                    MapMarker m = baseMapParameters.mapMarkers[i];

                    Vector3 wPos = m.GetMarkerPosition();

                    Vector3 lPos = parent.InverseTransformPoint(wPos);

                    Vector2 anchorPos_unMoved = new Vector2(lPos.x / cameraSize * halfSize.x, lPos.z / cameraSize * halfSize.y);
                    Vector2 anchorPos = anchorPos_unMoved + halfSize;
                    
                    bool markerEnabled = m.alwaysShowMiniMap || (anchorPos.x >= 0 && anchorPos.y >= 0 && anchorPos.x <= size.x && anchorPos.y <= size.y);
                    if (m.alwaysShowMiniMap) {

                        
                        /*
                             \  |
                            x \a|   panel radius
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
                        float angle = Mathf.Atan2(anchorPos_unMoved.x, anchorPos_unMoved.y) * Mathf.Rad2Deg;
                        angle = angle < 0 ? -angle : angle;
                        angle = angle > (180 - maxAngle) ? 180 - angle : angle;

                        if (angle >= maxAngle) {
                            adjacentSize = halfSize.x;
                            angle = angle > 90 ? (angle - 90) : (90 - angle);
                        }

                        float hypotenuse = adjacentSize / Mathf.Cos(angle * Mathf.Deg2Rad);
                        
                        anchorPos_unMoved = Vector2.ClampMagnitude(anchorPos_unMoved, hypotenuse);
                        anchorPos = anchorPos_unMoved + halfSize;
                    }

                    m.EnableMapMarker( markerEnabled);

                    if (markerEnabled) {
                        m.rectTransform.anchoredPosition = anchorPos;

                        if (m.showRotation) {
                            if (m.mainGraphic != null) {
                                Vector3 fwd = m.GetMarkerForward();
                                fwd.y = 0;
                                float angle = Vector3.Angle(fwd, Vector3.forward);
                                if (Vector3.Angle(fwd, Vector3.right) > 90) {
                                    angle *= -1;
                                }
                                m.mainGraphic.rectTransform.localRotation = Quaternion.Euler(0, 0, -angle + playerAngle);
                            }
                        }
                    }
                }
            // }
        }
    }
}
