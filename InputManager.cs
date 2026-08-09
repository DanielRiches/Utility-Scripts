using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using System.Collections;
using static InputManager;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

public class InputManager : MonoBehaviour
{
    public bool debug;

    // ------- change these depending on your map / action names
    // MAPS
    public const string keyboardControls = "KeyboardControls";
    public const string mouseControls = "MouseControls";
    public const string gamepadControls = "GamePadControls";
    public const string uiControls = "UIControls";
    // ACTIONS
    public const string any = "Any";
    public const string moveForward = "Forward";
    public const string moveBack = "Back";
    public const string moveLeft = "Left";
    public const string moveRight = "Right";
    private const string jump = "Jump";
    private const string interact = "Interact";
    private const string attack = "Attack";
    // MOUSE
    private const string leftClick = "LeftClick";
    private const string rightClick = "rightClick";
    private const string scrollWheelClick = "ScrollWheelClick";
    // UI
    private const string kbUIUp = "KbUIUp";
    private const string kbUIRight = "KbUIRight";
    private const string kbUILeft = "KbUILeft";
    private const string kbUIDown = "KbUIDown";
    private const string kbUISubmit = "KbUISubmit";
    private const string kbUICancel = "KbUICancel";

    private const string gpUIUp = "GpUIUp";
    private const string gpUIRight = "GpUIRight";
    private const string gpUILeft = "GpUILeft";
    private const string gpUIDown = "GpUIDown";
    private const string gpUISubmit = "GpUISubmit";
    private const string gpUICancel = "GpUICancel";
    // ---------------------------------------------------------

    public const string none = "None"; // GUI


    [System.Serializable]
    public class ActionAsset
    {
        [Header("Input Asset")]
        [Tooltip("Reference your Input Actions Asset here")]
        public InputActionAsset playerInputActions;

        [Header("Player Script Reference")]
#if UNITY_EDITOR
        [Tooltip("Reference your player script from your project window here")]
        public MonoScript playerScriptReference; // Editor-only
#endif

        [Tooltip("Select your Player Layer here")]
        public LayerMask playerLayer;

        [Tooltip("Select your Player Tag here")]
        [SerializeField] public string playerTag = "Untagged"; // will be drawn as a dropdown in the editor

        [SerializeField] private string playerScriptTypeName; // stores the type string for runtime

        /// <summary>
        /// Returns the System.Type of the player script.
        /// </summary>
        public System.Type PlayerScriptType
        {
            get
            {
#if UNITY_EDITOR
                if (playerScriptReference != null) return playerScriptReference.GetClass();
#endif
                if (!string.IsNullOrEmpty(playerScriptTypeName))
                    return System.Type.GetType(playerScriptTypeName);
                return null;
            }
        }

#if UNITY_EDITOR
        // Editor-only: call this in OnValidate or manually in inspector to store type string
        public void UpdateScriptTypeName()
        {
            if (playerScriptReference != null) playerScriptTypeName = playerScriptReference.GetClass().AssemblyQualifiedName;
        }
#endif

        /// <summary>
        /// Finds all objects on the playerLayer and assigns the referenced script to the corresponding PlayerInput,
        /// optionally filtering by a specific tag.
        /// </summary>
        public void GetPlayerScripts(InputManager manager)
        {
            var scriptType = PlayerScriptType;
            if (scriptType == null || !typeof(MonoBehaviour).IsAssignableFrom(scriptType))
            {
                if (GameManager.Instance.inputManager.debug) Utils.Log("[InputManager] Player script type is not assigned or invalid.", 0);
                return;
            }

            // Find all objects in the scene
            GameObject[] playerObjects = GameObject.FindObjectsByType<GameObject>();

            // Get layer mask value
            int layerMaskValue = playerLayer.value;

            foreach (var go in playerObjects)
            {
                // Skip objects not in the selected layer
                if ((layerMaskValue & (1 << go.layer)) == 0) continue;

                // --- TAG FILTER ---
                if (!string.IsNullOrEmpty(playerTag) && playerTag != "Untagged" && go.tag != playerTag) continue;

                // Get the target script
                var scriptInstance = go.GetComponent(scriptType);
                if (scriptInstance == null) continue;

                // Try to read playerNumber field
                var playerNumberField = scriptType.GetField("playerNumber");
                if (playerNumberField == null)
                {
                    if (GameManager.Instance.inputManager.debug) Utils.Log($"[InputManager] Script {scriptType.Name} must have a public int playerNumber field.", 0);
                    continue;
                }

                int playerNumber = (int)playerNumberField.GetValue(scriptInstance);

                if (playerNumber < 1 || playerNumber > manager.players.Count)
                {
                    if (GameManager.Instance.inputManager.debug) Utils.Log($"[InputManager] Invalid playerNumber {playerNumber} on object {go.name}", 0);
                    continue;
                }

                // Assign the script to the corresponding PlayerInput
                var playerInput = manager.players[playerNumber - 1];
                playerInput.assignedPlayerScript = scriptInstance as MonoBehaviour;
            }
        }
    }

