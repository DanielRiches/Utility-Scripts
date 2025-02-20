using UnityEngine;
using System.Collections;// Works on Steamdeck
using System.Collections.Generic;// Works on Steamdeck
using System.Linq;// Works on Steamdeck
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using TMPro;
public class OptionsManager : MonoBehaviour
{
    GameManager gameManager;
    [Header("---- SELECTED ----------------------------------------------------")]
    public bool requiresTimedConfirmation;
    [Header("Gameplay")]
    public bool selectedAutosaves = true;
    public int selectedMaximumAutosaves = 10;
    public int selectedMaximumQuicksaves = 10;
    public bool selectedGore;
    Display[] displays;// To help automatically detect if someone connects another display.
    private int lastDisplayCount; // To help automatically detect if someone connects another display, set to detect every 2 seconds at default.
    [SerializeField] private int selectedDisplayIndex;
    private string gpuName;// Graphics card name.
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    Resolution selectedResolution;
    [SerializeField] private int selectedResolutionIndex;
    [SerializeField] private int selectedDisplayModeIndex;
    public bool selectedFrameRateCap = true;
    public int selectedFrameRateCapValue = 120;
    [SerializeField] private int selectedVsyncIndex;
    [SerializeField] private int selectedAntiAliasIndex;
    [SerializeField] private HDRenderPipelineAsset appliedHDRPAsset;
    public int selectedQualityAssetIndex;
    [Header("Audio")]
    [SerializeField] private float selectedMasterVolume;
    [SerializeField] private float selectedMusicVolume;
    [SerializeField] private float selectedAmbientVolume;
    [SerializeField] private float selectedSfxVolume;
    [SerializeField] private float selectedUiVolume;
    [SerializeField] private float selectedVoiceVolume;
    [SerializeField] private float selectedEventVolume;
    [Header("Keyboard")]
    [SerializeField] private float selectedCameraZoomSensitivity;
    [SerializeField] private float selectedCameraMoveSensitivity;
    [SerializeField] private float selectedCameraPanSensitivity;
    [Header("Gamepad")]
    [SerializeField] private float selectedGamepadCameraZoomSensitivity;
    [SerializeField] private float selectedGamepadCameraMoveSensitivity;
    [SerializeField] private float selectedGamepadCameraPanSensitivity;
    [SerializeField] private float selectedGamepadCameraZoomDeadzone;
    [SerializeField] private float selectedGamepadCameraMoveDeadzone;
    [SerializeField] private float selectedGamepadCameraPanDeadzone;
    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.optionsManager = this;
    }

    void Start()
    {
        appliedHDRPAsset = QualitySettings.renderPipeline as HDRenderPipelineAsset;
        lastDisplayCount = Display.displays.Length;// DISPLAY DEVICE
        gpuName = SystemInfo.graphicsDeviceName;// DISPLAY ADAPTER
        gameManager.scripts.uiManager.gpuName.text = gpuName;// DISPLAY ADAPTER 
        PopulateSavesSliders();
        PopulateGore();
        DetectDisplays();// DISPLAY DEVICE
        StartCoroutine(RuntimeMonitorDisplays());// WILL UPDATE DISPLAY DEVICE DROPDOWN AT RUNTIME IF MONITORS ARE ADDED / REMOVED
        PopulateDisplayModeDropdown();
        PopulateFrameRateCapSlider();
        PopulateVSyncDropdown();
        PopulateAntiAliasDropdown();
        PopulateQualityDropdown();
        PopulateAudioSliders();
        PopulateMouseSliders();
        PopulateGamepadSliders();
    }

    /*
    void ApplySettings()
    {
        if (Application.isPlaying)
        {
            if (requiresTimedConfirmation)
            {
                // ISSUE WARNING
            }
            else
            {
                // GAMEPLAY--------------
                if (selectedAutosaves)
                {
                    appliedAutosaves = true;
                }
                else if (!selectedAutosaves)
                {
                    appliedAutosaves = false;
                }

                appliedMaximumAutosaves = selectedMaximumAutosaves;
                appliedMaximumQuicksaves = selectedMaximumQuicksaves;

                if (selectedGore)
                {
                    appliedGore = true;
                }
                else if (!selectedGore)
                {
                    appliedGore = false;
                }

                // VIDEO--------------
                if (filteredResolutions.Count > selectedResolutionIndex)
                {
                    // Get the corresponding resolution from the filtered list
                    Resolution selectedResolution = filteredResolutions[selectedResolutionIndex];

                    if (selectedDisplayModeIndex == 1)
                    {
                        Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.Windowed, selectedResolution.refreshRateRatio);
                    }
                    else
                    {
                        Screen.SetResolution(selectedResolution.width, selectedResolution.height, FullScreenMode.FullScreenWindow, selectedResolution.refreshRateRatio);
                    }


                    // Update appliedResolutionIndex after applying the selected resolution
                    appliedResolutionIndex = selectedResolutionIndex;
                    appliedDisplayModeIndex = selectedDisplayModeIndex;
                    //Debug.Log($"Applied resolution: {selectedResolution.width}<color=red>x</color>{selectedResolution.height} {selectedResolution.refreshRateRatio.numerator}/{selectedResolution.refreshRateRatio.denominator}<color=red>Hz</color>");
                }

                appliedQualityAssetIndex = selectedQualityAssetIndex;
                if (appliedQualityAssetIndex == 0)
                {
                    QualitySettings.SetQualityLevel(0, true);// HIGH FIDELITY
                }
                else if (appliedQualityAssetIndex == 1)
                {
                    QualitySettings.SetQualityLevel(1, true);// BALANCED
                }
                else if (appliedQualityAssetIndex == 2)
                {
                    QualitySettings.SetQualityLevel(2, true);// PERFORMANT
                }
                appliedHDRPAsset = QualitySettings.renderPipeline as HDRenderPipelineAsset;


                appliedAntiAliasIndex = selectedAntiAliasIndex;
                if (appliedAntiAliasIndex == 0)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
                }
                else if (appliedAntiAliasIndex == 1)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
                }
                else if (appliedAntiAliasIndex == 2)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Low;
                }
                else if (appliedAntiAliasIndex == 3)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Medium;
                }
                else if (appliedAntiAliasIndex == 4)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.High;
                }
                else if (appliedAntiAliasIndex == 5)
                {
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                    QualitySettings.antiAliasing = 1; // TURN ON MSAA LOW
                }
                else if (appliedAntiAliasIndex == 6)
                {
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                    QualitySettings.antiAliasing = 2; // TURN ON MSAA MEDIUM
                }
                else if (appliedAntiAliasIndex == 7)
                {
                    gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                    QualitySettings.antiAliasing = 3; // TURN ON MSAA HIGH
                }
            }
        }
    }
    
    void RevertChanges()
    {
        selectedDisplayIndex = appliedDisplayIndex;
        selectedResolutionIndex = appliedResolutionIndex;
        selectedDisplayModeIndex = appliedDisplayModeIndex;
        selectedVsyncIndex = appliedVsyncIndex;
        selectedAntiAliasIndex = appliedAntiAliasIndex;
        selectedMaximumAutosaves = appliedMaximumAutosaves;
        selectedMaximumQuicksaves = appliedMaximumQuicksaves;
        requiresTimedConfirmation = false;
    }
    */

    #region Setup
    void PopulateSavesSliders()
    {
        selectedAutosaves = true;
        gameManager.scripts.uiManager.autosavesToggle.isOn = true;
        gameManager.scripts.uiManager.autosavesToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.autosavesToggle.onValueChanged.AddListener(OnAutosavesToggleChanged);

        selectedMaximumAutosaves = 10;        
        gameManager.scripts.uiManager.maximumAutosavesSlider.value = selectedMaximumAutosaves;
        gameManager.scripts.uiManager.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.maximumAutosavesSlider.value.ToString();
        gameManager.scripts.uiManager.maximumAutosavesSlider.onValueChanged.AddListener(OnAutosavesSliderChanged);

        selectedMaximumQuicksaves = 10;
        gameManager.scripts.uiManager.maximumQuicksavesSlider.value = selectedMaximumQuicksaves;
        gameManager.scripts.uiManager.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.maximumQuicksavesSlider.value.ToString();
        gameManager.scripts.uiManager.maximumQuicksavesSlider.onValueChanged.AddListener(OnQuicksavesSliderChanged);
    }
    void PopulateGore()
    {
        selectedGore = true;
        gameManager.scripts.uiManager.goreToggle.isOn = true;
        gameManager.scripts.uiManager.goreToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.goreToggle.onValueChanged.AddListener(OnGoreChanged);
    }
    void DetectDisplays()
    {
        selectedDisplayIndex = 0;
        gameManager.scripts.uiManager.displayDevicesDropdown.ClearOptions();// Clear existing options

        displays = Display.displays;// Get all connected displays
        var displayNames = new System.Collections.Generic.List<string>();

        for (int i = 0; i < displays.Length; i++)
        {
            string displayName = $"{i + 1}<color=red> : </color> {displays[i].systemWidth} <color=red>x</color> {displays[i].systemHeight}";
            displayNames.Add(displayName);
        }

        gameManager.scripts.uiManager.displayDevicesDropdown.AddOptions(displayNames);// Add options to dropdown

        if (displays.Length > 0)// Automatically populate resolutions for the display
        {
            PopulateResolutionsDropdown(0);
        }
        selectedDisplayIndex = gameManager.scripts.uiManager.displayDevicesDropdown.value;// Get selected display index from the dropdown  
        gameManager.scripts.uiManager.displayDevicesDropdown.onValueChanged.RemoveAllListeners();// Add listener to update resolutions when display is selected
        gameManager.scripts.uiManager.displayDevicesDropdown.onValueChanged.AddListener(PopulateResolutionsDropdown);
        gameManager.scripts.uiManager.displayDevicesDropdown.onValueChanged.AddListener(OnDisplayDeviceChanged);
        //OptionsModifiedValueCheck(null, false, null, 0, gameManager.scripts.uiManager.displayDevicesDropdown, appliedDisplayIndex, ref gameManager.scripts.uiManager.resolutionsModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
    }
    void PopulateResolutionsDropdown(int displayIndex)
    {
        if (gameManager.scripts.uiManager.resolutionsDropdown == null)
        {
            return;
        }

        gameManager.scripts.uiManager.resolutionsDropdown.ClearOptions();
        resolutions = Screen.resolutions;

        if (resolutions.Length == 0)
        {
            Debug.LogWarning("Detected no resolutions available on Display Device.");
            return;
        }

        var resolutionOptions = new List<string>();
        filteredResolutions.Clear();

        int bootResolutionIndex = 0;
        int preferredResolutionIndex = -1;

        Resolution detectedStartResolution = Screen.currentResolution;
        int maxRefreshRate = resolutions.Max(res => (int)res.refreshRateRatio.numerator / (int)res.refreshRateRatio.denominator);

        List<(int width, int height, int refreshRate)> seenResolutions = new List<(int, int, int)>();

        // Store resolutions first
        foreach (var res in resolutions)
        {
            int refreshRate = (int)res.refreshRateRatio.numerator / (int)res.refreshRateRatio.denominator;
            var resolutionTuple = (res.width, res.height, refreshRate);

            if (refreshRate == maxRefreshRate && !seenResolutions.Contains(resolutionTuple))
            {
                filteredResolutions.Add(res);
                seenResolutions.Add(resolutionTuple);
            }
        }

        // Sort resolutions in DESCENDING order (first by width, then by height)
        filteredResolutions.Sort((a, b) =>
        {
            if (a.width != b.width)
                return b.width.CompareTo(a.width); // Reverse order
            return b.height.CompareTo(a.height); // Reverse order
        });

        // Populate dropdown
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution res = filteredResolutions[i];
            string resolutionText = $"{res.width}<color=red> x </color>{res.height}";
            resolutionOptions.Add(resolutionText);

            if (res.width == detectedStartResolution.width && res.height == detectedStartResolution.height)
            {
                bootResolutionIndex = i;
            }

            if (res.width == 1920 && res.height == 1080)
            {
                preferredResolutionIndex = i;
            }
        }

        selectedResolutionIndex = (preferredResolutionIndex != -1) ? preferredResolutionIndex : bootResolutionIndex;
        gameManager.scripts.uiManager.resolutionsDropdown.AddOptions(resolutionOptions);
        gameManager.scripts.uiManager.resolutionsDropdown.value = selectedResolutionIndex;
        gameManager.scripts.uiManager.resolutionsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.resolutionsDropdown.onValueChanged.AddListener(OnResolutionChanged);
        gameManager.scripts.uiManager.resolutionsDropdown.RefreshShownValue();
    }
    void PopulateDisplayModeDropdown()
    {
        gameManager.scripts.uiManager.displayModeDropdown.ClearOptions();
        gameManager.scripts.uiManager.displayModeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Fullscreen",
            "Windowed",
        });

        gameManager.scripts.uiManager.displayModeDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
    }
    void PopulateFrameRateCapSlider()
    {
        selectedFrameRateCap = true;
        gameManager.scripts.uiManager.frameRateCapToggle.isOn = true;
        gameManager.scripts.uiManager.frameRateCapToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.frameRateCapToggle.onValueChanged.AddListener(OnFrameRateCapToggleChanged);

        selectedFrameRateCapValue = 120;
        gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
        gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
        gameManager.scripts.uiManager.frameRateCapSlider.onValueChanged.AddListener(OnframeRateCapSliderChanged);
    }
    void PopulateVSyncDropdown()
    {
        gameManager.scripts.uiManager.vSyncDropdown.ClearOptions();
        gameManager.scripts.uiManager.vSyncDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Off",
            "V<color=red>-</color>Blank",
            "Second V<color=red>-</color>Blank",
        });

        gameManager.scripts.uiManager.vSyncDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.vSyncDropdown.onValueChanged.AddListener(OnVSyncChanged);
        gameManager.scripts.uiManager.vSyncDropdown.RefreshShownValue();// Update the dropdown to reflect the selected VSync
    }
    void PopulateAntiAliasDropdown()
    {
        gameManager.scripts.uiManager.antiAliasDropdown.ClearOptions();
        gameManager.scripts.uiManager.antiAliasDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Off",
            "FXAA",
            "SMAA <color=red>-</color> 2 <color=red>x</color>",
            "SMAA <color=red>-</color> 4 <color=red>x</color>",
            "SMAA <color=red>-</color> 8 <color=red>x</color>",
            "MSAA <color=red>-</color> 2 <color=red>x</color>",
            "MSAA <color=red>-</color> 4 <color=red>x</color>",
            "MSAA <color=red>-</color> 8 <color=red>x</color>",
            "TAA <color=red>-</color> 2 <color=red>x</color>",
            "TAA <color=red>-</color> 4 <color=red>x</color>",
            "TAA <color=red>-</color> 8 <color=red>x</color>",
        });

        gameManager.scripts.uiManager.antiAliasDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.antiAliasDropdown.onValueChanged.AddListener(OnAntiAliasChanged);
        gameManager.scripts.uiManager.antiAliasDropdown.value = 3;
        selectedAntiAliasIndex = gameManager.scripts.uiManager.antiAliasDropdown.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.antiAliasDropdown.RefreshShownValue();// Update the dropdown to reflect the selected aa
    }
    void PopulateQualityDropdown()
    {
        selectedQualityAssetIndex = 1;
        gameManager.scripts.uiManager.qualityDropdown.ClearOptions();
        gameManager.scripts.uiManager.qualityDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "High",
            "Medium",
            "Low",
        });

        gameManager.scripts.uiManager.qualityDropdown.value = 1;
        gameManager.scripts.uiManager.qualityDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.qualityDropdown.onValueChanged.AddListener(OnQualitySettingsChanged);
        gameManager.scripts.uiManager.qualityDropdown.RefreshShownValue();// Update the dropdown to reflect the selected Quality
        /*
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            Debug.Log($"Quality Level {i}: {QualitySettings.names[i]}");
        }
        */
    }
    void PopulateAudioSliders()
    {
        ChangeAudioSliderValue(ref selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.audioMasterSlider, gameManager.scripts.uiManager.audioMasterNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.audioMusicSlider, gameManager.scripts.uiManager.audioMusicNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.audioAmbientSlider, gameManager.scripts.uiManager.audioAmbientNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedSfxVolume, Strings.audioSFX, gameManager.scripts.uiManager.audioSFXSlider, gameManager.scripts.uiManager.audioSFXNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.audioUISlider, gameManager.scripts.uiManager.audioUINumberText, false, false, true);
        ChangeAudioSliderValue(ref selectedUiVolume, Strings.audioUIPitchShiftVolume, gameManager.scripts.uiManager.audioUISlider, gameManager.scripts.uiManager.audioUINumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.audioVoiceSlider, gameManager.scripts.uiManager.audioVoiceNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.audioEventSlider, gameManager.scripts.uiManager.audioEventNumberText, false, false, true);

        // Add listeners
        gameManager.scripts.uiManager.audioMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        gameManager.scripts.uiManager.audioMusicSlider.onValueChanged.AddListener(SetMusicVolume);
        gameManager.scripts.uiManager.audioAmbientSlider.onValueChanged.AddListener(SetAmbientVolume);
        gameManager.scripts.uiManager.audioSFXSlider.onValueChanged.AddListener(SetSFXVolume);
        gameManager.scripts.uiManager.audioUISlider.onValueChanged.AddListener(SetUIVolume);
        gameManager.scripts.uiManager.audioVoiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        gameManager.scripts.uiManager.audioEventSlider.onValueChanged.AddListener(SetEventVolume);
    }
    void PopulateMouseSliders()
    {
        selectedCameraZoomSensitivity = 1;
        gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value = selectedCameraZoomSensitivity;
        gameManager.scripts.uiManager.cameraZoomSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraZoomSensitivitySlider.onValueChanged.AddListener(OnCameraZoomSensitivityChanged);

        selectedCameraMoveSensitivity = 1;
        gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value = selectedCameraMoveSensitivity;
        gameManager.scripts.uiManager.cameraMoveSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraMoveSensitivitySlider.onValueChanged.AddListener(OnCameraMoveSensitivityChanged);

        selectedCameraPanSensitivity = 1;
        gameManager.scripts.uiManager.cameraPanSensitivitySlider.value = selectedCameraPanSensitivity;
        gameManager.scripts.uiManager.cameraPanSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraPanSensitivitySlider.onValueChanged.AddListener(OnCameraPanSensitivityChanged);
    }
    void PopulateGamepadSliders()
    {
        selectedGamepadCameraZoomSensitivity = 1;
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value = selectedGamepadCameraZoomSensitivity;
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.onValueChanged.AddListener(OnGamepadCameraZoomSensitivityChanged);

        selectedGamepadCameraMoveSensitivity = 1;
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value = selectedGamepadCameraMoveSensitivity;
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.onValueChanged.AddListener(OnGamepadCameraMoveSensitivityChanged);

        selectedGamepadCameraPanSensitivity = 1;
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value = selectedGamepadCameraPanSensitivity;
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.onValueChanged.AddListener(OnGamepadCameraPanSensitivityChanged);

        selectedGamepadCameraZoomDeadzone = 0.1f;
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value = selectedGamepadCameraZoomDeadzone;
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.onValueChanged.AddListener(OnGamepadCameraZoomDeadzoneChanged);

        selectedGamepadCameraMoveDeadzone = 0.1f;
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value = selectedGamepadCameraMoveDeadzone;
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.onValueChanged.AddListener(OnGamepadCameraMoveDeadzoneChanged);

        selectedGamepadCameraPanDeadzone = 0.1f;
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value = selectedGamepadCameraPanDeadzone;
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.onValueChanged.AddListener(OnGamepadCameraPanDeadzoneChanged);
    }
    #endregion




    #region Runtime

    void OnAutosavesToggleChanged(bool autosavesChanged)
    {
        if (gameManager.scripts.uiManager.autosavesToggle.isOn)
        {
            selectedAutosaves = true;
        }
        else if (!gameManager.scripts.uiManager.autosavesToggle.isOn)
        {
            selectedAutosaves = false;
        }
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    void OnAutosavesSliderChanged(float sliderValue)
    {
        selectedMaximumAutosaves = (int)gameManager.scripts.uiManager.maximumAutosavesSlider.value;
        gameManager.scripts.uiManager.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.maximumAutosavesSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseAutosavesSlider()
    {
        gameManager.scripts.uiManager.maximumAutosavesSlider.value++;
        selectedMaximumAutosaves = (int)gameManager.scripts.uiManager.maximumAutosavesSlider.value;
        gameManager.scripts.uiManager.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.maximumAutosavesSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseAutosavesSlider()
    {
        gameManager.scripts.uiManager.maximumAutosavesSlider.value--;
        selectedMaximumAutosaves = (int)gameManager.scripts.uiManager.maximumAutosavesSlider.value;
        gameManager.scripts.uiManager.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.maximumAutosavesSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnQuicksavesSliderChanged(float sliderValue)
    {
        selectedMaximumQuicksaves = (int)gameManager.scripts.uiManager.maximumQuicksavesSlider.value;
        gameManager.scripts.uiManager.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.maximumQuicksavesSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseQuicksavesSlider()
    {
        gameManager.scripts.uiManager.maximumQuicksavesSlider.value++;
        selectedMaximumQuicksaves = (int)gameManager.scripts.uiManager.maximumQuicksavesSlider.value;
        gameManager.scripts.uiManager.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.maximumQuicksavesSlider.value.ToString(Strings.numberFormat0);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseQuicksavesSlider()
    {
        gameManager.scripts.uiManager.maximumQuicksavesSlider.value--;
        selectedMaximumQuicksaves = (int)gameManager.scripts.uiManager.maximumQuicksavesSlider.value;
        gameManager.scripts.uiManager.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.maximumQuicksavesSlider.value.ToString(Strings.numberFormat0);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnGoreChanged(bool goreChanged)
    {
        if (gameManager.scripts.uiManager.goreToggle.isOn)
        {
            selectedGore = true;
        }
        else if (!gameManager.scripts.uiManager.autosavesToggle.isOn)
        {
            selectedGore = false;
        }
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    IEnumerator RuntimeMonitorDisplays()
    {
        while (true)
        {
            yield return Times.two;
            //Debug.Log("Checking for new displays....");
            if (Display.displays.Length != lastDisplayCount)
            {
                //Debug.Log("Display configuration has changed!");
                lastDisplayCount = Display.displays.Length;
                DetectDisplays();
            }
        }
    }    
    void OnDisplayDeviceChanged(int deviceIndex)
    {
        selectedDisplayIndex = gameManager.scripts.uiManager.displayDevicesDropdown.value;
        requiresTimedConfirmation = true;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnResolutionChanged(int resolutionIndex)
    {
        selectedResolutionIndex = gameManager.scripts.uiManager.resolutionsDropdown.value;
        requiresTimedConfirmation = true;
        //Debug.Log($"Selected resolution index updated: {selectedResolutionIndex}. Will apply on ApplySettings().");
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnDisplayModeChanged(int displayModeIndex)
    {
        selectedDisplayModeIndex = gameManager.scripts.uiManager.displayModeDropdown.value;
        requiresTimedConfirmation = true;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnFrameRateCapToggleChanged(bool frameRateCapChanged)
    {
        if (gameManager.scripts.uiManager.frameRateCapToggle.isOn)
        {
            selectedFrameRateCap = true;
        }
        else if (!gameManager.scripts.uiManager.frameRateCapToggle.isOn)
        {
            selectedFrameRateCap = false;
        }
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    void OnframeRateCapSliderChanged(float sliderValue)
    {
        selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.frameRateCapSlider.value;
        gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseFrameRateCap()
    {
        gameManager.scripts.uiManager.frameRateCapSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.frameRateCapSlider.value + 1, gameManager.scripts.uiManager.frameRateCapSlider.minValue, gameManager.scripts.uiManager.frameRateCapSlider.maxValue);
        selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.frameRateCapSlider.value;
        gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseFrameRateCap()
    {
        gameManager.scripts.uiManager.frameRateCapSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.frameRateCapSlider.value - 1, gameManager.scripts.uiManager.frameRateCapSlider.minValue, gameManager.scripts.uiManager.frameRateCapSlider.maxValue);
        selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.frameRateCapSlider.value;
        gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnVSyncChanged(int vSyncIndex)
    {
        selectedVsyncIndex = gameManager.scripts.uiManager.vSyncDropdown.value;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnAntiAliasChanged(int antiAliasIndex)
    {
        selectedAntiAliasIndex = gameManager.scripts.uiManager.antiAliasDropdown.value;// Get selected display index from the dropdown
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnQualitySettingsChanged(int qualityIndex)
    {
        selectedQualityAssetIndex = gameManager.scripts.uiManager.qualityDropdown.value;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetMasterVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.audioMasterSlider, gameManager.scripts.uiManager.audioMasterNumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseMasterVolume()
    {
        ChangeAudioSliderValue(ref selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.audioMasterSlider, gameManager.scripts.uiManager.audioMasterNumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseMasterVolume()
    {
        ChangeAudioSliderValue(ref selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.audioMasterSlider, gameManager.scripts.uiManager.audioMasterNumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetMusicVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.audioMusicSlider, gameManager.scripts.uiManager.audioMusicNumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseMusicVolume()
    {
        ChangeAudioSliderValue(ref selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.audioMusicSlider, gameManager.scripts.uiManager.audioMusicNumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseMusicVolume()
    {
        ChangeAudioSliderValue(ref selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.audioMusicSlider, gameManager.scripts.uiManager.audioMusicNumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetAmbientVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.audioAmbientSlider, gameManager.scripts.uiManager.audioAmbientNumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseAmbientVolume()
    {
        ChangeAudioSliderValue(ref selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.audioAmbientSlider, gameManager.scripts.uiManager.audioAmbientNumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseAmbientVolume()
    {
        ChangeAudioSliderValue(ref selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.audioAmbientSlider, gameManager.scripts.uiManager.audioAmbientNumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetSFXVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedSfxVolume, Strings.audioAmbient, gameManager.scripts.uiManager.audioSFXSlider, gameManager.scripts.uiManager.audioSFXNumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseSFXVolume()
    {
        ChangeAudioSliderValue(ref selectedSfxVolume, Strings.audioSFX, gameManager.scripts.uiManager.audioSFXSlider, gameManager.scripts.uiManager.audioSFXNumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseSFXVolume()
    {
        ChangeAudioSliderValue(ref selectedSfxVolume, Strings.audioSFX, gameManager.scripts.uiManager.audioSFXSlider, gameManager.scripts.uiManager.audioSFXNumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetUIVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.audioUISlider, gameManager.scripts.uiManager.audioUINumberText, true, false, false);
        ChangeAudioSliderValue(ref selectedUiVolume, Strings.audioUIPitchShiftVolume, gameManager.scripts.uiManager.audioUISlider, gameManager.scripts.uiManager.audioUINumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseUIVolume()
    {
        ChangeAudioSliderValue(ref selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.audioUISlider, gameManager.scripts.uiManager.audioUINumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseUIVolume()
    {
        ChangeAudioSliderValue(ref selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.audioUISlider, gameManager.scripts.uiManager.audioUINumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetVoiceVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.audioVoiceSlider, gameManager.scripts.uiManager.audioVoiceNumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseVoiceVolume()
    {
        ChangeAudioSliderValue(ref selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.audioVoiceSlider, gameManager.scripts.uiManager.audioVoiceNumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseVoiceVolume()
    {
        ChangeAudioSliderValue(ref selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.audioVoiceSlider, gameManager.scripts.uiManager.audioVoiceNumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetEventVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.audioEventSlider, gameManager.scripts.uiManager.audioEventNumberText, true, false, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseEventVolume()
    {
        ChangeAudioSliderValue(ref selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.audioEventSlider, gameManager.scripts.uiManager.audioEventNumberText, true, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseEventVolume()
    {
        ChangeAudioSliderValue(ref selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.audioEventSlider, gameManager.scripts.uiManager.audioEventNumberText, false, true, false);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    private void ChangeAudioSliderValue(ref float optionsProperty, string mixerProperty, Slider slider, TextMeshProUGUI sliderText, bool increase, bool button, bool setup)
    {
        const float MIN_DB = -80f;// Min + Max is set to full possible range of the mixer, don't change.
        const float MAX_DB = 20f;
        float newSliderValue;
        float dB;        
        float change = increase ? 1 : -1;

        if (!setup)
        {
            if (button)
            {
                newSliderValue = Mathf.Clamp(slider.value + change, slider.minValue, slider.maxValue);
                slider.value = newSliderValue;
            }
            else
            {
                newSliderValue = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);
            }
        }
        else
        {
            if(slider == gameManager.scripts.uiManager.audioMasterSlider)// Master volume set to 100% by default
            {
                newSliderValue = Mathf.InverseLerp(MIN_DB, MAX_DB, MAX_DB) * slider.maxValue;// Convert master dB to slider range (0 - 100)
            }
            else
            {                
                newSliderValue = Mathf.InverseLerp(MIN_DB, MAX_DB, -30f) * slider.maxValue;
            }
            slider.value = newSliderValue;
        }

        dB = Mathf.Lerp(MIN_DB, MAX_DB, newSliderValue / slider.maxValue);
        gameManager.scripts.audioManager.mainMixer.SetFloat(mixerProperty, dB);
        optionsProperty = dB;
        sliderText.text = slider.value.ToString(Strings.numberFormat0) + "<color=red>%</color>";        
    }

    void OnCameraZoomSensitivityChanged(float sliderValue)
    {
        selectedCameraZoomSensitivity = gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraZoomSensitivitySliderText.text = gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value += 0.01f;
        selectedCameraZoomSensitivity = gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraZoomSensitivitySliderText.text = gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value -= 0.01f;
        selectedCameraZoomSensitivity = gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraZoomSensitivitySliderText.text = gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnCameraMoveSensitivityChanged(float sliderValue)
    {
        selectedCameraMoveSensitivity = gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraMoveSensitivitySliderText.text = gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value += 0.01f;
        selectedCameraMoveSensitivity = gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraMoveSensitivitySliderText.text = gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value -= 0.01f;
        selectedCameraMoveSensitivity = gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraMoveSensitivitySliderText.text = gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnCameraPanSensitivityChanged(float sliderValue)
    {
        selectedCameraPanSensitivity = gameManager.scripts.uiManager.cameraPanSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraPanSensitivitySliderText.text = gameManager.scripts.uiManager.cameraPanSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.cameraPanSensitivitySlider.value += 0.01f;
        selectedCameraPanSensitivity = gameManager.scripts.uiManager.cameraPanSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraPanSensitivitySliderText.text = gameManager.scripts.uiManager.cameraPanSensitivitySlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.cameraPanSensitivitySlider.value -= 0.01f;
        selectedCameraPanSensitivity = gameManager.scripts.uiManager.cameraPanSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraPanSensitivitySliderText.text = gameManager.scripts.uiManager.cameraPanSensitivitySlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnGamepadCameraZoomSensitivityChanged(float sliderValue)
    {
        selectedGamepadCameraZoomSensitivity = gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseGamepadCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value += 0.01f;
        selectedGamepadCameraZoomSensitivity = gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseGamepadCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value -= 0.01f;
        selectedGamepadCameraZoomSensitivity = gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnGamepadCameraMoveSensitivityChanged(float sliderValue)
    {
        selectedGamepadCameraMoveSensitivity = gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseGamepadCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value += 0.01f;
        selectedGamepadCameraMoveSensitivity = gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseGamepadCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value -= 0.01f;
        selectedGamepadCameraMoveSensitivity = gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnGamepadCameraPanSensitivityChanged(float sliderValue)
    {
        selectedGamepadCameraPanSensitivity = gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseGamepadCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value += 0.01f;
        selectedGamepadCameraPanSensitivity = gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseGamepadCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value -= 0.01f;
        selectedGamepadCameraPanSensitivity = gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value;
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySliderText.text = gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    #endregion

    void OnGamepadCameraZoomDeadzoneChanged(float sliderValue)
    {
        selectedGamepadCameraZoomDeadzone = gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseGamepadCameraZoomDeadzone()
    {
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value += 0.01f;
        selectedGamepadCameraZoomDeadzone = gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value;
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseGamepadCameraZoomDeadzone()
    {
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value -= 0.01f;
        selectedGamepadCameraZoomDeadzone = gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value;
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnGamepadCameraMoveDeadzoneChanged(float sliderValue)
    {
        selectedGamepadCameraMoveDeadzone = gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseGamepadCameraMoveDeadzone()
    {
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value += 0.01f;
        selectedGamepadCameraMoveDeadzone = gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value;
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseGamepadCameraMoveDeadzone()
    {
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value -= 0.01f;
        selectedGamepadCameraMoveDeadzone = gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value;
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnGamepadCameraPanDeadzoneChanged(float sliderValue)
    {
        selectedGamepadCameraPanDeadzone = gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseGamepadCameraPanDeadzone()
    {
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value += 0.01f;
        selectedGamepadCameraPanDeadzone = gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value;
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseGamepadCameraPanDeadzone()
    {
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value -= 0.01f;
        selectedGamepadCameraPanDeadzone = gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value;
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSliderText.text = gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value.ToString(Strings.numberFormat1);
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    // Descriptions
    public void OnActivateSettingHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsActivateSetting;
    }
    public void OnInvertSettingHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsInvertSetting;
    }
    public void OnRevertSettingHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsRevertSetting;
    }
    public void OnDescClear()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDescClear;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }

    public void OnAutosavesHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAutosavesDesc;
    }
    public void OnMaximumAutosavesHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsMaximumAutosavesDesc;
    }
    public void OnMaximumQuicksavesHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsMaximumQuicksavesDesc;
    }
    public void OnGoreHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGoreDesc;
    }
    public void OnDisplayDeviceHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDisplayDeviceDesc;
    }
    public void OnDisplayAdapterHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDisplayAdapterDesc;
    }
    public void OnResolutionsHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsResolutionDesc;
    }
    public void OnDisplayModeHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsDisplayModeDesc;
    }
    public void OnVSyncHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsVSyncDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsVSyncDescAdditional;
    }
    public void OnFramerateCapHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsFramerateCapDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFramerateCapDescAdditional;
    }
    public void OnAntiAliasHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAntiAliasDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsAntiAliasDescAdditional;
    }
}

