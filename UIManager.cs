﻿using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions; // Needed for regex filtering
using System.Text;

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
        public GameObject applyOptionsChanges;
        public GameObject revertOptionsChanges;
        [Space(5)]
        public TextMeshProUGUI optionsTitle;
        public TextMeshProUGUI optionsDescription;
        public TextMeshProUGUI optionsDescriptionAdditional;
        [Header("---- Gameplay ----")]
        public GameObject optionsGameplayMenu;
        public Image optionsGameplayMenuButtonBG;
        public UI optionsGameplayMenuButtonUIScript;
        [Space(5)]
        public Toggle autosavesToggle;
        public GameObject autosavesModifiedIcon;
        [Space(5)]
        public TMP_InputField maximumAutosavesSliderText;
        public Slider maximumAutosavesSlider;
        public GameObject maximumAutosavesModifiedIcon;
        [Space(5)]
        public TMP_InputField maximumQuicksavesSliderText;
        public Slider maximumQuicksavesSlider;
        public GameObject maximumQuicksavesModifiedIcon;
        [Space(5)]
        public GameObject goreEffect;
        public TMP_Dropdown goreDropdown;
        public GameObject goreModifiedIcon;
        [Space(5)]
        public GameObject destructionEffect;
        public TMP_Dropdown destructionDropdown;
        public GameObject destructionModifiedIcon;
        [Header("---- Video ----")]
        public GameObject optionsVideoMenu;
        public Image optionsVideoMenuButtonBG;
        public UI optionsVideoMenuButtonUIScript;
        [Space(5)]
        public GameObject optionsVideoPage1;
        public UI optionsVideoPage1UIScript;
        public GameObject optionsVideoPage1BG;
        [Space(5)]
        public GameObject optionsVideoPage2;
        public UI optionsVideoPage2UIScript;
        public GameObject optionsVideoPage2BG;
        [Space(5)]
        public TextMeshProUGUI gpuName;
        [Space(5)]
        public TMP_Dropdown displayDevicesDropdown;
        public GameObject displayDeviceModifiedIcon;
        [Space(5)]
        public TMP_Dropdown resolutionsDropdown;
        public GameObject resolutionsModifiedIcon;
        [Space(5)]
        public TMP_Dropdown displayModeDropdown;
        public GameObject displayModeModifiedIcon;
        [Space(5)]
        public GameObject framerateCapEffect;
        public TMP_InputField frameRateCapSliderText;
        public Toggle frameRateCapToggle;
        public Slider frameRateCapSlider;
        public GameObject framerateCapModifiedIcon;
        [Space(5)]
        public GameObject vSyncEffect;
        public TMP_Dropdown vSyncDropdown;
        public GameObject vSyncModifiedIcon;
        [Space(5)]
        public GameObject qualityEffect;
        public TMP_Dropdown qualityDropdown;
        public GameObject qualityModifiedIcon;
        [Space(5)]
        public GameObject antiAliasEffect;
        public TMP_Dropdown antiAliasDropdown;        
        public GameObject antiAliasModifiedIcon;
        [Space(5)]
        public TMP_Dropdown taaQualityDropdown;
        public GameObject taaQualityModifiedIcon;
        [Space(5)]
        public GameObject fogEffect;
        public TMP_Dropdown fogDropdown;
        public GameObject fogModifiedIcon;
        [Space(5)]
        public TMP_InputField fovSliderText;
        public Slider fovSlider;
        public GameObject fovModifiedIcon;
        [Space(5)]
        public TMP_InputField renderDistanceSliderText;
        public Slider renderDistanceSlider;
        public GameObject renderDistanceModifiedIcon;
        [Space(5)]
        public GameObject bloomEffect;
        public TMP_Dropdown bloomDropdown;
        public GameObject bloomModifiedIcon;
        [Space(5)]
        public GameObject ansioEffect;
        public TMP_Dropdown ansioDropdown;
        public GameObject ansioModifiedIcon;
        [Space(5)]
        public GameObject hdrEffect;
        public TMP_Dropdown hdrDropdown;
        public GameObject hdrModifiedIcon;
        [Space(5)]
        public TMP_Dropdown tonemappingDropdown;
        public GameObject tonemappingModifiedIcon;
        [Space(5)]
        public TMP_Dropdown tonemappingQualityDropdown;
        public GameObject tonemappingQualityModifiedIcon;
        [Space(5)]
        public GameObject globalIlluminationEffect;
        public TMP_Dropdown giDropdown;
        public GameObject giModifiedIcon;
        [Space(5)]
        public GameObject giResolutionEffect;
        public TMP_Dropdown giResolutionDropdown;
        public GameObject giResolutionModifiedIcon;        
        [Space(5)]        
        public TMP_Dropdown reflectionsDropdown;
        public GameObject reflectionsModifiedIcon;
        [Space(5)]
        public GameObject planarReflectionEffect;
        public TMP_Dropdown planarReflectionsDropdown;
        public GameObject planarReflectionsModifiedIcon;
        [Space(5)]
        public GameObject weatherEffectsEffect;
        public TMP_Dropdown weatherEffectsDropdown;
        public GameObject weatherEffectsModifiedIcon;
        [Space(5)]
        public GameObject lineRenderingEffect;
        public TMP_Dropdown lineRenderingDropdown;
        public GameObject lineRenderingModifiedIcon;
        [Space(5)]
        public GameObject crevicesEffect;
        public TMP_Dropdown crevicesDropdown;
        public GameObject crevicesModifiedIcon;
        [Space(5)]
        public TMP_Dropdown shadowQualityDropdown;
        public GameObject shadowQualityModifiedIcon;
        [Space(5)]
        public TMP_InputField shadowDistanceSliderText;
        public Slider shadowDistanceSlider;
        public GameObject shadowDistanceModifiedIcon;
        [Space(5)]
        public GameObject fpsCounter;
        public TextMeshProUGUI fpsCounterText;
        public Animator fpsCounterAnimator;
        public GameObject fpsCounterEffect;
        public Toggle fpsCounterToggle;
        public GameObject fpsCounterModifiedIcon;
        [Space(5)]
        public GameObject memoryEffect;
        public GameObject memoryCounter;
        public TextMeshProUGUI memoryCounterText;
        public Animator memoryCounterAnimator;
        public GameObject memoryCounterEffect;
        public Toggle memoryCounterToggle;
        public GameObject memoryCounterModifiedIcon;
        [Header("---- Audio ----")]
        public GameObject optionsAudioMenu;
        public Image optionsAudioMenuButtonBG;
        public UI optionsAudioMenuButtonUIScript;
        [Space(5)]
        public TextMeshProUGUI audioMasterNumberText;
        public Slider audioMasterSlider;
        public GameObject audioMasterModifiedIcon;
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
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsGameplaySectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsGameplaySectionTitle;
            }
        }
        else
        {
            //DeActivate Options Gameplay
        }

        if (gameManager.inOptionsVideoMenu)
        {
            if (optionsUI.optionsVideoPage1.activeSelf)
            {
                gameManager.scripts.uiManager.TintUIScriptTrigger(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1UIScript, true);
                gameManager.scripts.uiManager.TintUIScriptTrigger(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2UIScript, false);
            }
            else if (optionsUI.optionsVideoPage2.activeSelf)
            {
                gameManager.scripts.uiManager.TintUIScriptTrigger(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1UIScript, false);
                gameManager.scripts.uiManager.TintUIScriptTrigger(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2UIScript, true);
            }

            if (optionsUI.optionsVideoMenuButtonBG.color != Colors.red)
            {
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsVideoSectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsVideoSectionTitle;
            }
        }
        else
        {
            //DeActivate Options video
        }

        if (gameManager.inOptionsAudioMenu)
        {
            if (optionsUI.optionsAudioMenuButtonBG.color != Colors.red)
            {
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
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
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
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
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
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
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, false);
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
                TintUIScriptTrigger(optionsUI.optionsAccessibilityMenuButtonUIScript, true);
                TintUIScriptTrigger(optionsUI.optionsGameplayMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsVideoMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsAudioMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsKeyboardMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsGamepadMenuButtonUIScript, false);
                TintUIScriptTrigger(optionsUI.optionsInterfaceMenuButtonUIScript, false);
            }
            if (optionsUI.optionsTitle.text != GameStrings.GameStringsEnglish.optionsAccessibilitySectionTitle)
            {
                optionsUI.optionsTitle.text = GameStrings.GameStringsEnglish.optionsAccessibilitySectionTitle;
            }
        }
    }

    void TrimInput(Slider slider, TMP_InputField sliderText, bool decimals)
    {
        
        if (string.IsNullOrEmpty(sliderText.text))// If the input is empty, set to the minimum value
        {
            sliderText.text = slider.minValue.ToString();
        }
        
        StringBuilder numericText = new StringBuilder();// numeric-only string
        foreach (char c in sliderText.text)
        {
            if (char.IsDigit(c))
            {
                numericText.Append(c);
            }
        }
        sliderText.text = numericText.ToString();
        
        if (!decimals)
        {
            sliderText.text = float.TryParse(sliderText.text, out float result) ? result.ToString(Strings.numberFormat0) : sliderText.text;// Format the string to remove decimals
        }        
        
        int maxDigits = slider.maxValue.ToString().Length;// Check if the length exceeds max slider value
        if (sliderText.text.Length > maxDigits)
        {
            sliderText.text = sliderText.text.Remove(sliderText.text.Length - 1);// delete last digit
        }
    }
    public void OnInputEntered(Slider slider, TMP_InputField sliderText)
    {
        string input = sliderText.text;// Get the text directly from the input field
        //Debug.Log($"Raw Input: '{input}'");

        input = input.Trim();// Trim whitespace to avoid false "empty" detections
        //Debug.Log($"Trimmed Input: '{input}'");

        string numericInput = Regex.Replace(input, @"[^0-9]", "");// Filter out non-numeric characters
        //Debug.Log($"Filtered Numeric Input: '{numericInput}'");

        int minValue = (int)slider.minValue;
        int maxValue = (int)slider.maxValue;

        int inputValue;
        if (string.IsNullOrEmpty(numericInput))// If input is empty after filtering, set to half of max value
        {
            //Debug.Log("EMPTY - Defaulting to half maxValue");
            inputValue = maxValue / 2;
        }
        else
        {
            inputValue = int.Parse(numericInput);// Convert to integer
        }
        inputValue = Mathf.Clamp(inputValue, minValue, maxValue);

        //Debug.Log($"Final Value Applied: {inputValue}");        
        slider.value = inputValue;
        sliderText.text = inputValue.ToString();
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
                    scrollAmount = scrollDelta.y * gameManager.scripts.optionsManager.selectedProperties.selectedMouseScrollSensitivity;
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
                    scrollAmount = scrollDelta.y * gameManager.scripts.optionsManager.selectedProperties.selectedMouseScrollSensitivity;
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
        gameManager.inOptionsGameplayMenu = false;        
        gameManager.inOptionsAudioMenu = false;
        gameManager.inOptionsKeyboardMenu = false;
        gameManager.inOptionsGamepadMenu = false;
        gameManager.inOptionsInterfaceMenu = false;
        gameManager.inOptionsAccessibilityMenu = false;
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
    public void OnMaximumAutosavesInputField()
    {
        OnInputEntered(optionsUI.maximumAutosavesSlider, optionsUI.maximumAutosavesSliderText);
    }
    public void OnMaximumAutosavesInputChanged()
    {
        TrimInput(optionsUI.maximumAutosavesSlider, optionsUI.maximumAutosavesSliderText, false);
    }

    public void OnMaximumQuicksavesHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsMaximumQuicksavesDesc;
    }
    public void OnMaximumQuicksavesInputField()
    {
        OnInputEntered(optionsUI.maximumQuicksavesSlider, optionsUI.maximumQuicksavesSliderText);
    }
    public void OnMaximumQuicksavesInputChanged()
    {
        TrimInput(optionsUI.maximumQuicksavesSlider, optionsUI.maximumQuicksavesSliderText, false);
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
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsFramerateCapDesc;

        gameManager.scripts.optionsManager.refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;

        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFramerateCapInfoDesc + "<color=" + Colors.textCyan + ">" + gameManager.scripts.optionsManager.refreshRate.ToString("F0") + " Hz</color>.";
    }
    public void OnFramerateCapInputField()
    {
        OnInputEntered(optionsUI.frameRateCapSlider, optionsUI.frameRateCapSliderText);
    }
    public void OnFramerateCapInputChanged()
    {
        TrimInput(optionsUI.frameRateCapSlider, optionsUI.frameRateCapSliderText, false);
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
    public void OnFOVInputField()
    {
        OnInputEntered(optionsUI.fovSlider, optionsUI.fovSliderText);
    }
    public void OnFOVInputChanged()
    {
        TrimInput(optionsUI.fovSlider, optionsUI.fovSliderText, false);
    }

    public void OnRenderDistanceHover()
    {
        optionsUI.optionsDescription.text = GameStrings.GameStringsEnglish.optionsRenderDistanceDesc;
        optionsUI.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnRenderDistanceInputField()
    {
        OnInputEntered(optionsUI.renderDistanceSlider, optionsUI.renderDistanceSliderText);
    }
    public void OnRenderDistanceInputChanged()
    {
        TrimInput(optionsUI.renderDistanceSlider, optionsUI.renderDistanceSliderText, false);
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
        if (gameManager.scripts.optionsManager.selectedProperties.selectedGlobalIlluminationResIndex == 1)
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
    public void OnShadowDistanceInputField()
    {
        OnInputEntered(optionsUI.shadowDistanceSlider, optionsUI.shadowDistanceSliderText);
    }
    public void OnShadowDistanceInputChanged()
    {
        TrimInput(optionsUI.shadowDistanceSlider, optionsUI.shadowDistanceSliderText, false);
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

    // ---------------------------------
    public void TintUIScriptTrigger(UI uiScript, bool on)
    {
        if (on)
        {
            uiScript.UIMouseOver();
        }
        else
        {
            uiScript.UIMouseExit();
        }
    }
}
