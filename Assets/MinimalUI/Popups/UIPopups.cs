using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


using UnityTools;
using UnityTools.GameSettingsSystem;

namespace MinimalUI {
    public class UIPopups
    {
        static UISettings _settings;
        static UISettings settings {
            get {
                if (_settings == null) _settings = GameSettings.GetSettings<UISettings>();
                return _settings;
            }
        }

        public static event Action onPopupOpen, onPopupClose;
        public static bool popupOpen;

        static GameObject selectedWhenPoppedUp;

        static void HidePopup ( UIObject popup) {
            if (onPopupClose != null) {
                onPopupClose();
            }
            // UIManager.HideUI(popup);
            UIManager.HideUIComponent(popup, 0);

            // if (UIManager.shownUIsWithInput.Count == 0) {
                // List<int> actions = new List<int> { settings.submitAction, settings.cancelAction };
                // UIInput.MarkAllBaseUIInputsUnoccupied();
                // UIInput.MarkActionsUnoccupied(actions);
            // }

            // UIManager.SetAllActiveUIsSelectableActive(true);
            popupOpen = false;
            UIManager.SetSelectionDelayed(selectedWhenPoppedUp);
        }

        static void ShowPopup (bool saveSelection, UIWithInput popup, Action<UISelectable, object[], Vector2Int> onSubmit) {
            if (onPopupOpen != null) {
                onPopupOpen();
            }

            // Debug.Log("Displaying popup");
            
            if (saveSelection) {
                selectedWhenPoppedUp = UIManager.CurrentSelected();
            }

            // List<int> actions = new List<int> { settings.submitAction, settings.cancelAction };
            // List<string> hints = new List<string> { "Confirm", "Cancel" };
            
            // mark actions occupied if no ui open
            // if (UIManager.shownUIsWithInput.Count == 0) {
            //     // UIInput.MarkAllBaseUIInputsOccupied();
            //     UIInput.MarkActionsOccupied(actions);
            // }

            popup.SetOverrideActions(null, null);

            // Debug.Log("SHOWING POPUP");

            // UIManager.ShowUI(popup);
            UIManager.ShowUIComponent(popup, 0, 0, 0);
            popupOpen = true;

            // popup.AddActionHints(actions, hints);

            // UIManager.SetAllActiveUIsSelectableActive(false);
            popup.SubscribeToActionEvent(onSubmit);
        }

        static Action<bool, int> selectionReturnCallback;

        static void MakeButton (UISelectable element, string text, object[] data) {
            element.uiText.SetText(text);//, -1);
            element.data = data;
            element.EnableFlair(0, false);
            element.EnableFlair(1, false);
        }

        public static void ShowSelectionPopup(bool saveCurrentSelection, string msg, string[] options, Action<bool, int> returnValue) {
            UISelectionPopup popup = selectionPopup;//UISelectionPopup.instance;
            if (popup == null) {
                returnValue(false, 0);
                return;
            }
            
            ShowPopup (saveCurrentSelection, popup, OnSelectionSubmit);

            selectionReturnCallback = returnValue;

            popup.text.SetText( msg );

            UISelectable[] allElements = popup.GetAllSelectableElements(options.Length, false);
            
            UIManager.SetSelectionDelayed(allElements[0].gameObject);
            
            for (int i = 0 ; i < options.Length; i++) {
                MakeButton( allElements[i], options[i], new object[] { i } );   
            }
            popup.UpdateElementLayout();//true);
        }

        static void OnSelectionSubmit (UISelectable selectedObject, object[] data, Vector2Int input) {
            if (input.x == settings.cancelAction) {
                OnCancelSelectionUI();
                return;
            }
            else if (input.x == settings.submitAction) {
                
                HidePopup(selectionPopup);//.instance);
                
                if (selectionReturnCallback != null) {
                    selectionReturnCallback(true, (int)data[0]);

                    // not setting to null because callback might bring up another popup...
                    // selectionReturnCallback = null;
                }
            }   
        }
        static void OnCancelSelectionUI () {
            HidePopup( selectionPopup);//UISelectionPopup.instance);
            if (selectionReturnCallback != null) {
                selectionReturnCallback (false, 0);
            }
        }




        











        static Action onConfirmation;
        static void OnConfirmationReturn (bool used, int selected) {
            if (used && selected == 0) {
                if (onConfirmation != null) {
                    onConfirmation();
                    onConfirmation = null;
                }
            }
        }
        static readonly string[] confirmationOptions = new string[] { "Yes", "No" };
        public static void ShowConfirmationPopup(bool saveCurrentSelection, string msg, Action onConfirmation) {
            UIPopups.onConfirmation = onConfirmation;
            ShowSelectionPopup(saveCurrentSelection, msg, confirmationOptions, OnConfirmationReturn);
        }










        static UISelectionPopup _selectionPopup;
        static UISelectionPopup selectionPopup { get { return UIManager.instance.gameObject.GetComponentInChildrenIfNull<UISelectionPopup>(ref _selectionPopup, true); } }
        static UISliderPopup _sliderPopup;
        static UISliderPopup sliderPopup { get { return UIManager.instance.gameObject.GetComponentInChildrenIfNull<UISliderPopup>(ref _sliderPopup, true); } }
        











        static void OnSliderChange (UISelectable selected, object[] data) {
            float value = (float)data[0];
            sliderChangeCallback((int)value);
        }

        static Action<int> sliderChangeCallback;
        static Action<bool, int> sliderReturnCallback;
        public static void ShowIntSliderPopup(bool saveCurrentSelection, string title, int initialValue, int minValue, int maxValue, 
            Action<bool, int> returnValue, Action<int> onSliderChange
        ) {
            UISliderPopup popup = sliderPopup;//UISliderPopup.instance;
            if (popup == null) {
                Debug.Log("No Slider Popup Found");
                returnValue(false, 0);
                return;
            }

            ShowPopup (saveCurrentSelection, popup, OnSliderSubmit);

            sliderReturnCallback = returnValue;

            sliderChangeCallback = onSliderChange;
            if (onSliderChange != null) popup.SubscribeToSelectEvent(OnSliderChange);
            
            popup.SetTitle(title);
            popup.slider.wholeNumbers = true;
            popup.slider.minValue = minValue;
            popup.slider.maxValue = maxValue;
            popup.slider.value = initialValue;
        }

        static void OnSliderSubmit (UISelectable selectedObject, object[] data, Vector2Int input) {
            if (input.x == settings.cancelAction) {
                OnCancelSliderUI();
                return;
            }
            else if (input.x == settings.submitAction) {
                HidePopup(sliderPopup);//UISliderPopup.instance);
                if (sliderReturnCallback != null) {
                    sliderReturnCallback(true, (int)sliderPopup.sliderValue);
                }
            }
        }
        static void OnCancelSliderUI () {
            HidePopup( sliderPopup);//UISliderPopup.instance);
            if (sliderReturnCallback != null) {
                sliderReturnCallback (false, 0);
            }
        }
    }
}
