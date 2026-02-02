using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;// Works on Steamdeck
using System.IO;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

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

    #region Platform
    //Debug.Log("Running on Windows: " + Utils.Windows);
    /// <summary>
    /// Returns true if detected platform is Windows.
    /// </summary>
    public static bool Windows { get; private set; } = false;
    /// <summary>
    /// Returns true if detected platform is Steamdeck.
    /// </summary>
    public static bool Steamdeck { get; private set; } = false;
    /// <summary>
    /// Returns true if detected platform is Playstation5.
    /// </summary>
    public static bool Playstation5 { get; private set; } = false;
    /// <summary>
    /// Returns true if detected platform is XBox.
    /// </summary>
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

    /// <summary>
    /// Gets the preferred system language from the device.
    /// <code>
    /// if(Utils.GetPreferredSystemLanguage() == "en"){// SYSTEM USING ENGLISH, DO SOMETHING};
    /// </code>
    /// </summary>
    /// <returns>System Language</returns>
    public static string GetPreferredSystemLanguage()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string language = currentCulture.TwoLetterISOLanguageName;
        return language;
    }

    /// <summary>
    /// Checks if a file exists in the path provided.
    /// <code>
    /// if (Utils.CheckForFile(gameManager.saveManager.optionsFilePath)){/*OPTIONS SAVE EXISTS, DO SOMETHING*/};
    /// </code>
    /// </summary>
    /// <param name="filePath">Desired FilePath to check</param>
    /// <returns>True is file exists</returns>
    //
    public static bool CheckForFile(string filePath)
    {
        if (File.Exists(filePath)) return true;
        else return false;
    }
    #endregion

    #region Times
    public static WaitForSeconds wfsZeroOne = new WaitForSeconds(0.1f);
    public static WaitForSeconds wfsZeroTwo = new WaitForSeconds(0.2f);
    public static WaitForSeconds wfsZeroThree = new WaitForSeconds(0.3f);
    public static WaitForSeconds wfsZeroFour = new WaitForSeconds(0.4f);
    public static WaitForSeconds wfsZeroFive = new WaitForSeconds(0.5f);
    public static WaitForSeconds wfsZeroSix = new WaitForSeconds(0.6f);
    public static WaitForSeconds wfsZeroSeven = new WaitForSeconds(0.7f);
    public static WaitForSeconds wfsZeroEight = new WaitForSeconds(0.8f);
    public static WaitForSeconds wfsZeroNine = new WaitForSeconds(0.9f);
    public static WaitForSeconds wfsOne = new WaitForSeconds(1f);
    public static WaitForSeconds wfsTwo = new WaitForSeconds(2f);
    public static WaitForSeconds wfsThree = new WaitForSeconds(3f);
    public static WaitForSeconds wfsFour = new WaitForSeconds(4f);
    public static WaitForSeconds wfsFive = new WaitForSeconds(5f);
    public static WaitForSeconds wfsSix = new WaitForSeconds(6f);
    public static WaitForSeconds wfsSeven = new WaitForSeconds(7f);
    public static WaitForSeconds wfsEight = new WaitForSeconds(8f);
    public static WaitForSeconds wfsNine = new WaitForSeconds(9f);
    public static WaitForSeconds wfsTen = new WaitForSeconds(10f);
    #endregion

    #region Cursor

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
    #endregion

    #region Raycasts
    /// <summary>
    /// Performs a 3D raycast that respects a LayerMask, either from a custom origin/direction or from the cursor.
    /// </summary>
    /// <param name="hitPoint">World position of the hit, zero if nothing hit</param>
    /// <param name="hitLayers">LayerMask to test against</param>
    /// <param name="origin">Optional ray origin; if null, uses CursorRay3D()</param>
    /// <param name="direction">Optional ray direction; required if origin is specified</param>
    /// <param name="maxDistance">Optional max distance (defaults to Mathf.Infinity)</param>
    /// <returns>True if something was hit</returns>
    public static bool RayCastLayered(out Vector3 hitPoint, LayerMask hitLayers, Vector3? origin = null, Vector3? direction = null, float maxDistance = Mathf.Infinity, Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay())
        {
            hitPoint = Vector3.zero;
            return false;
        }

        Ray ray;

        if (origin.HasValue && direction.HasValue) ray = new Ray(origin.Value, direction.Value);
        else ray = CursorRay3D(null, mouseScreenPosition);

        if (ray.direction == Vector3.zero)
        {
            hitPoint = Vector3.zero;
            return false;
        }

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
        {
            hitPoint = hit.point;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that respects a LayerMask int.
    /// <code>
    /// int uiLayer = LayerMask.NameToLayer("Layer Name");
    /// Vector3 hitWorldPosition; 
    /// if (Utils.RayCastIn3DLayerSpecific(uiLayer,hitWorldPosition)){/*DO SOMETHING*/}
    /// </code>
    /// </summary>
    /// <param name="desiredLayer">Pass in a layer name using LayerMask.NameToLayer</param>
    /// <param name="hitPoint">Store the result using a Vector3</param>
    /// <returns>True if something was hit.</returns>
    public static bool RayCastIn3DLayerSpecific(int desiredLayer, out Vector3 hitPoint, Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay())
        {
            hitPoint = Vector3.zero;
            return false;
        }

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero)
        {
            hitPoint = Vector3.zero;
            return false;
        }

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

    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that respects a LayerMask int.
    /// <code>
    /// Vector3 hitWorldPosition;
    /// if (Utils.RayCastIn3DTagSpecific("Tag Name", hitWorldPosition))
    /// </code>
    /// </summary>
    /// <param name="tag">Tag name string</param>
    /// <param name="hitPoint">Vector3 hitWorldPosition; - to store the result</param>
    /// <returns>True if something was hit.</returns>
    public static bool RayCastIn3DTagSpecific(string tag, out Vector3 hitPoint, Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay())
        {
            hitPoint = Vector3.zero;
            return false;
        }

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero)
        {
            hitPoint = Vector3.zero;
            return false;
        }

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


    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that respects a LayerMask int and Tag string.
    /// <code>
    /// Vector3 hitWorldPosition;
    /// layer = LayerMask.NameToLayer("Layer Name");
    /// if (Utils.RayCastIn3DLayerAndTagSpecific(layer, "Tag Name", out hitWorldPosition))
    /// </code>
    /// </summary>
    /// <param name="desiredLayer">Layer int</param>
    /// <param name="tag">Tag name string</param>
    /// <param name="hitPoint">Position of the hit and result</param>
    /// <returns>True if something was hit.</returns>
    public static bool RayCastIn3DLayerAndTagSpecific(int desiredLayer, string tag, out Vector3 hitPoint, Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay())
        {
            hitPoint = Vector3.zero;
            return false;
        }

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero)
        {
            hitPoint = Vector3.zero;
            return false;
        }

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


    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that gets the first object hit and it's transform.
    /// <code>
    /// Transform targetTransform;
    /// GameObject targetObject;
    /// targetObject = Utils.GetCursorObjectAndTransform(out targetTransform);
    /// </code>
    /// </summary>
    /// <param name="transform">Transform reference to store the result</param>
    /// <param name="mouseScreenPosition">Optional mouse screen position</param>
    /// <returns>null if nothing was hit.</returns>
    public static GameObject GetCursorObjectAndTransform(out Transform transform, Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay())
        {
            transform = null;
            return null;
        }

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero)
        {
            transform = null;
            return null;
        }

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

    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that gets the Transform of the first object hit.
    /// <code>
    /// Transform mouseCursorObjectTransform = Utils.GetCursorObjectTransform();
    /// </code>
    /// </summary>
    /// <param name="mouseScreenPosition">Optional mouse screen position</param>
    /// <returns>null if nothing was hit.</returns>
    public static Transform GetCursorObjectTransform(Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay()) return null;

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero) return null;
        return Physics.Raycast(ray, out RaycastHit hit) ? hit.transform : null;
    }

    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that gets the Transform of the first object hit.
    /// <code>
    /// GameObject mouseCursorObject = Utils.GetCursorGameObject();
    /// </code>
    /// </summary>
    /// <param name="mouseScreenPosition">Optional mouse screen position</param>
    /// <returns>null if nothing was hit.</returns>
    public static GameObject GetCursorGameObject(Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay()) return null;

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero) return null;
        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity)) return cursorObject.transform.gameObject;
        else return null;
    }

    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that gets the GameObject of the first object hit that is not on the default layer.
    /// <code>
    /// GameObject firstCursorObject = Utils.CursorFirstObjectHitNotDefaultLayer();
    /// </code>
    /// </summary>
    /// <param name="mouseScreenPosition">Optional mouse screen position</param>
    /// <returns>null if nothing was hit.</returns>
    public const string defaultLayer = "Default";
    public static GameObject CursorFirstObjectHitNotDefaultLayer(Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay()) return null;

        var layerMask = Physics.DefaultRaycastLayers & ~LayerMask.GetMask(defaultLayer);

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero) return null;

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity, layerMask)) return cursorObject.transform.gameObject;
        else return null;
    }

    /// <summary>
    /// Performs a 3D raycast from CURSOR POSITION that gets the GameObject of the first object hit.
    /// <code>
    /// GameObject firstCursorObject = Utils.CursorFirstObjectHit();
    /// </code>
    /// </summary>
    /// <param name="mouseScreenPosition">Optional mouse screen position</param>
    /// <returns>null if nothing was hit.</returns>
    public static GameObject CursorFirstObjectHit(Vector3? mouseScreenPosition = null)
    {
        if (!CanUseCursorRay()) return null;

        var layerMask = Physics.DefaultRaycastLayers;

        Ray ray = CursorRay3D(null, mouseScreenPosition);
        if (ray.direction == Vector3.zero) return null;

        if (Physics.Raycast(ray, out RaycastHit cursorObject, Mathf.Infinity, layerMask)) return cursorObject.transform.gameObject;
        else return null;
    }
    #endregion

    #region Scene
    public static IEnumerator LoadUnloadScene(string sceneName, int sceneIndex, bool load)
    {
        bool useIndex = string.IsNullOrEmpty(sceneName);

        // Validate scene existence
        if (useIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings) yield break;
        }
        else
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName)) yield break;
        }

        Scene scene = useIndex ? SceneManager.GetSceneByBuildIndex(sceneIndex) : SceneManager.GetSceneByName(sceneName);

        if (load)
        {
            if (scene.isLoaded) yield break;
            AsyncOperation op = useIndex ? SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive) : SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = true;
            while (!op.isDone) yield return null;
        }
        else
        {
            if (!scene.isLoaded) yield break;
            AsyncOperation op = SceneManager.UnloadSceneAsync(scene);
            while (!op.isDone) yield return null;
        }
    }
    #endregion

    #region Object
    // Utils.ActivateObject(object, true);
    public static void ActivateObject(GameObject gameObject, bool activate)
    {
        if (!gameObject) return;

        if (activate)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }
        else
        {
            if(gameObject.activeSelf) gameObject.SetActive(false);
        }
    }

    // Utils.DontDestroyObjectOnLoad(this.gameObject);
    public static void DontDestroyOnLoad(GameObject gameObject)
    {
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
    }

    public static void EnableActionMap(InputActionMap inputActionMap, bool enable)
    {
        if (inputActionMap != null)
        {
            if (enable) inputActionMap.Enable();
            else inputActionMap.Disable();
        }
    }

    //Enemy enemy = null;
    //if (enemy == null) Utils.TryGetComponentInParent(hit, out enemy, true); // 2. Parent chain    
    public static bool TryGetComponentInParent<T>(Component start, out T result, bool includeInactive = true) where T : Component
    {
        result = null;

        Transform t = start.transform.parent; // skip self, handled by caller
        while (t != null)
        {
            if (includeInactive || t.gameObject.activeInHierarchy)
            {
                if (t.TryGetComponent<T>(out result)) return true;
            }
            t = t.parent;
        }

        return false;
    }

    //Enemy enemy = null;
    //if (enemy == null) Utils.TryGetComponentInChildren(hit, out enemy, true);// 3. Child chain
    public static bool TryGetComponentInChildren<T>(Component start, out T result, bool includeInactive = true) where T : Component
    {
        result = null;
        foreach (Transform child in start.transform)
        {
            if (!includeInactive && !child.gameObject.activeInHierarchy) continue;
            if (child.TryGetComponent<T>(out result)) return true;
            if (TryGetComponentInChildren(child, out result, includeInactive)) return true;
        }
        return false;
    }
    #endregion

    #region UI
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
    #endregion

    #region Options
    //Utils.CheckToggleValueModified(gameManager.scripts.uiManager.optionsUI.frameRateCapToggle, appliedProperties.appliedFrameRateCap, ref gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
    public static void CheckToggleValueModified(Toggle toggle, bool optionsAppliedBool, GameObject valueModifiedImage)
    {
        if (toggle && valueModifiedImage.activeSelf != (optionsAppliedBool != toggle.isOn)) valueModifiedImage.SetActive(optionsAppliedBool != toggle.isOn);
    }

    // Utils.PopulateToggle(gameManager.scripts.uiManager.optionsUI.frameRateCapToggle, true, gameManager.scripts.optionsManager.OnFrameRateCapToggleChanged);
    public static void PopulateToggle(Toggle toggle, bool defaultState, UnityAction<bool> callback)
    {
        if (!toggle) return;

        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(callback);
        toggle.isOn = defaultState;
        callback.Invoke(toggle.isOn); // Immediately call it with the current value
    }

    // Utils.PopulateSlider(gameManager.scripts.uiManager.optionsUI.maximumAutosavesSlider, 10, gameManager.scripts.optionsManager.OnAutosavesSliderChanged);
    public static void PopulateSlider(Slider slider, float defaultValue, TMP_InputField sliderNumberInputField, string numberFormat, UnityAction<float> callback)
    {
        if (!slider) return;
        slider.onValueChanged.AddListener(callback);
        slider.value = defaultValue;// Default value
        callback.Invoke(slider.value);

        if (sliderNumberInputField) sliderNumberInputField.text = slider.value.ToString(numberFormat);
    }

    // Utils.CheckSliderValueModified(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider, appliedFrameRateCapValue, gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
    public static void CheckSliderValueModified(Slider slider, float optionsAppliedFloat, GameObject valueModifiedImage)
    {
        if (slider && valueModifiedImage.activeSelf != (optionsAppliedFloat != slider.value)) valueModifiedImage.SetActive(optionsAppliedFloat != slider.value);
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
        if (!dropdown) return;
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
        if (dropdown && valueModifiedImage.activeSelf != (optionsAppliedInt != dropdown.value)) valueModifiedImage.SetActive(optionsAppliedInt != dropdown.value);
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

            if (slider == null) return;

            // Apply change and clamp
            slider.value = Mathf.Clamp(slider.value + change, slider.minValue, slider.maxValue);

            float roundedValue;

            // Use format if given, with fallback
            if (!string.IsNullOrEmpty(desiredNumberFormat))
            {
                try
                {
                    roundedValue = float.Parse(slider.value.ToString(desiredNumberFormat));
                }
                catch
                {
                    roundedValue = Mathf.Round(slider.value * 100f) / 100f;
                }
            }
            else if (slider.wholeNumbers) roundedValue = Mathf.Round(slider.value);
            else roundedValue = Mathf.Round(slider.value * 10f) / 10f;

            slider.value = roundedValue;

            // Audio mixer path
            if (mainMixer && !string.IsNullOrEmpty(mixerProperty))
            {
                float dB = Mathf.Lerp(MIN_DB, MAX_DB, roundedValue / slider.maxValue);
                mainMixer.SetFloat(mixerProperty, dB);

                if (text)
                {
                    string formattedValue = desiredNumberFormat != null ? roundedValue.ToString(desiredNumberFormat) : roundedValue.ToString("F2");
                    text.text = $"{formattedValue}";
                    //Debug.Log($"[Slider] Value: {roundedValue}, Format: {desiredNumberFormat}, Output: {roundedValue.ToString(desiredNumberFormat)}");
                }
            }
            // Regular float display
            else if (text)
            {
                string format = slider.wholeNumbers ? "F0" : "F1";
                text.text = !string.IsNullOrEmpty(desiredNumberFormat) ? roundedValue.ToString(desiredNumberFormat) : roundedValue.ToString(format);
            }

            setter(slider.value);
        };
    }
    #endregion

    #region VFX
    // StartCoroutine(Utils.WaitForVFXToFinish(VFX, .gameObject, new WaitForSeconds(0.1f)));
    public static IEnumerator WaitForVFXToFinish(VisualEffect vfx, GameObject objectToDestroy, WaitForSeconds waitTime)
    {
        if (vfx == null || objectToDestroy == null) yield break;
        yield return waitTime;
        if (objectToDestroy != null) UnityEngine.Object.Destroy(objectToDestroy);
    }
    #endregion

    #region editor
    /// <summary>
    /// Displays error logs if in editor or build is development - 0: Log / 1: Warning / 2: Error / 3: Assertion / 4: Exception
    /// <code>
    /// Log("Log.", 0);
    /// Log("Warning.", 1);
    /// Log("Error.", 2);
    /// Log("Assertion.", 3);
    /// Log("Exception.", 4);
    /// </code>
    /// </summary>
    public static void Log(string message, int type = 0, UnityEngine.Object context = null, System.Exception exception = null)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        switch (type)
        {
            case 0: // Info
                Debug.Log(message, context);
                break;

            case 1: // Warning
                Debug.LogWarning(message, context);
                break;

            case 2: // Error
                Debug.LogError(message, context);
                break;

            case 3: // Assertion
                Debug.LogAssertion(message, context);
                break;

            case 4: // Exception
                if (exception != null) Debug.LogException(exception, context);
                else Debug.LogError(message, context);
                break;
        }
