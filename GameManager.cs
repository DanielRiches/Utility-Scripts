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
    [Space(5)]
    public bool loadingGame = true;
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

    void Update()
    {

    }

    public void MainMenu()
    {
        inOptionsMenu = false;

        inMainMenu = true;        
    }

    public void OptionsMenu()
    {
        inMainMenu = false;

        inOptionsMenu = true;        
    }
    public void ExitGame()
    {
        Utils.ClearMemory();        
        Application.Quit();
    }
}
