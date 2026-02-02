using UnityEngine;

public class Strings
{
    // CORE------------------------------------------------------------------------------------------------------------------------
    public const string StartGame = "StartGame";
    public const string loadingProgressFormat = "{0}%";
    public const string cameraMain = "MainCamera";
    public const string numberFormat0 = "F0";// will display 0
    public const string numberFormat1 = "F1";// will display 0.0
    public const string numberFormat2 = "F2";// will display 0.00

    // LOCALIZATION-----------------------------------------------------------------------------------------------------------------------
    public const string languageEnglish = "en";
    public const string languageChineseSimplified = "zh-Hans";
    public const string languageIndonesian = "id";
    public const string languageRussian = "ru";
    public const string languageSpanish = "es";
    public const string languagePortugueseBrazilian = "pt-BR";
    public const string languageFrench = "fr";
    public const string languageItalian = "it";
    public const string languageGerman = "de";
    public const string languageSwedish = "sv";
    public const string languageGreek = "el";
    public const string languageTurkish = "tr";
    public const string languageArabic = "ar";
    public const string languageKorean = "ko";
    public const string languagePolish = "pl";
    public const string languagePersian = "fa";
    public const string languageJapanese = "ja";
    public const string languageSwahili = "sw";
    public const string languageHindi = "hi";
    public const string languageThai = "th-TH";

    public const string languageEnglishFull = "English";
    public const string languageChineseSimplifiedFull = "Chinese";
    public const string languageIndonesianFull = "Indonesian";
    public const string languageRussianFull = "Russian";
    public const string languageSpanishFull = "Spanish";
    public const string languagePortugueseBrazilianFull = "Portuguese";
    public const string languageFrenchFull = "French";
    public const string languageItalianFull = "Italian";
    public const string languageGermanFull = "German";
    public const string languageSwedishFull = "Swedish";
    public const string languageGreekFull = "Greek";
    public const string languageTurkishFull = "Turkish";
    public const string languageArabicFull = "Arabic";
    public const string languageKoreanFull = "Korean";
    public const string languagePolishFull = "Polish";
    public const string languagePersianFull = "Persian";
    public const string languageJapaneseFull = "Japanese";
    public const string languageSwahiliFull = "Swahili";
    public const string languageHindiFull = "Hindi";
    public const string languageThaiFull = "Thai";

    // INPUT-----------------------------------------------------------------------------------------------------------------------
    // Maps
    public const string keyboardActionMap = "KeyboardControls";
    public const string mouseActionMap = "MouseControls";
    public const string keyboardMouseLeftClick = "LeftClick";
    public const string keyboardMouseRightClick = "RightClick";
    public const string keyboardMouseScrollWheel = "ScrollWheel";
    public const string gamepadActionMap = "GamepadControls";

    // TAGS----------------------------------------------------------------------------------------------------------------------
    public const string tagGameManager = "GameManager";
    public const string tagMainCamera = "MainCamera";
    public const string tagStairs = "Stairs"; // Slopes are detected via raycast normal direction in Player script
    public const string tagDoor = "Door";
    public const string tagPlayer = "Player";
    public const string tagUntagged = "Untagged";

    // SCRIPT--------------------------------------------------------------------------------------------------------------------
    public const string playerNumber = "playerNumber";

    // SHADERS-------------------------------------------------------------------------------------------------------------------
    public static readonly int shaderEmissionColorID = Shader.PropertyToID("_EmissiveColorIntensity");

    //VFX------------------------------------------------------------------------------------------------------------------------
    public const string vfxExplosionPos = "explosionPos";
    public const string vfxFragGrenade = "Frag";
    public const string vfxBulletImpact = "Impact";
    public const string vfxFire = "Fire";
    public const string vfxRange = "Range";
    public const string vfxSmoke = "Smoke";
    public const string vfxOn = "On";
    public const string vfxVisible = "Visible";
    public const string vfxTransition = "Transition";
    public const string vfxColor = "Color";
    public const string vfxColorRainbow = "Rainbow";
    // Player
    public const string vfxShrink = "Shrink";
    public const string vfxDie = "Die";
    public const string vfxPupilsState = "PupilsState";
    public const string vfxEyebrowState = "EyebrowState";
    public const string vfxEyeState = "EyeState";
    public const string vfxMouthState = "MouthState";
    public const string vfxPupilRightPosition = "PupilRightPosition";
    public const string vfxPupilLeftPosition = "PupilLeftPosition";
    public const string vfxWhistle = "Whistle";
    public const string vfxTeleport = "Teleport";
    public const string vfxReposition = "Reposition";
    // UI
    // Teleporter
    public const string uiState = "UIState";
    public const string uiActiveNum = "UIActiveNum";
    public const string uiBox = "Box";
    public const string uiBox1 = "Box1";
    public const string uiBox2 = "Box2";
    public const string uiBox3 = "Box3";
    public const string uiBox4 = "Box4";
    public const string uiBox5 = "Box5";
    public const string uiBox6 = "Box6";
    public const string uiBox7 = "Box7";
    public const string uiBox8 = "Box8";

    // MATERIALS-----------------------------------------------------------------------------------------------------------------
    public const string materialHDRPColor = "_BaseColor";
    public const string materialUnityColor = "_Color";
    public const string materialUnityEmissionColor = "_EmissionColor";

    // LAYERS--------------------------------------------------------------------------------------------------------------------
    public static readonly int defaultLayer = LayerMask.NameToLayer("Default");
    public static readonly int environmentLayer = LayerMask.NameToLayer("Environment");
    public static readonly int playerLayer = LayerMask.NameToLayer("Player");
    public static readonly int enemyLayer = LayerMask.NameToLayer("Enemy");
    public static readonly int levelLayer = LayerMask.NameToLayer("Level");

    // ANIMATION-----------------------------------------------------------------------------------------------------------------
    public static readonly int animIdle = Animator.StringToHash("Idle");
    public static readonly int animIdleLook = Animator.StringToHash("IdleLook");
    public static readonly int animJump = Animator.StringToHash("Jumping");
    public static readonly int animMove = Animator.StringToHash("Moving");
    public static readonly int animFall = Animator.StringToHash("Falling");
    public static readonly int animLanding = Animator.StringToHash("Landing");
    public static readonly int animTransition = Animator.StringToHash("Transition");
    public static readonly int animTeleporting = Animator.StringToHash("Teleporting");
    public static readonly int animLandHash = Animator.StringToHash("Land");
    public static readonly int animShoot = Animator.StringToHash("Shoot");
    public static readonly int animActive = Animator.StringToHash("Active");
    public static readonly int animDrawEnergy = Animator.StringToHash("DrawEnergy");
}
