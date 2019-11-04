


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;

using UnityTools;
namespace MinimalUI {

    public abstract class UIWithInput : UIObject
    {

        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     if (actionHintsPanel != null)
        //         actionHintsPanel.OnComponentClose();
        // }

        public ActionHintsPanel hintsPanel;
        ActionHintsPanel _actionHintsPanel;
        protected ActionHintsPanel actionHintsPanel {
            get {
                if (hintsPanel != null) return hintsPanel;
                if (_actionHintsPanel == null) _actionHintsPanel = GetComponentInChildren<ActionHintsPanel>();
                return _actionHintsPanel;
            }
        }

        bool enabledFrame;
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            // SetSelectablesActive(true);
            enabledFrame = true;

            AddActionHints(GetUsedActions(), GetUsedHints());

            Debug.Log("On Component enabled");
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
            if (Application.isPlaying) {
                RemoveAllEvents();

                SetOverrideActions(null, null);

                // SetSelectablesActive(false);
            }
        }

        public UITextPanel textPanel;

        public void SetPanelText (string text) {
            textPanel.SetTexts(null, text, false);
            UpdateElementLayout();
        }

            
        

        

        // protected virtual void OnDisable () {
            // if (Application.isPlaying)
            //     RemoveAllEvents();
            // if (!isQuitting) {
            //     RemoveActionHints();
            // }
        // }
        

        // public 
        void AddActionHints (List<int> actionHints, List<string> hintNames) {
            if (actionHintsPanel != null) {
                actionHintsPanel.AddHintElements (actionHints, hintNames);
            }
            else {
                Debug.LogError("cant add hint for " + name + " contorl hint panel == null");
            }
        }
        // public void RemoveActionHints () {
        //     if (actionHintsPanel != null) 
        //         actionHintsPanel.RemoveAllHintElements();
        // }

        [HideInInspector] public UIWithInput parent = null;
        public abstract bool CurrentSelectedIsOurs (GameObject currentSelected);
        // protected abstract bool IsPopup();
        public abstract void SetSelectablesActive(bool active);


        public void DoAction (Vector2Int action) {
            if (parent != null) {
                parent.DoAction(action);
                return;                
            }
            
            object[] data;
            UISelectable selected = GetCurrentSelectedData ( out data);
            
            BroadcastActionEvent(selected, data, action);
        }

        
        UISelectable GetCurrentSelectedData (out object[] data) {
        
            data = null;

            GameObject currentSelected = UIManager.CurrentSelected();
            if (currentSelected == null)
                return null;
            
            if (!CurrentSelectedIsOurs(currentSelected)) {
                Debug.LogError("Getting Input for" + name + " but current selected isnt part of our set, "+currentSelected.name);
                return null;
            }

            UISelectable currentSelectedCheck = currentSelected.GetComponent<UISelectable>();
            if (currentSelectedCheck == null)
                return null;
            
            data = currentSelectedCheck.data;
            return currentSelectedCheck;
        }
        
        // protected override 
        protected virtual void Update () {
            // base.Update();
            if (Application.isPlaying) {
                if ((object)parent == null) {

                    if (UIManager.UIIsLastInStack(this)) {
                    // if (!UIPopups.popupOpen || IsPopup()) {

                        if (isActive) {

                            // was having issues with input that opens ui, counting towards newly opened ui
                            if (enabledFrame) {
                                enabledFrame = false;
                                return;
                            }

                            // Vector2Int action = new Vector2Int(-1,-1);


                            Vector2Int action = UIInput.GetUIActions( GetUsedActions() );
                            
                            // if (actionHandler == null) {
                            //     action = DefaultActionHandler();
                            // }
                            // else {
                            //     action = actionHandler();
                            // }
                            if (action.x >= 0) {
                                // Debug.Log("Doing Action");
                                DoAction(action);
                            }
                        }
                    }  
                }
            }
        }


        List<int> overrideActions;
        List<string> overrideActionHints;

        static List<int> _defaultActions;
        static List<int> defaultActions {
            get {
                if (_defaultActions == null) _defaultActions = new List<int> { settings.cancelAction, settings.submitAction };
                return _defaultActions;
            }
        }
        static List<string> _defaultHints;
        static List<string> defaultHints {
            get {
                if (_defaultHints == null) _defaultHints = new List<string> { "Submit", "Cancel/Back" };
                return _defaultHints;
            }
        }

        public void SetOverrideActions (List<int> overrideActions, List<string> overrideActionHints) {
            this.overrideActions = overrideActions;
            this.overrideActionHints = overrideActionHints;
        }

        public List<int> GetUsedActions () {
            return overrideActions != null ? overrideActions : defaultActions;
        }
        public List<string> GetUsedHints () {
            return overrideActionHints != null ? overrideActionHints : defaultHints;
        }

        // static Vector2Int DefaultActionHandler (){
        //     List<int> actions = new List<int> { settings.cancelAction, settings.submitAction };
        //     return UIInput.GetUIActions( actions );
        // }

        // Func<Vector2Int> _actionHandler;
        // public Func<Vector2Int> actionHandler { 
        //     get { return (object)parent != null ? parent.actionHandler : _actionHandler; } 
        //     set {
        //         if ((object)parent != null) {
        //             parent.actionHandler = value;
        //         }
        //         else {
        //             _actionHandler = value;
        //         }
        //     }
        // }

        
        

        List<Action<UISelectable, object[]>> onSelectdelegates = new List<Action<UISelectable, object[]>>();
        event Action<UISelectable, object[]> _onSelect;
        
        public void BroadcastSelectEvent (UISelectable buttonObject, object[] data) {
            if ((object)parent != null) {
                parent.BroadcastSelectEvent(buttonObject, data);
                return;
            }
            if (_onSelect != null) _onSelect(buttonObject, data);
        }
        public void SubscribeToSelectEvent (Action<UISelectable, object[]> callback) {
            if ((object)parent != null) {
                parent.SubscribeToSelectEvent(callback);
                return;
            }
            _onSelect += callback;
            onSelectdelegates.Add(callback);
        }

        List<Action<UISelectable, object[], Vector2Int>> onActionDelegates = new List<Action<UISelectable, object[], Vector2Int>>();
        event Action<UISelectable, object[], Vector2Int> _onAction;
        public void BroadcastActionEvent (UISelectable selected, object[] data, Vector2Int submit) {
            
            // Debug.Log("Boradcast action" + name);
            if ((object)parent != null) {
                parent.BroadcastActionEvent(selected, data, submit);
                return;
            }
            if (_onAction != null) _onAction(selected, data, submit);
        }
        public void SubscribeToActionEvent (Action<UISelectable, object[], Vector2Int> callback) {
            
            // Debug.Log("Subscribing events " + name);
            
            if ((object)parent != null) {
                parent.SubscribeToActionEvent(callback);
                return;
            }
            _onAction += callback;
            onActionDelegates.Add(callback);
        }

        
        public void RemoveAllEvents()
        {
            // Debug.Log("Removing events " + name);

            for (int i = 0; i < onSelectdelegates.Count; i++) _onSelect -= onSelectdelegates[i];
            // foreach(var eh in onSelectdelegates) _onSelect -= eh;
            onSelectdelegates.Clear();
            
            
            for (int i = 0; i < onActionDelegates.Count; i++) _onAction -= onActionDelegates[i];
            // foreach(var  eh in onActionDelegates) _onAction -= eh;
            onActionDelegates.Clear();

            // actionHandler = null;
        }        
    }
}
