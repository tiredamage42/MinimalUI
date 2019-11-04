using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
using UnityTools.EditorTools;
using UnityEngine.UI;
namespace MinimalUI {

    // [ExecuteInEditMode] 
    public class UIMainMap : UIWithInput
    {
        public override void SetSelectablesActive(bool active) { }
        public override bool CurrentSelectedIsOurs (GameObject selected) { return true; }
        // protected override bool IsPopup () { return false; }



        static UIMainMap _instance;
        public static UIMainMap instance { get { return Singleton.GetInstance<UIMainMap>(ref _instance, true); } }   
        public UIBaseMap baseMapParameters;

        void Awake () {
            if (Application.isPlaying) {
                // if (SetUIComponentInstance<UIMainMap>(ref _instance, this)) {
                    baseMapParameters.OnEnable(transform, OnMarkerAdd, OnMarkerRemove);
                    // UIManager.HideUI(this);

                    // UIManager.HideUIComponent(this, 0);
                // }
            }
        }

        public override void OnComponentEnable () {
            base.OnComponentEnable();
            baseMapParameters.OnComponentEnable();
            // if (Application.isPlaying) {
            // }
            UIMaps.onObjectDeregister += OnObjectDeregister;

        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();          
            baseMapParameters.OnComponentDisable();
            
            UIMaps.onObjectDeregister -= OnObjectDeregister;
        }



        // protected override void OnEnable() {
        //     // baseMapParameters.OnEnable(transform, OnMarkerAdd, OnMarkerRemove);
        //     base.OnEnable();

        //     if (Application.isPlaying) {
        //         UIMaps.onObjectDeregister += OnObjectDeregister;
        //     }
        // }


        void OnDrawGizmosSelected () {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(mapCenter.x, 0, mapCenter.y), new Vector3(mapRadius * 2, 10, mapRadius * 2));
        }

        public Vector2 mapCenter = new Vector2(25, 25);
        public float mapRadius = 25;

        public float cursorSize = .1f;



        public float moveSpeed = 1;
        public float zoomSpeed = 1;

        public float mapSize = 5;
        public UIPanelParameters panelParameters;

        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     baseMapParameters.OnComponentClose();
        //     cursor.OnComponentClose();
        // }
        


        public UIImage cursor;


        // public Vector2 moveOffset;

        RawImage _mapImage;
        RawImage mapImage { get { return gameObject.GetComponentInChildrenIfNull<RawImage>(ref _mapImage, true); } }

        // UIPanel _panel;
        // UIPanel panel { get { return gameObject.GetComponentInChildrenIfNull<UIPanel>(ref _panel); } }


        public override void UpdateElementLayout(){//bool firstBuild) {
            baseMapParameters.UpdateElementLayout();//firstBuild);

        // public override void UpdateElementLayout() {
        //     base.UpdateElementLayout();
            panel.rectTransform.sizeDelta = Vector2.one * mapSize;
            
            UIBaseParameters.CopyParameters(ref panel.parameters, panelParameters);
                
            panel.UpdateElementLayout();//firstBuild);

            mapImage.rectTransform.sizeDelta = panel.rectTransform.sizeDelta;
            
            cursor.rectTransform.sizeDelta = Vector2.one * cursorSize;

            // return Vector2.zero;
        }
        [Range(1, 10)] public float maxZoom = 5;
        float zoom = 1;
        Vector2 move;

        // public float selectedMarkerSize = 1.2f;
        public float snapDist = .1f;


        // protected override 
        void OnMarkerRemove(MapMarker marker) {
            if (selectedMarker != null) {
                if (marker == selectedMarker) {
                    DeselectMarker(marker);
                }
            }
        }


        void ShowExtras (MapMarker marker, bool show) {
            marker.ShowMessage(show);
            marker.ShowDistance(show);
        }
        // protected override 
        void OnMarkerAdd (MapMarker marker) {
            ShowExtras(marker, false);
            marker.ShowDirectionals(false);
        }

