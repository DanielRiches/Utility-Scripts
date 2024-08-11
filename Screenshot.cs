using UnityEngine;

public class Screenshot : MonoBehaviour
{
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Left Click"))
        {
            ScreenCapture.CaptureScreenshot("D:\\Recordings\\Screenshot.png");
        }

    }
}
