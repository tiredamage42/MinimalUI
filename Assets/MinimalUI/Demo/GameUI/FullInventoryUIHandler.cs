// using System.Collections.Generic;
// using UnityEngine;

// using Game.InventorySystem;
// using SimpleUI;
// namespace Game.UI {


//     public class FullInventoryUIHandler : UISelectableElementHandler
//     {
//         Inventory showingInventory;
//         protected override object[] GetDefaultColdOpenParams () { return new object[] { Actor.playerActor.inventory, null }; }
//         protected override string GetDisplayForButtonObject(object buttonObject) {
//             InventorySlot slot = buttonObject as InventorySlot;
//             return slot.item.itemName + " ( x"+slot.count+" )";
//         }

//         static FullInventoryUIHandler _instance;
//         public static FullInventoryUIHandler instance { get { return GetInstance<FullInventoryUIHandler> (ref _instance); } }
        
//         public void OpenInventoryManagementUI (Inventory inventory, List<int> categoryFilter) {
//             OpenUI (  0, new object[] { inventory, categoryFilter });
//         }

//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) {
//             string txt = "";
//             if (customData != null) {
//                 InventorySlot slot = customData[0] as InventorySlot;
//                 if (slot != null) txt = slot.item.itemDescription;
//             }   
//             (uiObject as SimpleUI.UIPage).textPanel.SetText(txt);
//         }

//         protected override List<object> BuildButtonObjectsListForDisplay(int panelIndex){
//             return ToObjectList(showingInventory.GetFilteredInventory(openedWithParams[1] as List<int>));
//         }
        
//         protected override void OnOpenUI() {
//             showingInventory = openedWithParams[0] as Inventory;
//             BuildButtons("Inventory", true, 0);
//         }
        
        
//         InventorySlot highlightedItemSlot;

//         void OnGetDropAmount (bool used, int value) {
//             if (used) {
//                 // drop
//                 if (value > 0) {       
//                     showingInventory.DropItem(highlightedItemSlot, value, true, -1, true, sendMessage: false);
//                     UpdateUIButtons();
//                 }
//             }
//         }

//         void OnItemActionSelection(bool used, int value) {
//             if (used) {
//                 // drop
//                 if (value == 0) {       
//                     // get slider value
//                     UIManager.ShowIntSliderPopup(false, "\n\nDrop " + highlightedItemSlot.item.itemName + ":", 0, highlightedItemSlot.count, OnGetDropAmount);
//                 }
//                 // favorite
//                 else if (value == 1) {
//                     showingInventory.FavoriteItem(highlightedItemSlot.item);    
//                     UpdateUIButtons();
//                 }
//             }
//         }

//         public int consumeAction = 0;
//         public int optionsAction = 1;

//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Use", "Options" };
//             return new List<int>() { consumeAction, optionsAction };
//         }

//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
//             InventorySlot slot = mainObject as InventorySlot;
//             if (slot != null) {
//                 element.EnableFlair(0, showingInventory.equipper.ItemIsEquipped(-1, slot.item));
//                 element.EnableFlair(1, showingInventory.IsFavorited(slot.item));
//             }
//         }



//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {
//             if (customData != null) {

//                 highlightedItemSlot = customData[0] as InventorySlot;
                
//                 if (highlightedItemSlot != null) {
                                    
//                     if (input.x == consumeAction){
//                         if (highlightedItemSlot.item.OnConsume(showingInventory, count: 1, input.y)){
//                             UpdateUIButtons();
//                         }
//                     }
//                     // drop
//                     else if (input.x == optionsAction){
//                         UIManager.ShowSelectionPopup(true, "\n\n"+highlightedItemSlot.item.itemName+":", new string[] {"Drop", "Favorite"}, OnItemActionSelection);
//                     }
//                 }
//             }
// 		}
//     }
// }