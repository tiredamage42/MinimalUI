using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MinimalUI {

    public class UIMapDynamicObject : MonoBehaviour {
            public string message;
            public string miniMapIconName; 
            public string mainMapIconName; 
            public string compassIconName; 
            public string worldViewIconName;
            UIMapsObject registeredUIObj;
            
            void OnDisable () {
                DeregisterWithUI();
            }
            public void DeregisterWithUI () {
                registeredUIObj = UIMaps.DeregisterDynamicObjectWithUI (GetInstanceID());   
            }
            
            void RegisterWithUI () {
                registeredUIObj = UIMaps.RegisterDynamicObjectWithUI (gameObject.name, GetInstanceID(), transform.position, transform.forward, message, miniMapIconName, mainMapIconName, compassIconName, worldViewIconName);      
                registeredUIObj.SetWorldPositionGetter( () => transform.position );
                registeredUIObj.SetWorldForwardGetter( () => transform.forward );
            }

            public void ShowType (UIMapsObject.ShowType showType, bool show) {
                if (show) {
                    if (registeredUIObj == null) RegisterWithUI();
                    registeredUIObj.ShowMapType(showType, true);
                }
                else {
                    if (registeredUIObj != null) registeredUIObj.ShowMapType(showType, false);
                }
            }


            // public void ShowMainMap (bool show) {
            //     if (show) {
            //         if (registeredUIObj == null) RegisterWithUI();
            //         registeredUIObj.ShowMainMap(true);
            //     }
            //     else {
            //         if (registeredUIObj != null) registeredUIObj.ShowMainMap(false);
            //     }
            // }

            // public void ShowMiniMap (bool show) {
            //     if (show) {
            //         if (registeredUIObj == null) RegisterWithUI();
            //         registeredUIObj.ShowMinimap(true);
            //     }
            //     else {
            //         if (registeredUIObj != null) registeredUIObj.ShowMinimap(false);
            //     }
            // }
            // public void ShowCompass (bool show) {
            //     if (show) {
            //         if (registeredUIObj == null) RegisterWithUI();
            //         registeredUIObj.ShowCompass(true);
            //     }
            //     else {
            //         if (registeredUIObj != null) registeredUIObj.ShowCompass(false);
            //     }
            // }

            // public void ShowScreenMap (bool show) {
            //     if (show) {
            //         if (registeredUIObj == null) RegisterWithUI();
            //         registeredUIObj.ShowScreenMap(true);
            //     }
            //     else {
            //         if (registeredUIObj != null) registeredUIObj.ShowScreenMap(false);
            //     }
            // }    
        }
}
