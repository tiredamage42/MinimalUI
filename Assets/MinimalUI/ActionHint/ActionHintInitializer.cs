

using UnityEngine;
using UnityTools.GameSettingsSystem;
namespace MinimalUI {
    public abstract class ActionHintInitializer : GameSettingsObject
    {
        public abstract ActionHint GetPrefabForAction (int action);
        public abstract ActionHint GetPrefabForAxis (int axis);
        public abstract void OnActionHintInitialized (ActionHint actionHintInstance, int action);
        public abstract void OnAxisHintInitialized (ActionHint axisHintInstance, int action);
    }
}
