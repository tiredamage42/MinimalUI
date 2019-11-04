// using System.Collections.Generic;
// using UnityEngine;
// using SimpleUI;
// namespace Game.UI {

//     public class UIGameValuePage : UISelectableElementHandler
//     {
//        static UIGameValuePage _instance;
//         public static UIGameValuePage instance {
//             get {
//                 if (_instance == null) _instance = GameObject.FindObjectOfType<UIGameValuePage>();
//                 return _instance;
//             }
//         }
//         public void OpenGameValuesUI (string valuesName, Dictionary<string, GameValue> valuesShow) {
//             instance.OpenUI ( 0, new object[] { valuesName, valuesShow } );
//         }

//         protected override object[] GetDefaultColdOpenParams() { 
//             return new object[] { Actor.playerActor.actorName, Actor.playerActor.actorValues }; 
//         }

//         protected override void OnOpenUI() { 
//             BuildButtons((openedWithParams[0] as string) + " Stats", true, 0);
//         }

//         protected override List<object> BuildButtonObjectsListForDisplay (int panelIndex) {
//             Dictionary<string, GameValue> valuesToShow = openedWithParams[1] as Dictionary<string, GameValue>;
//             List<object> r = new List<object>();
//             foreach (var k in valuesToShow.Keys) {
//                 if (valuesToShow[k].showInStats) {
//                     r.Add(valuesToShow[k]);
//                 }
//             }
//             return r;
//         }

//         protected override string GetDisplayForButtonObject(object obj) { 
//             return (obj as GameValue).name; 
//         }

//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) { 
//             string txt = "";
                    
//             if (customData != null) {
//                 GameValue g = customData[0] as GameValue;   
//                 if (g != null) {
//                     txt = g.description + "\n\n" + g.GetValue() + " / " + g.GetMaxValue();
//                     // TODO: add modifiers to show
//                 }
//             }
//             (uiObject as SimpleUI.UIPage).textPanel.SetText(txt);

//         }

//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { };
//             return new List<int>() { };
//         }

//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
            
//         }


//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {

//         }
//     }
// }
