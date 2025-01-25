using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class Options : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public CinemachineCamera playerCinemachineCamera;
    private Display mainDisplay;
    private Resolution currentResolution;
    private Resolution[] resolutions;
    private float fpsDisplay;    
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private int totalSystemMemory;
    private float totalSystemMemoryGB;
    private long allocatedMemory;
    private float allocatedMemoryGB;
    
    [HideInInspector] public bool invertMouseVertical;
    [HideInInspector] public float mouseHorizontalSensitivity = 1f;
    [HideInInspector] public float mouseVerticalSensitivity = 1f;
    [HideInInspector] public bool invertControllerVertical;
    [HideInInspector] public float controllerHorizontalSensitivity = 1f;
    [HideInInspector] public float controllerVerticalSensitivity = 1f;   
    
    [Header("---- Apply -----")]
    [Space(5)]
    public bool applySettings;
    [Header("---- SELECTED ------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private bool selectedAutosaves;
    [SerializeField] private int selectedMaximumAutosaves;
    [SerializeField] private int selectedMaximumQuicksaves;
    [SerializeField] private bool selectedGore;

    [Tooltip("Unlimited FPS Cap")]
    [SerializeField] private bool selectedUncapFPS;
    [Tooltip("Desired FPS Cap")]
    [SerializeField, Range(30, 1000)] private int fps;
    [Tooltip("Desired Field of View on players camera")]
    [SerializeField, Range(65, 120)] private int fov;
    [Header("---- Controls ------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private bool invertMouseY;
    [SerializeField] private float mouseXSensitivity = 1f;
    [SerializeField] private float mouseYSensitivity = 1f;
    [SerializeField] private bool invertControllerY;
    [SerializeField] private float controllerXSensitivity = 1f;
    [SerializeField] private float controllerYSensitivity = 1f;
    [Header("---- Diagnostics ------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private float updateInterval = 0.1f;
    public bool selectedShowFPS;    
    public bool selectedShowPing;    
    public bool selectedShowMemory;
    
    [Header("[Localization]")]
    [Space(5)]
    public bool languageEnglish;
    public bool languageFrench;
    public bool languageGerman;
    public bool languageItalian;
    public bool languageSpanish;
    public bool languageAmerican;
    public bool languageRussian;
    public bool languageChinese;
    public bool languageJapanese;
    public bool languageBrazilian;

    [Header("---- APPLIED ------------------------------------------------------------")]
    [Header("Gameplay")]
    public bool appliedAutosaves;
    public int appliedAutosavesAmount;
    public int appliedQuicksavesAmount;
    public bool appliedGore;
    public bool appliedUncapFPS;
    public int appliedFPS;
    public bool appliedLanguageEnglish;
    public bool appliedLanguageFrench;
    public bool appliedLanguageGerman;
    public bool appliedLanguageItalian;
    public bool appliedLanguageSpanish;
    public bool appliedLanguageAmerican;
    public bool appliedLanguageRussian;
    public bool appliedLanguageChinese;
    public bool appliedLanguageJapanese;
    public bool appliedLanguageBrazilian;










    [HideInInspector] public bool appliedMemory;
    private void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        timeleft = updateInterval;
        gameManager.options = this;
    }

    private void Start()
    {        
        selectedAutosaves = true;
        selectedMaximumAutosaves = 10;
        selectedMaximumQuicksaves = 10;
        selectedGore = true;
        selectedUncapFPS = false;
        

        totalSystemMemory = SystemInfo.systemMemorySize;
        totalSystemMemoryGB = totalSystemMemory / 1024f;
        mainDisplay = Display.main;
    }

    public void ApplySettings()
    {
        if (Application.isPlaying)
        {
            // GAMEPLAY--------------
            if (selectedAutosaves)
            {
                appliedAutosaves = true;
            }
            else if(!selectedAutosaves)
            {
                appliedAutosaves = false;
            }

            appliedAutosavesAmount = selectedMaximumAutosaves;
            appliedQuicksavesAmount = selectedMaximumQuicksaves;

            if (selectedGore)
            {
                appliedGore = true;
            }
            else if (!selectedGore)
            {
                appliedGore = false;
            }

            if (languageEnglish)
            {
                appliedLanguageEnglish = true;
            }
            if (languageFrench)
            {
                appliedLanguageFrench = true;
            }
            if (languageGerman)
            {
                appliedLanguageGerman = true;
            }
            if (languageItalian)
            {
                appliedLanguageItalian = true;
            }
            if (languageSpanish)
            {
                appliedLanguageSpanish = true;
            }
            if (languageAmerican)
            {
                appliedLanguageAmerican = true;
            }
            if (languageRussian)
            {
                appliedLanguageRussian = true;
            }
            if (languageChinese)
            {
                appliedLanguageChinese = true;
            }
            if (languageJapanese)
            {
                appliedLanguageJapanese = true;
            }
            if (languageBrazilian)
            {
                appliedLanguageBrazilian = true;
            }

            OptionsRevertModifications();
            gameManager.uiManager.OptionsUpdateUIValues();
            gameManager.saveManager.SaveOptionsAsync();

            gameManager.optionsValuesModified = false;
            Utils.ClearMemory();
        }
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (applySettings)
            {
                ApplySettings();
                applySettings = false;
            }
        }
    }

    public void OptionsModifiedValueCheck(Toggle toggle, bool toggleCheck, Slider slider, float sliderCheck, ref GameObject modifiedValueImageObject)
    {
        if (toggle && slider)
        {
            Debug.LogError("referenced toggle AND slider, please do one or the other.");
            return;
        }
        if (toggle)
        {            
            if (toggle.isOn && toggleCheck != true || !toggle.isOn && toggleCheck == true)
            {                
                if (modifiedValueImageObject)
                {                    
                    if (!modifiedValueImageObject.activeSelf)
                    {                        
                        modifiedValueImageObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (modifiedValueImageObject)
                {                    
                    if (modifiedValueImageObject.activeSelf)
                    {                        
                        modifiedValueImageObject.SetActive(false);
                    }
                }
            }
        }

        if(slider)
        {
            if (slider.value != sliderCheck)
            {
                if (modifiedValueImageObject)
                {
                    if (!modifiedValueImageObject.activeSelf)
                    {
                        modifiedValueImageObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (modifiedValueImageObject)
                {
                    if (modifiedValueImageObject.activeSelf)
                    {
                        modifiedValueImageObject.SetActive(false);
                    }
                }
            }
        }
        Utils.ClearMemory();
    }

    public void OptionsRevertModifications()
    {
        // GAMEPLAY--------------
        if (appliedAutosaves)
        {
            selectedAutosaves = true;
            gameManager.uiManager.autosavesToggle.isOn = true;
        }
        else if (!appliedAutosaves)
        {
            selectedAutosaves = false;
            gameManager.uiManager.autosavesToggle.isOn = false;
        }
        gameManager.uiManager.autosavesToggleModifiedGameobject.SetActive(false);

        selectedMaximumAutosaves = appliedAutosavesAmount;
        gameManager.uiManager.maximumAutosavesSlider.value = appliedAutosavesAmount;
        gameManager.uiManager.maximumAutosavesSliderModifiedGameobject.SetActive(false);

        selectedMaximumQuicksaves = appliedQuicksavesAmount;
        gameManager.uiManager.maximumQuicksavesSlider.value = appliedQuicksavesAmount;
        gameManager.uiManager.maximumQuicksavesSliderModifiedGameobject.SetActive(false);

        if (appliedGore)
        {
            selectedGore = true;
            gameManager.uiManager.goreToggle.isOn = true;
        }
        else if (!appliedGore)
        {
            selectedGore = false;
            gameManager.uiManager.goreToggle.isOn = false;
        }
        gameManager.uiManager.goreToggleModifiedGameobject.SetActive(false);

        if (appliedLanguageEnglish)
        {
            languageEnglish = true;
        }
        if (appliedLanguageFrench == true)
        {
            languageFrench = true;
        }
        if (appliedLanguageGerman == true)
        {
            languageGerman = true;
        }
        if (appliedLanguageItalian == true)
        {
            languageItalian = true;
        }
        if (appliedLanguageSpanish == true)
        {
            languageSpanish = true;
        }
        if (appliedLanguageAmerican == true)
        {
            languageAmerican = true;
        }
        if (appliedLanguageRussian == true)
        {
            languageRussian = true;
        }
        if (appliedLanguageChinese == true)
        {
            languageChinese = true;
        }
        if (appliedLanguageJapanese == true)
        {
            languageJapanese = true;
        }
        if (appliedLanguageBrazilian == true)
        {
            languageBrazilian = true;
        }


        gameManager.optionsValuesModified = false;
        Utils.ClearMemory();
    }

    public void CheckOptionsSaved()
    {
        if (gameManager.optionsValuesModified)
        {
            gameManager.viewingWarningNotification = true;
            gameManager.localizeLanguage = true;
        }
        else
        {
            gameManager.MainMenu();
        }
    }

    public void OptionsSelectedShowFPS()
    {
        if (!selectedShowFPS)
        {
            selectedShowFPS = true;
        }
        else
        {
            selectedShowFPS = false;
        }        
    }
    public void OptionsSelectedShowPing()
    {
        if (!selectedShowPing)
        {
            selectedShowPing = true;
        }
        else
        {
            selectedShowPing = false;
        }
    }
    public void OptionsSelectedShowMemory()
    {
        if (!selectedShowMemory)
        {
            selectedShowMemory = true;
        }
        else
        {
            selectedShowMemory = false;
        }
    }

    public void OptionsSelectedAutosaves()
    {
        if (gameManager.uiManager.autosavesToggle.isOn)
        {
            selectedAutosaves = true;
        }
        else if (!gameManager.uiManager.autosavesToggle.isOn)
        {
            selectedAutosaves = false;
        }        
        OptionsModifiedValueCheck(gameManager.uiManager.autosavesToggle, appliedAutosaves, null, 0, ref gameManager.uiManager.autosavesToggleModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
    }
    public void OptionsSelectedGore()
    {
        if (gameManager.uiManager.goreToggle.isOn)
        {
            selectedGore = true;
        }
        else if (!gameManager.uiManager.autosavesToggle.isOn)
        {
            selectedGore = false;
        }
        OptionsModifiedValueCheck(gameManager.uiManager.goreToggle, appliedGore, null, 0, ref gameManager.uiManager.goreToggleModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE        
    }

    public void OptionsMaximumAutosavesSliderChanged()
    {
        selectedMaximumAutosaves = (int)gameManager.uiManager.maximumAutosavesSlider.value;
        
        gameManager.uiManager.OptionsUpdateUIValues();
        gameManager.localizeLanguage = true;
        OptionsModifiedValueCheck(null, false, gameManager.uiManager.maximumAutosavesSlider, appliedAutosavesAmount, ref gameManager.uiManager.maximumAutosavesSliderModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
    }
    public void OptionsMaximumAutosavesSliderIncreased()
    {
        gameManager.uiManager.maximumAutosavesSlider.value = gameManager.uiManager.maximumAutosavesSlider.value + 1;
        selectedMaximumAutosaves = (int)gameManager.uiManager.maximumAutosavesSlider.value;
        
        gameManager.uiManager.OptionsUpdateUIValues();
        gameManager.localizeLanguage = true;
        OptionsModifiedValueCheck(null, false, gameManager.uiManager.maximumAutosavesSlider, appliedAutosavesAmount, ref gameManager.uiManager.maximumAutosavesSliderModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
    }
    public void OptionsMaximumAutosavesSliderDecreased()
    {
        gameManager.uiManager.maximumAutosavesSlider.value = gameManager.uiManager.maximumAutosavesSlider.value - 1;
        selectedMaximumAutosaves = (int)gameManager.uiManager.maximumAutosavesSlider.value;
        
        OptionsModifiedValueCheck(null, false, gameManager.uiManager.maximumAutosavesSlider, appliedAutosavesAmount, ref gameManager.uiManager.maximumAutosavesSliderModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
        gameManager.uiManager.OptionsUpdateUIValues();
        gameManager.localizeLanguage = true;
    }

    public void OptionsMaximumQuicksavesSliderChanged()
    {
        selectedMaximumQuicksaves = (int)gameManager.uiManager.maximumQuicksavesSlider.value;        

        OptionsModifiedValueCheck(null, false, gameManager.uiManager.maximumQuicksavesSlider, appliedQuicksavesAmount, ref gameManager.uiManager.maximumQuicksavesSliderModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
        gameManager.uiManager.OptionsUpdateUIValues();
        gameManager.localizeLanguage = true;
    }
    public void OptionsMaximumQuicksavesSliderIncreased()
    {
        gameManager.uiManager.maximumQuicksavesSlider.value = gameManager.uiManager.maximumQuicksavesSlider.value + 1;
        selectedMaximumQuicksaves = (int)gameManager.uiManager.maximumQuicksavesSlider.value;        

        OptionsModifiedValueCheck(null, false, gameManager.uiManager.maximumQuicksavesSlider, appliedQuicksavesAmount, ref gameManager.uiManager.maximumQuicksavesSliderModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
        gameManager.uiManager.OptionsUpdateUIValues();
        gameManager.localizeLanguage = true;
    }
    public void OptionsMaximumQuicksavesSliderDecreased()
    {
        gameManager.uiManager.maximumQuicksavesSlider.value = gameManager.uiManager.maximumQuicksavesSlider.value - 1;
        selectedMaximumQuicksaves = (int)gameManager.uiManager.maximumQuicksavesSlider.value;        

        OptionsModifiedValueCheck(null, false, gameManager.uiManager.maximumQuicksavesSlider, appliedQuicksavesAmount, ref gameManager.uiManager.maximumQuicksavesSliderModifiedGameobject);// ACTIVATE VALUE MODIFIED IMAGE
        gameManager.uiManager.OptionsUpdateUIValues();
        gameManager.localizeLanguage = true;
    }

    #region Localization
    public void ApplyEnglishLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.englishFlag;
                
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageEnglish = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyFrenchLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.frenchFlag;

        languageEnglish = false;        
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageFrench = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyGermanLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.germanFlag;

        languageEnglish = false;
        languageFrench = false;        
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageGerman = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyItalianLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.italianFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;        
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageItalian = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplySpanishLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.spanishFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;        
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageSpanish = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyAmericanLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.americanFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;        
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageAmerican = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyRussianLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.russianFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;        
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = false;
        languageRussian = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyChineseLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.chineseFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;        
        languageJapanese = false;
        languageBrazilian = false;
        languageChinese = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyJapaneseLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.japaneseFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;        
        languageBrazilian = false;
        languageJapanese = true;
        gameManager.localizeLanguage = true;
    }

    public void ApplyBrazilianLanguage()
    {
        gameManager.uiManager.selectedLanguageImage.sprite = gameManager.uiManager.brazilFlag;

        languageEnglish = false;
        languageFrench = false;
        languageGerman = false;
        languageItalian = false;
        languageSpanish = false;
        languageAmerican = false;
        languageRussian = false;
        languageChinese = false;
        languageJapanese = false;
        languageBrazilian = true;
        gameManager.localizeLanguage = true;
    }
    #endregion
}

/*
             // DIAGNOSTICS ----------------------------------------
            if (appliedUncapFPS)
            {
                timeleft -= Time.deltaTime;
                accum += Time.timeScale / Time.deltaTime;
                frames++;

                if (timeleft <= 0.0f)
                {      
                    fpsDisplay = Mathf.RoundToInt(accum / frames);

                    timeleft = updateInterval;// Reset counters
                    accum = 0;
                    frames = 0;                   

                    if (fpsDisplay < 40)
                    {
                        gameManager.uiManager.diagnosticsFPSCounterText.color = Color.red;
                    }
                    else if (fpsDisplay >= 40 && fpsDisplay <= 70)
                    {
                        gameManager.uiManager.diagnosticsFPSCounterText.color = Color.yellow;
                    }
                    else
                    {
                        gameManager.uiManager.diagnosticsFPSCounterText.color = Color.green;
                    }

                    gameManager.uiManager.diagnosticsFPSCounterText.text = $"<color=white>FPS:</color> {fpsDisplay}";
                }
            }

            if (appliedMemory)
            {
                allocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
                allocatedMemoryGB = allocatedMemory / (1024f * 1024f * 1024f);
                gameManager.uiManager.diagnosticsMemoryCounterText.color = Color.white;
                gameManager.uiManager.diagnosticsMemoryCounterText.text = $"Mem: {allocatedMemoryGB:F2} GB / {totalSystemMemoryGB:F2} GB";
            }




       currentResolution = Screen.currentResolution;
        resolutions = Screen.resolutions;
        float maxRefreshRate = mainDisplay.systemWidth / mainDisplay.systemHeight; // Display ratio, you might need an alternative approach       
        
        /*
        foreach (Resolution resolution in resolutions)
        {
            Debug.Log("Resolution: " + resolution.width + "x" + resolution.height + " @ " + resolution.refreshRateRatio + "Hz");
        }
        Debug.Log("Maximum Monitor Refresh Rate Ratio: " + currentResolution.refreshRateRatio);
        

// VIDEO-------------------
if (selectedUncapFPS)
{
    appliedUncapFPS = true;
    Application.targetFrameRate = 0; // Uncap the frame rate
    appliedFPS = fps;
}
else if (fps != appliedFPS)
{
    appliedUncapFPS = false;

    if (fps > currentResolution.refreshRateRatio.value)
    {
        fps = (int)currentResolution.refreshRateRatio.value;
        Debug.Log("Monitor is set @ " + currentResolution.refreshRateRatio + "Hz, FPS will be capped to this value");
    }

    Application.targetFrameRate = fps;
    Debug.Log(Application.targetFrameRate);
    appliedFPS = fps;
}
else
{
    appliedUncapFPS = false;
    Application.targetFrameRate = fps;
    appliedFPS = fps;
    Debug.Log(Application.targetFrameRate);
}

if (playerCinemachineCamera)
{
    playerCinemachineCamera.Lens.FieldOfView = fov;
}

if (selectedShowFPS)
{
    gameManager.uiManager.diagnosticsFPSCounterText.enabled = true;
    appliedUncapFPS = true;
    gameManager.viewingFPS = true;
}
else
{
    appliedUncapFPS = false;
    gameManager.viewingFPS = false;
    gameManager.uiManager.diagnosticsFPSCounterText.enabled = false;
}

if (selectedShowPing)
{
    gameManager.uiManager.diagnosticsPingCounterText.enabled = true;
    gameManager.viewingPing = true;
    appliedPing = true;
}
else
{
    appliedPing = false;
    gameManager.viewingPing = false;
    gameManager.uiManager.diagnosticsPingCounterText.enabled = false;
}

if (selectedShowMemory)
{
    gameManager.uiManager.diagnosticsMemoryCounterText.enabled = true;
    gameManager.viewingMemory = true;
    appliedMemory = true;
}
else
{
    appliedMemory = false;
    gameManager.viewingMemory = false;
    gameManager.uiManager.diagnosticsMemoryCounterText.enabled = false;
}

if (invertMouseY)
{
    invertMouseVertical = true;
}
else
{
    invertMouseVertical = false;
}
mouseHorizontalSensitivity = mouseXSensitivity;
mouseVerticalSensitivity = mouseYSensitivity;

if (invertControllerY)
{
    invertControllerVertical = true;
}
else
{
    invertControllerVertical = false;
}
controllerHorizontalSensitivity = controllerXSensitivity;
controllerVerticalSensitivity = controllerYSensitivity;
*/