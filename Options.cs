using UnityEngine;
using Unity.Cinemachine;
using TMPro;
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
    [Tooltip("Unlimited FPS Cap")]
    [SerializeField] private bool selectedUncapFPS = false;
    [Tooltip("Desired FPS Cap")]
    [SerializeField, Range(30, 1000)] private int fps = 90;
    [Tooltip("Desired Field of View on players camera")]
    [SerializeField, Range(65, 120)] private int fov = 90;
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
    [SerializeField] private float updateInterval = 0f;
    public bool selectedShowFPS;    
    public bool selectedShowPing;    
    public bool selectedShowMemory;
    
    [Header("[Localization")]
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
    [Space(5)]
    [HideInInspector] public bool appliedUncapFPS;
    [HideInInspector] public int appliedFPS;

    [HideInInspector] public bool appliedPing;
    [HideInInspector] public bool appliedMemory;
    private void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        timeleft = updateInterval;
    }

    private void Start()
    {
        gameManager.options = this;
        totalSystemMemory = SystemInfo.systemMemorySize;
        totalSystemMemoryGB = totalSystemMemory / 1024f;
        mainDisplay = Display.main;
        currentResolution = Screen.currentResolution;
        resolutions = Screen.resolutions;
        float maxRefreshRate = mainDisplay.systemWidth / mainDisplay.systemHeight; // Display ratio, you might need an alternative approach       

        /*
        foreach (Resolution resolution in resolutions)
        {
            Debug.Log("Resolution: " + resolution.width + "x" + resolution.height + " @ " + resolution.refreshRateRatio + "Hz");
        }
        Debug.Log("Maximum Monitor Refresh Rate Ratio: " + currentResolution.refreshRateRatio);
        */
    }

    void ApplySettings()
    {
        if (Application.isPlaying)
        {
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

            if (appliedPing)
            {
                // Ping display logic
            }

            if (appliedMemory)
            {
                allocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
                allocatedMemoryGB = allocatedMemory / (1024f * 1024f * 1024f);
                gameManager.uiManager.diagnosticsMemoryCounterText.color = Color.white;
                gameManager.uiManager.diagnosticsMemoryCounterText.text = $"Mem: {allocatedMemoryGB:F2} GB / {totalSystemMemoryGB:F2} GB";
            }
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