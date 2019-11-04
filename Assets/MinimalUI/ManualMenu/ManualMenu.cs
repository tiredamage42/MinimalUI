
using UnityEngine;
// using MinimalUI;
using System.Collections.Generic;

using UnityTools;
namespace MinimalUI {
    
    public class ManualMenu : MonoBehaviour
    {   

        public static ManualMenu mainMenu;
        static Dictionary<string, ManualMenu> menuInstances = new Dictionary<string, ManualMenu>();

        public static ManualMenu GetMenu (string name) {
            ManualMenu mm;
            if (!menuInstances.TryGetValue(name, out mm)) {
                Debug.LogError("No Menu named " + name + " found");
                return null;
            }
            return mm;
        }
        public static void ToggleMenu (string name, Actor[] actorContexts) {
            ManualMenu m = GetMenu(name);
            if (m != null) m.ToggleMenu(actorContexts);
        }

        public static void OpenMenu (string name, Actor[] actorContexts) {
            ManualMenu m = GetMenu(name);
            if (m != null) m.OpenMenu(actorContexts);
        }
        public static void CloseMenu (string name) {
            ManualMenu m = GetMenu(name);
            if (m != null) m.CloseMenu();
        }



        bool CheckIfCopy () {
            string myName = gameObject.name;
            if (!menuInstances.ContainsKey(myName)) {
                menuInstances[myName] = this;
                DontDestroyOnLoad(gameObject);
                
                
                if (isMainMenu) {
                    if (mainMenu != null) {
                        Debug.LogWarning("More than one main menu specified, using: " + mainMenu.gameObject.name + ", leaving out: " + myName);    
                    }
                    else {
                        mainMenu = this;
                    }
                }
                return false;
            }
            if (menuInstances[myName] == this) {
                return false;
            }
            // Debug.LogWarning("Deleting Copy of Menu: " + myName);
            Destroy(gameObject);
            return true;
        }

        public bool darkenBackground;
        public bool disablesHud;
        public bool isMainMenu;
        public bool pauseGame;
        public event System.Action onMenuOpen, onMenuClose;
        MenuPage currentPage, firstPage;
        // UISelectable[] buttonReferences;
        // [HideInInspector] public UISelectableHolder uiObject;

        [HideInInspector] public bool isOpen;

        // UIImage _darkenBackground;
        // UIImage darkenBackground {
        //     get {
        //         if (_darkenBackground == null) _darkenBackground = UIManager.GetUIComponentByName<UIImage>("DarkenBackground");
        //         return _darkenBackground;
        //     }
        // }

        void Awake () {

            if (!CheckIfCopy()) {
                firstPage = GetComponent<MenuPage>();
            }
        }

        public void GoToPage (MenuPage newPage) {
            if (newPage == null && isMainMenu && GameManager.isInMainMenuScene) {
                return;
            }
            
            if (currentPage != null) {
                currentPage.UninitializeUI ();
                // UIManager.HideUI(currentPage.uiObject);
            }
            
            currentPage = newPage;
            if (newPage == null) {
                // if (!GameManager.isInMainMenuScene || !isMainMenu) {
                    // Debug.LogError("CLOSE");
                    CloseMenu ();
                // }
                return;
            }


            currentPage.manualMenu = this;
            currentPage.InitializeUI ( actorContexts );
            
            // UIManager.ShowUI(currentPage.uiObject);
            // currentPage.uiObject.SubscribeToActionEvent(currentPage.OnUIInput);    
            // currentPage.uiObject.SubscribeToSelectEvent(currentPage.OnUISelect);
            // currentPage.uiObject.actionHandler = currentPage.GetUIActions;
            // currentPage.uiObject.AddActionHints()


            
            // // add controller hints for that menu page....
            // UIManager.AddDefaultControllerHintsToUI (currentPage.uiObject);






            
            // update buttons
            // buttonReferences = null;

            // UISelectableHolder asSelectable = newPage.uiObject as UISelectableHolder;
            // if (asSelectable != null) {

                
            //     asSelectable.SetMainText(newPage.PageTitle());
            //     // (uiObject as UIPage).SetTitle(newPage.pageTitle);
            //     List<ManualMenuButton> allButtons = currentPage.GetAllButtons();
            //     int c = allButtons.Count;// currentPage.menuButtons.Length;
                
            //     buttonReferences = asSelectable.GetAllSelectableElements(c);
            //     UIManager.SetSelectionDelayed(buttonReferences[0].gameObject);
                
            //     for (int i = 0 ; i < c; i++) {
                    
            //         buttonReferences[i].uiText.SetText(allButtons[i].displayName);
            //         buttonReferences[i].customData = new object[] { allButtons[i] };
            //     }
            // }
        }


