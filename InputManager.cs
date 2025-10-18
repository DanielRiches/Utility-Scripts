using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;
    public InputActionAsset playerInputActions;

    [System.Serializable]
    public class Devices
    {
        public int totalDevicesCount;
        public string[] devices;
        public Keyboard[] keyboards;
        public Mouse[] mice;
        public Gamepad[] controllers;
    }
    public Devices devices = new Devices();

    [System.Serializable]
    public class KeyboardInput
    {
        public bool usingPlayer1Keyboard;
        public Keyboard player1Keyboard;
        public InputActionMap player1KeyboardActionMap;

        [Space(10)]
        public bool usingPlayer2Keyboard;
        public Keyboard player2Keyboard;
        public InputActionMap player2KeyboardActionMap;
    }
    public KeyboardInput keyboardInput = new KeyboardInput();

    [System.Serializable]
    public class MouseInput
    {
        public bool usingPlayer1Mouse;
        public Mouse player1Mouse;
        public InputActionMap player1MouseActionMap;
        public Vector2 player1CursorPosition;
        public float player1MouseScrollDelta;
        public bool player1ScrolledUp;
        public bool player1ScrolledDown;

        [Space(10)]
        public bool usingPlayer2Mouse;
        public Mouse player2Mouse;
        public InputActionMap player2MouseActionMap;
        public Vector2 player2CursorPosition;
        public float player2MouseScrollDelta;
        public bool player2ScrolledUp;
        public bool player2ScrolledDown;

        public void UpdateState()
        {
            if (usingPlayer1Mouse && player1Mouse != null)
            {
                player1CursorPosition = player1Mouse.position.ReadValue();
                player1MouseScrollDelta = player1Mouse.scroll.ReadValue().y;
                player1ScrolledUp = player1MouseScrollDelta > 0;
                player1ScrolledDown = player1MouseScrollDelta < 0;
            }

            if (usingPlayer2Mouse && player2Mouse != null)
            {
                player2CursorPosition = player2Mouse.position.ReadValue();
                player2MouseScrollDelta = player2Mouse.scroll.ReadValue().y;
                player2ScrolledUp = player2MouseScrollDelta > 0;
                player2ScrolledDown = player2MouseScrollDelta < 0;
            }
        }
    }
    public MouseInput mouseInput = new MouseInput();

    [System.Serializable]
    public class GamepadInput
    {
        public bool usingPlayer1Gamepad;
        public Gamepad player1Gamepad;
        public InputActionMap player1GamepadActionMap;

        [Space(10)]
        public bool usingPlayer2Gamepad;
        public Gamepad player2Gamepad;
        public InputActionMap player2GamepadActionMap;
    }
    public GamepadInput gamepadInput = new GamepadInput();

    private InputUser player1User;
    private InputUser player2User;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.inputManager = this;

        RefreshDeviceList();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void Start()
    {
        keyboardInput.player1KeyboardActionMap = playerInputActions.FindActionMap(Strings.keyboardActionMap);
        gamepadInput.player1GamepadActionMap = playerInputActions.FindActionMap(Strings.gamepadActionMap);
        mouseInput.player1MouseActionMap = playerInputActions.FindActionMap(Strings.mouseActionMap);

        // Clone for player 2
        keyboardInput.player2KeyboardActionMap = CloneActionMap(keyboardInput.player1KeyboardActionMap, "_P2");
        gamepadInput.player2GamepadActionMap = CloneActionMap(gamepadInput.player1GamepadActionMap, "_P2");
        mouseInput.player2MouseActionMap = CloneActionMap(mouseInput.player1MouseActionMap, "_P2");

        BindDevices();
    }

    private InputActionMap CloneActionMap(InputActionMap source, string suffix)
    {
        var newMap = new InputActionMap(source.name + suffix);
        foreach (var action in source.actions)
        {
            var newAction = newMap.AddAction(action.name, action.type);
            foreach (var binding in action.bindings)
                newAction.AddBinding(binding);
        }
        return newMap;
    }

    private void Update()
    {
        mouseInput.UpdateState();
        /*
        if (mouseInput.usingPlayer1Mouse)
            Debug.Log($"[DEBUG] P1 Mouse Position: {mouseInput.player1CursorPosition}");
        if (mouseInput.usingPlayer2Mouse)
            Debug.Log($"[DEBUG] P2 Mouse Position: {mouseInput.player2CursorPosition}");
        */
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            RefreshDeviceList();
            BindDevices();
        }
    }

    private void RefreshDeviceList()
    {
        devices.devices = InputSystem.devices.Select(d => d.displayName).ToArray();
        devices.totalDevicesCount = InputSystem.devices.Count;
        devices.keyboards = InputSystem.devices.OfType<Keyboard>().ToArray();
        devices.mice = InputSystem.devices.OfType<Mouse>().ToArray();
        devices.controllers = InputSystem.devices.OfType<Gamepad>().ToArray();
    }

    private void BindDevices()
    {
        if (player1User.valid) player1User.UnpairDevices();
        if (player2User.valid) player2User.UnpairDevices();

        keyboardInput.usingPlayer1Keyboard = false;
        keyboardInput.usingPlayer2Keyboard = false;
        mouseInput.usingPlayer1Mouse = false;
        mouseInput.usingPlayer2Mouse = false;
        gamepadInput.usingPlayer1Gamepad = false;
        gamepadInput.usingPlayer2Gamepad = false;

        // ===== Player 1 =====
        if (devices.keyboards.Length > 0)
        {
            keyboardInput.usingPlayer1Keyboard = true;
            keyboardInput.player1Keyboard = devices.keyboards[0];
            player1User = InputUser.PerformPairingWithDevice(devices.keyboards[0]);
            player1User.AssociateActionsWithUser(keyboardInput.player1KeyboardActionMap);
        }

        if (devices.mice.Length > 0)
        {
            mouseInput.usingPlayer1Mouse = true;
            mouseInput.player1Mouse = devices.mice[0];
            player1User = InputUser.PerformPairingWithDevice(devices.mice[0], player1User);
            player1User.AssociateActionsWithUser(mouseInput.player1MouseActionMap);
            mouseInput.player1MouseActionMap.Enable();
        }

        if (devices.controllers.Length > 0)
        {
            gamepadInput.usingPlayer1Gamepad = true;
            gamepadInput.player1Gamepad = devices.controllers[0];
            player1User = InputUser.PerformPairingWithDevice(devices.controllers[0]);
            player1User.AssociateActionsWithUser(gamepadInput.player1GamepadActionMap);
        }

        // ===== Player 2 =====
        if (devices.keyboards.Length > 1)
        {
            keyboardInput.usingPlayer2Keyboard = true;
            keyboardInput.player2Keyboard = devices.keyboards[1];
            player2User = InputUser.PerformPairingWithDevice(devices.keyboards[1]);
            player2User.AssociateActionsWithUser(keyboardInput.player2KeyboardActionMap);
        }

        if (devices.mice.Length > 1)
        {
            mouseInput.usingPlayer2Mouse = true;
            mouseInput.player2Mouse = devices.mice[1];
            player2User = InputUser.PerformPairingWithDevice(devices.mice[1], player2User);
            player2User.AssociateActionsWithUser(mouseInput.player2MouseActionMap);
            mouseInput.player2MouseActionMap.Enable();
        }

        if (devices.controllers.Length > 1)
        {
            gamepadInput.usingPlayer2Gamepad = true;
            gamepadInput.player2Gamepad = devices.controllers[1];
            player2User = InputUser.PerformPairingWithDevice(devices.controllers[1]);
            player2User.AssociateActionsWithUser(gamepadInput.player2GamepadActionMap);
        }
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}