        void SelectMarker (MapMarker marker) {
            ShowExtras(marker, true);
            selectedMarker = marker;
        }
        void DeselectMarker (MapMarker marker) {
            ShowExtras(marker, false);
            selectedMarker = null;
        }


        public float unsnapTime = .25f;

        bool unsnapProhibit;
        float snappedTime;
        MapMarker selectedMarker;
        protected override 
        void Update() {
            base.Update();
            if (!Application.isPlaying) return;

            if (!isActive) return;


            if (!GameManager.playerExists)
                return;


            baseMapParameters.UpdateMapMarkers(GameManager.playerActor.GetPosition());

            
            zoom -= UIInput.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
            zoom = Mathf.Clamp(zoom, 1, maxZoom);

            float z = 1f/zoom;
            float speed = moveSpeed * Time.deltaTime * z;

            float inputX = UIInput.GetHorizontalAxis ();
            float inputY = UIInput.GetVerticalAxis ();

            if (unsnapProhibit) {
                if (inputX == 0 && inputY == 0 || Time.time - snappedTime >= unsnapTime) {
                    unsnapProhibit = false;
                }
                inputX = 0;
                inputY = 0;
            }


            move.x = Mathf.Clamp(move.x + inputX * speed, -.5f, .5f);
            move.y = Mathf.Clamp(move.y + inputY * speed, -.5f, .5f);
            
            float zOffset = (-0.5f) * z + 0.5f;
            
            Vector2 move2 = move * 2;
            float halfMapUI = mapSize * .5f;
            float halfMapUIZoom = halfMapUI * zoom;


            
            for (int k = 0; k < baseMapParameters. mapMarkers.Count; k++) {
            // foreach (var k in mapMarkers.Keys) {
                MapMarker m = baseMapParameters.mapMarkers[k];

                Vector3 wPos = m.GetMarkerPosition();
                Vector2 uiPos = (((new Vector2(wPos.x, wPos.z) - mapCenter) / mapRadius) - move2) * halfMapUIZoom;
                
                bool markerEnabled = uiPos.x <= halfMapUI && uiPos.x >= -halfMapUI && uiPos.y <= halfMapUI && uiPos.y >= -halfMapUI;
                m.EnableMapMarker(markerEnabled);
                
                if (markerEnabled && uiPos.sqrMagnitude <= snapDist) {
                    if (selectedMarker == null){// && (markerToSelect == null || markerToSelect == currentWaypointMarker)) {
                        // markerToSelect = m;
                        move += (uiPos / halfMapUI) / 2 * z;
                        uiPos = Vector2.zero;
                        
                        // // move.x += (uiPos.x / halfMapUI)/2 * z;
                        // // move.y += (uiPos.y / halfMapUI)/2 * z;
                        unsnapProhibit = true;
                        snappedTime = Time.time;
                        SelectMarker(m);
                    }
                }
                else {
                    if (selectedMarker == m) {
                        DeselectMarker(m);
                    }
                }

                if (markerEnabled) {

                    if (m.showRotation) {
                        if (m.mainGraphic != null) {
                            Vector3 fwd = m.GetMarkerForward();
                            fwd.y = 0;
                            float angle = Vector3.Angle(fwd, Vector3.forward);
                            if (Vector3.Angle(fwd, Vector3.right) > 90) {
                                angle *= -1;
                            }
                                
                            m.mainGraphic.rectTransform.localRotation = Quaternion.Euler(0,0,-angle);
                        }
                    }
                    m.rectTransform.anchoredPosition = uiPos;
                }
                    
            }

            mapImage.uvRect = new Rect( move.x + zOffset, move.y + zOffset, z, z );

            if (UIInput.GetSubmitDown()) {
                AddWaypoint ( zOffset );
            }            
        }

        // protected override void OnDisable () {
        //     base.OnDisable();
        //     if (Application.isPlaying) {
        //         UIMaps.onObjectDeregister -= OnObjectDeregister;
        //     }
        // }

        

