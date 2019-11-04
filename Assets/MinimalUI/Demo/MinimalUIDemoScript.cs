using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityTools;
using UnityTools.EditorTools;
using MinimalUI;

namespace MinimalUIDemo {

    [System.Serializable] public class NeatActionHintsArray : NeatArrayWrapper<ActionHint> {  }
    public class MinimalUIDemoScript : MonoBehaviour
    {
        const string interactableName = "Interactable";
        const string lootBoxName = "LootBox";
        const string npcName = "NPC";
        
        bool lookingAtInteractable, lookingAtLootBox;

        UITextGroup _ammoHud;
        UITextGroup ammoHud { get { return UIManager.GetUIComponentByName<UITextGroup>("AmmoHUD", ref _ammoHud); } }
        [Action] public int shootAction;

        const int maxClip = 16;
        int ammo = 256;
        int clip = 3;

        [NeatArray] public GameValueModifierArray onKillModifiers;
        void UpdateShooting () {
            if (GameManager.isPaused)
                return;
            
            if (ActionsInterface.GetActionDown(shootAction)) {
                if (hitActor != null) {
                    hitActor.AddModifiers(damageModifiers, "Shoot", true, GameManager.playerActor.gameObject, hitActor.gameObject);

                    GameValue npcHealth = hitActor.GetGameValueObject(healthBarGameValueName);
                    if (npcHealth.GetValueComponent(GameValue.GameValueComponent.BaseValue) <= 0) {
                        npcHealth.ReInitialize();
                        GameManager.playerActor.AddModifiers(onKillModifiers, "On Kill", true, hitActor.gameObject, GameManager.playerActor.gameObject);
                    }      
                }

                clip--;
                if (clip <= 0) {
                    clip = maxClip;
                    ammo -= maxClip;
                    ammoHud.allTexts[1].SetText("| " + ammo.ToString());
                }
                ammoHud.allTexts[0].SetText(clip.ToString() + " ");
            }
        }
        // ActionHintsPanel _interactablePrompt;
        // ActionHintsPanel interactablePrompt { get { return UIManager.GetUIComponentByName<ActionHintsPanel>("InteractionHint", ref _interactablePrompt); } }
        
        void UpdateInteractableHint () {
            if (GameManager.isPaused)
                return;
            
            RaycastHit hit;
            if (Physics.Raycast(GameManager.playerCamera.transform.position, GameManager.playerCamera.transform.forward, out hit, 3)) {
                if (hit.transform.name == interactableName) {
                    if (!lookingAtInteractable) {

                        UIManager.ShowInteractionPrompt(0, "INTERACTABLE", new List<int> { 0, 3, 4 }, new List<string> { "USE", "TAKE", "EAT" });
                        
                        // interactablePrompt.textUI.SetText("INTERACTABLE");
                        // interactablePrompt.FadeIn(.1f, 0, 0);
                        // interactablePrompt.AddHintElements(new List<int> { 0, 3, 4 }, new List<string> { "USE", "TAKE", "EAT" });
                        // UIManager.ShowInteractablePrompt(0, .1f, new int[] { 0, 3, 4 }, new string[] { "USE", "TAKE", "EAT" });
                        lookingAtInteractable = true;
                    }
                }
                else {
                    if (lookingAtInteractable) {
                        UIManager.HideInteractionPrompt (0);

                        // interactablePrompt.FadeOut(.1f);
                        // UIManager.HideInteractionHint(0, .1f);
                        lookingAtInteractable = false;
                    }
                }

                if (hit.transform.name == lootBoxName) {
                    if (!lookingAtLootBox) {
                        ManualMenu.OpenMenu( "QuickTradeMenu", null );
                        lookingAtLootBox = true;
                    }
                }
                else {
                    if (lookingAtLootBox) {
                        ManualMenu.CloseMenu( "QuickTradeMenu" );
                        lookingAtLootBox = false;
                    }
                }
            }
            else {
                if (lookingAtInteractable) {
                    UIManager.HideInteractionPrompt (0);

                    // interactablePrompt.FadeOut(.1f);
                    // UIManager.HideInteractionHint(0, .1f);
                    lookingAtInteractable = false;
                }
                if (lookingAtLootBox) {
                    ManualMenu.CloseMenu( "QuickTradeMenu" );
                    lookingAtLootBox = false;
                }
            }
        }

