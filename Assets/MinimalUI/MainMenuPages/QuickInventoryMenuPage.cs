
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {

    public class QuickInventoryMenuPage : MenuPage
    {
        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            if (input.x == settings.submitAction) {


                manualMenu.CloseMenu();
                
                if (data != null) {
                    // InventorySlot item = data[0] as InventorySlot;
                    // if (item != null) item.item.OnConsume(showingInventory, count: 1, input.y);
                }
            }
        }

        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) { }

        protected override void OnOpenUI(Actor[] actorContexts) {
            // showingInventory = openedWithParams[0] as Inventory;
            
            BuildButtons("", false, 0);
        }

        
        protected override int MaxButtons() {
            return 8; //max favorites...
        }

        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {
            
            // int count = showingInventory.favorites.Count;
            int count = 8;

            for (int i = 0; i < count; i++) {
                
                // slot.item.itemName + " ( x"+slot.count+" )"
                // string displayName = showingInventory.allInventory[showingInventory.favorites[i]].itemName;
                // object itemObject = showingInventory.allInventory[showingInventory.favorites[i]]
                
                string displayName = "Item " + i.ToString();
                object itemObject = null;
                buttons.Add(new ManualMenuButton(displayName, null, null, new object[] { itemObject } ));
            }
        }
    }
}

// namespace Game.UI {

//     public class QuickInventoryUIHandler : UISelectableElementHandler
//     {
//         Inventory showingInventory;

    
//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
            
//         }

//         public void OpenQuickInventoryUI (int interactorID, Inventory inventory) {
//             OpenUI(interactorID, new object[] { inventory });
//         }

//         // no cold open
//         protected override object[] GetDefaultColdOpenParams () { return null; }

//         protected override void OnOpenUI() {
//             showingInventory = openedWithParams[0] as Inventory;
//             BuildButtons("", false, 0);
//         }
