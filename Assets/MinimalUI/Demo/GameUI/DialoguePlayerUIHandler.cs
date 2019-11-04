// using UnityEngine;
// using System.Collections.Generic;
// using System;

// using SimpleUI;
// using Game.DialogueSystem;
// namespace Game.UI {


//     public class DialoguePlayerUIHandler : UISelectableElementHandler{
    
//         static DialoguePlayerUIHandler _instance;
//         public static DialoguePlayerUIHandler instance { get { return GetInstance<DialoguePlayerUIHandler> (ref _instance); } }
        
//         public void OpenDialogueResponseUI (List<DialogueResponse> responses, Action<DialogueResponse> onRespond) {
//             this.onRespond = onRespond;
//             OpenUI ( -1, new object[] { responses } );
//         }

//         Action<DialogueResponse> onRespond;
//         protected override void OnOpenUI() { 
//             maxButtons = (openedWithParams[0] as List<DialogueResponse>).Count;
//             BuildButtons("", true, 0);
//         }

//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) { }
        
//         // dont allow cold open
//         protected override object[] GetDefaultColdOpenParams() { return null; }
        
//         protected override List<object> BuildButtonObjectsListForDisplay (int panelIndex){
//             return ToObjectList(openedWithParams[0] as List<DialogueResponse>);
//         }

//         protected override string GetDisplayForButtonObject(object obj) { 
//             return (obj as DialogueResponse).bark; 
//         }

//         public int selectAction;
//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Select" };
//             return new List<int>() { selectAction };
//         }


//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
            
//         }



//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {
//             if (input.x == selectAction) {
//                 onRespond(customData[0] as DialogueResponse);
//                 CloseUI();
//             }
// 		}
//     }
// }
