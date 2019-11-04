using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinimalUI {

    public class UIWithInputCollection : UIWithInput
    {

        public override void OnComponentEnable () {
            base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();
        }
        

        // public override void OnComponentClose() {
        //     base.OnComponentClose();
        //     for (int i = 0; i < subHolders.Length; i++) {
        //         subHolders[i].OnComponentClose();
        //     }
        // }
        // protected override bool IsPopup() { return false; }
        public UIWithInput[] subHolders;

        public override bool CurrentSelectedIsOurs(GameObject currentSelected) {
            // if (isHoldersCollection) {
                for (int i = 0; i< subHolders.Length; i++) {
                    if (subHolders[i].CurrentSelectedIsOurs(currentSelected)) {
                        return true;
                    }
                }
            // }
            // else {
            //     for (int i = 0; i < allElements.Count; i++) {
            //         if (allElements[i].gameObject == currentSelected) {
            //             return true;
            //         }
            //     }
            // }
            return false;
        }
        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // if (isHoldersCollection) {
                for (int i = 0 ; i < subHolders.Length; i++) {
                    subHolders[i].parent = this;
                }
            // }
            // else {

            //     Vector3 textScale = Vector3.one * TextScale();
                
            //     for (int i = 0; i < allElements.Count; i++) {
            //         allElements[i].uiText.transform.localScale = textScale;  
            //         allElements[i].UpdateElement();  
            //     }
            // }

            // return Vector2.zero;
        }
            
        public override void SetSelectablesActive(bool active) {
            // if (!IsPopup()) {
                // if (isHoldersCollection) {
                    for (int i =0 ; i< subHolders.Length; i++) {
                        subHolders[i].SetSelectablesActive(active);
                    }
                    return;
                // }
                // for (int i = 0; i < allElements.Count; i++) {
                //     Button button = allElements[i].GetComponent<Button>();
                //     if (button) {
                //         Navigation customNav = button.navigation;
                //         customNav.mode = active ? Navigation.Mode.Automatic : Navigation.Mode.None;
                //         button.navigation = customNav;
                //     }
                // }
            // }
        }


        
        
    }
}
