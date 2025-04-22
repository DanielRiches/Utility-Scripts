using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public InputActionAsset playerInputActions;
    public InputActionMap keyboardActionMap;
    public InputActionMap gamepadActionMap;
    [Space(10)]
    public InputActionAsset uiInputActions;
    public InputSystemUIInputModule inputSystemUIInputModule;
    public InputActionMap uiActionMap;
    public Vector2 uiActionMapScrollDelta;
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
        uiInputActions = inputSystemUIInputModule.actionsAsset;
        uiActionMap = uiInputActions.FindActionMap("UI");

        keyboardActionMap.Enable();
        gamepadActionMap.Enable();
        uiActionMap.Enable();

        foreach (var action in keyboardActionMap.actions)
            action.performed += OnKeyboardInput;

        foreach (var action in gamepadActionMap.actions)
            action.performed += OnGamepadInput;

        foreach (var action in uiActionMap.actions)
            action.performed += OnUIInput;
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

    private void OnUIInput(InputAction.CallbackContext context)
    {
        uiActionMapScrollDelta = uiActionMap.FindAction(Strings.inputSystemUIInputModuleScrollWheel).ReadValue<Vector2>();
    }

    private void OnDisable()// Unsubscribe from events to prevent memory leaks
    {        
        foreach (var action in keyboardActionMap.actions)
            action.performed -= OnKeyboardInput;

        foreach (var action in gamepadActionMap.actions)
            action.performed -= OnGamepadInput;

        foreach (var action in uiActionMap.actions)
            action.performed -= OnGamepadInput;
    }
}
