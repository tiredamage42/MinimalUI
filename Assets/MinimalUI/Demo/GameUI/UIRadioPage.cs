// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// using Game.RadioSystem;
// using System;
// using SimpleUI;
// namespace Game.UI {

//     public class UIRadioPage : UISelectableElementHandler
//     {

//         static UIRadioPage _instance;
//         public static UIRadioPage instance { get { return GetInstance<UIRadioPage> (ref _instance); } }
        
//         public void OpenRadioPage (Radio radio) {
//             OpenUI ( -1, new object[] { radio } );
//         }
//         Radio showingRadio;
//         protected override void OnOpenUI() { 
//             showingRadio = (openedWithParams[0] as Radio); 
//             BuildButtons("Radio", true, 0);
//         }

//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) { 
//             string txt = "";
//             if (customData != null) {
//                 RuntimeRadioStation station = customData[0] as RuntimeRadioStation;
//                 if (station != null) txt = station.station.displayName;
//             }   
//             (uiObject as SimpleUI.UIPage).textPanel.SetText(txt);
//         }
        
//         // dont allow cold open
//         protected override object[] GetDefaultColdOpenParams() { return new object[] { Actor.playerActor.GetComponent<Radio>() }; }
        
//         protected override List<object> BuildButtonObjectsListForDisplay (int panelIndex){
//             return ToObjectList(RadioManager.instance.allAvailableRadioStations);
//         }

//         protected override string GetDisplayForButtonObject(object obj) { 
//             return (obj as RuntimeRadioStation).station.displayName; 
//         }

//         public int selectAction;
//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Select" };
//             return new List<int>() { selectAction };
//         }
// /*
// \u2022 buller


// \u2023 arraow hyphen

// \u2665


//  */
//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
//             RuntimeRadioStation station = mainObject as RuntimeRadioStation;
//             if (station != null) {
//                 element.EnableFlair(0, showingRadio.isOn && showingRadio.currentStation == station.station);
//             }
//         }

//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {
//             if (input.x == selectAction) {
//                 if (customData != null) {
//                     RuntimeRadioStation selectedStation = customData[0] as RuntimeRadioStation;
//                     if (selectedStation != null) {
//                         if (showingRadio.isOn && showingRadio.currentStation == selectedStation.station) {
//                             showingRadio.TurnOff();
//                         }
//                         else {
//                             showingRadio.SwitchStation(selectedStation.station);
//                         }
//                     }
//                 }
//             }
// 		}
//     }
// }

