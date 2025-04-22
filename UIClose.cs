using UnityEngine;

public class UIClose : MonoBehaviour
{
    // This script is triggered at the end of animations via events in the Animation window.
    GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
    }
    public void CloseOptions()
    {
        gameManager.scripts.uiManager.CloseOptions();
    }
    public void ActivateOptionsImages()
    {
        gameManager.scripts.uiManager.ActivateOptionsImages();
    }
}
