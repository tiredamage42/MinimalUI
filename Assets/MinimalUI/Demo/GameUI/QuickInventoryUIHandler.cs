// using System.Collections.Generic;
// using UnityEngine;
// using Game.InventorySystem;
// using SimpleUI;
// namespace Game.UI {

//     public class QuickInventoryUIHandler : UISelectableElementHandler
//     {
//         Inventory showingInventory;
//         protected override string GetDisplayForButtonObject(object buttonObject) {
//             InventorySlot slot = buttonObject as InventorySlot;
//             return slot.item.itemName + " ( x"+slot.count+" )";
//         }

//         static QuickInventoryUIHandler _instance;

//         public static QuickInventoryUIHandler instance { get { return GetInstance<QuickInventoryUIHandler> (ref _instance); } }
        

//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
            
//         }

//         public void OpenQuickInventoryUI (int interactorID, Inventory inventory) {
//             OpenUI(interactorID, new object[] { inventory });
//         }

//         // no cold open
//         protected override object[] GetDefaultColdOpenParams () { return null; }

//         protected override List<object> BuildButtonObjectsListForDisplay(int panelIndex){
//             List<object> r = new List<object>();
//             for (int i = 0; i < showingInventory.favorites.Count; i++) {
//                 r.Add(showingInventory.allInventory[showingInventory.favorites[i]]);
//             }
//             return r;
//         }

//         protected override void OnOpenUI() {
//             showingInventory = openedWithParams[0] as Inventory;
//             BuildButtons("", false, 0);
//         }
        
//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) {

//         }
                    

//         public int useAction;
//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Use" };
//             return new List<int>() { useAction };
//         }

//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {
//         	if (input.x == useAction) {
                
//                 CloseUI();
//                 if (customData != null) {
//                     InventorySlot item = customData[0] as InventorySlot;
//                     if (item != null) item.item.OnConsume(showingInventory, count: 1, input.y);
//                 }
//             }
// 		}
//     }
// }