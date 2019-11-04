
// using UnityEngine;
// using System.Collections.Generic;
// using Game.InventorySystem;
// using Game.InventorySystem.CraftingSystem;
// using SimpleUI;
// namespace Game.UI {
//     public class CraftingUIHandler : UISelectableElementHandler
//     {

//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
            
//         }


        

//         public static string BuildConfirmationText(ItemComposition[] compositionsToShow, string actionPrefix, string objectName, Actor actorValues) {
//             string msgText = "\n " + actionPrefix + " " + objectName + "?\n";
//             List<ItemComposition> filteredComposition = Inventory.FilterItemComposition (compositionsToShow, actorValues, actorValues);
//             for (int i = 0; i < filteredComposition.Count; i++) {
//                 msgText += filteredComposition[i].item.itemName + ":\t" + filteredComposition[i].amount + "\n";
//             }
//             return msgText;
//         }

//         public static string BuildRequiredComponentsText(ItemComposition[] requiredComposition, ItemBehavior buildItem, Inventory inventory, Actor actorValues) {
//             string txt = buildItem.itemDescription + "\n\nRequires:\n";
                            
//             List<ItemComposition> required = Inventory.FilterItemComposition (requiredComposition, actorValues, actorValues);
//             for (int i = 0; i < required.Count; i++) {
//                 int hasCount = inventory.GetItemCountAfterAutoScrap(required[i].item, actorValues, actorValues);
//                 txt += required[i].item.itemName + ":\t" + hasCount + " / " + required[i].amount + "\n";
//             }
//             return txt;
//         }
//         public static string BuildBreakdownComponentsText (ItemComposition[] compositions, ItemBehavior scrapItem, Actor actorValues) {
//             string msgText = scrapItem.itemDescription + "\n\nScrap For:\n";
//             List<ItemComposition> composedOf = Inventory.FilterItemComposition (compositions, actorValues, actorValues);
//             for (int i = 0; i < composedOf.Count; i++) {
//                 msgText += composedOf[i].item.itemName + ":\t" + composedOf[i].amount + "\n";
//             }
//             return msgText;
//         }
        
//         Inventory showingInventory;
//         // dont allow cold open
//         protected override object[] GetDefaultColdOpenParams () { return null; }
        
//         protected override string GetDisplayForButtonObject(object buttonObject) {
//             InventorySlot slot = buttonObject as InventorySlot;
//             return slot.item.itemName + " ( x"+slot.count+" )";
//         }
        
//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) {
//             string txt = "";
            
//             //cehck for empty....
//             if (customData != null) {
//                 InventorySlot selectedSlot = customData[0] as InventorySlot;
//                 if (selectedSlot != null) {
                
//                     int uiIndex = (int)customData[1];
//                     //selected on the recipes page...
//                     if (uiIndex == 0) {
//                         CraftingRecipeBehavior recipe = selectedSlot.item.FindItemBehavior<CraftingRecipeBehavior>();
//                         if (recipe != null) {
//                             txt = BuildRequiredComponentsText(recipe.requires, selectedSlot.item, showingInventory, showingInventory.actor);
//                         }
//                     }
//                     //selected on scrappable categories page
//                     else {
//                         txt = BuildBreakdownComponentsText (selectedSlot.item.composedOf, selectedSlot.item, showingInventory.actor);
//                     }
//                 }
//             }
//             (uiObject as SimpleUI.ElementHolderCollection).textPanel.SetText(txt);
//         }

//         List<InventorySlot> BuildInventorySlotsForDisplay (int panelIndex) {
//             if (panelIndex == 0) {
//                 return showingInventory.GetFilteredInventory(openedWithParams[1] as List<int>);
//             }
//             else {
//                 List<InventorySlot> r = showingInventory.GetFilteredInventory(showingInventory.scrappableCategories);
//                 for (int i = r.Count - 1; i >= 0; i--) {
//                     // check if item is in fact scrappable (not base components)
//                     if (r[i].item.composedOf.list.Length == 0) {
//                         r.Remove(r[i]);
//                     }
//                 } 
//                 return r;
//             }
//         }
//         protected override List<object> BuildButtonObjectsListForDisplay(int panelIndex){
//             return ToObjectList(BuildInventorySlotsForDisplay(panelIndex));
//         }

//         static CraftingUIHandler _instance;
//         public static CraftingUIHandler instance { get { return GetInstance<CraftingUIHandler> (ref _instance); } }
        
//         public void OpenCraftingUI (Inventory craftingInventory, List<int> categoryFilter) {
//             OpenUI (  0, new object[] { craftingInventory, categoryFilter } );
//         }


//         protected override void OnOpenUI() {
//             showingInventory = openedWithParams[0] as Inventory;
//             BuildButtons("Recipes", true, 0);
//             BuildButtons("Scrappable", false, 1);
//         }
        
//         // const int craftAction = 0;

//         void OnConfirmationSelection(bool used, int selectedOption) {
//             if (used && selectedOption == 0) {
//                 //scrap
//                 if (currentPanelIndex == 1) {
//                     showingInventory.ScrapItem(currentSlot.item, 1, -1, true, sendMessage: true, showingInventory.actor, showingInventory.actor);
//                 }
//                 // craft recipe
//                 else {
//                     currentSlot.item.OnConsume(showingInventory, 1, 0);
//                 }
//                 UpdateUIButtons( );
//             }
//         }





//         int currentPanelIndex;
//         InventorySlot currentSlot;

//         public int craftAction;
//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Craft / Scrap" };
//             return new List<int>() { craftAction };
//         }

//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {
        
//     		if (customData != null) {
//                 currentSlot = customData[0] as InventorySlot;
//                 currentPanelIndex = (int)customData[1];

//                 if (input.x == craftAction) {
                
//                     // showign crafting recipes
//                     if (currentPanelIndex == 0) {
//                         CraftingRecipeBehavior recipe = currentSlot.item.FindItemBehavior<CraftingRecipeBehavior>();
//                         if (recipe != null) {
//                             if (showingInventory.ItemCompositionAvailableInInventoryAfterAutoScrap (recipe.requires, showingInventory.actor, showingInventory.actor)) {
//                                 UIManager.ShowSelectionPopup(true, BuildConfirmationText(recipe.requires, "Craft", currentSlot.item.itemName, (openedWithParams[0] as Inventory).actor), new string[] {"Yes", "No"}, OnConfirmationSelection);
//                             }
//                         }
//                     }
//                     else {
//                         UIManager.ShowSelectionPopup(true, BuildConfirmationText(currentSlot.item.composedOf, "Scrap", currentSlot.item.itemName, (openedWithParams[0] as Inventory).actor), new string[] {"Yes", "No"}, OnConfirmationSelection);
//                     }
//                 }                
//             }
// 		}
//     }
// }