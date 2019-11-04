// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MinimalUI {


    [System.Serializable] public class ManualMenuButton {
        public MenuPage goToPage;
        public string displayName = "Menu Button";
        public string displayDescription = "Description";
        public string callStaticMethod = "";

        // [System.NonSerialized] [HideInInspector] public System.Action<GameObject, object[], Vector2Int> onButtonClick;
        [System.NonSerialized] [HideInInspector] public object[] data;
        public ManualMenuButton (string displayName, string displayDescription, MenuPage goToPage, 
            // System.Action<GameObject, object[], Vector2Int> onButtonClick, 
            object[] data) {
            this.displayName = displayName;
            this.displayDescription = displayDescription;
            this.goToPage = goToPage;
            // this.onButtonClick = onButtonClick;
            this.callStaticMethod = null;
            this.data = data;
        }
    }

    public abstract class ManualMenuButtonBuilder : MonoBehaviour {
        public abstract void AddButtons (List<ManualMenuButton> allButtons, int panelIndex);
    }
        
    // [System.Serializable] public class MenuButtonEvent : UnityEvent { };
 
    public class MenuButton : ManualMenuButtonBuilder {

        public ManualMenuButton menuButton;
        // public MenuPage goToPage;
        // public string displayName = "Menu Button";
        // public MenuButtonEvent onClick = new MenuButtonEvent();

        public override void AddButtons(List<ManualMenuButton> allButtons, int panelIndex) {
            allButtons.Add(menuButton);
        }
    }
}
