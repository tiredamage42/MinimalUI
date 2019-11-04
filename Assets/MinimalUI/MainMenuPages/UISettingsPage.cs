using System.Collections;
using System.Collections.Generic;
using UnityEngine;



using UnityTools;
namespace MinimalUI {

    // show minimap
    // show compass
    // show subtitles
    // color r, g, b
    // show crosshair
    public class UISettingsPage : MenuPage
    {
        
        protected override void OnUIInput(UISelectable selectedObject, object[] data, Vector2Int input, int panelIndex) {
            if (input.x == settings.submitAction) {
                if (data != null) {
                    int buttonSelect = (int)data[0];

                    if (buttonSelect < 4) {
                        bool origValue = (bool)data[1];
                        switch (buttonSelect) {
                            case 0: UIManager.uiSettings.showCrosshair = !origValue; break;
                            case 1: UIManager.uiSettings.showCompass = !origValue; break;
                            case 2: UIManager.uiSettings.showSubtitles = !origValue; break;
                            case 3: UIManager.uiSettings.showMiniMap = !origValue; break;
                        }
                        UpdateUIButtons(panelIndex);//, false);
                        SaveLoad.SaveSettingsOptions();
                    }
                    else {
                        OnChangeColor (buttonSelect - 4);
                    }
                }
            }
        }


        void OnSliderDone (bool used, int value) {
            if (used) {
                OnSliderChange(value);
                SaveLoad.SaveSettingsOptions();
            }
            else {
                UIManager.SetUIColorValue(changingColorIndex, originalColorValue);
            }
        }
        void OnSliderChange (int value) {
            UIManager.SetUIColorValue(changingColorIndex, value);
        }

        int changingColorIndex, originalColorValue;

        void OnChangeColor(int colorIndex) {
            changingColorIndex = colorIndex;
            if (colorIndex == 0) originalColorValue = UIManager.uiSettings.redValue; 
            if (colorIndex == 1) originalColorValue = UIManager.uiSettings.greenValue; 
            if (colorIndex == 2) originalColorValue = UIManager.uiSettings.blueValue; 
            UIManager.ShowIntSliderPopup(true, "Change Color", originalColorValue, 0, 255, OnSliderDone, OnSliderChange);
        }

        protected override void SetFlairs(UISelectable element, ManualMenuButton button, int panelIndex) { 

            int buttonSelect = (int)button.data[0];
            if (buttonSelect < 4) {
                element.EnableFlair(0, (bool)button.data[1]);
            }
        }

        protected override void OnOpenUI(Actor[] actorContexts) {
            BuildButtons("UI Settings", true, 0);
        }

        protected override int MaxButtons() {
            return 6;
        }

        /*
        // color r, g, b
        */
        protected override void GetInternalButtons (List<ManualMenuButton> buttons, int panelIndex) {  
            buttons.Add(new ManualMenuButton("Show Crosshair", "Show The Crosshair", null, new object[] { 0, UIManager.uiSettings.showCrosshair } ) );
            buttons.Add(new ManualMenuButton("Show Compass", "Show The Compass", null, new object[] { 1, UIManager.uiSettings.showCompass } ) );
            buttons.Add(new ManualMenuButton("Show Subtitles", "Show Subtitles", null, new object[] { 2, UIManager.uiSettings.showSubtitles } ) );
            buttons.Add(new ManualMenuButton("Show Minimap", "Show The Minimap", null, new object[] { 3, UIManager.uiSettings.showMiniMap } ) );

            buttons.Add(new ManualMenuButton("Color R", "Change The 'Red' Value Of The Normal UI Color", null, new object[] { 4 } ) );
            buttons.Add(new ManualMenuButton("Color G", "Change The 'Green' Value Of The Normal UI Color", null, new object[] { 5 } ) );
            buttons.Add(new ManualMenuButton("Color B", "Change The 'Blue' Value Of The Normal UI Color", null, new object[] { 6 } ) );
        }
    }
}
