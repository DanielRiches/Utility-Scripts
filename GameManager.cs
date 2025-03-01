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
    public bool inOptionsVideoMenu2;

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
