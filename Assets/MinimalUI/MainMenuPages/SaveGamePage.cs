

// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {

    public class SaveGamePage : MenuPage
    {
        
        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            if (input.x == settings.submitAction) {
                if (data != null) {
                    int slotSelect = (int)data[0];
                    SaveGame(slotSelect);
                }
            }
        }

        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) { 
            int slotSelect = (int)button.data[0];
            
            if (SaveLoad.SaveExists(slotSelect)) {
                element.EnableFlair(0, true);
            }
        }

        protected override void OnOpenUI(Actor[] actorContexts) {
            BuildButtons("Save Game", true, 0);
        }

        

        int confirmSlot;
        void OnSaveGameConfirmation () {
            FinishSaveGame(confirmSlot);
        }

        void FinishSaveGame(int slot) {
            SaveLoad.SaveGameState(slot);
            manualMenu.CloseMenu();
        }

        void SaveGame (int slot) {
            if (SaveLoad.SaveExists(slot)) {
            
                confirmSlot = slot;
                UIManager.ShowConfirmationPopup(true, "Are you sure you want to overwrite this save?", OnSaveGameConfirmation );
            }
            else {
                FinishSaveGame(slot);
            }
        }

    
        protected override int MaxButtons() {
            
            return GameManager.maxSaveSlots;
        }

        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {

            for (int i = 0; i < GameManager.maxSaveSlots; i++) {
                buttons.Add(
                    new ManualMenuButton(
                        "Slot " + i.ToString(), 
                        SaveLoad.SaveExists(i) ? SaveLoad.GetSaveDescription(i).ToString() : "Empty",
                        null, 
                        new object[] { i }
                    )
                );    
               
            }
        }
    }
}

