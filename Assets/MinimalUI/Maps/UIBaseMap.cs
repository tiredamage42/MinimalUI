using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityTools;

namespace MinimalUI {
    /*
        keeping this as a serializable "module" class instead of a base class,
        since the main map needs to inherit from UIWithInput class, and all the other maps can just be 
        UI objects

    */

    [System.Serializable] public class UIBaseMap {
        
        public void OnComponentDisable() {
            // base.OnComponentClose();
            // baseMapParameters.OnComponentClose();

            for (int i =0 ; i < mapMarkers.Count; i++) {
                mapMarkers[i].OnComponentDisable();
            }
        }
        public void OnComponentEnable () {
            for (int i =0 ; i < mapMarkers.Count; i++) {
                mapMarkers[i].OnComponentEnable();
            }
        }
        
        public MapMarkerParameters markerParameters;
        static PrefabPool<MapMarker> markerPool = new PrefabPool<MapMarker>();
        [System.NonSerialized] [HideInInspector] public List<MapMarker> mapMarkers = new List<MapMarker>();
        
        System.Action<MapMarker> onMarkerAdd, onMarkerRemove;
        Transform transform;
        public void OnEnable (Transform transform, System.Action<MapMarker> onMarkerAdd, System.Action<MapMarker> onMarkerRemove) {
            this.transform = transform;
            this.onMarkerAdd = onMarkerAdd;
            this.onMarkerRemove = onMarkerRemove;
        }

        public void UpdateMapMarkers (Vector3 playerPosition) {
            for (int i = 0; i < mapMarkers.Count; i++) {
                mapMarkers[i].UpdateMapMarker (playerPosition);
            }
        }
        

        public MapMarker AddMapMarker ( MapMarker prefab, UIMapsObject regObj) {
            
            MapMarker newMarker = markerPool.GetAvailable(prefab, null, false, null);
            newMarker.InitializeMapMarker(regObj);
            mapMarkers.Add(newMarker);

            newMarker.transform.SetParent(transform, Vector3.zero, Quaternion.identity, Vector3.one);
            newMarker.EnableMapMarker(true);

            UIBaseParameters.CopyParameters(ref newMarker.parameters, markerParameters);
            newMarker.UpdateElementLayout();//true);
            if (onMarkerAdd != null)
                onMarkerAdd(newMarker);


            newMarker.gameObject.SetActive(true);
            return newMarker;
        }

        public MapMarker RemoveMapMarker (MapMarker mapMarker) {
            if (mapMarkers.Contains(mapMarker)) {
                mapMarkers.Remove(mapMarker);
                mapMarker.gameObject.SetActive(false);
                if (onMarkerRemove != null)
                    onMarkerRemove(mapMarker);
            }
            return null;
        }
        public void UpdateElementLayout(){//bool firstBuild) {
            if (!Application.isPlaying)
                return;// Vector2.zero;
            // if (!Application.isPlaying) mapMarkers = transform.GetComponentsInChildren<MapMarker>().ToList();
            
            for (int i = 0; i < mapMarkers.Count; i++) {
                UIBaseParameters.CopyParameters(ref mapMarkers[i].parameters, markerParameters);
                
                mapMarkers[i].UpdateElementLayout();//firstBuild);
            }   

            // return Vector2.zero;
        }
    }
}
