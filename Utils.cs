using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Jobs;
using System.Globalization;// Works on Steamdeck
using System.IO;
using System;
using TMPro;
using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public static class Utils
{
    #if ENABLE_LEGACY_INPUT_MANAGER
    static bool legacyInputSystem = true;
    #else
    static bool legacyInputSystem = false;
    #endif

    #if ENABLE_INPUT_SYSTEM
    static bool newInputSystem = true;
    #else
    static bool newInputSystem = false;
    #endif

    // PLATFORM DETECTION
    //Debug.Log("Running on Windows: " + Utils.Windows);
    public static bool Windows { get; private set; } = false;
    public static bool Steamdeck { get; private set; } = false;
    public static bool Playstation5 { get; private set; } = false;
    public static bool XBox { get; private set; } = false;

    static Utils()
    {
        #if UNITY_STANDALONE_WIN
        Windows = true;
        #endif

        #if UNITY_STANDALONE_LINUX
        Steamdeck = true;
        #endif

        #if UNITY_PS5
        Playstation5 = true;
        #endif

        #if UNITY_XBOXONE || UNITY_GAMECORE
        XBox = true;
        #endif
    }

    // Vector3 mouseWorldPosition = Utils.GetCursorWorldPosition3D();
    public static Vector3 GetCursorWorldPosition3D()
    {
        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorWorldPosition, Mathf.Infinity))
        {
            return cursorWorldPosition.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public static void FollowCursorInUI(bool usingKeyboard, ref Transform cursorCanvasImageTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cursorCanvasImageTransform.parent as RectTransform, Input.mousePosition, null, out Vector2 localPoint);

        Vector2 offset;
        if (usingKeyboard)
        {
            offset = new Vector2(16f, -17f);
        }
        else
        {
            offset = Vector2.zero;
        }

        localPoint += offset;

        cursorCanvasImageTransform.localPosition = localPoint;
    }

    // Vector3 hitWorldPosition;
    // int uiLayer = LayerMask.NameToLayer("Layer Name");
    // if (Utils.RayCastIn3DLayerSpecific(uiLayer, out hitWorldPosition))
    public static bool RayCastIn3DLayerSpecific(int desiredLayer, out Vector3 hitPoint)
    {
        Ray ray = CursorRay3D();
        int layerMask = 1 << desiredLayer;

        if (Physics.Raycast(ray, out RaycastHit cursorRayCastHit, Mathf.Infinity, layerMask))
        {
            hitPoint = cursorRayCastHit.point;
            return true;
        }
        else
        {
            hitPoint = Vector3.zero;
            return false;
        }
    }

    // Vector3 hitWorldPosition;
    // if (Utils.RayCastIn3DTagSpecific("Tag Name", hitWorldPosition))
    public static bool RayCastIn3DTagSpecific(string tag, out Vector3 hitPoint)
    {
        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorRayCastHit, Mathf.Infinity) && cursorRayCastHit.transform.CompareTag(tag))
        {
            hitPoint = cursorRayCastHit.point;
            return true;
        }
        else
        {
            hitPoint = Vector3.zero;
            return false;
        }
    }

    // Vector3 hitWorldPosition;
    // uiLayer = LayerMask.NameToLayer("Layer Name");
    // if (Utils.RayCastIn3DLayerAndTagSpecific(uiLayer, "Tag Name", out hitWorldPosition))
    public static bool RayCastIn3DLayerAndTagSpecific(int desiredLayer, string tag, out Vector3 hitPoint)
    {
        Ray ray = CursorRay3D();
        int layerMask = 1 << desiredLayer;

        if (Physics.Raycast(ray, out RaycastHit cursorRayCastHit, Mathf.Infinity, layerMask) && cursorRayCastHit.transform.CompareTag(tag))
        {
            hitPoint = cursorRayCastHit.point;
            return true;
        }
        else
        {
            hitPoint = Vector3.zero;
            return false;
        }
    }

    // Transform targetTransform;
    // GameObject targetObject;
    // targetObject = Utils.GetCursorObjectAndTransform(out targetTransform);
    public static GameObject GetCursorObjectAndTransform(out Transform transform)
    {
        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity))
        {
            transform = cursorObject.transform;
            return cursorObject.transform.gameObject;            
        }
        else
        {
            transform = null;
            return null;            
        }
    }

    // Transform mouseCursorObjectTransform = Utils.GetCursorObjectTransform();
    public static Transform GetCursorObjectTransform()
    {
        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObjectTransform, Mathf.Infinity))
        {
            return cursorObjectTransform.transform;
        }
        else
        {
            return null;
        }
    }

    // GameObject mouseCursorObject = Utils.GetCursorGameObject();
    public static GameObject GetCursorGameObject()
    {
        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity))
        {
            return cursorObject.transform.gameObject;
        }
        else
        {
            return null;
        }
    }


    // GameObject firstCursorObject = Utils.CursorFirstObjectHitNotDefaultLayer();
    public const string defaultLayer = "Default";
    public static GameObject CursorFirstObjectHitNotDefaultLayer()
    {
        var layerMask = Physics.DefaultRaycastLayers & ~LayerMask.GetMask(defaultLayer);

        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity, layerMask))
        {
            return cursorObject.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    // GameObject firstCursorObject = Utils.CursorFirstObjectHit();
    public static GameObject CursorFirstObjectHit()
    {        
        var layerMask = Physics.DefaultRaycastLayers;

        Ray ray = CursorRay3D();

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity, layerMask))
        {
            return cursorObject.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    // if(Utils.GetPreferredSystemLanguage() == "en"){// SYSTEM USING ENGLISH, DO SOMETHING};
    public static string GetPreferredSystemLanguage()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string language = currentCulture.TwoLetterISOLanguageName;
        return language;
    }


    //if (Utils.CheckForOptionsSave(gameManager.saveManager.optionsFilePath)){/*SAVE EXISTS, DO SOMETHING*/};
    public static bool CheckForOptionsSave(string filePath)
    {
        if (File.Exists(filePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static IEnumerator LoadUnloadScene(string sceneName, bool load)
    {        
        if (!Application.CanStreamedLevelBeLoaded(sceneName))// Check if the scene exists in the build settings
        {
            yield break;
        }
        var scene = SceneManager.GetSceneByName(sceneName);
        
        if (load)
        {
            if (scene.isLoaded)
            {
                yield break;
            }

            AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
            sceneAsync.allowSceneActivation = true;

            while (!sceneAsync.isDone)
            {
                yield return null;
            }
        }
        else
        {
            if (!scene.isLoaded)
            {
                yield break;
            }

            AsyncOperation sceneAsync = SceneManager.UnloadSceneAsync(sceneName);

            while (!sceneAsync.isDone)
            {
                yield return null;
            }
        }
    }

    // Utils.ActivateObject(object, true);
    public static void ActivateObject(GameObject gameObject, bool activate)
    {
        if (!gameObject)
        {
            return;
        }

        if (activate)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }            
        }
        else
        {
            if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Utils.DontDestroyObjectOnLoad(this.gameObject);
    public static void DontDestroyOnLoad(GameObject gameObject)
    {
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
    }

    // Utils.ClearMemory();
    public static void ClearMemory()
    {
        System.GC.Collect();
    }

    // Utils.TintUI(image, onColor);
    public static void TintUI(Image image, Color32 desiredColor)
    {
        image.color = desiredColor;
    }

    // Gives full control over constant-duration fade or proportional-duration fade based on starting alpha with scaled or unscaled delta time.
    // StartCoroutine(Utils.FadeImageAlpha(gameManager.scripts.uiManager.blackscreenImageComponent, 7f, false, false, false));
    public static IEnumerator FadeImageAlpha(Image image, float duration, bool overFullDuration, bool deltaTime, bool fadeIn)
    {
        if (image == null)
            yield break;

        Color color = image.color;
        float startAlpha = color.a;
        float endAlpha = fadeIn ? 1f : 0f;

        float alphaDifference = Mathf.Abs(endAlpha - startAlpha);
        float scaledDuration = overFullDuration ? duration : duration * alphaDifference;
        
        if (scaledDuration <= 0f)// Skip if no fading needed
        {
            color.a = endAlpha;
            image.color = color;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < scaledDuration)
        {
            elapsed += deltaTime ? Time.deltaTime : Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / scaledDuration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            image.color = color;
            yield return null;
        }
        
        color.a = endAlpha;
        image.color = color;
    }

    //Utils.CheckToggleValueModified(gameManager.scripts.uiManager.optionsUI.frameRateCapToggle, appliedProperties.appliedFrameRateCap, ref gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
    public static void CheckToggleValueModified(Toggle toggle, bool optionsAppliedBool, GameObject valueModifiedImage)
    {
        if (toggle && valueModifiedImage.activeSelf != (optionsAppliedBool != toggle.isOn))
        {
            valueModifiedImage.SetActive(optionsAppliedBool != toggle.isOn);
        }
    }

    // Utils.PopulateToggle(gameManager.scripts.uiManager.optionsUI.frameRateCapToggle, true, gameManager.scripts.optionsManager.OnFrameRateCapToggleChanged);
    public static void PopulateToggle(Toggle toggle, bool defaultState, UnityAction<bool> callback)
    {
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(callback);
        toggle.isOn = defaultState;
        callback.Invoke(toggle.isOn); // Immediately call it with the current value
    }

    // Utils.PopulateSlider(gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider, 10, gameManager.scripts.optionsManager.OnAutosavesSliderChanged);
    public static void PopulateSlider(Slider slider, float defaultValue, TMP_InputField sliderNumberInputField, string numberFormat, UnityAction<float> callback)
    {
        slider.onValueChanged.AddListener(callback);
        slider.value = defaultValue;// Default value
        callback.Invoke(slider.value);

        if (sliderNumberInputField)
        {
            sliderNumberInputField.text = slider.value.ToString(numberFormat);
        }        
    }

    // Utils.CheckSliderValueModified(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider, appliedFrameRateCapValue, gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
    public static void CheckSliderValueModified(Slider slider, float optionsAppliedFloat, GameObject valueModifiedImage)
    {
        if (slider && valueModifiedImage.activeSelf != (optionsAppliedFloat != slider.value))
        {
            valueModifiedImage.SetActive(optionsAppliedFloat != slider.value);
        }
    }

    // Utils.SliderScaleUI(gameManager.scripts.uiManager.optionsUI.gamepadLeftDeadzoneSlider, gameManager.scripts.uiManager.optionsUI.gamepadLeftDeadzoneEffect.transform);
    public static void SliderScaleUI(Slider slider, Transform UIToScale)
    {
        if (slider)
        {
            Vector3 newScale;
            newScale.z = 1;
            newScale.x = slider.value;
            newScale.y = slider.value;
            UIToScale.localScale = newScale;
        }
    }

    // Action sliderAction;
    // sliderAction = Utils.CreateSliderScaleUIAction(Slider, ObjectToScale.transform);
    // sliderAction();
    public static Action CreateSliderScaleUIAction(Slider slider, Transform UIToScale)
    {
        return () =>
        {
            if (slider)
            {
                Vector3 newScale;
                newScale.z = 1;
                newScale.x = slider.value;
                newScale.y = slider.value;
                UIToScale.localScale = newScale;
            }
        };
    }

    // Utils.PopulateDropdown(gameManager.scripts.uiManager.optionsUI.autosavesDropdown, new List<string> { "Off", "On" }, 1, gameManager.scripts.optionsManager.OnAutosavesChanged);
    public static void PopulateDropdown(TMP_Dropdown dropdown, List<string> list, int defaultValue, UnityAction<int> callback)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(list);
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener(callback);
        dropdown.value = defaultValue;
        callback.Invoke(dropdown.value); // Immediately call it with the current value to trigger OnValueChanged
    }

    // Utils.CheckDropdownValueModified(ref gameManager.scripts.uiManager.optionsUI.fogDropdown, appliedFogIndex, ref gameManager.scripts.uiManager.optionsUI.fogModifiedIcon);
    public static void CheckDropdownValueModified(TMP_Dropdown dropdown, int optionsAppliedInt, GameObject valueModifiedImage)
    {
        if (dropdown && valueModifiedImage.activeSelf != (optionsAppliedInt != dropdown.value))
        {
            valueModifiedImage.SetActive(optionsAppliedInt != dropdown.value);
        }
    }

    //Action sliderAction;
    //sliderAction = Utils.CreateSliderAction(SLIDER, () => FLOATPROPERTY, v => FLOATPROPERTY = v, -1, INPUTFIELD, "F0", null, null);
    //sliderAction = Utils.CreateSliderAction(SLIDER, () => 0f, _ => { }, 0, null, null, MIXER, MIXERPROPERTY);
    //sliderAction();
    public static Action CreateSliderAction(Slider slider, Func<float> getter, Action<float> setter, float change, TMP_InputField text, string desiredNumberFormat, AudioMixer mainMixer, string mixerProperty)
    {
        return () =>
        {
            const float MIN_DB = -80f;
            const float MAX_DB = 5f;
            float dB;
            string desiredText;

            if (slider)
            {
                slider.value = Mathf.Clamp(slider.value + change, slider.minValue, slider.maxValue);
                
                float roundedValue;
                if (!string.IsNullOrEmpty(desiredNumberFormat))
                {
                    if (float.TryParse(slider.value.ToString(desiredNumberFormat), out float parsed))
                    {
                        roundedValue = parsed;
                    }                        
                    else
                    {
                        roundedValue = Mathf.Round(slider.value * 100f) / 100f; // fallback
                    }                        
                }
                else if (slider.wholeNumbers)
                {
                    roundedValue = Mathf.Round(slider.value);
                }
                else
                {
                    roundedValue = Mathf.Round(slider.value * 10f) / 10f;
                }

                slider.value = roundedValue;

                if (mainMixer && mixerProperty != null)
                {                    
                    if (desiredNumberFormat != null && text)
                    {
                        desiredText = roundedValue.ToString(desiredNumberFormat) + "<color=red>%</color>";
                        text.text = desiredText;
                        desiredText = null;
                    }
                    else if (text)
                    {
                        desiredText = roundedValue.ToString("F2") + "<color=red>%</color>";
                        text.text = desiredText;                        
                        desiredText = null;
                    }
                    dB = Mathf.Lerp(MIN_DB, MAX_DB, roundedValue / slider.maxValue);
                    mainMixer.SetFloat(mixerProperty, dB);
                }
                else if (text)
                {
                    if (desiredNumberFormat != null)
                    {
                        text.text = roundedValue.ToString(desiredNumberFormat);
                    }
                    else
                    {
                        if (slider.wholeNumbers)
                        {
                            text.text = roundedValue.ToString("F0");
                        }
                        else
                        {
                            // ROUND SLIDER.VALUE TO F1
                            text.text = roundedValue.ToString("F1");
                        }
                    }
                }

                setter(slider.value);
            }
        };
    }



    // -----------------------------------------------------
    public static Ray CursorRay3D(Camera camera = null)
    {
        if (!camera)
        {
            camera = Camera.main;
        }

        if (newInputSystem && legacyInputSystem || newInputSystem && !legacyInputSystem)
        {
            return camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        }
        else
        {
            return camera.ScreenPointToRay(Input.mousePosition);
        }        
    }
}
