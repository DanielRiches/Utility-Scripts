using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerInput playerInput; // Reference Auto-Generated C# Action Map Script
    [SerializeField] private GameManager gameManager;
    [Header("---- Fullscreen -----------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject blackScreenGameobject;
    [SerializeField] private CanvasGroup blackScreenCanvasGroup;
    [SerializeField] private CanvasGroup fullscreenCanvasGroup;
    [Header("---- Cursor --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup cursorCanvasGroup;
    [SerializeField] private Texture2D pcCursorTexture;
    [SerializeField] private Texture2D controllerCursorTexture;
    [SerializeField] private RawImage cursorRenderer;
    [SerializeField] private Transform cursorTransform;
    [Header("---- Splash --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject splashEpilepsyWarningObject;
    [SerializeField] private GameObject splashSaveWarningObject;
    public GameObject splashLanguageDropdownObject;// ACCESSED EXTERNALLY
    [SerializeField] private GameObject splashEnglishButtonObject;
    [SerializeField] private GameObject splashFrenchButtonObject;
    [SerializeField] private GameObject splashGermanButtonObject;
    [SerializeField] private GameObject splashItalianButtonObject;
    [SerializeField] private GameObject splashSpanishButtonObject;
    [SerializeField] private GameObject splashAfricanButtonObject;
    [SerializeField] private GameObject splashRussianButtonObject;
    [SerializeField] private GameObject splashChineseButtonObject;
    [SerializeField] private GameObject splashJapaneseButtonObject;
    [SerializeField] private GameObject splashBrazilianButtonObject;
    [Header("---- Main Menu --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject mainMenuGameobject;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private Animator mainMenuAnimator;
    [Header("Issue")]
    [Space(5)]
    [SerializeField] private GameObject notificationGameobject;
    [SerializeField] private CanvasGroup notificationCanvasGroup;
    [SerializeField] private Animator notificationAnimator;
    [Header("---- Options ----------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject optionsMenuGameobject;
    [SerializeField] private CanvasGroup optionsMenuCanvasGroup;
    [SerializeField] private Animator optionsBGAnimator;
    [SerializeField] private Animator optionsApplyAnimator;
    [SerializeField] private Animator optionsExitAnimator;    
    [SerializeField] private Animator optionsSectionTitleAnimator;    
    [Header("Gameplay")]
    [SerializeField] private GameObject optionsGameplaySectionObject;
    [SerializeField] private CanvasGroup optionsGameplaySectionCanvasGroup;
    [SerializeField] private Animator optionsGameplayButtonAnimator;
    [SerializeField] private Animator optionsGameplaySectionAnimator;
    [SerializeField] private EventTrigger optionsOnlineVisibilityDescriptionTrigger;
    [SerializeField] private EventTrigger optionsAutosavesDescriptionTrigger;
    [Header("Video")]
    [SerializeField] private GameObject optionsVideoSectionObject;
    [SerializeField] private CanvasGroup optionsVideoSectionCanvasGroup;
    [SerializeField] private Animator optionsVideoButtonAnimator;
    [SerializeField] private Animator optionsVideoSectionAnimator;
    [Header("Audio")]
    [SerializeField] private Animator optionsAudioButtonAnimator;
    [Header("Keyboard")]
    [SerializeField] private Animator optionsKeyboardButtonAnimator;
    [Header("Gamepad")]
    [SerializeField] private Animator optionsGamepadButtonAnimator;
    [Header("Interface")]
    [SerializeField] private Animator optionsInterfaceButtonAnimator;
    [Header("Accessibility")]
    [SerializeField] private Animator optionsAccessibilityButtonAnimator;
    [Header("---- Diagnostics ------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject diagnosticsObject;
    [Space(10)]
    [SerializeField] private GameObject diagnosticsFPSGameobject;
    [SerializeField] private CanvasGroup diagnosticsFPSCanvasGroup;
    [SerializeField] private Animator diagnosticsFPSAnimator;
    [SerializeField] private Animator diagnosticsFPSCanvasAnimator;
    [Header("Ping")]
    [Space(5)]
    [SerializeField] private GameObject diagnosticsPingGameobject;
    [SerializeField] private CanvasGroup diagnosticsPingCanvasGroup;
    [SerializeField] private Animator diagnosticsPingAnimator;
    [SerializeField] private Animator diagnosticsPingCanvasAnimator;
    [Header("Memory")]
    [Space(5)]
    [SerializeField] private GameObject diagnosticsMemoryGameobject;
    [SerializeField] private CanvasGroup diagnosticsMemoryCanvasGroup;
    [SerializeField] private Animator diagnosticsMemoryAnimator;
    [SerializeField] private Animator diagnosticsMemoryCanvasAnimator;
    [Header("---- Loading ------------------------------------------------------------------")]
    [Space(5)]
    public TextMeshProUGUI loadingProgress;// ACCESSED EXTERNALLY
    public GameObject loadingIcon;
    [SerializeField] private float loadingIconRotationSpeed = 1f;
    [HideInInspector] public float percentageCompleted = 0f;
    [HideInInspector] public float combinedProgress = 0f;
    [Header("---- Credits ------------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject creditsMenuGameobject;
    [SerializeField] private Animator creditsAnimator;
    private float creditsFadeTimer;
    [Header("---- Version ------------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private GameObject versionMenuGameobject;
    [SerializeField] private Animator versionCanvasGroupAnimator;
    [SerializeField] private CanvasGroup versionCanvasGroup;
    [SerializeField] private Animator versionTitleAnimator;
    [SerializeField] private Animator versionBGAnimator;
    [SerializeField] private Animator versionExitButtonAnimator;
    [SerializeField] private Animator versionButtonAnimator;
    [Space(10)]
    [Header("[Localization]")]    
    public Image selectedLanguageImage;// ACCESSED EXTERNALLY
    public Sprite englishFlag;// ACCESSED EXTERNALLY
    public Sprite frenchFlag;// ACCESSED EXTERNALLY
    public Sprite germanFlag;// ACCESSED EXTERNALLY
    public Sprite italianFlag;// ACCESSED EXTERNALLY
    public Sprite spanishFlag;// ACCESSED EXTERNALLY
    public Sprite americanFlag;// ACCESSED EXTERNALLY
    public Sprite russianFlag;// ACCESSED EXTERNALLY
    public Sprite chineseFlag;// ACCESSED EXTERNALLY
    public Sprite japaneseFlag;// ACCESSED EXTERNALLY
    public Sprite brazilFlag;// ACCESSED EXTERNALLY
    private string languageClassName;
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset chineseFont;
    [SerializeField] private TMP_FontAsset japaneseFont;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI splashTitleText;
    [SerializeField] private TextMeshProUGUI splashSelectLanguageText;
    [SerializeField] private TextMeshProUGUI splashSelectLanguageNoticeText;
    [SerializeField] private TextMeshProUGUI splashSelectedLanguageText;
    [SerializeField] private TextMeshProUGUI splashSelectLanguageConfirmButtonText;
    [SerializeField] private TextMeshProUGUI splashEpilepsyWarningText;
    [SerializeField] private TextMeshProUGUI splashSaveWarningText;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI mainMenuContinueButtonText;
    [Space(5)]
    [SerializeField] private TextMeshProUGUI optionsTitle;
    [SerializeField] private TextMeshProUGUI optionsDescription;
    [Space(5)]
    public TextMeshProUGUI diagnosticsFPSCounterText;// ACCESSED EXTERNALLY
    public TextMeshProUGUI diagnosticsPingCounterText;// ACCESSED EXTERNALLY
    public TextMeshProUGUI diagnosticsMemoryCounterText;// ACCESSED EXTERNALLY

    private void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.uiManager = this;

        playerInput = new PlayerInput();

        playerInput.UI.Enable(); // enable UI Action Maps
        playerInput.UI.Cancel.started += onUICancel;        
        playerInput.KeyboardControls.Disable();
        playerInput.GamePadControls.Disable();

        ActivateUI(false, fullscreenCanvasGroup, null, null);
        ActivateUI(false, cursorCanvasGroup, null, null);
        Cursor.visible = false;        
    }

    private void Update()
    {        
        Utils.FollowCursorInUI(gameManager.usingKeyboard, ref cursorTransform);

        if (gameManager.cursor)
        {
            ActivateCursor();
        }
        else
        {
            DeActivateCursor();
        }

        Loading();

        // ACTIVATE
        ActivateUI(gameManager.blackScreen, blackScreenCanvasGroup, blackScreenGameobject, null);// BLACKSCREEN
        ActivateUI(gameManager.inMainMenu, mainMenuCanvasGroup, mainMenuGameobject, versionButtonAnimator);// MAIN MENU

        ActivateUI(gameManager.inOptionsGameplayMenu, optionsGameplaySectionCanvasGroup, optionsGameplaySectionObject, optionsGameplaySectionAnimator);// OPTIONS
        ActivateUI(gameManager.inOptionsVideoMenu, optionsVideoSectionCanvasGroup, optionsVideoSectionObject, optionsVideoSectionAnimator);
        ActivateUI(gameManager.inOptionsMenu, optionsMenuCanvasGroup, optionsMenuGameobject, null);

        ActivateUI(gameManager.viewingPatchNotes, versionCanvasGroup, versionMenuGameobject, versionCanvasGroupAnimator);// VERSION


        ActivateUI(gameManager.viewingMemory || gameManager.viewingPing || gameManager.viewingFPS, null, diagnosticsObject, null); // DIAGNOSTICS
        if (diagnosticsObject.activeSelf)
        {
            ActivateUI(gameManager.viewingFPS, diagnosticsFPSCanvasGroup, diagnosticsFPSGameobject, diagnosticsFPSAnimator);
            ActivateUI(gameManager.viewingPing, diagnosticsPingCanvasGroup, diagnosticsPingGameobject, diagnosticsPingAnimator);
            ActivateUI(gameManager.viewingMemory, diagnosticsMemoryCanvasGroup, diagnosticsMemoryGameobject, diagnosticsMemoryAnimator);
        }        

        ActivateUI(gameManager.viewingNotification, notificationCanvasGroup, notificationGameobject, notificationAnimator);// NOTIFICATION
        ActivateUI(gameManager.viewingCredits, null, creditsMenuGameobject, creditsAnimator);// CREDITS

        // ANIMATE
        if (mainMenuGameobject.activeSelf)// MAIN MENU
        {            
            AnimateUI(gameManager.inMainMenu, mainMenuAnimator, 1, Strings.animUIOpen);
            AnimateUI(gameManager.inMainMenu, versionButtonAnimator, 0, Strings.animUIOpen);// VERSION BUTTON
        }     

        if (optionsMenuGameobject.activeSelf)// OPTIONS
        {
            AnimateUI(gameManager.inOptionsMenu, optionsApplyAnimator, 2, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsExitAnimator, 2, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsBGAnimator, 2, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsSectionTitleAnimator, 2, Strings.animUIOpen);

            AnimateUI(gameManager.inOptionsMenu, optionsGameplayButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsGameplayButtonAnimator, 2, Strings.animUIOpen);
            

            AnimateUI(gameManager.inOptionsMenu, optionsVideoButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsVideoButtonAnimator, 2, Strings.animUIOpen);
            

            AnimateUI(gameManager.inOptionsMenu, optionsAudioButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsAudioButtonAnimator, 2, Strings.animUIOpen);


            AnimateUI(gameManager.inOptionsMenu, optionsKeyboardButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsKeyboardButtonAnimator, 2, Strings.animUIOpen);


            AnimateUI(gameManager.inOptionsMenu, optionsGamepadButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsGamepadButtonAnimator, 2, Strings.animUIOpen);


            AnimateUI(gameManager.inOptionsMenu, optionsInterfaceButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsInterfaceButtonAnimator, 2, Strings.animUIOpen);


            AnimateUI(gameManager.inOptionsMenu, optionsAccessibilityButtonAnimator, 0, Strings.animUIOpen);
            AnimateUI(gameManager.inOptionsMenu, optionsAccessibilityButtonAnimator, 2, Strings.animUIOpen);
        }

        if (optionsGameplaySectionObject.activeSelf)// OPTIONS GAMEPLAY SECTION
        {
            AnimateUI(gameManager.inOptionsGameplayMenu, optionsGameplaySectionAnimator, 1, Strings.animUIOpen);            
        }

        if (optionsVideoSectionObject.activeSelf)// OPTIONS VIDEO SECTION
        {            
            AnimateUI(gameManager.inOptionsVideoMenu, optionsVideoSectionAnimator, 1, Strings.animUIOpen);
        }

        if (creditsMenuGameobject.activeSelf)// CREDITS
        {                       
            AnimateUI(gameManager.viewingCredits, creditsAnimator, 0, Strings.animUIOpen);
        }        

        if (notificationGameobject.activeSelf)// NOTIFICATION
        {
            AnimateUI(gameManager.viewingNotification, notificationAnimator, 0, Strings.animUIOpen); 
        }

        AnimateUI(gameManager.viewingFPS, diagnosticsFPSCanvasAnimator, 1, Strings.animUIOpen);// DIAGNOSTICS FPS
        AnimateUI(gameManager.viewingFPS, diagnosticsFPSAnimator, 0, Strings.animUIOpen);
        AnimateUI(gameManager.viewingPing, diagnosticsPingCanvasAnimator, 1, Strings.animUIOpen);// DIAGNOSTICS PING
        AnimateUI(gameManager.viewingPing, diagnosticsPingAnimator, 0, Strings.animUIOpen);
        AnimateUI(gameManager.viewingMemory, diagnosticsMemoryCanvasAnimator, 1, Strings.animUIOpen);// DIAGNOSTICS MEMORY
        AnimateUI(gameManager.viewingMemory, diagnosticsMemoryAnimator, 0, Strings.animUIOpen);

        if (versionMenuGameobject.activeSelf)// VERSION
        {                       
            AnimateUI(gameManager.viewingPatchNotes, versionCanvasGroupAnimator, 1, Strings.animUIOpen);
            AnimateUI(gameManager.viewingPatchNotes, versionBGAnimator, 2, Strings.animUIOpen);
            AnimateUI(gameManager.viewingPatchNotes, versionExitButtonAnimator, 2, Strings.animUIOpen);
            AnimateUI(gameManager.viewingPatchNotes, versionTitleAnimator, 2, Strings.animUIOpen);
        }

        // LOCALIZE
        if (gameManager.localizeLanguage)
        {
            if (gameManager.inSplashScreen)
            {                
                Utils.LocalizeText(gameManager, ref languageClassName, ref splashTitleText, "gameTitle", defaultFont, chineseFont, japaneseFont);

                Utils.LocalizeText(gameManager, ref languageClassName, ref splashSelectLanguageText, "splashSelectLanguageText", defaultFont, chineseFont, japaneseFont);
                Utils.LocalizeText(gameManager, ref languageClassName, ref splashSelectedLanguageText, "splashSelectedLanguageText", defaultFont, chineseFont, japaneseFont);
                Utils.LocalizeText(gameManager, ref languageClassName, ref splashSelectLanguageNoticeText, "splashSelectLanguageNoticeText", defaultFont, chineseFont, japaneseFont);
                Utils.LocalizeText(gameManager, ref languageClassName, ref splashSelectLanguageConfirmButtonText, "splashSelectLanguageConfirmButtonText", defaultFont, chineseFont, japaneseFont);

                if (splashEpilepsyWarningObject && gameManager.viewingEpilepsyWarning)
                {                    
                    Utils.LocalizeText(gameManager, ref languageClassName, ref splashEpilepsyWarningText, "splashEpilepsyWarningText", defaultFont, chineseFont, japaneseFont);
                }
                if (splashSaveWarningObject && gameManager.viewingSaveWarning)
                {
                    Utils.LocalizeText(gameManager, ref languageClassName, ref splashSaveWarningText, "splashSaveWarningText", defaultFont, chineseFont, japaneseFont);
                }
            }

            if (gameManager.inMainMenu)
            {
                Utils.LocalizeText(gameManager, ref languageClassName, ref mainMenuContinueButtonText, "mainMenuContinueButtonText", defaultFont, chineseFont, japaneseFont);                
            }

            if (gameManager.inOptionsGameplayMenu)
            {
                Utils.LocalizeText(gameManager, ref languageClassName, ref optionsTitle, "optionsGameplaySectionTitle", defaultFont, chineseFont, japaneseFont);
            }
            else if(gameManager.inOptionsVideoMenu)
            {
                Utils.LocalizeText(gameManager, ref languageClassName, ref optionsTitle, "optionsVideoSectionTitle", defaultFont, chineseFont, japaneseFont);
            }            

            gameManager.localizeLanguage = false;
            Utils.ClearMemory();
        }
    }

    private void Loading()
    {
        if (gameManager.loading)
        {
            if (loadingProgress)
            {
                if (!loadingProgress.enabled)
                {
                    loadingProgress.enabled = true;
                }
                loadingProgress.text = string.Format(Strings.loadingProgressFormat, percentageCompleted);
            }

            if (loadingIcon)
            {
                if (!loadingIcon.activeSelf)
                {
                    loadingIcon.SetActive(true);
                }
                if (loadingIcon)
                {
                    if (loadingIcon.activeSelf)
                    {
                        loadingIcon.transform.Rotate(0, 0, loadingIconRotationSpeed * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            if (loadingIcon)
            {
                if (loadingIcon.activeSelf)
                {
                    loadingIcon.SetActive(false);
                }
            }
        }
    }

    #region UI
    private void ActivateUI(bool targetMenu, CanvasGroup canvasGroup, GameObject parentObject, Animator animator)
    {
        if (targetMenu)
        {
            if (parentObject)
            {
                if (!parentObject.activeSelf)
                {
                    parentObject.SetActive(true);
                    Utils.ClearMemory();
                }
            }

            if (canvasGroup)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }         
        }
        else
        {
            if (animator)
            {
                if (canvasGroup)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }

                if (parentObject.activeSelf)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0f && animator.GetCurrentAnimatorStateInfo(1).normalizedTime <= 0f && animator.GetCurrentAnimatorStateInfo(2).normalizedTime <= 0f)
                    {
                        if (parentObject == optionsMenuGameobject)
                        {
                            if (parentObject.activeSelf && !optionsGameplaySectionObject.activeSelf && !optionsVideoSectionObject.activeSelf)
                            {
                                parentObject.SetActive(false);                                
                            }
                        }
                        else if(parentObject == diagnosticsObject)
                        {
                            if (parentObject.activeSelf && !diagnosticsFPSGameobject.activeSelf && !diagnosticsPingGameobject.activeSelf && diagnosticsMemoryGameobject.activeSelf)
                            {
                                parentObject.SetActive(false);                                
                            }
                        }
                        else
                        {
                            if (parentObject.activeSelf)
                            {
                                parentObject.SetActive(false);                                
                            }
                        }
                        Utils.ClearMemory();
                    }
                }
            }
            else
            {
                if (canvasGroup)
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }

                if (parentObject)
                {
                    if (parentObject.activeSelf)
                    {
                        parentObject.SetActive(false);
                        Utils.ClearMemory();
                    }
                }
            }
        }
        
    }

    private void AnimateUI(bool trigger, Animator animator, int layer, int animationName)
    {
        if (trigger)
        {
            if (animator == creditsAnimator && animator.isActiveAndEnabled)
            {
                if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 0.97f)
                {
                    creditsFadeTimer += Time.deltaTime;
                    if (creditsFadeTimer > 7f)
                    {
                        creditsFadeTimer = 0f;
                        gameManager.MainMenu();
                    }
                }
            }

            if (animator.isActiveAndEnabled)
            {
                if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f)
                {
                    if (layer == 0)
                    {
                        animator.SetFloat(Strings.animUISpeed, 1);
                    }
                    else if (layer == 1)
                    {
                        animator.SetFloat(Strings.animUIAlphaSpeed, 1);
                    }
                    else if (layer == 2)
                    {
                        animator.SetFloat(Strings.animUIScaleSpeed, 1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1f)
                {
                    if (layer == 0)
                    {
                        animator.SetFloat(Strings.animUISpeed, 0);
                    }
                    else if (layer == 1)
                    {
                        animator.SetFloat(Strings.animUIAlphaSpeed, 0);
                    }
                    else if (layer == 2)
                    {
                        animator.SetFloat(Strings.animUIScaleSpeed, 0);
                    }
                }
            }
        }
        else
        {
            if (animator == creditsAnimator || animator == versionCanvasGroupAnimator || animator == mainMenuAnimator && gameManager.viewingCredits || animator == optionsGameplaySectionAnimator && gameManager.inOptionsVideoMenu || animator == optionsVideoSectionAnimator && gameManager.inOptionsGameplayMenu)
            {
                if (animator.isActiveAndEnabled)
                {
                    ResetAnimation(animator, layer, animationName);
                }                
            }

            if (animator.isActiveAndEnabled)
            {
                if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime > 0f)
                {
                    if (layer == 0)
                    {
                        animator.SetFloat(Strings.animUISpeed, -1);
                    }
                    else if (layer == 1)
                    {
                        animator.SetFloat(Strings.animUIAlphaSpeed, -1);
                    }
                    else if (layer == 2)
                    {
                        animator.SetFloat(Strings.animUIScaleSpeed, -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime <= 0f)
                {
                    if (layer == 0)
                    {
                        animator.SetFloat(Strings.animUISpeed, 0);
                    }
                    else if (layer == 1)
                    {
                        animator.SetFloat(Strings.animUIAlphaSpeed, 0);
                    }
                    else if (layer == 2)
                    {
                        animator.SetFloat(Strings.animUIScaleSpeed, 0);
                    }
                }
            }
        }
    }

    void onUICancel(InputAction.CallbackContext started)
    {
        if (gameManager.inOptionsMenu || gameManager.viewingCredits || gameManager.viewingPatchNotes)
        {
            gameManager.MainMenu();
            creditsFadeTimer = 0f;
        }
    }

    private void ResetAnimation(Animator animator, int layer, int animationName)
    {        
        animator.Play(animationName, layer, 0f);// Play the animation at the start (normalized time 0.0f)        
        animator.Update(0f);// Immediately update the animator to apply the change
    }
    #endregion

    #region Cursor
    public void ActivateCursor()
    {
        if (cursorCanvasGroup.alpha != 1)
        {
            cursorCanvasGroup.alpha = 1;
            Cursor.visible = false;
        }

        if (gameManager.usingKeyboard && cursorRenderer.texture != pcCursorTexture)
        {
            cursorRenderer.texture = pcCursorTexture;
        }
        else if (gameManager.usingController && cursorRenderer.texture != controllerCursorTexture)
        {
            cursorRenderer.texture = controllerCursorTexture;
        }

        if (cursorCanvasGroup)
        {
            cursorCanvasGroup.interactable = true;
            cursorCanvasGroup.blocksRaycasts = true;
        }     
    }

    public void DeActivateCursor()
    {
        if (cursorCanvasGroup.alpha != 0)
        {
            cursorCanvasGroup.alpha = 0;
            Cursor.visible = false;
        }

        if (cursorCanvasGroup)
        {
            cursorCanvasGroup.interactable = false;
            cursorCanvasGroup.blocksRaycasts = false;
        }       
    }
    #endregion

    #region Options Sections
    public void OnOptionsDescriptionExit()
    {
        if (optionsDescription.text != null)
        {
            if (gameManager.inOptionsGameplayMenu)
            {
                optionsDescription.text = null;
            }
        }
    }

    public void OnOptionsOnlineVisibilityDescriptionEnter()
    {
        if (optionsDescription.text != "Who is able to see your game in the lobby and join.")
        {
            optionsDescription.text = "Who is able to see your game in the lobby and join.";
        }
    }
    public void OnOptionsAutosaveDescriptionEnter()
    {
        if (optionsDescription.text != "Allow the game to save at certain points.")
        {
            optionsDescription.text = "Allow the game to save at certain points.";
        }        
    }
    public void OnOptionsMaximumAutosaveDescriptionEnter()
    {
        if (optionsDescription.text != "The maximum number of Autosaves the game will make. The game will then overwrite the oldest Autosave each time.")
        {
            optionsDescription.text = "The maximum number of Autosaves the game will make. The game will then overwrite the oldest Autosave each time.";
        }
    }
    public void OnOptionsMaximumQuicksaveDescriptionEnter()
    {
        if (optionsDescription.text != "The maximum number of Quicksaves the game will make. The game will then overwrite the oldest Quicksave each time.")
        {
            optionsDescription.text = "The maximum number of Quicksaves the game will make. The game will then overwrite the oldest Quicksave each time.";
        }
    }
    public void OnOptionsGoreDescriptionEnter()
    {
        if (optionsDescription.text != "The game will not display blood. This will not affect certain chosen glitches.")
        {
            optionsDescription.text = "The game will not display blood. This will not affect certain chosen glitches.";
        }
    }

    public void OnOptionsShareCinematicsDescriptionEnter()
    {
        if (optionsDescription.text != "Allow other players to view your cinematics if they choose to.")
        {
            optionsDescription.text = "Allow other players to view your cinematics if they choose to.";
        }
    }
    #endregion

    #region Localization
    public void SplashLanguageSelectDropdown()
    {
        if (splashLanguageDropdownObject)
        {
            if (!splashLanguageDropdownObject.activeSelf)
            {
                splashLanguageDropdownObject.SetActive(true);

                if (splashEnglishButtonObject && GameStrings.GameStringsEnglish.available)
                {
                    splashEnglishButtonObject.SetActive(true);
                }
                if (splashFrenchButtonObject && GameStrings.GameStringsFrench.available)
                {
                    splashFrenchButtonObject.SetActive(true);
                }
                if (splashGermanButtonObject && GameStrings.GameStringsGerman.available)
                {
                    splashGermanButtonObject.SetActive(true);
                }
                if (splashItalianButtonObject && GameStrings.GameStringsItalian.available)
                {
                    splashItalianButtonObject.SetActive(true);
                }
                if (splashSpanishButtonObject && GameStrings.GameStringsItalian.available)
                {
                    splashSpanishButtonObject.SetActive(true);
                }
                if (splashAfricanButtonObject && GameStrings.GameStringsAmerican.available)
                {
                    splashAfricanButtonObject.SetActive(true);
                }
                if (splashRussianButtonObject && GameStrings.GameStringsRussian.available)
                {
                    splashRussianButtonObject.SetActive(true);
                }
                if (splashChineseButtonObject && GameStrings.GameStringsChinese.available)
                {
                    splashChineseButtonObject.SetActive(true);
                }
                if (splashJapaneseButtonObject && GameStrings.GameStringsJapanese.available)
                {
                    splashJapaneseButtonObject.SetActive(true);
                }
                if (splashBrazilianButtonObject && GameStrings.GameStringsBrazilian.available)
                {
                    splashBrazilianButtonObject.SetActive(true);
                }
            }
            else
            {
                splashLanguageDropdownObject.SetActive(false);
            }
        }
    }
    #endregion
}
