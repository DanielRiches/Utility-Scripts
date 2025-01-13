using UnityEngine;

public class Strings
{
    // CORE
    public static string loadingProgressFormat = "{0}%";
    public static string cameraMain = "MainCamera";

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