    public ActionAsset playerInputActionAsset = new ActionAsset();

    [System.Serializable]
    public class DeviceCollection
    {
        public List<string> connectedDeviceNames = new List<string>();
        [System.NonSerialized] public Keyboard[] keyboards;
        [System.NonSerialized] public Mouse[] mice;
        [System.NonSerialized] public Gamepad[] gamepads;
    }
    public DeviceCollection devices = new DeviceCollection();

    [Space(10)]
    [Range(1, 4)] public int totalPlayers;

    public enum DeviceType
    {
        Keyboard = 0,
        Mouse = 1,
        Gamepad = 2
    }

    [System.Serializable]
    public class PlayerInput
    {
        public int playerNumber;

        [Header("Assigned Devices")]
        [System.NonSerialized] public Keyboard keyboard;
        [System.NonSerialized] public Mouse mouse;
        [System.NonSerialized] public Gamepad gamepad;

        [Header("Bound Devices")]
        public string keyboardName;
        public string mouseName;
        public string gamepadName;

        [Header("Current Input Values")]
        public Vector2 keyboardMove;
        public Vector2 mousePosition;
        public Vector2 mouseDelta;
        public float scrollDelta;
        Vector2 lastMouseDelta;
        public Vector2 gamepadMove;
        public Vector2 gamepadLook;
        public Vector2 gamepadUIMove;

        [Header("Active Input Flags")]
        public bool usingKeyboard;
        public bool usingGamePad;
        public bool usingUI;

        [Header("Assigned Player Script")]
        [SerializeField]
        internal MonoBehaviour assignedPlayerScript; // assignable internally
        public MonoBehaviour AssignedPlayerScript => assignedPlayerScript; // read-only externally
        [System.NonSerialized] [HideInInspector] public InputUser user;

        // KEYBOARD ----------------------------------------------------
        [HideInInspector] public InputActionMap keyboardActionMap;
        [HideInInspector] public InputAction keyboardAnyAction;
        public bool keyboardAnyPressed { get; private set; }
        public bool keyboardAnyPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction jumpAction;
        public bool jumpPressed { get; private set; }
        public bool jumpPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction interactAction;
        public bool interactPressed { get; private set; }
        public bool interactPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction attackAction;
        public bool attackPressed { get; private set; }
        public bool attackPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction grenadeAction;
        public bool grenadePressed { get; private set; }
        public bool grenadePressedThisFrame { get; private set; }

        // MOUSE --------------------------------------------------------
        [HideInInspector] public InputActionMap mouseActionMap;
        [HideInInspector] public InputAction mouseAnyAction;
        public bool mouseAnyPressed { get; private set; }
        public bool mouseAnyPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction leftClickAction;
        public bool leftClickPressed { get; private set; }
        public bool leftClickPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction rightClickAction;
        public bool rightClickPressed { get; private set; }
        public bool rightClickPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction scrollWheelClickAction;
        public bool scrollWheelClickPressed { get; private set; }
        public bool scrollWheelClickPressedThisFrame { get; private set; }

        // GAMEPAD ------------------------------------------------------
        [HideInInspector] public InputActionMap gamepadActionMap;
        [HideInInspector] public InputAction gamepadAnyAction;
        public bool gamepadAnyPressed { get; private set; }
        public bool gamepadAnyPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gamepadJumpAction;
        public bool gamepadJumpPressed { get; private set; }
        public bool gamepadJumpPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gamepadInteractAction;
        public bool gamepadInteractPressed { get; private set; }
        public bool gamepadInteractPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gamepadAimAction;
        public bool gamepadAimPressed { get; private set; }
        public bool gamepadAimPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gamepadAttackAction;
        public bool gamepadAttackPressed { get; private set; }
        public bool gamepadAttackPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gamepadGrenadeAction;
        public bool gamepadGrenadePressed { get; private set; }
        public bool gamepadGrenadePressedThisFrame { get; private set; }

        // UI (Keyboard + Gamepad) -------------------------------------
        [HideInInspector] public InputActionMap uiActionMap;

        // Keyboard
        [HideInInspector] public InputAction kbUIUpAction;
        public bool kbUIUpPressed { get; private set; }
        public bool kbUIUpPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction kbUIRightAction;
        public bool kbUIRightPressed { get; private set; }
        public bool kbUIRightPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction kbUILeftAction;
        public bool kbUILeftPressed { get; private set; }
        public bool kbUILeftPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction kbUIDownAction;
        public bool kbUIDownPressed { get; private set; }
        public bool kbUIDownPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction kbUISubmitAction;
        public bool kbUISubmitPressed { get; private set; }
        public bool kbUISubmitPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction kbUICancelAction;
        public bool kbUICancelPressed { get; private set; }
        public bool kbUICancelPressedThisFrame { get; private set; }

