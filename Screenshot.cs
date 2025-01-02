using UnityEngine;

public class Screenshot : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            ScreenCapture.CaptureScreenshot("D:\\Screenshot.png");
        }

    }
}
