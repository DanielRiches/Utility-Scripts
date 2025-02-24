using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Scripts
    {
        public OptionsManager optionsManager;
        public UIManager uiManager;
        public AudioManager audioManager;
        public CameraManager cameraManager;
    }    
    public Scripts scripts;
    [Header("---- Garbage Collect ----------------------------------------------------")]
    [SerializeField] float lastGCTime = 0f;
    [SerializeField] private float gcInterval = 5f; // Run GC every 5 seconds
    [Header("---- Game States ----------------------------------------------------")]
    public bool splashScreen;
    public bool inMainMenu;
    public bool inOptionsMenu;

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
    }

    public void MainMenu()
    {
        splashScreen = false;
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
}
