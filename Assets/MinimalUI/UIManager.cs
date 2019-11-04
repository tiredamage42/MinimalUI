/*
    TODO:

        
        crosshair

*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using UnityTools;
using UnityTools.GameSettingsSystem;
using UnityEngine.EventSystems;

using UnityEngine.Video;
using MinimalUI;

/*
    outside namepsace so we dont have to include it just for the enum...
*/
public enum UIColorScheme { Normal = 0, Warning = 1, Invalid = 2, Black = 3, White = 4, };

namespace MinimalUI {
    public class UIManager : Singleton<UIManager>
    {
        public static T GetUIComponentByName<T> (string name) where T : UIComponent {
            T[] all = instance.GetComponentsInChildren<T>(true);
            for (int i = 0; i < all.Length; i++) {
                if (all[i].gameObject.name == name) {
                    return all[i];
                }
            }
            Debug.LogError("Couldnt find UIComponent by name: " + name);
            return null;
        }
        public static T GetUIComponentByName<T> (string name, ref T variable) where T : UIComponent{
            if (variable == null) variable = GetUIComponentByName<T>(name);
            return variable;
        }

        public static Color32 currentUIColor { get { return new Color32((byte)uiSettings.redValue, (byte)uiSettings.greenValue, (byte)uiSettings.blueValue, 255); } }
        public static Color32 DarkenColor (Color32 original) {
            return new Color32(
                (byte)(original.r * settings.rgbDarkenMultiplier.x),
                (byte)(original.g * settings.rgbDarkenMultiplier.y),
                (byte)(original.b * settings.rgbDarkenMultiplier.z),
                (byte)settings.darkenAlpha
            );
        }
        [System.Serializable] public class UIOptions {
            public bool showCrosshair = true, showCompass = true, showSubtitles, showMiniMap = true;
            public int redValue, greenValue, blueValue;
            public void InitializeDefaultColor () {
                redValue = settings.mainLightColor.r;
                greenValue = settings.mainLightColor.g;
                blueValue = settings.mainLightColor.b;
                if (onUIColorChange != null) 
                    onUIColorChange();
            }
        }

        public static void SetUIColorValue (int colorIndex, int value) {
            if (colorIndex == 0) uiSettings.redValue = value;
            if (colorIndex == 1) uiSettings.greenValue = value;
            if (colorIndex == 2) uiSettings.blueValue = value;
            
            if (onUIColorChange != null && (colorIndex == 0 || colorIndex == 1 || colorIndex == 2)) {
                onUIColorChange();
            }
        }

        public static event Action onUIColorChange;

        const string UIManagerSettingsSaveKey = "UIManager.Settings";
        public static UIOptions uiSettings = new UIOptions();
        static void OnSettingsOptionsLoaded () {
            if (SaveLoad.settingsSaveState.SaveStateContainsKey(UIManagerSettingsSaveKey)) {
                uiSettings = (UIOptions)SaveLoad.settingsSaveState.LoadSaveStateObject(UIManagerSettingsSaveKey);
                if (onUIColorChange != null) onUIColorChange();
            }
        }
        static void OnSaveSettingsOptions () {
            SaveLoad.settingsSaveState.UpdateSaveState(UIManagerSettingsSaveKey, uiSettings);
        }


        // void UpdateUIElementShowBySettings (GameObject uiElement, bool settingsShow, bool isInMainMenuScene) {
        //     bool shouldBeActive = settingsShow;// && !isInMainMenuScene;
        //     if (uiElement.activeSelf != shouldBeActive) {
        //         uiElement.SetActive(shouldBeActive);
        //     }
        // }
        void UpdateUIElementShowBySettings (UIComponent uiElement, bool settingsShow) {
            if (uiElement.canvas == null) {
                Debug.LogWarning(uiElement.name + "Doesnt have it's own canvas to enable disable");
                return;
            }
            if (uiElement.canvas.enabled != settingsShow) {
                // uiElement.canvas.enabled = settingsShow;

                if (settingsShow)
                    ShowUIComponent(uiElement, 0, 0, 0);
                else
                    HideUIComponent(uiElement, 0);
                
            }
        }

        static UIMiniMap _miniMap;
        static UIMiniMap miniMap { get { return instance.gameObject.GetComponentInChildrenIfNull<UIMiniMap>(ref _miniMap, true); } }
        static UICompass _compass;
        static UICompass compass { get { return instance.gameObject.GetComponentInChildrenIfNull<UICompass>(ref _compass, true); } }
        

