using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityTools;
using UnityTools.EditorTools;
using UnityEditor;
using System.Linq;
namespace MinimalUI {
    [System.Serializable] public class UIMessageCenterParameters : UIBaseParameters<UIMessageCenterParameters> {
        public UIMessageElementParameters messageParams;
        public bool upToDown = true;
        
        [Tooltip("negative values flip directions")]
        public float startXOffset = .1f;
        public float moveSpeed = 5;
        public float duration = 1;
        public float frequency = 1f;
        public Vector2 fadeInOut = new Vector2( .25f, .25f );

        public override void CopyFrom(UIMessageCenterParameters other) {
            UIBaseParameters.CopyParameters(ref messageParams, other.messageParams);

            upToDown = other.upToDown;
            startXOffset = other.startXOffset;
            moveSpeed = other.moveSpeed;
            duration = other.duration;
            frequency = other.frequency;
            fadeInOut = other.fadeInOut;
        }
    }
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIMessageCenterParameters))] class UIMessageCenterParametersDrawer : Internal.ParametersDrawer {
        protected override void DrawParams(Rect position, SerializedProperty property) {
            DrawProp(ref position, property, "messageParams", true);
            DrawProp(ref position, property, "upToDown", false);
            DrawProp(ref position, property, "startXOffset", false);
            DrawProp(ref position, property, "moveSpeed", false);
            DrawProp(ref position, property, "duration", false);
            DrawProp(ref position, property, "frequency", false);
            DrawProp(ref position, property, "fadeInOut", false);
        }
        
        static readonly string[] subClassNames = new string[] { "messageParams" };
        protected override string[] SubClassNames (SerializedProperty prop) { return subClassNames; }
        protected override int SinglePropsCount(SerializedProperty prop) { return 6; }
    }
    #endif

    [ExecuteInEditMode] public class UIMessageCenter : UIObject
    {
        // static UIMessageCenter _instance;
        // public static UIMessageCenter instance { get { return Singleton.GetInstance<UIMessageCenter> (ref _instance, true); } }

        Queue<string> messageQ = new Queue<string>();
        Queue<bool> bulletedQ = new Queue<bool>();
        Queue<UIColorScheme> schemesQ = new Queue<UIColorScheme>();
        static PrefabPool<UIMessageElement> elementPool = new PrefabPool<UIMessageElement>();

        List<UIMessageElement> shownElements = new List<UIMessageElement>();
        public UIMessageCenterParameters parameters;
        public event System.Action<string> onShowMessage;


        // public override void OnComponentClose() {
        //     base.OnComponentClose();

        //     for (int i = 0; i < shownElements.Count; i++)
        //         shownElements[i].OnComponentClose();
        
        // }
        public override void OnComponentEnable () {
            // base.OnComponentEnable();
            
        }
        public override void OnComponentDisable () {
            // base.OnComponentDisable();          
            
        }

        void Start () {
            if (Application.isPlaying) {
                StartCoroutine(HandleMessageShowing());
            }
        }

        IEnumerator HandleMessageShowing () {
            while (true) {
                if (messageQ.Count > 0) {
                    ShowMessageImmediate(messageQ.Dequeue(), schemesQ.Dequeue(), bulletedQ.Dequeue());
                }
                yield return new WaitForSeconds(parameters.frequency);
            }
        }

        public void ShowMessage (string message, bool immediate, UIColorScheme scheme, bool bulleted) {
            if (immediate || (messageQ.Count == 0 && shownElements.Count == 0)) {
                ShowMessageImmediate(message, scheme, bulleted);
            }
            else {
                messageQ.Enqueue(message);
                schemesQ.Enqueue(scheme);
                bulletedQ.Enqueue(bulleted);
            }
        }

        float yPivot { get { return rectTransform.pivot.y >= .5f ? 1 : 0; } }


        void UpdateMessageLayout (UIMessageElement message, string displayMessage, bool bulleted) {
            message.text.SetText(displayMessage);

            message.EnableFlair(bulleted);

            UIBaseParameters.CopyParameters(ref message.parameters, parameters.messageParams);

            message.SetPivotAndAnchor( new Vector2( rectTransform.pivot.x, yPivot )) ;
            message.UpdateElementLayout();//true);
        }

        public void ShowMessageImmediate (string message, UIColorScheme scheme, bool bulleted) {
            if (onShowMessage != null) {
                onShowMessage(message);
            }

            UIMessageElement newMessage = elementPool.GetAvailable(settings.messageElement, null, false, null);
            shownElements.Add(newMessage);


            if (newMessage.transform.parent != transform)
                newMessage.transform.SetParent(transform, Vector3.zero, Quaternion.identity, Vector3.one);
            

            
            UpdateMessageLayout(newMessage, message, bulleted);

            newMessage.ShowMessage ( parameters.duration, parameters.fadeInOut.x, parameters.fadeInOut.y, scheme );
            



            float pivotOffset = (1.0f - yPivot) * shownElements.Count * parameters.messageParams.messageHeight;
            // newMessage.rectTransform.anchoredPosition = new Vector2((rectTransform.anchorMin.x == 0 ? 0 : -newMessage.elementWidth) + parameters.startXOffset,  - pivotOffset);
            newMessage.rectTransform.anchoredPosition = new Vector2( parameters.startXOffset,  - pivotOffset);
            
            newMessage.gameObject.SetActive(true);
        }

        public void DisableAllMessages () {
            for (int i =0; i < shownElements.Count; i++) {
                shownElements[i].DisableMessage();
            }
            shownElements.Clear();
        }


        public override void UpdateElementLayout(){//bool firstBuild) {
            // return Vector2.zero;
        }

        // protected override 
        void Update()
        {
            // base.Update();

            if (!isActive)
                return;

            if (!Application.isPlaying) {
                shownElements = GetComponentsInChildren<UIMessageElement>().ToList();
                for (int i = 0; i < shownElements.Count; i++) {
                    UpdateMessageLayout(shownElements[i], "Message " + (i * 999) + " sdkjfksjhflSDFSFSFDfsdfsdfsfsfsdfsdfsfsf", Random.value < .5f);
                }
                // return;
            }

            float deltaTime = Time.unscaledDeltaTime;
            float speed = Application.isPlaying ? deltaTime * parameters.moveSpeed : 1;

            bool leftToRight = rectTransform.anchorMin.x == 0;
            int lastIndex = shownElements.Count - 1;

            float pivotOffset = (1.0f - yPivot) * (shownElements.Count * parameters.messageParams.messageHeight + parameters.messageParams.messageHeight);
            
            
            for (int i = shownElements.Count - 1; i >= 0; i--) {
            
                UIMessageElement e = shownElements[i];
                e.rectTransform.anchoredPosition = Vector2.Lerp(
                    e.rectTransform.anchoredPosition, 
                    new Vector2 (
                        0, 
                        // leftToRight ? 0 : -e.elementWidth, 
                        ((lastIndex - i) * (parameters.upToDown ? -e.elementHeight : e.elementHeight)) - pivotOffset
                    ), 
                    speed
                );
                
                if (Application.isPlaying) {
                    if (e.UpdateElement(deltaTime)) {
                        shownElements.Remove(e);
                    }
                }
            }
        }
    }
}