        public void ToggleMenu (Actor[] actorContexts) {
            if (isOpen) CloseMenu();
            else OpenMenu(actorContexts);
        }

        public void CloseMenu () {
            if (currentPage != null) {
                currentPage.UninitializeUI ();
                currentPage = null;
            }

            if (darkenBackground)
                UIManager.UIDarkenBackground(false);

            // darkenBackground.gameObject.SetActive(false);

            // UIManager.OnMenuClose();
            if (disablesHud)
                UIManager.EnableHud(true);
            
            
            isOpen = false;
            // mark all selection axes unoccupied
            // UIInput.MarkAllBaseUIInputsUnoccupied();
            

            if (pauseGame) GameManager.UnpauseGame();
            // if (!UIObjectActive()) return;
            // UIManager.HideUI(uiObject);
            // if (onUIClose != null) onUIClose (uiObject.baseObject);
            if (onMenuClose != null) onMenuClose ();
        }

        object[] openedWithParams;
        
        Actor[] actorContexts;
        public void OpenMenu(Actor[] actorContexts) {
            

            if (!GameManager.isInMainMenuScene) {
                if (darkenBackground)
                    UIManager.UIDarkenBackground(true);

                // darkenBackground.gameObject.SetActive(true);
            }

            if (disablesHud)
                UIManager.EnableHud(false);
            

            // UIManager.OnMenuOpen();

            this.actorContexts = actorContexts;

            // check if any other menu is open...



            isOpen = true;

            // mark all selection axes occupied

            // UIInput.MarkAllBaseUIInputsOccupied();


            if (pauseGame) GameManager.PauseGame();
            // if (UIObjectActive()) return;
            // if (UIManager.AnyUIOpen(out _)) return;

            // UIManager.ShowUI(uiObject);
            // uiObject.SubscribeToActionEvent(OnUIInput);            
            // UIManager.AddDefaultControllerHintsToUI (uiObject);


            // go to page, main page
            GoToPage(firstPage);

            // if (onUIOpen != null) onUIOpen (uiObject.baseObject);
            if (onMenuOpen != null) onMenuOpen ();
        }
        // void OnUIInput(GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input) {
        //     if (input.x == UIManager.instance.cancelAction) {
        //         GoToPage(currentPage.backPage);
        //         return;
        //     }
                
        //     if (customData != null) {
        //         ManualMenuButton menuButton = customData[0] as ManualMenuButton;
        //         if (menuButton != null) {      

        //             if (input.x == UIManager.instance.submitAction){
        //                 if (menuButton.goToPage != null) {
        //                     GoToPage(menuButton.goToPage);
        //                 }
        //                 else {
        //                     if (menuButton.onButtonClick != null) {
        //                         menuButton.onButtonClick(menuButton.data);
        //                     }
        //                     // menuButton.onClick.Invoke ();
        //                 }
        //             }
        //         }
        //     }
        // }


        
        // public bool UIObjectActive() {
        //     return uiObject.gameObject.activeInHierarchy;
        // }

        
        
        // public void SetUIObject (UISelectableHolder uiObject) {
        //     this.uiObject = uiObject;
        // }        
    }
}
