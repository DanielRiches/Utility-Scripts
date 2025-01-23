using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject mainCamera;
    [Header("---- Scripts ------------------------------------------------------------")]
    [Space(5)]
    public PlayerInput playerInputs;
    public Options options;
    public UIManager uiManager;
    [Header("---- Game States --------------------------------------------------------")]
    [Space(5)]
    public bool skipScreen;
    public bool localizeLanguage;
    [Header("UI")]
    [Space(5)]
    public bool cursor;
    public bool inOptionsGameplayMenu;
    public bool inOptionsVideoMenu;
    public bool viewingNotification;
    public bool viewingCredits;
    public bool viewingPatchNotes;
    public bool viewingFPS;
    public bool viewingPing;
    public bool viewingMemory;
    [Header("Game")]
    [Space(5)]
    public bool blackScreen;
    public bool loadingGame;
    public bool viewingEpilepsyWarning;
    public bool viewingSaveWarning;
    public bool inSplashScreen = true;
    public bool loading;
    public bool inMainMenu;
    public bool inOptionsMenu;

    public bool usingKeyboard = true;
    public bool usingController;

    void Awake()
    {
        Utils.DontDestroyOnLoad(this.gameObject);
        Utils.DontDestroyOnLoad(mainCamera);
    }

    public void MainMenu()
    {
        inSplashScreen = false;
        inOptionsMenu = false;
        viewingCredits = false;
        viewingPatchNotes = false;
        inOptionsVideoMenu = false;
        inOptionsGameplayMenu = false;


        inMainMenu = true;
        viewingNotification = true;
    }

    public void OptionsMenu()
    {
        viewingEpilepsyWarning = false;
        viewingSaveWarning = false;
        inSplashScreen = false;
        inMainMenu = false;
        viewingCredits = false;
        viewingPatchNotes = false;
        inOptionsGameplayMenu = true;
        viewingNotification = false;

        inOptionsMenu = true;
    }

    public void OptionsMenuGameplay()
    {
        inOptionsVideoMenu = false;
        viewingNotification = false;

        inOptionsGameplayMenu = true;
    }

    public void OptionsMenuVideo()
    {
        inOptionsGameplayMenu = false;
        viewingNotification = false;

        inOptionsVideoMenu = true;
    }

    public void Credits()
    {
        viewingSaveWarning = false;
        viewingSaveWarning = false;
        inSplashScreen = false;
        inMainMenu = false;
        inOptionsMenu = false;
        viewingPatchNotes = false;
        viewingNotification = false;

        viewingCredits = true;
    }

    public void Notification()
    {
        viewingNotification = true;
    }

    public void PatchNotes()
    {
        viewingSaveWarning = false;
        viewingSaveWarning = false;
        inSplashScreen = false;
        inMainMenu = false;
        inOptionsMenu = false;
        viewingCredits = false;
        viewingNotification = false;

        viewingPatchNotes = true;
    }

    public void ViewFPS()
    {
        viewingFPS = true;
    }

    public void ViewPing()
    {
        viewingPing = true;
    }

    public void ViewMemory()
    {
        viewingMemory = true;
    }

    public void ApplyOptionsSettings()
    {
        options.applySettings = true;
    }

    public void Localize()
    {
        localizeLanguage = true;
    }

    public void ExitGame()
    {
        Utils.ClearMemory();
        Application.Quit();
    }

    public void LoadGame()
    {
        viewingEpilepsyWarning = true;
        loadingGame = true;
    }
}
