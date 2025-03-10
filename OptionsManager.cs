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
    // MAKE SURE ALL SPAWNED OBJECTS WITH PLANAR REFLECTION PROBES CALL gameManager.scripts.optionsManager.SetPlanarReflection()

    GameManager gameManager;
    [SerializeField] private Volume postProcess;
    [SerializeField] private Fog fog;
    [SerializeField] private Bloom bloom;
    [SerializeField] private Tonemapping tonemapping;
    [SerializeField] private GlobalIllumination globalIllumination;
    [SerializeField] private ScreenSpaceReflection reflections;
    //[SerializeField] private HDAdditionalReflectionData planarReflection; // possibly needed to determine Planar Reflection resolution based on camera distance at runtime
    private string gpuName;// Graphics card name.
    Display[] displays;// To help automatically detect if someone connects another display.
    FullScreenMode screenMode;
    float refreshRate;
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    Resolution selectedResolution;
    int lastDisplayCount; // To help automatically detect if someone connects another display, set to detect every 2 seconds at default.
    [SerializeField] private HDRenderPipelineAsset appliedHDRPAsset;
    RayCastingMode rayMode;
    bool rayTracingSupported;
    TimeSpan timeSpan; // for keep settings timer
    [HideInInspector] public List<string> desiredList;// ACCESSED BY POPULATE METHODS BELOW
    [Header("---- SELECTED ----------------------------------------------------")]
    [SerializeField] private bool requiresTimedConfirmation;
    [SerializeField] private float requiresTimedConfirmationTimer = 25;
    [Space(5)]
    [Header("Gameplay")]
    [SerializeField] private bool selectedAutosaves;
    [SerializeField] private int selectedMaximumAutosaves;
    [SerializeField] private int selectedMaximumQuicksaves;
    [SerializeField] private bool selectedGore;
    [Header("Video")]    
    [SerializeField] private int selectedDisplayIndex;
    [SerializeField] private int selectedResolutionIndex;
    [SerializeField] private int selectedDisplayModeIndex;
    [SerializeField] private bool selectedFrameRateCap;
    [SerializeField] private int selectedFrameRateCapValue;
    [SerializeField] private int selectedVsyncIndex;
    [SerializeField] private int selectedAntiAliasIndex;
    [SerializeField] private int selectedTaaQualityIndex;
    [SerializeField] private int selectedQualityAssetIndex;
    [SerializeField] private int selectedFogIndex;
    [SerializeField] private int selectedFOV;
    [SerializeField] private int selectedRenderDistance;
    [SerializeField] private int selectedBloomIndex;
    [SerializeField] private bool selectedHDR;
    [SerializeField] private int selectedAnsioIndex;
    [SerializeField] private int selectedTonemappingIndex;
    [SerializeField] private bool selectedGlobalIlluminationFullRes;
    [SerializeField] private int selectedGlobalIlluminationIndex;
    [SerializeField] private int selectedReflectionsIndex;
    [SerializeField] private bool selectedPlanarReflections;
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
    [Header("Video")]
    public int appliedDisplayIndex;
    public int appliedResolutionIndex;
    public int appliedDisplayModeIndex;
    public bool appliedFrameRateCap;
    public int appliedFrameRateCapValue;
    public int appliedVsyncIndex;
    public int appliedAntiAliasIndex;
    public int appliedTaaQualityIndex;
    public int appliedQualityAssetIndex;
    public int appliedFogIndex;
    public int appliedFOV;
    public int appliedRenderDistance;
    public int appliedBloomIndex;
    public bool appliedHDR;
    public int appliedAnsioIndex;
    public int appliedTonemappingIndex;
    public bool appliedGlobalIlluminationFullRes;
    public int appliedGlobalIlluminationIndex;
    public int appliedReflectionsIndex;
    public bool appliedPlanarReflections;
    [Header("Audio")]
    public float appliedMasterVolume;
    public float appliedMusicVolume;
    public float appliedAmbientVolume;
    public float appliedSfxVolume;
    public float appliedUiVolume;
    public float appliedVoiceVolume;
    public float appliedEventVolume;
    [Header("Keyboard")]
    public float appliedCameraZoomSensitivity;
    public float appliedCameraMoveSensitivity;
    public float appliedCameraPanSensitivity;
    [Header("Gamepad")]
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
        PopulateTAAQualityDropdown();
        PopulateFogDropdown();
        PopulateFovSlider();
        PopulateRenderDistanceSlider();
        PopulateBloomDropdown();
        PopulateHDR();
        PopulateAnsioDropdown();
        PopulateTonemappingDropdown();
        PopulateGlobalIlluminationDropdown();
        PopulateReflectionsDropdown();
        PopulateAudioSliders();
        PopulateMouseSliders();
        PopulateGamepadSliders();

        ApplySettings();
        desiredList.Clear();
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
                Utils.ActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges, false);
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

                if (selectedResolutionIndex != appliedResolutionIndex || selectedDisplayModeIndex != appliedDisplayModeIndex)
                {
                    Screen.SetResolution(selectedResolution.width, selectedResolution.height, screenMode, selectedResolution.refreshRateRatio);
                    //Debug.Log($"Applied resolution: {selectedResolution.width}<color=red>x</color>{selectedResolution.height} {selectedResolution.refreshRateRatio.numerator}/{selectedResolution.refreshRateRatio.denominator}<color=red>Hz</color>");
                    requiresTimedConfirmation = true;
                }
            }
            else
            {
                Debug.Log("Tried to switch to a resolution index that doesn't exist.");
                if (selectedDisplayModeIndex != appliedDisplayModeIndex)// If Display Mode has changed
                {
                    Screen.SetResolution(Screen.width, Screen.height, screenMode, selectedResolution.refreshRateRatio);
                    requiresTimedConfirmation = true;
                }                
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
                if (selectedVsyncIndex == 0)// If VSync is off
                {
                    if (selectedFrameRateCap)// use framerate cap
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
                        Application.targetFrameRate = -1;// if not, unlimited
                    }
                }
            }


            if (selectedAntiAliasIndex != appliedAntiAliasIndex)
            {
                if (selectedAntiAliasIndex == 0 || selectedAntiAliasIndex == 1 || selectedAntiAliasIndex == 2 || selectedAntiAliasIndex == 3 || selectedAntiAliasIndex == 4)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    if (selectedAntiAliasIndex == 0)
                    {                        
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;// TURN OFF FXAA, SMAA, TAA
                    }
                    else if (selectedAntiAliasIndex == 1)
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;// TURN ON FXAA
                    }
                    else
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;// TURN ON SMAA
                        if (selectedAntiAliasIndex == 2)
                        {
                            gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Low;// SET TO 2X
                        }
                        else if (selectedAntiAliasIndex == 2)
                        {
                            gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Medium;// SET TO 4X
                        }
                        else if (selectedAntiAliasIndex == 2)
                        {
                            gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.High;// SET TO 8X
                        }
                    }
                }
                else
                {
                    if (selectedAntiAliasIndex == 5 || selectedAntiAliasIndex == 6 || selectedAntiAliasIndex == 7)
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                    }
                    else
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing; // TURN ON TAA
                    }

                    if (selectedAntiAliasIndex == 5)
                    {
                        QualitySettings.antiAliasing = 1; // TURN ON MSAA LOW
                    }
                    else if (selectedAntiAliasIndex == 6)
                    {
                        QualitySettings.antiAliasing = 2; // TURN ON MSAA MEDIUM
                    }
                    else if (selectedAntiAliasIndex == 7)
                    {
                        QualitySettings.antiAliasing = 3; // TURN ON MSAA HIGH
                    }
                    else if (selectedAntiAliasIndex == 8)
                    {
                        gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.Low;// SET TO 2X
                    }
                    else if (selectedAntiAliasIndex == 9)
                    {
                        gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.Medium;// SET TO 4X
                    }
                    else if (selectedAntiAliasIndex == 10)
                    {
                        gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.High;// SET TO 8X
                    }
                }
            }


            if (selectedTaaQualityIndex != appliedTaaQualityIndex)
            {
                if (selectedTaaQualityIndex == 0)
                {
                    gameManager.scripts.cameraManager.cameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.LowQuality;
                }
                else if(selectedTaaQualityIndex == 1)
                {
                    gameManager.scripts.cameraManager.cameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.PostSharpen;
                }
                else if (selectedTaaQualityIndex == 2)
                {
                    gameManager.scripts.cameraManager.cameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.ContrastAdaptiveSharpening;
                }                
            }


            if (fog)
            {
                if (selectedFogIndex != appliedFogIndex)
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
            }            

            if (selectedFOV != appliedFOV)
            {
                gameManager.scripts.cameraManager.cinemachineCamera.Lens.FieldOfView = selectedFOV;
            }

            if (selectedRenderDistance != appliedRenderDistance)
            {
                gameManager.scripts.cameraManager.cinemachineCamera.Lens.FarClipPlane = selectedRenderDistance;
            }

            if (bloom)
            {
                if (selectedBloomIndex != appliedBloomIndex)
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
            }

            if (selectedHDR != appliedHDR)
            {
                if (selectedHDR)
                {
                    gameManager.scripts.cameraManager.mainCamera.allowHDR = true;
                }
                else
                {
                    gameManager.scripts.cameraManager.mainCamera.allowHDR = false;
                }
            }

            if (selectedAnsioIndex != appliedAnsioIndex)
            {
                SetAnsio(null, true);
            }            

            if (tonemapping)
            {
                if (selectedTonemappingIndex != appliedTonemappingIndex)
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
            }

            if (globalIllumination)
            {
                if (selectedGlobalIlluminationIndex != appliedGlobalIlluminationIndex)
                {
                    if (selectedGlobalIlluminationIndex != 0)
                    {
                        globalIllumination.enable.value = true;
                        globalIllumination.active = true;

                        if (selectedGlobalIlluminationIndex == 1 || selectedGlobalIlluminationIndex == 2 || selectedGlobalIlluminationIndex == 3)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            globalIllumination.tracing.value = rayMode;
                        }
                        else
                        {
                            rayMode = RayCastingMode.RayTracing;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }

                    if (selectedGlobalIlluminationIndex == 0)
                    {
                        globalIllumination.active = false;
                        globalIllumination.enable.value = false;
                    }
                    else if (selectedGlobalIlluminationIndex == 1)
                    {  
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                        
                    }
                    else if (selectedGlobalIlluminationIndex == 2)
                    {     
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    }
                    else if (selectedGlobalIlluminationIndex == 3)
                    {   
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    }
                    else if (selectedGlobalIlluminationIndex == 4)
                    {   
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Low;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedGlobalIlluminationIndex = 2;
                            gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }
                    else if (selectedGlobalIlluminationIndex == 5)
                    { 
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedGlobalIlluminationIndex = 2;
                            gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }
                    else if (selectedGlobalIlluminationIndex == 6)
                    {  
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.High;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedGlobalIlluminationIndex = 2;
                            gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }
                }
            }

            if (reflections)
            {
                if (selectedReflectionsIndex != appliedReflectionsIndex)
                {
                    reflections.enabled.overrideState = true; // Enable manual override

                    if (selectedReflectionsIndex != 0)
                    {
                        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
                        RenderSettings.customReflectionTexture = null;// Let HDRI Sky handle reflections
                        reflections.enabled.value = true;
                        reflections.active = true;
                        // DEACTIVATE SCENE REFLECTION PROBES (NOT PLANAR)

                        if (selectedReflectionsIndex == 1 || selectedReflectionsIndex == 2 || selectedReflectionsIndex == 3)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            reflections.tracing.value = rayMode;
                        }
                        else
                        {
                            rayMode = rayMode = RayCastingMode.RayTracing;
                            reflections.tracing.value = rayMode;
                        }
                    }

                    if (selectedReflectionsIndex == 0)
                    {
                        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
                        RenderSettings.customReflectionTexture = null;
                        reflections.active = false;
                        reflections.enabled.value = false;
                        // ACTIVATE SCENE REFLECTION PROBES (NOT PLANAR)                       
                    }
                    else if (selectedReflectionsIndex == 1)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Low; 
                    }
                    else if (selectedReflectionsIndex == 2)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    }
                    else if (selectedReflectionsIndex == 3)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    }
                    else if (selectedReflectionsIndex == 4)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Low;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedReflectionsIndex = 2;
                            gameManager.scripts.uiManager.reflectionsDropdown.value = selectedReflectionsIndex;
                            reflections.tracing.value = rayMode;
                        }
                    }
                    else if (selectedReflectionsIndex == 5)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedReflectionsIndex = 2;
                            gameManager.scripts.uiManager.reflectionsDropdown.value = selectedReflectionsIndex;
                            reflections.tracing.value = rayMode;
                        }
                    }
                    else if (selectedReflectionsIndex == 6)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.High;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedReflectionsIndex = 2;
                            gameManager.scripts.uiManager.reflectionsDropdown.value = selectedReflectionsIndex;
                            reflections.tracing.value = rayMode;
                        }
                    }
                }
            }

            if (selectedPlanarReflections != appliedPlanarReflections)
            {
                if (selectedPlanarReflections)
                {
                    SetPlanarReflection(null, true, true);
                }
                else
                {
                    SetPlanarReflection(null, true, false);
                }
            }

            if (requiresTimedConfirmation)
            {                
                requiresTimedConfirmationTimer -= Time.deltaTime;

                if (requiresTimedConfirmationTimer <= 0)
                {
                    RevertChanges();
                    requiresTimedConfirmationTimer = 24f;
                }
                Utils.ActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges, true);
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
        appliedAutosaves = selectedAutosaves;
        appliedMaximumAutosaves = selectedMaximumAutosaves;
        appliedMaximumQuicksaves = selectedMaximumQuicksaves;
        appliedGore = selectedGore;
        appliedResolutionIndex = selectedResolutionIndex;
        appliedDisplayModeIndex = selectedDisplayModeIndex;
        appliedQualityAssetIndex = selectedQualityAssetIndex;
        appliedVsyncIndex = selectedVsyncIndex;
        appliedFrameRateCap = selectedFrameRateCap;
        appliedFrameRateCapValue = selectedFrameRateCapValue;
        appliedAntiAliasIndex = selectedAntiAliasIndex;
        appliedTaaQualityIndex = selectedTaaQualityIndex;
        appliedFogIndex = selectedFogIndex;
        appliedFOV = selectedFOV;
        appliedRenderDistance = selectedRenderDistance;
        appliedBloomIndex = selectedBloomIndex;
        appliedHDR = selectedHDR;
        appliedAnsioIndex = selectedAnsioIndex;
        appliedTonemappingIndex = selectedTonemappingIndex;
        appliedGlobalIlluminationFullRes = selectedGlobalIlluminationFullRes;
        appliedGlobalIlluminationIndex = selectedGlobalIlluminationIndex;
        appliedReflectionsIndex = selectedReflectionsIndex;
        appliedPlanarReflections = selectedPlanarReflections;
        appliedMasterVolume = selectedMasterVolume;
        appliedMusicVolume = selectedMusicVolume;
        appliedAmbientVolume = selectedAmbientVolume;
        appliedSfxVolume = selectedSfxVolume;
        appliedUiVolume = selectedUiVolume;
        appliedVoiceVolume = selectedVoiceVolume;
        appliedEventVolume = selectedEventVolume;

        appliedCameraZoomSensitivity = selectedCameraZoomSensitivity;
        appliedCameraMoveSensitivity = selectedCameraMoveSensitivity;
        appliedCameraPanSensitivity = selectedCameraPanSensitivity;
        appliedGamepadCameraZoomSensitivity = selectedGamepadCameraZoomSensitivity;
        appliedGamepadCameraMoveSensitivity = selectedGamepadCameraMoveSensitivity;
        appliedGamepadCameraPanSensitivity = selectedGamepadCameraPanSensitivity;
        appliedGamepadCameraZoomDeadzone = selectedGamepadCameraZoomDeadzone;
        appliedGamepadCameraMoveDeadzone = selectedGamepadCameraMoveDeadzone;
        appliedGamepadCameraPanDeadzone = selectedGamepadCameraPanDeadzone;        

        requiresTimedConfirmation = false;
        Utils.ActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges, false);
        Utils.ClearMemory();
    }
    public void RevertChanges()
    {
        selectedAutosaves = appliedAutosaves;
        gameManager.scripts.uiManager.autosavesToggle.isOn = selectedAutosaves;
        selectedMaximumAutosaves = appliedMaximumAutosaves;
        gameManager.scripts.uiManager.maximumAutosavesSlider.value = selectedMaximumAutosaves;
        selectedMaximumQuicksaves = appliedMaximumQuicksaves;
        gameManager.scripts.uiManager.maximumQuicksavesSlider.value = selectedMaximumQuicksaves;
        selectedGore = appliedGore;

        gameManager.scripts.uiManager.goreToggle.isOn = selectedGore;
        selectedResolutionIndex = appliedResolutionIndex;
        gameManager.scripts.uiManager.resolutionsDropdown.value = selectedResolutionIndex;
        selectedDisplayModeIndex = appliedDisplayModeIndex;
        gameManager.scripts.uiManager.displayModeDropdown.value = selectedDisplayModeIndex;
        selectedQualityAssetIndex = appliedQualityAssetIndex;
        gameManager.scripts.uiManager.qualityDropdown.value = selectedQualityAssetIndex;
        selectedVsyncIndex = appliedVsyncIndex;
        gameManager.scripts.uiManager.vSyncDropdown.value = selectedVsyncIndex;
        selectedFrameRateCap = appliedFrameRateCap;
        gameManager.scripts.uiManager.frameRateCapToggle.isOn = selectedFrameRateCap;
        selectedFrameRateCapValue = appliedFrameRateCapValue;
        gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
        gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
        selectedAntiAliasIndex = appliedAntiAliasIndex;
        gameManager.scripts.uiManager.antiAliasDropdown.value = selectedAntiAliasIndex;
        selectedTaaQualityIndex = appliedTaaQualityIndex;
        gameManager.scripts.uiManager.taaQualityDropdown.value = selectedTaaQualityIndex;
        selectedFogIndex = appliedFogIndex;
        gameManager.scripts.uiManager.fogDropdown.value = selectedFogIndex;
        selectedFOV = appliedFOV;
        gameManager.scripts.uiManager.fovSlider.value = selectedFOV;
        selectedRenderDistance = appliedRenderDistance;
        gameManager.scripts.uiManager.renderDistanceSlider.value = selectedRenderDistance;
        selectedBloomIndex = appliedBloomIndex;
        gameManager.scripts.uiManager.bloomDropdown.value = selectedBloomIndex;
        selectedHDR = appliedHDR;
        gameManager.scripts.uiManager.hdrToggle.isOn = selectedHDR;
        selectedAnsioIndex = appliedAnsioIndex;
        gameManager.scripts.uiManager.ansioDropdown.value = selectedAnsioIndex;
        selectedTonemappingIndex = appliedTonemappingIndex;
        gameManager.scripts.uiManager.tonemappingDropdown.value = selectedTonemappingIndex;
        selectedGlobalIlluminationFullRes = appliedGlobalIlluminationFullRes;
        gameManager.scripts.uiManager.giResolutionToggle.isOn = selectedGlobalIlluminationFullRes;
        selectedGlobalIlluminationIndex = appliedGlobalIlluminationIndex;
        gameManager.scripts.uiManager.giDropdown.value = selectedGlobalIlluminationIndex;
        selectedReflectionsIndex = appliedReflectionsIndex;
        gameManager.scripts.uiManager.reflectionsDropdown.value = selectedReflectionsIndex;
        selectedPlanarReflections = appliedPlanarReflections;
        gameManager.scripts.uiManager.planarReflectionsToggle.isOn = selectedPlanarReflections;

        selectedMasterVolume = appliedMasterVolume;
        gameManager.scripts.uiManager.audioMasterSlider.value = selectedMasterVolume;
        selectedMusicVolume = appliedMusicVolume;
        gameManager.scripts.uiManager.audioMusicSlider.value = selectedMusicVolume;
        selectedAmbientVolume = appliedAmbientVolume;
        gameManager.scripts.uiManager.audioAmbientSlider.value = selectedAmbientVolume;
        selectedSfxVolume = appliedSfxVolume;
        gameManager.scripts.uiManager.audioSFXSlider.value = selectedSfxVolume;
        selectedUiVolume = appliedUiVolume;
        gameManager.scripts.uiManager.audioUISlider.value = selectedUiVolume;
        selectedVoiceVolume = appliedVoiceVolume;
        gameManager.scripts.uiManager.audioVoiceSlider.value = selectedVoiceVolume;
        selectedEventVolume = appliedEventVolume;
        gameManager.scripts.uiManager.audioEventSlider.value = selectedEventVolume;

        selectedCameraZoomSensitivity = appliedCameraZoomSensitivity;
        gameManager.scripts.uiManager.cameraZoomSensitivitySlider.value = selectedCameraZoomSensitivity;
        selectedCameraMoveSensitivity = appliedCameraMoveSensitivity;
        gameManager.scripts.uiManager.cameraMoveSensitivitySlider.value = selectedCameraMoveSensitivity;
        selectedCameraPanSensitivity = appliedCameraPanSensitivity;
        gameManager.scripts.uiManager.cameraPanSensitivitySlider.value = selectedCameraPanSensitivity;
        selectedGamepadCameraZoomSensitivity = appliedGamepadCameraZoomSensitivity;
        gameManager.scripts.uiManager.cameraGamepadZoomSensitivitySlider.value = selectedGamepadCameraZoomSensitivity;
        selectedGamepadCameraMoveSensitivity = appliedGamepadCameraMoveSensitivity;
        gameManager.scripts.uiManager.cameraGamepadMoveSensitivitySlider.value = selectedGamepadCameraMoveSensitivity;
        selectedGamepadCameraPanSensitivity = appliedGamepadCameraPanSensitivity;
        gameManager.scripts.uiManager.cameraGamepadPanSensitivitySlider.value = selectedGamepadCameraPanSensitivity;
        selectedGamepadCameraZoomDeadzone = appliedGamepadCameraZoomDeadzone;
        gameManager.scripts.uiManager.cameraGamepadZoomDeadzoneSlider.value = selectedGamepadCameraZoomDeadzone;
        selectedGamepadCameraMoveDeadzone = appliedGamepadCameraMoveDeadzone;
        gameManager.scripts.uiManager.cameraGamepadMoveDeadzoneSlider.value = selectedGamepadCameraMoveDeadzone;
        selectedGamepadCameraPanDeadzone = appliedGamepadCameraPanDeadzone;
        gameManager.scripts.uiManager.cameraGamepadPanDeadzoneSlider.value = selectedGamepadCameraPanDeadzone;

        requiresTimedConfirmation = false;
        Utils.ActivateObject(gameManager.scripts.uiManager.confirmOptionsChanges, false);
        ApplySettings();// RE-APPLY PREVIOUS SETTINGS
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
            string displayName = $"{i + 1}<color=red>:</color> {displays[i].systemWidth}<color=red>x</color>{displays[i].systemHeight}";
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
        resolutions = Display.displays.Length > displayIndex ? Display.displays[displayIndex].systemWidth > 0 ? Screen.resolutions : new Resolution[0] : new Resolution[0];

        if (resolutions.Length == 0)
        {
            Debug.LogWarning("Detected no resolutions available on selected Display Device.");
            return;
        }

        var resolutionOptions = new List<string>();
        filteredResolutions.Clear();

        int bootResolutionIndex = 0;
        int preferredResolutionIndex = -1;

        Resolution detectedStartResolution = Screen.currentResolution;
        int maxRefreshRate = resolutions.Max(res => (int)res.refreshRateRatio.numerator / (int)res.refreshRateRatio.denominator);

        List<(int width, int height, float refreshRate)> seenResolutions = new List<(int, int, float)>();

        // Store resolutions first, filtering out any below 1024x768
        foreach (var res in resolutions)
        {
            refreshRate = res.refreshRateRatio.numerator / res.refreshRateRatio.denominator;
            var resolutionTuple = (res.width, res.height, refreshRate);

            if (res.width >= 1024 && res.height >= 768 && refreshRate == maxRefreshRate && !seenResolutions.Contains(resolutionTuple))
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
        
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution res = filteredResolutions[i];
            string resolutionText = $"{res.width}<color=red>x</color>{res.height}";
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
        selectedDisplayModeIndex = 0;
        gameManager.scripts.uiManager.displayModeDropdown.value = selectedDisplayModeIndex;
        gameManager.scripts.uiManager.displayModeDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
    }
    void PopulateFrameRateCapSlider()
    {
        selectedFrameRateCap = true;
        gameManager.scripts.uiManager.frameRateCapToggle.isOn = selectedFrameRateCap;
        gameManager.scripts.uiManager.frameRateCapToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.frameRateCapToggle.onValueChanged.AddListener(OnFrameRateCapToggleChanged);

        if (selectedFrameRateCap)
        {
            Utils.ActivateObject(gameManager.scripts.uiManager.framerateCapEffect, true);
        }
        else
        {
            Utils.ActivateObject(gameManager.scripts.uiManager.framerateCapEffect, false);
        }

        selectedFrameRateCapValue = 120;
        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;
                     
        if (selectedFrameRateCapValue > refreshRate)
        {            
            selectedFrameRateCapValue = (int)refreshRate;
            gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
            gameManager.scripts.uiManager.frameRateCapSliderText.text = refreshRate.ToString(Strings.numberFormat0);
        }
        else
        {            
            gameManager.scripts.uiManager.frameRateCapSlider.value = selectedFrameRateCapValue;
            gameManager.scripts.uiManager.frameRateCapSliderText.text = gameManager.scripts.uiManager.frameRateCapSlider.value.ToString();
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
        selectedAntiAliasIndex = 3;
        gameManager.scripts.uiManager.antiAliasDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Off",
            "FXAA",
            "SMAA <color=red>-</color> 2<color=red>x</color>",
            "SMAA <color=red>-</color> 4<color=red>x</color>",
            "SMAA <color=red>-</color> 8<color=red>x</color>",
            "MSAA <color=red>-</color> 2<color=red>x</color>",
            "MSAA <color=red>-</color> 4<color=red>x</color>",
            "MSAA <color=red>-</color> 8<color=red>x</color>",
            "TAA <color=red>-</color> 2<color=red>x</color>",
            "TAA <color=red>-</color> 4<color=red>x</color>",
            "TAA <color=red>-</color> 8<color=red>x</color>",
        };
        gameManager.scripts.uiManager.antiAliasDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.antiAliasDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.antiAliasDropdown.onValueChanged.AddListener(OnAntiAliasChanged);
        gameManager.scripts.uiManager.antiAliasDropdown.value = selectedAntiAliasIndex;
        gameManager.scripts.uiManager.antiAliasDropdown.RefreshShownValue();// Update the dropdown to reflect the selected aa
    }
    void PopulateTAAQualityDropdown()
    {
        selectedTaaQualityIndex = 1;
        gameManager.scripts.uiManager.taaQualityDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Low Quality",
            "Post Sharpen",
            "Contrast Adapt"
        };
        gameManager.scripts.uiManager.taaQualityDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.taaQualityDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.taaQualityDropdown.onValueChanged.AddListener(OnTaaQualityChanged);
        gameManager.scripts.uiManager.taaQualityDropdown.value = selectedTaaQualityIndex;
        gameManager.scripts.uiManager.taaQualityDropdown.RefreshShownValue();// Update the dropdown to reflect the selected aa
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

        gameManager.scripts.uiManager.qualityDropdown.value = selectedQualityAssetIndex;
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
        gameManager.scripts.uiManager.fovSliderText.text = gameManager.scripts.uiManager.fovSlider.value.ToString();
    }
    void PopulateRenderDistanceSlider()
    {
        selectedRenderDistance = 5000;
        gameManager.scripts.uiManager.renderDistanceSlider.value = selectedRenderDistance;
        gameManager.scripts.uiManager.renderDistanceSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.renderDistanceSlider.onValueChanged.AddListener(OnRenderDistanceChanged);
        gameManager.scripts.uiManager.renderDistanceSliderText.text = gameManager.scripts.uiManager.renderDistanceSlider.value.ToString();
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
        gameManager.scripts.uiManager.hdrToggle.isOn = selectedHDR;
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
            "2<color=red>x</color>",
            "4<color=red>x</color>",
            "8<color=red>x</color>",
            "16<color=red>x</color>",
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
        selectedGlobalIlluminationFullRes = true;
        gameManager.scripts.uiManager.giResolutionToggle.isOn = selectedGlobalIlluminationFullRes;
        gameManager.scripts.uiManager.giResolutionToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.giResolutionToggle.onValueChanged.AddListener(OnGiResolutionChanged);

        if (selectedGlobalIlluminationFullRes)
        {
            Utils.ActivateObject(gameManager.scripts.uiManager.globalIlluminationEffect, true);
        }
        else
        {
            Utils.ActivateObject(gameManager.scripts.uiManager.globalIlluminationEffect, false);
        }

        if (postProcess.profile.TryGet(out globalIllumination)) { }
        
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
    }
    void PopulateReflectionsDropdown()
    {
        selectedPlanarReflections = false;
        gameManager.scripts.uiManager.planarReflectionsToggle.isOn = selectedPlanarReflections;
        gameManager.scripts.uiManager.planarReflectionsToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.planarReflectionsToggle.onValueChanged.AddListener(OnPlanarRelectionsChanged);

        if (selectedPlanarReflections)
        {
            Utils.ActivateObject(gameManager.scripts.uiManager.planarReflectionEffect, true);
        }
        else
        {
            Utils.ActivateObject(gameManager.scripts.uiManager.planarReflectionEffect, false);
        }

        if (postProcess.profile.TryGet(out reflections)) { }

        selectedReflectionsIndex = 2;
        gameManager.scripts.uiManager.reflectionsDropdown.ClearOptions();

        desiredList.Clear();
        desiredList = new List<string>
        {
            "Realtime",
            "SSR Low",
            "SSR Medium",
            "SSR High",
            "RTR Low",
            "RTR Medium",
            "RTR High"
        };
        gameManager.scripts.uiManager.reflectionsDropdown.AddOptions(desiredList);

        gameManager.scripts.uiManager.reflectionsDropdown.value = selectedReflectionsIndex;
        gameManager.scripts.uiManager.reflectionsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.reflectionsDropdown.onValueChanged.AddListener(OnReflectionsChanged);
        gameManager.scripts.uiManager.reflectionsDropdown.RefreshShownValue();
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
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage2, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage2Tab, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage1Tab, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage1, true);
    }
    public void VideoPage2()
    {
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage1, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage1Tab, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage2Tab, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsVideoPage2, true);
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
            Utils.ActivateObject(gameManager.scripts.uiManager.framerateCapEffect, true);
        }
        else if (!gameManager.scripts.uiManager.frameRateCapToggle.isOn)
        {
            selectedFrameRateCap = false;
            Utils.ActivateObject(gameManager.scripts.uiManager.framerateCapEffect, false);
        }
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    void OnframeRateCapSliderChanged(float sliderValue)
    {
        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;

        if (gameManager.scripts.uiManager.frameRateCapSlider.value >= refreshRate)
        {
            gameManager.scripts.uiManager.frameRateCapSliderText.text = refreshRate.ToString(Strings.numberFormat0);
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
    void OnTaaQualityChanged(int taaQualityIndex)
    {
        selectedTaaQualityIndex = gameManager.scripts.uiManager.taaQualityDropdown.value;// Get selected display index from the dropdown
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

    void OnRenderDistanceChanged(float sliderValue)
    {
        selectedRenderDistance = (int)gameManager.scripts.uiManager.renderDistanceSlider.value;
        gameManager.scripts.uiManager.renderDistanceSliderText.text = gameManager.scripts.uiManager.renderDistanceSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void IncreaseRenderDistance()
    {
        gameManager.scripts.uiManager.renderDistanceSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.renderDistanceSlider.value + 1, gameManager.scripts.uiManager.renderDistanceSlider.minValue, gameManager.scripts.uiManager.renderDistanceSlider.maxValue);
        selectedRenderDistance = (int)gameManager.scripts.uiManager.renderDistanceSlider.value;
        gameManager.scripts.uiManager.fovSliderText.text = gameManager.scripts.uiManager.renderDistanceSlider.value.ToString();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void DecreaseRenderDistance()
    {
        gameManager.scripts.uiManager.renderDistanceSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.renderDistanceSlider.value - 1, gameManager.scripts.uiManager.renderDistanceSlider.minValue, gameManager.scripts.uiManager.renderDistanceSlider.maxValue);
        selectedRenderDistance = (int)gameManager.scripts.uiManager.renderDistanceSlider.value;
        gameManager.scripts.uiManager.renderDistanceSliderText.text = gameManager.scripts.uiManager.renderDistanceSlider.value.ToString();
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

    public void SetAnsio(Material material, bool scanScene)
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
            if (scanScene)
            {                
                Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);

                foreach (Renderer renderer in renderers)
                {
                    foreach (Material mat in renderer.sharedMaterials)
                    {
                        if (mat && mat.mainTexture)
                        {
                            //Debug.Log("Material and mainTexture present..");
                            if (selectedAnsioIndex == 0) // Set desired anisotropic level
                            {
                                //Debug.Log("Texture Set to OFF");
                                mat.mainTexture.anisoLevel = 0;
                            }
                            else if (selectedAnsioIndex == 1)
                            {
                                //Debug.Log("Texture Set to 2x");
                                mat.mainTexture.anisoLevel = 2;
                            }
                            else if (selectedAnsioIndex == 2)
                            {
                                //Debug.Log("Texture Set to 4x");
                                mat.mainTexture.anisoLevel = 4;
                            }
                            else if (selectedAnsioIndex == 3)
                            {
                                //Debug.Log("Texture Set to 8x");
                                mat.mainTexture.anisoLevel = 8;
                            }
                            else if (selectedAnsioIndex == 4)
                            {
                                //Debug.Log("Texture Set to 16x");
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
                    renderers = null;
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
    void OnGiResolutionChanged(bool giResolutionChanged)
    {
        if (gameManager.scripts.uiManager.giResolutionToggle.isOn)
        {
            selectedGlobalIlluminationFullRes = true;
            Utils.ActivateObject(gameManager.scripts.uiManager.globalIlluminationEffect, true);
        }
        else if (!gameManager.scripts.uiManager.giResolutionToggle.isOn)
        {
            selectedGlobalIlluminationFullRes = false;
            Utils.ActivateObject(gameManager.scripts.uiManager.globalIlluminationEffect, false);
        }
        OnGlobalIlluminationResHover();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    void OnReflectionsChanged(int reflectionsIndex)
    {
        selectedReflectionsIndex = gameManager.scripts.uiManager.reflectionsDropdown.value;
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    void OnPlanarRelectionsChanged(bool planarRelectionsChanged)
    {
        if (gameManager.scripts.uiManager.planarReflectionsToggle.isOn)
        {
            selectedPlanarReflections = true;
            Utils.ActivateObject(gameManager.scripts.uiManager.planarReflectionEffect, true);
        }
        else if (!gameManager.scripts.uiManager.planarReflectionsToggle.isOn)
        {
            selectedPlanarReflections = false;
            Utils.ActivateObject(gameManager.scripts.uiManager.planarReflectionEffect, false);
        }
        OnPlanarReflectionsHover();
        //ACTIVATE MODIFIED VALUE BUTTON
    }
    public void SetPlanarReflection(PlanarReflectionProbe probe, bool scanScene, bool activate)
    {
        if (probe)
        {
            if (activate)
            {
                probe.mode = ProbeSettings.Mode.Realtime; // Enable real-time reflections
                probe.realtimeMode = ProbeSettings.RealtimeMode.EveryFrame;
                probe.enabled = true;
            }
            else
            {
                probe.enabled = false;
            }
        }
        else
        {
            if (scanScene)
            {
                PlanarReflectionProbe[] probes = FindObjectsByType<PlanarReflectionProbe>(FindObjectsSortMode.None);

                foreach (PlanarReflectionProbe reflectionProbe in probes)
                {
                    if (activate)
                    {
                        reflectionProbe.mode = ProbeSettings.Mode.Realtime;
                        reflectionProbe.realtimeMode = ProbeSettings.RealtimeMode.EveryFrame;
                        reflectionProbe.enabled = true;
                    }
                    else
                    {
                        reflectionProbe.enabled = false;
                    }
                }
                probes = null;
            }
        }
        Utils.ClearMemory();
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
    public void OnOptionsPage1Hover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPage1Desc;
    }
    public void OnOptionsPage2Hover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPage2Desc;
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
    public void OnTaaQualityHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsTaaQualityDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsTaaQualityDescAdditional;
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
    public void OnRenderDistanceHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsRenderDistanceDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsDescClear;
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
    public void OnGlobalIlluminationResHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResDesc;
        if (selectedGlobalIlluminationFullRes)
        {
            gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResStatusFullDesc;
        }
        else
        {
            gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResStatusHalfDesc;
        }        
    }
    public void OnGlobalIlluminationResDescHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResStatusDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsGlobalIlluminationResDescAdditional;
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
    public void OnReflectionsHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsReflectionsDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsReflectionsDescAdditional;
    }
    public void OnPlanarReflectionsHover()
    {
        gameManager.scripts.uiManager.optionsDescription.text = GameStrings.GameStringsEnglish.optionsPlanarReflectionsDesc;
        gameManager.scripts.uiManager.optionsDescriptionAdditional.text = GameStrings.GameStringsEnglish.optionsPlanarReflectionsDescAdditional;
    }

}

