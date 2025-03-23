using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    public GameObject mainMenu;

    [System.Serializable]
    public class OptionsUI
    {        
        public GameObject optionsMenu;
        public Animator optionsMenuAnimator;
        [Space(5)]
        public TextMeshProUGUI confirmOptionsChangesTimerText;
        public GameObject confirmOptionsChanges;
        [Space(10)]
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
        public TMP_Dropdown vSyncDropdown;
        [Space(5)]
        public TMP_Dropdown qualityDropdown;
        [Space(5)]
        public TMP_Dropdown antiAliasDropdown;
        public TMP_Dropdown taaQualityDropdown;
        [Space(5)]
        public TMP_Dropdown fogDropdown;
        [Space(5)]
        public TextMeshProUGUI fovSliderText;
        public Slider fovSlider;
        [Space(5)]
        public TextMeshProUGUI renderDistanceSliderText;
        public Slider renderDistanceSlider;
        [Space(5)]
        public TMP_Dropdown bloomDropdown;
        [Space(5)]
        public TMP_Dropdown ansioDropdown;
        [Space(5)]
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
        public GameObject fpsCounter;
        public TextMeshProUGUI fpsCounterText;
        public Animator fpsCounterAnimator;
        public GameObject fpsCounterEffect;
        public Toggle fpsCounterToggle;
        [Space(5)]
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
    public void ExitOptions()
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
}
