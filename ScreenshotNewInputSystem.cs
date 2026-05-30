using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenshotNewInputSystem : MonoBehaviour
{
    [Header("Existing input asset")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Action name or path")]
    [SerializeField] private string submitActionName = "Submit";
    // If needed, use "UI/Submit" or "Player/Submit" instead.

    [Header("Screenshot")]
    [SerializeField] private string fileName = "Screenshot.png";

    private InputAction submitAction;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogError("No InputActionAsset assigned.");
            return;
        }

        submitAction = inputActions.FindAction(submitActionName, true);
        submitAction.Enable();
    }

    private void OnDisable()
    {
        submitAction?.Disable();
    }

    private void Update()
    {
        if (submitAction != null && submitAction.WasPressedThisFrame())
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log($"Screenshot saved to: {path}");
        }
    }
}