        void Update () {
            UpdateUIElementShowBySettings(crosshair, uiSettings.showCrosshair);
            UpdateUIElementShowBySettings(compass, uiSettings.showCompass);
            UpdateUIElementShowBySettings(miniMap, uiSettings.showMiniMap);
            UpdateUIElementShowBySettings(subtitles, uiSettings.showSubtitles);
            


            // bool isInMainMenuScene = false;// GameManager.isInMainMenuScene;

            // if (UICrosshairManager.instance != null) 
            // UpdateUIElementShowBySettings(crosshair.baseObject, uiSettings.showCrosshair, isInMainMenuScene);
            // // if (UICompass.instance != null) 
            // UpdateUIElementShowBySettings(compass.baseObject, uiSettings.showCompass, isInMainMenuScene);
            // // if (UIMiniMap.instance != null) 
            // UpdateUIElementShowBySettings(miniMap.baseObject, uiSettings.showMiniMap, isInMainMenuScene);
            
            // UpdateUIElementShowBySettings(subtitles.baseObject, uiSettings.showSubtitles, isInMainMenuScene);
            
            // if (UISubtitles.instance != null) {
            // }
            // if ((isInMainMenuScene || !uiSettings.showSubtitles) && UISubtitles.instance.baseObject.activeSelf) {
            //     UISubtitles.instance.baseObject.SetActive(false);
            // }
        }

        // should only happen on main menu scene

        
        static UICanvasGroup _hudGroup;
        static UICanvasGroup hudGroup { get { return GetUIComponentByName<UICanvasGroup>("HUD", ref _hudGroup); } }



        static UIComponent[] GetTransformComponents (Transform t) {
            int id = t.GetInstanceID();
            UIComponent[] components;
            if (!uiComponentsPerTransform.TryGetValue(id, out components)) {
                components = t.gameObject.GetComponents<UIComponent>(true);
                uiComponentsPerTransform[id] = components;
            }
            return components;
        }
        static Canvas GetCanvasComponent (Transform t) {
            int id = t.GetInstanceID();
            Canvas canvas;
            if (!canvasPerTransform.TryGetValue(id, out canvas)) {
                canvas = t.gameObject.GetComponent<Canvas>(true);
                canvasPerTransform[id] = canvas;
            }
            return canvas;
        }


        static Dictionary<int, Canvas> canvasPerTransform = new Dictionary<int, Canvas>();
        public static bool TransformIsActiveInUICanvas (Transform t) {
            Canvas canvas = GetCanvasComponent(t);
            if (canvas != null && !canvas.enabled) return false;
            if (t.parent == null) return true;

            return TransformIsActiveInUICanvas(t.parent);
        }            
        static Dictionary<int, UIComponent[]> uiComponentsPerTransform = new Dictionary<int, UIComponent[]>();
        static void OnUIComponentOpen (Transform t) {

            Canvas canvas = GetCanvasComponent(t);
            if (canvas != null && !canvas.enabled) return;

            UIComponent[] components = GetTransformComponents(t);
            for (int i = 0; i < components.Length; i++) {
                components[i].OnComponentEnable();
                components[i].UpdateElementLayout();
            }
            for (int i = 0; i < t.childCount; i++) 
                OnUIComponentOpen(t.GetChild(i));
        }

        static void OnUIComponentClose (Transform t) {
            UIComponent[] components = GetTransformComponents(t);
            for (int i = 0; i < components.Length; i++) components[i].OnComponentDisable();
            for (int i = 0; i < t.childCount; i++) OnUIComponentClose(t.GetChild(i));
        }

        
        // static GameObject hudObject;
        protected override void Awake() {
            base.Awake();
            if (!thisInstanceErrored) {
                // Debug.LogError("hiding hud");
                HideUIComponent(hudGroup, 0);

                // hudObject = transform.GetChild(0).gameObject;
                // hudObject.SetActive(false);

                uiSettings.InitializeDefaultColor();
                InitializeUIManager();

                // // prebuiltMenus = GetComponentsInChildren<ManualMenu>();

                // // play intro video
                // // on intro video end, play main menu scene background video

                // mainMenu.OpenMenu(null);

                // DontDestroyOnLoad(mainMenu.gameObject);

                GameManager.onPlayerCreated += OnPlayerCreated;
                GameManager.onPlayerDestroyed += OnPlayerDestroyed;
            }
            // else {
            //     Destroy(mainMenu.gameObject);
            // }
        }

        static void OnEnterMainMenuScene () {
            HideUIComponent(hudGroup, 0);
            if (settings.mainMenuBackgroundVideo != null)
                PlayClip (settings.mainMenuBackgroundVideo, true, .1f, 0, null, false);
            ManualMenu.mainMenu.OpenMenu(null);
        }

        static void OnExitMainMenuScene () {
            ManualMenu.mainMenu.CloseMenu();
            StopVideoClips (0);
            ShowUIComponent(hudGroup, 0, 0, 0);
        }

        void Start () {
            if (!thisInstanceErrored) {
                // prebuiltMenus = GetComponentsInChildren<ManualMenu>();

                // play intro video
                // on intro video end, play main menu scene background video
                
                if (settings.introVideo != null) {
                    PlayClip (settings.introVideo, false, .1f, .1f, OnEnterMainMenuScene, true);
                }
                else {
                    OnEnterMainMenuScene();
                }


                // mainMenu.OpenMenu(null);
            }
        }