        UIGameValueTracker _enemyHealthBar;
        UIGameValueTracker enemyHealthBar { get { return UIManager.GetUIComponentByName<UIGameValueTracker>("EnemyStatsBar", ref _enemyHealthBar); } }
        
        Actor hitActor;
        void UpdateWeaponsDemo () {
            if (GameManager.isPaused)
                return;
            
            RaycastHit hit;
            if (Physics.Raycast(GameManager.playerCamera.transform.position, GameManager.playerCamera.transform.forward, out hit, 10)) {
                hitActor = hit.transform.GetComponent<Actor>();
                if (hitActor != null) {

                    UIManager.ShowUIComponent(enemyHealthBar, .1f, 0, 0);
                    // enemyHealthBar.FadeIn(.1f, 0, 0);
                    enemyHealthBar.SetGameValue(hitActor.GetGameValueObject(healthBarGameValueName));
                }
                else {
                    enemyHealthBar.UntrackGameValue();
                    UIManager.HideUIComponent(enemyHealthBar, .1f);
                    // enemyHealthBar.FadeOut(.1f);
                }
            }
            else {
                hitActor = null;
                enemyHealthBar.UntrackGameValue();
                UIManager.HideUIComponent(enemyHealthBar, .1f);
                // enemyHealthBar.FadeOut(.1f);
            }
                
        }



        [Action] public int scopeAction;
        public Sprite[] scopeSprites;
        public int maxCrosshairs = 4;

        void UpdateScope () {
            if (GameManager.isPaused)
                return;
            
            if (ActionsInterface.GetActionDown(scopeAction)) {
                UIManager.ShowScopeOverlay(scopeSprites.GetRandom(null), .1f);
                UIManager.EnableCrosshair(-1);
            }
            if (ActionsInterface.GetActionUp(scopeAction)) {
                // Debug.Log("DISABLING");
                UIManager.DisableScopeOverlay(.1f);
                UIManager.EnableCrosshair(Random.Range(0, maxCrosshairs));
            }
        }

        public Transform npcTransform;
        public Transform grenadeTransform;
        public Transform hitOriginTransform;

        public UIDirectionalIcon awarenessIconPrefab, hitIconPrefab, grenadeIconPrefab;
        public float awarenessSpeedFill = 2;
        public float hitIconTime = 1;


        UIDirectionalIcon awarenessIcon;
        FilledCircleSegment awarenessSegment;


        Color RandomRedHue () {
            float r = Random.value;
            float gb = Random.Range(0.0f, r);
            return new Color (r, gb, gb, 1.0f);
        }

        IEnumerator DemoHitOriginIndicator () {
            while (true) {
                yield return new WaitForSeconds(Random.Range(2, 10));    
                UIDirectionalIcon hitIcon = UIManager.AddDirectionalIcon(hitIconPrefab, () => hitOriginTransform.position);
                
                UIManager.ShowDirtOverlay(
                    // 1, 
                    .1f, .25f, 1, RandomRedHue());
                if (Random.value < .5f) 
                    UIManager.ShowDirtOverlay(
                        // 1, 
                        .1f, .25f, 1, RandomRedHue());
                
                yield return new WaitForSeconds(hitIconTime);
                UIManager.RemoveDirectionalIcon(hitIcon);
            }
        }

        void StartGrenadeIndicatorDemo () {
            // UIDirectionalIcon grenadeIcon = 
            UIManager.AddDirectionalIcon(grenadeIconPrefab, () => grenadeTransform.position);
        }


        void UpdateAwarenessUI () {
            if (GameManager.isPaused)
                return;

            Vector3 npcFwd = npcTransform.forward;
            npcFwd.y = 0;

            Vector3 dirToPlayer = GameManager.playerActor.GetPosition() - npcTransform.position;
            dirToPlayer.y = 0;

            if (Vector3.Angle(npcFwd, dirToPlayer) < 90) {
                if (awarenessIcon == null) {
                    awarenessIcon = UIManager.AddDirectionalIcon(awarenessIconPrefab, () => npcTransform.position);
                    awarenessSegment = awarenessIcon.GetComponentInChildren<FilledCircleSegment>();
                    awarenessSegment.currentFill = 0;
                }
                if (awarenessSegment != null) {
                    if (awarenessSegment.currentFill < 1)
                        awarenessSegment.SetFill (awarenessSegment.currentFill + Time.deltaTime * awarenessSpeedFill);
                }
            }
            else {
                if (awarenessIcon != null) {
                    UIManager.RemoveDirectionalIcon(awarenessIcon);
                    awarenessIcon = null;
                }
            }
        }



