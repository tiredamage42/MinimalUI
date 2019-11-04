

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools.EditorTools;
using UnityEditor;
using UnityTools;
using UnityTools.GameSettingsSystem;


namespace MinimalUI {


    public class UIMaps {
        public static bool RegisteredObjectIsPlayer (UIMapsObject obj) {
            return obj == playerIconObj;
        }
        static UIMapsObject playerIconObj;
        public static void InitializeMaps () {
            playerIconObj = RegisterDynamicObjectWithUI("PlayerIcon", -1, Vector3.zero, Vector3.forward, "Player", "PlayerIcon", "PlayerIcon", null, null);
            playerIconObj.SetWorldPositionGetter(
                () => {
                    return GameManager.playerExists ? GameManager.playerActor.GetPosition() : Vector3.zero;
                    // Transform t = UIManager.GetMainCamera().transform;
                    // return t == null ? Vector3.zero : t.position;
                }
            );
            playerIconObj.SetWorldForwardGetter(
                () => {
                    return GameManager.playerExists ? GameManager.playerActor.GetForward() : Vector3.forward;
                    
                    // Transform t = UIManager.GetMainCamera().transform;
                    // return t == null ? Vector3.forward : t.forward;
                }
            );

            playerIconObj.ShowMapType(UIMapsObject.ShowType.MainMap, true);
            playerIconObj.ShowMapType(UIMapsObject.ShowType.MiniMap, true);



            

            string[] directions = new string[] { "N", "S", "E", "W" };
            
            float distanceValue = 99999;
            Vector3[] dirs = new Vector3[] { 
                Vector3.forward * distanceValue, 
                -Vector3.forward * distanceValue, 
                Vector3.right * distanceValue, 
                -Vector3.right * distanceValue 
            };
            
            for (int i = 0; i < 4; i++) {
                Vector3 d = dirs[i];

                UIMapsObject regObj = RegisterDynamicObjectWithUI(
                    "Direction " + directions[i], - (i + 2), Vector3.zero, d,
                    null, "DirectionIcon", null, "DirectionIcon", null
                );

                regObj.SetWorldPositionGetter(
                    () => {

                        return (GameManager.playerExists ? GameManager.playerActor.GetPosition() : Vector3.zero) + d;
                
                        // Transform t = UIManager.GetMainCamera().transform;
                        // if (t == null) return d;
                        // return t.position + d;
                    }
                );
                regObj.SetMarkerInitializer ((m) => (m.mainGraphic as UIText).SetText(directions[i]));
                
                regObj.ShowMapType(UIMapsObject.ShowType.Compass, true);
                regObj.ShowMapType(UIMapsObject.ShowType.MiniMap, true);
            }

        }



        public static System.Action<UIMapsObject> onObjectDeregister;

        // TODO: save static objects on game save / load



        // locations discovered / main map objects 
        // that we need to display even when particular scene isnt loaded.
        // for example a radio tower on the other side of the map
        // also map icons that get saved between runs
        static Dictionary<int, UIMapsObject> staticObjects = new Dictionary<int, UIMapsObject>();

        // for waypoints / enemies
        // using a different dictionary in case insance id keys conflict with
        // the serializable safe hash keys for static object above
        static Dictionary<int, UIMapsObject> dynamicObjects = new Dictionary<int, UIMapsObject>();

        public static bool StaticObjectRegistered (int key, out UIMapsObject obj) {
            return staticObjects.TryGetValue(key, out obj);
        }
        public static UIMapsObject RegisterStaticObjectWithUI (
            int key, 
            Vector3 worldPosition, Vector3 worldForward, 
            string message,
            string miniMapIconName, string mainMapIconName, string compassIconName, string worldViewIconName, bool loadedLocally
        ) {
            UIMapsObject obj;
            if (!staticObjects.TryGetValue(key, out obj)) {
                obj = new UIMapsObject(worldPosition, worldForward, message, miniMapIconName, mainMapIconName, compassIconName, worldViewIconName, true);
                staticObjects.Add(key, obj);
            }
            obj.loadedLocally = loadedLocally;
            return obj;
        }
        public static UIMapsObject DeregisterStaticObjectWithUI (int key) {
            UIMapsObject obj;
            if (staticObjects.TryGetValue(key, out obj)) {
                obj.ShowMapType(UIMapsObject.ShowType.Compass, false);
                obj.ShowMapType(UIMapsObject.ShowType.MainMap, false);
                obj.ShowMapType(UIMapsObject.ShowType.MiniMap, false);
                obj.ShowMapType(UIMapsObject.ShowType.ScreenMap, false);
                staticObjects.Remove(key);

                if (onObjectDeregister != null) {
                    onObjectDeregister(obj);
                }

            }
            return null;
        }


        public static UIMapsObject RegisterDynamicObjectWithUI (
            string debugName, 
            int key, Vector3 worldPosition, Vector3 worldForward, string message,
            string miniMapIconName, string mainMapIconName, string compassIconName, string worldViewIconName    
        ) {
            if (dynamicObjects.ContainsKey(key)) {
                Debug.LogWarning(debugName + ": already registered with ui");
                return dynamicObjects[key];
            }
            UIMapsObject obj = new UIMapsObject(worldPosition, worldForward, message, miniMapIconName, mainMapIconName, compassIconName, worldViewIconName, true);
            obj.loadedLocally = true;
            dynamicObjects.Add(key, obj);
            return obj;
        }
        public static UIMapsObject DeregisterDynamicObjectWithUI (int key) {
            UIMapsObject obj;
            if (dynamicObjects.TryGetValue(key, out obj)) {

                obj.ShowMapType(UIMapsObject.ShowType.Compass, false);
                obj.ShowMapType(UIMapsObject.ShowType.MainMap, false);
                obj.ShowMapType(UIMapsObject.ShowType.MiniMap, false);
                obj.ShowMapType(UIMapsObject.ShowType.ScreenMap, false);
                
                dynamicObjects.Remove(key);

                if (onObjectDeregister != null) {
                    onObjectDeregister(obj);
                }
            }
            return null;
        }
    }
}

