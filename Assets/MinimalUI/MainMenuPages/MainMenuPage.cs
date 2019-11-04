using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {

    /*
    
        TODO:
            continue last save button
    */
    public class MainMenuPage : MenuPage
    {
        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            if (input.x == settings.submitAction) {
                if (data != null) {
                    int methodSelect = (int)data[0];
                    if (methodSelect == 0) {
                        OnStartNewGame();
                    }
                    else if (methodSelect == 1) {
                        OnQuitGame();
                    }
                }
            }
        }

        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) { }

        protected override void OnOpenUI(Actor[] actorContexts) {
            BuildButtons("Main Menu", true, 0);
        }

        bool isInMainMenuScene { get { return GameManager.isInMainMenuScene; } }

        void OnConfirmNewGame () {
            StartNewGame();
        }
        void StartNewGame () {
            // Debug.Log("Starting New Game!");
            manualMenu.CloseMenu();
            GameManager.StartNewGame();
        }

        void OnStartNewGame () {
            if (!isInMainMenuScene) {
                UIManager.ShowConfirmationPopup(true, "Are you sure you want to start a new game?\nAny unsaved progress will be lost!", OnConfirmNewGame);
            }
            else {
                StartNewGame();
            }
        }

        void OnConfirmQuit (bool used, int selected) {
            if (used) {
                if (selected == 0) {
                    GameManager.QuitToMainMenu();
                }
                else if (selected == 1) {
                    GameManager.QuitApplication();
                }
            }
        }

        void OnConfirmQuitMenu () {
            GameManager.QuitApplication();    
        }
        
        void OnQuitGame () {
            if (!isInMainMenuScene) {
                UIManager.ShowSelectionPopup(true, "Are you sure you want to quit?\nAny unsaved progress will be lost!", new string[] { "Quit (Main Menu)", "Quit (Desktop)", "No" }, OnConfirmQuit);
            }
            else {
                UIManager.ShowConfirmationPopup(true, "Are you sure you want to quit?", OnConfirmQuitMenu);
            }
        }
        
        public MenuPage loadGamePage, saveGamePage;
        public MenuPage settingsPage;

        protected override int MaxButtons() {
            return !isInMainMenuScene ? 5 : 4;
        }

        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {
            buttons.Add(new ManualMenuButton("New Game", "Start A New Game", null, new object[] { 0 }));
            buttons.Add(new ManualMenuButton("Load Game", "Load An Existing Save", loadGamePage, null));
            
            if (!isInMainMenuScene) 
                buttons.Add(new ManualMenuButton("Save Game", "Save Game Progress", saveGamePage, null));
            
            buttons.Add(new ManualMenuButton("Settings", "Adjust Settings", settingsPage, null));
            buttons.Add(new ManualMenuButton("Quit", "Quit Game", null, new object[] { 1 }));
        }
    }
}