#endif
    }
    #endregion

    #region Screenshot
    /// <summary>
    /// <para>Screenshot resolutions (preset index):</para>
    /// 0:32x32
    /// 1:64x64
    /// 2:128x128
    /// 3:256x256
    /// 4:512x512
    /// 5:1024x1024
    /// 6:2048x2048
    /// 7:4096x4096 (max supported)</para>
    /// </summary>
    /// <param name="camera">Camera to capture from. If null, uses Camera.main.</param>
    /// <param name="resolution">Width/height. If zero, presetIndex is used.</param>
    /// <param name="presetIndex">0-7 for preset resolutions.</param>
    /// <param name="includeAlpha">If true, captures alpha channel, otherwise ignores it to save memory.</param>
    public static IEnumerator TakeScreenshot(Camera camera, Vector2 resolution, int presetIndex = 7, bool includeAlpha = false)
    {
        Log("Screenshot coroutine started.", 0);

        if (!camera) camera = Camera.main;
        if (!camera)
        {
            //Log("TakeScreenshot failed: No camera provided and Camera.main is null.", 0);
            yield break;
        }
        //Log($"Using camera: {camera.name}", 0);

        int[] presets = { 32, 64, 128, 256, 512, 1024, 2048, 4096 };
        int width = (int)resolution.x;
        int height = (int)resolution.y;

        if (width == 0 || height == 0)
        {
            presetIndex = Mathf.Clamp(presetIndex, 0, presets.Length - 1);
            width = height = presets[presetIndex];
        }

        int maxTexSize = Mathf.Min(SystemInfo.maxTextureSize, 4096);
        width = Mathf.Clamp(width, 1, maxTexSize);
        height = Mathf.Clamp(height, 1, maxTexSize);
        //Log($"Screenshot resolution: {width}x{height}", 0);

        //string folder = Path.Combine(Application.persistentDataPath, "Screenshots");
        string folder = Path.Combine(Application.persistentDataPath, "PixelVerseScreenshots");
        Directory.CreateDirectory(folder);
        string fileName = $"{Application.productName}_Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        string fullPath = Path.Combine(folder, fileName);
        //Log($"Screenshot path: {fullPath}", 0);

        TextureFormat texFormat = includeAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24;
        //Log($"TextureFormat: {texFormat}", 0);

        // --- Create RenderTexture ---
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = rt;
        //Log("RenderTexture created and assigned to camera.", 0);

        yield return new WaitForEndOfFrame();
        //Log("End of frame reached.", 0);

        camera.Render();
        //Log("Camera rendered.", 0);

        // --- Read pixels into temp texture ---
        RenderTexture.active = rt;
        Texture2D tempTex = new Texture2D(width, height, texFormat, false);
        tempTex.ReadPixels(camera.pixelRect, 0, 0);
        tempTex.Apply();
        //Log("Pixels read into temporary Texture2D.", 0);

        // --- Spread copy into final texture over frames (no flip) ---
        Texture2D finalTex = new Texture2D(width, height, texFormat, false);
        Color32[] sourcePixels = tempTex.GetPixels32();
        int totalPixels = sourcePixels.Length;
        int pixelsPerFrame = 65536 * 3; // adjust for smoother spreading

        Log("Starting frame-spread copy.", 0);
        for (int i = 0; i < totalPixels; i += pixelsPerFrame)
        {
            int chunk = Mathf.Min(pixelsPerFrame, totalPixels - i);
            for (int j = 0; j < chunk; j++)
            {
                int index = i + j;
                finalTex.SetPixel(index % width, index / width, sourcePixels[index]);
            }
            yield return null; // spread over frames
        }

        finalTex.Apply();
        //Log("Final texture applied.", 0);

        // --- Save PNG ---
        try
        {
            File.WriteAllBytes(fullPath, finalTex.EncodeToPNG());
            Log($"Screenshot saved: {fullPath}", 0);
        }
        catch (System.Exception e)
        {
            Log("Failed to write PNG: " + e, 2, null, e);
        }

        // --- Cleanup ---
        camera.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.Destroy(rt);
        UnityEngine.Object.Destroy(tempTex);
        UnityEngine.Object.Destroy(finalTex);
        //Log("Cleanup done.", 0);
    }

    #endregion


    // -----------------------------------------------------
    private static bool CanUseCursorRay()
    {
        // App not focused (alt-tab, Windows key, etc.)
        if (!Application.isFocused) return false;
        return true;
    }

    public static Ray CursorRay3D(Camera camera = null, Vector3? mouseScreenPosition = null)
    {
        if (camera == null) camera = Camera.main;
        // HARD FAILSAFE
        if (camera == null) return new Ray(Vector3.zero, Vector3.forward);
        // APPLICATION / FOCUS GUARD
        if (!Application.isFocused || Screen.width <= 0 || Screen.height <= 0) return new Ray(Vector3.zero, Vector3.zero);

        Vector3 screenPos;
        if (mouseScreenPosition.HasValue) screenPos = mouseScreenPosition.Value;
        else if (newInputSystem)// New Input System
        {
            try
            {
                if (Mouse.current != null) screenPos = Mouse.current.position.ReadValue();
                else screenPos = Input.mousePosition;
            }
            catch
            {
                return new Ray(Vector3.zero, Vector3.zero);
            }
        }
        else
        {
            if (legacyInputSystem) screenPos = Input.mousePosition;
            else screenPos = Input.mousePosition;
        }

        // SCREEN BOUNDS GUARD
        if (screenPos.x < 0 || screenPos.y < 0 || screenPos.x > Screen.width || screenPos.y > Screen.height) return new Ray(Vector3.zero, Vector3.zero);
        return camera.ScreenPointToRay(screenPos);
    }
}
