﻿// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Audio {
    [RequireComponent(typeof(AudioSource))]
    class PooledAudioSource : MonoBehaviour {

        AudioSource _source;
        public AudioSource source { get { return gameObject.GetOrAddComponent<AudioSource>(ref _source, true); } }

        void Update () {
            source.playOnAwake = false;
            if (!source.isPlaying) {
                transform.SetParent(null);
                gameObject.SetActive(false);
            }
        }
    }

    public class AudioTools {

        static ComponentPool<PooledAudioSource> pool = new ComponentPool<PooledAudioSource>();

        public static AudioSource PlayClip (ExtendedAudioClip clip, float fadeDuration, Vector3 position) {
            AudioSource source = pool.GetAvailable().source;
            source.transform.position = position;
            if (source.Play(clip, fadeDuration)) {
                return source;
            }
            return null;
        }
        public static AudioSource PlayClip (ExtendedAudioClip clip, float fadeDuration, Transform parent, Vector3 localPosition) {
            AudioSource source = pool.GetAvailable().source;
            source.transform.SetParent(parent, localPosition);
            if (source.Play(clip, fadeDuration)) {
                return source;
            }
            return null;
        }
    }
}
