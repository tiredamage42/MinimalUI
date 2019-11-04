

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Serialization;

using System.Text;



using UnityEngine.EventSystems;


using Object = UnityEngine.Object;
using UnityTools;
using UnityTools.GameSettingsSystem;
namespace MinimalUI
{
  
    [RequireComponent(typeof(EventSystem))]
    //   [AddComponentMenu("Event/Standalone Input Module")]
    public class UIInput : PointerInputModule
    {
        [SerializeField] private float m_InputActionsPerSecond = 10f;
        [SerializeField] private float m_RepeatDelay = 0.5f;
        // [SerializeField] private bool m_ForceModuleActive;
        
        private float m_PrevActionTime;
        private Vector2 m_LastMoveVector;
        private int m_ConsecutiveMoveCount;
        // private Vector2 m_LastMousePosition;
        // private Vector2 m_MousePosition;
        

        protected override void Awake () {
            base.Awake();
            DontDestroyOnLoad(gameObject);

        }
        


        

        public override void UpdateModule()
        {
        // this.m_LastMousePosition = this.m_MousePosition;
        // this.m_MousePosition = mousePosition;
        }

        public override bool IsModuleSupported()
        {
            return true;
        }

        public override bool ShouldActivateModule()
        {
            if (!base.ShouldActivateModule())
                return false;


            // return gameObject.activeSelf;

            if (!gameObject.activeSelf)
                return false;

            // return gameObject.activeSelf;
            bool flag = //this.m_ForceModuleActive | 
                GetActionDown(settings.submitAction, false) | GetActionDown(settings.cancelAction, false) //|
                // (double) (this.m_MousePosition - this.m_LastMousePosition).sqrMagnitude > 0.0 | 
                // GetMouseButtonDown(0)
            ;

            for (int i = 0; i < settings.verticalAxes.Length; i++) {
                flag |= !Mathf.Approximately(GetAxis(settings.verticalAxes[i]), 0.0f);
            }

            for (int i = 0; i < settings.horizontalAxes.Length; i++) {
                flag |= !Mathf.Approximately(GetAxis(settings.horizontalAxes[i]), 0.0f);
            }
        
            return flag;
        }

        public override void ActivateModule()
        {
        base.ActivateModule();
        // this.m_MousePosition = mousePosition;
        // this.m_LastMousePosition = mousePosition;
        GameObject selectedGameObject = this.eventSystem.currentSelectedGameObject;
        if ((UnityEngine.Object) selectedGameObject == (UnityEngine.Object) null)
            selectedGameObject = this.eventSystem.firstSelectedGameObject;
        this.eventSystem.SetSelectedGameObject(selectedGameObject, this.GetBaseEventData());
        }

        public override void DeactivateModule()
        {
        base.DeactivateModule();
        this.ClearSelection();
        }

        public override void Process()
        {
        bool selectedObject = false;// eventSystem.currentSelectedGameObject != null;// this.SendUpdateEventToSelectedObject();
        if (this.eventSystem.sendNavigationEvents)
        {
            if (!selectedObject)
                selectedObject |= this.SendMoveEventToSelectedObject();
            if (!selectedObject) this.SendSubmitEventToSelectedObject();
        }
        // this.ProcessMouseEvent();
        }
        /// <summary>
        ///   <para>Calculate and send a submit event to the current selected object.</para>
        /// </summary>
        /// <returns>
        ///   <para>If the submit event was used by the selected object.</para>
        /// </returns>
        protected bool SendSubmitEventToSelectedObject()
        {
        if ((UnityEngine.Object) this.eventSystem.currentSelectedGameObject == (UnityEngine.Object) null)
            return false;
        BaseEventData baseEventData = this.GetBaseEventData();
        if (GetActionDown(settings.submitAction, false)) ExecuteEvents.Execute<ISubmitHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
        if (GetActionDown(settings.cancelAction, false)) ExecuteEvents.Execute<ICancelHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
        return baseEventData.used;
        }

