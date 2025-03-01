using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    public GameObject mainMenu;
    [Header("---- Options ----------------------------------------------------")]
    public GameObject optionsMenu;
    public GameObject optionsGameplayMenu;
    public GameObject optionsVideoMenu;
    public GameObject optionsAudioMenu;
    public GameObject optionsKeyboardMenu;
    public GameObject optionsGamepadMenu;
    public GameObject optionsInterfaceMenu;
    public GameObject optionsAccessibilityMenu;
    [Space(10)]
    public TextMeshProUGUI confirmOptionsChangesTimerText;// ACCESSED BY OPTIONS
    public GameObject confirmOptionsChanges;
    [Space(10)]
    public TextMeshProUGUI optionsDescription;// ACCESSED BY OPTIONS
    public TextMeshProUGUI optionsDescriptionAdditional;// ACCESSED BY OPTIONS
    [Header("---- Gameplay ----")]
    public GameObject gameplayButton;
    [Space(5)]
    public Toggle autosavesToggle;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI maximumAutosavesSliderText;// ACCESSED BY OPTIONS
    public Slider maximumAutosavesSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI maximumQuicksavesSliderText;// ACCESSED BY OPTIONS
    public Slider maximumQuicksavesSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public Toggle goreToggle;// ACCESSED BY OPTIONS
    [Header("---- Video ----")]
    public GameObject optionsVideoPage1;
    public GameObject optionsVideoPage1Tab;
    [Space(5)]
    public GameObject optionsVideoPage2;
    public GameObject optionsVideoPage2Tab;
    [Space(5)]
    public TextMeshProUGUI gpuName;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown displayDevicesDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown resolutionsDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown displayModeDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public GameObject framerateCapEffect;
    public TextMeshProUGUI frameRateCapSliderText;// ACCESSED BY OPTIONS
    public Toggle frameRateCapToggle;// ACCESSED BY OPTIONS
    public Slider frameRateCapSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown vSyncDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown qualityDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown antiAliasDropdown;// ACCESSED BY OPTIONS    
    public TMP_Dropdown taaQualityDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown fogDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI fovSliderText;// ACCESSED BY OPTIONS
    public Slider fovSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI renderDistanceSliderText;// ACCESSED BY OPTIONS
    public Slider renderDistanceSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown bloomDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown ansioDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public Toggle hdrToggle;// ACCESSED BY OPTIONS
    [Space(5)]
    public TMP_Dropdown tonemappingDropdown;// ACCESSED BY OPTIONS
    [Space(5)]
    public GameObject globalIlluminationEffect;
    public TMP_Dropdown giDropdown;// ACCESSED BY OPTIONS
    public GameObject giCandleImage;// ACCESSED BY OPTIONS
    public Toggle giResolutionToggle;// ACCESSED BY OPTIONS
    [Space(5)]
    public GameObject planarReflectionEffect;
    public TMP_Dropdown reflectionsDropdown;// ACCESSED BY OPTIONS
    public Toggle planarReflectionsToggle;// ACCESSED BY OPTIONS
    [Header("---- Audio ----")]
    public TextMeshProUGUI audioMasterNumberText;// ACCESSED BY OPTIONS
    public Slider audioMasterSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI audioMusicNumberText;// ACCESSED BY OPTIONS
    public Slider audioMusicSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI audioAmbientNumberText;// ACCESSED BY OPTIONS
    public Slider audioAmbientSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI audioSFXNumberText;// ACCESSED BY OPTIONS
    public Slider audioSFXSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI audioUINumberText;// ACCESSED BY OPTIONS
    public Slider audioUISlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI audioVoiceNumberText;// ACCESSED BY OPTIONS
    public Slider audioVoiceSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI audioEventNumberText;// ACCESSED BY OPTIONS
    public Slider audioEventSlider;// ACCESSED BY OPTIONS
    [Header("---- Keyboard ----")]
    public TextMeshProUGUI cameraZoomSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraZoomSensitivitySlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraMoveSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraMoveSensitivitySlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraPanSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraPanSensitivitySlider;// ACCESSED BY OPTIONS
    [Header("---- Gamepad ----")]
    public TextMeshProUGUI cameraGamepadZoomSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadZoomSensitivitySlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraGamepadMoveSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadMoveSensitivitySlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraGamepadPanSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadPanSensitivitySlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraGamepadZoomDeadzoneSliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadZoomDeadzoneSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraGamepadMoveDeadzoneSliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadMoveDeadzoneSlider;// ACCESSED BY OPTIONS
    [Space(5)]
    public TextMeshProUGUI cameraGamepadPanDeadzoneSliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadPanDeadzoneSlider;// ACCESSED BY OPTIONS

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.uiManager = this;
    } 

    public void ClickedOptions()
    {
        EventSystem.current.SetSelectedGameObject(gameplayButton);
        Utils.ActivateObject(mainMenu, false);
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDescInitial;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescInitialAdditional;
        Utils.ActivateObject(optionsMenu, true);        
    }
    public void ExitOptions()
    {        
        Utils.ActivateObject(optionsVideoMenu, false);
        Utils.ActivateObject(optionsAudioMenu, false);
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsGameplayMenu, true);
        Utils.ActivateObject(optionsMenu, false);
        gameManager.inOptionsMenu = false;
        Utils.ActivateObject(mainMenu, true);
    }

    public void ClickedGameplayOptions()
    {
        Utils.ActivateObject(optionsVideoMenu, false);        
        Utils.ActivateObject(optionsAudioMenu, false);        
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsGameplayMenu, true);
        gameManager.inOptionsGameplayMenu = true;
    }
    public void ClickedVideoOptions()
    {
        Utils.ActivateObject(optionsGameplayMenu, false);
        Utils.ActivateObject(optionsAudioMenu, false);
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        Utils.ActivateObject(optionsVideoPage2, false);
        Utils.ActivateObject(optionsVideoPage2Tab, false);
        gameManager.inOptionsGameplayMenu = false;        
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsVideoPage1Tab, true);
        Utils.ActivateObject(optionsVideoPage1, true);        
        Utils.ActivateObject(optionsVideoMenu, true);
        gameManager.inOptionsVideoMenu = true;
    }

    public void ClickedAudioOptions()
    {
        Utils.ActivateObject(optionsGameplayMenu, false);
        Utils.ActivateObject(optionsVideoMenu, false);
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;        
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsAudioMenu, true);
        gameManager.inOptionsAudioMenu = true;
    }
    public void ClickedKeyboardOptions()
    {
        Utils.ActivateObject(optionsGameplayMenu, false);
        Utils.ActivateObject(optionsVideoMenu, false);
        Utils.ActivateObject(optionsAudioMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;        
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsKeyboardMenu, true);
        gameManager.inOptionsKeyboardMenu = true;
    }
    public void ClickedGamepadOptions()
    {
        Utils.ActivateObject(optionsGameplayMenu, false);
        Utils.ActivateObject(optionsVideoMenu, false);
        Utils.ActivateObject(optionsAudioMenu, false);
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsInterfaceMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;        
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsGamepadMenu, true);
        gameManager.inOptionsGamepadMenu = true;
    }

    public void ClickedInterfaceOptions()
    {
        Utils.ActivateObject(optionsGameplayMenu, false);
        Utils.ActivateObject(optionsVideoMenu, false);
        Utils.ActivateObject(optionsAudioMenu, false);
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsAccessibilityMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;        
        gameManager.inOptionsAccessibilityMenu = false;

        Utils.ActivateObject(optionsInterfaceMenu, true);
        gameManager.inOptionsInterfaceMenu = true;
    }

    public void ClickedAccessibilityOptions()
    {
        Utils.ActivateObject(optionsGameplayMenu, false);
        Utils.ActivateObject(optionsVideoMenu, false);
        Utils.ActivateObject(optionsAudioMenu, false);
        Utils.ActivateObject(optionsKeyboardMenu, false);
        Utils.ActivateObject(optionsGamepadMenu, false);
        Utils.ActivateObject(optionsInterfaceMenu, false);
        gameManager.inOptionsGameplayMenu = false;
        gameManager.inOptionsVideoMenu = false;
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;        

        Utils.ActivateObject(optionsAccessibilityMenu, true);
        gameManager.inOptionsAccessibilityMenu = true;
    }
}