        void OnPlayerCreated () {
            OnExitMainMenuScene();
        }
        void OnPlayerDestroyed() {
            OnEnterMainMenuScene();
        }


        // public static void OnMenuOpen () {
        //     if (!GameManager.isInMainMenuScene) {
        //         hudObject.SetActive(false);
        //     }
        // }
        // public static void OnMenuClose () {
        //     if (!GameManager.isInMainMenuScene) {
        //         hudObject.SetActive(true);
        //     }
        // }

        static UISettings _settings;
        static UISettings settings {
            get {
                if (_settings == null) _settings = GameSettings.GetSettings<UISettings>();
                return _settings;
            }
        }

        static void InitializeUIManager () {
            UIMaps.InitializeMaps ();

            eventSystem.sendNavigationEvents = true;

            inputModule.gameObject.SetActive(false);

            SaveLoad.onSettingsOptionsLoaded += OnSettingsOptionsLoaded;
            SaveLoad.onSaveSettingsOptions += OnSaveSettingsOptions;
        }

        // public static void ShowSubtitles (string speaker, string subtitles) {
        //     if (uiSettings.showSubtitles && UISubtitles.instance != null)
        //         UISubtitles.instance.ShowSubtitles ( speaker, subtitles);
        // }

        // public static void ShowGameMessage (string message, bool immediate, UIColorScheme scheme) {
        //     if (UIMessageCenter.instance != null)
        //         UIMessageCenter.instance.ShowMessage ( message, immediate, scheme );
        // }


        // remove input module control in late update, so we dont use inputs
        // the same frame they're closing
        void LateUpdate () {
            if (uiInputActive && uiStack.Count == 0) {
                inputModule.gameObject.SetActive(false);
                UIInput.MarkAllSelectionAxesUnoccupied();
            }
        }   

        public static void SetSelectionDelayed(GameObject selection) {
            UpdateManager.instance.StartCoroutine(_SetSelection(selection));
        }

        static IEnumerator _SetSelection(GameObject selection) {
            yield return null;
            SetSelection(selection);
        }

        public static bool uiInputActive { get { return inputModule.gameObject.activeSelf; } }

        public static void SetSelection(GameObject selection) {

            // make sure selection events happen if we're already selected
            if (eventSystem.currentSelectedGameObject == selection)
                eventSystem.SetSelectedGameObject(null);

            eventSystem.SetSelectedGameObject(selection);
        }
        
        public static GameObject CurrentSelected () {
            return eventSystem.currentSelectedGameObject;
        }
        static UIInput _inputModule;
        public static UIInput inputModule {
            get {
                if (_inputModule == null) {
                    _inputModule = GameObjects.FindObjectOfType<UIInput>(true);
                    if (_inputModule == null) _inputModule = new GameObject("UIInput").AddComponent<UIInput>();
                }
                return _inputModule;
            }
        }
        static EventSystem _eventSystem;
        public static EventSystem eventSystem { get { return inputModule.gameObject.GetComponentIfNull<EventSystem>(ref _eventSystem, true); } }

        // public static bool popupOpen;

        // public static void HideUI (UIObject uiObject) {
        //     UIWithInput asInputTaker = uiObject as UIWithInput;
        //     if (asInputTaker != null) {
        //         if (shownUIsWithInput.Contains(asInputTaker)) {
        //             shownUIsWithInput.Remove(asInputTaker);
        //         }
        //     }    

        //     uiObject.OnComponentClose ();
        //     uiObject.baseObject.SetActive(false);
        // }


        static Dictionary<int, IEnumerator> fadeOutCoroutines = new Dictionary<int, IEnumerator>();
        static Dictionary<int, IEnumerator> fadeInCoroutines = new Dictionary<int, IEnumerator>();
        static Dictionary<int, IEnumerator> flashCoroutines = new Dictionary<int, IEnumerator>();

        public static void HideUIComponent (UIComponent uiObject, float fadeOutTime) {

            if (uiObject.canvas == null) {
                Debug.LogWarning(uiObject.name + " doesnt have a canvas component, cant fully disable");
                // return;
            }
            else{

                if (!uiObject.canvas.enabled) {
                    // Debug.Log(uiObject.name + " is already deisabled");
                    return;
                }
            }


                
            UIWithInput asInputTaker = uiObject as UIWithInput;
            if (asInputTaker != null) {

                RemoveUIFromStack(asInputTaker);
                // if (shownUIsWithInput.Contains(asInputTaker)) {
                //     shownUIsWithInput.Remove(asInputTaker);
                // }
            }    

            int id = uiObject.GetInstanceID();


            // if (fadeOutCoroutines.ContainsKey(id)) {
            //     if (fadeOutCoroutines[id] != null) {
            //         return;
            //     }
            // }
            if (CoroutineRunning(fadeOutCoroutines, id)) return;
            


            bool isActiveInHiearchy = TransformIsActiveInUICanvas(uiObject.baseObject.transform);
            FadeOutComponent(uiObject, fadeOutTime, isActiveInHiearchy);

            // uiObject.OnComponentClose ();
            // uiObject.baseObject.SetActive(false);
        }

