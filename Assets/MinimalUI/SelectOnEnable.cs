using UnityEngine;

namespace MinimalUI {
    public class SelectOnEnable : MonoBehaviour
    {
        bool hasSelected;
        public GameObject toSelect;
        void OnDisable () {
            if (hasSelected) {
                UIManager.SetSelection(null);
                hasSelected = false;
            }
        }

        void Update()
        {
            if (!hasSelected){
                // Debug.LogError("selected :: " + toSelect.name+ " on " + name);
                UIManager.SetSelection(toSelect);
                hasSelected = true;
            }
        }
    }
}
