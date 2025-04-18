using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Jobs;
using System.Globalization;// Works on Steamdeck
using System.IO;
using System;
using TMPro;

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


    //if (Utils.CheckForOptionsSave(gameManager.saveManager.optionsFilePath)){//SAVE EXISTS, DO SOMETHING};
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

    // Utils.ActivateObject(object, true);
    public static void ActivateObject(GameObject gameObject, bool activate)
    {
        if (activate)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            if (gameObject.activeSelf)
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

    // Utils.CheckToggleValueModified(gameManager.scripts.uiManager.optionsUI.frameRateCapToggle, appliedFrameRateCap, gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
    public static void CheckToggleValueModified(ref Toggle toggle, bool optionsAppliedBool, ref GameObject valueModifiedImage)
    {
        if (toggle)
        {
            if (optionsAppliedBool != toggle.isOn)
            {
                if (!valueModifiedImage.activeSelf)
                {
                    valueModifiedImage.SetActive(true);
                }
            }
            else
            {
                if (valueModifiedImage.activeSelf)
                {
                    valueModifiedImage.SetActive(false);
                }
            }
        }
    }

    // Utils.CheckSliderValueModified(gameManager.scripts.uiManager.optionsUI.frameRateCapSlider, appliedFrameRateCapValue, gameManager.scripts.uiManager.optionsUI.framerateCapModifiedIcon);
    public static void CheckSliderValueModified(ref Slider slider, float optionsAppliedFloat, ref GameObject valueModifiedImage)
    {
        if (slider)
        {
            if (optionsAppliedFloat != slider.value)
            {
                if (!valueModifiedImage.activeSelf)
                {
                    valueModifiedImage.SetActive(true);
                }
            }
            else
            {
                if (valueModifiedImage.activeSelf)
                {
                    valueModifiedImage.SetActive(false);
                }
            }
        }
    }

    public static void CheckDropdownValueModified(ref TMP_Dropdown dropdown, int optionsAppliedInt, ref GameObject valueModifiedImage)
    {
        if (dropdown)
        {
            if (optionsAppliedInt != dropdown.value)
            {
                if (!valueModifiedImage.activeSelf)
                {
                    valueModifiedImage.SetActive(true);
                }
            }
            else
            {
                if (valueModifiedImage.activeSelf)
                {
                    valueModifiedImage.SetActive(false);
                }
            }
        }
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
