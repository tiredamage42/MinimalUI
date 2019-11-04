using UnityEngine;
using UnityTools;
namespace MinimalUI {
    public abstract class UIObject : UIComponent
    {
        
        // [Tooltip("Object to Enable / Disable")]
        // public GameObject baseGameObject;
        // public GameObject baseObject { get { return baseGameObject != null ? baseGameObject : gameObject; } }

        // public override void OnComponentClose() {
        //     if (textPanel != null)
        //         textPanel.OnComponentClose();
            
        //     if (panel != null)
        //         panel.OnComponentClose();
        // }
        // public UITextPanel textPanel;

        UIPanel _uiPanel;
        protected UIPanel panel { get { return gameObject.GetComponentInChildrenIfNull<UIPanel>(ref _uiPanel, true); } }
        
        // public void EnableUIObject (bool enabled) {
        //     baseObject.SetActive(enabled);
        // }       
    }
}