        public static float GetHorizontalAxis () {
            float f = 0;
            for (int i = 0; i < settings.horizontalAxes.Length; i++) f += GetAxis(settings.horizontalAxes[i]);
            f = Mathf.Clamp(f, -1, 1);
            return f;
        }
        public static float GetVerticalAxis () {
            float f = 0;
            for (int i = 0; i < settings.verticalAxes.Length; i++) f += GetAxis(settings.verticalAxes[i]);
            f = Mathf.Clamp(f, -1, 1);
            return f;
        }
        public static bool GetSubmitDown () {
            return GetActionDown(settings.submitAction, false);
        }


        private Vector2 GetRawMoveVector()
        {
        Vector2 zero = new Vector2(GetHorizontalAxis(), GetVerticalAxis());

            bool horizontalDown = false;
            for (int i = 0; i < settings.horizontalAxes.Length; i++) {
                horizontalDown = GetActionDown(settings.horizontalAxes[i], true);
                if (horizontalDown) break;
            }
            bool verticalDown = false;
            for (int i = 0; i < settings.verticalAxes.Length; i++) {
                verticalDown = GetActionDown(settings.verticalAxes[i], true);
                if (verticalDown) break;
            }

            

        if (horizontalDown)
        {
            if ((double) zero.x < 0.0)
            zero.x = -1f;
            if ((double) zero.x > 0.0)
            zero.x = 1f;
        }
        if (verticalDown)
        {
            if ((double) zero.y < 0.0)
            zero.y = -1f;
            if ((double) zero.y > 0.0)
            zero.y = 1f;
        }
        return zero;
        }

        /// <summary>
        ///   <para>Calculate and send a move event to the current selected object.</para>
        /// </summary>
        /// <returns>
        ///   <para>If the move event was used by the selected object.</para>
        /// </returns>
        protected bool SendMoveEventToSelectedObject()
        {
        float unscaledTime = Time.unscaledTime;
        Vector2 rawMoveVector = this.GetRawMoveVector();
        if (Mathf.Approximately(rawMoveVector.x, 0.0f) && Mathf.Approximately(rawMoveVector.y, 0.0f))
        {
            this.m_ConsecutiveMoveCount = 0;
            return false;
        }
        bool flag1 = false;
        for (int i = 0; i < settings.verticalAxes.Length; i++) {
            flag1 |= GetActionDown(settings.verticalAxes[i], true);
        }
        for (int i = 0; i < settings.horizontalAxes.Length; i++) {
            flag1 |= GetActionDown(settings.horizontalAxes[i], true);
        }
        
        bool flag2 = (double) Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0.0;
        if (!flag1)
            flag1 = !flag2 || this.m_ConsecutiveMoveCount != 1 ? (double) unscaledTime > (double) this.m_PrevActionTime + 1.0 / (double) this.m_InputActionsPerSecond : (double) unscaledTime > (double) this.m_PrevActionTime + (double) this.m_RepeatDelay;
        if (!flag1)
            return false;
        AxisEventData axisEventData = this.GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
        if (axisEventData.moveDir != MoveDirection.None)
        {
            ExecuteEvents.Execute<IMoveHandler>(this.eventSystem.currentSelectedGameObject, (BaseEventData) axisEventData, ExecuteEvents.moveHandler);
            if (!flag2)
            this.m_ConsecutiveMoveCount = 0;
            ++this.m_ConsecutiveMoveCount;
            this.m_PrevActionTime = unscaledTime;
            this.m_LastMoveVector = rawMoveVector;
        }
        else
            this.m_ConsecutiveMoveCount = 0;
        return axisEventData.used;
        }

        // protected void ProcessMouseEvent()
        // {
        // this.ProcessMouseEvent(0);
        // }

