using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using UnityEngine.UI;



using UnityTools;
namespace MinimalUI {

    // TODO: unselect on disable

    [ExecuteInEditMode] public abstract class UISelectableHolder : UIWithInput
    {
        // bool initialized;
        // protected abstract float TextScale();
        // public UIElementHolder[] subHolders;


        static PrefabPool<UISelectable> selectablePool = new PrefabPool<UISelectable>();


        public abstract void SetMainText (string text);
        
        protected abstract UISelectable ElementPrefab () ;

        // bool isHoldersCollection { get { return subHolders != null && subHolders.Length > 0; } }

        // Transform _elementsParent;
        // Transform elementsParent {
        //     get {
        //         // if (isHoldersCollection) return null;
        //         if (_elementsParent == null) {
        //             _elementsParent = ElementsParent();
        //             // Debug.Log(_elementsParent);
        //         }
        //         return _elementsParent;
        //     }
        // }


        // protected abstract Transform ElementsParent ();
        public List<UISelectable> showElements = new List<UISelectable>();
        // public List<UISelectable> allElements = new List<UISelectable>();

        public override void OnComponentEnable () {
            base.OnComponentEnable();
        
        }
        public override void OnComponentDisable () {
            base.OnComponentDisable();

            for (int i = 0; i < showElements.Count; i++) {
                showElements[i].selected = false;
                showElements[i].gameObject.SetActive(false);
                showElements[i].transform.SetParent(UIMainCanvas.instance.transform);
            }
            // allElements.AddRange(showElements);
            showElements.Clear();            
        }
        

        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     for (int i = 0; i < shownElements.Count; i++) {
        //         // shownElements[i].selected = false;
        //         shownElements[i].gameObject.SetActive(false);
        //         shownElements[i].transform.SetParent(UIMainCanvas.instance.transform);
        //     }
        //     shownElements.Clear();

        //     // for (int i = 0; i< allElements.Count; i++)
        //     //     allElements[i].OnComponentClose();
        // }

        public override bool CurrentSelectedIsOurs(GameObject currentSelected) {
            // if (isHoldersCollection) {
            //     for (int i = 0; i< subHolders.Length; i++) {
            //         if (subHolders[i].CurrentSelectedIsOurs(currentSelected)) {
            //             return true;
            //         }
            //     }
            // }
            // else {

                // for (int i = 0; i < allElements.Count; i++) {
                //     if (allElements[i].gameObject == currentSelected) {
                for (int i = 0; i < showElements.Count; i++) {
                    if (showElements[i].gameObject == currentSelected) {
                
                        return true;
                    }
                }
            // }
            return false;
        }


        // just debugging....
        public UISelectable AddNewElement (string elementText, bool updateHolder) {
            // if (isHoldersCollection) return null;
            
            UISelectable newElement = Instantiate(ElementPrefab());
            newElement.parentUI = this;
            newElement.transform.SetParent( transform, Vector3.zero, Quaternion.identity, Vector3.one );
            // newElement.transform.localPosition = Vector3.zero;
            // newElement.transform.localRotation = Quaternion.identity;
            // newElement.transform.localScale = Vector3.one;
            
            // allElements.Add(newElement);
            showElements.Add(newElement);
            
            newElement.uiText.SetText( elementText );//, -1);

            if (updateHolder) {
                // Debug.Log("Updating after Add");
                UpdateElementLayout();//true, true);// UpdateSelectableElementHolder();
            }
            return newElement;
        }


        UISelectable GetAvailableSelectable () {
            UISelectable s = selectablePool.GetAvailable(ElementPrefab(), null, false, null);
            s.selected = false;
            s.parentUI = this;
            // s.transform.SetParent(transform);
            s.transform.SetParent( transform, Vector3.zero, Quaternion.identity, Vector3.one );

            Button button = s.button;//.GetComponent<Button>();
            if (button) {
        
                Navigation customNav = button.navigation;
                customNav.mode = Navigation.Mode.Automatic;// : Navigation.Mode.None;
                // customNav.mode = active ? Navigation.Mode.Horizontal : Navigation.Mode.None;
                button.navigation = customNav;
                
            }

            s.gameObject.SetActive(true);
            
            return s;
        }

        // void GetSelectableElementReferences () {
                
        //     // if (isHoldersCollection) return;

        //     if (Application.isPlaying) {
        //         initialized = false;
        //         allElements.Clear();
        //     }

        //     if (!initialized) {
        //         UISelectable[] _allElements = GetComponentsInChildren<UISelectable>(true);
        //         for (int i = 0; i < _allElements.Length; i++) {
        //             _allElements[i].parentUI = this;
        //             allElements.Add(_allElements[i]);                
        //         }
        //         initialized = true;
        //     }
        // }

        // public override void UpdateElementLayout() {
            // if (isHoldersCollection) {
            //     for (int i = 0 ; i < subHolders.Length; i++) {
            //         subHolders[i].parentElement = this;
            //     }
            // }
            // else {

                // Vector3 textScale = Vector3.one * TextScale();
                
                // for (int i = 0; i < allElements.Count; i++) {
                //     allElements[i].uiText.transform.localScale = textScale;  
                //     allElements[i].UpdateElementLayout();  
                // }
            // }
            
        // }

        // public virtual void UpdateSelectableElementHolder () {
        //     if (isHoldersCollection) return;

        //     Vector3 textScale = Vector3.one * TextScale();
            
        //     for (int i = 0; i < allElements.Count; i++) {
        //         allElements[i].uiText.transform.localScale = textScale;  
        //         allElements[i].UpdateElement();  
        //     }
        // }

        // void InitializeSubSelectableElementHolders () {
        //     if (!isHoldersCollection) return;
        //     for (int i = 0 ; i < subHolders.Length; i++) {
        //         subHolders[i].parentElement = this;
        //     }
        // }

        // void InitializeSelectableElements () {
        //     GetSelectableElementReferences();
        //     UpdateSelectableElementHolder();    
        // }
        
        // protected virtual void OnEnable () {
        // protected override void OnEnable () {
        //     GetSelectableElementReferences();
        //     base.OnEnable();
        //     // InitializeSubSelectableElementHolders();
        //     // InitializeSelectableElements();
        // }
               
        // protected virtual void Update () {
//         protected override 
//         void Update () {
//             // base.Update();
// #if UNITY_EDITOR 
//             if (!Application.isPlaying) {
//                 GetSelectableElementReferences();
//                 // InitializeSelectableElements();
//             }
// #endif
//             base.Update();
//         }


        public override void SetSelectablesActive(bool active) {
            // if (!IsPopup()) {
                // if (isHoldersCollection) {
                //     for (int i =0 ; i< subHolders.Length; i++) {
                //         subHolders[i].SetSelectablesActive(active);
                //     }
                //     return;
                // }


                // for (int i = 0; i < allElements.Count; i++) {
                //     Button button = allElements[i].GetComponent<Button>();

                // Button lastButton = null;
                for (int i = 0; i < showElements.Count; i+=1) {
                    Button button = showElements[i].button;//.GetComponent<Button>();
                    if (button) {
                
                        Navigation customNav = button.navigation;
                        customNav.mode = active ? Navigation.Mode.Automatic : Navigation.Mode.None;
                        // customNav.mode = active ? Navigation.Mode.Horizontal : Navigation.Mode.None;
                        button.navigation = customNav;
                        
                    }
                }
                // for (int i = 0; i < allElements.Count; i+=1) {
                //     Button button = allElements[i].button;//.GetComponent<Button>();
                //     if (button) {
                //         Navigation customNav = button.navigation;
                //         customNav.mode = active ? Navigation.Mode.Automatic : Navigation.Mode.None;
                //         button.navigation = customNav;
                //     }
                // }
            // }
        }

        // protected override void OnDisable () {
        //     base.OnDisable();

        //     // Debug.LogError("setting selected false");
        //     for (int i =0 ; i < allElements.Count; i++) allElements[i].selected = false;
        // }


        public UISelectable[] GetAllSelectableElements (int targetCount, bool updateLayout) {
            // if (isHoldersCollection) return null;

            int c = showElements.Count;
            
            if (c < targetCount) {

                // for (int i = 0 ; i < c; i++) {
                //     if (!shownElements[i].gameObject.activeSelf)
                //         shownElements[i].gameObject.SetActive(true);
                // }

                int cnt = targetCount - c;
                for (int i = 0 ; i < cnt; i++) {

                    // if (allElements.Count > 0) {
                    //     UISelectable s = allElements[0];
                    //     allElements.Remove(s);
                    //     showElements.Add(s);
                    //     s.gameObject.SetActive(true);

                    // }
                    // else {

                        showElements.Add(GetAvailableSelectable());

                        // AddNewElement("Adding new", updateLayout && i == (cnt - 1));
                    // }
                }

                // UpdateElementLayout();//true, true);// UpdateSelectableElementHolder();
            }
            else if (c > targetCount) {

                // for (int i =0 ; i < targetCount; i++) {
                //     if (!shownElements[i].gameObject.activeSelf)
                //         shownElements[i].gameObject.SetActive(true);
                // }

                for (int i = c - 1; i >= targetCount; i--){// targetCount; i < c; i++) {
                    UISelectable s = showElements[i];
                    s.gameObject.SetActive(false);
                    showElements.Remove(s);
                    // allElements.Add(s);


                    // if (shownElements[i].gameObject.activeSelf)
                    //     shownElements[i].gameObject.SetActive(false);

                    s.transform.SetParent(UIMainCanvas.instance.transform);

                    // shownElements.Remove(shownElements[i]);
                }

                // WiggleActive();

                // List<UISelectable> r = new List<UISelectable>();
                // for (int i = 0; i < targetCount; i++) {
                //     r.Add(shownElements[i]);
                // }


                // UpdateElementLayout();//true, true);// UpdateSelectableElementHolder();

                // return shownElements.ToArray();// 
                // return r.ToArray();
            }

            UpdateElementLayout();

            // SetSelectablesActive(true);

            // WiggleActive();
            return showElements.ToArray();
        }

        

        

        #if UNITY_EDITOR
        public void RemoveAllElements () {
            UISelectable[] allSelectables = GetComponentsInChildren<UISelectable>(true);

            for (int i = 0; i < allSelectables.Length; i++) {
                // if (allSelectables[i] != null)
                    DestroyImmediate(allSelectables[i].gameObject);
            }
            showElements.Clear();
            // allElements.Clear();
            UpdateElementLayout();//true, true);
        }
        #endif

    }
}