        // Gamepad
        [HideInInspector] public InputAction gpUIUpAction;
        public bool gpUIUpPressed { get; private set; }
        public bool gpUIUpPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gpUIRightAction;
        public bool gpUIRightPressed { get; private set; }
        public bool gpUIRightPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gpUILeftAction;
        public bool gpUILeftPressed { get; private set; }
        public bool gpUILeftPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gpUIDownAction;
        public bool gpUIDownPressed { get; private set; }
        public bool gpUIDownPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gpUISubmitAction;
        public bool gpUISubmitPressed { get; private set; }
        public bool gpUISubmitPressedThisFrame { get; private set; }
        [HideInInspector] public InputAction gpUICancelAction;
        public bool gpUICancelPressed { get; private set; }
        public bool gpUICancelPressedThisFrame { get; private set; }

        // ------------------------
        // UPDATE INPUT
        // ------------------------
        public void UpdateInput()
        {
            // ------------------------
            // READ RAW INPUT VALUES
            // ------------------------
            // Keyboard movement
            if (keyboardActionMap != null && keyboardActionMap.enabled)
            {
                float forward = keyboardActionMap.FindAction(InputManager.moveForward)?.ReadValue<float>() ?? 0f;
                float back = keyboardActionMap.FindAction(InputManager.moveBack)?.ReadValue<float>() ?? 0f;
                float left = keyboardActionMap.FindAction(InputManager.moveLeft)?.ReadValue<float>() ?? 0f;
                float right = keyboardActionMap.FindAction(InputManager.moveRight)?.ReadValue<float>() ?? 0f;
                keyboardMove = new Vector2(right - left, forward - back);
            }
            else keyboardMove = Vector2.zero;

            // Mouse (absolute position only for gameplay/UI, NOT detection)
            if (mouse != null)
            {
                mousePosition = mouse.position.ReadValue();
                mouseDelta = mouse.delta.ReadValue();
            }               
                
            // Gamepad
            if (gamepad != null && !usingUI)
            {
                gamepadMove = gamepad.leftStick.ReadValue();
                gamepadLook = gamepad.rightStick.ReadValue();
            }
            else
            {
                gamepadMove = Vector2.zero;
                gamepadLook = Vector2.zero;
            }

            // UI
            if (gamepad != null && uiActionMap != null && uiActionMap.enabled && usingUI) gamepadUIMove = gamepad.leftStick.ReadValue();
            else gamepadUIMove = Vector2.zero;

            // ------------------------
            // DETECT DEVICE USAGE
            // ------------------------
            bool gamepadUsedThisFrame = false;
            bool keyboardUsedThisFrame = false;
            bool mouseUsedThisFrame = false;

            // --- GAMEPAD ---
            if (gamepad != null && gamepadActionMap != null && gamepadActionMap.enabled)
            {
                if (gamepadMove.sqrMagnitude > 0.0004f || gamepadLook.sqrMagnitude > 0.0004f || gamepadAnyAction?.WasPressedThisFrame() == true) gamepadUsedThisFrame = true;
            }

            // --- KEYBOARD ---
            if (keyboardActionMap != null && keyboardActionMap.enabled)
            {
                if (keyboardMove.sqrMagnitude > 0.0004f || keyboardAnyAction?.WasPressedThisFrame() == true) keyboardUsedThisFrame = true;
            }

            // --- MOUSE ---
            if (mouse != null && mouseActionMap != null && mouseActionMap.enabled)
            {
                Vector2 mouseDelta = mouse.delta.ReadValue();
                if (mouseDelta.sqrMagnitude > 0.01f || leftClickAction?.WasPressedThisFrame() == true || rightClickAction?.WasPressedThisFrame() == true) mouseUsedThisFrame = true;

                Vector2 scroll = mouse.scroll.ReadValue();
                if (Mathf.Abs(scroll.y) > 0.01f) scrollDelta = scroll.y;
                else scrollDelta = 0f;
            }

            bool keyboardMouseUsedThisFrame = keyboardUsedThisFrame || mouseUsedThisFrame;

            // ------------------------
            // APPLY DEVICE SWITCHING
            // ------------------------

            // Switch TO gamepad only on explicit gamepad input
            if (gamepadUsedThisFrame && !usingGamePad)
            {
                usingGamePad = true;
                usingKeyboard = false;

                if (assignedPlayerScript)// SEND INPUT CHANGE UI NOTIFICATION
                {
                    if (assignedPlayerScript is Player player)
                    {
                        //if (!player.playerStates.inSplashScreen) player.playerObject.playerUI.InputChangeNotification();
                    }
                }                
            }
            // Switch TO keyboard/mouse only on explicit keyboard or mouse input
            else if (keyboardMouseUsedThisFrame && !usingKeyboard)
            {
                usingKeyboard = true;
                usingGamePad = false;
                if (assignedPlayerScript)// SEND INPUT CHANGE UI NOTIFICATION
                {
                    if (assignedPlayerScript is Player player)
                    {
                        //if (!player.playerStates.inSplashScreen) player.playerObject.playerUI.InputChangeNotification();
                    }
                }
            }
        }

