using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] private HighQualityLineRenderingVolumeComponent lineRenderer;

    //[SerializeField] private HDAdditionalReflectionData planarReflection; // possibly needed to determine Planar Reflection resolution based on camera distance at runtime

    private Display[] displays;
    [HideInInspector] public int lastDisplayCount; // To help automatically detect if someone connects another display, set to detect every 2 seconds at default, accessed by GameManager.

    private string gpuName;

    private FullScreenMode screenMode;

    [HideInInspector] public float refreshRate;// Accessed by UI Manager
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();
    private Resolution selectedResolution;
    

    [SerializeField] private HDRenderPipelineAsset appliedHDRPAsset;
    private RayCastingMode rayMode;
    [HideInInspector] public bool rayTracingSupported;// Accessed by UI Manager
    
    public float FPSCounterUpdateInterval = 0.1f; // How often to update the FPS display (in seconds), accessed by GameManager
    [HideInInspector] public float FPStimer;// Accessed by GameManager
    [HideInInspector] public int frameCount;// Accessed by GameManager
    [HideInInspector] public float fps;// Accessed by GameManager
    public float memoryCounterUpdateInterval = 0.1f; // How often to update the memory display (in seconds), accessed by GameManager
    [HideInInspector] public float memoryTimer;// Accessed by GameManager
    [HideInInspector] public long totalMemory;// Accessed by GameManager
    [HideInInspector] public float memoryInMB;// Accessed by GameManager
    [HideInInspector] public float memoryInGB;// Accessed by GameManager
    [HideInInspector] public List<string> desiredList;// Accessed by populate methods below

    [HideInInspector] public TimeSpan timeSpan; // for keep settings timer, accessed by GameManager
    [HideInInspector] public bool requiresTimedConfirmation;// Accessed by GameManager
    public float requiresTimedConfirmationTimer = 25;

    [System.Serializable]// Accessed by UI Manager and GameManager, selectedMouseScrollSensitivity is accessed by UI Manager for scroll wheel speed
    public class SelectedProperties
    {
        public int selectedAutosavesIndex;
        public int selectedMaximumAutosaves;
        public int selectedMaximumQuicksaves;
        public int selectedGoreIndex;
        public int selectedDestructionIndex;
        public int selectedCrowdsIndex;
        public int selectedTrafficIndex;
        public int selectedWildlifeIndex;
        public int selectedDisplayIndex;
        public int selectedResolutionIndex;
        public int selectedDisplayModeIndex;
        public bool selectedFrameRateCap;
        public int selectedFrameRateCapValue;
        public int selectedVsyncIndex;
        public int selectedAntiAliasIndex;
        public int selectedTaaQualityIndex;
        public int selectedQualityAssetIndex;
        public int selectedFogIndex;
        public int selectedFOV;
        public int selectedRenderDistance;
        public int selectedBloomIndex;
        public int selectedHDRIndex;
        public int selectedAnsioIndex;
        public int selectedTonemappingIndex;
        public int selectedTonemappingQualityIndex;
        public int selectedTonemappingQualityACESIndex;
        public int selectedGlobalIlluminationResIndex;
        public int selectedGlobalIlluminationIndex;
        public int selectedReflectionsIndex;
        public int selectedPlanarReflectionsIndex;
        public int selectedShadowQualityIndex;
        public int selectedWeatherEffectsIndex;
        public int selectedLineRenderingIndex;
        public int selectedCrevicesIndex;
        public int selectedShadowDistance;
        public bool selectedFPSCounter;
        public bool selectedMemoryCounter;
        public float selectedMasterVolume;
        public float selectedMusicVolume;
        public float selectedAmbientVolume;
        public float selectedSfxVolume;
        public float selectedUiVolume;
        public float selectedVoiceVolume;
        public float selectedEventVolume;
        public float selectedCameraZoomSensitivity;
        public float selectedCameraMoveSensitivity;
        public float selectedCameraPanSensitivity;
        public float selectedMouseScrollSensitivity;
        public float selectedGamepadCameraZoomSensitivity;
        public float selectedGamepadCameraMoveSensitivity;
        public float selectedGamepadCameraPanSensitivity;
        public float selectedGamepadCameraZoomDeadzone;
        public float selectedGamepadCameraMoveDeadzone;
        public float selectedGamepadCameraPanDeadzone;
    }
    public SelectedProperties selectedProperties;

    [Header("---- APPLIED ----------------------------------------------------")] // Accessed by Save+Load, game mostly uses these values to react at runtime
    public int appliedAutosavesIndex;
    public int appliedMaximumAutosaves;
    public int appliedMaximumQuicksaves;
    public int appliedGoreIndex;
    public int appliedDestructionIndex;
    public int appliedCrowdsIndex;
    public int appliedTrafficIndex;
    public int appliedWildlifeIndex;
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
    public int appliedHDRIndex;
    public int appliedAnsioIndex;
    public int appliedTonemappingIndex;
    public int appliedTonemappingQualityIndex;
    public int appliedTonemappingQualityACESIndex;
    public int appliedGlobalIlluminationResIndex;
    public int appliedGlobalIlluminationIndex;
    public int appliedReflectionsIndex;
    public int appliedPlanarReflectionsIndex;
    public int appliedShadowQualityIndex;
    public int appliedShadowDistance;
    public int appliedWeatherEffectsIndex;
    public int appliedLineRenderingIndex;
    public int appliedCrevicesIndex;
    public bool appliedFPSCounter;
    public bool appliedMemoryCounter;
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
    public float appliedGamepadCameraZoomDeadzone;// Accessed by UI Manager for gamepad dropdown scroll deadzone
    public float appliedGamepadCameraMoveDeadzone;
    public float appliedGamepadCameraPanDeadzone;

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.optionsManager = this;
    }

    void Start()
    {
        PopulateAutosavesDropdown();
        PopulateMaximumAutoSavesSliders();
        PopulateMaximumQuickSavesSlider();
        PopulateGoreDropdown();
        PopulateDestructionDropdown();
        PopulateCrowdsDropdown();
        PopulateTrafficDropdown();
        PopulateWildlifeDropdown();
        DetectDisplays();// resolutions dropdown is populated inside this    
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
        PopulateHDRDropdown();
        PopulateAnsioDropdown();
        PopulateWeatherEffectsDropdown();
        PopulateLineRenderingDropdown();
        PopulateCrevicesDropdown();
        PopulateTonemappingDropdown();// make sure PopulateHDRDropdown() is before this as this needs to check it
        PopulateTonemappingQualityDropdown();
        PopulateGlobalIlluminationDropdown();
        PopulateGlobalIlluminationResDropdown();
        PopulateReflectionsDropdown();
        PopulatePlanarReflectionsDropdown();
        PopulateShadowQualityDropdown();
        PopulateShadowDistanceSlider();
        PopulateFPSCounterToggle();
        PopulateMemoryCounterToggle();
        PopulateAudioSliders();
        PopulateMouseSliders();
        PopulateGamepadSliders();

        ApplySettings();
        desiredList.Clear();
    }


    public void ApplySettings()
    {
        if (Application.isPlaying)
        {
            // GAMEPLAY--------------
            // VIDEO--------------
            if (selectedProperties.selectedDisplayIndex != appliedDisplayIndex)
            {
                Display.displays[selectedProperties.selectedDisplayIndex].Activate();
                requiresTimedConfirmation = true;
            }

            if (selectedProperties.selectedDisplayModeIndex != appliedDisplayModeIndex)
            {
                if (selectedProperties.selectedDisplayModeIndex == 1)
                {
                    screenMode = FullScreenMode.Windowed;
                }
                else
                {
                    screenMode = FullScreenMode.FullScreenWindow;
                }
                requiresTimedConfirmation = true;
            }

            if (filteredResolutions.Count > selectedProperties.selectedResolutionIndex)
            {
                selectedResolution = filteredResolutions[selectedProperties.selectedResolutionIndex];// Get the chosen resolution from the filtered list

                if (selectedProperties.selectedResolutionIndex != appliedResolutionIndex || selectedProperties.selectedDisplayModeIndex != appliedDisplayModeIndex)
                {
                    Screen.SetResolution(selectedResolution.width, selectedResolution.height, screenMode, selectedResolution.refreshRateRatio);
                    //Debug.Log($"Applied resolution: {selectedResolution.width}<color=red>x</color>{selectedResolution.height} {selectedResolution.refreshRateRatio.numerator}/{selectedResolution.refreshRateRatio.denominator}<color=red>Hz</color>");
                    requiresTimedConfirmation = true;
                }
            }
            else
            {
                //Debug.Log("Tried to switch to a resolution index that doesn't exist.");
                if (selectedProperties.selectedDisplayModeIndex != appliedDisplayModeIndex)// If Display Mode has changed
                {
                    Screen.SetResolution(Screen.width, Screen.height, screenMode, selectedResolution.refreshRateRatio);// Default back to screen.width + height to avoid error
                    requiresTimedConfirmation = true;
                }                
            }

            if (selectedProperties.selectedVsyncIndex != appliedVsyncIndex)
            {
                if (selectedProperties.selectedVsyncIndex != 0)// If VSync is on
                {
                    if (selectedProperties.selectedVsyncIndex == 1)
                    {
                        Application.targetFrameRate = (int)selectedResolution.refreshRateRatio.value;
                        gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = ((int)selectedResolution.refreshRateRatio.value).ToString(Strings.numberFormat0);
                        gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = (int)selectedResolution.refreshRateRatio.value;
                    }
                    else if (selectedProperties.selectedVsyncIndex == 2)
                    {
                        Application.targetFrameRate = (int)(selectedResolution.refreshRateRatio.value / 2);
                        gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = ((int)(selectedResolution.refreshRateRatio.value / 2)).ToString(Strings.numberFormat0);
                        gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = (int)selectedResolution.refreshRateRatio.value / 2;
                    }
                }
            }

            if (selectedProperties.selectedFrameRateCapValue != appliedFrameRateCapValue)
            {
                if (selectedProperties.selectedVsyncIndex == 0)// If VSync is off
                {
                    if (selectedProperties.selectedFrameRateCap)// Use framerate cap
                    {
                        if (selectedProperties.selectedFrameRateCapValue > selectedResolution.refreshRateRatio.value)// Cap to monitors max refresh rate
                        {
                            //Debug.Log("Selected FramerateCap value of " + selectedFrameRateCapValue + " is higher than monitors max refresh of " + selectedResolution.refreshRateRatio + ", clamping to that value");
                            Application.targetFrameRate = (int)selectedResolution.refreshRateRatio.value;
                            selectedProperties.selectedFrameRateCapValue = (int)selectedResolution.refreshRateRatio.value;
                            gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = selectedProperties.selectedFrameRateCapValue;
                            gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value.ToString(Strings.numberFormat0);
                        }
                        else
                        {
                            Application.targetFrameRate = selectedProperties.selectedFrameRateCapValue;
                        }
                    }
                    else
                    {
                        Application.targetFrameRate = -1;// if not, unlimited
                    }
                }
            }

            if (selectedProperties.selectedAntiAliasIndex != appliedAntiAliasIndex)
            {
                if (selectedProperties.selectedAntiAliasIndex == 0 || selectedProperties.selectedAntiAliasIndex == 1 || selectedProperties.selectedAntiAliasIndex == 2 || selectedProperties.selectedAntiAliasIndex == 3 || selectedProperties.selectedAntiAliasIndex == 4)
                {
                    QualitySettings.antiAliasing = 0; // TURN OFF MSAA
                    if (selectedProperties.selectedAntiAliasIndex == 0)
                    {                        
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;// TURN OFF FXAA, SMAA, TAA
                    }
                    else if (selectedProperties.selectedAntiAliasIndex == 1)
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;// TURN ON FXAA
                    }
                    else
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;// TURN ON SMAA
                        if (selectedProperties.selectedAntiAliasIndex == 2)
                        {
                            gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Low;// SET TO 2X
                        }
                        else if (selectedProperties.selectedAntiAliasIndex == 3)
                        {
                            gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.Medium;// SET TO 4X
                        }
                        else if (selectedProperties.selectedAntiAliasIndex == 4)
                        {
                            gameManager.scripts.cameraManager.cameraData.SMAAQuality = HDAdditionalCameraData.SMAAQualityLevel.High;// SET TO 8X
                        }
                    }
                }
                else
                {
                    if (selectedProperties.selectedAntiAliasIndex == 5 || selectedProperties.selectedAntiAliasIndex == 6 || selectedProperties.selectedAntiAliasIndex == 7)
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None; // TURN OFF FXAA, SMAA, TAA
                    }
                    else
                    {
                        gameManager.scripts.cameraManager.cameraData.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing; // TURN ON TAA
                    }

                    if (selectedProperties.selectedAntiAliasIndex == 5)
                    {
                        QualitySettings.antiAliasing = 1; // TURN ON MSAA LOW
                    }
                    else if (selectedProperties.selectedAntiAliasIndex == 6)
                    {
                        QualitySettings.antiAliasing = 2; // TURN ON MSAA MEDIUM
                    }
                    else if (selectedProperties.selectedAntiAliasIndex == 7)
                    {
                        QualitySettings.antiAliasing = 3; // TURN ON MSAA HIGH
                    }
                    else if (selectedProperties.selectedAntiAliasIndex == 8)
                    {
                        gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.Low;// SET TO 2X
                    }
                    else if (selectedProperties.selectedAntiAliasIndex == 9)
                    {
                        gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.Medium;// SET TO 4X
                    }
                    else if (selectedProperties.selectedAntiAliasIndex == 10)
                    {
                        gameManager.scripts.cameraManager.cameraData.TAAQuality = HDAdditionalCameraData.TAAQualityLevel.High;// SET TO 8X
                    }
                }
            }


            if (selectedProperties.selectedTaaQualityIndex != appliedTaaQualityIndex)
            {
                if (selectedProperties.selectedTaaQualityIndex == 0)
                {
                    gameManager.scripts.cameraManager.cameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.LowQuality;
                }
                else if(selectedProperties.selectedTaaQualityIndex == 1)
                {
                    gameManager.scripts.cameraManager.cameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.PostSharpen;
                }
                else if (selectedProperties.selectedTaaQualityIndex == 2)
                {
                    gameManager.scripts.cameraManager.cameraData.taaSharpenMode = HDAdditionalCameraData.TAASharpenMode.ContrastAdaptiveSharpening;
                }                
            }


            if (fog)
            {
                if (selectedProperties.selectedFogIndex != appliedFogIndex)
                {
                    if (selectedProperties.selectedFogIndex == 0)
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

                        if (selectedProperties.selectedFogIndex == 1)
                        {
                            fog.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                        }
                        else if (selectedProperties.selectedFogIndex == 2)
                        {
                            fog.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                        }
                        else if (selectedProperties.selectedFogIndex == 3)
                        {
                            fog.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                        }
                        //Debug.Log("Fog Quality Set to: " + (ScalableSettingLevelParameter.Level)fog.quality.value);
                    }
                }
            }            

            if (selectedProperties.selectedFOV != appliedFOV)
            {
                gameManager.scripts.cameraManager.cinemachineCamera.Lens.FieldOfView = selectedProperties.selectedFOV;
            }

            if (selectedProperties.selectedRenderDistance != appliedRenderDistance)
            {
                gameManager.scripts.cameraManager.cinemachineCamera.Lens.FarClipPlane = selectedProperties.selectedRenderDistance;
            }

            if (bloom)
            {
                if (selectedProperties.selectedBloomIndex != appliedBloomIndex)
                {
                    if (selectedProperties.selectedBloomIndex == 0)
                    {
                        bloom.active = false;
                    }
                    else
                    {
                        bloom.active = true;

                        if (selectedProperties.selectedBloomIndex == 1)
                        {
                            bloom.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                        }
                        else if (selectedProperties.selectedBloomIndex == 2)
                        {
                            bloom.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                        }
                        else if (selectedProperties.selectedBloomIndex == 3)
                        {
                            bloom.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                        }
                        //Debug.Log("Bloom Quality Set to: " + (ScalableSettingLevelParameter.Level)bloom.quality.value);
                    }
                }
            }

            if (selectedProperties.selectedHDRIndex != appliedHDRIndex)
            {
                if (selectedProperties.selectedHDRIndex == 1)
                {
                    gameManager.scripts.cameraManager.mainCamera.allowHDR = true;
                }
                else
                {
                    gameManager.scripts.cameraManager.mainCamera.allowHDR = false;
                }
            }

            if (selectedProperties.selectedAnsioIndex != appliedAnsioIndex)
            {
                SetAnsio(null, true);
            }            

            if (tonemapping)
            {
                if (selectedProperties.selectedTonemappingIndex != appliedTonemappingIndex)
                {
                    if (selectedProperties.selectedTonemappingIndex == 0)
                    {
                        tonemapping.mode.value = TonemappingMode.Neutral;
                        tonemapping.neutralHDRRangeReductionMode.value = NeutralRangeReductionMode.Reinhard;
                    }
                    else if (selectedProperties.selectedTonemappingIndex == 1)
                    {
                        tonemapping.mode.value = TonemappingMode.Neutral;
                        tonemapping.neutralHDRRangeReductionMode.value = NeutralRangeReductionMode.BT2390;
                    }
                    else if (selectedProperties.selectedTonemappingIndex == 2)
                    {
                        tonemapping.mode.value = TonemappingMode.ACES;
                        tonemapping.acesPreset.value = HDRACESPreset.ACES1000Nits;
                    }
                    else if (selectedProperties.selectedTonemappingIndex == 3)
                    {
                        tonemapping.mode.value = TonemappingMode.ACES;
                        tonemapping.acesPreset.value = HDRACESPreset.ACES2000Nits;
                    }
                    else if (selectedProperties.selectedTonemappingIndex == 4)
                    {
                        tonemapping.mode.value = TonemappingMode.ACES;
                        tonemapping.acesPreset.value = HDRACESPreset.ACES4000Nits;
                    }
                }
            }

            if (globalIllumination)
            {
                if (selectedProperties.selectedGlobalIlluminationIndex != appliedGlobalIlluminationIndex)
                {
                    if (selectedProperties.selectedGlobalIlluminationIndex != 0)
                    {
                        globalIllumination.enable.value = true;
                        globalIllumination.active = true;

                        if (selectedProperties.selectedGlobalIlluminationIndex == 1 || selectedProperties.selectedGlobalIlluminationIndex == 2 || selectedProperties.selectedGlobalIlluminationIndex == 3)
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

                    if (selectedProperties.selectedGlobalIlluminationIndex == 0)
                    {
                        globalIllumination.active = false;
                        globalIllumination.enable.value = false;
                    }
                    else if (selectedProperties.selectedGlobalIlluminationIndex == 1)
                    {  
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Low;
                        
                    }
                    else if (selectedProperties.selectedGlobalIlluminationIndex == 2)
                    {     
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    }
                    else if (selectedProperties.selectedGlobalIlluminationIndex == 3)
                    {   
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    }
                    else if (selectedProperties.selectedGlobalIlluminationIndex == 4)
                    {   
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Low;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedProperties.selectedGlobalIlluminationIndex = 2;
                            gameManager.scripts.uiManager.optionsUI.giDropdown.value = selectedProperties.selectedGlobalIlluminationIndex;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }
                    else if (selectedProperties.selectedGlobalIlluminationIndex == 5)
                    { 
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedProperties.selectedGlobalIlluminationIndex = 2;
                            gameManager.scripts.uiManager.optionsUI.giDropdown.value = selectedProperties.selectedGlobalIlluminationIndex;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }
                    else if (selectedProperties.selectedGlobalIlluminationIndex == 6)
                    {  
                        globalIllumination.quality.value = (int)ScalableSettingLevelParameter.Level.High;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedProperties.selectedGlobalIlluminationIndex = 2;
                            gameManager.scripts.uiManager.optionsUI.giDropdown.value = selectedProperties.selectedGlobalIlluminationIndex;
                            globalIllumination.tracing.value = rayMode;
                        }
                    }
                }
            }

            if (reflections)
            {
                if (selectedProperties.selectedReflectionsIndex != appliedReflectionsIndex)
                {
                    reflections.enabled.overrideState = true; // Enable manual override

                    if (selectedProperties.selectedReflectionsIndex != 0)
                    {
                        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
                        RenderSettings.customReflectionTexture = null;// Let HDRI Sky handle reflections
                        reflections.enabled.value = true;
                        reflections.active = true;
                        // DEACTIVATE SCENE REFLECTION PROBES (NOT PLANAR)

                        if (selectedProperties.selectedReflectionsIndex == 1 || selectedProperties.selectedReflectionsIndex == 2 || selectedProperties.selectedReflectionsIndex == 3)
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

                    if (selectedProperties.selectedReflectionsIndex == 0)
                    {
                        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
                        RenderSettings.customReflectionTexture = null;
                        reflections.active = false;
                        reflections.enabled.value = false;
                        // ACTIVATE SCENE REFLECTION PROBES (NOT PLANAR)                       
                    }
                    else if (selectedProperties.selectedReflectionsIndex == 1)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Low; 
                    }
                    else if (selectedProperties.selectedReflectionsIndex == 2)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;
                    }
                    else if (selectedProperties.selectedReflectionsIndex == 3)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.High;
                    }
                    else if (selectedProperties.selectedReflectionsIndex == 4)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Low;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedProperties.selectedReflectionsIndex = 2;
                            gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value = selectedProperties.selectedReflectionsIndex;
                            reflections.tracing.value = rayMode;
                        }
                    }
                    else if (selectedProperties.selectedReflectionsIndex == 5)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.Medium;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedProperties.selectedReflectionsIndex = 2;
                            gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value = selectedProperties.selectedReflectionsIndex;
                            reflections.tracing.value = rayMode;
                        }
                    }
                    else if (selectedProperties.selectedReflectionsIndex == 6)
                    {
                        reflections.quality.value = (int)ScalableSettingLevelParameter.Level.High;

                        if (!rayTracingSupported)
                        {
                            rayMode = RayCastingMode.RayMarching;
                            selectedProperties.selectedReflectionsIndex = 2;
                            gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value = selectedProperties.selectedReflectionsIndex;
                            reflections.tracing.value = rayMode;
                        }
                    }
                }
            }

            if (selectedProperties.selectedPlanarReflectionsIndex != appliedPlanarReflectionsIndex)
            {
                if (selectedProperties.selectedPlanarReflectionsIndex != 0)
                {
                    SetPlanarReflection(null, true, true);
                }
                else
                {
                    SetPlanarReflection(null, true, false);
                }
            }

            if (lineRenderer && selectedProperties.selectedLineRenderingIndex != appliedLineRenderingIndex)
            {
                if (selectedProperties.selectedLineRenderingIndex == 1)
                {
                    lineRenderer.active = true;
                    lineRenderer.enable.value = true;
                }
                else
                {                    
                    lineRenderer.enable.value = false;
                    lineRenderer.active = false;
                }
            }

            if (selectedProperties.selectedShadowQualityIndex != appliedShadowQualityIndex)
            {
                if (selectedProperties.selectedShadowQualityIndex == 0)
                {
                    QualitySettings.shadowResolution = ShadowResolution.High;
                }
                if (selectedProperties.selectedShadowQualityIndex == 1)
                {
                    QualitySettings.shadowResolution = ShadowResolution.Medium;
                }
                if (selectedProperties.selectedShadowQualityIndex == 2)
                {
                    QualitySettings.shadowResolution = ShadowResolution.High;
                }
                if (selectedProperties.selectedShadowQualityIndex == 3)
                {
                    QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                }
            }

            if (selectedProperties.selectedShadowDistance != appliedShadowDistance)
            {
                QualitySettings.shadowDistance = selectedProperties.selectedShadowDistance;
            }

            if (selectedProperties.selectedFPSCounter != appliedFPSCounter)
            {
                if (selectedProperties.selectedFPSCounter)
                {
                    Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.fpsCounter, true);
                    gameManager.scripts.uiManager.optionsUI.fpsCounterAnimator.Play(Strings.on);
                }
                else
                {
                    gameManager.scripts.uiManager.optionsUI.fpsCounterAnimator.Play(Strings.off);
                }
            }

            if (selectedProperties.selectedMemoryCounter != appliedMemoryCounter)
            {
                if (selectedProperties.selectedMemoryCounter)
                {
                    Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.memoryCounter, true);
                    gameManager.scripts.uiManager.optionsUI.memoryCounterAnimator.Play(Strings.on);
                }
                else
                {
                    gameManager.scripts.uiManager.optionsUI.memoryCounterAnimator.Play(Strings.off);
                }
            }


            if (requiresTimedConfirmation && !gameManager.inSplashScreen)
            {                
                requiresTimedConfirmationTimer -= Time.deltaTime;

                if (requiresTimedConfirmationTimer <= 0)
                {
                    RevertChanges();
                    requiresTimedConfirmationTimer = 24f;
                }
                Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.confirmOptionsChanges, true);
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
        //GAMEPLAY
        appliedAutosavesIndex = selectedProperties.selectedAutosavesIndex;
        appliedMaximumAutosaves = selectedProperties.selectedMaximumAutosaves;
        appliedMaximumQuicksaves = selectedProperties.selectedMaximumQuicksaves;
        appliedGoreIndex = selectedProperties.selectedGoreIndex;
        appliedDestructionIndex = selectedProperties.selectedDestructionIndex;
        appliedCrowdsIndex = selectedProperties.selectedCrowdsIndex;
        appliedTrafficIndex = selectedProperties.selectedTrafficIndex;
        appliedWildlifeIndex = selectedProperties.selectedWildlifeIndex;
        //VIDEO
        appliedResolutionIndex = selectedProperties.selectedResolutionIndex;
        appliedDisplayModeIndex = selectedProperties.selectedDisplayModeIndex;
        appliedQualityAssetIndex = selectedProperties.selectedQualityAssetIndex;
        appliedVsyncIndex = selectedProperties.selectedVsyncIndex;
        appliedFrameRateCap = selectedProperties.selectedFrameRateCap;
        appliedFrameRateCapValue = selectedProperties.selectedFrameRateCapValue;
        appliedAntiAliasIndex = selectedProperties.selectedAntiAliasIndex;
        appliedTaaQualityIndex = selectedProperties.selectedTaaQualityIndex;
        appliedFogIndex = selectedProperties.selectedFogIndex;
        appliedFOV = selectedProperties.selectedFOV;
        appliedRenderDistance = selectedProperties.selectedRenderDistance;
        appliedBloomIndex = selectedProperties.selectedBloomIndex;
        appliedHDRIndex = selectedProperties.selectedHDRIndex;
        appliedAnsioIndex = selectedProperties.selectedAnsioIndex;
        appliedWeatherEffectsIndex = selectedProperties.selectedWeatherEffectsIndex;
        appliedLineRenderingIndex = selectedProperties.selectedLineRenderingIndex;
        appliedCrevicesIndex = selectedProperties.selectedCrevicesIndex;
        appliedTonemappingIndex = selectedProperties.selectedTonemappingIndex;
        appliedTonemappingQualityIndex = selectedProperties.selectedTonemappingQualityIndex;
        appliedTonemappingQualityACESIndex = selectedProperties.selectedTonemappingQualityACESIndex;
        appliedGlobalIlluminationResIndex = selectedProperties.selectedGlobalIlluminationResIndex;
        appliedGlobalIlluminationIndex = selectedProperties.selectedGlobalIlluminationIndex;
        appliedReflectionsIndex = selectedProperties.selectedReflectionsIndex;
        appliedPlanarReflectionsIndex = selectedProperties.selectedPlanarReflectionsIndex;
        appliedShadowQualityIndex = selectedProperties.selectedShadowQualityIndex;
        appliedShadowDistance = selectedProperties.selectedShadowDistance;
        appliedFPSCounter = selectedProperties.selectedFPSCounter;
        appliedMemoryCounter = selectedProperties.selectedMemoryCounter;
        //AUDIO
        appliedMasterVolume = selectedProperties.selectedMasterVolume;
        appliedMusicVolume = selectedProperties.selectedMusicVolume;
        appliedAmbientVolume = selectedProperties.selectedAmbientVolume;
        appliedSfxVolume = selectedProperties.selectedSfxVolume;
        appliedUiVolume = selectedProperties.selectedUiVolume;
        appliedVoiceVolume = selectedProperties.selectedVoiceVolume;
        appliedEventVolume = selectedProperties.selectedEventVolume;
        //KEYBOARD
        appliedCameraZoomSensitivity = selectedProperties.selectedCameraZoomSensitivity;
        appliedCameraMoveSensitivity = selectedProperties.selectedCameraMoveSensitivity;
        appliedCameraPanSensitivity = selectedProperties.selectedCameraPanSensitivity;
        //GAMEPAD
        appliedGamepadCameraZoomSensitivity = selectedProperties.selectedGamepadCameraZoomSensitivity;
        appliedGamepadCameraMoveSensitivity = selectedProperties.selectedGamepadCameraMoveSensitivity;
        appliedGamepadCameraPanSensitivity = selectedProperties.selectedGamepadCameraPanSensitivity;
        appliedGamepadCameraZoomDeadzone = selectedProperties.selectedGamepadCameraZoomDeadzone;
        appliedGamepadCameraMoveDeadzone = selectedProperties.selectedGamepadCameraMoveDeadzone;
        appliedGamepadCameraPanDeadzone = selectedProperties.selectedGamepadCameraPanDeadzone;        

        requiresTimedConfirmation = false;
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.confirmOptionsChanges, false);
        CheckModified();
    }
    public void RevertChanges()
    {
        //GAMEPLAY
        gameManager.scripts.uiManager.optionsUI.autosavesDropdown.value = appliedAutosavesIndex;
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value = appliedMaximumAutosaves;
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value = appliedMaximumQuicksaves;
        gameManager.scripts.uiManager.optionsUI.destructionDropdown.value = appliedDestructionIndex;
        gameManager.scripts.uiManager.optionsUI.crowdsDropdown.value = appliedCrowdsIndex;
        gameManager.scripts.uiManager.optionsUI.trafficDropdown.value = appliedTrafficIndex;
        gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.value = appliedWildlifeIndex;
        //VIDEO
        gameManager.scripts.uiManager.optionsUI.goreDropdown.value = appliedGoreIndex;
        gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.value = appliedResolutionIndex;
        gameManager.scripts.uiManager.optionsUI.displayModeDropdown.value = appliedDisplayModeIndex;
        gameManager.scripts.uiManager.optionsUI.qualityDropdown.value = appliedQualityAssetIndex;
        gameManager.scripts.uiManager.optionsUI.vSyncDropdown.value = appliedVsyncIndex;
        gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.isOn = appliedFrameRateCap;
        gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = appliedFrameRateCapValue;
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.value = appliedAntiAliasIndex;
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.value = appliedTaaQualityIndex;
        gameManager.scripts.uiManager.optionsUI.fogDropdown.value = appliedFogIndex;
        gameManager.scripts.uiManager.optionsUI.fovSlider.value = appliedFOV;
        gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value = appliedRenderDistance;
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.value = appliedBloomIndex;
        gameManager.scripts.uiManager.optionsUI.hdrDropdown.value = appliedHDRIndex;
        gameManager.scripts.uiManager.optionsUI.ansioDropdown.value = appliedAnsioIndex;
        gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.value = appliedWeatherEffectsIndex;
        gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.value = appliedLineRenderingIndex;
        gameManager.scripts.uiManager.optionsUI.crevicesDropdown.value = appliedCrevicesIndex;
        gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value = appliedTonemappingIndex;

        if (appliedTonemappingIndex == 0)
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = appliedTonemappingIndex;
        }
        else if (appliedTonemappingIndex == 1)
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = appliedTonemappingQualityIndex;
        }
        else if (appliedTonemappingIndex == 2)
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = appliedTonemappingQualityACESIndex;
        }

        gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.value = appliedGlobalIlluminationResIndex;
        gameManager.scripts.uiManager.optionsUI.giDropdown.value = appliedGlobalIlluminationIndex;
        gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value = appliedReflectionsIndex;
        gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.value = appliedPlanarReflectionsIndex;
        gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.value = appliedShadowQualityIndex;
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value = appliedShadowDistance;
        gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.isOn = appliedFPSCounter;
        gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.isOn = appliedMemoryCounter;
        //AUDIO
        gameManager.scripts.uiManager.optionsUI.audioMasterSlider.value = appliedMasterVolume;
        gameManager.scripts.uiManager.optionsUI.audioMusicSlider.value = appliedMusicVolume;
        gameManager.scripts.uiManager.optionsUI.audioAmbientSlider.value = appliedAmbientVolume;
        gameManager.scripts.uiManager.optionsUI.audioSFXSlider.value = appliedSfxVolume;
        gameManager.scripts.uiManager.optionsUI.audioUISlider.value = appliedUiVolume;
        gameManager.scripts.uiManager.optionsUI.audioVoiceSlider.value = appliedVoiceVolume;
        gameManager.scripts.uiManager.optionsUI.audioEventSlider.value = appliedEventVolume;
        //KEYBOARD
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value = appliedCameraZoomSensitivity;
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value = appliedCameraMoveSensitivity;
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value = appliedCameraPanSensitivity;
        //GAMEPAD
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value = appliedGamepadCameraZoomSensitivity;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value = appliedGamepadCameraMoveSensitivity;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value = appliedGamepadCameraPanSensitivity;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value = appliedGamepadCameraZoomDeadzone;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value = appliedGamepadCameraMoveDeadzone;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value = appliedGamepadCameraPanDeadzone;

        requiresTimedConfirmation = false;
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.confirmOptionsChanges, false);
        ApplySettings();// RE-APPLY PREVIOUS SETTINGS
    }
    public void CheckModified()
    {
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.autosavesDropdown, appliedAutosavesIndex, ref gameManager.scripts.uiManager.optionsUI.autosavesModifiedIcon);
        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider, appliedMaximumAutosaves, ref gameManager.scripts.uiManager.optionsUI.maximumAutosavesModifiedIcon);
        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider, appliedMaximumQuicksaves, ref gameManager.scripts.uiManager.optionsUI.maximumQuicksavesModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.goreDropdown, appliedGoreIndex, ref gameManager.scripts.uiManager.optionsUI.goreModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.destructionDropdown, appliedDestructionIndex, ref gameManager.scripts.uiManager.optionsUI.destructionModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.crowdsDropdown, appliedCrowdsIndex, ref gameManager.scripts.uiManager.optionsUI.crowdsModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.trafficDropdown, appliedTrafficIndex, ref gameManager.scripts.uiManager.optionsUI.trafficModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.wildlifeDropdown, appliedWildlifeIndex, ref gameManager.scripts.uiManager.optionsUI.wildlifeModifiedIcon);

        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown, appliedDisplayIndex, ref gameManager.scripts.uiManager.optionsUI.displayDeviceModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.resolutionsDropdown, appliedResolutionIndex, ref gameManager.scripts.uiManager.optionsUI.resolutionsModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.displayModeDropdown, appliedDisplayModeIndex, ref gameManager.scripts.uiManager.optionsUI.displayModeModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.vSyncDropdown, appliedVsyncIndex, ref gameManager.scripts.uiManager.optionsUI.vSyncModifiedIcon);
        Utils.CheckToggleValueModified(ref gameManager.scripts.uiManager.optionsUI.frameRateCapToggle, appliedFrameRateCap, ref gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.frameRateCapSlider, appliedFrameRateCapValue, ref gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);        

        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.renderDistanceSlider, appliedRenderDistance, ref gameManager.scripts.uiManager.optionsUI.renderDistanceModifiedIcon);
        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.fovSlider, appliedFOV, ref gameManager.scripts.uiManager.optionsUI.fovModifiedIcon);                
        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider, appliedShadowDistance, ref gameManager.scripts.uiManager.optionsUI.shadowDistanceModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.qualityDropdown, appliedQualityAssetIndex, ref gameManager.scripts.uiManager.optionsUI.qualityModifiedIcon);
             
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.bloomDropdown, appliedBloomIndex, ref gameManager.scripts.uiManager.optionsUI.bloomModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.giDropdown, appliedGlobalIlluminationIndex, ref gameManager.scripts.uiManager.optionsUI.giModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.giResolutionDropdown, appliedGlobalIlluminationResIndex, ref gameManager.scripts.uiManager.optionsUI.giResolutionModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.fogDropdown, appliedFogIndex, ref gameManager.scripts.uiManager.optionsUI.fogModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.ansioDropdown, appliedAnsioIndex, ref gameManager.scripts.uiManager.optionsUI.ansioModifiedIcon);

        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown, appliedWeatherEffectsIndex, ref gameManager.scripts.uiManager.optionsUI.weatherEffectsModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown, appliedLineRenderingIndex, ref gameManager.scripts.uiManager.optionsUI.lineRenderingModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.crevicesDropdown, appliedCrevicesIndex, ref gameManager.scripts.uiManager.optionsUI.crevicesModifiedIcon);

        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.reflectionsDropdown, appliedReflectionsIndex, ref gameManager.scripts.uiManager.optionsUI.reflectionsModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown, appliedPlanarReflectionsIndex, ref gameManager.scripts.uiManager.optionsUI.planarReflectionsModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown, appliedShadowQualityIndex, ref gameManager.scripts.uiManager.optionsUI.shadowQualityModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.antiAliasDropdown, appliedAntiAliasIndex, ref gameManager.scripts.uiManager.optionsUI.antiAliasModifiedIcon);
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.taaQualityDropdown, appliedTaaQualityIndex, ref gameManager.scripts.uiManager.optionsUI.taaQualityModifiedIcon);

        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.hdrDropdown, appliedHDRIndex, ref gameManager.scripts.uiManager.optionsUI.hdrModifiedIcon);        
        Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.tonemappingDropdown, appliedTonemappingIndex, ref gameManager.scripts.uiManager.optionsUI.tonemappingModifiedIcon);

        if (selectedProperties.selectedTonemappingIndex == 1)
        {
            Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown, appliedTonemappingQualityIndex, ref gameManager.scripts.uiManager.optionsUI.tonemappingQualityModifiedIcon);
        }
        else if (selectedProperties.selectedTonemappingIndex == 2 && selectedProperties.selectedHDRIndex == 1)
        {
            Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown, appliedTonemappingQualityACESIndex, ref gameManager.scripts.uiManager.optionsUI.tonemappingQualityModifiedIcon);
        }
        else
        {
            Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown, 0, ref gameManager.scripts.uiManager.optionsUI.tonemappingQualityModifiedIcon);
        }

        Utils.CheckToggleValueModified(ref gameManager.scripts.uiManager.optionsUI.fpsCounterToggle, appliedFPSCounter, ref gameManager.scripts.uiManager.optionsUI.fpsCounterModifiedIcon);
        Utils.CheckToggleValueModified(ref gameManager.scripts.uiManager.optionsUI.memoryCounterToggle, appliedMemoryCounter, ref gameManager.scripts.uiManager.optionsUI.memoryCounterModifiedIcon);

        Utils.CheckSliderValueModified(ref gameManager.scripts.uiManager.optionsUI.audioMasterSlider, appliedMasterVolume, ref gameManager.scripts.uiManager.optionsUI.audioMasterModifiedIcon);
    }

    #region Setup
    void PopulateAutosavesDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.autosavesDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "On"};
        gameManager.scripts.uiManager.optionsUI.autosavesDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.autosavesDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.autosavesDropdown.onValueChanged.AddListener(OnAutosavesChanged);
        gameManager.scripts.uiManager.optionsUI.autosavesDropdown.value = 1;// Default value
        OnAutosavesChanged(gameManager.scripts.uiManager.optionsUI.autosavesDropdown.value);
    }
    void PopulateMaximumAutoSavesSliders()
    {
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.onValueChanged.AddListener(OnAutosavesSliderChanged);
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value = 10;// Default value
        OnAutosavesSliderChanged(gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value);
    }
    void PopulateMaximumQuickSavesSlider()
    {
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.onValueChanged.AddListener(OnQuicksavesSliderChanged);
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value = 10;// Default value
        OnQuicksavesSliderChanged(gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value);
    }
    void PopulateGoreDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.goreDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "Blood", "Limbs", "Full"};
        gameManager.scripts.uiManager.optionsUI.goreDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.goreDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.goreDropdown.onValueChanged.AddListener(OnGoreChanged);
        gameManager.scripts.uiManager.optionsUI.goreDropdown.value = 3;// Default value
        OnGoreChanged(gameManager.scripts.uiManager.optionsUI.goreDropdown.value);
    }
    void PopulateDestructionDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.destructionDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "Rigidbodies", "Destruction", "Full"};
        gameManager.scripts.uiManager.optionsUI.destructionDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.destructionDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.destructionDropdown.onValueChanged.AddListener(OnDestructionChanged);
        gameManager.scripts.uiManager.optionsUI.destructionDropdown.value = 3;// Default value
        OnDestructionChanged(gameManager.scripts.uiManager.optionsUI.destructionDropdown.value);
    }
    public void DetectDisplays()
    {
        lastDisplayCount = Display.displays.Length;
        rayTracingSupported = SystemInfo.supportsRayTracing;
        gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.ClearOptions();

        displays = Display.displays;// Get all connected displays
        var displayNames = new System.Collections.Generic.List<string>();

        for (int i = 0; i < displays.Length; i++)
        {
            string displayName = $"{i + 1}<color=red>:</color> {displays[i].systemWidth}<color=red>x</color>{displays[i].systemHeight}";
            displayNames.Add(displayName);
        }

        gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.AddOptions(displayNames);

        if (displays.Length > 0)// Automatically populate resolutions for the display
        {
            PopulateResolutionsDropdown(0);
        }
        selectedProperties.selectedDisplayIndex = gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.value;// Default value
        gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.onValueChanged.AddListener(PopulateResolutionsDropdown);
        gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.onValueChanged.AddListener(OnDisplayDeviceChanged);
    }
    void PopulateDisplayAdapter()
    {
        gpuName = SystemInfo.graphicsDeviceName;// DISPLAY ADAPTER
        gameManager.scripts.uiManager.optionsUI.gpuName.text = gpuName;// DISPLAY ADAPTER
    }
    void PopulateResolutionsDropdown(int displayIndex)
    {
        if (gameManager.scripts.uiManager.optionsUI.resolutionsDropdown == null)
        {
            return;
        }

        gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.ClearOptions();
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

        // display only a single list of resolutions at the monitors max refresh rate, the refresh rate can then be changed via VSync / Framerate Cap
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
        
        for (int i = 0; i < filteredResolutions.Count; i++)// Create dropdown list and list the results
        {
            Resolution res = filteredResolutions[i];
            string resolutionText = $"{res.width}<color=red>x</color>{res.height}";
            resolutionOptions.Add(resolutionText);

            if (res.width == detectedStartResolution.width && res.height == detectedStartResolution.height)
            {
                bootResolutionIndex = i;
            }

            if (res.width == 1920 && res.height == 1080)// If 1080p is available, choose that as default preferred
            {
                preferredResolutionIndex = i;
            }
        }

        selectedProperties.selectedResolutionIndex = (preferredResolutionIndex != -1) ? preferredResolutionIndex : bootResolutionIndex;
        gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.AddOptions(resolutionOptions);        
        gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.onValueChanged.AddListener(OnResolutionChanged);
        gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.value = selectedProperties.selectedResolutionIndex;// Default value
        OnResolutionChanged(gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.value);
    }
    void PopulateDisplayModeDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.displayModeDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Fullscreen", "Windowed"};
        gameManager.scripts.uiManager.optionsUI.displayModeDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.displayModeDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        gameManager.scripts.uiManager.optionsUI.displayModeDropdown.value = 0;// Default value
        OnDisplayModeChanged(gameManager.scripts.uiManager.optionsUI.displayModeDropdown.value);
    }
    void PopulateFrameRateCapSlider()
    {
        gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.onValueChanged.AddListener(OnFrameRateCapToggleChanged);
        gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.isOn = true;
        OnFrameRateCapToggleChanged(gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.isOn);

        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;
        gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.onValueChanged.AddListener(OnframeRateCapSliderChanged);

        if (selectedProperties.selectedFrameRateCapValue > refreshRate)
        {
            selectedProperties.selectedFrameRateCapValue = (int)refreshRate;
            gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = 90;// Default value
            gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = refreshRate.ToString(Strings.numberFormat0);
        }
        else
        {            
            gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = 90;// Default value
            gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value.ToString(Strings.numberFormat0);
        }
        OnframeRateCapSliderChanged(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value);
    }
    void PopulateVSyncDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.vSyncDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "V<color=red>-</color>Blank", "Second V<color=red>-</color>Blank"};
        gameManager.scripts.uiManager.optionsUI.vSyncDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.vSyncDropdown.value = 1;// Default value 
        gameManager.scripts.uiManager.optionsUI.vSyncDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.vSyncDropdown.onValueChanged.AddListener(OnVSyncChanged);
        OnVSyncChanged(gameManager.scripts.uiManager.optionsUI.vSyncDropdown.value);
    }
    void PopulateAntiAliasDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.ClearOptions();
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
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.onValueChanged.AddListener(OnAntiAliasChanged);        
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.value = 3;// Default value
        OnAntiAliasChanged(gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.value);
    }
    void PopulateTAAQualityDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Low Quality", "Post Sharpen", "Contrast Adapt"};
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.onValueChanged.AddListener(OnTaaQualityChanged);        
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.value = 1;// Default value
        OnTaaQualityChanged(gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.value);
    }
    void PopulateQualityDropdown()
    {
        appliedHDRPAsset = QualitySettings.renderPipeline as HDRenderPipelineAsset;
        gameManager.scripts.uiManager.optionsUI.qualityDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Low", "Medium", "High", "Ultra"};
        gameManager.scripts.uiManager.optionsUI.qualityDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.qualityDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.qualityDropdown.onValueChanged.AddListener(OnQualitySettingsChanged);        
        gameManager.scripts.uiManager.optionsUI.qualityDropdown.value = 2;// Default value
        OnQualitySettingsChanged(gameManager.scripts.uiManager.optionsUI.qualityDropdown.value);
        /*
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            Debug.Log($"Quality Level {i}: {QualitySettings.names[i]}");
        }
        */
    }    
    void PopulateFogDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.fogDropdown.ClearOptions();
        desiredList.Clear();
        if (postProcess.profile.TryGet(out fog))
        {
            desiredList = new List<string>{"Off", "Low", "Medium", "High"};
        }
        else
        {
            desiredList = new List<string>{"ERROR"};
        }
        gameManager.scripts.uiManager.optionsUI.fogDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.fogDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.fogDropdown.onValueChanged.AddListener(OnFogChanged);        
        gameManager.scripts.uiManager.optionsUI.fogDropdown.value = 2;// Default value
        OnFogChanged(gameManager.scripts.uiManager.optionsUI.fogDropdown.value);
    }
    void PopulateFovSlider()
    {        
        gameManager.scripts.uiManager.optionsUI.fovSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.fovSlider.onValueChanged.AddListener(OnFOVChanged);
        gameManager.scripts.uiManager.optionsUI.fovSlider.value = 90;// Default value
        OnFOVChanged(gameManager.scripts.uiManager.optionsUI.fovSlider.value);
    }
    void PopulateRenderDistanceSlider()
    {        
        gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.onValueChanged.AddListener(OnRenderDistanceChanged);
        gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value = 2500;// Default value
        OnRenderDistanceChanged(gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value);
    }
    void PopulateBloomDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.ClearOptions();
        desiredList.Clear();
        if (postProcess.profile.TryGet(out bloom))
        {
            desiredList = new List<string> { "Off", "Low", "Medium", "High" };
        }
        else
        {
            desiredList = new List<string> { "ERROR" };
        }
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.onValueChanged.AddListener(OnBloomChanged);        
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.value = 2;// Default value
        OnBloomChanged(gameManager.scripts.uiManager.optionsUI.bloomDropdown.value);
    }
    void PopulateHDRDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.hdrDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "On"};
        gameManager.scripts.uiManager.optionsUI.hdrDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.hdrDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.hdrDropdown.onValueChanged.AddListener(OnHDRChanged);
        gameManager.scripts.uiManager.optionsUI.hdrDropdown.value = 0;// Default value
        OnHDRChanged(gameManager.scripts.uiManager.optionsUI.hdrDropdown.value);
    }
    void PopulateAnsioDropdown()
    {
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        gameManager.scripts.uiManager.optionsUI.ansioDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "2<color=red>x</color>", "4<color=red>x</color>", "8<color=red>x</color>", "16<color=red>x</color>"};
        gameManager.scripts.uiManager.optionsUI.ansioDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.ansioDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.ansioDropdown.onValueChanged.AddListener(OnAnsioChanged);        
        gameManager.scripts.uiManager.optionsUI.ansioDropdown.value = 2;// Default value
        OnAnsioChanged(gameManager.scripts.uiManager.optionsUI.ansioDropdown.value);
        SetAnsio(null, true);
    }
    void PopulateWeatherEffectsDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "Half", "Full"};
        gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.onValueChanged.AddListener(OnWeatherEffectsChanged);
        gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.value = 2;// Default value
        OnWeatherEffectsChanged(gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.value);
    }
    void PopulateLineRenderingDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.ClearOptions();
        desiredList.Clear();
        if (postProcess.profile.TryGet(out lineRenderer))
        {
            desiredList = new List<string>{"Standard", "High Quality"};
        }
        else
        {
            desiredList = new List<string> { "ERROR" };
        }
        
        gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.onValueChanged.AddListener(OnLineRenderingChanged);
        gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.value = 0;// Default value
        OnLineRenderingChanged(gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.value);
    }
    void PopulateCrevicesDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.crevicesDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "On"};
        gameManager.scripts.uiManager.optionsUI.crevicesDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.crevicesDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.crevicesDropdown.onValueChanged.AddListener(OnCrevicesChanged);
        gameManager.scripts.uiManager.optionsUI.crevicesDropdown.value = 1;// Default value
        OnCrevicesChanged(gameManager.scripts.uiManager.optionsUI.crevicesDropdown.value);
    }
    void PopulateTonemappingDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.ClearOptions();
        if (postProcess.profile.TryGet(out tonemapping))
        {
            desiredList = new List<string> { "Off", "Neutral", "ACES" };
        }
        else
        {
            desiredList = new List<string> { "ERROR" };
        }  
        gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.onValueChanged.AddListener(OnTonemappingChanged);        
        gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value = 2;// Default value
        OnTonemappingChanged(gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value);
        desiredList.Clear();
    }
    void PopulateTonemappingQualityDropdown()
    {
        if (gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value == 0 || selectedProperties.selectedHDRIndex == 0)
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.ClearOptions();
            desiredList.Clear();
            desiredList = new List<string>{"N/A"};
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.AddOptions(desiredList);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.RemoveAllListeners();
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.AddListener(OnTonemappingQualityChanged);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = 0;// Default value
            OnTonemappingQualityChanged(gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value);
        }
        else if (gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value == 1)
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.ClearOptions();
            desiredList.Clear();
            desiredList = new List<string>{"Reinhard", "BT2390"};
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.AddOptions(desiredList);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.RemoveAllListeners();
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.AddListener(OnTonemappingQualityChanged);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = 1;// Default value
            OnTonemappingQualityChanged(gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value);            
        }
        else
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.ClearOptions();
            desiredList.Clear();
            desiredList = new List<string>{"ACES1000", "ACES2000", "ACES4000"};
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.AddOptions(desiredList);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.RemoveAllListeners();
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.AddListener(OnTonemappingQualityChanged);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = 0;// Default value
            OnTonemappingQualityChanged(gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value);            
        }
        selectedProperties.selectedTonemappingQualityIndex = 1;// Default value for neutral
        selectedProperties.selectedTonemappingQualityACESIndex = 0;// Default value for ACES
        desiredList.Clear();
    }
    void PopulateGlobalIlluminationDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.giDropdown.ClearOptions();
        desiredList.Clear();
        if (postProcess.profile.TryGet(out globalIllumination))
        {
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
        }
        else
        {
            desiredList = new List<string> { "ERROR" };
        }
        gameManager.scripts.uiManager.optionsUI.giDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.giDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.giDropdown.onValueChanged.AddListener(OnGlobalIlluminationChanged);        
        gameManager.scripts.uiManager.optionsUI.giDropdown.value = 2;// Default value
        OnGlobalIlluminationChanged(gameManager.scripts.uiManager.optionsUI.giDropdown.value);
    }
    void PopulateGlobalIlluminationResDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Half", "Full"};
        gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.onValueChanged.AddListener(OnGiResolutionChanged);
        gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.value = 1;
        OnGiResolutionChanged(gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.value);
    }
    void PopulateReflectionsDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.ClearOptions();
        desiredList.Clear();
        if (postProcess.profile.TryGet(out reflections))
        {
            desiredList = new List<string>
            {
                "Off",
                "SSR Low",
                "SSR Medium",
                "SSR High",
                "RTR Low",
                "RTR Medium",
                "RTR High",
                "Realtime"
            };
        }
        else
        {
            desiredList = new List<string> { "ERROR" };
        }
        gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.onValueChanged.AddListener(OnReflectionsChanged);        
        gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value = 2;// Default value
        OnReflectionsChanged(gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value);
    }
    void PopulatePlanarReflectionsDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "Low", "Medium", "High"};
        gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.onValueChanged.AddListener(OnPlanarRelectionsChanged);
        gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.value = 0;// Default value
        OnPlanarRelectionsChanged(gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.value);
    }
    void PopulateShadowQualityDropdown()
    {
        QualitySettings.shadows = ShadowQuality.All;
        QualitySettings.shadowCascades = 4;
        gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string>{"Off", "Low", "Medium", "High", "Ultra"};
        gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.AddOptions(desiredList);        
        gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.onValueChanged.AddListener(OnShadowQualityChanged);        
        gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.value = 2;// Default value
        OnShadowQualityChanged(gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.value);
    }
    void PopulateShadowDistanceSlider()
    {        
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.onValueChanged.AddListener(OnShadowDistanceChanged);
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value = 2000;// Default value
        OnShadowDistanceChanged(gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value);
    }
    void PopulateCrowdsDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.crowdsDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string> { "Minimum", "Standard", "Full" };
        gameManager.scripts.uiManager.optionsUI.crowdsDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.crowdsDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.crowdsDropdown.onValueChanged.AddListener(OnCrowdsChanged);
        gameManager.scripts.uiManager.optionsUI.crowdsDropdown.value = 1;// Default value
        OnCrowdsChanged(gameManager.scripts.uiManager.optionsUI.crowdsDropdown.value);
    }
    void PopulateTrafficDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.trafficDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string> { "Minimum", "Standard", "Full" };
        gameManager.scripts.uiManager.optionsUI.trafficDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.trafficDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.trafficDropdown.onValueChanged.AddListener(OnTrafficChanged);
        gameManager.scripts.uiManager.optionsUI.trafficDropdown.value = 1;// Default value
        OnTrafficChanged(gameManager.scripts.uiManager.optionsUI.trafficDropdown.value);
    }
    void PopulateWildlifeDropdown()
    {
        gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.ClearOptions();
        desiredList.Clear();
        desiredList = new List<string> { "Minimum", "Standard", "Full" };
        gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.AddOptions(desiredList);
        gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.onValueChanged.AddListener(OnWildlifeChanged);
        gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.value = 1;// Default value
        OnWildlifeChanged(gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.value);
    }

    void PopulateFPSCounterToggle()
    {        
        gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.onValueChanged.AddListener(OnFPSCounterChanged);
        gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.isOn = false;// Default value
        OnFPSCounterChanged(gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.isOn);
    }
    void PopulateMemoryCounterToggle()
    {        
        gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.onValueChanged.AddListener(OnMemoryCounterChanged);
        gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.isOn = false;// Default value
        OnMemoryCounterChanged(gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.isOn);
    }
    void PopulateAudioSliders()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.optionsUI.audioMasterSlider, gameManager.scripts.uiManager.optionsUI.audioMasterNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedProperties.selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.optionsUI.audioMusicSlider, gameManager.scripts.uiManager.optionsUI.audioMusicNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedProperties.selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.optionsUI.audioAmbientSlider, gameManager.scripts.uiManager.optionsUI.audioAmbientNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedProperties.selectedSfxVolume, Strings.audioSFX, gameManager.scripts.uiManager.optionsUI.audioSFXSlider, gameManager.scripts.uiManager.optionsUI.audioSFXNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedProperties.selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.optionsUI.audioUISlider, gameManager.scripts.uiManager.optionsUI.audioUINumberText, false, false, true);
        ChangeAudioSliderValue(ref selectedProperties.selectedUiVolume, Strings.audioUIPitchShiftVolume, gameManager.scripts.uiManager.optionsUI.audioUISlider, gameManager.scripts.uiManager.optionsUI.audioUINumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedProperties.selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.optionsUI.audioVoiceSlider, gameManager.scripts.uiManager.optionsUI.audioVoiceNumberText, false, false, true);

        ChangeAudioSliderValue(ref selectedProperties.selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.optionsUI.audioEventSlider, gameManager.scripts.uiManager.optionsUI.audioEventNumberText, false, false, true);

        // Add listeners
        gameManager.scripts.uiManager.optionsUI.audioMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        gameManager.scripts.uiManager.optionsUI.audioMusicSlider.onValueChanged.AddListener(SetMusicVolume);
        gameManager.scripts.uiManager.optionsUI.audioAmbientSlider.onValueChanged.AddListener(SetAmbientVolume);
        gameManager.scripts.uiManager.optionsUI.audioSFXSlider.onValueChanged.AddListener(SetSFXVolume);
        gameManager.scripts.uiManager.optionsUI.audioUISlider.onValueChanged.AddListener(SetUIVolume);
        gameManager.scripts.uiManager.optionsUI.audioVoiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        gameManager.scripts.uiManager.optionsUI.audioEventSlider.onValueChanged.AddListener(SetEventVolume);
    }
    void PopulateMouseSliders()
    {
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.onValueChanged.AddListener(OnCameraZoomSensitivityChanged);
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value = 1;// Default value
        OnCameraZoomSensitivityChanged(gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.onValueChanged.AddListener(OnCameraMoveSensitivityChanged);
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value = 1;// Default value
        OnCameraMoveSensitivityChanged(gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.onValueChanged.AddListener(OnCameraPanSensitivityChanged);
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value = 1;// Default value
        OnCameraPanSensitivityChanged(gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value);
    }
    void PopulateGamepadSliders()
    {        
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.onValueChanged.AddListener(OnGamepadCameraZoomSensitivityChanged);
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value = 1;// Default value
        OnGamepadCameraZoomSensitivityChanged(gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.onValueChanged.AddListener(OnGamepadCameraMoveSensitivityChanged);
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value = 1;// Default value
        OnGamepadCameraMoveSensitivityChanged(gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.onValueChanged.AddListener(OnGamepadCameraPanSensitivityChanged);
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value = 1;// Default value
        OnGamepadCameraPanSensitivityChanged(gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.onValueChanged.AddListener(OnGamepadCameraZoomDeadzoneChanged);
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value = 1;// Default value
        OnGamepadCameraZoomDeadzoneChanged(gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.onValueChanged.AddListener(OnGamepadCameraMoveDeadzoneChanged);
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value = 1;// Default value
        OnGamepadCameraMoveDeadzoneChanged(gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value);

        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.onValueChanged.RemoveAllListeners();
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.onValueChanged.AddListener(OnGamepadCameraPanDeadzoneChanged);
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value = 1;// Default value
        OnGamepadCameraPanDeadzoneChanged(gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value);
    }
    #endregion


    #region Runtime
    // GAMEPLAY
    void OnAutosavesChanged(int autosavesIndex)
    {
        selectedProperties.selectedAutosavesIndex = gameManager.scripts.uiManager.optionsUI.autosavesDropdown.value;
        CheckModified();
    }
    void OnAutosavesSliderChanged(float sliderValue)
    {
        selectedProperties.selectedMaximumAutosaves = (int)gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value;
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value.ToString();
        CheckModified();
    }
    public void IncreaseAutosavesSlider()
    {
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value++;
        selectedProperties.selectedMaximumAutosaves = (int)gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value;
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value.ToString();
        CheckModified();
    }
    public void DecreaseAutosavesSlider()
    {
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value--;
        selectedProperties.selectedMaximumAutosaves = (int)gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value;
        gameManager.scripts.uiManager.optionsUI.maximumAutosavesSliderText.text = gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider.value.ToString();
        CheckModified();
    }
    void OnQuicksavesSliderChanged(float sliderValue)
    {
        selectedProperties.selectedMaximumQuicksaves = (int)gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value;
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value.ToString();
        CheckModified();
    }
    public void IncreaseQuicksavesSlider()
    {
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value++;
        selectedProperties.selectedMaximumQuicksaves = (int)gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value;
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value.ToString(Strings.numberFormat0);
        CheckModified();
    }
    public void DecreaseQuicksavesSlider()
    {
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value--;
        selectedProperties.selectedMaximumQuicksaves = (int)gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value;
        gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSliderText.text = gameManager.scripts.uiManager.optionsUI.maximumQuicksavesSlider.value.ToString(Strings.numberFormat0);
        CheckModified();
    }
    void OnGoreChanged(int goreIndex)
    {
        selectedProperties.selectedGoreIndex = gameManager.scripts.uiManager.optionsUI.goreDropdown.value;
        CheckModified();
    }
    void OnDestructionChanged(int destructionIndex)
    {
        selectedProperties.selectedDestructionIndex = gameManager.scripts.uiManager.optionsUI.destructionDropdown.value;
        CheckModified();
    }

    // VIDEO
    public void VideoPage1()
    {
        gameManager.scripts.uiManager.TintUIScriptTrigger(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2UIScript, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2Images, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2Icons, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2Effects, false);

        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1Images, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1Icons, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1Effects, true);
    }
    public void VideoPage2()
    {
        gameManager.scripts.uiManager.TintUIScriptTrigger(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1UIScript, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1Images, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1Icons, false);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage1Effects, false);

        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2Images, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2Icons, true);
        Utils.ActivateObject(gameManager.scripts.uiManager.optionsUI.optionsVideoPage2Effects, true);
    }
    public void VideoPageIncrease()
    {
        if (gameManager.scripts.uiManager.optionsUI.optionsVideoPage1.activeSelf)
        {
            VideoPage2();
        }
    }
    public void VideoPageDecrease()
    {
        if (gameManager.scripts.uiManager.optionsUI.optionsVideoPage2.activeSelf)
        {
            VideoPage1();
        }
    }

    
    void OnDisplayDeviceChanged(int deviceIndex)
    {
        selectedProperties.selectedDisplayIndex = gameManager.scripts.uiManager.optionsUI.displayDevicesDropdown.value;
        CheckModified();
    }
    void OnResolutionChanged(int resolutionIndex)
    {
        selectedProperties.selectedResolutionIndex = gameManager.scripts.uiManager.optionsUI.resolutionsDropdown.value;
        //Debug.Log($"Selected resolution index updated: {selectedResolutionIndex}. Will apply on ApplySettings().");
        CheckModified();
    }
    void OnDisplayModeChanged(int displayModeIndex)
    {
        selectedProperties.selectedDisplayModeIndex = gameManager.scripts.uiManager.optionsUI.displayModeDropdown.value;
        CheckModified();
    }
    void OnFrameRateCapToggleChanged(bool frameRateCapChanged)
    {
        if (gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.isOn)
        {
            selectedProperties.selectedFrameRateCap = true;
        }
        else if (!gameManager.scripts.uiManager.optionsUI.frameRateCapToggle.isOn)
        {
            selectedProperties.selectedFrameRateCap = false;
        }
        CheckModified();
    }
    void OnframeRateCapSliderChanged(float sliderValue)
    {
        refreshRate = (Screen.currentResolution.refreshRateRatio.denominator != 0) ? Screen.currentResolution.refreshRateRatio.numerator / (float)Screen.currentResolution.refreshRateRatio.denominator : 0f;

        if (gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value >= refreshRate)
        {
            gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = refreshRate.ToString(Strings.numberFormat0);
            selectedProperties.selectedFrameRateCapValue = (int)refreshRate;
            gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = (int)refreshRate;
        }
        else if(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value < refreshRate)
        {
            gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value.ToString(Strings.numberFormat0);
            selectedProperties.selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value;
        }        
        gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value.ToString(Strings.numberFormat0);
        CheckModified();
    }
    public void IncreaseFrameRateCap()
    {
        gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value + 1, gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.minValue, gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.maxValue);
        selectedProperties.selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value;
        gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value.ToString();
        CheckModified();
    }
    public void DecreaseFrameRateCap()
    {
        gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value - 1, gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.minValue, gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.maxValue);
        selectedProperties.selectedFrameRateCapValue = (int)gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value;
        gameManager.scripts.uiManager.optionsUI.frameRateCapSliderText.text = gameManager.scripts.uiManager.optionsUI.frameRateCapSlider.value.ToString();
        CheckModified();
    }
    void OnVSyncChanged(int vSyncIndex)
    {
        selectedProperties.selectedVsyncIndex = gameManager.scripts.uiManager.optionsUI.vSyncDropdown.value;
        CheckModified();
    }
    void OnAntiAliasChanged(int antiAliasIndex)
    {
        selectedProperties.selectedAntiAliasIndex = gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.value;
        gameManager.scripts.uiManager.optionsUI.antiAliasDropdown.RefreshShownValue();
        CheckModified();
    }
    void OnTaaQualityChanged(int taaQualityIndex)
    {
        selectedProperties.selectedTaaQualityIndex = gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.value;
        gameManager.scripts.uiManager.optionsUI.taaQualityDropdown.RefreshShownValue();
        CheckModified();
    }
    void OnFogChanged(int fogIndex)
    {
        selectedProperties.selectedFogIndex = gameManager.scripts.uiManager.optionsUI.fogDropdown.value;
        gameManager.scripts.uiManager.optionsUI.fogDropdown.RefreshShownValue();
        CheckModified();
    }
    void OnQualitySettingsChanged(int qualityIndex)
    {
        selectedProperties.selectedQualityAssetIndex = gameManager.scripts.uiManager.optionsUI.qualityDropdown.value;
        CheckModified();
    }
    public void OnFOVChanged(float sliderValue)
    {
        selectedProperties.selectedFOV = (int)gameManager.scripts.uiManager.optionsUI.fovSlider.value;
        gameManager.scripts.uiManager.optionsUI.fovSliderText.text = gameManager.scripts.uiManager.optionsUI.fovSlider.value.ToString();
        CheckModified();
    }
    public void IncreaseFOV()
    {
        gameManager.scripts.uiManager.optionsUI.fovSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.fovSlider.value + 1, gameManager.scripts.uiManager.optionsUI.fovSlider.minValue, gameManager.scripts.uiManager.optionsUI.fovSlider.maxValue);
        selectedProperties.selectedFOV = (int)gameManager.scripts.uiManager.optionsUI.fovSlider.value;
        gameManager.scripts.uiManager.optionsUI.fovSliderText.text = gameManager.scripts.uiManager.optionsUI.fovSlider.value.ToString();
        CheckModified();
    }
    public void DecreaseFOV()
    {
        gameManager.scripts.uiManager.optionsUI.fovSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.fovSlider.value - 1, gameManager.scripts.uiManager.optionsUI.fovSlider.minValue, gameManager.scripts.uiManager.optionsUI.fovSlider.maxValue);
        selectedProperties.selectedFOV = (int)gameManager.scripts.uiManager.optionsUI.fovSlider.value;
        gameManager.scripts.uiManager.optionsUI.fovSliderText.text = gameManager.scripts.uiManager.optionsUI.fovSlider.value.ToString();
        CheckModified();
    }
    void OnRenderDistanceChanged(float sliderValue)
    {
        selectedProperties.selectedRenderDistance = (int)gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value;
        gameManager.scripts.uiManager.optionsUI.renderDistanceSliderText.text = gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value.ToString();
        CheckModified();
    }
    public void IncreaseRenderDistance()
    {
        gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value + 1, gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.minValue, gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.maxValue);
        selectedProperties.selectedRenderDistance = (int)gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value;
        gameManager.scripts.uiManager.optionsUI.renderDistanceSliderText.text = gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value.ToString();
        CheckModified();
    }
    public void DecreaseRenderDistance()
    {
        gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value - 1, gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.minValue, gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.maxValue);
        selectedProperties.selectedRenderDistance = (int)gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value;
        gameManager.scripts.uiManager.optionsUI.renderDistanceSliderText.text = gameManager.scripts.uiManager.optionsUI.renderDistanceSlider.value.ToString();
        CheckModified();
    }
    void OnBloomChanged(int bloomIndex)
    {
        selectedProperties.selectedBloomIndex = gameManager.scripts.uiManager.optionsUI.bloomDropdown.value;
        gameManager.scripts.uiManager.optionsUI.bloomDropdown.RefreshShownValue();
        CheckModified();
    }
    void OnHDRChanged(int hdrChanged)
    {
        selectedProperties.selectedHDRIndex = gameManager.scripts.uiManager.optionsUI.hdrDropdown.value;
        OnTonemappingChanged(gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value);
        CheckModified();
    }
    void OnAnsioChanged(int ansioIndex)
    {
        selectedProperties.selectedAnsioIndex = gameManager.scripts.uiManager.optionsUI.ansioDropdown.value;
        CheckModified();
    }

    void OnWeatherEffectsChanged(int weatherEffectsIndex)
    {
        selectedProperties.selectedWeatherEffectsIndex = gameManager.scripts.uiManager.optionsUI.weatherEffectsDropdown.value;
        CheckModified();
    }

    void OnLineRenderingChanged(int lineRenderingIndex)
    {
        selectedProperties.selectedLineRenderingIndex = gameManager.scripts.uiManager.optionsUI.lineRenderingDropdown.value;
        CheckModified();
    }

    void OnCrevicesChanged(int crevicesIndex)
    {
        selectedProperties.selectedCrevicesIndex = gameManager.scripts.uiManager.optionsUI.crevicesDropdown.value;
        CheckModified();
    }

    public void SetAnsio(Material material, bool scanScene)
    {
        if (material && material.mainTexture)
        {
            if (selectedProperties.selectedAnsioIndex == 0)
            {
                material.mainTexture.anisoLevel = 0;
            }
            else if (selectedProperties.selectedAnsioIndex == 1)
            {
                material.mainTexture.anisoLevel = 2;
            }
            else if (selectedProperties.selectedAnsioIndex == 2)
            {
                material.mainTexture.anisoLevel = 4;
            }
            else if (selectedProperties.selectedAnsioIndex == 3)
            {
                material.mainTexture.anisoLevel = 8;
            }
            else if (selectedProperties.selectedAnsioIndex == 4)
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
                            if (selectedProperties.selectedAnsioIndex == 0)
                            {
                                //Debug.Log("Texture Set to OFF");
                                mat.mainTexture.anisoLevel = 0;
                            }
                            else if (selectedProperties.selectedAnsioIndex == 1)
                            {
                                //Debug.Log("Texture Set to 2x");
                                mat.mainTexture.anisoLevel = 2;
                            }
                            else if (selectedProperties.selectedAnsioIndex == 2)
                            {
                                //Debug.Log("Texture Set to 4x");
                                mat.mainTexture.anisoLevel = 4;
                            }
                            else if (selectedProperties.selectedAnsioIndex == 3)
                            {
                                //Debug.Log("Texture Set to 8x");
                                mat.mainTexture.anisoLevel = 8;
                            }
                            else if (selectedProperties.selectedAnsioIndex == 4)
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
    }

    void OnTonemappingChanged(int tonemappingIndex)
    {
        selectedProperties.selectedTonemappingIndex = gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value;        


        if (gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value == 2 && selectedProperties.selectedHDRIndex == 1)// Re-Populate Tonemapping Quality
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.ClearOptions();

            desiredList.Clear();
            desiredList = new List<string>
            {
                "ACES1000",
                "ACES2000",
                "ACES4000"
            };
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.AddOptions(desiredList);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = appliedTonemappingQualityACESIndex;// Default value
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.RemoveAllListeners();
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.AddListener(OnTonemappingQualityChanged);            
        }
        else if (gameManager.scripts.uiManager.optionsUI.tonemappingDropdown.value == 1)
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.ClearOptions();

            desiredList.Clear();
            desiredList = new List<string>
            {
                "Reinhard",
                "BT2390"
            };
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.AddOptions(desiredList);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = appliedTonemappingQualityIndex;// Default value
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.RemoveAllListeners();
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.AddListener(OnTonemappingQualityChanged);
        }
        else
        {
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.ClearOptions();

            desiredList.Clear();
            desiredList = new List<string>
            {
                "N/A"
            };
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.AddOptions(desiredList);
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value = 0;// Default value
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.RemoveAllListeners();
            gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.onValueChanged.AddListener(OnTonemappingQualityChanged);
        }

        desiredList.Clear();
        CheckModified();
    }
    void OnTonemappingQualityChanged(int tonemappingQualityIndex)
    {
        if (selectedProperties.selectedTonemappingIndex == 0 || selectedProperties.selectedHDRIndex == 1)
        {            
            selectedProperties.selectedTonemappingQualityIndex = gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value;
        }
        else if (selectedProperties.selectedTonemappingIndex == 1)
        {
            selectedProperties.selectedTonemappingQualityIndex = gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value;
        }
        else
        {
            selectedProperties.selectedTonemappingQualityACESIndex = gameManager.scripts.uiManager.optionsUI.tonemappingQualityDropdown.value;
        }        
        CheckModified();
    }

    void OnGlobalIlluminationChanged(int globalIlluminationIndex)
    {
        selectedProperties.selectedGlobalIlluminationIndex = gameManager.scripts.uiManager.optionsUI.giDropdown.value;
        CheckModified();
    }
    void OnGiResolutionChanged(int giResolutionChanged)
    {
        selectedProperties.selectedGlobalIlluminationResIndex = gameManager.scripts.uiManager.optionsUI.giResolutionDropdown.value;
        CheckModified();
    }

    void OnReflectionsChanged(int reflectionsIndex)
    {
        selectedProperties.selectedReflectionsIndex = gameManager.scripts.uiManager.optionsUI.reflectionsDropdown.value;
        CheckModified();
    }
    void OnPlanarRelectionsChanged(int planarRelectionsChanged)
    {
        selectedProperties.selectedPlanarReflectionsIndex = gameManager.scripts.uiManager.optionsUI.planarReflectionsDropdown.value;
        CheckModified();
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
    }

    void OnShadowQualityChanged(int shadowIndex)
    {
        selectedProperties.selectedShadowQualityIndex = gameManager.scripts.uiManager.optionsUI.shadowQualityDropdown.value;
        CheckModified();
    }
    void OnShadowDistanceChanged(float sliderValue)
    {
        selectedProperties.selectedShadowDistance = (int)gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value;
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSliderText.text = gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value.ToString();
        CheckModified();
    }
    public void IncreaseShadowDistance()
    {
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value + 1, gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.minValue, gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.maxValue);
        selectedProperties.selectedShadowDistance = (int)gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value;
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSliderText.text = gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value.ToString();
        CheckModified();
    }
    public void DecreaseShadowDistance()
    {
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value = Mathf.Clamp(gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value - 1, gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.minValue, gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.maxValue);
        selectedProperties.selectedShadowDistance = (int)gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value;
        gameManager.scripts.uiManager.optionsUI.shadowDistanceSliderText.text = gameManager.scripts.uiManager.optionsUI.shadowDistanceSlider.value.ToString();
        CheckModified();
    }
    void OnCrowdsChanged(int crowdsIndex)
    {
        selectedProperties.selectedCrowdsIndex = gameManager.scripts.uiManager.optionsUI.crowdsDropdown.value;
        CheckModified();
    }
    void OnTrafficChanged(int trafficIndex)
    {
        selectedProperties.selectedTrafficIndex = gameManager.scripts.uiManager.optionsUI.trafficDropdown.value;
        CheckModified();
    }
    void OnWildlifeChanged(int wildlifeIndex)
    {
        selectedProperties.selectedWildlifeIndex = gameManager.scripts.uiManager.optionsUI.wildlifeDropdown.value;
        CheckModified();
    }
    void OnFPSCounterChanged(bool fpsCounterChanged)
    {
        if (gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.isOn)
        {
            selectedProperties.selectedFPSCounter = true;
        }
        else if (!gameManager.scripts.uiManager.optionsUI.fpsCounterToggle.isOn)
        {
            selectedProperties.selectedFPSCounter = false;
        }
        CheckModified();
    }

    void OnMemoryCounterChanged(bool memoryCounterChanged)
    {
        if (gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.isOn)
        {
            selectedProperties.selectedMemoryCounter = true;
        }
        else if (!gameManager.scripts.uiManager.optionsUI.memoryCounterToggle.isOn)
        {
            selectedProperties.selectedMemoryCounter = false;
        }
        CheckModified();
    }

    // AUDIO
    public void SetMasterVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.optionsUI.audioMasterSlider, gameManager.scripts.uiManager.optionsUI.audioMasterNumberText, true, false, false);

    }
    public void IncreaseMasterVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.optionsUI.audioMasterSlider, gameManager.scripts.uiManager.optionsUI.audioMasterNumberText, true, true, false);
    }
    public void DecreaseMasterVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMasterVolume, Strings.audioMaster, gameManager.scripts.uiManager.optionsUI.audioMasterSlider, gameManager.scripts.uiManager.optionsUI.audioMasterNumberText, false, true, false);
    }

    public void SetMusicVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.optionsUI.audioMusicSlider, gameManager.scripts.uiManager.optionsUI.audioMusicNumberText, true, false, false);
    }
    public void IncreaseMusicVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.optionsUI.audioMusicSlider, gameManager.scripts.uiManager.optionsUI.audioMusicNumberText, true, true, false);
    }
    public void DecreaseMusicVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedMusicVolume, Strings.audioMusic, gameManager.scripts.uiManager.optionsUI.audioMusicSlider, gameManager.scripts.uiManager.optionsUI.audioMusicNumberText, false, true, false);
    }

    public void SetAmbientVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.optionsUI.audioAmbientSlider, gameManager.scripts.uiManager.optionsUI.audioAmbientNumberText, true, false, false);
    }
    public void IncreaseAmbientVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.optionsUI.audioAmbientSlider, gameManager.scripts.uiManager.optionsUI.audioAmbientNumberText, true, true, false);
    }
    public void DecreaseAmbientVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedAmbientVolume, Strings.audioAmbient, gameManager.scripts.uiManager.optionsUI.audioAmbientSlider, gameManager.scripts.uiManager.optionsUI.audioAmbientNumberText, false, true, false);
    }

    public void SetSFXVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedSfxVolume, Strings.audioAmbient, gameManager.scripts.uiManager.optionsUI.audioSFXSlider, gameManager.scripts.uiManager.optionsUI.audioSFXNumberText, true, false, false);
    }
    public void IncreaseSFXVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedSfxVolume, Strings.audioSFX, gameManager.scripts.uiManager.optionsUI.audioSFXSlider, gameManager.scripts.uiManager.optionsUI.audioSFXNumberText, true, true, false);
    }
    public void DecreaseSFXVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedSfxVolume, Strings.audioSFX, gameManager.scripts.uiManager.optionsUI.audioSFXSlider, gameManager.scripts.uiManager.optionsUI.audioSFXNumberText, false, true, false);
    }

    public void SetUIVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.optionsUI.audioUISlider, gameManager.scripts.uiManager.optionsUI.audioUINumberText, true, false, false);
        ChangeAudioSliderValue(ref selectedProperties.selectedUiVolume, Strings.audioUIPitchShiftVolume, gameManager.scripts.uiManager.optionsUI.audioUISlider, gameManager.scripts.uiManager.optionsUI.audioUINumberText, true, false, false);
    }
    public void IncreaseUIVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.optionsUI.audioUISlider, gameManager.scripts.uiManager.optionsUI.audioUINumberText, true, true, false);
    }
    public void DecreaseUIVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedUiVolume, Strings.audioUI, gameManager.scripts.uiManager.optionsUI.audioUISlider, gameManager.scripts.uiManager.optionsUI.audioUINumberText, false, true, false);
    }

    public void SetVoiceVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.optionsUI.audioVoiceSlider, gameManager.scripts.uiManager.optionsUI.audioVoiceNumberText, true, false, false);
    }
    public void IncreaseVoiceVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.optionsUI.audioVoiceSlider, gameManager.scripts.uiManager.optionsUI.audioVoiceNumberText, true, true, false);
    }
    public void DecreaseVoiceVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedVoiceVolume, Strings.audioVoice, gameManager.scripts.uiManager.optionsUI.audioVoiceSlider, gameManager.scripts.uiManager.optionsUI.audioVoiceNumberText, false, true, false);
    }

    public void SetEventVolume(float sliderValue)
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.optionsUI.audioEventSlider, gameManager.scripts.uiManager.optionsUI.audioEventNumberText, true, false, false);
    }
    public void IncreaseEventVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.optionsUI.audioEventSlider, gameManager.scripts.uiManager.optionsUI.audioEventNumberText, true, true, false);
    }
    public void DecreaseEventVolume()
    {
        ChangeAudioSliderValue(ref selectedProperties.selectedEventVolume, Strings.audioEvent, gameManager.scripts.uiManager.optionsUI.audioEventSlider, gameManager.scripts.uiManager.optionsUI.audioEventNumberText, false, true, false);
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
            if(slider == gameManager.scripts.uiManager.optionsUI.audioMasterSlider)// Master volume set to 100% by default
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
        selectedProperties.selectedCameraZoomSensitivity = gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value += 0.01f;
        selectedProperties.selectedCameraZoomSensitivity = gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value.ToString();
    }
    public void DecreaseCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value -= 0.01f;
        selectedProperties.selectedCameraZoomSensitivity = gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraZoomSensitivitySlider.value.ToString();
    }

    void OnCameraMoveSensitivityChanged(float sliderValue)
    {
        selectedProperties.selectedCameraMoveSensitivity = gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value += 0.01f;
        selectedProperties.selectedCameraMoveSensitivity = gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value.ToString();
    }
    public void DecreaseCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value -= 0.01f;
        selectedProperties.selectedCameraMoveSensitivity = gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraMoveSensitivitySlider.value.ToString();
    }

    void OnCameraPanSensitivityChanged(float sliderValue)
    {
        selectedProperties.selectedCameraPanSensitivity = gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value;// Get selected display index from the dropdown
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value += 0.01f;
        selectedProperties.selectedCameraPanSensitivity = gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value.ToString();
    }
    public void DecreaseCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value -= 0.01f;
        selectedProperties.selectedCameraPanSensitivity = gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraPanSensitivitySlider.value.ToString();
    }

    void OnGamepadCameraZoomSensitivityChanged(float sliderValue)
    {
        selectedProperties.selectedGamepadCameraZoomSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseGamepadCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value += 0.01f;
        selectedProperties.selectedGamepadCameraZoomSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void DecreaseGamepadCameraZoomSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value -= 0.01f;
        selectedProperties.selectedGamepadCameraZoomSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomSensitivitySlider.value.ToString(Strings.numberFormat1);
    }

    void OnGamepadCameraMoveSensitivityChanged(float sliderValue)
    {
        selectedProperties.selectedGamepadCameraMoveSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseGamepadCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value += 0.01f;
        selectedProperties.selectedGamepadCameraMoveSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void DecreaseGamepadCameraMoveSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value -= 0.01f;
        selectedProperties.selectedGamepadCameraMoveSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveSensitivitySlider.value.ToString(Strings.numberFormat1);
    }

    void OnGamepadCameraPanSensitivityChanged(float sliderValue)
    {
        selectedProperties.selectedGamepadCameraPanSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseGamepadCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value += 0.01f;
        selectedProperties.selectedGamepadCameraPanSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value.ToString(Strings.numberFormat1);
    }
    public void DecreaseGamepadCameraPanSensitivity()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value -= 0.01f;
        selectedProperties.selectedGamepadCameraPanSensitivity = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanSensitivitySlider.value.ToString(Strings.numberFormat1);
    }

    void OnGamepadCameraZoomDeadzoneChanged(float sliderValue)
    {
        selectedProperties.selectedGamepadCameraZoomDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseGamepadCameraZoomDeadzone()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value += 0.01f;
        selectedProperties.selectedGamepadCameraZoomDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    public void DecreaseGamepadCameraZoomDeadzone()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value -= 0.01f;
        selectedProperties.selectedGamepadCameraZoomDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadZoomDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }

    void OnGamepadCameraMoveDeadzoneChanged(float sliderValue)
    {
        selectedProperties.selectedGamepadCameraMoveDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseGamepadCameraMoveDeadzone()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value += 0.01f;
        selectedProperties.selectedGamepadCameraMoveDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    public void DecreaseGamepadCameraMoveDeadzone()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value -= 0.01f;
        selectedProperties.selectedGamepadCameraMoveDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadMoveDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }

    void OnGamepadCameraPanDeadzoneChanged(float sliderValue)
    {
        selectedProperties.selectedGamepadCameraPanDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    public void IncreaseGamepadCameraPanDeadzone()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value += 0.01f;
        selectedProperties.selectedGamepadCameraPanDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    public void DecreaseGamepadCameraPanDeadzone()
    {
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value -= 0.01f;
        selectedProperties.selectedGamepadCameraPanDeadzone = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value;
        gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSliderText.text = gameManager.scripts.uiManager.optionsUI.cameraGamepadPanDeadzoneSlider.value.ToString(Strings.numberFormat1);
    }
    #endregion
}

