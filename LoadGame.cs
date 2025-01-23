using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject gameLoader;
    [SerializeField] private GameObject bootCanvasObject;
    [SerializeField] private GameObject splashGameName;
    [SerializeField] private GameObject splashLanguageSelectObject;
    [SerializeField] private Image splashLanguageSelectedImage;
    [SerializeField] private GameObject epilepsyWarningObject;
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
        if (gameManager.inSplashScreen)
        {
            // CHECK FOR EXISTING GAME SETTINGS FILE TO SEE IF LANGUAGE WAS PREVIOUSLY SELECTED
            //  IF IT WAS APPLY THE LANGUAGE, APPLY ALL OTHER SETTINGS FOR OPTIONS THEN FINALLY CALL gameManager.LoadGame();
            // ELSE
            if (splashLanguageSelectObject && !gameManager.viewingEpilepsyWarning && !gameManager.viewingSaveWarning)
            {
                if (!splashLanguageSelectObject.activeSelf)
                {
                    if (Utils.GetPreferredSystemLanguage() == "en")
                    {
                        if (GameStrings.GameStringsEnglish.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }

                    }
                    else if (Utils.GetPreferredSystemLanguage() == "fr")
                    {
                        if (GameStrings.GameStringsFrench.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.frenchFlag;
                            gameManager.options.languageFrench = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "de")
                    {
                        if (GameStrings.GameStringsGerman.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.germanFlag;
                            gameManager.options.languageGerman = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "it")
                    {
                        if (GameStrings.GameStringsItalian.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.italianFlag;
                            gameManager.options.languageItalian = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "es")
                    {
                        if (GameStrings.GameStringsSpanish.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.spanishFlag;
                            gameManager.options.languageSpanish = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "en-US")
                    {
                        if (GameStrings.GameStringsAmerican.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.americanFlag;
                            gameManager.options.languageAmerican = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "ru")
                    {
                        if (GameStrings.GameStringsRussian.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.russianFlag;
                            gameManager.options.languageRussian = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "zh")
                    {
                        if (GameStrings.GameStringsChinese.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.chineseFlag;
                            gameManager.options.languageChinese = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "ja")
                    {
                        if (GameStrings.GameStringsJapanese.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.japaneseFlag;
                            gameManager.options.languageJapanese = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else if (Utils.GetPreferredSystemLanguage() == "pt")
                    {
                        if (GameStrings.GameStringsBrazilian.available)
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.brazilFlag;
                            gameManager.options.languageBrazilian = true;
                        }
                        else
                        {
                            splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                            gameManager.options.languageEnglish = true;
                        }
                    }
                    else
                    {
                        splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
                        Debug.Log("No compatible languages detected, defaulted to English.");
                    }

                    splashLanguageSelectObject.SetActive(true);                    
                }

                gameManager.cursor = true;
                // ONCE LANGUAGE SELECTED CREATE NEW GAME SETTINGS FILE AND SAVE ALL CURRENT DEFAULTS + SELECTED LANGUAGE
            }
        }

        if (gameManager.inSplashScreen && gameManager.loadingGame)
        {
            gameManager.cursor = false;
            gameManager.loadingGame = false;
            gameManager.viewingSaveWarning = false;
            splashLanguageSelectObject.SetActive(false);

            if (!splashGameName.activeSelf)
            {
                splashGameName.SetActive(true);
                gameManager.localizeLanguage = true;
            }
            
            StartCoroutine(LoadTheGame());              
        }

        if (gameManager.skipScreen)
        {
            StopCoroutine(LoadTheGame());
            if (epilepsyWarningObject)
            {
                UnityEngine.Object.Destroy(epilepsyWarningObject);
                gameManager.viewingEpilepsyWarning = false;
            }
            if (autosaveWarning)
            {
                UnityEngine.Object.Destroy(autosaveWarning);
                gameManager.viewingSaveWarning = false;
            }
            if (bootCanvasObject)
            {
                UnityEngine.Object.Destroy(bootCanvasObject);
            }

            gameManager.skipScreen = false;

            menuAnchor.position = mainMenuAnchor.position;
            if (mainMenuAnchor)
            {
                UnityEngine.Object.Destroy(mainMenuAnchor.gameObject);
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
            UnityEngine.Object.Destroy(splashGameName);
        }
        if (epilepsyWarningObject)
        {
            epilepsyWarningObject.SetActive(true);
            gameManager.viewingEpilepsyWarning = true;
            gameManager.localizeLanguage = true;                       
        }   
        
        yield return Times.seven;
        if (epilepsyWarningObject)
        {
            UnityEngine.Object.Destroy(epilepsyWarningObject);
            gameManager.viewingEpilepsyWarning = false;
        }
        canSkip = true;
        if (autosaveWarning)
        {
            autosaveWarning.SetActive(true);
            gameManager.viewingSaveWarning = true;
            gameManager.localizeLanguage = true;            
        }
        
        yield return Times.four;
        if (autosaveWarning)
        {
            UnityEngine.Object.Destroy(autosaveWarning);
            gameManager.viewingSaveWarning = false;
        }
        if (bootCanvasObject)
        {
            UnityEngine.Object.Destroy(bootCanvasObject);
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
            UnityEngine.Object.Destroy(mainMenuAnchor.gameObject);
        }

        Utils.ClearMemory();
        gameManager.MainMenu();
        gameManager.Localize();
        gameManager.Notification();
        gameManager.cursor = true;
        playerInput.KeyboardControls.Disable();
        playerInput.GamePadControls.Disable();
        if (gameLoader)
        {
            UnityEngine.Object.Destroy(gameLoader);
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

    public void ApplyEnglish()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.englishFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyFrench()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.frenchFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyGerman()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.germanFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyItaly()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.italianFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplySpanish()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.spanishFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyAfrican()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.americanFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyRussian()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.russianFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyChinese()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.chineseFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyJapanese()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.japaneseFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
    }
    public void ApplyBrazil()
    {
        splashLanguageSelectedImage.sprite = gameManager.uiManager.brazilFlag;
        gameManager.uiManager.splashLanguageDropdownObject.SetActive(false);
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