        public Sprite[] randomIconSprites;
        List<UIIconTableElement> iconElements = new List<UIIconTableElement>();
        IEnumerator DemoIconTable () {
            while (true) {
                yield return new WaitForSeconds(Random.Range(1, 3));
                int action = Random.Range(0, 3);
                switch (action) {
                    case 0:
                        if (randomIconSprites.Length > 0 && iconElements.Count < 10) 
                            iconElements.Add(UIManager.AddIconToIconTable( randomIconSprites.GetRandom(null) ));
                        break;
                    case 1:
                        if (iconElements.Count > 0)
                            UIManager.RemoveIconFromTable(iconElements.GetRandom(null));
                        break;
                    case 2:
                        if (iconElements.Count > 0)
                            iconElements.GetRandom(null).SetIconColorScheme((UIColorScheme)Random.Range(0, 3));
                        break;
                }
            }
        }
        // [NeatArray] public NeatActionHintsArray actionHints;
        // [NeatArray] public NeatActionHintsArray axisHints;

        // Camera _mainCamera;
        // Camera mainCamera {
        //     get {
        //         if (_mainCamera == null) _mainCamera = Camera.main;
        //         return _mainCamera;
        //     }
        // }

        // public ManualMenu mainMenu;
        [Action] public int mainMenuAction, quickInventoryAction;
        public string quickInventoryMenuName = "QuickInventoryMenu";

        public string healthBarGameValueName = "HP";

        GameValue healthGameValue;
        
        void Start()
        {
            // UIManager.InitializeUIManager( 
            //     (i) => actionHints[i], 
            //     (i) => axisHints[i], 
            //     // () => mainCamera, 
            //     this 
            // );    

            StartCoroutine(DemoSubtitles());
            StartCoroutine(DemoGameMessages());
            StartCoroutine(DemoObjectives());
            
            StartCoroutine(DemoTutorialMessages());
            StartCoroutine(DemoQuestPanel());

            
            StartCoroutine(DemoIconTable());

            StartCoroutine(DemoHitOriginIndicator());
            StartGrenadeIndicatorDemo();


            xpGameValue = Actor.playerActor.GetGameValueObject(xpGameValueName);
            xpGameValue.AddChangeListener(OnXPValueChange);
            UIXPLevelBar xpBar = UIManager.instance.gameObject.GetComponentInChildren<UIXPLevelBar>(true);
            xpBar.SetXPGameValue(xpGameValue);

            

            healthGameValue = Actor.playerActor.GetGameValueObject(healthBarGameValueName);

            UIGameValueTracker healthBar = UIManager.GetUIComponentByName<UIGameValueTracker>(HPBar);
            healthBar.colorSchemeThresholds = colorSchemeThresholds;
            healthBar.SetGameValue(healthGameValue);
            StartCoroutine(DemoHealth());
        }
        GameValue xpGameValue;

        public string xpGameValueName = "XP";

        void OnXPValueChange (float delta, float newValue, float min, float max) {
            // Debug.Log("CHanged XP");
            if (newValue >= max) {
                xpGameValue.ReInitialize();
            }
        }

            







        void Update () {
            UpdateWeaponsDemo();
            UpdateShooting();
            UpdateInteractableHint();

            UpdateScope();

            UpdateAwarenessUI();


            if (ActionsInterface.GetActionDown(mainMenuAction, false, -1, true)) {
                // if (UIManager.instance.mainMenu.isOpen) {
                //     UIManager.instance.mainMenu.CloseMenu();
                // }
                // else {
                //     UIManager.instance.mainMenu.OpenMenu(null);
                // }
                ManualMenu.mainMenu.ToggleMenu(null);
            }            

            if (ActionsInterface.GetActionDown(quickInventoryAction, false, -1, true)) {
                ManualMenu.ToggleMenu(quickInventoryMenuName, new Actor[] { GameManager.playerActor });
            }
        }

        #region SUBTITLES
        string[] speakers = new string[] { 
            "John", "Mohammed", "Jimmy" 
        };
        string[] barks = new string[] { 
            "Hey How's It Going?", 
            "The weather is looking good today! Time to go for a walk", 
            "Oh No! I dropped my ice cream into the Grand Canyon. What should I do now? This sucks really badly, I should've gotten something I cant spill" 
        };
        