        static void FadeOutComponent (UIComponent c, float time, bool isActiveInHiearchy) {
            // if (!c.baseObject.activeSelf) return;

            // if (!TransformIsActiveInUICanvas(c.transform))
            //     return;
            
            int id = c.GetInstanceID();


            // if (fadeOutCoroutines.ContainsKey(id)) {
            //     if (fadeOutCoroutines[id] != null) {
            //         return;
            //     }
            // }

            


            StopCoroutinesIfRunning(flashCoroutines, id);
            StopCoroutinesIfRunning(fadeInCoroutines, id);
            
            
            // if (fadeInCoroutine != null) {
            //     StopCoroutine(fadeInCoroutine);
            //     fadeInCoroutine = null;
            // }
            // if (flashCoroutine != null) {
            //     StopCoroutine(flashCoroutine);
            //     flashCoroutine = null;
            // }


            bool doCoroutine = isActiveInHiearchy && time > 0;

            if (c.canvasGroup == null && doCoroutine) {
                Debug.LogWarning("Cant Fade UIComponent " + c.name + ", no Canvas Group Found");
                doCoroutine = false;
            }

            if (doCoroutine) {

                fadeOutCoroutines[id] = FadeOutCoroutine(c, id, time, isActiveInHiearchy);
                instance.StartCoroutine(fadeOutCoroutines[id]);
            }
            else {

            
                if (isActiveInHiearchy)
                    OnUIComponentClose(c.baseObject.transform);

                if (c.canvas != null) {
                    c.canvas.enabled = false;
                    // Debug.Log(c.name + " DISABLING");
                }

                if (c.canvasGroup != null)
                    c.canvasGroup.alpha = 0;



            }
            

            // if (canvasGroup != null) {
            //     // fadeOutCoroutine = FadeOutCoroutine(time);
            //     // StartCoroutine(fadeOutCoroutine);
            // }
            // else {
            //     CloseObject();
            // }
        }
        static IEnumerator FadeOutCoroutine (UIComponent c, int id, float time, bool isActiveInHiearchy) {
            CanvasGroup canvasGroup = c.canvasGroup;

            // if (isActiveInHiearchy) {
                // if (canvasGroup == null && time > 0) 
                //     Debug.LogWarning("Cant Fade UIComponent " + c.name + ", no Canvas Group Found");
                
                // if (canvasGroup != null)
                    yield return instance.StartCoroutine( FadeCoroutine ( c, time, 0 ) );
            // }
            

            // if (canvasGroup != null) {
            //     if (time > 0) {
            //         canvasGroup.alpha = 1;
            //         float t = 0;
            //         while (t < time) {
            //             yield return null;
            //             t += Time.unscaledDeltaTime;
            //             if (t > time) t = time;
            //             canvasGroup.alpha = 1 - (t / time);
            //         }
            //     }
            //     canvasGroup.alpha = 0;
            // }

            fadeOutCoroutines[id] = null;
            // fadeOutCoroutine = null;
            // CloseObject();


            if (isActiveInHiearchy)
                OnUIComponentClose(c.baseObject.transform);


            if (c.canvas != null) {
                c.canvas.enabled = false;
            }


            // else {
            //     Debug.LogWarning(c.name + " doesnt have a canvas component, cant fully disable");
            // }
            
            // c.OnComponentClose ();
            // c.baseObject.SetActive(false);

            // gameObject.SetActive(false);
        }


        public static void FlashUIComponent (UIComponent c, int flashRounds, float flashFrequency) {
            
            if (!TransformIsActiveInUICanvas(c.transform))
                return;

            // if (!c.baseObject.activeSelf)
            //     return;


            if (c.canvasGroup == null) {
                Debug.LogWarning("Cant Flash UIComponent " + c.name + ", no Canvas Group Found");
                return;
            }




            
                
            int id = c.GetInstanceID();

            if (CoroutineRunning(flashCoroutines, id)) return;
            if (CoroutineRunning(fadeOutCoroutines, id)) return;
            
            StopCoroutinesIfRunning(fadeInCoroutines, id);
                
            // if (fadeOutCoroutine != null) {
            //     StopCoroutine(fadeOutCoroutine);
            //     fadeOutCoroutine = null;
            // }
            // if (fadeInCoroutine != null) {
            //     StopCoroutine(fadeInCoroutine);
            //     fadeInCoroutine = null;
            // }
            // flashCoroutine = FlashCoroutine(flashRounds, flashFrequency);
            // StartCoroutine(flashCoroutine);
   
            flashCoroutines[id] = FlashCoroutine(c, id, flashRounds, flashFrequency);
            instance.StartCoroutine(flashCoroutines[id]);
        }

