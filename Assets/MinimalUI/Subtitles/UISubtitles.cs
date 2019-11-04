using UnityEngine;

using UnityTools;

using UnityEditor;
using UnityTools.EditorTools;
namespace MinimalUI {
    [System.Serializable] public class UISubtitlesParameters : UIBaseParameters<UISubtitlesParameters> {
        public float maxWidth = 2;
        public float speakerOffset = .1f;
        public UITextParameters speakerParameters;
        public UITextParameters barkParameters;
        // public float textDuration = 3;

        public override void CopyFrom(UISubtitlesParameters other) {
            maxWidth = other.maxWidth;
            speakerOffset = other.speakerOffset;
            // textDuration = other.textDuration;

            UIBaseParameters.CopyParameters(ref speakerParameters, other.speakerParameters);
            UIBaseParameters.CopyParameters(ref barkParameters, other.barkParameters);
        }
    }
    public class UISubtitles : UIObject
    {

        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     for (int i = 0; i < texts.Length; i++)
        //         texts[i].OnComponentClose();
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }



        // static UISubtitles _instance;
        // public static UISubtitles instance { get { return Singleton.GetInstance<UISubtitles>(ref _instance, true); } }

        public UISubtitlesParameters parameters;
        UIText[] _texts;
        UIText[] texts { get { return gameObject.GetComponentsInChildrenIfNull<UIText>(ref _texts, true); } }
        // float timer;
        bool showing { get { return gameObject.activeInHierarchy; } }

        // void Awake () {
            // if (Application.isPlaying) {
            //     if (SetUIComponentInstance<UISubtitles>(ref _instance, this)) {
            //         UIManager.HideUI(this);
            //     }
            // }
        // }

        // void Start () {

        //     if (Application.isPlaying) {
        //         if (_instance != null && _instance != this) {
        //             Debug.LogWarning("More than one subtitles instance in the scene, destroying instance named " + gameObject.name);
        //             Destroy(gameObject);
        //         }
        //         else {
        //             _instance = this;
        //             StopShowing();
        //         }
        //     }
        // } 


        UIText speaker { get { return texts[0]; } }
        UIText bark { get { return texts[1]; } }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {

            // need to set size delta first, so text wraps, and therefore
            // get height and get width are correct...
            bark.rectTransform.sizeDelta = new Vector2(parameters.maxWidth / parameters.barkParameters.scale, 0);
            bark.rectTransform.anchoredPosition = Vector2.zero;
            UIBaseParameters.CopyParameters(ref bark.parameters, parameters.barkParameters);

            // Vector2 barkSize = 
            bark.UpdateElementLayout();//firstBuild, true);

            float barkHeight = bark.getHeight;

            speaker.rectTransform.anchoredPosition = new Vector2(0, parameters.speakerOffset + barkHeight);
            UIBaseParameters.CopyParameters(ref speaker.parameters, parameters.speakerParameters);

            speaker.UpdateElementLayout();//firstBuild, false);
            // return Vector2.zero;
        }

        public void ShowSubtitles (string speaker, string bark, float fadeInTime, float duration, float fadeOutTime) {
            if (texts.Length >= 2) {
                this.speaker.SetText(speaker);
                this.bark.SetText(bark);
                UpdateElementLayout();//true, true);    
            }
            if (Application.isPlaying) {

                UIManager.ShowUIComponent(this, fadeInTime, duration, fadeOutTime);
                // if (!showing) 
                //     baseObject.SetActive(true);
                // timer = 0;
            }
        }

        // void StopShowing () {
        //     baseObject.SetActive(false);
        // }
        
        // protected override void Update()
        // {
        //     base.Update();
        //     if (showing) {
        //         if (Application.isPlaying) {
        //             timer += Time.deltaTime;
        //             if (timer >= parameters.textDuration) {
        //                 UIManager.HideUI(this);
        //                 // StopShowing();
        //                 timer = 0;
        //             }
        //         }
        //     }
        // }
    }
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UISubtitlesParameters))] class UISubtitlesParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "maxWidth", false);
            DrawProp(ref position, property, "speakerOffset", false);
            DrawProp(ref position, property, "speakerParameters", true);
            DrawProp(ref position, property, "barkParameters", true);
            // DrawProp(ref position, property, "textDuration", false);
        }
        static readonly string[] subClassNames = new string[] { "speakerParameters", "barkParameters" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 2; }
    }
    [CustomEditor(typeof(UISubtitles))] public class UISubtitlesEditor : Editor {
        static string speaker = "Speaker";
        static string bark = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.\nVestibulum malesuada congue est quis convallis.";
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GUITools.Space(2);
            EditorGUILayout.LabelField("Debug:", GUITools.boldLabel);
            speaker = EditorGUILayout.TextField("Speaker", speaker);
            bark = EditorGUILayout.TextArea(bark);
            if (GUILayout.Button("Start Debug")) {
                (target as UISubtitles).ShowSubtitles(speaker, bark, 0, 0, 0);
                EditorUtility.SetDirty(target);   
            }
        }
    }
    #endif
}
