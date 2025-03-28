using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] private InputSystemUIInputModule uiInput;
    [SerializeField] private Scrollbar activeScrollbar;
    public InputAction scrollAction; // Accessed by inputManager
    private Vector2 scrollDelta;
    private float scrollAmount = 0f;
    private string inputText;
    [Header("-------------------------------------------------------------------")]
    [SerializeField] private GameObject mainMenu;

    [System.Serializable]
    public class OptionsUI
    {        
        public GameObject optionsMenu;
        public Animator optionsMenuAnimator;
        [Space(5)]
        public TextMeshProUGUI confirmOptionsChangesTimerText;
        public GameObject confirmOptionsChanges;
        [Space(10)]
        public TextMeshProUGUI optionsTitle;
        public TextMeshProUGUI optionsDescription;
        public TextMeshProUGUI optionsDescriptionAdditional;
        [Header("---- Gameplay ----")]
        public GameObject optionsGameplayMenu;
        public Image optionsGameplayMenuButtonBG;
        public UI optionsGameplayMenuButtonUIScript;
        [Space(5)]
        [Space(5)]
        public Toggle autosavesToggle;
        [Space(5)]
        public TextMeshProUGUI maximumAutosavesSliderText;
        public Slider maximumAutosavesSlider;
        [Space(5)]
        public TextMeshProUGUI maximumQuicksavesSliderText;
        public Slider maximumQuicksavesSlider;
        [Space(5)]
        public Toggle goreToggle;
        [Header("---- Video ----")]
        public GameObject optionsVideoMenu;
        public Image optionsVideoMenuButtonBG;
        public UI optionsVideoMenuButtonUIScript;
        [Space(5)]
        public GameObject optionsVideoPage1;
        public GameObject optionsVideoPage1Tab;
        [Space(5)]
        public GameObject optionsVideoPage2;
        public GameObject optionsVideoPage2Tab;
        [Space(5)]
        public TextMeshProUGUI gpuName;
        [Space(5)]
        public TMP_Dropdown displayDevicesDropdown;
        [Space(5)]
        public TMP_Dropdown resolutionsDropdown;
        [Space(5)]
        public TMP_Dropdown displayModeDropdown;
        [Space(5)]
        public GameObject framerateCapEffect;
        public TextMeshProUGUI frameRateCapSliderText;
        public Toggle frameRateCapToggle;
        public Slider frameRateCapSlider;
        [Space(5)]
        public GameObject vSyncEffect;
        public TMP_Dropdown vSyncDropdown;
        [Space(5)]
        public TMP_Dropdown qualityDropdown;
        [Space(5)]
        public GameObject antiAliasEffect;
        public TMP_Dropdown antiAliasDropdown;
        public TMP_Dropdown taaQualityDropdown;
        [Space(5)]
        public GameObject fogEffect;
        public TMP_Dropdown fogDropdown;
        [Space(5)]
        public TextMeshProUGUI fovSliderText;
        public Slider fovSlider;
        [Space(5)]
        public TMP_InputField renderDistanceSliderText;
        public Slider renderDistanceSlider;
        [Space(5)]
        public GameObject bloomEffect;
        public TMP_Dropdown bloomDropdown;
        [Space(5)]
        public GameObject ansioEffect;
        public TMP_Dropdown ansioDropdown;
        [Space(5)]
        public GameObject hdrEffect;
        public Toggle hdrToggle;
        [Space(5)]
        public TMP_Dropdown tonemappingDropdown;
        [Space(5)]
        public GameObject globalIlluminationEffect;
        public TMP_Dropdown giDropdown;
        public Toggle giResolutionToggle;
        [Space(5)]
        public GameObject planarReflectionEffect;
        public TMP_Dropdown reflectionsDropdown;
        public Toggle planarReflectionsToggle;
        [Space(5)]
        public TMP_Dropdown shadowQualityDropdown;
        [Space(5)]
        public TextMeshProUGUI shadowDistanceSliderText;
        public Slider shadowDistanceSlider;
        [Space(5)]
        public GameObject diagnosticsEffect;
        public GameObject fpsEffect;
        public GameObject fpsCounter;
        public TextMeshProUGUI fpsCounterText;
        public Animator fpsCounterAnimator;
        public GameObject fpsCounterEffect;
        public Toggle fpsCounterToggle;
        [Space(5)]
        public GameObject memoryEffect;
        public GameObject memoryCounter;
        public TextMeshProUGUI memoryCounterText;
        public Animator memoryCounterAnimator;
        public GameObject memoryCounterEffect;
        public Toggle memoryCounterToggle;
        [Header("---- Audio ----")]
        public GameObject optionsAudioMenu;
        public Image optionsAudioMenuButtonBG;
        public UI optionsAudioMenuButtonUIScript;
        [Space(5)]
        public TextMeshProUGUI audioMasterNumberText;
        public Slider audioMasterSlider;
        [Space(5)]
        public TextMeshProUGUI audioMusicNumberText;
        public Slider audioMusicSlider;
        [Space(5)]
        public TextMeshProUGUI audioAmbientNumberText;
        public Slider audioAmbientSlider;
        [Space(5)]
        public TextMeshProUGUI audioSFXNumberText;
        public Slider audioSFXSlider;
        [Space(5)]
        public TextMeshProUGUI audioUINumberText;
        public Slider audioUISlider;
        [Space(5)]
        public TextMeshProUGUI audioVoiceNumberText;
        public Slider audioVoiceSlider;
        [Space(5)]
        public TextMeshProUGUI audioEventNumberText;
        public Slider audioEventSlider;
        [Header("---- Keyboard ----")]
        public GameObject optionsKeyboardMenu;
        public Image optionsKeyboardMenuButtonBG;
        public UI optionsKeyboardMenuButtonUIScript;
        [Space(5)]
        public TextMeshProUGUI cameraZoomSensitivitySliderText;
        public Slider cameraZoomSensitivitySlider;
        [Space(5)]
        public TextMeshProUGUI cameraMoveSensitivitySliderText;
        public Slider cameraMoveSensitivitySlider;
        [Space(5)]
        public TextMeshProUGUI cameraPanSensitivitySliderText;
        public Slider cameraPanSensitivitySlider;
        [Header("---- Gamepad ----")]
        public GameObject optionsGamepadMenu;
        public Image optionsGamepadMenuButtonBG;
        public UI optionsGamepadMenuButtonUIScript;
        [Space(5)]
        public TextMeshProUGUI cameraGamepadZoomSensitivitySliderText;
        public Slider cameraGamepadZoomSensitivitySlider;
        [Space(5)]
        public TextMeshProUGUI cameraGamepadMoveSensitivitySliderText;
        public Slider cameraGamepadMoveSensitivitySlider;
        [Space(5)]
        public TextMeshProUGUI cameraGamepadPanSensitivitySliderText;
        public Slider cameraGamepadPanSensitivitySlider;
        [Space(5)]
        public TextMeshProUGUI cameraGamepadZoomDeadzoneSliderText;
        public Slider cameraGamepadZoomDeadzoneSlider;
        [Space(5)]
        public TextMeshProUGUI cameraGamepadMoveDeadzoneSliderText;
        public Slider cameraGamepadMoveDeadzoneSlider;
        [Space(5)]
        public TextMeshProUGUI cameraGamepadPanDeadzoneSliderText;
        public Slider cameraGamepadPanDeadzoneSlider;
        [Header("---- Interface ----")]
        public GameObject optionsInterfaceMenu;
        public Image optionsInterfaceMenuButtonBG;
        public UI optionsInterfaceMenuButtonUIScript;
        [Space(5)]
        [Header("---- Accessibility ----")]
        public GameObject optionsAccessibilityMenu;
        public Image optionsAccessibilityMenuButtonBG;
        public UI optionsAccessibilityMenuButtonUIScript;
    }
    public OptionsUI optionsUI;


    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.uiManager = this;
    }

    private void Update()
    {
        OptionsScrollbar();
        Options();
    }    

    private void Options()
    {
        if (gameManager.inOptionsGameplayMenu)
        {
            if (optionsUI.optionsGameplayMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsGameplaySectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsGameplaySectionTitle;
            }
        }
        else if (gameManager.inOptionsVideoMenu)
        {
            if (optionsUI.optionsVideoMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsVideoSectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsVideoSectionTitle;
            }

            if (gameManager.scripts.optionsManager.selectedFogIndex != 0)
            {
                Utils.ActivateObject(optionsUI.fogEffect, true);
            }
            else if (optionsUI.fogEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.fogEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedAnsioIndex != 0)
            {
                Utils.ActivateObject(optionsUI.ansioEffect, true);
            }
            else if (optionsUI.ansioEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.ansioEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedAntiAliasIndex != 0)
            {
                Utils.ActivateObject(optionsUI.antiAliasEffect, true);
            }
            else if (optionsUI.antiAliasEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.antiAliasEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedVsyncIndex != 0)
            {
                Utils.ActivateObject(optionsUI.vSyncEffect, true);
            }
            else if (optionsUI.vSyncEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.vSyncEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedBloomIndex != 0)
            {
                Utils.ActivateObject(optionsUI.bloomEffect, true);
            }
            else if (optionsUI.bloomEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.bloomEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedPlanarReflections)
            {
                Utils.ActivateObject(optionsUI.planarReflectionEffect, true);
            }
            else if (optionsUI.planarReflectionEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.planarReflectionEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedHDR)
            {
                Utils.ActivateObject(optionsUI.hdrEffect, true);
            }
            else if (optionsUI.hdrEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.hdrEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedFPSCounter || gameManager.scripts.optionsManager.selectedMemoryCounter)
            {
                Utils.ActivateObject(optionsUI.diagnosticsEffect, true);
            }
            else if (optionsUI.diagnosticsEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.diagnosticsEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedFPSCounter)
            {
                Utils.ActivateObject(optionsUI.fpsEffect, true);
            }
            else if (optionsUI.fpsEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.fpsEffect, false);
            }

            if (gameManager.scripts.optionsManager.selectedMemoryCounter)
            {
                Utils.ActivateObject(optionsUI.memoryEffect, true);
            }
            else if (optionsUI.memoryEffect.activeSelf)
            {
                Utils.ActivateObject(optionsUI.memoryEffect, false);
            }
        }
        else if (gameManager.inOptionsAudioMenu)
        {
            if (optionsUI.optionsAudioMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsAudioSectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsAudioSectionTitle;
            }
        }
        else if (gameManager.inOptionsKeyboardMenu)
        {
            if (optionsUI.optionsKeyboardMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsKeyboardSectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsKeyboardSectionTitle;
            }
        }
        else if (gameManager.inOptionsGamepadMenu)
        {
            if (optionsUI.optionsGamepadMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsGamepadSectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsGamepadSectionTitle;
            }
        }
        else if (gameManager.inOptionsInterfaceMenu)
        {
            if (optionsUI.optionsInterfaceMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsInterfaceSectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsInterfaceSectionTitle;
            }
        }
        else if (gameManager.inOptionsAccessibilityMenu)
        {
            if (optionsUI.optionsAccessibilityMenuButtonBG.color != Colors.red)
            {
                Utils.TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, true);
                Utils.TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                Utils.TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsAccessibilitySectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsAccessibilitySectionTitle;
            }
        }
    }

    public void OnRenderDistanceInputField(string input)
    {
        inputText = input;
        optionsUI.renderDistanceSliderText.text = inputText;
    }


    public void ScrollbarActive()
    {
        Scrollbar[] scrollbars = optionsUI.optionsMenu.GetComponentsInChildren<Scrollbar>(true);
        foreach (Scrollbar scrollbar in scrollbars)
        {
            if (scrollbar.gameObject.activeInHierarchy)
            {
                activeScrollbar = scrollbar;
                scrollbars = null;
                return;
            }
        }
    }
    private void OptionsScrollbar()
    {
        if (uiInput != null)
        {
            if (gameManager.scripts.inputManager.keyboard)
            {
                scrollDelta = uiInput.input.mouseScrollDelta;

                if (scrollDelta.y != 0)
                {
                    scrollAmount = scrollDelta.y * gameManager.scripts.optionsManager.selectedMouseScrollSensitivity;
                }
                else
                {
                    scrollAmount = 0f;
                }
            }
            else if (gameManager.scripts.inputManager.gamepad)
            {
                scrollDelta = scrollAction.ReadValue<Vector2>();

                if (scrollDelta.y > gameManager.scripts.optionsManager.appliedGamepadCameraZoomDeadzone || scrollDelta.y < -gameManager.scripts.optionsManager.appliedGamepadCameraZoomDeadzone)
                {
                    scrollAmount = scrollDelta.y * gameManager.scripts.optionsManager.selectedMouseScrollSensitivity;
                }
                else
                {
                    scrollAmount = 0f;
                }
            }

            if (activeScrollbar)
            {
                activeScrollbar.value += scrollAmount;
            }
        }
    }

    public void ClickedOptions()
    {        
        Utils.ActivateObject(mainMenu, false);
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDescInitial;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescInitialAdditional;
        Utils.ActivateObject(optionsUI.optionsMenu, true);
        optionsUI.optionsMenuAnimator.Play(Strings.on);
    }
    public void ClickedExitOptions()
    {        
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsGameplayMenu, true);
        optionsUI.optionsMenuAnimator.Play(Strings.off);
    }
    public void CloseOptions()
    {
        Utils.ActivateObject(optionsUI.optionsMenu, false);
        gameManager.inOptionsMenu = false;
        Utils.ActivateObject(mainMenu, true);
    }
    public void ClickedGameplayOptions()
    {
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);        
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);        
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsGameplayMenu, true);
        gameManager.inOptionsGameplayMenu = true;
    }
    public void ClickedVideoOptions()
    {
        Utils.ActivateObject(optionsUI.optionsGameplayMenu, false);
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        Utils.ActivateObject(optionsUI.optionsVideoPage2, false);
        Utils.ActivateObject(optionsUI.optionsVideoPage2Tab, false);
        gameManager.inOptionsGameplayMenu = false;        
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsVideoPage1Tab, true);
        Utils.ActivateObject(optionsUI.optionsVideoPage1, true);        
        Utils.ActivateObject(optionsUI.optionsVideoMenu, true);
        gameManager.inOptionsVideoMenu = true;
    }
    public void ClickedAudioOptions()
    {
        Utils.ActivateObject(optionsUI.optionsGameplayMenu, false);
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;        
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsAudioMenu, true);
        gameManager.inOptionsAudioMenu = true;
    }
    public void ClickedKeyboardOptions()
    {
        Utils.ActivateObject(optionsUI.optionsGameplayMenu, false);
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;        
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, true);
        gameManager.inOptionsKeyboardMenu = true;
    }
    public void ClickedGamepadOptions()
    {
        Utils.ActivateObject(optionsUI.optionsGameplayMenu, false);
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;        
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsGamepadMenu, true);
        gameManager.inOptionsGamepadMenu = true;
    }
    public void ClickedInterfaceOptions()
    {
        Utils.ActivateObject(optionsUI.optionsGameplayMenu, false);
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;        
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, true);
        gameManager.inOptionsInterfaceMenu = true;
    }
    public void ClickedAccessibilityOptions()
    {
        Utils.ActivateObject(optionsUI.optionsGameplayMenu, false);
        Utils.ActivateObject(optionsUI.optionsVideoMenu, false);
        Utils.ActivateObject(optionsUI.optionsAudioMenu, false);
        Utils.ActivateObject(optionsUI.optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsUI.optionsGamepadMenu, false);
        Utils.ActivateObject(optionsUI.optionsInterfaceMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;        

        Utils.ActivateObject(optionsUI.optionsAccessibilityMenu, true);
        gameManager.inOptionsAccessibilityMenu = true;
    }

    public void CloseFPSCounter()
    {
        Utils.ActivateObject(optionsUI.fpsCounter, false);
    }
    public void CloseMemoryCounter()
    {
        Utils.ActivateObject(optionsUI.memoryCounter, false);
    }

    // TEXT
    public void OnTextInputHover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsTextInputDesc;
    }
    public void OnActivateSettingHover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsActivateSetting;
    }
    public void OnInvertSettingHover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsInvertSetting;
    }
    public void OnModifiedSettingHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsModifiedSetting;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnRevertSettingHover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsRevertSetting;
    }
    public void OnDescClear()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDescClear;
        gameManager.scripts.uiManager.optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnPageIncreaseHover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsNextPage;
        gameManager.scripts.uiManager.optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnPageDecreaseHover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPreviousPage;
        gameManager.scripts.uiManager.optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnOptionsPage1Hover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPage1Desc;
    }
    public void OnOptionsPage2Hover()
    {
        gameManager.scripts.uiManager.optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPage2Desc;
    }
    public void OnRaytracingInfoHover()
    {
        if (gameManager.scripts.optionsManager.rayTracingSupported)
        {
            gameManager.scripts.uiManager.optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.raytracingOnDesc;
        }
        else
        {
            gameManager.scripts.uiManager.optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.raytracingOffDesc;
        }
    }

    public void OnAutosavesHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAutosavesDesc;
    }
    public void OnMaximumAutosavesHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsMaximumAutosavesDesc;
    }
    public void OnMaximumQuicksavesHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsMaximumQuicksavesDesc;
    }
    public void OnGoreHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGoreDesc;
    }

    public void OnDisplayDeviceHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDisplayDeviceDesc;
    }
    public void OnDisplayAdapterHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDisplayAdapterDesc;
    }
    public void OnResolutionsHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsResolutionDesc;
    }
    public void OnDisplayModeHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDisplayModeDesc;
    }
    public void OnVSyncHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsVSyncDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsVSyncDescAdditional;
    }
    public void OnFramerateCapHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsFramerateCapDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFramerateCapDescAdditional;
    }
    public void OnFramerateCapInfoHover()
    {
        gameManager.scripts.optionsManager.refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;

        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFramerateCapInfoDesc + "<color=" + Colors.textCyan + ">" + gameManager.scripts.optionsManager.refreshRate.ToString("F0") + " Hz</color>.";
    }
    public void OnQualityHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsQualityDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsQualityDescAdditional;
    }
    public void OnAntiAliasHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAntiAliasDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsAntiAliasDescAdditional;
    }
    public void OnTaaQualityHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsTaaQualityDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsTaaQualityDescAdditional;
    }
    public void OnVolumetricFogHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsVolumetricFogDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsVolumetricFogDescAdditional;
    }
    public void OnFOVHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsFOVDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFOVDescAdditional;
    }
    public void OnRenderDistanceHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsRenderDistanceDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnBloomHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsBloomDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsBloomDescAdditional;
    }
    public void OnHDRHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsHDRDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsHDRDescAdditional;
    }
    public void OnAnsioHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAnsioDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsAnsioDescAdditional;
    }
    public void OnTonemappingHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsTonemappingDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsTonemappingDescAdditional;
    }
    public void OnGlobalIlluminationHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDescAdditional;
    }
    public void OnGlobalIlluminationResHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResDesc;
        if (gameManager.scripts.optionsManager.selectedGlobalIlluminationFullRes)
        {
            optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResStatusFullDesc;
        }
        else
        {
            optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResStatusHalfDesc;
        }
    }
    public void OnGlobalIlluminationResDescHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResStatusDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResDescAdditional;
    }
    public void OnGlobalIlluminationInfoHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDesc;
        OnRaytracingInfoHover();
    }
    public void OnReflectionsHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsReflectionsDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsReflectionsDescAdditional;
    }
    public void OnReflectionsPlanarHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPlanarReflectionsDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsPlanarReflectionsDescAdditional;
    }
    public void OnShadowQualityHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsShadowQualityDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsShadowQualityDescAdditional;
    }
    public void OnShadowDistanceHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsShadowDistanceDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnDiagnosticsHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDiagnosticsDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnFPSHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsFPSDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnMemoryHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsMemoryDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
}