        static IEnumerator FlashCoroutine (UIComponent c, int id, int flashRounds, float flashFrequency) {
            c.canvasGroup.alpha = 1;
            for (int i = 0; i < flashRounds; i++) {
                yield return new WaitForSecondsRealtime(flashFrequency);
                c.canvasGroup.alpha = 1 - c.canvasGroup.alpha;
            }
            c.canvasGroup.alpha = 1;
            flashCoroutines[id] = null;



            // flashCoroutine = null;
        }

        // public static void SetAllActiveUIsSelectableActive(bool active) {
        //     foreach (var e in shownUIsWithInput) {
        //         e.SetSelectablesActive(active);
        //     }
        // }

        // public static bool AnyUIOpen(out string name) {
        //     name = "";
        //     if (shownUIsWithInput.Count > 0) {
        //         foreach (var ui in shownUIsWithInput) {
        //             name = ui.name;
        //             break;
        //         }
        //     }
        //     return uiInputActive;
        // }

        static List<UIWithInput> uiStack = new List<UIWithInput>();

        static void AddUIToStack (UIWithInput ui) {
            

            if (uiStack.Count == 0) {
                UIInput.MarkAllSelectionAxesOccupied();
                inputModule.gameObject.SetActive(true);
            }
            else {
                if (uiStack.Contains(ui)) {
                    Debug.LogError("Adding duplicate ui: " + ui.name);
                }
                for (int i = 0; i < uiStack.Count; i++) {

                    uiStack[i].SetSelectablesActive(false);
                    if (i == uiStack.Count - 1) 
                        UIInput.MarkActionsUnoccupied(uiStack[i].GetUsedActions());
                    
                }
            }

            UIInput.MarkActionsOccupied(ui.GetUsedActions());
            ui.SetSelectablesActive(true);
            uiStack.Add(ui);
        }

        public static bool UIIsLastInStack (UIWithInput ui) {
            return uiStack.Count > 0 && ui == uiStack[uiStack.Count - 1];
        }
        
        static void RemoveUIFromStack (UIWithInput ui) {
            bool wasTop = UIIsLastInStack(ui);
            
            uiStack.Remove(ui);
            if (wasTop) {
                UIInput.MarkActionsUnoccupied(ui.GetUsedActions());

                if (uiStack.Count > 0) {
                
                    UIWithInput newTop = uiStack[uiStack.Count - 1];
                    UIInput.MarkActionsOccupied(newTop.GetUsedActions());
                    newTop.SetSelectablesActive(true);
                }
            }
        }

        // public static HashSet<UIWithInput> shownUIsWithInput = new HashSet<UIWithInput>();

        public static void ShowUIComponent (UIComponent uiObject, float fadeInTime, float duration, float fadeOutTime) {
            
            if (uiObject.canvas == null) {
                Debug.LogWarning(uiObject.name + " doesnt have a canvas component, cant fully enable");
                // return;
            }
            else {

                

                int id = uiObject.GetInstanceID();
                if (StopCoroutinesIfRunning(fadeOutCoroutines, id))
                    uiObject.canvas.enabled = false;
                
                if (uiObject.canvas.enabled) {
                    // Debug.LogWarning(uiObject.name + " is already open...");
                    return;
                }
                // Debug.Log(uiObject.name + " Setting canvas enebaled true");
                uiObject.canvas.enabled = true;
            }
            // if (uiObject.canvas != null)

            
            bool isActiveInHiearchy = TransformIsActiveInUICanvas(uiObject.baseObject.transform);

            if (!isActiveInHiearchy) {
                Debug.LogWarning(uiObject.name + " is not active in hiearchy...");
            }
            
            UIWithInput asInputTaker = uiObject as UIWithInput;
            
            if (isActiveInHiearchy) {
                if (asInputTaker != null) {

                    AddUIToStack(asInputTaker);
                    // inputModule.gameObject.SetActive(true);
                    // shownUIsWithInput.Add(asInputTaker);
                }
            }
            
            // uiObject.baseObject.SetActive(true);
            FadeInComponent (uiObject, fadeInTime, duration, fadeOutTime, isActiveInHiearchy);

            if (isActiveInHiearchy) {

                if (asInputTaker != null) {
                    asInputTaker.SetSelectablesActive(true);
                    if (onAnyUISelect != null) {
                        foreach (var d in onAnyUISelect.GetInvocationList()) {
                            asInputTaker.SubscribeToSelectEvent((Action<UISelectable, object[]>)d);
                        }
                    }
                    if (onAnyUISubmit != null) {
                        foreach (var d in onAnyUISubmit.GetInvocationList()) {
                            asInputTaker.SubscribeToActionEvent((Action<UISelectable, object[], Vector2Int>)d);
                        }
                    }
                }
            }
        }

