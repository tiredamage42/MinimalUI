using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;

namespace MinimalUI {
    public class UIXPLevelBar : UIComponent
    {
        public float showDuration = 2;
        public float fadeInOut = .1f;
        public float flashFrequency = .5f;
        public int flashRounds = 4;

        void Awake () {
            gameValueTracker.onGameValueChange += OnGameValueChange;
            // UIManager.HideUIComponent(this, 0);
        }

        public void SetXPGameValue (GameValue gv) {
            gameValueTracker.SetGameValue(gv);
        }
        public void UntrackGameValue () {
            gameValueTracker.UntrackGameValue();
        }

        // float flashTime;
        void OnGameValueChange (float delta, float newValue, float min, float max) {
            // if (!gameObject.activeSelf) {
            //     FadeIn(fadeInOut, showDuration, fadeInOut);
            // }
            UIManager.ShowUIComponent(this, fadeInOut, showDuration, fadeInOut);

            // Debug.LogError("Showing XP");

            // flashTime = Time.time;
            text.SetText("+" + ((int)delta).ToString());

            // or rollover....
            if (newValue >= max) {
                if (!levelUpText.gameObject.activeSelf)
                    levelUpText.gameObject.SetActive(true);
                
                UIManager.FlashUIComponent(levelUpText, flashRounds, flashFrequency);
                // levelUpText.Flash(flashRounds, flashFrequency);

            }
            else {
                if (levelUpText.gameObject.activeSelf)
                    levelUpText.gameObject.SetActive(false);
                
            }
        }
        // protected override void Update () {
        //     base.Update();
        //     if (Application.isPlaying) {
        //         if (Time.time - flashTime >= showDuration) {
        //             FadeOut(fadeInOut);
        //         }
        //     }
        // }
        UIGameValueTracker _gameValueTracker;
        UIGameValueTracker gameValueTracker { get { return gameObject.GetComponentInChildrenIfNull<UIGameValueTracker>(ref _gameValueTracker, true); } }
        // UIText _text;
        UIText text { get { return texts[0]; } }
        UIText levelUpText { get { return texts[1]; } }
        UIText[] _texts;
        UIText[] texts { get { return gameObject.GetComponentsInChildrenIfNull<UIText>(ref _texts, true); } }
        
        // public override void OnComponentClose() {
            
        //     gameValueTracker.OnComponentClose();
        //     for (int i = 0; i < texts.Length; i++) {
        //         texts[i].OnComponentClose();
        //     }
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }
        

        
        public override void UpdateElementLayout(){//bool firstBuild) {
            // return Vector2.zero;
        }

    }
}