        IEnumerator DemoSubtitles () {
            while (true) {
                yield return new WaitForSeconds(Random.Range(1, 6));
                // Debug.LogError("Showing subtitile");
                int index = Random.Range(0, speakers.Length);
                UIManager.ShowSubtitles(speakers[index], barks[index], .1f, 3, .1f);
            }
        }

        #endregion
        

        #region TUTORIALS
        string[] tutorials = new string[] { 
            "Pick up items to make the game easier", 
            "Use some cheat codes, or mods, i dont care!", 
            "You probably should try and win the game..." 
        };
        IEnumerator DemoTutorialMessages () {
            while (true) {
                yield return new WaitForSecondsRealtime(Random.Range(.25f, 3));
                int index = Random.Range(0, tutorials.Length);
                UIManager.ShowTutorialPopup(tutorials[index]);
            }
        }
        #endregion

        #region QUESTPANEL
        string[] quests = new string[] { 
            "Radio Gaga", 
            "Another Quest To Avoid", 
        };
        string[] questsTitles = new string[] { 
            "Completed:", 
            "Started:", 
        };


        IEnumerator DemoQuestPanel () {
            while (true) {
                yield return new WaitForSecondsRealtime(Random.Range(.25f, 3));
                int index = Random.Range(0, quests.Length);
                UIManager.ShowQuestPopup(quests[index], questsTitles[index]);
            }
        }
        #endregion

        

        #region MESSAGING
        string[] messages = new string[] { 
            "Picked Up Some Items", 
            "This is an extra long message to see if we should wrap text around", 
            "Your Health went down!" 
        };
        IEnumerator DemoGameMessages () {
            while (true) {
                yield return new WaitForSecondsRealtime(Random.Range(.25f, 3));
                int index = Random.Range(0, messages.Length);
                int scheme = Random.Range(0, 3);
                UIManager.ShowGameMessage(messages[index], false, (UIColorScheme)scheme, Random.value < .5f);
            }
        }
        string[] objectives = new string[] { 
            "Go to the old fort", 
            "a Pretty long objective, that should probaly go on two lines", 
            "Pick up 2 fusion cores" 
        };
        IEnumerator DemoObjectives () {
            while (true) {
                yield return new WaitForSecondsRealtime(Random.Range(.25f, 3));
                int index = Random.Range(0, objectives.Length);
                int scheme = Random.Range(0, 3);
                UIManager.ShowObjectivesMessage(objectives[index], false, (UIColorScheme)scheme, Random.value < .5f);
            }
        }
        #endregion


        [NeatArray] public GameValueModifierArray damageModifiers;

        // float health = 100;
        void GiveDamage(float damage) {

            

            Actor.playerActor.AddModifiers(damageModifiers, "Damage", true, gameObject, Actor.playerActor.gameObject);

            if (healthGameValue.GetValueComponent(GameValue.GameValueComponent.BaseValue) <= 0) {
                healthGameValue.ReInitialize();
            }

            // health -= damage;
            // if (health <= 0) {
            //     health = 100;
            // }
            // UpdateHealthBar();
        } 

        IEnumerator DemoHealth () {
            while (true) {
                if (!GameManager.isPaused) {
                    yield return new WaitForSeconds(Random.Range(1, 6));
                    GiveDamage(Random.Range(1.0f, 25.0f));
                }
            }
        }


        #region HEALTHBAR

        [Header("Invalid/Warning Thresholds For Health")]
        [NeatArray(2)] public NeatFloatArray colorSchemeThresholds;

        // UIGameValueTracker _healthBar;
        // UIGameValueTracker healthBar {
        //     get {
        //         if (_healthBar == null) _healthBar = UIManager.GetUIComponentByName<UIGameValueTracker>(HPBar);
        //         return _healthBar;
        //     }
        // }
        
        const string HPBar = "HPBar";

        // UIValueTracker _healthBar;
        // UIValueTracker healthBar {
        //     get {
        //         if (_healthBar == null) _healthBar = UIManager.GetUIComponentByName<UIValueTracker>(HPBar);
        //         return _healthBar;
        //     }
        // }
        

        // UIValueTracker healthBar
        // public UIValueTracker healthBar;
        // void UpdateHealthBar () {
        //     if (healthBar == null) {
        //         Debug.LogError("No Healthbar Supplied");
        //         return;
        //     }
        //     healthBar.SetValue(health / 100, health > 25 ? UIColorScheme.Normal : UIColorScheme.Invalid);
        // }
        #endregion

    }
}