        static bool StopCoroutinesIfRunning (Dictionary<int, IEnumerator> runningCoroutines, int id) {
            IEnumerator coroutine;
            if (runningCoroutines.TryGetValue(id, out coroutine)) {
                if (coroutine != null) {
                    instance.StopCoroutine(coroutine);
                    runningCoroutines[id] = null;
                    return true;
                }
            }
            return false;
        }
        static bool CoroutineRunning (Dictionary<int, IEnumerator> runningCoroutines, int id) {
            return runningCoroutines.ContainsKey(id) && runningCoroutines[id] != null;
        }
        

        static void FadeInComponent (UIComponent c, float time, float duration, float fadeOut, bool isActiveInHiearchy) {
            



            // if (TransformIsActiveInUICanvas(c.canvas.transform))
            //     return;

            // if (c.baseObject.activeSelf) return;

            // c.baseObject.SetActive(true);
            
            // if (c.canvas == null) {
            //     Debug.LogWarning(c.name + " doesnt have a canvas component, cant fully enable");
            //     return;
            // }
            
            // if (c.canvas.enabled)
            //     return;

            // if (c.canvas != null)
            //     c.canvas.enabled = true;


            if (isActiveInHiearchy)
                OnUIComponentOpen(c.baseObject.transform);

            int id = c.GetInstanceID();

            // StopCoroutinesIfRunning( id);
            
            // IEnumerator coroutine;
            // if (runningCoroutines.TryGetValue(id, out coroutine)) {
            //     if (coroutine != null)
            //         instance.StopCoroutine(coroutine);
            // }

            bool doCoroutine = isActiveInHiearchy && (time > 0 || duration > 0);
            
            // if (c.canvasGroup == null && doCoroutine) {
            //     Debug.LogWarning("Cant Fade UIComponent " + c.name + ", no Canvas Group Found");
            //     doCoroutine = false;
            // }

            if (doCoroutine) {
                fadeInCoroutines[id] = FadeInCoroutine(c, id, time, duration, fadeOut, isActiveInHiearchy);
                instance.StartCoroutine(fadeInCoroutines[id]);

            }
            else {
                if (c.canvasGroup != null)
                    c.canvasGroup.alpha = 1;

            }

        }

        // static void UpdateCanvas (CanvasGroup c, ref float t, float time, bool inverse) {
        //     t += Time.unscaledDeltaTime;
        //     if (t > time) t = time;
        //     float a = (t / time);
        //     c.alpha = inverse ? 1 - a : a;
        // }

        static IEnumerator FadeCoroutine (UIComponent c, float time, float target) {
            float t = 0;
            CanvasGroup canvasGroup = c.canvasGroup;

            if (time > 0) {
                float start = 1 - target;
                canvasGroup.alpha = start;
                while (t < time) {
                    yield return null;
                    t += Time.unscaledDeltaTime;
                    if (t > time) t = time;
                    float a = (t / time);
                    canvasGroup.alpha = a * target + (1-a) * start;
                }
            }
            canvasGroup.alpha = target;
        }
            

        static IEnumerator FadeInCoroutine (UIComponent c, int id, float time, float duration, float fadeOutTime, bool isActiveInHiearchy) {
            
            CanvasGroup canvasGroup = c.canvasGroup;

            // if (isActiveInHiearchy) {
                if (canvasGroup == null && (time > 0 || fadeOutTime > 0)) {
                    Debug.LogWarning("Cant Fade UIComponent " + c.name + ", no Canvas Group Found");
                }

                if (canvasGroup != null)
                    yield return instance.StartCoroutine( FadeCoroutine ( c, time, 1 ) );

                
            // }
            // else {
            //     if (canvasGroup != null)
            //         canvasGroup.alpha = 1;
            // }

            
            // float t = 0;
            // if (canvasGroup != null) {
            //     if (time > 0) {
            //         canvasGroup.alpha = 0;
            //         while (t < time) {
            //             yield return null;
            //             UpdateCanvas (canvasGroup, ref t, time, false);
            //         }
            //     }
            //     canvasGroup.alpha = 1;
            // }

            // bool disableAfterCoroutine = isActiveInHiearchy && duration > 0;
            bool disableAfterCoroutine = duration > 0;
            

            // if (isActiveInHiearchy) {

                if (duration > 0) {
                    yield return new WaitForSecondsRealtime(duration);

                    if (canvasGroup != null) {
                        yield return instance.StartCoroutine( FadeCoroutine ( c, fadeOutTime, 0 ) );
                    }
                    else {
                        canvasGroup.alpha = 0;
                    }

                    // if (canvasGroup != null) {

                    //     if (fadeOutTime > 0) {
                    //         t = 0;
                    //         canvasGroup.alpha = 1;
                    //         while (t < time) {
                    //             yield return null;
                    //             UpdateCanvas (canvasGroup, ref t, fadeOutTime, true);
                    //         }
                    //         canvasGroup.alpha = 0;
                    //     }
                    // }
                }
            // }

            // fadeInCoroutine = null;
            fadeInCoroutines[id] = null;

            if (disableAfterCoroutine) {
                HideUIComponent(c, 0);
            }
        }




