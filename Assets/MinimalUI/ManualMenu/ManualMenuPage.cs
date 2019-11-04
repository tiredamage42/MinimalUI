using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools;
namespace MinimalUI {
    // continue, new, load, settings, 
    public class ManualMenuPage : MenuPage {
        ManualMenuButtonBuilder[] _buttonBuilders;
        ManualMenuButtonBuilder[] buttonBuilders { get { return gameObject.GetComponentsIfNull<ManualMenuButtonBuilder>(ref _buttonBuilders, true); } }

        public string pageTitle = "Page Title";
        public int maxButtons = 8;
        protected override int MaxButtons() {

            return buttonBuilders.Length;
            // return maxButtons;
        }

        protected override void OnOpenUI(Actor[] actorContexts) {
            BuildButtons(pageTitle, true, 0);
        }
        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) {
            
        }
        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            
        }
        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {
            for (int i = 0; i < buttonBuilders.Length; i++) 
                buttonBuilders[i].AddButtons(buttons, panelIndex);
            
        }
    }
}
