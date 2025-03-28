using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public InputActionAsset playerInputActions;
    public InputActionMap keyboardActionMap;
    public InputActionMap gamepadActionMap;
    public InputActionMap uiActionMap;
    public bool keyboard;
    public bool gamepad;

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.inputManager = this;
    }

    void Start()
    {
        keyboardActionMap = playerInputActions.FindActionMap("KeyboardControls");
        gamepadActionMap = playerInputActions.FindActionMap("GamePadControls");
        uiActionMap = playerInputActions.FindActionMap("UI");
        gameManager.scripts.uiManager.scrollAction = gameManager.scripts.inputManager.uiActionMap.FindAction("Scroll");

        keyboardActionMap.Enable();
        gamepadActionMap.Enable();
        uiActionMap.Enable();

        foreach (var action in keyboardActionMap.actions)
            action.performed += OnKeyboardInput;

        foreach (var action in gamepadActionMap.actions)
            action.performed += OnGamepadInput;
    }

    private void OnKeyboardInput(InputAction.CallbackContext context)
    {
        keyboard = true;
        gamepad = false;
        // POPUP DEVICE CHANGED! MESSAGE
    }

    private void OnGamepadInput(InputAction.CallbackContext context)
    {
        keyboard = false;
        gamepad = true;
        // POPUP DEVICE CHANGED! MESSAGE
    }

    private void OnDisable()// Unsubscribe from events to prevent memory leaks, no need to bother with UI map as either of these will determine keyboard / gamepad anyways
    {        
        foreach (var action in keyboardActionMap.actions)
            action.performed -= OnKeyboardInput;

        foreach (var action in gamepadActionMap.actions)
            action.performed -= OnGamepadInput;
    }
}
