using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {

    [System.Serializable] public class UIMapsObject {
        public int key;
        public string message;
        public string miniMapIconName; 
        public string mainMapIconName; 
        public string compassIconName; 
        public string screenMapIconName;
        public sVector3 worldPos, worldFwd;    

        public void SetPosition (Vector3 position) {
            worldPos = position;
        }
        public void SetForward (Vector3 forward) {
            worldFwd = forward;
        }

        [System.NonSerialized] System.Func<Vector3> positionGetter;

        public Vector3 worldPosition {
            get {
                if (positionGetter != null) return positionGetter();
                return worldPos;
            }
        }
        [System.NonSerialized] System.Func<Vector3> forwardGetter;

        public Vector3 worldForward {
            get {
                if (forwardGetter != null) return forwardGetter();
                return worldFwd;
            }
        }

        public void SetWorldPositionGetter (System.Func<Vector3> positionGetter) {
            this.positionGetter = positionGetter;
        }
        public void SetWorldForwardGetter (System.Func<Vector3> forwardGetter) {
            this.forwardGetter = forwardGetter;
        }


        [System.NonSerialized] System.Action<MapMarker> markerInitializer;
        public void SetMarkerInitializer (System.Action<MapMarker> markerInitializer) {
            this.markerInitializer = markerInitializer;
        }


        public void InitializeWithValues (
            Vector3 worldPosition, Vector3 worldForward,
            string message, 
            string miniMapIconName, string mainMapIconName, string compassIconName, string screenMapIconName
        ) {
            SetPosition( worldPosition );
            SetForward( worldForward );
            this.message = message;
            this.miniMapIconName = miniMapIconName;
            this.mainMapIconName = mainMapIconName;
            this.compassIconName = compassIconName;
            this.screenMapIconName = screenMapIconName;
        }    


        public UIMapsObject (
            Vector3 worldPosition, Vector3 worldForward,
            string message, 
            string miniMapIconName, string mainMapIconName, string compassIconName, string screenMapIconName, bool runtime
        ) {
            InitializeWithValues( worldPosition, worldForward, message, miniMapIconName, mainMapIconName, compassIconName, screenMapIconName );
            if (runtime) InitializeRuntime();
        }



        const string iconReferencesObjectName = "UIMapIconReferences";
        const string emptyIconName = "EmptyIcon";

        static MapMarker GetMapMarkerPrefab (string name) {
            if (string.IsNullOrEmpty(name))
                name = emptyIconName;
            return PrefabReferences.GetPrefabReference<MapMarker> (iconReferencesObjectName, name);
        }


        // dont serialize
        [System.NonSerialized] bool _isWaypoint, _loadedLocally;

        public bool isWaypoint { get { return _isWaypoint; } }
        public bool loadedLocally {
            get { return _loadedLocally; }
            set { _loadedLocally = value; }
        }

        public void SetAsWaypoint(bool isWaypoint) {
            _isWaypoint = isWaypoint;
        }

        public class ShowMap {
            MapMarker mapMarker;
            public void Show(bool show, string iconPrefabName, UIBaseMap mapInstance, UIMapsObject regObj, System.Action<MapMarker> markerInitializer) {
                if (mapInstance == null)
                    return;
                
                if (show) {
                    if (mapMarker == null) {
                        MapMarker prefab = GetMapMarkerPrefab(iconPrefabName);
                        if (prefab != null) {
                            mapMarker = mapInstance.AddMapMarker(prefab, regObj);
                            if (markerInitializer != null) 
                                markerInitializer(mapMarker);

                        }
                    }
                }
                else {
                    if (mapMarker != null) {
                        // should set to null
                        mapMarker = mapInstance.RemoveMapMarker(mapMarker);
                    }
                }
            }
        }

        public enum ShowType { MainMap = 0, MiniMap = 1, Compass = 2, ScreenMap = 3 };

        
        public void InitializeRuntime () {
            showMaps = new ShowMap[] { new ShowMap(), new ShowMap(), new ShowMap(), new ShowMap() };
            instances = new UIBaseMap[] { 
                UIMainMap.instance.baseMapParameters, 
                UIMiniMap.instance.baseMapParameters, 
                UICompass.instance.baseMapParameters, 
                UIScreenMap.instance.baseMapParameters 
            };
            iconNames = new string[] { mainMapIconName, miniMapIconName, compassIconName, screenMapIconName };
        }

        [System.NonSerialized] ShowMap[] showMaps;
        [System.NonSerialized] UIBaseMap[] instances;
        [System.NonSerialized] string[] iconNames;

        public void ShowMapType (ShowType showType, bool show) {
            int i = (int)showType;
            showMaps[i].Show(show, iconNames[i], instances[i], this, markerInitializer);
        }
        public void ShowAll (bool show) {
            for (int i = 0; i < 4; i++) showMaps[i].Show(show, iconNames[i], instances[i], this, markerInitializer);
        }
    }

}
