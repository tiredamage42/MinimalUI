using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
using UnityTools.GameSettingsSystem;

using UnityEditor;

using UnityTools;


namespace MinimalUI {

    public class UIBaseParameters {

        public static void CopyParameters<T> (ref T a, T b) 
            where T : UIBaseParameters<T>, new() 
        {
            if (a == b) a = new T();
            a.CopyFrom(b);
        }
    }
    public abstract class UIBaseParameters<T> where T : UIBaseParameters<T> {
        public abstract void CopyFrom (T other);
    }


    public class _UpdateLayoutAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(_UpdateLayoutAttribute))]
    class UpdateLayoutAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (GUI.Button(position, "Update Layouts")) {
                UIComponent c = property.serializedObject.targetObject as UIComponent;
                c.UpdateElementLayout();//true, true);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }

    public abstract class UIComponent : MonoBehaviour//, ILayoutElement
    {

        // IEnumerator fadeInCoroutine, fadeOutCoroutine, flashCoroutine;

        // public void FadeIn (float time, float duration, float fadeOut) {
        //     if (gameObject.activeSelf) return;


        //     gameObject.SetActive(true);
        //     if (fadeOutCoroutine != null) {
        //         StopCoroutine(fadeOutCoroutine);
        //         fadeOutCoroutine = null;
        //     }
        //     if (flashCoroutine != null) {
        //         StopCoroutine(flashCoroutine);
        //         flashCoroutine = null;
        //     }
        //     fadeInCoroutine = FadeInCoroutine(time, duration, fadeOut);
        //     StartCoroutine(fadeInCoroutine);
        // }

        // public abstract void OnClose ();
        // public abstract void OnOpen ();

        // public void Flash (int flashRounds, float flashFrequency) {
        //     if (!gameObject.activeSelf)
        //         gameObject.SetActive(true);

        //     if (canvasGroup != null) {
        //         if (fadeOutCoroutine != null) {
        //             StopCoroutine(fadeOutCoroutine);
        //             fadeOutCoroutine = null;
        //         }
        //         if (fadeInCoroutine != null) {
        //             StopCoroutine(fadeInCoroutine);
        //             fadeInCoroutine = null;
        //         }
        //         flashCoroutine = FlashCoroutine(flashRounds, flashFrequency);
        //         StartCoroutine(flashCoroutine);
        //     }
        // }

        // IEnumerator FlashCoroutine (int flashRounds, float flashFrequency) {
        //     canvasGroup.alpha = 1;
        //     for (int i = 0; i < flashRounds; i++) {
        //         yield return new WaitForSecondsRealtime(flashFrequency);
        //         canvasGroup.alpha = 1 - canvasGroup.alpha;
        //     }
        //     canvasGroup.alpha = 1;
        //     flashCoroutine = null;
        // }

            

        // public void FadeOut (float time) {
        //     if (!gameObject.activeSelf) return;

        //     if (fadeInCoroutine != null) {
        //         StopCoroutine(fadeInCoroutine);
        //         fadeInCoroutine = null;
        //     }
        //     if (flashCoroutine != null) {
        //         StopCoroutine(flashCoroutine);
        //         flashCoroutine = null;
        //     }
            
        //     if (canvasGroup != null) {
        //         fadeOutCoroutine = FadeOutCoroutine(time);
        //         StartCoroutine(fadeOutCoroutine);
        //     }
        //     else {
        //         CloseObject();
        //     }
        // }

        // void CloseObject () {
        //     UIObject o = this as UIObject;
        //     if (o != null) {
        //         UIManager.HideUI(o);
        //     }
        //     else {
        //         gameObject.SetActive(false);
        //     }
        // }

        // IEnumerator FadeOutCoroutine (float time) {
        //     canvasGroup.alpha = 1;
        //     float t = 0;
        //     while (t < time) {
        //         yield return null;
        //         t += Time.unscaledDeltaTime;
        //         if (t > time) t = time;
        //         canvasGroup.alpha = 1 - (t / time);
        //     }
        //     canvasGroup.alpha = 0;
        //     fadeOutCoroutine = null;
        //     CloseObject();
        //     // gameObject.SetActive(false);
        // }

        // IEnumerator FadeInCoroutine (float time, float duration, float fadeOutTime) {
        //     canvasGroup.alpha = 0;
        //     float t = 0;
        //     while (t < time) {
        //         yield return null;
        //         t += Time.unscaledDeltaTime;
        //         if (t > time) t = time;
        //         canvasGroup.alpha = t / time;
        //     }
        //     canvasGroup.alpha = 1;

        //     if (duration > 0) {
        //         yield return new WaitForSecondsRealtime(duration);

        //         canvasGroup.alpha = 1;
        //         t = 0;
        //         while (t < time) {
        //             yield return null;
        //             t += Time.unscaledDeltaTime;
        //             if (t > time) t = time;
        //             canvasGroup.alpha = 1 - (t / time);
        //         }
        //         canvasGroup.alpha = 0;
        //     }
        //     fadeInCoroutine = null;
        // }

        CanvasGroup _canvasGroup;
        public CanvasGroup canvasGroup { get { return baseObject.GetComponentIfNull<CanvasGroup>(ref _canvasGroup, true); } }
        
        // public Canvas overrideCanvas;
        Canvas _canvas;
        public Canvas canvas { get { return baseObject.GetComponentIfNull<Canvas>(ref _canvas, true); } }
        public bool isActive { get { return !Application.isPlaying || UIManager.TransformIsActiveInUICanvas(transform); } }

        

        [Tooltip("Object to Enable / Disable")]
        public GameObject baseGameObject;
        public GameObject baseObject { get { return baseGameObject != null ? baseGameObject : gameObject; } }


        // protected bool isQuitting;

        // void OnApplicationQuit () {
        //     isQuitting = true;
        // }


        [_UpdateLayout] public bool _updateBool;

        protected virtual void OnEnable () { 
            UpdateElementLayout();//true, true); 
        }

        // public virtual void OnComponentEnable () {
        //     // UpdateElementLayout(true);
        // }
        // public virtual void OnComponentDisable () {

        // }

        public abstract void OnComponentEnable ();
        public abstract void OnComponentDisable ();

        // void SetDirty () {
        //     #if UNITY_EDITOR
        //     if (!Application.isPlaying) UnityEditor.EditorUtility.SetDirty(gameObject);
        //     #endif
        //     isDirty = true;
        // }

        // public bool isDirty;
        // protected virtual void Update () {
            // if (isDirty || !Application.isPlaying) {
            //     UpdateElementLayout();
            //     isDirty = false;
            // }
        // }
            

        // protected void SetDirtyRecursive () {
        //     UIComponent[] allChildren = GetComponentsInChildren<UIComponent>();
        //     for (int i = 0; i < allChildren.Length; i++) {
        //         if (allChildren[i] != this) {
        //             allChildren[i].SetDirty();// = true;
        //         }
        //     }
        // }
        public abstract void UpdateElementLayout();//bool firstBuild, bool needsSize);



        // protected void UpdateElementsLayoutWithChildren () {
        //     UIComponent[] selfComponents = GetComponents<UIComponent>();
        //     for (int i = 0; i < selfComponents.Length; i++) {
        //         selfComponents[i].UpdateElementLayout();
        //     }


        //     for (int i = 0; i < transform.childCount; i++) {
        //         Transform t = transform.GetChild(i);
        //         UIComponent c = t.GetComponent<UIComponent>();
        //         if (c != null) {
        //             c.UpdateElementsLayoutWithChildren();
        //         } 
        //     }


        //     // UIComponent[] allChildren = GetComponentsInChildren<UIComponent>();

        //     // for (int i = 0; i < allChildren.Length; i++) {
        //     //     allChildren[i].UpdateElementLayout();
        //     // }
        // }
        

        
        static UISettings _settings = null;
        protected static UISettings settings {
            get {
                if ((object)_settings == null) _settings = GameSettings.GetSettings<UISettings>();
                return _settings;
            }
        }
        RectTransform _rectTransform = null;
        public RectTransform rectTransform { get { return gameObject.GetComponentIfNull<RectTransform>(ref _rectTransform, true); } }

        public void SetPivotAndAnchor (Vector2 anchor) {
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
        }

        protected static Color32 GetUIColor (UIColorScheme schemeType, bool useDarker) {
            if (settings == null) 
                return Color.magenta;

            switch(schemeType) {
                // case UIColorScheme.Normal: return useDarker ? settings.mainDarkColor : settings.mainLightColor;
                // case UIColorScheme.Warning: return useDarker ? settings.warningDarkColor : settings.warningLightColor;
                // case UIColorScheme.Invalid: return useDarker ? settings.invalidDarkColor : settings.invalidLightColor;
                
                case UIColorScheme.Normal: return useDarker ? UIManager.DarkenColor(Application.isPlaying ? UIManager.currentUIColor : settings.mainLightColor) : Application.isPlaying ? UIManager.currentUIColor : settings.mainLightColor;
                case UIColorScheme.Warning: return useDarker ? UIManager.DarkenColor(settings.warningLightColor) : settings.warningLightColor;
                case UIColorScheme.Invalid: return useDarker ? UIManager.DarkenColor(settings.invalidLightColor) : settings.invalidLightColor;
                
                case UIColorScheme.Black : return settings.blackColor;
                case UIColorScheme.White : return settings.whiteColor;
            }
            return Color.magenta;
        }





        // protected static T GetUIComponentInstance<T> (ref T _variable) where T : MonoBehaviour {
        //     if (_variable == null) {
        //         _variable = GameObject.FindObjectOfType<T>();
        //         if (_variable == null) {
        //             Debug.LogError("Cant Find UIComponent instance of type: " + typeof(T).Name);
        //         }
        //     }
        //     return _variable;
        // }
        // protected static bool SetUIComponentInstance<T> (ref T _variable, T instance) where T : MonoBehaviour {

        //     if (_variable != null && _variable != instance) {
        //         Debug.LogWarning("Instance Of " + typeof(T).Name + " already exists, destroying: " + instance.gameObject.name);
        //         if (Application.isPlaying) {
        //             Destroy(instance.gameObject);
        //         }
        //         return false;
        //     }
        //     _variable = instance;
        //     return true;
        // }
    }
}
