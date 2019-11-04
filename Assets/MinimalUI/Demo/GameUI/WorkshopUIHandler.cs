


// using System.Collections.Generic;
// using UnityEngine;

// using SimpleUI;
// using Game.InventorySystem;
// using Game.InventorySystem.WorkshopSystem;
// namespace Game.UI {

//     public class WorkshopUIHandler : UISelectableElementHandler
//     {
//         Inventory showingInventory;
//         protected override object[] GetDefaultColdOpenParams () { return new object[] { Actor.playerActor.inventory, Actor.playerActor.inventory.workshopItemsFilter }; }
//         protected override string GetDisplayForButtonObject(object buttonObject) {
//             return (buttonObject as InventorySlot).item.itemName;
//         }
//         static WorkshopUIHandler _instance;
//         public static WorkshopUIHandler instance { get { return GetInstance<WorkshopUIHandler> (ref _instance); } }
        
//         public void OpenWorkshopUI (Inventory inventory, List<int> categoryFilter) {
//             OpenUI ( 1, new object[] { inventory, categoryFilter });
//         }

//         [Header("Sync actions with workshop mode actions...")]
//         public int buildAction;
//         public int exitAction = 7;
//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Build", "Scrap", "Exit" };
//             return new List<int>() { buildAction, cancelAction, exitAction };
//         }



//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){

//         }
//         // set flair any in inventory ?
//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
            
//         }

        
//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) {
//             string txt = "";
//             //cehck for empty....
//             if (customData != null) {
//                 InventorySlot selectedSlot = customData[0] as InventorySlot;
//                 if (selectedSlot != null) {
                    
//                     // TODO: figureo out return item trhough workshop item behavior, 
//                     // and print how many we have currently in inventory

//                     WorkshopRecipe recipe = selectedSlot.item.FindItemBehavior<WorkshopRecipe>();
//                     if (recipe != null) {
//                         txt = CraftingUIHandler.BuildRequiredComponentsText(recipe.requires, selectedSlot.item, showingInventory, showingInventory.actor);
//                     }
//                     else {
//                         txt = "\n\n" + selectedSlot.item + " doesnt have a crafting recipe behavior\nto specify required components build";
//                     }
//                 }
//             }
//             (uiObject as SimpleUI.UIPage).textPanel.SetText(txt);
//         }

//         protected override List<object> BuildButtonObjectsListForDisplay(int panelIndex){
//             return ToObjectList(showingInventory.GetFilteredInventory(openedWithParams[1] as List<int>));
//         }        
//         protected override void OnOpenUI() {
//             showingInventory = openedWithParams[0] as Inventory;
//             BuildButtons("Workshop Items", true, 0);
//         }
//     }
// }