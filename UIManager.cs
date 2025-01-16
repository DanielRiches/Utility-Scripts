using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerInput playerInput; // Reference Auto-Generated C# Action Map Script
    [SerializeField] private GameManager gameManager;
    [Header("---- Boot --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup bootCanvasGroup;
    [Header("---- Fullscreen --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup fullscreenCanvasGroup;
    [Header("---- Cursor --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup cursorCanvasGroup;
    [SerializeField] private Texture2D pcCursorTexture;
    [SerializeField] private Texture2D controllerCursorTexture;
    [SerializeField] private RawImage cursorRenderer;
    [SerializeField] private Transform cursorTransform;
    [Header("---- Main Menu --------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup mainMenuCanvasGroup2;
    [SerializeField] private Animator mainMenuAnimator;
    [Header("Issue")]
    [Space(5)]
    [SerializeField] private Animator issueImageAnimator;
    [Header("---- Options ----------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup optionsMenuCanvasGroup;
    [SerializeField] private Animator optiongBGAnimator;
    [SerializeField] private Animator optionsExitAnimator;
    [SerializeField] private Transform optionsExitImage;
    private float optionsExitImageScaleDurationElapsedTime = 0f;
    [SerializeField] private float optionsExitImageScaleDuration = 1f;
    [Header("---- Diagnostics ------------------------------------------------------------")]
    [Space(5)]
    [SerializeField] private CanvasGroup diagnosticsCanvasGroup;
    [Space(5)]
    [SerializeField] private Animator diagnosticsFPSAnimator;
    [SerializeField] private Animator diagnosticsFPSCanvasAnimator;
    [Header("Ping")]
    [Space(5)]
    [SerializeField] private Animator diagnosticsPingAnimator;
    [SerializeField] private Animator diagnosticsPingCanvasAnimator;
    [Header("Memory")]
    [Space(5)]
    [SerializeField] private Animator diagnosticsMemoryAnimator;
    [SerializeField] private Animator diagnosticsMemoryCanvasAnimator;
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
        playerInput = new PlayerInput();
        playerInput.KeyboardControls.KeyboardEsc.started += onKeyboardEsc;

        mainMenuAnimator.SetBool(Strings.animUITrigger, true);
        issueImageAnimator.SetBool(Strings.animUITrigger, true);
        optiongBGAnimator.SetBool(Strings.animUITrigger, true);
        diagnosticsFPSAnimator.SetBool(Strings.animUITrigger, true);
        diagnosticsFPSCanvasAnimator.SetBool(Strings.animUITrigger, true);
        diagnosticsPingAnimator.SetBool(Strings.animUITrigger, true);
        diagnosticsPingCanvasAnimator.SetBool(Strings.animUITrigger, true);
        diagnosticsMemoryAnimator.SetBool(Strings.animUITrigger, true);
        diagnosticsMemoryCanvasAnimator.SetBool(Strings.animUITrigger, true);

        ActivateUI(false, fullscreenCanvasGroup);
        ActivateUI(false, bootCanvasGroup);
        ActivateUI(false, cursorCanvasGroup);
        ActivateUI(false, diagnosticsCanvasGroup);      
    }

    private void Update()
    {
        Cursor.visible = false;
        Utils.FollowCursorInUI(gameManager.usingKeyboard, ref cursorTransform);
        if (gameManager.usingKeyboard)
        {
            cursorRenderer.texture = pcCursorTexture;
        }
        else if (gameManager.usingController)
        {
            cursorRenderer.texture = controllerCursorTexture;
        }

        Loading();


        ActivateUI(gameManager.inMainMenu, mainMenuCanvasGroup2);
        ActivateUI(gameManager.inOptionsMenu, optionsMenuCanvasGroup);

        AnimateUI(gameManager.inMainMenu, mainMenuAnimator, 1, Strings.animUIOpen);
        AnimateUI(gameManager.inMainMenu, issueImageAnimator, 0, Strings.animUIOpen);

        AnimateUI(gameManager.inOptionsMenu, optiongBGAnimator, 2, Strings.animUIOpen);

        AnimateUI(gameManager.options.displayFPS, diagnosticsFPSAnimator, 0, Strings.animUIOpen);// DIAGNOSTICS
        AnimateUI(gameManager.options.displayFPS, diagnosticsFPSCanvasAnimator, 1, Strings.animUIOpen);
        AnimateUI(gameManager.options.displayPing, diagnosticsPingAnimator, 0, Strings.animUIOpen);
        AnimateUI(gameManager.options.displayPing, diagnosticsPingCanvasAnimator, 1, Strings.animUIOpen);
        AnimateUI(gameManager.options.displayMemory, diagnosticsMemoryAnimator, 0, Strings.animUIOpen);
        AnimateUI(gameManager.options.displayMemory, diagnosticsMemoryCanvasAnimator, 1, Strings.animUIOpen);
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

    private void ActivateUI(bool targetMenu, CanvasGroup canvasGroup)
    {
        if (targetMenu)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;           
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    private void AnimateUI(bool trigger, Animator animator, int layer, int animationName)
    {
        if (trigger)
        {
            if(animator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 0.98f)
            {
                if (layer == 0)
                {                    
                    animator.SetFloat(Strings.animUISpeed, 1);
                }
                else if (layer == 1)
                {
                    animator.SetFloat(Strings.animUIAlphaSpeed, 1);
                }
                else if (layer == 2)
                {
                    animator.SetFloat(Strings.animUIScaleSpeed, 1);
                }                
            }
            
            if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 0.98f)
            {
                if (layer == 0)
                {
                    animator.SetFloat(Strings.animUISpeed, 0);
                }
                else if (layer == 1)
                {
                    animator.SetFloat(Strings.animUIAlphaSpeed, 0);
                }
                else if (layer == 2)
                {
                    animator.SetFloat(Strings.animUIScaleSpeed, 0);
                }
            }
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime > 0.1f)
            {
                if (layer == 0)
                {
                    animator.SetFloat(Strings.animUISpeed, -1);
                }
                else if (layer == 1)
                {
                    animator.SetFloat(Strings.animUIAlphaSpeed, -1);
                }
                else if (layer == 2)
                {
                    animator.SetFloat(Strings.animUIScaleSpeed, -1);
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(layer).normalizedTime <= 0.1f)
            {
                if (layer == 0)
                {
                    animator.SetFloat(Strings.animUISpeed, 0);
                }
                else if (layer == 1)
                {
                    animator.SetFloat(Strings.animUIAlphaSpeed, 0);
                }
                else if (layer == 2)
                {
                    animator.SetFloat(Strings.animUIScaleSpeed, 0);
                }
            }
        }
    }

    void onKeyboardEsc(InputAction.CallbackContext started)
    {
        if (gameManager.inOptionsMenu)
        {
            gameManager.MainMenu();
        }
    }
}
