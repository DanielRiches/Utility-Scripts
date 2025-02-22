using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] public GameObject mainMenu;
    [Header("---- Options ----------------------------------------------------")]
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] public GameObject optionsGameplayMenu;
    [SerializeField] public GameObject optionsVideoMenu;
    [SerializeField] public GameObject optionsAudioMenu;
    [SerializeField] public GameObject optionsKeyboardMenu;
    [SerializeField] public GameObject optionsGamepadMenu;
    [SerializeField] public GameObject optionsInterfaceMenu;
    [SerializeField] public GameObject optionsAccessibilityMenu;
    [Space(10)]
    public TextMeshProUGUI optionsDescription;// ACCESSED BY OPTIONS
    public TextMeshProUGUI optionsDescriptionAdditional;// ACCESSED BY OPTIONS
    [Header("---- Gameplay ----")]
    [SerializeField] public GameObject gameplayButton;
    public Toggle autosavesToggle;// ACCESSED BY OPTIONS
    public TextMeshProUGUI maximumAutosavesSliderText;// ACCESSED BY OPTIONS
    public Slider maximumAutosavesSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI maximumQuicksavesSliderText;// ACCESSED BY OPTIONS
    public Slider maximumQuicksavesSlider;// ACCESSED BY OPTIONS
    public Toggle goreToggle;// ACCESSED BY OPTIONS
    [Header("---- Video ----")]
    public TextMeshProUGUI gpuName;// ACCESSED BY OPTIONS
    public TMP_Dropdown displayDevicesDropdown;// ACCESSED BY OPTIONS
    public TMP_Dropdown resolutionsDropdown;// ACCESSED BY OPTIONS
    public TMP_Dropdown displayModeDropdown;// ACCESSED BY OPTIONS
    public TextMeshProUGUI frameRateCapSliderText;// ACCESSED BY OPTIONS
    public Toggle frameRateCapToggle;// ACCESSED BY OPTIONS
    public Slider frameRateCapSlider;// ACCESSED BY OPTIONS
    public TMP_Dropdown vSyncDropdown;// ACCESSED BY OPTIONS
    public TMP_Dropdown qualityDropdown;// ACCESSED BY OPTIONS
    public TMP_Dropdown antiAliasDropdown;// ACCESSED BY OPTIONS    
    public TMP_Dropdown fogDropdown;// ACCESSED BY OPTIONS
    public TextMeshProUGUI fovSliderText;// ACCESSED BY OPTIONS
    public Slider fovSlider;// ACCESSED BY OPTIONS
    public TMP_Dropdown bloomDropdown;// ACCESSED BY OPTIONS
    public TMP_Dropdown ansioDropdown;// ACCESSED BY OPTIONS
    public Toggle hdrToggle;// ACCESSED BY OPTIONS
    public TMP_Dropdown tonemappingDropdown;// ACCESSED BY OPTIONS
    public TMP_Dropdown giDropdown;// ACCESSED BY OPTIONS
    [Header("---- Audio ----")]
    public TextMeshProUGUI audioMasterNumberText;// ACCESSED BY OPTIONS
    public Slider audioMasterSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI audioMusicNumberText;// ACCESSED BY OPTIONS
    public Slider audioMusicSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI audioAmbientNumberText;// ACCESSED BY OPTIONS
    public Slider audioAmbientSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI audioSFXNumberText;// ACCESSED BY OPTIONS
    public Slider audioSFXSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI audioUINumberText;// ACCESSED BY OPTIONS
    public Slider audioUISlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI audioVoiceNumberText;// ACCESSED BY OPTIONS
    public Slider audioVoiceSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI audioEventNumberText;// ACCESSED BY OPTIONS
    public Slider audioEventSlider;// ACCESSED BY OPTIONS
    [Header("---- Keyboard ----")]
    public TextMeshProUGUI cameraZoomSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraZoomSensitivitySlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI cameraMoveSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraMoveSensitivitySlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI cameraPanSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraPanSensitivitySlider;// ACCESSED BY OPTIONS
    [Header("---- Gamepad ----")]
    public TextMeshProUGUI cameraGamepadZoomSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadZoomSensitivitySlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI cameraGamepadMoveSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadMoveSensitivitySlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI cameraGamepadPanSensitivitySliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadPanSensitivitySlider;// ACCESSED BY OPTIONS

    public TextMeshProUGUI cameraGamepadZoomDeadzoneSliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadZoomDeadzoneSlider;// ACCESSED BY OPTIONS
    public TextMeshProUGUI cameraGamepadMoveDeadzoneSliderText;// ACCESSED BY OPTIONS
    public Slider cameraGamepadMoveDeadzoneSlider;// ACCESSED BY OPTIONS
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
        Utils.DeActivateObject(mainMenu);
        Utils.ActivateObject(optionsMenu);        
    }
    public void ExitOptions()
    {        
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsInterfaceMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);

        Utils.ActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsMenu);
        Utils.ActivateObject(mainMenu);
    }

    public void ClickedGameplayOptions()
    {
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsInterfaceMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);
        Utils.ActivateObject(optionsGameplayMenu);
    }
    public void ClickedVideoOptions()
    {
        Utils.DeActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);
        Utils.ActivateObject(optionsVideoMenu);
    }

    public void ClickedAudioOptions()
    {
        Utils.DeActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsInterfaceMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);
        Utils.ActivateObject(optionsAudioMenu);
    }
    public void ClickedKeyboardOptions()
    {
        Utils.DeActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsInterfaceMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);
        Utils.ActivateObject(optionsKeyboardMenu);
    }
    public void ClickedGamepadOptions()
    {
        Utils.DeActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsInterfaceMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);
        Utils.ActivateObject(optionsGamepadMenu);
    }

    public void ClickedInterfaceOptions()
    {
        Utils.DeActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsAccessibilityMenu);
        Utils.ActivateObject(optionsInterfaceMenu);
    }

    public void ClickedAccessibilityOptions()
    {
        Utils.DeActivateObject(optionsGameplayMenu);
        Utils.DeActivateObject(optionsVideoMenu);
        Utils.DeActivateObject(optionsAudioMenu);
        Utils.DeActivateObject(optionsKeyboardMenu);
        Utils.DeActivateObject(optionsGamepadMenu);
        Utils.DeActivateObject(optionsInterfaceMenu);
        Utils.ActivateObject(optionsAccessibilityMenu);
    }
}
