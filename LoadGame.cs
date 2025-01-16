using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject gameLoader;
    [SerializeField] private GameObject bootCanvasObject;
    [SerializeField] private GameObject splashGameName;
    [SerializeField] private GameObject epilepsyWarning;
    [SerializeField] private GameObject autosaveWarning;
    [SerializeField] private Transform menuAnchor;
    [SerializeField] private Transform mainMenuAnchor;
    private PlayerInput playerInput; // Reference Auto-Generated C# Action Map Script
    private AsyncOperation loadMenus;
    private AsyncOperation loadTheGame;
    private bool canSkip;
    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        playerInput = new PlayerInput();
        playerInput.KeyboardControls.AnyKey.started += onSkipSplash;
        playerInput.GamePadControls.Start.started += onSkipSplash;
    }

    
    void Update()
    {
        if (gameManager.loadingGame)
        {
            StartCoroutine(LoadTheGame());
            gameManager.loadingGame = false;
        }

        if (gameManager.skipScreen)
        {
            StopCoroutine(LoadTheGame());
            if (epilepsyWarning)
            {
                Object.Destroy(epilepsyWarning);
            }
            if (autosaveWarning)
            {
                Object.Destroy(autosaveWarning);
            }
            if (bootCanvasObject)
            {
                Object.Destroy(bootCanvasObject);
            }

            gameManager.skipScreen = false;

            menuAnchor.position = mainMenuAnchor.position;
            if (mainMenuAnchor)
            {
                Object.Destroy(mainMenuAnchor.gameObject);
            }

            gameManager.MainMenu();
            Destroy(this);
        }
    }

    public IEnumerator LoadTheGame()
    {
        yield return Times.two;

        loadMenus = SceneManager.LoadSceneAsync(Strings.menuSceneName, LoadSceneMode.Additive);
        loadTheGame = SceneManager.LoadSceneAsync(Strings.loadingSceneName, LoadSceneMode.Additive);

        while (!loadMenus.isDone || !loadTheGame.isDone)
        {
            gameManager.uiManager.combinedProgress = (loadMenus.progress + loadTheGame.progress) / 2f;

            // Since progress goes from 0 to 0.9, we scale it to 0 to 1 and ensure it doesn't exceed 0.9
            gameManager.uiManager.combinedProgress = Mathf.Min(gameManager.uiManager.combinedProgress, 0.9f);
            gameManager.uiManager.percentageCompleted = Mathf.FloorToInt((gameManager.uiManager.combinedProgress / 0.9f) * 100f);

            yield return null;
        }

        if (splashGameName)
        {
            Object.Destroy(splashGameName);
        }
        if (epilepsyWarning)
        {
            epilepsyWarning.SetActive(true);
        }
        yield return Times.seven;
        if (epilepsyWarning)
        {
            Object.Destroy(epilepsyWarning);
        }
        canSkip = true;
        if (autosaveWarning)
        {
            autosaveWarning.SetActive(true);
        }
        yield return Times.four;
        if (autosaveWarning)
        {
            Object.Destroy(autosaveWarning);
        }
        if (bootCanvasObject)
        {
            Object.Destroy(bootCanvasObject);
        }

        while (Vector3.Distance(menuAnchor.position, mainMenuAnchor.position) > 0.05f)// Lerp the position
        {
            menuAnchor.position = Vector3.Lerp(menuAnchor.position, mainMenuAnchor.position, 2 * Time.deltaTime);

            if (Vector3.Distance(menuAnchor.position, mainMenuAnchor.position) <= 0.05f)
            {
                menuAnchor.position = mainMenuAnchor.position;
            }

            yield return null;
        }
        menuAnchor.position = mainMenuAnchor.position;

        if (mainMenuAnchor)
        {
            Object.Destroy(mainMenuAnchor.gameObject);
        }

        Utils.ClearMemory();
        gameManager.MainMenu();
        playerInput.KeyboardControls.Disable();
        playerInput.GamePadControls.Disable();
        if (gameLoader)
        {
            Object.Destroy(gameLoader);
        }

        yield return null;
    }

    void onSkipSplash(InputAction.CallbackContext started)
    {     
        if (canSkip)
        {            
            gameManager.skipScreen = true;
        }
    }

    void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.KeyboardControls.Enable(); // When the script is enabled, enable Action Maps
            playerInput.GamePadControls.Enable();
        }
    }
    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.KeyboardControls.Disable(); // When the script is disabled, disable Action Maps
            playerInput.GamePadControls.Disable();
        }
    }
}
