using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityTools;
using UnityTools.EditorTools;
using UnityEngine.UI;
using System.Linq;
namespace MinimalUI {

    [System.Serializable] public class NeatSpriteArray : NeatArrayWrapper<Sprite> { }
    public class UIDirtOverlay : UIComponent
    {
        [NeatArray] public NeatSpriteArray sprites;

        class Overlay {
            Image image;
            UICanvasGroup canvasGroup;
            public Overlay (Transform parent, Sprite sprite, int index) {

                GameObject newOverlay = new GameObject("Dirt Overlay " + index);
                RectTransform t = newOverlay.AddComponent<RectTransform>();
                t.SetParent(parent, Vector3.zero, Quaternion.identity, Vector3.one);
                t.pivot = new Vector2(.5f, .5f);
                t.anchorMin = new Vector2(0, 0);
                t.anchorMax = new Vector2(1, 1);

                canvasGroup = newOverlay.AddComponent<UICanvasGroup>();
                image = newOverlay.AddComponent<Image>();
                image.sprite = sprite;
                image.raycastTarget = false;
                image.color = Color.clear;
            }

            // float severity, inSpeed, duration, outSpeed;
            // float currentAlpha, timer;
            // public int phase;
            // public float alphaT { get { return currentAlpha / severity; } }

            // float startAlpha;
            // public float aT;

            public bool isActive { get { return canvasGroup.canvasGroup.alpha == 0; } }

            public void Show (
                // float severity, 
                float inSpeed, float duration, float outSpeed, Color color
            ) {
                
                
                UIManager.ShowUIComponent(canvasGroup, inSpeed, duration, outSpeed);
                image.color = color;//new Color(color.r, color.g, color.b, currentAlpha);

                // isActive = true;
                // phase = 0;
                // timer = 0;
                // aT = 0;

                // startAlpha = image.color.a;
                
                // this.severity = severity;
                // this.inSpeed = inSpeed;
                // this.duration = duration;
                // this.outSpeed = outSpeed;

                if (!image.enabled) image.enabled = true;

                // float currentAlpha = startAlpha;

                // image.color = new Color(color.r, color.g, color.b, currentAlpha);
            }

            // public bool isActive;

            // void UpdateCurrentAlpha (float target, float speed) {
            //     aT += speed;// Time.deltaTime / speed;

            //     float currentAlpha = Mathf.Lerp(startAlpha, target, aT);

            //     // currentAlpha = Mathf.Lerp(currentAlpha, target, speed);


            //     Color c = image.color;
            //     c.a = currentAlpha;
            //     image.color = c;
            // }
            
            // public void Update (float deltaTime) {
            //     if (!isActive) return;

            //     if (phase == 0) {

            //         // UpdateCurrentAlpha (severity, deltaTime * inSpeed);
            //         UpdateCurrentAlpha(severity, deltaTime / inSpeed);

            //         // if (currentAlpha >= severity) {
            //         if (aT >= 1) {
            //             phase++;
            //         }
            //     }
            //     else if (phase == 1) {
            //         timer += deltaTime;
            //         if (timer >= duration) {
            //             startAlpha = severity;
            //             phase++;
            //             aT = 0;
            //         }
            //     }
            //     else if (phase == 2) {


            //         UpdateCurrentAlpha(0, deltaTime / outSpeed);
            //         // UpdateCurrentAlpha (0, deltaTime * outSpeed);
                    
            //         if (aT <= 0) {
            //         // if (currentAlpha <= 0) {
            //             image.enabled = false;
            //             isActive = false;
            //         }
            //     }
            // }
        }

        List<Overlay> overlays = new List<Overlay>();
        List<int> indicies = new List<int>();


        Overlay GetAvailableOverlay () {
            indicies = indicies.OrderBy(i => Random.value).ToList();

            for (int i = 0; i < indicies.Count; i++) {
                int index = indicies[i];
                Overlay o = overlays[indicies[i]];
                if (!o.isActive) return o;
            }
            return overlays[indicies[0]];
            

            // Overlay farthestAlong = overlays[indicies[0]];

            // for (int i = 1; i < indicies.Count; i++) {
            //     Overlay o = overlays[indicies[i]];
            
            //     if (o.phase > farthestAlong.phase) {
            //         farthestAlong = o;
            //     }
            //     else if (o.phase == farthestAlong.phase) {
            //         // if (o.alphaT < farthestAlong.alphaT) {
            //         if (o.aT > farthestAlong.aT) {
            //             farthestAlong = o;
            //         }
            //     }
            // }
            // return farthestAlong;
        }

        public void ShowDirtOverlay (
            // float severity, 
            float inSpeed, float duration, float outSpeed, Color color) {
            GetAvailableOverlay().Show(
                // severity, 
                inSpeed, duration, outSpeed, color); 
        }

        // protected override 
        // void Update() {
        //     // base.Update();
        //     if (Application.isPlaying) {
        //         if (isActive) {

        //             for (int i =0 ; i < overlays.Count; i++) {
        //                 overlays[i].Update(Time.deltaTime);
        //             }
        //         }
        //     }
        // }

        void Awake () {
            if (Application.isPlaying) {
                for (int i = 0; i < sprites.Length; i++) {
                    overlays.Add(new Overlay(transform, sprites[i], i));
                    indicies.Add(i);
                }
            }
        }

        // protected override void OnEnable() {
        //     base.OnEnable();
        // }
        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            // return Vector2.zero;
        }
        // public override void OnComponentClose() {
        // }

        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();
        }
        

    }
}
