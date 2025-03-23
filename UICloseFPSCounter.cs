using UnityEngine;

public class UICloseFPSCounter : MonoBehaviour
{
    // This script is triggered at the end of animations via events in the Animation window.
    GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
    }
    public void CloseFPSCounter()
    {
        gameManager.scripts.uiManager.CloseFPSCounter();
    }
}