        // /// <summary>
        // ///   <para>Iterate through all the different mouse events.</para>
        // /// </summary>
        // /// <param name="id">The mouse pointer Event data id to get.</param>
        // protected void ProcessMouseEvent(int id)
        // {
        // PointerInputModule.MouseState pointerEventData = this.GetMousePointerEventData(id);
        // PointerInputModule.MouseButtonEventData eventData = pointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
        // this.ProcessMousePress(eventData);
        // this.ProcessMove(eventData.buttonData);
        // this.ProcessDrag(eventData.buttonData);
        // this.ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
        // this.ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
        // this.ProcessMousePress(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
        // this.ProcessDrag(pointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
        // if (Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
        //     return;
        // ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), (BaseEventData) eventData.buttonData, ExecuteEvents.scrollHandler);
        // }

        /// <summary>
        ///   <para>Send a update event to the currently selected object.</para>
        /// </summary>
        /// <returns>
        ///   <para>If the update event was used by the selected object.</para>
        /// </returns>
        // protected bool SendUpdateEventToSelectedObject()
        // {
        // if ((UnityEngine.Object) this.eventSystem.currentSelectedGameObject == (UnityEngine.Object) null)
        //     return false;
        // BaseEventData baseEventData = this.GetBaseEventData();
        // ExecuteEvents.Execute<IUpdateSelectedHandler>(this.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
        // return baseEventData.used;
        // }

