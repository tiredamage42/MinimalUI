using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinimalUI {

    public abstract class UICrosshair : UIComponent
    {
        public abstract void UpdateCrosshair (float spread);
    }
}
