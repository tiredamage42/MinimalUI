using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools;
namespace MinimalUI {

    [ExecuteInEditMode] public class UICrosshairManager : UIComponent
    {
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }

        // public override void OnComponentClose() {
        //     // base.OnComponentClose();

        //     for (int i =0 ; i < allCrosshairs.Length; i++)
        //         allCrosshairs[i].OnComponentClose();
        // }
        // static UICrosshairManager _instance;
        // public static UICrosshairManager instance { get { return Singleton.GetInstance<UICrosshairManager>(ref _instance, true); } }

        UICrosshair[] allCrosshairs;
        public float spread = 1;


        void Awake () {
            if (Application.isPlaying) {
                allCrosshairs = gameObject.GetComponentsInChildren<UICrosshair>(true);
                EnableCrosshair(0);
            }
        }

        // protected override void OnEnable () {
        //     if (!Application.isPlaying) allCrosshairs = GetComponentsInChildren<UICrosshair>();
        //     base.OnEnable();

        //     // if (Application.isPlaying) {
        //     //     EnableCrosshair(0);
        //     // }
        // }

        public void EnableCrosshair (int index) {
            for (int i = 0; i < allCrosshairs.Length; i++) {
                bool active = i == index;
                if (allCrosshairs[i].gameObject.activeSelf != active)
                    allCrosshairs[i].gameObject.SetActive(active);
                
            }
        }

        public override void UpdateElementLayout(){//bool firstBuild, bool needsSize) {
            if (!Application.isPlaying)
                allCrosshairs = GetComponentsInChildren<UICrosshair>();
            
            for (int i = 0; i < allCrosshairs.Length; i++) {
                if (allCrosshairs[i].gameObject.activeSelf)
                    allCrosshairs[i].UpdateElementLayout();//firstBuild, needsSize);
            }
            // return Vector2.zero;
        }

        // protected override 
        void Update() {
            // base.Update();
            
            if (!Application.isPlaying)
                allCrosshairs = GetComponentsInChildren<UICrosshair>();
            

            if (!isActive)
                return;

            for (int i = 0; i < allCrosshairs.Length; i++) {
                if (allCrosshairs[i].gameObject.activeSelf)
                    allCrosshairs[i].UpdateCrosshair(spread);
            }
        }

    }
}
