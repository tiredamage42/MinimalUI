// using System.Collections.Generic;
// using UnityEngine;


// using SimpleUI;
// using Game.PerkSystem;
// namespace Game.UI {

//     // TODO: order game value modifiers by :
//         // set, add, add percentage, multiply
//         // when adding percentage : add percentage of base value so things stack correctly
//     // TODO: add game modifier message for ui showing


//     /*
//         TODO: make quest that initializes perk table available to player into player perk holder
//     */

//     public class UIPerksTable : UISelectableElementHandler
//     {

//         static UIPerksTable _instance;
//         public static UIPerksTable instance { get { return GetInstance<UIPerksTable> (ref _instance); } }
        
//         public void OpenPerksManagementUI (Actor actorToEdit, PerkHandler perkPointsContainer) {
//             instance.OpenUI (  0, new object[] { actorToEdit, perkPointsContainer } );
//         }
//         protected override object[] GetDefaultColdOpenParams() { 
//             return new object[] {  Actor.playerActor, Actor.playerActor.perkHandler  }; 
//         }



//         // TODO: move to game settings
//         [NeatArray] public NeatStringArray specialNames;
//         GameValueModifier addPerkPointModifier;

//         protected override void OnEnable () {
//             base.OnEnable();
//             addPerkPointModifier = new GameValueModifier (GameValue.GameValueComponent.BaseValue, GameValueModifier.ModifyBehavior.Add, 1.0f);
//         }


//         Actor actorToEdit { get { return openedWithParams[0] as Actor; } }
//         PerkHandler perkPointsContainer { get { return openedWithParams[1] as PerkHandler; } }

//         protected override void OnOpenUI() { 
//             BuildButtons(actorToEdit.actorName + " Perks", true, 0);
//             BuildButtons(actorToEdit.actorName + " Special", false, 1);
//         }

//         protected override void OnUISelect (GameObject selectedObject, GameObject[] data, object[] customData) { 
//             string txt = "";
                    
//             if (customData != null) {

//                 int panelIndex = (int)customData[1];
//                 int currentPerkPoints = perkPointsContainer.perkPoints;

//                 //showing perks
//                 if (panelIndex == 0) {
//                     PerkHolder perk = customData[0] as PerkHolder;  

//                     if (perk != null) {

//                         txt = perk.perk.displayName + "\n\n" + perk.perk.description;
                        
//                         for (int i = 0; i < perk.perk.descriptions.list.Length; i++) txt += "Level " + (i+1) + ": " + perk.perk.descriptions.list[i] + "\n";
                        
//                         txt += "Current Level: " + perk.level + " / " + (perk.perk.levels+1);

//                         if (currentPerkPoints > 0 && perk.level < perk.perk.levels+1) txt += "\n\nClick To Add Perk Point";
                        
//                         txt += "\n\nCurrent Perk Points: " + currentPerkPoints;
//                     }   
//                 }
//                 // showing special
//                 else {
//                     GameValue special = customData[0] as GameValue;   
//                     if (special != null) {

//                         txt = special.description + "\n\n" + special.baseValue + " / " + special.baseMinMax.y;
                        
//                         if (currentPerkPoints > 0 && special.baseValue < special.baseMinMax.y) txt += "\n\nClick To Add Perk Point";
                        
//                         txt += "\n\nCurrent Perk Points: " + currentPerkPoints;

//                         // TODO: add modifiers to show
//                     }
//                 }
//             }
//             (uiObject as SimpleUI.ElementHolderCollection).textPanel.SetText(txt);
//         }
        
        
//         protected override List<object> BuildButtonObjectsListForDisplay (int panelIndex){
                        
//             List<object> r = new List<object>();

//             //build perks list
//             if (panelIndex == 0) {
//                 PerkHandler perkHandler = actorToEdit.perkHandler;
//                 for (int i = 0; i < perkHandler.allPerks.Count; i++) {
//                     if (perkHandler.allPerks[i].perk.playerEdit || perkHandler.allPerks[i].level > 0) {
//                         r.Add(perkHandler.allPerks[i]);
//                     }
//                 }
//             }
//             else {
//                 for (int i =0; i < specialNames.list.Length; i++) {
//                     GameValue v = actorToEdit.GetGameValue(specialNames.list[i]);
//                     if (v != null) r.Add(v);
//                 }
//             }
//             return r;
//         }

//         protected override string GetDisplayForButtonObject(object obj) { 
//             PerkHolder holder = obj as PerkHolder;
//             if (holder != null) {
//                 return holder.perk.displayName;
//             }
//             else {
//                 return (obj as GameValue).name;
//             }
//         }

//         int currentPanelIndex;
//         PerkHolder currentPerkHolder;
//         GameValue currentGameValue;
//         void OnConfirmationSelection(bool used, int selectedOption) {
//             if (used && selectedOption == 0) {                
//                 perkPointsContainer.perkPoints--;
//                 if (currentPanelIndex == 0) {
//                     currentPerkHolder.SetLevel(currentPerkHolder.level+1, actorToEdit); 
//                 }
//                 else {
//                     currentGameValue.AddModifier (addPerkPointModifier, 1, Vector3Int.zero);
//                 }
//                 UpdateUIButtons( );
//             }

//         }

//         public int upgradeAction = 0;
//         protected override List<int> InitializeInputsAndNames (out List<string> names) {
//             names = new List<string>() { "Upgrade" };
//             return new List<int>() { upgradeAction };
//         }


//         protected override void SetFlairs(SelectableElement element, object mainObject, int panelIndex) {
//             PerkHolder perkHolder = mainObject as PerkHolder;
//             if (perkHolder != null) {
//                 element.EnableFlair(0, perkHolder.level >= perkHolder.perk.levels);
//             }
//             else {
//                 GameValue specialValue = mainObject as GameValue;
//                 if (specialValue != null) {
//                     element.EnableFlair(0, specialValue.baseValue >= specialValue.baseMinMax.y);
//                 }
                
//             }
//         }

//         protected override void OnUIInput (GameObject selectedObject, GameObject[] data, object[] customData, Vector2Int input){//, int actionOffset) {
//             if (customData != null) {

//                 if (input.x == upgradeAction) {
                
//                     if (perkPointsContainer.perkPoints > 0) {
//                         currentPanelIndex = (int)customData[1];
//                         //showing perks
//                         if (currentPanelIndex == 0) {
//                             currentPerkHolder = customData[0] as PerkHolder;   
//                             if (currentPerkHolder != null) {
//                                 if (currentPerkHolder.level < currentPerkHolder.perk.levels+1) {
//                                     UIManager.ShowSelectionPopup(true, "\n\nAre you sure you want to add a perk point into " + currentPerkHolder.perk.displayName + " ?\n", new string[] {"Yes", "No"}, OnConfirmationSelection);
//                                 }
//                             }
//                         }
//                         // showing special
//                         else {
//                             currentGameValue = customData[0] as GameValue;   
//                             if (currentGameValue != null) {
//                                 if (currentGameValue.baseValue < currentGameValue.baseMinMax.y) {
//                                     UIManager.ShowSelectionPopup(true, "\n\nAre you sure you want to add a perk point into " + currentGameValue.name + " ?\n", new string[] {"Yes", "No"}, OnConfirmationSelection);
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }
// 		}
//     }    
// }    