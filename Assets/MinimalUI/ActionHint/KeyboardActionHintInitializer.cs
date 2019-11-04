using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {
    [CreateAssetMenu(menuName="Minimal UI/Keyboard Action Hint Initializer", fileName = "Keyboard_ActionHint_Initializer")]
    public class KeyboardActionHintInitializer : ActionHintInitializer
    {
        public KeyboardActionsInterface keyboardActionsInterface;
        public ActionHint keyboardTextHint;

        public override ActionHint GetPrefabForAction (int action) {
            return keyboardTextHint;
        }
        public override ActionHint GetPrefabForAxis (int axis) {
            return keyboardTextHint;
        }
        public override void OnActionHintInitialized (ActionHint actionHintInstance, int action) {
            if (keyboardActionsInterface == null) return;
            if (!actionHintInstance.isKeyboardHint) return;    
            actionHintInstance.keyboardHintText.SetText( keyboardActionsInterface.actions[action].ToString() + " )" );
        }

        public override void OnAxisHintInitialized (ActionHint axisHintInstance, int action) {
            if (keyboardActionsInterface == null) return;
            if (!axisHintInstance.isKeyboardHint) return;
            axisHintInstance.keyboardHintText.SetText( keyboardActionsInterface.axesNeg[action].ToString() + "/" + keyboardActionsInterface.axesPos[action].ToString() + " )");
        }

        
    }
}
