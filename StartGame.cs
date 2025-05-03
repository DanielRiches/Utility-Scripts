using UnityEngine;
using static AudioManager;
using static GameManager;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private float startTimer;
    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
    }    

    void Update()
    {
        if (gameManager.inSplashScreen)
        {
            startTimer += Time.deltaTime;
        }        

        if (startTimer >= 5f)
        {
            if (gameManager.scripts.uiManager.blackscreen.activeSelf)
            {
                Utils.ActivateObject(gameManager.scripts.uiManager.blackscreen, false);
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.gameTitle, true);
                startTimer = 0;
            }
            else if (gameManager.scripts.uiManager.splashUI.gameTitle.activeSelf)
            {
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.gameTitle, false);
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.epilepsyWarning, true);
                startTimer = 0;
            }
            else if (gameManager.scripts.uiManager.splashUI.epilepsyWarning.activeSelf)
            {
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.epilepsyWarning, false);
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.saveWarning, true);
                startTimer = 0;
            }
            else if (gameManager.scripts.uiManager.splashUI.saveWarning.activeSelf)
            {
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.saveWarning, false);
                Utils.ActivateObject(gameManager.scripts.uiManager.blackscreen, true);
                // LOAD MAIN MENU SCENE HERE
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.pressAnyKey, true);               
                Utils.ActivateObject(gameManager.scripts.uiManager.blackscreen, false);
                startTimer = 0;
                gameManager.inSplashScreen = false;
                gameManager.scripts.audioManager.music.musicLayerSource.Play();
            }
        }

        if (gameManager.scripts.uiManager.splashUI.pressAnyKey.activeSelf)
        {
            if (gameManager.scripts.inputManager.keyboardActionMap[Strings.menuKeyboardStartGame].WasPressedThisFrame() ||
                gameManager.scripts.inputManager.gamepadActionMap[Strings.menuGamepadStartGame].WasPressedThisFrame())
            {
                gameManager.scripts.audioManager.music.musicLayerSource.mute = true;
                gameManager.scripts.audioManager.music.musicAudioSource.mute = false;
                gameManager.scripts.audioManager.music.musicLayerSource.Stop();
                gameManager.scripts.audioManager.music.musicAudioSource.Play();

                //Debug.Log("Input detected on splash screen!");
                gameManager.inSplashScreen = false;
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.pressAnyKey, false);
                Utils.ActivateObject(gameManager.scripts.uiManager.splashUI.splashMenu, false);
                Utils.ActivateObject(gameManager.scripts.uiManager.mainMenu, true);
            }
        }
    }
}
