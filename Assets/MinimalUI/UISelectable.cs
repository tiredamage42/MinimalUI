using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MinimalUI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


using UnityTools;
namespace MinimalUI {

    

    [System.Serializable] public class UIButtonClickWData : UnityEvent<GameObject[]> {}
    [ExecuteInEditMode] public class UISelectable : UIComponent, ISelectHandler, IDeselectHandler, ISubmitHandler
    {    

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }
        
        // public override void OnComponentClose() {
        //     uiText.OnComponentClose();
        //     for (int i =0 ; i < flairs.Length; i++) {
        //         flairs[i].OnComponentClose();
        //     }
        // }

        // public bool selectInvertsTextColor;
        public bool selected;
        [HideInInspector] public UIWithInput parentUI;
        
        UIText _text;
        public UIText uiText {
            get {
                if (_text == null) _text = GetComponentInChildren<UIText>();
                return _text;
            }
        }
        
        // Image _mainImage;
        // public Image mainImage {
        //     get {
        //         if (_mainImage == null)
        //             _mainImage = GetComponentsInChildren<Image>()[0];
        //         return _mainImage;
        //     }
        // }   

        UIImage _image;
        public UIImage image { get { return gameObject.GetComponentInChildrenIfNull<UIImage>(ref _image, true); } }
        Button _button;
        public Button button { get { return gameObject.GetComponentInChildrenIfNull<Button>(ref _button, true); } }
        

        public UIImage[] flairs;
        public object[] data;
        
        public void EnableFlair(int index, bool enabled) {
            if (index >= 0 && index < flairs.Length)
                flairs[index].gameObject.SetActive(enabled);
        }

        // Button _button;
        // Button button {
        //     get {
        //         if (_button == null) {
        //             _button = GetComponent<Button>();
        //             if (_button != null && Application.isPlaying) {
        //                 _button.onClick.AddListener(OnSubmit);
        //             }
        //         }
        //         return _button;
        //     }
        // }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // Button button = this.button;

            
            // Debug.Log("Updating! " + selected);
            image.overrideColor = true;

            Color32 c = selected ? GetUIColor(UIColorScheme.Normal, false) : new Color32(0,0,0,0);
            if (image.mainGraphic.color != c)
                image.mainGraphic.color = c;
            // image.mainGraphic.color = selected ? GetUIColor(UIColorScheme.Normal, false) : new Color32(0,0,0,0);
            
            if (image.mainGraphic.raycastTarget)
                image.mainGraphic.raycastTarget = true;

            // for (int i = 0; i < flairs.Length; i++)
            //     if (flairs[i].gameObject.activeSelf) 
            //         flairs[i].UpdateElementLayout();


            // if (selectInvertsTextColor) uiText.SetColorScheme(UIColorScheme.Normal, selected);

            // return Vector2.zero;
        }
        
        //Do this when the selectable UI object is selected.
        public void OnSelect(BaseEventData eventData)
        {
            // Debug.LogError("SELECTED");
            selected = true;
            UpdateElementLayout();//false, true);
            parentUI.BroadcastSelectEvent(this, data);
        }
        public void OnDeselect(BaseEventData data)
        {
            selected = false;
            UpdateElementLayout();//false, true);
        }

        // having trouble with double submits when using normal actions....
        public void OnSubmit (){
            // parentUI.DoAction(new Vector2Int(settings.submitAction, 0));
        }
        public void OnSubmit (BaseEventData data) {
            // OnSubmit();
        }

       
        // public void DoSubmit (Vector2Int submitAction) {
            // Debug.Log("Submitted on " + name);
            // if (onClick != null) {
            //     onClick.Invoke(data);
            // }
        // }

    }
}
