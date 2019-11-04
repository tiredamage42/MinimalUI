using UnityEngine;

using System.Collections.Generic;
using UnityTools;

// using MinimalUI;
using UnityTools.GameSettingsSystem;
namespace MinimalUI {
        
    public abstract class MenuPage : MonoBehaviour {

        static UISettings _settings;
        protected static UISettings settings {
            get {
                if (_settings == null) _settings = GameSettings.GetSettings<UISettings>();
                return _settings;
            }
        }

        public string uiObjectName = "PageWithPanelPage";

        UIWithInput _uiObject;
        protected UIWithInput uiObject {
            get {
                if (_uiObject == null) _uiObject = UIManager.GetUIComponentByName<UIWithInput>(uiObjectName);
                return _uiObject;
            }
        }

        // public UIWithInput uiObject;

        bool IsUIPage (out UIPage page) {
            page = uiObject as UIPage;
            return page != null;
        }
        bool IsCollection (out UIWithInputCollection collection) {
            collection = uiObject as UIWithInputCollection;
            return collection != null;
        }
        bool isCollection { get { return IsCollection(out _); } }
        bool isPage { get { return IsUIPage(out _); } }
        bool isPaginated { get { return (isCollection || isPage) && MaxButtons() > 0; } }
        

        // bool isCollection { get { return (uiObject as UIWithInputCollection) != null; } }
        // bool usesRadial { get { return (uiObject as UIRadial) != null; } }
        // bool isPaginated { get { return !usesRadial && MaxButtons() > 0; } }

    
        void OnEnable () {
            // defaultActions = new List<int> () { settings.submitAction };
            // defaultHints = new List<string> () { "Select" };

            int maxUIPanels = 1;
            UIWithInputCollection collection;
            if (IsCollection(out collection)) {
                maxUIPanels = collection.subHolders.Length;
            }
            buttonReferences = new UISelectable[maxUIPanels][];
            paginatedOffsets = new int[maxUIPanels];
            lastElementsShownCount = new int[maxUIPanels];


            BuildActionsAndHints(out pageActions, out pageActionHints);

            if (allowCancel) {
                pageActions.Add(settings.cancelAction);
                pageActionHints.Add("Cancel/Back");
            }
        }

        [HideInInspector] public MenuPage backPage;
        [HideInInspector] public ManualMenu manualMenu;

        // List<int> defaultActions;// = new List<int> () { settings.submitAction };
        // List<string> defaultHints;// = new List<string> () { "Select" };


        List<int> pageActions = new List<int>();
        List<string> pageActionHints = new List<string>();

        protected virtual void BuildActionsAndHints (out List<int> actions, out List<string> hints) {
            actions = new List<int> () { settings.submitAction };
            hints = new List<string> () { "Select" }; 
        }

        // protected virtual void GetActionsAndHints (out List<int> actions, out List<string> hints) {
        //     actions = GetActions();
        //     // hints = defaultHints;
        //     hints = new List<string> () { "Select" }; 
        // }

        // protected virtual List<int> GetActions () {
        //     // return defaultActions;
        //     return new List<int> () { settings.submitAction };
        // }

        public void UninitializeUI () {
            // UIManager.HideUI(uiObject);
            UIManager.HideUIComponent(uiObject, 0);

            // UIInput.MarkActionsUnoccupied (pageActions);// GetActions());
            for(int i = 0; i < buttonReferences.Length; i++) buttonReferences[i] = null;
        }

        protected abstract void OnOpenUI (Actor[] actorContexts);
        // protected object[] openedWithParams;

        public void InitializeUI (Actor[] actorContexts) {
            // UIManager.ShowUI(uiObject);


            uiObject.SetOverrideActions(pageActions, pageActionHints);

            UIManager.ShowUIComponent(uiObject, 0, 0, 0);

            // Debug.Log("Subscribing");
            uiObject.SubscribeToActionEvent(OnUIInput);    
            uiObject.SubscribeToSelectEvent(OnUISelect);

            if (isPaginated) {
                uiObject.SubscribeToSelectEvent(OnPaginatedUISelect);
                for(int i = 0; i < paginatedOffsets.Length; i++) paginatedOffsets[i] = 0;
            }
            
            // this.openedWithParams = parameters;

            // List<int> actions;
            // List<string> hints;
            // GetActionsAndHints (out actions, out hints);

            // UIInput.MarkActionsOccupied (pageActions);// actions);

            // if (allowCancel) {
            //     actions.Add(settings.cancelAction);
            //     hints.Add("Cancel/Back");
            // }

            // uiObject.actionHandler = GetUIActions;

            // uiObject.AddActionHints(actions, hints);
            // uiObject.AddActionHints(pageActions, pageActionHints);

            OnOpenUI( actorContexts );
        }

