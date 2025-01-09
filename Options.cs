using UnityEngine;
using Unity.Cinemachine;
using TMPro;
using UnityEngine.Profiling;

public class Options : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public CinemachineCamera playerCinemachineCamera;
    [SerializeField] private TextMeshProUGUI fpsCounterTMP;
    [SerializeField] private TextMeshProUGUI pingCounterTMP;
    [SerializeField] private TextMeshProUGUI memoryCounterTMP;
    private Display mainDisplay;
    private Resolution currentResolution;
    private Resolution[] resolutions;
    private float fpsDisplay;
    private int lastFPS;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private int totalSystemMemory;
    private float totalSystemMemoryGB;
    private long allocatedMemory;
    private float allocatedMemoryGB;
    private bool uncapFPS;
    [HideInInspector] public bool invertMouseVertical;
    [HideInInspector] public float mouseHorizontalSensitivity = 1f;
    [HideInInspector] public float mouseVerticalSensitivity = 1f;
    [HideInInspector] public bool invertControllerVertical;
    [HideInInspector] public float controllerHorizontalSensitivity = 1f;
    [HideInInspector] public float controllerVerticalSensitivity = 1f;
    [HideInInspector] public bool displayFPS;
    [HideInInspector] public bool displayPing;
    [HideInInspector] public bool displayMemory;
    [Header("---- Apply -----")]
    [Space(5)]
    [SerializeField] private bool applySettings;
    [Header("---- Graphics ------------------------------------------------------------")]
    [Space(5)]
    [Tooltip("Unlimited FPS Cap")]
    [SerializeField] private bool unlimitedFPS = false;
    [Tooltip("Desired FPS Cap")]
    [SerializeField, Range(30, 280)] private int fps = 90;
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
    public bool fpsCounter;
    public bool pingCounter;
    public bool memoryCounter;


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
            if (unlimitedFPS)
            {    
                uncapFPS = true;                
                Application.targetFrameRate = fps; // Uncap the frame rate
                lastFPS = fps;
            }
            else if (fps != lastFPS)
            {
                uncapFPS = false;

                if (fps > currentResolution.refreshRateRatio.value)
                {
                    fps = (int)currentResolution.refreshRateRatio.value;
                    Debug.Log("Monitor is set @ " + currentResolution.refreshRateRatio + "Hz, FPS will be capped to this value");                    
                }
                
                Application.targetFrameRate = fps;
                Debug.Log(Application.targetFrameRate);
                lastFPS = fps;
            }
            else
            {
                uncapFPS = false;
                Application.targetFrameRate = fps;
                lastFPS = fps;
                Debug.Log(Application.targetFrameRate);
            }

            if (playerCinemachineCamera)
            {
                playerCinemachineCamera.Lens.FieldOfView = fov;
            }

            if (fpsCounter)
            {
                fpsCounterTMP.enabled = true;
                displayFPS = true;
            }
            else
            {
                displayFPS = false;
                fpsCounterTMP.enabled = false;
            }

            if (pingCounter)
            {
                pingCounterTMP.enabled = true;
                displayPing = true;
            }
            else
            {
                displayPing = false;
                pingCounterTMP.enabled = false;
            }

            if (memoryCounter)
            {
                memoryCounterTMP.enabled = true;
                displayMemory = true;
            }
            else
            {
                displayMemory = false;
                memoryCounterTMP.enabled = false;
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
            if (displayFPS)
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
                        fpsCounterTMP.color = Color.red;
                    }
                    else if (fpsDisplay >= 40 && fpsDisplay <= 70)
                    {
                        fpsCounterTMP.color = Color.yellow;
                    }
                    else
                    {
                        fpsCounterTMP.color = Color.green;
                    }

                    fpsCounterTMP.text = $"<color=white>FPS:</color> {fpsDisplay}";
                }
            }

            if (displayPing)
            {
                // Ping display logic
            }

            if (displayMemory)
            {
                allocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
                allocatedMemoryGB = allocatedMemory / (1024f * 1024f * 1024f);
                memoryCounterTMP.color = Color.white;
                memoryCounterTMP.text = $"Mem: {allocatedMemoryGB:F2} GB / {totalSystemMemoryGB:F2} GB";
            }
        }
    }

}