        public static event Action<UISelectable, object[]> onAnyUISelect;
        public static event Action<UISelectable, object[], Vector2Int> onAnyUISubmit;


        #region VIDEOPLAYER
        static UIVideoPlayer _uiVideoPlayer;
        static UIVideoPlayer uiVideoPlayer { get { return instance.gameObject.GetComponentInChildrenIfNull<UIVideoPlayer>(ref _uiVideoPlayer, true); } }
        public static void PlayClip (VideoClip clip, bool loop, float fadeIn, float fadeOut, Action onClipEnd, bool skippable) {
            uiVideoPlayer.PlayClip (clip, loop, fadeIn, fadeOut, onClipEnd, skippable);
        }
        public static void StopVideoClips (float fadeOut) {
            uiVideoPlayer.Stop (fadeOut);
        }
        #endregion


        #region HUD

        public static void EnableHud (bool enabled, float fadeTime = .1f) {

            if (enabled)
                ShowUIComponent(hudGroup, fadeTime, 0, 0);
            else 
                HideUIComponent(hudGroup, fadeTime);
            // hudObject.SetActive(enabled);
            // if (enabled)
            //     hudObject.FadeIn(fadeTime, 0, 0);
            // else
            //     hudObject.FadeOut(fadeTime);
        }
        #endregion

        #region INTERACTABLE_PROMPT
        static ActionHintsPanel _interactablePrompt;
        static ActionHintsPanel interactablePrompt { get { return GetUIComponentByName<ActionHintsPanel>("InteractionHint", ref _interactablePrompt); } }
        
        public static void ShowInteractionPrompt (int promptIndex, string interactableName, List<int> actions, List<string> actionsNames, float fadeInTime = .1f) {
            ShowUIComponent(interactablePrompt, fadeInTime, 0, 0);
            interactablePrompt.textUI.SetText(interactableName);
            interactablePrompt.AddHintElements(actions, actionsNames);
        }    

        public static void HideInteractionPrompt (int promptIndex, float fadeOutTime = .1f) {
            HideUIComponent(interactablePrompt, fadeOutTime);
        }
        #endregion

        #region BACKGROUND_DARKEN
        static UIImage _darkenBackground;
        static UIImage darkenBackground { get { return GetUIComponentByName<UIImage>("DarkenBackground", ref _darkenBackground); } }
        public static void UIDarkenBackground (bool darken, float fadeTime = .1f) {
            if (darken)
                ShowUIComponent(darkenBackground, fadeTime, 0, 0);
                // darkenBackground.FadeIn(fadeTime, 0, 0);
            else
                HideUIComponent(darkenBackground, fadeTime);
                // darkenBackground.FadeOut(fadeTime);
        }
        #endregion



        #region TUTORIAL_PANEL
        static UITextPanel _tutorialPanel;
        static UITextPanel tutorialPanel { get { return GetUIComponentByName<UITextPanel>("TutorialPanel", ref _tutorialPanel); } }
        public static void ShowTutorialPopup (string message, string title = "Hint:", float fadeIn = .1f, float duration = 3, float fadeOut = .1f) {
            // tutorialPanel.mainText.SetText(message);
            // tutorialPanel.titleText.SetText(title);


            ShowUIComponent(tutorialPanel, fadeIn, duration, fadeOut);
            tutorialPanel.SetTexts(title, message, true);
            // tutorialPanel.FadeIn(fadeIn, duration, fadeOut);
        }
        #endregion

        #region QUEST_PANEL
        static UITextPanel _questPanel;
        static UITextPanel questPanel { get { return GetUIComponentByName<UITextPanel>("QuestPanel", ref _questPanel); } }
        public static void ShowQuestPopup (string message, string title, float fadeIn = .1f, float duration = 3, float fadeOut = .1f) {
            
            // questPanel.mainText.SetText(message);
            // questPanel.titleText.SetText(title);
            ShowUIComponent(questPanel, fadeIn, duration, fadeOut);
            questPanel.SetTexts(title, message, true);
            
            // questPanel.FadeIn(fadeIn, duration, fadeOut);
        }
        #endregion