        public void UpdateActionStates()
        {
            bool kbActive = usingKeyboard;
            bool gpActive = usingGamePad;
            bool uiActive = usingUI;

            // ------------------------
            // KEYBOARD ACTIONS
            // ------------------------
            if (keyboardActionMap != null && kbActive)
            {
                keyboardAnyPressed = keyboardAnyAction?.IsPressed() ?? false;
                keyboardAnyPressedThisFrame = keyboardAnyAction?.WasPressedThisFrame() ?? false;

                jumpPressed = jumpAction?.IsPressed() ?? false;
                jumpPressedThisFrame = jumpAction?.WasPressedThisFrame() ?? false;

                attackPressed = attackAction?.IsPressed() ?? false;
                attackPressedThisFrame = attackAction?.WasPressedThisFrame() ?? false;

                interactPressed = interactAction?.IsPressed() ?? false;
                interactPressedThisFrame = interactAction?.WasPressedThisFrame() ?? false;
            }
            else
            {
                keyboardAnyPressed = false;
                keyboardAnyPressedThisFrame = false;
                jumpPressed = false;
                jumpPressedThisFrame = false;
                attackPressed = false;
                attackPressedThisFrame = false;
                interactPressed = false;
                interactPressedThisFrame = false;
            }

            // ------------------------
            // MOUSE ACTIONS
            // ------------------------
            if (mouseActionMap != null && kbActive)
            {
                mouseAnyPressed = mouseAnyAction?.IsPressed() ?? false;
                mouseAnyPressedThisFrame = mouseAnyAction?.WasPressedThisFrame() ?? false;

                leftClickPressed = leftClickAction?.IsPressed() ?? false;
                leftClickPressedThisFrame = leftClickAction?.WasPressedThisFrame() ?? false;

                rightClickPressed = rightClickAction?.IsPressed() ?? false;
                rightClickPressedThisFrame = rightClickAction?.WasPressedThisFrame() ?? false;

                scrollWheelClickPressed = scrollWheelClickAction?.IsPressed() ?? false;
                scrollWheelClickPressedThisFrame = scrollWheelClickAction?.WasPressedThisFrame() ?? false;
            }
            else
            {
                mouseAnyPressed = false;
                mouseAnyPressedThisFrame = false;
                leftClickPressed = false;
                leftClickPressedThisFrame = false;
                rightClickPressed = false;
                rightClickPressedThisFrame = false;
                scrollWheelClickPressed = false;
                scrollWheelClickPressedThisFrame = false;
            }

            // ------------------------
            // GAMEPAD ACTIONS
            // ------------------------
            if (gamepadActionMap != null && gpActive)
            {
                gamepadAnyPressed = gamepadAnyAction?.IsPressed() ?? false;
                gamepadAnyPressedThisFrame = gamepadAnyAction?.WasPressedThisFrame() ?? false;

                gamepadJumpPressed = gamepadJumpAction?.IsPressed() ?? false;
                gamepadJumpPressedThisFrame = gamepadJumpAction?.WasPressedThisFrame() ?? false;

                gamepadAttackPressed = gamepadAttackAction?.IsPressed() ?? false;
                gamepadAttackPressedThisFrame = gamepadAttackAction?.WasPressedThisFrame() ?? false;

                gamepadInteractPressed = gamepadInteractAction?.IsPressed() ?? false;
                gamepadInteractPressedThisFrame = gamepadInteractAction?.WasPressedThisFrame() ?? false;
            }
            else
            {
                gamepadAnyPressed = false;
                gamepadAnyPressedThisFrame = false;
                gamepadJumpPressed = false;
                gamepadJumpPressedThisFrame = false;
                gamepadAttackPressed = false;
                gamepadAttackPressedThisFrame = false;
                gamepadInteractPressed = false;
                gamepadInteractPressedThisFrame = false;
            }

            // ------------------------
            // UI ACTIONS
            // ------------------------
            if (uiActionMap != null && uiActive)
            {
                // Keyboard UI
                kbUIUpPressed = kbUIUpAction?.IsPressed() ?? false;
                kbUIUpPressedThisFrame = kbUIUpAction?.WasPressedThisFrame() ?? false;

                kbUIRightPressed = kbUIRightAction?.IsPressed() ?? false;
                kbUIRightPressedThisFrame = kbUIRightAction?.WasPressedThisFrame() ?? false;

                kbUILeftPressed = kbUILeftAction?.IsPressed() ?? false;
                kbUILeftPressedThisFrame = kbUILeftAction?.WasPressedThisFrame() ?? false;

                kbUIDownPressed = kbUIDownAction?.IsPressed() ?? false;
                kbUIDownPressedThisFrame = kbUIDownAction?.WasPressedThisFrame() ?? false;

                kbUISubmitPressed = kbUISubmitAction?.IsPressed() ?? false;
                kbUISubmitPressedThisFrame = kbUISubmitAction?.WasPressedThisFrame() ?? false;

                kbUICancelPressed = kbUICancelAction?.IsPressed() ?? false;
                kbUICancelPressedThisFrame = kbUICancelAction?.WasPressedThisFrame() ?? false;

                // Gamepad UI
                gpUIUpPressed = gpUIUpAction?.IsPressed() ?? false;
                gpUIUpPressedThisFrame = gpUIUpAction?.WasPressedThisFrame() ?? false;

                gpUIRightPressed = gpUIRightAction?.IsPressed() ?? false;
                gpUIRightPressedThisFrame = gpUIRightAction?.WasPressedThisFrame() ?? false;

                gpUILeftPressed = gpUILeftAction?.IsPressed() ?? false;
                gpUILeftPressedThisFrame = gpUILeftAction?.WasPressedThisFrame() ?? false;

                gpUIDownPressed = gpUIDownAction?.IsPressed() ?? false;
                gpUIDownPressedThisFrame = gpUIDownAction?.WasPressedThisFrame() ?? false;

                gpUISubmitPressed = gpUISubmitAction?.IsPressed() ?? false;
                gpUISubmitPressedThisFrame = gpUISubmitAction?.WasPressedThisFrame() ?? false;

                gpUICancelPressed = gpUICancelAction?.IsPressed() ?? false;
                gpUICancelPressedThisFrame = gpUICancelAction?.WasPressedThisFrame() ?? false;
            }
            else
            {
                kbUIUpPressed = kbUIUpPressedThisFrame = false;
                kbUIRightPressed = kbUIRightPressedThisFrame = false;
                kbUILeftPressed = kbUILeftPressedThisFrame = false;
                kbUIDownPressed = kbUIDownPressedThisFrame = false;
                kbUISubmitPressed = kbUISubmitPressedThisFrame = false;
                kbUICancelPressed = kbUICancelPressedThisFrame = false;

                gpUIUpPressed = gpUIUpPressedThisFrame = false;
                gpUIRightPressed = gpUIRightPressedThisFrame = false;
                gpUILeftPressed = gpUILeftPressedThisFrame = false;
                gpUIDownPressed = gpUIDownPressedThisFrame = false;
                gpUISubmitPressed = gpUISubmitPressedThisFrame = false;
                gpUICancelPressed = gpUICancelPressedThisFrame = false;
            }
        }

