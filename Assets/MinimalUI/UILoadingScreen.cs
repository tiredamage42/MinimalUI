using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
namespace MinimalUI {
    public class UILoadingScreen : UIComponent
    {


        void Awake () {
            SceneLoading.onSceneLoadUpdate += OnSceneLoadUpdate;
            SceneLoading.prepareForSceneLoad += PrepareForSceneLoad;
            SceneLoading.endSceneLoad += EndSceneLoad;
            // EndSceneLoad();
            // UIManager.HideUIComponent(this, 0);
        }

        void OnDestroy () {
            SceneLoading.onSceneLoadUpdate -= OnSceneLoadUpdate;
            SceneLoading.prepareForSceneLoad -= PrepareForSceneLoad;
            SceneLoading.endSceneLoad -= EndSceneLoad;
        }
            

        void EndSceneLoad () {

            UIManager.HideUIComponent(this, 1);
            // gameObject.SetActive(false);
        }


        void PrepareForSceneLoad (string targetScene) {
            loadInfoPanel.SetTexts(string.Empty, "Loading:\n" + targetScene, false);
            
            UIManager.ShowUIComponent(this, 0, 0, 0);
            // gameObject.SetActive(true);
            
            // loadInfoPanel.mainText.SetText("Loading:\n" + targetScene);
            progress.SetValue(0, UIColorScheme.Normal);
        }
        void OnSceneLoadUpdate (float progress) {
            this.progress.SetValue(progress, UIColorScheme.Normal);
            int progressPercent = (int)(progress * 100);
            this.progress.text.SetText("%" + progressPercent);
        }


        // protected override 
        void Update() {
            // base.Update();
            if (!Application.isPlaying)
                return;

            if (!isActive)
                return;

            if (loadIcon != null) {
                loadIcon.transform.rotation = Quaternion.Euler(0, 0, Time.unscaledDeltaTime * loadIconSpeed);
            }
        }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }
        

        // public override void OnComponentClose() {
        //     for (int i = 0; i < textPanels.Length; i++) {
        //         textPanels[i].OnComponentClose();
        //     }
        //     progress.OnComponentClose();
        // }



        UITextPanel[] _textPanels;
        UITextPanel[] textPanels { get { return gameObject.GetComponentsInChildrenIfNull<UITextPanel>(ref _textPanels, true); } }
        UITextPanel hintsPanel { get { return textPanels[0]; } }
        UITextPanel loadInfoPanel { get { return textPanels[1]; } }
        public RectTransform loadIcon;
        public float loadIconSpeed = 2;
        UIValueTracker _tracker;
        UIValueTracker progress { get { return gameObject.GetComponentInChildrenIfNull<UIValueTracker>(ref _tracker, true); } }


        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // return Vector2.zero;

        }
    }
}