        protected abstract int MaxButtons ();
        int[] lastElementsShownCount;
        protected UISelectable[][] buttonReferences;
        int[] paginatedOffsets;


        void SetTitle(string title, int panel) {

            UISelectableHolder holder;
            UIWithInputCollection collection;
            
            if (IsCollection(out collection))
                holder = collection.subHolders[panel] as UISelectableHolder;

            else holder = uiObject as UISelectableHolder;
            
            if (holder != null) 
                holder.SetMainText(title);

            // if (uiObject.textPanel != null)
            //     uiObject.textPanel.SetTexts(string.Empty, string.Empty, false);
                // uiObject.textPanel.titleText.SetText("");
        }

    
        protected void BuildButtons (string title, bool setSelection, int panelIndex) {

            SetTitle(title, panelIndex);

            UISelectableHolder uiObject = null;

            UIWithInputCollection collection;
            if (IsCollection(out collection)) {
                uiObject = collection.subHolders[panelIndex] as UISelectableHolder;
            }
            else {
                uiObject = this.uiObject as UISelectableHolder;
            }

            // if (isCollection) {
            //     uiObject = (this.uiObject as UIWithInputCollection).subHolders[panelIndex] as UISelectableHolder;
            // }
            // else {
            //     uiObject = this.uiObject as UISelectableHolder;
            // }

            if (uiObject != null) {
                buttonReferences[panelIndex] = uiObject.GetAllSelectableElements(MaxButtons(), false);
                
                if (setSelection) {
                    if (buttonReferences[panelIndex].Length > 0) {
                        // Debug.Log("setting selection");
                        UIManager.SetSelectionDelayed(buttonReferences[panelIndex][0].gameObject);
                    }
                }
                UpdateUIButtons(panelIndex);//, true);


            }

            
        }

        protected abstract void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex);
        public List<ManualMenuButton> GetAllButtons (int panelIndex) {
            List<ManualMenuButton> b = new List<ManualMenuButton>();
            GetInternalButtons (b, panelIndex);
            
            // for (int i = 0; i < buttonBuilders.Length; i++) 
            //     buttonBuilders[i].AddButtons(b, panelIndex);
            
            return b;
        }
        
