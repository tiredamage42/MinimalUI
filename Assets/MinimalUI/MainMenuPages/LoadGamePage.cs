using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {

    public class LoadGamePage : MenuPage
    {
        
        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            if (input.x == settings.submitAction) {
                if (data != null) {
                    int slotSelect = (int)data[0];
                    LoadGame(slotSelect);
                }
            }
        }

        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) { }

        protected override void OnOpenUI(Actor[] actorContexts) {
            BuildButtons("Load Game", true, 0);
        }

        bool isInMainMenuScene { get { return GameManager.isInMainMenuScene; } }

        int confirmSlot;
        
        void OnLoadGameConfirmation () {
            FinishLoadGame(confirmSlot);
            
        }

        void FinishLoadGame(int slot) {
            manualMenu.CloseMenu();
            SaveLoad.LoadGameState(slot);
        }

        void LoadGame (int slot) {
            if (!isInMainMenuScene) {
                confirmSlot = slot;
                UIManager.ShowConfirmationPopup(true, "Are you sure you want to load this save?\nAny unsaved progress will be lost!", OnLoadGameConfirmation);
            }
            else {
                FinishLoadGame(slot);
            }
        }

    
        protected override int MaxButtons() {
            int c = 0;
            for (int i = 0; i < GameManager.maxSaveSlots; i++) {
                if (SaveLoad.SaveExists(i)) {
                    c++;  
                }
            }
            return c;
        }

        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {

            for (int i = 0; i < GameManager.maxSaveSlots; i++) {
                if (SaveLoad.SaveExists(i)) {
                    buttons.Add(
                        new ManualMenuButton(
                            "Slot " + i.ToString(), 
                            SaveLoad.GetSaveDescription(i).ToString(),
                            null, 
                            new object[] { i }
                        )
                    );    
                }
            }
        }
    }
}