        protected void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
        {
        PointerEventData buttonData = data.buttonData;
        GameObject gameObject1 = buttonData.pointerCurrentRaycast.gameObject;
        if (data.PressedThisFrame())
        {
            buttonData.eligibleForClick = true;
            buttonData.delta = Vector2.zero;
            buttonData.dragging = false;
            buttonData.useDragThreshold = true;
            buttonData.pressPosition = buttonData.position;
            buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
            this.DeselectIfSelectionChanged(gameObject1, (BaseEventData) buttonData);
            GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.pointerDownHandler);
            if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) null)
            gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
            float unscaledTime = Time.unscaledTime;
            if ((UnityEngine.Object) gameObject2 == (UnityEngine.Object) buttonData.lastPress)
            {
            if ((double) (unscaledTime - buttonData.clickTime) < 0.300000011920929)
                ++buttonData.clickCount;
            else
                buttonData.clickCount = 1;
            buttonData.clickTime = unscaledTime;
            }
            else
            buttonData.clickCount = 1;
            buttonData.pointerPress = gameObject2;
            buttonData.rawPointerPress = gameObject1;
            buttonData.clickTime = unscaledTime;
            buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
            if ((UnityEngine.Object) buttonData.pointerDrag != (UnityEngine.Object) null)
            ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.initializePotentialDrag);
        }
        if (!data.ReleasedThisFrame())
            return;
        ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerUpHandler);
        GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
        if ((UnityEngine.Object) buttonData.pointerPress == (UnityEngine.Object) eventHandler && buttonData.eligibleForClick)
            ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerClickHandler);
        else if ((UnityEngine.Object) buttonData.pointerDrag != (UnityEngine.Object) null && buttonData.dragging)
            ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.dropHandler);
        buttonData.eligibleForClick = false;
        buttonData.pointerPress = (GameObject) null;
        buttonData.rawPointerPress = (GameObject) null;
        if ((UnityEngine.Object) buttonData.pointerDrag != (UnityEngine.Object) null && buttonData.dragging)
            ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.endDragHandler);
        buttonData.dragging = false;
        buttonData.pointerDrag = (GameObject) null;
        if (!((UnityEngine.Object) gameObject1 != (UnityEngine.Object) buttonData.pointerEnter))
            return;
        this.HandlePointerExitAndEnter(buttonData, (GameObject) null);
        this.HandlePointerExitAndEnter(buttonData, gameObject1);
        }
    }
    public abstract class PointerInputModule : BaseInputModule
    {

        static  UISettings _settings;
        protected static UISettings settings {
            get {
                if (_settings == null) _settings = GameSettings.GetSettings<UISettings>();
                return _settings;
            }
        }
        public static int usingController;

 
        public static Vector2 mouseAxis {
            get {
                return ActionsInterface.GetMouseAxis(usingController, false);
                // float mouseRatioX = (mousePosition.x / Screen.width) * 2 - 1;
                // float mouseRatioY = (mousePosition.y / Screen.height) * 2 - 1;
                // return new Vector2(mouseRatioX, mouseRatioY);
            }
        }
        public static Vector2 mousePosition { get { return ActionsInterface.GetMousePos(usingController, false); } }
        public static Vector2 mouseScrollDelta { get { return ActionsInterface.GetMouseScrollDelta(usingController, false); } }
        public static bool GetMouseButtonDown (int button) { return ActionsInterface.GetMouseButtonDown(button, usingController, false); }
        public static bool GetMouseButtonUp (int button) { return ActionsInterface.GetMouseButtonUp(button, usingController, false); }
        public static bool GetMouseButton (int button) { return ActionsInterface.GetMouseButton(button, usingController, false); }
        
        public static bool GetActionDown (int action, bool checkingAxis) { return ActionsInterface.GetActionDown(action, checkingAxis, usingController, false); }
        public static bool GetActionUp (int action, bool checkingAxis) { return ActionsInterface.GetActionUp(action, checkingAxis, usingController, false); }
        public static bool GetAction (int action, bool checkingAxis) { return ActionsInterface.GetAction(action, checkingAxis, usingController, false); }
        public static float GetAxis (int axisIndex) { return ActionsInterface.GetAxis(axisIndex, usingController, false); }
        
        public static Vector2Int GetUIActions (List<int> actions){
            bool usesAnyController = usingController < 0;
            for (int i = 0; i < actions.Count; i++) {
                int action = actions[i];
                if (usesAnyController) {
                    for (int controller = 0; controller < ActionsInterface.maxControllers; controller++) {
                        if (ActionsInterface.GetActionDown(action, false, controller, false)) {

                            Debug.Log("GOT ACTION " + action);
                            return new Vector2Int(action, controller);
                        }
                    }
                }
                else {
                    if (ActionsInterface.GetActionDown(action, false, usingController, false)) {    
                        return new Vector2Int(action, usingController);
                    }
                }
            }
            return new Vector2Int(-1, usingController);        
        }



        public static void MarkAllSelectionAxesOccupied () {
            for (int i = 0; i < settings.horizontalAxes.Length; i++)
                ActionsInterface.MarkAxisOccupied(settings.horizontalAxes[i], usingController);
            for (int i = 0; i < settings.verticalAxes.Length; i++)
                ActionsInterface.MarkAxisOccupied(settings.verticalAxes[i], usingController);
        }
        public static void MarkAllSelectionAxesUnoccupied () {
            for (int i = 0; i < settings.horizontalAxes.Length; i++)
                ActionsInterface.MarkAxisUnoccupied(settings.horizontalAxes[i]);
            for (int i = 0; i < settings.verticalAxes.Length; i++)
                ActionsInterface.MarkAxisUnoccupied(settings.verticalAxes[i]);
        }

        public static void MarkActionsOccupied (List<int> actions) {
            for (int i = 0; i < actions.Count; i++)
                ActionsInterface.MarkActionOccupied(actions[i], usingController);
        }
        public static void MarkActionsUnoccupied (List<int> actions) {
            for (int i = 0; i < actions.Count; i++)
                ActionsInterface.MarkActionUnoccupied(actions[i]);
        }

        public static void MarkAllBaseUIInputsOccupied () {
            MarkAllSelectionAxesOccupied();
            ActionsInterface.MarkActionOccupied(settings.cancelAction, usingController);
        }
        public static void MarkAllBaseUIInputsUnoccupied () {
            MarkAllSelectionAxesUnoccupied();
            ActionsInterface.MarkActionUnoccupied(settings.cancelAction);
        }

            
        
        protected Dictionary<int, PointerEventData> m_PointerData = new Dictionary<int, PointerEventData>();
        private readonly PointerInputModule.MouseState m_MouseState = new PointerInputModule.MouseState();
        
        protected bool GetPointerData(int id, out PointerEventData data, bool create)
        {
        if (this.m_PointerData.TryGetValue(id, out data) || !create)
            return false;
        data = new PointerEventData(this.eventSystem)
        {
            pointerId = id
        };
        this.m_PointerData.Add(id, data);
        return true;
        }


        protected void CopyFromTo(PointerEventData from, PointerEventData to)
        {
        to.position = from.position;
        to.delta = from.delta;
        to.scrollDelta = from.scrollDelta;
        to.pointerCurrentRaycast = from.pointerCurrentRaycast;
        to.pointerEnter = from.pointerEnter;
        }

        // protected PointerEventData.FramePressState StateForMouseButton(int buttonId)
        // {
        // bool mouseButtonDown = GetMouseButtonDown(buttonId);
        // bool mouseButtonUp = GetMouseButtonUp(buttonId);
        // if (mouseButtonDown && mouseButtonUp)
        //     return PointerEventData.FramePressState.PressedAndReleased;
        // if (mouseButtonDown)
        //     return PointerEventData.FramePressState.Pressed;
        // return mouseButtonUp ? PointerEventData.FramePressState.Released : PointerEventData.FramePressState.NotChanged;
        // }

        // protected virtual PointerInputModule.MouseState GetMousePointerEventData(int id)
        // {
        // PointerEventData data1;
        // bool pointerData = this.GetPointerData(-1, out data1, true);
        
        // data1.Reset();
        
        // if (pointerData)
        //     data1.position = mousePosition;

        // data1.delta = mousePosition - data1.position;
        // data1.position = mousePosition;
        // data1.scrollDelta = mouseScrollDelta;
        // data1.button = PointerEventData.InputButton.Left;
        // this.eventSystem.RaycastAll(data1, this.m_RaycastResultCache);
        // RaycastResult firstRaycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
        // data1.pointerCurrentRaycast = firstRaycast;
        // this.m_RaycastResultCache.Clear();
        // PointerEventData data2;
        // this.GetPointerData(-2, out data2, true);
        // this.CopyFromTo(data1, data2);
        // data2.button = PointerEventData.InputButton.Right;
        // PointerEventData data3;
        // this.GetPointerData(-3, out data3, true);
        // this.CopyFromTo(data1, data3);
        // data3.button = PointerEventData.InputButton.Middle;
        // this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), data1);
        // this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), data2);
        // this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), data3);
        // return this.m_MouseState;
        // }

        protected PointerEventData GetLastPointerEventData(int id)
        {
        PointerEventData data;
        this.GetPointerData(id, out data, false);
        return data;
        }

        private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
        {
        if (!useDragThreshold)
            return true;
        return (double) (pressPos - currentPos).sqrMagnitude >= (double) threshold * (double) threshold;
        }

        protected virtual void ProcessMove(PointerEventData pointerEvent)
        {
        GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
        this.HandlePointerExitAndEnter(pointerEvent, gameObject);
        }

        protected virtual void ProcessDrag(PointerEventData pointerEvent)
        {
        bool flag = pointerEvent.IsPointerMoving();
        if (flag && (Object) pointerEvent.pointerDrag != (Object) null && (!pointerEvent.dragging && PointerInputModule.ShouldStartDrag(pointerEvent.pressPosition, pointerEvent.position, (float) this.eventSystem.pixelDragThreshold, pointerEvent.useDragThreshold)))
        {
            ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.beginDragHandler);
            pointerEvent.dragging = true;
        }
        if (!pointerEvent.dragging || !flag || !((Object) pointerEvent.pointerDrag != (Object) null))
            return;
        if ((Object) pointerEvent.pointerPress != (Object) pointerEvent.pointerDrag)
        {
            ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, (BaseEventData) pointerEvent, ExecuteEvents.pointerUpHandler);
            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = (GameObject) null;
            pointerEvent.rawPointerPress = (GameObject) null;
        }
        ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, (BaseEventData) pointerEvent, ExecuteEvents.dragHandler);
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
        PointerEventData pointerEventData = this.GetLastPointerEventData(pointerId);
        if (pointerEventData != null)
            return (Object) pointerEventData.pointerEnter != (Object) null;
        return false;
        }

        protected void ClearSelection()
        {
        BaseEventData baseEventData = this.GetBaseEventData();
        using (Dictionary<int, PointerEventData>.ValueCollection.Enumerator enumerator = this.m_PointerData.Values.GetEnumerator())
        {
            while (enumerator.MoveNext())
            this.HandlePointerExitAndEnter(enumerator.Current, (GameObject) null);
        }
        this.m_PointerData.Clear();
        this.eventSystem.SetSelectedGameObject((GameObject) null, baseEventData);
        }

        public override string ToString()
        {
        StringBuilder stringBuilder = new StringBuilder("<b>Pointer Input Module of type: </b>" + (object) this.GetType());
        stringBuilder.AppendLine();
        using (Dictionary<int, PointerEventData>.Enumerator enumerator = this.m_PointerData.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
            KeyValuePair<int, PointerEventData> current = enumerator.Current;
            if (current.Value != null)
            {
                stringBuilder.AppendLine("<B>Pointer:</b> " + (object) current.Key);
                stringBuilder.AppendLine(current.Value.ToString());
            }
            }
        }
        return stringBuilder.ToString();
        }

        /// <summary>
        ///   <para>Deselect the current selected GameObject if the currently pointed-at GameObject is different.</para>
        /// </summary>
        /// <param name="currentOverGo">The GameObject the pointer is currently over.</param>
        /// <param name="pointerEvent">Current event data.</param>
        protected void DeselectIfSelectionChanged(GameObject currentOverGo, BaseEventData pointerEvent)
        {
        if (!((Object) ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo) != (Object) this.eventSystem.currentSelectedGameObject))
            return;
        this.eventSystem.SetSelectedGameObject((GameObject) null, pointerEvent);
        }

        protected class ButtonState
        {
        public PointerEventData.InputButton button;
        public PointerInputModule.MouseButtonEventData eventData;
        }

        protected class MouseState
        {
        private List<PointerInputModule.ButtonState> m_TrackedButtons = new List<PointerInputModule.ButtonState>();

        public PointerInputModule.ButtonState GetButtonState(PointerEventData.InputButton button)
        {
            PointerInputModule.ButtonState buttonState = (PointerInputModule.ButtonState) null;
            for (int index = 0; index < this.m_TrackedButtons.Count; ++index)
            {
            if (this.m_TrackedButtons[index].button == button)
            {
                buttonState = this.m_TrackedButtons[index];
                break;
            }
            }
            if (buttonState == null)
            {
            buttonState = new PointerInputModule.ButtonState()
            {
                button = button,
                eventData = new PointerInputModule.MouseButtonEventData()
            };
            this.m_TrackedButtons.Add(buttonState);
            }
            return buttonState;
        }

        public void SetButtonState(PointerEventData.InputButton button, PointerEventData.FramePressState stateForMouseButton, PointerEventData data)
        {
            PointerInputModule.ButtonState buttonState = this.GetButtonState(button);
            buttonState.eventData.buttonState = stateForMouseButton;
            buttonState.eventData.buttonData = data;
        }
        }

        public class MouseButtonEventData
        {
        public PointerEventData.FramePressState buttonState;
        public PointerEventData buttonData;

        public bool PressedThisFrame()
        {
            if (this.buttonState != PointerEventData.FramePressState.Pressed)
            return this.buttonState == PointerEventData.FramePressState.PressedAndReleased;
            return true;
        }

        public bool ReleasedThisFrame()
        {
            if (this.buttonState != PointerEventData.FramePressState.Released)
            return this.buttonState == PointerEventData.FramePressState.PressedAndReleased;
            return true;
        }
        }
    }
    }