        public void UpdateBoundDeviceStrings()
        {
            keyboardName = keyboard != null ? keyboard.displayName : InputManager.none;
            mouseName = mouse != null ? mouse.displayName : InputManager.none;
            gamepadName = gamepad != null ? gamepad.displayName : InputManager.none;
        }

        public string GetKeyboardMapName(int index) => index == 0 ? InputManager.keyboardControls : $"KeyboardControlsP{index + 1}";
        public string GetMouseMapName(int index) => index == 0 ? InputManager.mouseControls : $"MouseControlsP{index + 1}";
        public string GetGamepadMapName(int index) => index == 0 ? InputManager.gamepadControls : $"GamePadControlsP{index + 1}";
        public string GetUIMapName(int index) => index == 0 ? InputManager.uiControls : $"UIControlsP{index + 1}";
    }

    public List<PlayerInput> players = new List<PlayerInput>();

    private void Start()
    {
        //GameManager.Instance.inputManager = this;
        while (players.Count < totalPlayers) players.Add(new PlayerInput() { playerNumber = players.Count + 1 });

        RefreshDeviceList();
        AssignInitialDevices();
        ForceRebindAllPlayers();

        // --- Assign player scripts per playerNumber ---
        if (playerInputActionAsset != null)
        {
#if UNITY_EDITOR
            // Update the stored type string from the MonoScript reference
            playerInputActionAsset.UpdateScriptTypeName();
#endif
            // Assign the scripts at runtime
            if (playerInputActionAsset.PlayerScriptType != null) playerInputActionAsset.GetPlayerScripts(this);
        }

        InputSystem.onDeviceChange += OnDeviceChange;
    }


    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Update()
    {
        foreach (var player in players)
        {
            if (player.keyboard == null && player.mouse == null && player.gamepad == null) continue;
            player.UpdateInput();
            player.UpdateActionStates();
        }
    }

