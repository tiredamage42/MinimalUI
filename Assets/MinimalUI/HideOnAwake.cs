using UnityEngine;

namespace MinimalUI {
    public class HideOnAwake : MonoBehaviour
    {
        void Awake () {
            UIComponent c = GetComponent<UIComponent>();
            if (c != null) UIManager.HideUIComponent(c, 0);
        }
    }
}