        void OnObjectDeregister (UIMapsObject deregistered) {
            if (deregistered == currentWaypointRegisterObj) {
                if (currentWaypointRegisterObj != null)
                    currentWaypointRegisterObj = null;
                    // AddWaypoint(0);
            }
        }

        // public MapMarker waypointIcon;
        // public MapMarker waypointIcon;
        
        UIMapsObject currentWaypointRegisterObj;
        UIMapsObject defaultWaypointRegisteredObj;

        void AddWaypoint (float zOffset) {
            if (defaultWaypointRegisteredObj == null && currentWaypointRegisterObj == null) {
            // if (currentWaypointMarker != null) {

                // TODO: Get ground Y
                if (selectedMarker == null || UIMaps.RegisteredObjectIsPlayer(selectedMarker.uIRegisteredObject)) {
                    Vector3 worldPos = Vector3.zero;
                    Vector2 wPos = (move);// + Vector2.one * zOffset);// * mapSize;
                    Debug.LogError(wPos);


                    worldPos.x = mapCenter.x + wPos.x * mapRadius * 2;
                    worldPos.z = mapCenter.y + wPos.y * mapRadius * 2;
                    Debug.LogError(worldPos);

                    // register the default empty icon
                    defaultWaypointRegisteredObj = UIMaps.RegisterDynamicObjectWithUI(
                        "MapWaypoint", GetInstanceID(), worldPos, Vector3.forward, "Waypoint", null, null, null, null
                    );

                    defaultWaypointRegisteredObj.SetAsWaypoint(true);

                    defaultWaypointRegisteredObj.ShowMapType(UIMapsObject.ShowType.MainMap, true);
                    defaultWaypointRegisteredObj.ShowMapType(UIMapsObject.ShowType.MiniMap, true);
                    defaultWaypointRegisteredObj.ShowMapType(UIMapsObject.ShowType.Compass, true);
                    defaultWaypointRegisteredObj.ShowMapType(UIMapsObject.ShowType.ScreenMap, true);

                }
                else  {
                    // Debug.Log("making waypoing already thtere");
                    currentWaypointRegisterObj = selectedMarker.uIRegisteredObject;
                    
                    // worldPos = selectedMarker.GetMarkerPosition();
                    currentWaypointRegisterObj.SetAsWaypoint(true);

                    // alraedy showing map...
                    // currentWaypointMarker.uIRegisteredObject.ShowMainMap(true);
                    
                    currentWaypointRegisterObj.ShowMapType(UIMapsObject.ShowType.MiniMap, true);
                    currentWaypointRegisterObj.ShowMapType(UIMapsObject.ShowType.Compass, true);
                    currentWaypointRegisterObj.ShowMapType(UIMapsObject.ShowType.ScreenMap, true);

                    // currentWaypointMarker = AddMapMarker ("MapWaypoint", GetInstanceID(), waypointIcon, () => worldPos, "Waypoint");
                }
            }
            else {
                if (currentWaypointRegisterObj != null) {
                    currentWaypointRegisterObj.SetAsWaypoint(false);
                    
                    currentWaypointRegisterObj.ShowMapType(UIMapsObject.ShowType.ScreenMap, false);

                    if (!currentWaypointRegisterObj.loadedLocally) {
                        currentWaypointRegisterObj.ShowMapType(UIMapsObject.ShowType.MiniMap, false);
                        currentWaypointRegisterObj.ShowMapType(UIMapsObject.ShowType.Compass, false);
                    }
                    currentWaypointRegisterObj = null;
                }
                // else {
                // }
                if (defaultWaypointRegisteredObj != null)
                    defaultWaypointRegisteredObj = UIMaps.DeregisterDynamicObjectWithUI (GetInstanceID());

                // currentWaypointMarker = null;
                // defaultWaypointRegisteredObj = null;



                // RemoveMapMarker(GetInstanceID());
                // currentWaypointMarker = null;
            }

        
        }
    }

}