    private void RefreshDeviceList()
    {
        devices.keyboards = InputSystem.devices.OfType<Keyboard>().ToArray();
        devices.mice = InputSystem.devices.OfType<Mouse>().ToArray();
        devices.gamepads = InputSystem.devices.OfType<Gamepad>().ToArray();

        devices.connectedDeviceNames.Clear();
        foreach (var d in InputSystem.devices) devices.connectedDeviceNames.Add(d.displayName);
        if (debug) Utils.Log("[InputManager] Connected devices: " + string.Join(", ", devices.connectedDeviceNames, 0));
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                RefreshDeviceList();

                // Auto-assign new device to the first available player
                if (device is Gamepad gp)
                {
                    // Find first player without a gamepad
                    var player = players.FirstOrDefault(p => p.gamepad == null);
                    if (player != null) AssignGamepadPlayer(gp, player.playerNumber);
                }
                else if (device is Keyboard kb)
                {
                    var player = players.FirstOrDefault(p => p.keyboard == null);
                    if (player != null) AssignKeyboardMousePlayer(kb, player.playerNumber);
                }
                else if (device is Mouse m)
                {
                    var player = players.FirstOrDefault(p => p.mouse == null);
                    if (player != null) AssignKeyboardMousePlayer(m, player.playerNumber);
                }
                break;

            case InputDeviceChange.Removed:
                RefreshDeviceList();

                // Unbind device from any player that had it
                foreach (var player in players)
                {
                    if (player == null) continue;
                    if (player.keyboard == device || player.mouse == device || player.gamepad == device) UnbindDeviceFromPlayer(device, player);
                }
                break;

            case InputDeviceChange.Disconnected:
                // Optionally handle temporary disconnection without unbinding
                break;

            case InputDeviceChange.Reconnected:
                // Reassign device if needed
                break;

            default:
                break;
        }
    }

    private void UnbindDeviceFromPlayer(InputDevice device, PlayerInput player)
    {
        if (player == null || !player.user.valid) return;

        if (player.keyboard == device)
        {
            player.user.UnpairDevice(player.keyboard);
            player.keyboard = null;
            if (player.keyboardActionMap != null) player.keyboardActionMap.Disable();
        }

        if (player.mouse == device)
        {
            player.user.UnpairDevice(player.mouse);
            player.mouse = null;
            if (player.mouseActionMap != null) player.mouseActionMap.Disable();
        }

        if (player.gamepad == device)
        {
            player.user.UnpairDevice(player.gamepad);
            player.gamepad = null;
            if (player.gamepadActionMap != null) player.gamepadActionMap.Disable();
        }

        ReassociateAllMaps(player);

        player.usingGamePad = false;
        player.usingKeyboard = player.keyboard != null || player.mouse != null;

        player.UpdateBoundDeviceStrings();
    }

    private void AssignInitialDevices()
    {
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];

            // Assign physical devices
            if (i < devices.keyboards.Length) player.keyboard = devices.keyboards[i];
            if (i < devices.mice.Length) player.mouse = devices.mice[i];
            if (i < devices.gamepads.Length) player.gamepad = devices.gamepads[i];

            // Create user WITHOUT pairing
            if (!player.user.valid) player.user = InputUser.CreateUserWithoutPairedDevices();

            // ------------------------
            // CLONE & ASSOCIATE MAPS
            // ------------------------

            if (playerInputActionAsset == null || playerInputActionAsset.playerInputActions == null) continue;

            // KEYBOARD
            if (player.keyboard != null)
            {
                var map = playerInputActionAsset.playerInputActions.FindActionMap(player.GetKeyboardMapName(i));
                if (map != null)
                {
                    player.keyboardActionMap = map.Clone();
                    player.user.AssociateActionsWithUser(player.keyboardActionMap);
                    player.keyboardActionMap.Enable();

                    player.keyboardAnyAction = player.keyboardActionMap.FindAction(any);
                    player.jumpAction = player.keyboardActionMap.FindAction(jump);
                    player.interactAction = player.keyboardActionMap.FindAction(interact);
                    player.attackAction = player.keyboardActionMap.FindAction(attack);
                }
            }

            // MOUSE
            if (player.mouse != null)
            {
                var map = playerInputActionAsset.playerInputActions.FindActionMap(player.GetMouseMapName(i));
                if (map != null)
                {
                    player.mouseActionMap = map.Clone();
                    player.user.AssociateActionsWithUser(player.mouseActionMap);
                    player.mouseActionMap.Enable();

                    player.mouseAnyAction = player.mouseActionMap.FindAction(any);
                    player.leftClickAction = player.mouseActionMap.FindAction(leftClick);
                    player.rightClickAction = player.mouseActionMap.FindAction(rightClick);
                    player.scrollWheelClickAction = player.mouseActionMap.FindAction(scrollWheelClick);
                }
            }

            // GAMEPAD
            if (player.gamepad != null)
            {
                var map = playerInputActionAsset.playerInputActions.FindActionMap(player.GetGamepadMapName(i));
                if (map != null)
                {
                    player.gamepadActionMap = map.Clone();
                    player.user.AssociateActionsWithUser(player.gamepadActionMap);
                    player.gamepadActionMap.Enable();

                    player.gamepadAnyAction = player.gamepadActionMap.FindAction(any);
                    player.gamepadJumpAction = player.gamepadActionMap.FindAction(jump);
                    player.gamepadInteractAction = player.gamepadActionMap.FindAction(interact);
                    player.gamepadAttackAction = player.gamepadActionMap.FindAction(attack);
                }
            }

            // ------------------------
            // UI MAP
            // ------------------------
            var uiMap = playerInputActionAsset.playerInputActions.FindActionMap(player.GetUIMapName(i));
            if (uiMap != null)
            {
                player.uiActionMap = uiMap.Clone();
                player.user.AssociateActionsWithUser(player.uiActionMap);
                player.uiActionMap.Enable();
                // Keyboard
                player.kbUIUpAction = player.uiActionMap.FindAction(kbUIUp);
                player.kbUIRightAction = player.uiActionMap.FindAction(kbUIRight);
                player.kbUILeftAction = player.uiActionMap.FindAction(kbUILeft);
                player.kbUIDownAction = player.uiActionMap.FindAction(kbUIDown);
                player.kbUISubmitAction = player.uiActionMap.FindAction(kbUISubmit);
                player.kbUICancelAction = player.uiActionMap.FindAction(kbUICancel);
                // Gamepad
                player.gpUIUpAction = player.uiActionMap.FindAction(gpUIUp);
                player.gpUIRightAction = player.uiActionMap.FindAction(gpUIRight);
                player.gpUILeftAction = player.uiActionMap.FindAction(gpUILeft);
                player.gpUIDownAction = player.uiActionMap.FindAction(gpUIDown);
                player.gpUISubmitAction = player.uiActionMap.FindAction(gpUISubmit);
                player.gpUICancelAction = player.uiActionMap.FindAction(gpUICancel);                
            }

            // ------------------------
            // PAIR DEVICES LAST
            // ------------------------
            if (player.keyboard != null) InputUser.PerformPairingWithDevice(player.keyboard, player.user);
            if (player.mouse != null) InputUser.PerformPairingWithDevice(player.mouse, player.user);
            if (player.gamepad != null) InputUser.PerformPairingWithDevice(player.gamepad, player.user);
            player.UpdateBoundDeviceStrings();
        }
        if (debug) PrintPlayerBindings();
    }


    private void ForceRebindAllPlayers()
    {
        foreach (var p in players)
        {
            if (!p.user.valid) continue;
            if (p.keyboard != null) InputUser.PerformPairingWithDevice(p.keyboard, p.user);
            if (p.mouse != null) InputUser.PerformPairingWithDevice(p.mouse, p.user);
            if (p.gamepad != null) InputUser.PerformPairingWithDevice(p.gamepad, p.user);
        }
    }

    private void AssignKeyboardMousePlayer(InputDevice device, int playerNumber)
    {
        if (device == null) return;
        if (playerNumber < 1 || playerNumber > players.Count) return;

        var player = players[playerNumber - 1];

        if (!player.user.valid) player.user = InputUser.CreateUserWithoutPairedDevices();

        if (device is Keyboard kb)
        {
            if (player.keyboard == kb) return;
            if (player.keyboard != null) player.user.UnpairDevice(player.keyboard);
            player.keyboard = kb;
            InputUser.PerformPairingWithDevice(kb, player.user);
        }

        if (device is Mouse m)
        {
            if (player.mouse == m) return;
            if (player.mouse != null) player.user.UnpairDevice(player.mouse);
            player.mouse = m;
            InputUser.PerformPairingWithDevice(m, player.user);
        }

        ReassociateAllMaps(player);

        if (player.keyboardActionMap != null && !player.keyboardActionMap.enabled) player.keyboardActionMap.Enable();
        if (player.mouseActionMap != null && !player.mouseActionMap.enabled) player.mouseActionMap.Enable();
        if (player.uiActionMap != null && !player.uiActionMap.enabled) player.uiActionMap.Enable();
        player.UpdateBoundDeviceStrings();
    }

    private void AssignGamepadPlayer(Gamepad gp, int playerNumber)
    {
        if (gp == null) return;
        if (playerNumber < 1 || playerNumber > players.Count) return;

        var player = players[playerNumber - 1];

        // Already assigned → do nothing
        if (player.gamepad == gp && player.user.valid) return;

        // Ensure user exists
        if (!player.user.valid)
            player.user = InputUser.CreateUserWithoutPairedDevices();

        // Unpair old gamepad if necessary
        if (player.gamepad != null && player.gamepad != gp) player.user.UnpairDevice(player.gamepad);

        // Pair the new gamepad first
        player.gamepad = gp;
        InputUser.PerformPairingWithDevice(gp, player.user);

        // Clone the action map **after pairing**
        var map = playerInputActionAsset.playerInputActions.FindActionMap(player.GetGamepadMapName(player.playerNumber - 1));
        if (map != null)
        {
            player.gamepadActionMap = map.Clone(); // clone AFTER pairing
            player.user.AssociateActionsWithUser(player.gamepadActionMap);
            player.gamepadActionMap.Enable();

            // Bind actions
            player.gamepadAnyAction = player.gamepadActionMap.FindAction(any);
            player.gamepadJumpAction = player.gamepadActionMap.FindAction(jump);
            player.gamepadInteractAction = player.gamepadActionMap.FindAction(interact);
            player.gamepadAttackAction = player.gamepadActionMap.FindAction(attack);
        }

        // Ensure UI actions are associated
        if (player.uiActionMap != null) player.user.AssociateActionsWithUser(player.uiActionMap);
        player.UpdateBoundDeviceStrings();
    }

    private void ReassociateAllMaps(InputManager.PlayerInput player)
    {
        if (!player.user.valid) return;
        if (player.keyboardActionMap != null) player.user.AssociateActionsWithUser(player.keyboardActionMap);
        if (player.mouseActionMap != null) player.user.AssociateActionsWithUser(player.mouseActionMap);
        if (player.gamepadActionMap != null) player.user.AssociateActionsWithUser(player.gamepadActionMap);
        if (player.uiActionMap != null) player.user.AssociateActionsWithUser(player.uiActionMap);
    }

    public void PrintPlayerBindings()
    {
        if (!debug) return;

        for (int i = 0; i < players.Count; i++)
        {
            var p = players[i];
            Utils.Log($"Player {p.playerNumber} -> Keyboard: {p.keyboardName}, Mouse: {p.mouseName}, Gamepad: {p.gamepadName}", 0);
        }
    }

    private Coroutine RumbleCoroutine;
    //inputManager.RumbleGamepad(players[0], leftMotor, rightMotor, duration);
    public void RumbleGamepad(PlayerInput player, float leftMotor, float rightMotor, float duration)
    {
        if (player.gamepad == null) return;
        // Stop existing rumble
        if (RumbleCoroutine != null) StopCoroutine(RumbleCoroutine);
        RumbleCoroutine = StartCoroutine(RumbleRoutine(player.gamepad, leftMotor, rightMotor, duration));
    }
    private IEnumerator RumbleRoutine(Gamepad gamepad, float left, float right, float time)
    {
        gamepad.SetMotorSpeeds(left, right);
        yield return new WaitForSeconds(time);
        gamepad.SetMotorSpeeds(0f, 0f);
        RumbleCoroutine = null;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ActionAsset))]
