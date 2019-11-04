using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Video;
using UnityEngine.UI;
using System;

using UnityTools;
namespace MinimalUI {

    
    
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(AudioSource))]
    public class UIVideoPlayer : UIComponent
    {

        public override void UpdateElementLayout () {
            // update anchor and recttransform based on video player clip
        }

        void OnLoopPointReached(VideoPlayer v) {
            if (!shouldLoop) {
                Stop();
                // if (onClipEnd != null) {
                //     onClipEnd();
                //     onClipEnd = null;
                // }
            }
        }

        UIVideoPlayerModule videoPlayer;

        Action onClipEnd;

        void Awake () {
            videoPlayer = new UIVideoPlayerModule(gameObject);
            videoPlayer.videoPlayer.loopPointReached += OnLoopPointReached;
        }

        void OnDisable () {
            videoPlayer.OnDisable ();
        }
        public override void OnComponentEnable () { }
        public override void OnComponentDisable () { }

        UIVideoPlayerSkipControls _skipControls;
        UIVideoPlayerSkipControls skipControls { get { return UIManager.instance.gameObject.GetComponentInChildrenIfNull<UIVideoPlayerSkipControls>(ref _skipControls, true); } }

        bool shouldLoop;
        bool skipControlsOn;

        float fadeOut;
        public void PlayClip (VideoClip clip, bool loop, float fadeIn, float fadeOut, Action onClipEnd, bool skippable) {

            shouldLoop = loop;
            this.fadeOut = fadeOut;

            if (onClipEnd != null && loop) {
                Debug.LogWarning("Cant use OnClipEnd callback when looping...");
                this.onClipEnd = null;
            }
            else this.onClipEnd = onClipEnd;

            videoPlayer.Play(clip);

            UIManager.ShowUIComponent(this, fadeIn, 0, 0);

            if (!skipControlsOn && skippable) {
                UIManager.ShowUIComponent(skipControls, 0, 0, 0);
                skipControlsOn = true;
            }
            if (skipControlsOn && !skippable) DisableSkipControls();   
        }

        void DisableSkipControls () {
            UIManager.HideUIComponent(skipControls, 0);
            skipControlsOn = false;
        }

        public void Pause () {
            videoPlayer.Pause();
        }
        public void Resume () {
            videoPlayer.Resume();
        }

        public void Stop () {
            Stop(fadeOut);
        }

        public void Stop (float fadeOut) {
            videoPlayer.Stop();
            UIManager.HideUIComponent(this, fadeOut);
            if (skipControlsOn) DisableSkipControls();

            if (onClipEnd != null) {
                onClipEnd();
                onClipEnd = null;
            }
        }
    }
}