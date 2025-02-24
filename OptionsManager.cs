using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;// Works on Steamdeck
using System.Collections.Generic;// Works on Steamdeck
using System.Linq;// Works on Steamdeck
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionsManager : MonoBehaviour
{
    // MAKE SURE ALL SPAWNED OBJECTS WITH RENDERERS CALL gameManager.scripts.optionsManager.SetAnsio

    GameManager gameManager;
    public Volume postProcess;
    Fog fog;
    Bloom bloom;
    Tonemapping tonemapping;
    GlobalIllumination globalIllumination;
    private string gpuName;// Graphics card name.
    Display[] displays;// To help automatically detect if someone connects another display.
    FullScreenMode screenMode;
    float refreshRate;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    Resolution selectedResolution;
    int lastDisplayCount; // To help automatically detect if someone connects another display, set to detect every 2 seconds at default.
    HDRenderPipelineAsset appliedHDRPAsset;
    RayCastingMode rayMode;
    bool rayTracingSupported;
    TimeSpan timeSpan; // for keep settings timer

    [Header("---- SELECTED ----------------------------------------------------")]
    [SerializeField] private List<string> desiredList;
    [Header("Gameplay")]
    [SerializeField] private bool selectedAutosaves;
    [SerializeField] private int selectedMaximumAutosaves;
    [SerializeField] private int selectedMaximumQuicksaves;
    [SerializeField] private bool selectedGore;
    [Header("Video")]
    [SerializeField] private bool requiresTimedConfirmation;
    [SerializeField] private float requiresTimedConfirmationTimer = 25;
    [Space(10)]
    [SerializeField] private int selectedDisplayIndex;
    [SerializeField] private int selectedResolutionIndex;
    [SerializeField] private int selectedDisplayModeIndex;
    [SerializeField] private bool selectedFrameRateCap;
    [SerializeField] private int selectedFrameRateCapValue;
    [SerializeField] private int selectedVsyncIndex;
    [SerializeField] private int selectedAntiAliasIndex;    
    [SerializeField] private int selectedQualityAssetIndex;
    [SerializeField] private int selectedFogIndex;
    [SerializeField] private int selectedFOV;
    [SerializeField] private int selectedBloomIndex;
    [SerializeField] private bool selectedHDR;
    [SerializeField] private int selectedAnsioIndex;
    [SerializeField] private int selectedTonemappingIndex;
    [SerializeField] private int selectedGlobalIlluminationIndex;    
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
    [Header("---- APPLIED ----------------------------------------------------")] // ACCESSED BY SAVE+LOAD
    public bool appliedAutosaves;
    public int appliedMaximumAutosaves;
    public int appliedMaximumQuicksaves;
    public bool appliedGore;
    public int appliedDisplayIndex;
    public int appliedResolutionIndex;
    public int appliedDisplayModeIndex;
    public bool appliedFrameRateCap;
    public int appliedFrameRateCapValue;
    public int appliedVsyncIndex;
    public int appliedAntiAliasIndex;
    public int appliedQualityAssetIndex;
    public int appliedFogIndex;
    public int appliedFOV;
    public int appliedBloomIndex;
    public bool appliedHDR;
    public int appliedAnsioIndex;
    public int appliedTonemappingIndex;
    public float appliedMasterVolume;
    public float appliedMusicVolume;
    public float appliedAmbientVolume;
    public float appliedSfxVolume;
    public float appliedUiVolume;
    public float appliedVoiceVolume;
    public float appliedEventVolume;
    public float appliedCameraZoomSensitivity;
    public float appliedCameraMoveSensitivity;
    public float appliedCameraPanSensitivity;
    public float appliedGamepadCameraZoomSensitivity;
    public float appliedGamepadCameraMoveSensitivity;
    public float appliedGamepadCameraPanSensitivity;
    public float appliedGamepadCameraZoomDeadzone;
    public float appliedGamepadCameraMoveDeadzone;
    public float appliedGamepadCameraPanDeadzone;

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.optionsManager = this;
    }

    void Start()
    {          
        PopulateSavesSliders();
        PopulateGore();
        DetectDisplays();// DISPLAY DEVICE
        StartCoroutine(RuntimeMonitorDisplays());// WILL UPDATE DISPLAY DEVICE DROPDOWN AT RUNTIME IF MONITORS ARE ADDED / REMOVED
        PopulateDisplayAdapter();
        PopulateDisplayModeDropdown();
        PopulateFrameRateCapSlider();
        PopulateVSyncDropdown();
        PopulateQualityDropdown();
        PopulateAntiAliasDropdown();        
        PopulateFogDropdown();
        PopulateFovSlider();
        PopulateBloomDropdown();
        PopulateHDR();
        PopulateAnsioDropdown();
        PopulateTonemappingDropdown();
        PopulateGlobalIlluminationDropdown();
        PopulateAudioSliders();
        PopulateMouseSliders();
        PopulateGamepadSliders();

        desiredList.Clear();
        Utils.ClearMemory();
    }

    private void Update()
    {
        if (gameManager.scripts.uiManager.confirmOptionsChanges.activeSelf)
        {            
            requiresTimedConfirmationTimer -= Time.deltaTime;
            timeSpan = TimeSpan.FromSeconds(requiresTimedConfirmationTimer);
            gameManager.scripts.uiManager.confirmOptionsChangesTimerText.text = string.Format("{0:D2} <color=" + Colors.textRed + ">:</color> {1:D2} <color=" + Colors.textRed + ">:</color> {2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            if (requiresTimedConfirmationTimer <= 0f)
            {
                RevertChanges();
                requiresTimedConfirmation = false;
                Utils.DeActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges);
            }
        }
        else if (requiresTimedConfirmationTimer != 25f)
        {
            requiresTimedConfirmationTimer = 25f;
        }
    }


    public void ApplySettings()
    {
        if (Application.isPlaying)
        {
            // GAMEPLAY--------------


            // VIDEO--------------
            if (selectedDisplayIndex != appliedDisplayIndex)
            {
                Display.displays[selectedDisplayIndex].Activate();
                requiresTimedConfirmation = true;
            }

            if (selectedDisplayModeIndex != appliedDisplayModeIndex)
            {
                if (selectedDisplayModeIndex == 1)
                {
                    screenMode = FullScreenMode.Windowed;
                }
                else
                {
                    screenMode = FullScreenMode.FullScreenWindow;
                }
                requiresTimedConfirmation = true;
            }



            if (filteredResolutions.Count > selectedResolutionIndex)
            {
                selectedResolution = filteredResolutions[selectedResolutionIndex];// Get the chosen resolution from the filtered list

                if (selectedResolutionIndex != appliedResolutionIndex)
                {
                    Screen.SetResolution(selectedResolution.width, selectedResolution.height, screenMode, selectedResolution.refreshRateRatio);
                    //Debug.Log($"Applied resolution: {selectedResolution.width}<color=red>x</color>{selectedResolution.height} {selectedResolution.refreshRateRatio.numerator}/{selectedResolution.refreshRateRatio.denominator}<color=red>Hz</color>");
                    requiresTimedConfirmation = true;
                }
                else if (selectedDisplayModeIndex != appliedDisplayModeIndex)
                {
                    Screen.SetResolution(selectedResolution.width, selectedResolution.height, screenMode, selectedResolution.refreshRateRatio);
                    //Debug.Log($"Applied resolution: {selectedResolution.width}<color=red>x</color>{selectedResolution.height} {selectedResolution.refreshRateRatio.numerator}/{selectedResolution.refreshRateRatio.denominator}<color=red>Hz</color>");
                    requiresTimedConfirmation = true;
                }
            }
            else
            {
                Debug.Log("Tried to switch to a resolution index that doesn't exist.");
            }

            if (selectedVsyncIndex != appliedVsyncIndex)
            {
                if (selectedVsyncIndex != 0)// If VSync is on
                {
                    if (selectedVsyncIndex == 1)
                    {
                        Application.targetFrameRate = (int)selectedResolution.refreshRateRatio.value;
                        gameManager.scripts.uiManager.frameRateCapSliderText.text = "<color=" + Colors.textCyan + ">" + ((int)selectedResolution.refreshRateRatio.value).ToString(Strings.numberFormat0) + " </color>";
                        gameManager.scripts.uiManager.frameRateCapSlider.value = (int)selectedResolution.refreshRateRatio.value;
                    }
                    else if (selectedVsyncIndex == 2)
                    {
                        Application.targetFrameRate = (int)(selectedResolution.refreshRateRatio.value / 2);
                        gameManager.scripts.uiManager.frameRateCapSliderText.text = "<color=" + Colors.textCyan + ">" + ((int)(selectedResolution.refreshRateRatio.value / 2)).ToString(Strings.numberFormat0) + " </color>";
                        gameManager.scripts.uiManager.frameRateCapSlider.value = (int)selectedResolution.refreshRateRatio.value / 2;
                    }
                }
            }

            if (selectedFrameRateCapValue != appliedFrameRateCapValue)
            {
                if (selectedVsyncIndex != 0)
                {
                    if (selectedFrameRateCap)// If VSync is off use framerate cap
                    {
                        if (selectedFrameRateCapValue > selectedResolution.refreshRateRatio.value)// Cap to monitors max refresh rate
                        {
                            //Debug.Log("Selected FramerateCap value of " + selectedFrameRateCapValue + " is higher than monitors max refresh of " + selectedResolution.refreshRateRatio + ", clamping to that value");
                            Application.targetFrameRate = (int)selectedResolution.refreshRateRatio.value;
                            selectedFrameRateCapValue = (int)selectedResolution.refreshRateRatio.value;
                            gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
                            gameManager.scripts.uiManager.frameRateCapSliderText.text = "<color=" + Colors.textCyan + ">" + gameManager.scripts.uiManager.frameRateCapSlider.value.ToString(Strings.numberFormat0) + " </color>";
                        }
                        else
                        {
                            Application.targetFrameRate = selectedFrameRateCapValue;
                        }
                    }
                    else
                    {
                        Application.targetFrameRate = -1;
                    }
                }
            }

            if (selectedAntiAliasIndex == 0)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;// TURN OFF FXAA, SMAA, TAA
            }
            else if (selectedAntiAliasIndex == 1)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;// TURN ON FXAA
            }
            else if (selectedAntiAliasIndex == 2)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;// TURN ON SMAA
                gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Low;// SET TO 2X
            }
            else if (selectedAntiAliasIndex == 3)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;// TURN ON SMAA
                gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Medium;// SET TO 4X
            }
            else if (selectedAntiAliasIndex == 4)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;// TURN ON SMAA
                gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.High;// SET TO 8X
            }
            else if (selectedAntiAliasIndex == 5)
            {
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                QualitySettings.antiAliasing = 1; // TURN ON MSAA LOW
            }
            else if (selectedAntiAliasIndex == 6)
            {
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                QualitySettings.antiAliasing = 2; // TURN ON MSAA MEDIUM
            }
            else if (selectedAntiAliasIndex == 7)
            {
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                QualitySettings.antiAliasing = 3; // TURN ON MSAA HIGH
            }
            else if (selectedAntiAliasIndex == 8)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing; // TURN ON TAA
                gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.Low;// SET TO 2X
            }
            else if (selectedAntiAliasIndex == 9)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing; // TURN ON TAA                 
                gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.Medium;// SET TO 4X
            }
            else if (selectedAntiAliasIndex == 10)
            {
                QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing; // TURN ON TAA                
                gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.High;// SET TO 8X
            }

            if (fog)
            {
                if (selectedFogIndex == 0)
                {
                    fog.enableVolumetricFog.value = false;
                    fog.active = false;
                    fog.enabled.value = false;
                }
                else
                {
                    fog.enabled.value = true;
                    fog.active = true;
                    fog.enableVolumetricFog.value = true;

                    if (selectedFogIndex == 1)
                    {
                        fog.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                    }
                    else if (selectedFogIndex == 2)
                    {
                        fog.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    }
                    else if (selectedFogIndex == 3)
                    {
                        fog.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    }
                    //Debug.Log("Fog Quality Set to: " + (ScalableSettingLevelParameter.Level)fog.quality.value);
                }
            }            

            gameManager.scripts.cameraManager.cinemachineCamera.Lens.FieldOfView = selectedFOV;

            if (bloom)
            {
                if (selectedBloomIndex == 0)
                {
                    bloom.active = false;
                }
                else
                {
                    bloom.active = true;

                    if (selectedBloomIndex == 1)
                    {
                        bloom.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                    }
                    else if (selectedBloomIndex == 2)
                    {
                        bloom.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    }
                    else if (selectedBloomIndex == 3)
                    {
                        bloom.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    }
                    //Debug.Log("Bloom Quality Set to: " + (ScalableSettingLevelParameter.Level)bloom.quality.value);
                }
            }

            if (selectedHDR)
            {
                gameManager.scripts.cameraManager.mainCamera.allowHDR = true;
            }
            else
            {
                gameManager.scripts.cameraManager.mainCamera.allowHDR = false;
            }

            SetAnsio(null, true);

            if (tonemapping)
            {
                if (selectedTonemappingIndex == 0)
                {
                    tonemapping.mode.value = TonemappingMode.Neutral;
                    tonemapping.neutralHDRRangeReductionMode.value = NeutralRangeReductionMode.Reinhard;
                }
                else if (selectedTonemappingIndex == 1)
                {
                    tonemapping.mode.value = TonemappingMode.Neutral;
                    tonemapping.neutralHDRRangeReductionMode.value = NeutralRangeReductionMode.BT2390;
                }
                else if (selectedTonemappingIndex == 2)
                {
                    tonemapping.mode.value = TonemappingMode.ACES;
                    tonemapping.acesPreset.value = HDRACESPreset.ACES1000Nits;
                }
                else if (selectedTonemappingIndex == 3)
                {
                    tonemapping.mode.value = TonemappingMode.ACES;
                    tonemapping.acesPreset.value = HDRACESPreset.ACES2000Nits;
                }
                else if (selectedTonemappingIndex == 4)
                {
                    tonemapping.mode.value = TonemappingMode.ACES;
                    tonemapping.acesPreset.value = HDRACESPreset.ACES4000Nits;
                }

            }

            if (globalIllumination)
            {
                if (selectedGlobalIlluminationIndex == 0)
                {
                    globalIllumination.active = false;
                    globalIllumination.enable.value = false;
                }
                else if (selectedGlobalIlluminationIndex == 1)
                {
                    globalIllumination.active = true;
                    globalIllumination.enable.value = true;
                    globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                    globalIllumination.tracing.value = RayCastingMode.RayMarching;
                }
                else if (selectedGlobalIlluminationIndex == 2)
                {
                    globalIllumination.active = true;
                    globalIllumination.enable.value = true;
                    globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    globalIllumination.tracing.value = RayCastingMode.RayMarching;
                }
                else if (selectedGlobalIlluminationIndex == 3)
                {
                    globalIllumination.active = true;
                    globalIllumination.enable.value = true;
                    globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    globalIllumination.tracing.value = RayCastingMode.RayMarching;
                }
                else if (selectedGlobalIlluminationIndex == 4)
                {
                    globalIllumination.active = true;
                    globalIllumination.enable.value = true;
                    globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                    
                    if (!rayTracingSupported)
                    {
                        rayMode = RayCastingMode.RayMarching;
                        selectedGlobalIlluminationIndex = 1;
                        gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
                        globalIllumination.tracing.value = rayMode;
                    }
                    else
                    {
                        rayMode = RayCastingMode.RayTracing;
                        globalIllumination.tracing.value = rayMode;
                    }   
                }
                else if (selectedGlobalIlluminationIndex == 5)
                {
                    globalIllumination.active = true;
                    globalIllumination.enable.value = true;
                    globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;

                    if (!rayTracingSupported)
                    {
                        rayMode = RayCastingMode.RayMarching;
                        selectedGlobalIlluminationIndex = 2;
                        gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
                        globalIllumination.tracing.value = rayMode;
                    }
                    else
                    {
                        rayMode = RayCastingMode.RayTracing;
                        globalIllumination.tracing.value = rayMode;
                    }
                }
                else if (selectedGlobalIlluminationIndex == 6)
                {
                    globalIllumination.active = true;
                    globalIllumination.enable.value = true;
                    globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.High;

                    if (!rayTracingSupported)
                    {
                        rayMode = RayCastingMode.RayMarching;
                        selectedGlobalIlluminationIndex = 3;
                        gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
                        globalIllumination.tracing.value = rayMode;
                    }
                    else
                    {
                        rayMode = RayCastingMode.RayTracing;
                        globalIllumination.tracing.value = rayMode;
                    }
                }
            }

            if (requiresTimedConfirmation)
            {
                // ACTIVATE UI
                requiresTimedConfirmationTimer -= Time.deltaTime;

                if (requiresTimedConfirmationTimer <= 0)
                {
                    RevertChanges();
                    requiresTimedConfirmationTimer = 24f;
                }
                Utils.ActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges);
            }
            else
            {
                ApplySettingsPermanent();
                requiresTimedConfirmationTimer = 24f;
            }
        }
    }
    public void ApplySettingsPermanent()
    {        
        if (selectedAutosaves)
        {
            appliedAutosaves = true;
        }
        else
        {
            appliedAutosaves = false;
        }

        appliedMaximumAutosaves = selectedMaximumAutosaves;
        appliedMaximumQuicksaves = selectedMaximumQuicksaves;

        if (selectedGore)
        {
            appliedGore = true;
        }
        else
        {
            appliedGore = false;
        }

        appliedResolutionIndex = selectedResolutionIndex;
        appliedDisplayModeIndex = selectedDisplayModeIndex;
        appliedQualityAssetIndex = selectedQualityAssetIndex;
        appliedVsyncIndex = selectedVsyncIndex;
        appliedFrameRateCap = selectedFrameRateCap;
        appliedFrameRateCapValue = selectedFrameRateCapValue;
        appliedAntiAliasIndex = selectedAntiAliasIndex;
        appliedFogIndex = selectedFogIndex;
        appliedFOV = selectedFOV;
        appliedBloomIndex = selectedBloomIndex;
        appliedHDR = selectedHDR;
        appliedAnsioIndex = selectedAnsioIndex;
        appliedTonemappingIndex = selectedTonemappingIndex;

        appliedMasterVolume = selectedMasterVolume;
        appliedMusicVolume = selectedMusicVolume;
        appliedAmbientVolume = selectedAmbientVolume;
        appliedSfxVolume = selectedSfxVolume;
        appliedUiVolume = selectedUiVolume;
        appliedVoiceVolume = selectedVoiceVolume;
        appliedEventVolume = selectedEventVolume;

        requiresTimedConfirmation = false;
        Utils.DeActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges);
    }
    public void RevertChanges()
    {
        // SELECTED = APPLIED
        // UPDATE UI

        requiresTimedConfirmation = false;
        Utils.DeActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges);
    }
    

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
        lastDisplayCount = Display.displays.Length;// DISPLAY DEVICE
        rayTracingSupported = SystemInfo.supportsRayTracing;
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
    void PopulateDisplayAdapter()
    {
        gpuName = SystemInfo.graphicsDeviceName;// DISPLAY ADAPTER
        gameManager.scripts.uiManager.gpuName.text = gpuName;// DISPLAY ADAPTER
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

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Fullscreen",
            "Windowed",
        };
        gameManager.scripts.uiManager.displayModeDropdown.AddOptions(desiredList);

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
        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;
                     
        if (selectedFrameRateCapValue > refreshRate)
        {
            gameManager.scripts.uiManager.frameRateCapSliderText.text = "<color=" + Colors.textCyan + ">" + refreshRate.ToString(Strings.numberFormat0) + "</color>";
            selectedFrameRateCapValue = (int)refreshRate;
            gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
        }
        else
        {
            gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
            gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
        }        
        gameManager.scripts.uiManager.frameRateCapSlider.onValueChanged.AddListener(OnframeRateCapSliderChanged);
    }
    void PopulateVSyncDropdown()
    {
        gameManager.scripts.uiManager.vSyncDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Off",
            "V<color=red>-</color>Blank",
            "Second V<color=red>-</color>Blank",
        };
        gameManager.scripts.uiManager.vSyncDropdown.AddOptions(desiredList);
        selectedVsyncIndex = 1;
        gameManager.scripts.uiManager.vSyncDropdown.value = selectedVsyncIndex;
        gameManager.scripts.uiManager.vSyncDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.vSyncDropdown.onValueChanged.AddListener(OnVSyncChanged);
        gameManager.scripts.uiManager.vSyncDropdown.RefreshShownValue();// Update the dropdown to reflect the selected VSync
    }
    void PopulateAntiAliasDropdown()
    {
        gameManager.scripts.uiManager.antiAliasDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
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
        };
        gameManager.scripts.uiManager.antiAliasDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.antiAliasDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.antiAliasDropdown.onValueChanged.AddListener(OnAntiAliasChanged);
        gameManager.scripts.uiManager.antiAliasDropdown.value = 3;
        selectedAntiAliasIndex = gameManager.scripts.uiManager.antiAliasDropdown.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.antiAliasDropdown.RefreshShownValue();// Update the dropdown to reflect the selected aa
    }
    void PopulateQualityDropdown()
    {
        appliedHDRPAsset = QualitySettings.renderPipeline as HDRenderPipelineAsset;
        selectedQualityAssetIndex = 1;
        gameManager.scripts.uiManager.qualityDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "High",
            "Medium",
            "Low",
        };
        gameManager.scripts.uiManager.qualityDropdown.AddOptions(desiredList);

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
    void PopulateFogDropdown()
    {
        if (postProcess.profile.TryGet(out fog)){}

        selectedFogIndex = 2;
        gameManager.scripts.uiManager.fogDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Off",
            "Low",
            "Medium",
            "High"
        };
        gameManager.scripts.uiManager.fogDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.fogDropdown.value = selectedFogIndex;
        gameManager.scripts.uiManager.fogDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.fogDropdown.onValueChanged.AddListener(OnFogChanged);
        gameManager.scripts.uiManager.fogDropdown.RefreshShownValue();
    }
    void PopulateFovSlider()
    {
        selectedFOV = 90;
        gameManager.scripts.uiManager.fovSlider.value = selectedFOV;
        gameManager.scripts.uiManager.fovSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.fovSlider.onValueChanged.AddListener(OnFOVChanged);
    }
    void PopulateBloomDropdown()
    {
        if (postProcess.profile.TryGet(out bloom)) { }

        selectedBloomIndex = 2;
        gameManager.scripts.uiManager.bloomDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Off",
            "Low",
            "Medium",
            "High"
        };
        gameManager.scripts.uiManager.bloomDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.bloomDropdown.value = selectedBloomIndex;
        gameManager.scripts.uiManager.bloomDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.bloomDropdown.onValueChanged.AddListener(OnBloomChanged);
        gameManager.scripts.uiManager.bloomDropdown.RefreshShownValue();
    }
    void PopulateHDR()
    {        
        selectedHDR = false;
        gameManager.scripts.uiManager.hdrToggle.isOn = false;
        gameManager.scripts.uiManager.hdrToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.hdrToggle.onValueChanged.AddListener(OnHDRChanged);
    }
    void PopulateAnsioDropdown()
    {
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        selectedAnsioIndex = 2;
        gameManager.scripts.uiManager.ansioDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Off",
            "x2",
            "x4",
            "x8",
            "x16",
        };
        gameManager.scripts.uiManager.ansioDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.ansioDropdown.value = selectedAnsioIndex;
        gameManager.scripts.uiManager.ansioDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.ansioDropdown.onValueChanged.AddListener(OnAnsioChanged);
        gameManager.scripts.uiManager.ansioDropdown.RefreshShownValue();
        SetAnsio(null, true);
    }
    void PopulateTonemappingDropdown()
    {
        if (postProcess.profile.TryGet(out tonemapping)){}

        selectedTonemappingIndex = 2;
        gameManager.scripts.uiManager.tonemappingDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {            
            "Reinhard",
            "BT2390",
            "ACES 1000",
            "ACES 2000",
            "ACES 4000"
        };
        gameManager.scripts.uiManager.tonemappingDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.tonemappingDropdown.value = selectedTonemappingIndex;
        gameManager.scripts.uiManager.tonemappingDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.tonemappingDropdown.onValueChanged.AddListener(OnTonemappingChanged);
        gameManager.scripts.uiManager.tonemappingDropdown.RefreshShownValue();
    }
    void PopulateGlobalIlluminationDropdown()
    {
        if (postProcess.profile.TryGet(out globalIllumination)){}
        
        selectedGlobalIlluminationIndex = 2;
        gameManager.scripts.uiManager.giDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Realtime",
            "SSGI Low",
            "SSGI Medium",
            "SSGI High",
            "RTGI Low",
            "RTGI Medium",
            "RTGI High"
        };
        gameManager.scripts.uiManager.giDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
        gameManager.scripts.uiManager.giDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.giDropdown.onValueChanged.AddListener(OnGlobalIlluminationChanged);
        gameManager.scripts.uiManager.giDropdown.RefreshShownValue();

        /*if (rayTracingSupported)
        {
            Debug.Log("Ray Tracing is supported, Ray Tracing available.");
        }
        else
        {
            Debug.Log("Ray Tracing is not supported, defaulting to Ray Marching.");
        }*/
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

    public void VideoPage1()
    {
        Utils.DeActivateObject(gameManager.scripts.uiManager.optionsVideoPage2);
        Utils.DeActivateObject(gameManager.scripts.uiManager.optionsVideoPage2Tab);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage1Tab);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage1);
    }
    public void VideoPage2()
    {
        Utils.DeActivateObject(gameManager.scripts.uiManager.optionsVideoPage1);
        Utils.DeActivateObject(gameManager.scripts.uiManager.optionsVideoPage1Tab);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage2Tab);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage2);
    }
    public void VideoPageIncrease()
    {
        if (gameManager.scripts.uiManager.optionsVideoPage1.activeSelf)
        {
            VideoPage2();
        }
    }
    public void VideoPageDecrease()
    {
        if (gameManager.scripts.uiManager.optionsVideoPage2.activeSelf)
        {
            VideoPage1();
        }
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
        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;

        if (gameManager.scripts.uiManager.frameRateCapSlider.value >= refreshRate)
        {
            gameManager.scripts.uiManager.frameRateCapSliderText.text = "<color=" + Colors.textCyan + ">" + refreshRate.ToString(Strings.numberFormat0) + "</color>";
            selectedFrameRateCapValue = (int)refreshRate;
            gameManager.scripts.uiManager.frameRateCapSlider.value = (int)refreshRate;
        }
        else if(gameManager.scripts.uiManager.frameRateCapSlider.value < refreshRate)
        {
            gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString(Strings.numberFormat0);
            selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.frameRateCapSlider.value;
        }        
        gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString(Strings.numberFormat0);
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

    void OnFogChanged(int fogIndex)
    {
        selectedFogIndex = gameManager.scripts.uiManager.fogDropdown.value;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnQualitySettingsChanged(int qualityIndex)
    {
        selectedQualityAssetIndex = gameManager.scripts.uiManager.qualityDropdown.value;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void OnFOVChanged(float sliderValue)
    {
        selectedFOV = (int)gameManager.scripts.uiManager.fovSlider.value;
        gameManager.scripts.uiManager.fovSliderText.text = gameManager.scripts.uiManager.fovSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseFOV()
    {
        gameManager.scripts.uiManager.fovSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.fovSlider.value + 1, gameManager.scripts.uiManager.fovSlider.minValue, gameManager.scripts.uiManager.fovSlider.maxValue);
        selectedFOV = (int)gameManager.scripts.uiManager.fovSlider.value;
        gameManager.scripts.uiManager.fovSliderText.text = gameManager.scripts.uiManager.fovSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseFOV()
    {
        gameManager.scripts.uiManager.fovSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.fovSlider.value - 1, gameManager.scripts.uiManager.fovSlider.minValue, gameManager.scripts.uiManager.fovSlider.maxValue);
        selectedFOV = (int)gameManager.scripts.uiManager.fovSlider.value;
        gameManager.scripts.uiManager.fovSliderText.text = gameManager.scripts.uiManager.fovSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnBloomChanged(int bloomIndex)
    {
        selectedBloomIndex = gameManager.scripts.uiManager.bloomDropdown.value;
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnHDRChanged(bool hdrChanged)
    {
        if (gameManager.scripts.uiManager.hdrToggle.isOn)
        {
            selectedHDR = true;
        }
        else if (!gameManager.scripts.uiManager.hdrToggle.isOn)
        {
            selectedHDR = false;
        }
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    void OnAnsioChanged(int ansioIndex)
    {
        selectedAnsioIndex = gameManager.scripts.uiManager.ansioDropdown.value;        
        //ACTIVATE MODIFIED VALUE BUTTON
    }

    public void SetAnsio(Material material, bool scan)
    {
        if (material && material.mainTexture)
        {
            if (selectedAnsioIndex == 0) // Set desired anisotropic level
            {
                material.mainTexture.anisoLevel = 0;
            }
            else if (selectedAnsioIndex == 1)
            {
                material.mainTexture.anisoLevel = 2;
            }
            else if (selectedAnsioIndex == 2)
            {
                material.mainTexture.anisoLevel = 4;
            }
            else if (selectedAnsioIndex == 3)
            {
                material.mainTexture.anisoLevel = 8;
            }
            else if (selectedAnsioIndex == 4)
            {
                material.mainTexture.anisoLevel = 16;
            }
        }
        else
        {
            if (scan)
            {                
                Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);

                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        //Debug.Log("Renderer Found....");
                        if (mat && mat.mainTexture)
                        {
                            //Debug.Log("Material and mainTexture present..");
                            if (selectedAnsioIndex == 0) // Set desired anisotropic level
                            {
                                //Debug.Log("Texture Set to OFF!");
                                mat.mainTexture.anisoLevel = 0;
                            }
                            else if (selectedAnsioIndex == 1)
                            {
                                //Debug.Log("Texture Set to 2x!");
                                mat.mainTexture.anisoLevel = 2;
                            }
                            else if (selectedAnsioIndex == 2)
                            {
                                //Debug.Log("Texture Set to 4x!");
                                mat.mainTexture.anisoLevel = 4;
                            }
                            else if (selectedAnsioIndex == 3)
                            {
                                //Debug.Log("Texture Set to 8x!");
                                mat.mainTexture.anisoLevel = 8;
                            }
                            else if (selectedAnsioIndex == 4)
                            {
                                //Debug.Log("Texture Set to 16x!");
                                mat.mainTexture.anisoLevel = 16;
                            }
                        }
                        else
                        {
                            if (!mat)
                            {
                                //Debug.Log("Material NOT present on " + renderer.name);
                            }
                            else if (!mat.mainTexture)
                            {
                                //Debug.Log("Material Texture NOT present on " + renderer.name);
                            }
                        }
                    }
                }
            }            
        }
        Utils.ClearMemory();
    }

    void OnTonemappingChanged(int tonemappingIndex)
    {
        selectedTonemappingIndex = gameManager.scripts.uiManager.tonemappingDropdown.value;        
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    void OnGlobalIlluminationChanged(int globalIlluminationIndex)
    {
        selectedGlobalIlluminationIndex = gameManager.scripts.uiManager.giDropdown.value;
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
    #endregion



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
    public void OnPageIncreaseHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsNextPage;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
    }
    public void OnPageDecreaseHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPreviousPage;
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
    public void OnFramerateCapInfoHover()
    {
        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;

        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFramerateCapInfoDesc + "<color=" + Colors.textCyan + ">" + refreshRate.ToString("F0") + " Hz</color>.";
    }
    public void OnQualityHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsQualityDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsQualityDescAdditional;
    }
    public void OnAntiAliasHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAntiAliasDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsAntiAliasDescAdditional;
    }
    public void OnVolumetricFogHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsVolumetricFogDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsVolumetricFogDescAdditional;
    }
    public void OnFOVHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsFOVDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsFOVDescAdditional;
    }
    public void OnBloomHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsBloomDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsBloomDescAdditional;
    }
    public void OnHDRHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsHDRDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsHDRDescAdditional;
    }
    public void OnAnsioHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsAnsioDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsAnsioDescAdditional;
    }
    public void OnTonemappingHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsTonemappingDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsTonemappingDescAdditional;
    }
    public void OnGlobalIlluminationHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDescAdditional;
    }
    public void OnGlobalIlluminationInfoHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDesc;

        if (rayTracingSupported)
        {
            gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDescAdditional3;
        }
        else
        {
            gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationDescAdditional2;
        }
    }
}

