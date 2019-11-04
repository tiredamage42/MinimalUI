using System.Collections.Generic;
using UnityEngine;
using UnityTools;

namespace MinimalUI {

    public class UIVideoPlayerSkipControls : UIWithInput
    {
        public override void SetSelectablesActive (bool active) { }
        // protected override bool IsPopup() { return false; }
        public override bool CurrentSelectedIsOurs(GameObject g) { return true; }
        public override void UpdateElementLayout () { }

        UIVideoPlayer _uiVideoPlayer;
        UIVideoPlayer uiVideoPlayer { get { return UIManager.instance.gameObject.GetComponentInChildrenIfNull<UIVideoPlayer>(ref _uiVideoPlayer, true); } }

        void Awake () {
            pageActions = new List<int> () { settings.submitAction };
            pageActionHints = new List<string> () { "Skip" }; 

            // actionHandler = GetUIActions;
            // SubscribeToActionEvent(OnUIInput);    
        }

        public override void OnComponentEnable () {
            // need to set this before probablt....
            SetOverrideActions(pageActions, pageActionHints);

            // Debug.Log("hiding panel");
            UIManager.HideUIComponent(actionHintsPanel, 0);
            actionsShowing = false;

            base.OnComponentEnable();

            UIInput.MarkActionsOccupied (pageActions);
            SubscribeToActionEvent(OnUIInput);    
        }

        void ShowActionHints () {
            UIManager.ShowUIComponent(actionHintsPanel, .1f, 0, 0);
            actionsShowing = true;


            // AddActionHints(pageActions, pageActionHints);
        }

        public override void OnComponentDisable () {

            base.OnComponentDisable();

            UIInput.MarkActionsUnoccupied (pageActions);
            actionsShowing = false;
        }

        List<int> pageActions;
        List<string> pageActionHints;

        // Vector2Int GetUIActions () {
        //     return UIInput.GetUIActions(pageActions);            
        // }

        bool actionsShowing;

        public virtual void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input) {
                
            if (input.x == settings.submitAction){
                Debug.LogError("PRESSED SUBMIT");

                if (actionsShowing)
                    uiVideoPlayer.Stop();
                else 
                    ShowActionHints();
                
                // if action hints panel showing, stop movie
                // else fade in action hints panel
            }
        }
    }
}



