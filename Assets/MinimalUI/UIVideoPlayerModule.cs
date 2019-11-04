using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace MinimalUI {

    public class UIVideoPlayerModule
    {
        RawImage image;
        AudioSource audioSource;
        public VideoPlayer videoPlayer;

        public UIVideoPlayerModule (GameObject targetObject) {
            image = targetObject.GetComponent<RawImage>();

            audioSource = targetObject.GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.dopplerLevel = 0;
            audioSource.loop = false;
            audioSource.spatialBlend = 0;

            videoPlayer = targetObject.GetComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = true;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;            
        }

        RenderTexture renderTex;

        void ReleaseCurrentTexture () {
            if (renderTex != null) {
                RenderTexture.ReleaseTemporary(renderTex);
                image.texture = null;
            }
        }
        public void OnDisable () {
            ReleaseCurrentTexture();
        }
        void CheckTextureSize (VideoClip clip) {
            if (renderTex == null || ((ulong)renderTex.width != clip.width) || ((ulong)renderTex.height != clip.height)) {
                ReleaseCurrentTexture();
                renderTex = GetNewRenderTexture((int)clip.width, (int)clip.height);
                image.texture = renderTex;
                videoPlayer.targetTexture = renderTex;
            }
        }
        RenderTexture GetNewRenderTexture (int w, int h) {
            RenderTexture texture = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1, RenderTextureMemoryless.None, VRTextureUsage.None, false);
            texture.autoGenerateMips = false;
            texture.filterMode = FilterMode.Point;
            texture.useMipMap = false;
            return texture;
        }

        public void Pause () {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
        }
        public void Resume () {
            if (!videoPlayer.isPlaying)
                videoPlayer.Play();
        }

        public bool Play (VideoClip clip) {
            bool wasOff = true;
            if (videoPlayer.isPlaying) {
                videoPlayer.Stop();
                wasOff = false;
            }

            CheckTextureSize(clip);
            videoPlayer.clip = clip;
            videoPlayer.Play();

            return wasOff;
        }

        public void Stop () {
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();
        }

        public void GetCurrentTime (out string minutes, out string seconds) {
            GetTime(videoPlayer.time, out minutes, out seconds);
        }
        public void GetTotalTime (out string minutes, out string seconds) {
            GetTime(videoPlayer.clip.length, out minutes, out seconds);
        }
        void GetTime (double time, out string minutes, out string seconds) {
            minutes = Mathf.Floor ((int)time / 60).ToString ("00");
            seconds = ((int)time % 60).ToString ("00");
        }
        public double CalculatePlayedFraction()
        {
            return (double)videoPlayer.frame / (double)videoPlayer.clip.frameCount;
        }                
    }
}