public class ActionAssetDrawer : PropertyDrawer
{
    private bool foldout = true; // Track foldout state

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Retrieve the properties
        SerializedProperty layerProp = property.FindPropertyRelative("playerLayer");
        SerializedProperty tagProp = property.FindPropertyRelative("playerTag");
        SerializedProperty actionAssetProp = property.FindPropertyRelative("playerInputActions");
        SerializedProperty scriptRefProp = property.FindPropertyRelative("playerScriptReference");

        // Define basic layout settings
        float y = position.y;
        float lh = EditorGUIUtility.singleLineHeight;
        float spacing = 3f; // Increased spacing for better separation

        // Draw the foldout header
        foldout = EditorGUI.Foldout(new Rect(position.x, y, position.width, lh), foldout, "Action Asset Settings", true);
        y += lh + spacing;

        if (foldout) // Only draw the content if the foldout is expanded
        {
            // Draw Input Action Asset reference field first (at the top)
            float actionAssetHeight = EditorGUI.GetPropertyHeight(actionAssetProp, true); // Ensure correct height for this property
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, actionAssetHeight), actionAssetProp);
            y += actionAssetHeight + spacing;

            // Draw Player Script Reference field second (below the action asset)
            float scriptRefHeight = EditorGUI.GetPropertyHeight(scriptRefProp, true); // Ensure correct height for this property
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, scriptRefHeight), scriptRefProp);
            y += scriptRefHeight + spacing;

            // Draw Player Layer dropdown field third (below the script reference)
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, lh), layerProp);
            y += lh + spacing;

            // Draw Player Tag dropdown field last (below the player layer)
            string[] tags = InternalEditorUtility.tags;
            int currentIndex = System.Array.IndexOf(tags, tagProp.stringValue);
            if (currentIndex < 0) currentIndex = 0;
            int newIndex = EditorGUI.Popup(new Rect(position.x, y, position.width, lh), "Player Tag", currentIndex, tags);
            tagProp.stringValue = tags[newIndex];
            y += lh + spacing;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Define line height and spacing
        float lh = EditorGUIUtility.singleLineHeight;
        float spacing = 10f; // Increased spacing

        // Calculate the heights of the complex properties
        SerializedProperty actionAssetProp = property.FindPropertyRelative("playerInputActions");
        SerializedProperty scriptRefProp = property.FindPropertyRelative("playerScriptReference");

        // Add extra height for ActionAsset and ScriptReference fields
        float actionAssetHeight = EditorGUI.GetPropertyHeight(actionAssetProp, true);
        float scriptRefHeight = EditorGUI.GetPropertyHeight(scriptRefProp, true);

        // Calculate total height needed for all fields if expanded
        if (foldout) return actionAssetHeight + scriptRefHeight + (lh * 2) + (spacing * 4); // Increased spacing
        else return lh +spacing; // Just the foldout title height when collapsed
    }
}
#endif

