using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinimalUI {
    [RequireComponent(typeof(UIMapDynamicObject))]
    public class UIMapDynamicObjectRegisterOnEnable : MonoBehaviour
    {

        public bool showCompass = true;
        public bool showMinimap = true;
        public bool showMainMap = true;
        public bool showScreenMap;

        void OnEnable () {
            UIMapDynamicObject mapDynamicObject = GetComponent<UIMapDynamicObject>();

            mapDynamicObject.ShowType(UIMapsObject.ShowType.Compass, showCompass);
            mapDynamicObject.ShowType(UIMapsObject.ShowType.MainMap, showMainMap);
            mapDynamicObject.ShowType(UIMapsObject.ShowType.MiniMap, showMinimap);
            mapDynamicObject.ShowType(UIMapsObject.ShowType.ScreenMap, showScreenMap);
        }
        
    }
}
