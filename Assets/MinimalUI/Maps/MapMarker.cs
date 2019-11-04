using UnityEngine;
using UnityTools.EditorTools;
using UnityEditor;
using UnityTools;

namespace MinimalUI {

    
    [System.Serializable] public class MapMarkerParameters : UIBaseParameters<MapMarkerParameters> {
        public float iconSize = .125f;
        public UITextParameters textParams;

        public override void CopyFrom(MapMarkerParameters other) {
            iconSize = other.iconSize;
            UIBaseParameters.CopyParameters(ref textParams, other.textParams);
        }
    }


    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MapMarkerParameters))] class MapMarkerParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "iconSize", false);
            DrawProp(ref position, property, "textParams", true);
        }
        static readonly string[] subClassNames = new string[] { "textParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 1; }
    }
    #endif

    public class MapMarker : UIComponent
    {
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
        }


        public bool showRotation;
        public bool alwaysShowMiniMap;
        public float iconSizeMultiplier = 1;
        public MapMarkerParameters parameters;

        UIMapsObject registeredObject;

        public UIMapsObject uIRegisteredObject { get { return registeredObject; } }

        public void InitializeMapMarker (UIMapsObject registeredObject) {
            this.registeredObject = registeredObject;
            if (messageText != null)
                messageText.SetText(registeredObject.message);
        }

        public Vector3 GetMarkerPosition () {
            return registeredObject.worldPosition;
        }
        public Vector3 GetMarkerForward () {
            return registeredObject.worldForward;
        }

        // public override void OnComponentClose() {
        //     // base.OnComponentClose();
        //     // baseMapParameters.OnComponentClose();

        //     for (int i = 0; i < directionalComponents.Length; i++)
        //         directionalComponents[i].OnComponentClose();

        //     if (distanceText != null)
        //         distanceText.OnComponentClose();
            
        //     if (messageText != null)
        //         messageText.OnComponentClose();
            

        //     if (selectedIcon != null)
        //         selectedIcon.OnComponentClose();
            
        //     if (mainGraphic != null)
        //         mainGraphic.OnComponentClose();
        // }
        

        [Tooltip("Up/Down Icons")]
        public UIImage[] directionalComponents;
        public UIText distanceText, messageText;
        
        UIImage _selectedIcon;
        public UIImage selectedIcon {
            get {
                if (_selectedIcon == null && selectedParent != null) _selectedIcon = selectedParent.GetComponentInChildren<UIImage>();
                return _selectedIcon;
            }
        }

        public UIGraphic mainGraphic;
        public RectTransform selectedParent;


        public void ShowMessage(bool enabled) {
            if (messageText != null)
                messageText.gameObject.SetActive(enabled);
        }
        public void ShowDistance (bool enabled) {
            if (distanceText != null)
                distanceText.gameObject.SetActive(enabled);
        }
        public void ShowDirectionals (bool enabled) {
            for (int i = 0; i< directionalComponents.Length; i++) {
                directionalComponents[i].gameObject.SetActive(enabled);
            }
        }
        public void EnableMapMarker (bool enabled) {
            canvasGroup.alpha = enabled ? 1 : 0;
        }

        // CanvasGroup _canvasGroup;
        // CanvasGroup canvasGroup { get { return gameObject.GetComponentIfNull<CanvasGroup>(ref _canvasGroup, true); } }

        public override void UpdateElementLayout(){//bool firstBuild) {

            if (mainGraphic != null) {

                UIText asText = mainGraphic as UIText;
                if (asText != null) {

                    UIBaseParameters.CopyParameters(ref asText.parameters, parameters.textParams);
                    
                }
                else {
                    mainGraphic.rectTransform.sizeDelta = Vector2.one * parameters.iconSize * iconSizeMultiplier;
                }
                mainGraphic.UpdateElementLayout();//firstBuild);
            }

            bool directionalComponentsActive = directionalComponents.Length != 0 && directionalComponents[0].gameObject.activeSelf;
            bool waypointComponentActive = selectedParent != null && selectedParent.gameObject.activeSelf;


            if (directionalComponentsActive) {
                float iconsOffset = mainGraphic != null ? parameters.iconSize * .5f : 0;
                for (int i = 0; i < directionalComponents.Length; i++) {
                    directionalComponents[i].rectTransform.sizeDelta = Vector2.one * parameters.iconSize;
                    directionalComponents[i].rectTransform.anchoredPosition = new Vector2(0, iconsOffset * (i == 0 ? 1 : -1));
                    directionalComponents[i].UpdateElementLayout();//firstBuild);
                }
            }


            float selectMultiplier = mainGraphic != null ? 2 : 1.5f;

            if (directionalComponentsActive) {
                selectMultiplier += .5f;
            }


            if (waypointComponentActive) {
                selectedParent.sizeDelta = Vector2.one * parameters.iconSize * selectMultiplier;
                selectedIcon.UpdateElementLayout();//firstBuild);
            }
            

            
            float multiplier = mainGraphic != null ? 1f : 0.75f;
            
            if (waypointComponentActive) {
                multiplier += .5f;
            }

            

            if (directionalComponentsActive) {
                multiplier += .5f;
            }

            UpdateTextLayout(distanceText, multiplier);//, firstBuild);
            UpdateTextLayout(messageText, -multiplier);//, firstBuild);

            // return Vector2.zero;
            
        }

        void UpdateTextLayout (UIText text, float multiplier){//}, bool firstBuild) {
        
            if (text != null) {

                UIBaseParameters.CopyParameters(ref text.parameters, parameters.textParams);
                text.UpdateElementLayout();//firstBuild);
                text.rectTransform.anchoredPosition = new Vector2(0, parameters.iconSize * multiplier);
            }
        }


        public void UpdateMapMarker (Vector3 playerPosition) {
            // if (!Application.isPlaying)
            //     return;

            // if (!isActive)
            //     return;

            if (selectedParent != null) {
                if (registeredObject.isWaypoint != selectedParent.gameObject.activeSelf) {
                    selectedParent.gameObject.SetActive(registeredObject.isWaypoint);
                    UpdateElementLayout();//true);
                }
            }

            // Actor playerActor = GameManager.playerActor;
            // if (playerActor == null)
            //     return;

            // Vector3 playerPos = playerActor.GetPosition();
                
                
            bool directionalComponentsActive = directionalComponents != null && directionalComponents.Length == 2 && directionalComponents[0].gameObject.activeSelf;
            bool distanceTextActive = distanceText != null && distanceText.gameObject.activeSelf;
                

            if (directionalComponentsActive || distanceTextActive) {
                    
                Vector3 dir = registeredObject.worldPosition - playerPosition;

                if (distanceTextActive)
                    distanceText.SetText(((int)dir.magnitude).ToString());
                    
                if (directionalComponentsActive) {
                    directionalComponents[0].mainGraphic.enabled = dir.y > 5;
                    directionalComponents[1].mainGraphic.enabled = dir.y < -5;
                }
            }
        }


        // protected override 
        // void Update () {
        //     // base.Update();

        //     if (!Application.isPlaying)
        //         return;

            
        //     if (!isActive)
        //         return;

        //     if (selectedParent != null) {
        //         if (registeredObject.isWaypoint != selectedParent.gameObject.activeSelf) {
        //             selectedParent.gameObject.SetActive(registeredObject.isWaypoint);
        //             UpdateElementLayout();//true);
        //         }
        //     }

        //     Actor playerActor = GameManager.playerActor;
        //     if (playerActor == null)
        //         return;

        //     Vector3 playerPos = playerActor.GetPosition();
                
                
        //     bool directionalComponentsActive = directionalComponents != null && directionalComponents.Length == 2 && directionalComponents[0].gameObject.activeSelf;
        //     bool distanceTextActive = distanceText != null && distanceText.gameObject.activeSelf;
                

        //     if (directionalComponentsActive || distanceTextActive) {
                    
        //         Vector3 dir = registeredObject.worldPosition - playerPos;

        //         if (distanceTextActive)
        //             distanceText.SetText(((int)dir.magnitude).ToString());
                    
        //         if (directionalComponentsActive) {
        //             directionalComponents[0].mainGraphic.enabled = dir.y > 5;
        //             directionalComponents[1].mainGraphic.enabled = dir.y < -5;
        //         }
        //     }
                
        // }
    }
}