        #region GAME_MESSAGES
        static UIMessageCenter _messages;
        static UIMessageCenter messages { get { return GetUIComponentByName<UIMessageCenter>("GameMessages", ref _messages); } }
        public static void ShowGameMessage (string message, bool immediate, UIColorScheme scheme, bool bulleted) {
            messages.ShowMessage ( message, immediate, scheme, bulleted );
        }
        #endregion

        #region OBJECTIVE_MESSAGES
        static UIMessageCenter _objectiveMessages;
        static UIMessageCenter objectiveMessages { get { return GetUIComponentByName<UIMessageCenter>("ObjectiveMessages", ref _objectiveMessages); } }
        public static void ShowObjectivesMessage (string message, bool immediate, UIColorScheme scheme, bool bulleted) {
            objectiveMessages.ShowMessage ( message, immediate, scheme, bulleted );
        }
        #endregion


        #region SUBTITLES
        static UISubtitles _subtitles;
        static UISubtitles subtitles { get { return instance.gameObject.GetComponentInChildrenIfNull<UISubtitles>(ref _subtitles, true); } }
        public static void ShowSubtitles (string speaker, string bark, float fadeIn, float duration, float fadeOut) {
            if (uiSettings.showSubtitles) subtitles.ShowSubtitles ( speaker, bark, fadeIn, duration, fadeOut );
        }
        #endregion

        #region CROSSHAIR
        static UICrosshairManager _crosshair;
        static UICrosshairManager crosshair { get { return instance.gameObject.GetComponentInChildrenIfNull<UICrosshairManager>(ref _crosshair, true); } }
        public static void EnableCrosshair (int index) {
            crosshair.EnableCrosshair(index);
        }
        #endregion

        #region SCOPE_OVERLAY
        static UIScopeOverlay _scopeOverlay;
        static UIScopeOverlay scopeOverlay { get { return instance.gameObject.GetComponentInChildrenIfNull<UIScopeOverlay>(ref _scopeOverlay, true); } }
        public static void ShowScopeOverlay (Sprite scopeSprite, float fadeInTime) {
            scopeOverlay.ShowScope(scopeSprite, fadeInTime);
        }
        public static void DisableScopeOverlay (float fadeOutTime) {
            // scopeOverlay.FadeOut(fadeOutTime);
            HideUIComponent(questPanel, fadeOutTime);
            
        }
        #endregion

        #region DIRT_OVERLAY
        static UIDirtOverlay _dirtOverlay;
        static UIDirtOverlay dirtOverlay { get { return instance.gameObject.GetComponentInChildrenIfNull<UIDirtOverlay>(ref _dirtOverlay, true); } }
        public static void ShowDirtOverlay (
            // float severity, 
            float inSpeed, float duration, float outSpeed, Color color) {
            dirtOverlay.ShowDirtOverlay(
                // severity, 
                inSpeed, duration, outSpeed, color);
        }
        #endregion

        #region DIRECTIONAL_INDICATOR
        static UIDirectionIndicator _directionIndicator;
        static UIDirectionIndicator directionIndicator { get { return instance.gameObject.GetComponentInChildrenIfNull<UIDirectionIndicator>(ref _directionIndicator, true); } }
        public static UIDirectionalIcon AddDirectionalIcon ( UIDirectionalIcon prefab, Func<Vector3> getWorldPosition ) {
            return directionIndicator.AddDirectionalIcon(prefab, getWorldPosition);
        }
        public static UIDirectionalIcon RemoveDirectionalIcon (UIDirectionalIcon icon) {
            return directionIndicator.RemoveDirectionalIcon(icon);
        }
        #endregion

        #region ICONTABLE
        static UIIconTable _iconTable;
        static UIIconTable iconTable { get { return instance.gameObject.GetComponentInChildrenIfNull<UIIconTable>(ref _iconTable, true); } }
        public static UIIconTableElement AddIconToIconTable (Sprite iconSprite) {
            return iconTable.AddIcon(iconSprite);
        }
        public static UIIconTableElement RemoveIconFromTable (UIIconTableElement icon) {
            return iconTable.RemoveIcon(icon);
        }
        #endregion

        #region POPUPS
        public static void ShowSelectionPopup(bool saveCurrentSelection, string msg, string[] options, Action<bool, int> returnValue) {
            UIPopups.ShowSelectionPopup(saveCurrentSelection, msg, options, returnValue);
        }
        public static void ShowConfirmationPopup(bool saveCurrentSelection, string msg, Action onConfirmation) {
            UIPopups.ShowConfirmationPopup( saveCurrentSelection, msg, onConfirmation);
        }
        public static void ShowIntSliderPopup(bool saveCurrentSelection, string title, int initialValue, int minValue, int maxValue, Action<bool, int> returnValue, Action<int> onValueChange) {
            UIPopups.ShowIntSliderPopup( saveCurrentSelection, title, initialValue, minValue, maxValue, returnValue, onValueChange);
        }
        #endregion
    }
}
