using UnityEngine;

public class Strings
{
    // CORE
    public static string loadingProgressFormat = "{0}%";
    public static string cameraMain = "MainCamera";
    public static string numberFormat0 = "F0";// will display 0
    public static string numberFormat1 = "F1";// will display 0.0
    public static string numberFormat2 = "F2";// will display 0.00

    // UI
    public static string dropdownViewport = "Viewport";
    public static string dropdownContent = "Content";
    public static string inputSystemUIInputModuleScrollWheel = "ScrollWheel";

    // AUDIO
    public static string audioMaster = "audioMaster";
    public static string audioMusic = "audioMusic";
    public static string audioAmbient = "audioAmbient";
    public static string audioSFX = "audioSFX";
    public static string audioUI = "audioUI";
    public static string audioUIPitchShift = "audioUIPitchShift";
    public static string audioUIPitchShiftVolume = "audioUIPitchShiftVolume";
    public static string audioVoice = "audioVoice";
    public static string audioEvent = "audioEvent";

    // SCENES
    public const string menuSceneName = "Menus";
    public const string loadingSceneName = "LevelTransitionLoadingScreen";

    // TAGS
    public const string gameManagerTag = "GameManager";
    public const string slopeTag = "Slope";

    // LAYERS
    public const string defaultLayer = "Default";
    public const string environmentLayer = "Environment";

    // ANIMATION
    public static readonly int on = Animator.StringToHash("On");
    public static readonly int off = Animator.StringToHash("Off");

    public static readonly int animGrounded = Animator.StringToHash("grounded");
    public static readonly int animIdle = Animator.StringToHash("idle");
    public static readonly int animMoveForward = Animator.StringToHash("moveForward");
    public static readonly int animMoveBack = Animator.StringToHash("moveBack");
    public static readonly int animMoveLeft = Animator.StringToHash("moveLeft");
    public static readonly int animMoveRight = Animator.StringToHash("moveRight");
    public static readonly int animRunning = Animator.StringToHash("running");
    public static readonly int animJumping = Animator.StringToHash("jumping");
    public static readonly int animFalling = Animator.StringToHash("falling");
}
