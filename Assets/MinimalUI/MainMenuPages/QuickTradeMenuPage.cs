
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {

    public class QuickTradeMenuPage : MenuPage
    {
        // public string fullTradeMenuName = "FullTradeMenu";
        // [Action] public int singleTradeAction = 0, tradeAllAction = 1, switchToFullTradeAction = 2;

        // protected override void GetActionsAndHints (out List<int> actions, out List<string> hints) {
        //     actions = GetActions();
        //     hints = new List<string>() { "Take", "Take All", "Trade" };
        // }

        // protected override List<int> GetActions () {
        //     return new List<int> () { singleTradeAction, tradeAllAction, switchToFullTradeAction };
        // }

        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            

            bool updateButtons = false;

            
            // single trade
            // if (input.x == singleTradeAction){
            //     if (data != null) {
            //         InventorySlot item = data[0] as InventorySlot;
            //         if (item != null) {
            //             showingInventory.TransferItemAlreadyInInventoryTo(item, item.count, takingInventory, sendMessage: false);
            //             updateButtons = true;
            //         }
            //     }
            // }
            // // take all
            // else if (input.x == tradeAllAction){
            //     // TODO: check if shown inventory has anything
            //     showingInventory.TransferInventoryContentsTo(takingInventory, sendMessage: false);
            //     updateButtons = true;
            // }
            // else if (input.x == switchToFullTradeAction){
            //     manualMenu.CloseMenu();

            //     //maybe use update mananger....
            //     StartCoroutine(OpenFullTrade());
            // }
            
            if (updateButtons){
                UpdateUIButtons(panelIndex);//, true);
            }
        }

        System.Collections.IEnumerator OpenFullTrade () {
            yield return null;
            // ManualMenu.OpenMenu(fullTradeMenuName, null);
            // GameUI.tradeUI.OpenTradUI(takingInventory, showingInventory, null);
        }


        

        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) { }

        protected override void OnOpenUI(Actor[] actorContexts) {
            // showingInventory = openedWithParams[0] as Inventory;
            // takingInventory = openedWithParams[1] as Inventory;
            
            // BuildButtons(showingInventory.GetDisplayName(), true, 0);
            BuildButtons("Quick Loot Inventory", true, 0);
        }

        
        protected override int MaxButtons() {
            return 8; //max favorites...
        }

        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {
            
            // int count = showingInventory.allInventory.Count;
            int count = 8;

            for (int i = 0; i < count; i++) {
                
                // slot.item.itemName + " ( x"+slot.count+" )"
                // string displayName = showingInventory.allInventory[i].itemName;
                // object itemObject = showingInventory.allInventory[i]
                
                string displayName = "Item " + i.ToString();
                object itemObject = null;
                buttons.Add(new ManualMenuButton(displayName, null, null, new object[] { itemObject } ));
            }
        }
    }
}
