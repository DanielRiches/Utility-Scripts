using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Profiling;

public class Options : MonoBehaviour
{    
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI fpsCounterTMP;
    [SerializeField] private TextMeshProUGUI pingCounterTMP;
    [SerializeField] private TextMeshProUGUI memoryCounterTMP;
    [SerializeField] private List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    [Header("---- Apply -----")]
    [Space(5)]
    [SerializeField] private bool applySettings;
    [Space(5)]
    private const string gameManagerTag = "GameManager";    
    private float fpsDisplay;
    private int lastFPS;
    private string format;
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
    private bool displayFPS;
    private bool displayPing;
    private bool displayMemory;
    [Header("---- Graphics ------------------------------------------------------------")]
    [Space(5)]
    [Tooltip("Unlimited FPS Cap")]
    [SerializeField] private bool unlimitedFPS = false;
    [Tooltip("Desired FPS Cap")]
    [SerializeField, Range(30, 280)] private int fps = 120;
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
    [SerializeField] private float updateInterval = 0.5F;
    [SerializeField] private bool fpsCounter;    
    [SerializeField] private bool pingCounter;    
    [SerializeField] private bool memoryCounter;
    

    private void Start()
    {
        gameManager = GameObject.FindWithTag(gameManagerTag).GetComponent<GameManager>();
        gameManager.options = this;
        timeleft = updateInterval;
        totalSystemMemory = SystemInfo.systemMemorySize;
        totalSystemMemoryGB = totalSystemMemory / 1024f;
    }

    void ApplySettings()
    {
        if (Application.isPlaying)
        {
            if (unlimitedFPS)
            {
                Application.targetFrameRate = -1; // Uncap the frame rate
            }
            else if (fps != lastFPS)
            {
                Application.targetFrameRate = fps;
                lastFPS = fps;
            }

            if (cameras.Count > 0)
            {
                foreach (CinemachineCamera camera in cameras) { camera.Lens.FieldOfView = fov; }
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

            System.GC.Collect();
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
                ++frames;
            }

            if (timeleft <= 0.0f)
            {
                fpsDisplay = accum / frames; // Display two fractional digits (F2 format)

                if (displayFPS)
                {
                    if (fpsDisplay < 40)
                    {
                        format = System.String.Format(" <color=white>FPS:</color> <color=red>{0:F2}</color>", fpsDisplay);
                        fpsCounterTMP.color = Color.red;
                    }
                    else if (fpsDisplay > 40 && fpsDisplay < 70)
                    {
                        format = System.String.Format(" <color=white>FPS:</color> <color=yellow>{0:F2}</color>", fpsDisplay);
                        fpsCounterTMP.color = Color.yellow;
                    }
                    else
                    {
                        format = System.String.Format(" <color=white>FPS:</color> <color=green>{0:F2}</color>", fpsDisplay);
                        fpsCounterTMP.color = Color.green;
                    }

                    fpsCounterTMP.text = format;
                }

                if (displayMemory)
                {
                    allocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
                    allocatedMemoryGB = allocatedMemory / (1024f * 1024f * 1024f);
                    memoryCounterTMP.color = Color.white;
                    memoryCounterTMP.text = $"Mem: {allocatedMemoryGB:F2} GB / {totalSystemMemoryGB:F2} GB";
                }
              
                timeleft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }

            if (displayPing)
            {
                /*
                if (ping >= 150)
                {
                    // Red
                }
                else if (ping <= 80)
                {
                    // Green
                }
                else
                {
                    // Yellow
                }
                */
            }
        }
    }
    public void PopulateCamera(CinemachineCamera camera)
    {
        cameras.Add(camera);
    }
}