        protected abstract void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex);
                
        void MakeButton (UISelectable element, string text, ManualMenuButton button, int panelIndex, object[] data, bool enableFlairs) {
            element.uiText.SetText(text);
            element.data = data;

            element.EnableFlair(0, false);
            element.EnableFlair(1, false);
            if (enableFlairs)
                SetFlairs(element, button, panelIndex);
            
        }


        protected void UpdateUIButtons (int panelIndex)//, bool firstBuild)
        {

            // 7 count, max 6



            List<ManualMenuButton> allButtons = GetAllButtons(panelIndex);
            
            lastElementsShownCount[panelIndex] = allButtons.Count;
            
            UISelectable[] elements = buttonReferences[panelIndex];
            
            int start = 0;
            int end = allButtons.Count;
            
            /* ▲ ▼ */
            if (isPaginated) {
                end = MaxButtons();

                bool isAtBeginning = paginatedOffsets[panelIndex] == 0;
                if (!isAtBeginning){
                    MakeButton(elements[0], "▲", null, panelIndex, new object[]{ "B", panelIndex}, false );
                    start = 1;
                }

                // bool isAtEnd = paginatedOffsets[panelIndex] >= ((allButtons.Count) - (MaxButtons() + start));
                bool isAtEnd = paginatedOffsets[panelIndex] + (MaxButtons() - (!isAtBeginning ? 1 : 0)) >= ((allButtons.Count));
                
                if (!isAtEnd) {
                    MakeButton(elements[MaxButtons()-1], "▼", null, panelIndex, new object[]{ "F", panelIndex}, false );
                    end = MaxButtons()-1;
                }
            }
            
            for (int i = start ; i < end; i++) {
                int index = isPaginated ? (i-start) + paginatedOffsets[panelIndex] : i;
                // int index = isPaginated ? (i) + paginatedOffsets[panelIndex] : i;
                
                if (index < allButtons.Count)
                    MakeButton( elements[i], allButtons[index].displayName, allButtons[index], panelIndex, new object[] { allButtons[index], panelIndex }, true );
                else 
                    MakeButton( elements[i], "Empty", null, panelIndex, new object[] { null, panelIndex}, false );
            }

            uiObject.UpdateElementLayout();//firstBuild);
        }  
    

        public virtual void OnUISelect (UISelectable selectedObject, object[] data) {
            if (uiObject.textPanel != null) {
                if (data != null) {
                    ManualMenuButton menuButton = data[0] as ManualMenuButton;
                    if (menuButton != null)
                        uiObject.SetPanelText(menuButton.displayDescription);
                        // uiObject.textPanel.SetTexts(string.Empty, menuButton.displayDescription, true);
                        // uiObject.textPanel.mainText.SetText(menuButton.displayDescription);
                }
            }
        }


        // protected Vector2Int GetUIActions () {
        //     // List<int> actions = GetActions();
        //     // if (allowCancel) {
        //     //     actions.Add(settings.cancelAction);
        //     // }
        //     // Debug.Log("getting actions " + actions.Count);
        //     return UIInput.GetUIActions(pageActions);// actions);
        // }

        public bool allowCancel = true;

        protected abstract void OnUIInput (UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex);
    
        public virtual void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input) {
            if (allowCancel) {
                if (input.x == settings.cancelAction) {
                    // Debug.Log("GOINGBAKC");
                    manualMenu.GoToPage(backPage);
                    return;
                }
            }
                
            if (data != null) {
                ManualMenuButton menuButton = data[0] as ManualMenuButton;
                if (menuButton != null) {      

                    if (input.x == settings.submitAction){
                        if (menuButton.goToPage != null) {
                            // Debug.Log("GOING TO PAGE");
                            menuButton.goToPage.backPage = this;
                            manualMenu.GoToPage(menuButton.goToPage);
                        }
                        else {
                            if (!string.IsNullOrEmpty(menuButton.callStaticMethod)) {
                                SystemTools.CallStaticMethodSimple(menuButton.callStaticMethod);
                            }
                        }
                    }

                    int panelIndex = (int)data[1];
                    
                    OnUIInput(selectedObject, menuButton.data, input, panelIndex);
                }
            }
        }

        // ManualMenuButtonBuilder[] _buttonBuilders;
        // ManualMenuButtonBuilder[] buttonBuilders { get { return gameObject.GetComponentsIfNull<ManualMenuButtonBuilder>(ref _buttonBuilders); } }

        // handle paginated scrolling
        void OnPaginatedUISelect (UISelectable selectedObject, object[] data) {
			if (data != null) {
                
                string buttonSelectText = data[0] as string;
                if (buttonSelectText != null) {

                    int panelIndex = (int)data[1];
                    
                    bool updateButtons = false;
                    UISelectable newSelection = null;

                    // hovered over the page up button
                    if (buttonSelectText == "B") {
                        paginatedOffsets[panelIndex]--;

                        if (paginatedOffsets[panelIndex] != 0) {
                            newSelection = buttonReferences[panelIndex][1];
                        }
                        
                        if (paginatedOffsets[panelIndex] == 1)
                            paginatedOffsets[panelIndex]--;

                        updateButtons = true;
                    } 
                    
                    // hovered over the page down button
                    else if (buttonSelectText == "F") {


                        
                        paginatedOffsets[panelIndex]++;
                        
                        
                        // bool isAtEnd = paginatedOffsets[panelIndex] >= lastElementsShownCount[panelIndex] - MaxButtons();
                        // bool isAtEnd = paginatedOffsets[panelIndex] + (MaxButtons() - (paginatedOffsets[panelIndex] != 0 ? 2 : 0)) >= ((lastElementsShownCount[panelIndex]));
                        bool isAtEnd = paginatedOffsets[panelIndex] + (MaxButtons() - ( 1 )) >= ((lastElementsShownCount[panelIndex]));
                        
                        if (!isAtEnd) {
                            newSelection = buttonReferences[panelIndex][MaxButtons() - 2];
                            paginatedOffsets[panelIndex]++;
                        }
                    
        
                        updateButtons = true;
                    }

                    if (updateButtons){
                        UpdateUIButtons( panelIndex );//, false );
                        
                        if (newSelection != null) {
                            UIManager.SetSelectionDelayed(newSelection.gameObject);
                        }
                    }
                }   
            }
		}
        
    }
}