using System;
using UnityEngine;
using System.Collections;// Works on Steamdeck

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Scripts
    {
        public InputManager inputManager;
        public OptionsManager optionsManager;
        public UIManager uiManager;
        public AudioManager audioManager;
        public CameraManager cameraManager;
    }    
    public Scripts scripts;

    private Coroutine runtimeMonitorCoroutine;
    [Header("---- Garbage Collect ----------------------------------------------------")]
    [SerializeField] float lastGCTime = 0f;
    [SerializeField] private float gcInterval = 60f; // Run GC every 60 seconds
    [Header("---- Game States ----------------------------------------------------")]
    public bool inSplashScreen;
    public bool inMainMenu;
    [Space(10)]
    public bool inOptionsMenu;
    [Space(5)]
    public bool inOptionsGameplayMenu;    
    public bool inOptionsAudioMenu;
    public bool inOptionsKeyboardMenu;
    public bool inOptionsGamepadMenu;
    public bool inOptionsInterfaceMenu;
    public bool inOptionsAccessibilityMenu;
    public bool inOptionsVideoMenu;
    public bool inOptionsVideoMenuPage2;

    /*  Muscle(Strength)
        Awareness(Perception)
        Tolerance(Endurance)
        Resonance(Charisma)
        Input(Intelligence)
        X-Factor(Luck)  */

    void Update()
    {
        if (Time.realtimeSinceStartup - lastGCTime >= gcInterval)
        {
            Utils.ClearMemory();
            lastGCTime = Time.realtimeSinceStartup;
        }
        Options();
    }

    private void Options()
    {
        if (inOptionsMenu)// Detects additional monitors when in Options menu
        {
            if (runtimeMonitorCoroutine == null)
            {
                runtimeMonitorCoroutine = StartCoroutine(RuntimeMonitorDisplays());
            }
        }
        else
        {
            if (runtimeMonitorCoroutine != null)
            {
                StopCoroutine(runtimeMonitorCoroutine);
                runtimeMonitorCoroutine = null;
            }
        }

        if (scripts.uiManager.optionsUI.confirmOptionsChanges.activeSelf)// Options confirm changes.
        {
            scripts.optionsManager.requiresTimedConfirmationTimer -= Time.deltaTime;
            scripts.optionsManager.timeSpan = TimeSpan.FromSeconds(scripts.optionsManager.requiresTimedConfirmationTimer);
            scripts.uiManager.optionsUI.confirmOptionsChangesTimerText.text = string.Format("{0:D2} <color=" + Colors.textRed + "><b>:</b></color> {1:D2} <color=" + Colors.textRed + "><b>:</b></color> {2:D2}", scripts.optionsManager.timeSpan.Hours, scripts.optionsManager.timeSpan.Minutes, scripts.optionsManager.timeSpan.Seconds);

            if (scripts.optionsManager.requiresTimedConfirmationTimer <= 0f)
            {
                scripts.optionsManager.RevertChanges();
                scripts.optionsManager.requiresTimedConfirmation = false;
                Utils.ActivateObject(scripts.uiManager.optionsUI.confirmOptionsChanges, false);
            }
        }
        else if (scripts.optionsManager.requiresTimedConfirmationTimer != 25f)
        {
            scripts.optionsManager.requiresTimedConfirmationTimer = 25f;
        }

        if (scripts.optionsManager.appliedFPSCounter)
        {
            scripts.optionsManager.frameCount++;
            scripts.optionsManager.FPStimer += Time.unscaledDeltaTime;

            if (scripts.optionsManager.FPStimer >= scripts.optionsManager.FPSCounterUpdateInterval)
            {
                scripts.optionsManager.fps = scripts.optionsManager.frameCount / scripts.optionsManager.FPStimer;
                int targetFPS = (Application.targetFrameRate > 0) ? Application.targetFrameRate : int.MaxValue;// Cap FPS display to Application.targetFrameRate if it's set
                scripts.optionsManager.fps = Mathf.Min(scripts.optionsManager.fps, targetFPS);
                if (scripts.uiManager.optionsUI.fpsCounterText != null)
                {
                    int roundedFPS = Mathf.RoundToInt(scripts.optionsManager.fps);
                    scripts.uiManager.optionsUI.fpsCounterText.text = $"<color={Colors.textWhite}>FPS:</color> {roundedFPS}";


                    if (roundedFPS >= targetFPS * 0.85f)// Apply color based on performance thresholds
                    {
                        scripts.uiManager.optionsUI.fpsCounterText.color = Colors.green;
                    }
                    else if (roundedFPS >= targetFPS * 0.6f)
                    {
                        scripts.uiManager.optionsUI.fpsCounterText.color = Colors.yellow;
                    }
                    else
                    {
                        scripts.uiManager.optionsUI.fpsCounterText.color = Colors.red;
                    }
                }
                scripts.optionsManager.frameCount = 0;
                scripts.optionsManager.FPStimer = 0f;
            }
        }
        else
        {
            scripts.optionsManager.frameCount = 0;
            scripts.optionsManager.FPStimer = 0f;
        }

        if (scripts.optionsManager.appliedMemoryCounter)
        {
            scripts.optionsManager.memoryTimer += Time.unscaledDeltaTime;
            if (scripts.optionsManager.memoryTimer >= scripts.optionsManager.memoryCounterUpdateInterval)
            {
                scripts.optionsManager.totalMemory = System.GC.GetTotalMemory(false);

                scripts.optionsManager.memoryInMB = scripts.optionsManager.totalMemory / (1024f * 1024f);
                scripts.optionsManager.memoryInGB = scripts.optionsManager.memoryInMB / 1024f;

                if (scripts.optionsManager.memoryInMB >= 1024)
                {
                    scripts.uiManager.optionsUI.memoryCounterText.text = $"Memory: {scripts.optionsManager.memoryInGB:F1}GB";
                }
                else
                {
                    scripts.uiManager.optionsUI.memoryCounterText.text = $"Memory: {scripts.optionsManager.memoryInMB:F0}MB";
                }
                scripts.optionsManager.memoryTimer = 0f;
            }
        }
        else
        {
            scripts.optionsManager.memoryTimer = 0f;
        }
    }

    public void MainMenu()
    {
        inSplashScreen = false;
        inOptionsMenu = false;

        inMainMenu = true;
    }

    public void OptionsMenu()
    {
        inMainMenu = false;

        inOptionsMenu = true;
        scripts.uiManager.ClickedGameplayOptions();
    }

    public void QuitGame()
    {
        Utils.ClearMemory();
        Application.Quit();
    }

    IEnumerator RuntimeMonitorDisplays()
    {
        while (true)
        {
            yield return Times.two;
            //Debug.Log("Checking for new displays....");
            if (Display.displays.Length != scripts.optionsManager.lastDisplayCount)
            {
                //Debug.Log("Display configuration has changed!");
                scripts.optionsManager.lastDisplayCount = Display.displays.Length;
                scripts.optionsManager.DetectDisplays();
                // Ping notification
            }
        }
    }
}
