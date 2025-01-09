using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [Header("---- Main Menu ------------------------------------------------------------")]
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
    [SerializeField] private float issueImageFadeDuration = 1f;
    [Header("---- Options ------------------------------------------------------------")]
    [Space(5)]
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
    [Header("---- Loading ------------------------------------------------------------")]
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
        MainMenu();
        Options();
        Loading();
        Diagnostics();
    }

    private void MainMenu()
    {
        if (gameManager.inMainMenu)
        {
            if (mainMenuCanvasGroup.interactable == false || mainMenuCanvasGroup.blocksRaycasts == false)
            {
                mainMenuCanvasGroup.interactable = true; // Activate interaction
                mainMenuCanvasGroup.blocksRaycasts = true; // Activate blocking of raycasts
            }

            if (mainMenuCanvasGroup.alpha < 1)
            {
                if (mainMenuCanvasGroup.alpha == 0)
                {
                    mainMenuFadeDurationElapsedTime = 0f;
                }

                mainMenuFadeDurationElapsedTime += Time.deltaTime;
                mainMenuCanvasGroup.alpha = Mathf.Lerp(0, 1, mainMenuFadeDurationElapsedTime / mainMenuFadeDuration);
                mainMenuCanvasGroup.alpha = Mathf.Clamp01(mainMenuCanvasGroup.alpha); // Ensure alpha stays between 0 and 1
            }

            if (issueImage.position != issueImageOn.position)
            {
                issueImageFadeDurationElapsedTime += Time.deltaTime;
                issueImage.position = Vector3.Lerp(issueImageOff.position, issueImageOn.position, issueImageFadeDurationElapsedTime / issueImageFadeDuration);
            }
            else
            {
                issueImageFadeDurationElapsedTime = 0f;
            }
        }
        else
        {
            if (mainMenuCanvasGroup.interactable == true || mainMenuCanvasGroup.blocksRaycasts == true)
            {
                mainMenuCanvasGroup.interactable = false; // De-activate interaction
                mainMenuCanvasGroup.blocksRaycasts = false; // De-activate blocking of raycasts
            }

            if (mainMenuCanvasGroup.alpha > 0)
            {
                mainMenuCanvasGroup.alpha = 0f;
            }
        }
    }

    private void Options()
    {
        if (gameManager.inOptionsMenu)
        {
            if (optionsBGImage.localScale.x != 1)
            {
                optionsImageFadeDurationElapsedTime += Time.deltaTime;
                optionsBGImage.localScale = Vector3.Lerp(optionsBGImage.localScale, optionsDesiredScale, optionsImageFadeDurationElapsedTime / optionsImageFadeDuration);
            }
            else
            {
                if (optionsExitImage.localScale.x != 1)
                {
                    optionsExitImageFadeDurationElapsedTime += Time.deltaTime;
                    optionsBGImage.localScale = Vector3.Lerp(optionsExitImage.localScale, optionsDesiredScale, optionsExitImageFadeDurationElapsedTime / optionsExitImageFadeDuration);
                }
                else
                {
                    optionsExitImageFadeDurationElapsedTime = 0f;
                }

                optionsImageFadeDurationElapsedTime = 0f;
            }
        }
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
        LerpUI(gameManager.options.displayFPS, diagnosticsFPSImage, diagnosticsFPSImageOn, diagnosticsFPSImageOff, ref diagnosticsFPSImageFadeDurationElapsedTime, ref diagnosticsFPSCanvasFadeDuration, diagnosticsFPSCanvasGroup, ref diagnosticsFPSCanvasFadeDurationElapsedTime, ref diagnosticsFPSCanvasFadeDuration);
        LerpUI(gameManager.options.displayPing, diagnosticsPingImage, diagnosticsPingImageOn, diagnosticsPingImageOff, ref diagnosticsPingImageFadeDurationElapsedTime, ref diagnosticsPingCanvasFadeDuration, diagnosticsPingCanvasGroup, ref diagnosticsPingCanvasFadeDurationElapsedTime, ref diagnosticsPingCanvasFadeDuration);
        LerpUI(gameManager.options.displayMemory, diagnosticsMemoryImage, diagnosticsMemoryImageOn, diagnosticsMemoryImageOff, ref diagnosticsMemoryImageFadeDurationElapsedTime, ref diagnosticsMemoryCanvasFadeDuration, diagnosticsMemoryCanvasGroup, ref diagnosticsMemoryCanvasFadeDurationElapsedTime, ref diagnosticsMemoryCanvasFadeDuration);
    }

    void LerpUI(bool display, Transform image, Transform on, Transform off, ref float elapsedTime, ref float duration, CanvasGroup canvasGroup, ref float canvasGroupElapsedTime, ref float canvasGroupDuration)
    {
        if (display)
        {            
            if (image.position != on.position)
            {    
                elapsedTime += Time.deltaTime;
                image.position = Vector3.Lerp(off.position, on.position, elapsedTime / duration);
            }
            else
            {
                
                if (canvasGroup.alpha < 1)
                {                    
                    if (canvasGroup.alpha == 0)
                    {
                        canvasGroupElapsedTime = 0f;                        
                    }

                    elapsedTime = 0f;
                    canvasGroupElapsedTime += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(0, 1, canvasGroupElapsedTime / canvasGroupDuration);
                    canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha); // Ensure alpha stays between 0 and 1
                }
            }
        }
        else
        {
            if (canvasGroup.alpha > 0)
            {                
                canvasGroup.alpha = 0f;
                canvasGroupElapsedTime = 0f;
                elapsedTime = 0f;
            }

            if (image.position != off.position)
            {                
                elapsedTime += Time.deltaTime;
                image.position = Vector3.Lerp(on.position, off.position, elapsedTime / duration);
            }
            else
            {                
                elapsedTime = 0f;
            }
        }
    }
}
