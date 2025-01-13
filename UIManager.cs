using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [Header("---- Main Menu --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private float mainMenuFadeDuration = 8f;
    private float mainMenuFadeDurationElapsedTime = 0f;
    [Header("Issue")]
    [Space(5)]
    [SerializeField] private Transform issueImage;
    [SerializeField] private Transform issueImageOn;
    [SerializeField] private Transform issueImageOff;
    private float issueImageFadeDurationElapsedTime = 0f;
    private float issueImageAlphaFadeDurationElapsedTime = 0f;
    [SerializeField] private float issueImageFadeDuration = 1f;
    [Header("---- Options ----------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup optionsCanvasGroup;
    [SerializeField] private Transform optionsBGImage;
    private float optionsImageFadeDurationElapsedTime = 0f;
    [SerializeField] private float optionsImageFadeDuration = 1f;
    private Vector3 optionsDesiredScale = new Vector3(1f,1f,1f);
    [SerializeField] private Transform optionsExitImage;
    private float optionsExitImageFadeDurationElapsedTime = 0f;
    [SerializeField] private float optionsExitImageFadeDuration = 1f;
    [Header("---- Diagnostics ------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup diagnosticsFPSCanvasGroup;
    private float diagnosticsFPSCanvasFadeDurationElapsedTime = 0f;
    [SerializeField] private float diagnosticsFPSCanvasFadeDuration = 2f;
    [SerializeField] private Transform diagnosticsFPSImage;
    [SerializeField] private Transform diagnosticsFPSImageOn;
    [SerializeField] private Transform diagnosticsFPSImageOff;
    private float diagnosticsFPSImageFadeDurationElapsedTime = 0f;
    [SerializeField] private float diagnosticsFPSImageFadeDuration = 1f;
    [Header("Ping")]
    [Space(5)]
    [SerializeField] private CanvasGroup diagnosticsPingCanvasGroup;
    [SerializeField] private float diagnosticsPingCanvasFadeDuration = 2f;
    private float diagnosticsPingCanvasFadeDurationElapsedTime = 0f;
    [SerializeField] private Transform diagnosticsPingImage;
    [SerializeField] private Transform diagnosticsPingImageOn;
    [SerializeField] private Transform diagnosticsPingImageOff;
    private float diagnosticsPingImageFadeDurationElapsedTime = 0f;
    [SerializeField] private float diagnosticsPingImageFadeDuration = 1f;
    [Header("Memory")]
    [Space(5)]
    [SerializeField] private CanvasGroup diagnosticsMemoryCanvasGroup;
    [SerializeField] private float diagnosticsMemoryCanvasFadeDuration = 2f;
    private float diagnosticsMemoryCanvasFadeDurationElapsedTime = 0f;
    [SerializeField] private Transform diagnosticsMemoryImage;
    [SerializeField] private Transform diagnosticsMemoryImageOn;
    [SerializeField] private Transform diagnosticsMemoryImageOff;
    private float diagnosticsMemoryImageFadeDurationElapsedTime = 0f;
    [SerializeField] private float diagnosticsMemoryImageFadeDuration = 1f;
    [Header("---- Loading ------------------------------------------------------------------")]
    [Space(5)]
    public TextMeshProUGUI loadingProgress;
    public GameObject loadingIcon;
    [SerializeField] private float loadingIconRotationSpeed = 1f;
    [HideInInspector] public float percentageCompleted = 0f;
    [HideInInspector] public float combinedProgress = 0f;


    private void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.uiManager = this;
    }

    void Update()
    {
        LerpUIPosition(gameManager.inMainMenu, issueImage, issueImageOn, issueImageOff, ref issueImageFadeDurationElapsedTime, ref issueImageFadeDuration);
        LerpUIAlpha(gameManager.inMainMenu, ref mainMenuFadeDurationElapsedTime, ref mainMenuFadeDuration, mainMenuCanvasGroup);


        Options();
        Loading();
        Diagnostics();
    }

    private void Options()
    {
        LerpUIScale(gameManager.inOptionsMenu, optionsBGImage, ref optionsDesiredScale, ref optionsImageFadeDurationElapsedTime, ref optionsImageFadeDuration, optionsCanvasGroup);
        LerpUIScale(gameManager.inOptionsMenu, optionsExitImage, ref optionsDesiredScale, ref optionsExitImageFadeDurationElapsedTime, ref optionsExitImageFadeDuration, optionsCanvasGroup);
    }

    private void Loading()
    {
        if (gameManager.loading)
        {
            if (loadingProgress)
            {
                if (!loadingProgress.enabled)
                {
                    loadingProgress.enabled = true;
                }
                loadingProgress.text = string.Format(Strings.loadingProgressFormat, percentageCompleted);
            }

            if (loadingIcon)
            {
                if (!loadingIcon.activeSelf)
                {
                    loadingIcon.SetActive(true);
                }
                if (loadingIcon)
                {
                    if (loadingIcon.activeSelf)
                    {
                        loadingIcon.transform.Rotate(0, 0, loadingIconRotationSpeed * Time.deltaTime);
                    }
                }
            }
        }
        else
        {
            if (loadingIcon)
            {
                if (loadingIcon.activeSelf)
                {
                    loadingIcon.SetActive(false);
                }
            }
        }
    }

    private void Diagnostics()
    {
        if (gameManager.options.displayFPS || gameManager.options.displayPing || gameManager.options.displayMemory)
        {
            LerpUIPosition(gameManager.options.displayFPS, diagnosticsFPSImage, diagnosticsFPSImageOn, diagnosticsFPSImageOff, ref diagnosticsFPSImageFadeDurationElapsedTime, ref diagnosticsFPSCanvasFadeDuration);
            LerpUIPosition(gameManager.options.displayPing, diagnosticsPingImage, diagnosticsPingImageOn, diagnosticsPingImageOff, ref diagnosticsPingImageFadeDurationElapsedTime, ref diagnosticsPingCanvasFadeDuration);
            LerpUIPosition(gameManager.options.displayMemory, diagnosticsMemoryImage, diagnosticsMemoryImageOn, diagnosticsMemoryImageOff, ref diagnosticsMemoryImageFadeDurationElapsedTime, ref diagnosticsMemoryCanvasFadeDuration);
        }
    }

    void LerpUIPosition(bool targetMenu, Transform transform, Transform on, Transform off, ref float elapsedTime, ref float duration)
    {
        elapsedTime += Time.deltaTime;
        Vector3 targetPosition = targetMenu ? on.position : off.position;
        Vector3 startPosition = targetMenu ? off.position : on.position;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition;
            elapsedTime = 0f;
        }

        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
        }        
    }

    void LerpUIScale(bool targetMenu, Transform transform, ref Vector3 desiredScale, ref float elapsedTime, ref float duration, CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = targetMenu;
        canvasGroup.blocksRaycasts = targetMenu;

        elapsedTime += Time.deltaTime;
        Vector3 targetScale = targetMenu ? desiredScale : Vector3.zero;

        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsedTime / duration);
        }
        else
        {
            elapsedTime = 0f;
        }
    }

    void LerpUIAlpha(bool targetMenu, ref float elapsedTime, ref float duration, CanvasGroup canvasGroup)
    {
        if (targetMenu)
        {
            if (canvasGroup.alpha < 1)
            {
                if (canvasGroup.alpha == 0) { elapsedTime = 0f; }                    

                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(Mathf.Lerp(0, 1, elapsedTime / duration));
                canvasGroup.interactable = true; // Activate interaction
                canvasGroup.blocksRaycasts = true; // Activate blocking of raycasts
            }
        }
        else
        {
            if (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha = 0f;
                elapsedTime = 0f;
                canvasGroup.interactable = false; // Activate interaction
                canvasGroup.blocksRaycasts = false; // Activate blocking of raycasts
            }
        }
    }
}
