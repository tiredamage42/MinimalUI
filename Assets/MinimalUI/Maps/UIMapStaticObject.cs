using System.Collections;
using System.Collections.Generic;
using UnityEngine;



using UnityEditor;

using UnityTools;
using UnityTools.GameSettingsSystem;
using UnityTools.EditorTools;


namespace MinimalUI {

        // the actual scene representation of the static map ui object (location / collectible)

    // registers with ui on enabled if specified
    // deregister with ui on disabled if specified

    public class UIMapStaticObject : MonoBehaviour {
        public string message;
        public string miniMapIconName; 
        public string mainMapIconName; 
        public string compassIconName; 
        public string worldViewIconName;
        public bool updatePosition;

        public bool registerOnEnable = true;
        public bool deregisterOnDisable;

        // editor use only
        [HideInInspector] public string staticMapObjectRegisterName;

        UIMapsObject registeredUIObj;
        
        public int registerKey { get { return ("Static." + gameObject.name).GetPersistentHashCode(); } }
        
        void OnEnable () {
            if (registerOnEnable) 
                RegisterWithUI();
            else {
                // check if we're alreaady registered anyways

                if (UIMaps.StaticObjectRegistered(registerKey, out registeredUIObj)) {
                    RegisterWithUI();
                }
            }
        }
        void OnDisable () {
            if (deregisterOnDisable) DeregisterWithUI();

            // we unloaded the object from the scene, but if it's still registered, remove from local minimap / compass
            // if it's not waypointed....

            if (registeredUIObj != null) {
                registeredUIObj.loadedLocally = false;

                if (!registeredUIObj.isWaypoint) {
                    
                    registeredUIObj.ShowMapType(UIMapsObject.ShowType.MiniMap, false);
                    registeredUIObj.ShowMapType(UIMapsObject.ShowType.Compass, false);
                    
                }
            }
        }
        public void RegisterWithUI () {
            registeredUIObj = UIMaps.RegisterStaticObjectWithUI (registerKey, transform.position, transform.forward, message, miniMapIconName, mainMapIconName, compassIconName, worldViewIconName, true);   
            
            registeredUIObj.ShowMapType(UIMapsObject.ShowType.MainMap, true);
            
            // if we're registered with ui throug this script, it means we've loaded locally
            // so add to the mini map / compass
            registeredUIObj.ShowMapType(UIMapsObject.ShowType.MiniMap, true);
            registeredUIObj.ShowMapType(UIMapsObject.ShowType.Compass, true);
            
        }
        public void DeregisterWithUI () {
            registeredUIObj = UIMaps.DeregisterStaticObjectWithUI (registerKey);   
        }
        
        void Update () {
            if (updatePosition) {
                if (registeredUIObj != null) {
                    registeredUIObj.SetPosition(transform.position);
                }
            }
        }        
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(UIMapStaticObject))]
    public class StaticMapUIObjectEditor : Editor {

        UIMapStaticObject _uiObj;
        UIMapStaticObject uiObj {
            get {
                if (_uiObj == null) _uiObj = target as UIMapStaticObject;
                return _uiObj;
            }
        }

        public override void OnInspectorGUI() {
            if (!Application.isPlaying) {
                base.OnInspectorGUI();


                GUITools.Space(3);


                EditorGUI.BeginChangeCheck();
                uiObj.staticMapObjectRegisterName = EditorGUILayout.TextField("Registry Group Name", uiObj.staticMapObjectRegisterName);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(target);
                }

                DrawWarning();
                DrawRegisterWithMapObject();
                DrawDeregisterWithMapObject();
            }
        }

        
        UIMapsObject GetRegisteredObjectOnStaticRegister (StaticMapsObjectRegistry mapObjRegister) {
            int registerKey = uiObj.registerKey;

            for (int i = 0; i < mapObjRegister.staticObjects.Count; i++) {
                if (mapObjRegister.staticObjects[i].key == registerKey) {
                    return mapObjRegister.staticObjects[i];
                }
            }

            Debug.Log("static object '" + uiObj.gameObject.name + " is not registered with register group '" + mapObjRegister.name + "', adding it...");
            UIMapsObject neRegisteredObj = new UIMapsObject(uiObj.transform.position, uiObj.transform.forward, uiObj.message, uiObj.miniMapIconName, uiObj.mainMapIconName, uiObj.compassIconName, uiObj.worldViewIconName, false);
            neRegisteredObj.key = registerKey;
            mapObjRegister.staticObjects.Add(neRegisteredObj);
            EditorUtility.SetDirty(mapObjRegister);
            return neRegisteredObj;
        }

        void DrawRegisterWithMapObject () {
            if (GUILayout.Button("Register/Update With Static Object Registry")) {
                if (string.IsNullOrEmpty(uiObj.staticMapObjectRegisterName)) {
                    Debug.LogError("Need a static map object registry to register with");
                    return;
                }
                StaticMapsObjectRegistry mapObjRegister = GameSettings.GetSettings<StaticMapsObjectRegistry>(uiObj.staticMapObjectRegisterName);
                if (mapObjRegister == null)
                    return;

                UIMapsObject obj = GetRegisteredObjectOnStaticRegister(mapObjRegister);

                obj.InitializeWithValues(uiObj.transform.position, uiObj.transform.forward, uiObj.message, uiObj.miniMapIconName, uiObj.mainMapIconName, uiObj.compassIconName, uiObj.worldViewIconName);
            }
        }

        void DrawDeregisterWithMapObject () {
            if (GUILayout.Button("Deregister With Static Object Registry")) {
                if (string.IsNullOrEmpty(uiObj.staticMapObjectRegisterName)) {
                    Debug.LogError("Need a static map object registry to deregister with");
                    return;
                }
                StaticMapsObjectRegistry mapObjRegister = GameSettings.GetSettings<StaticMapsObjectRegistry>(uiObj.staticMapObjectRegisterName);
                if (mapObjRegister == null)
                    return;


                int registerKey = uiObj.registerKey;

                for (int i = 0; i < mapObjRegister.staticObjects.Count; i++) {
                    if (mapObjRegister.staticObjects[i].key == registerKey) {
                        mapObjRegister.staticObjects.Remove(mapObjRegister.staticObjects[i]);
                        return;
                    }
                }
            }
        }

        void DrawWarning () {
            EditorGUILayout.HelpBox ("Make Sure Gameobject Has A Unique Name when registering.\nDeregister with static register object before changin Name, or register object", MessageType.Warning);
        }
    }


    #endif




}

