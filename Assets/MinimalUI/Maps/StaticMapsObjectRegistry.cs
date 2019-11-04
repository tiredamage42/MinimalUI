using System.Collections.Generic;
using UnityEngine;
using UnityTools.GameSettingsSystem;

namespace MinimalUI {

    // to predefine objects that should be registered with ui
    // i.e: buying a map that shows all loot in a certain area on the other side of a huge open world map
    // this way we can show objects even when they're not locally loaded in the scene
    [CreateAssetMenu(menuName="Minimal UI/Maps/Static Maps Object Registry", fileName="Static Maps Object Registry")]
    public class StaticMapsObjectRegistry : GameSettingsObject {
        public List<UIMapsObject> staticObjects;
    }
}
