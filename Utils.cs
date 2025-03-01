using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Jobs;
using System.Globalization;// Works on Steamdeck
using System.IO;

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


    // int uiLayer = LayerMask.NameToLayer(Strings.uiLayerName);
    // if (Utils.RayCastIn3DLayerSpecific(uiLayer, out mouseWorldPosition))
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

    // if (Utils.RayCastIn3DTagSpecific(uiLayer, Strings.uiLayerName, out hitPoint))
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

    // uiLayer = LayerMask.NameToLayer(Strings.uiLayerName);
    // if (Utils.RayCastIn3DLayerAndTagSpecific(uiLayer, Strings.uiLayerName, out hitPoint))
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

    // OBJECT REFERENCE = Utils.GetCursorObjectAndTransform(out TRANSFORM REFERENCE);
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

    // mouseCursorObjectTransform = Utils.GetCursorObjectTransform();
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

    // if(Utils.GetPreferredSystemLanguage() == "en"){// DO SOMETHING}
    public static string GetPreferredSystemLanguage()
    {
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        string language = currentCulture.TwoLetterISOLanguageName;
        return language;
    }

    // Utils.LocalizeText(gameManager, ref languageClassName, ref splashTitleText, "VARIABLENAMEHERE");
    /*
    public static void LocalizeText(GameManager gameManager, ref string languageClassName, ref TextMeshProUGUI desiredTextField, string fieldName, TMP_FontAsset defaultFontAsset, TMP_FontAsset chineseFontAsset, TMP_FontAsset japaneseFontAsset)
    {
        if (desiredTextField)
        {
            languageClassName = null;

            if (gameManager.scriptReferences.options.languageEnglish)
            {
                languageClassName = "GameStringsEnglish";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageFrench)
            {
                languageClassName = "GameStringsFrench";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageGerman)
            {
                languageClassName = "GameStringsGerman";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageItalian)
            {
                languageClassName = "GameStringsItalian";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageSpanish)
            {
                languageClassName = "GameStringsSpanish";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageAmerican)
            {
                languageClassName = "GameStringsAmerican";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageRussian)
            {
                languageClassName = "GameStringsRussian";
                desiredTextField.fontStyle = FontStyles.Bold;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageChinese)
            {
                languageClassName = "GameStringsChinese";
                desiredTextField.fontStyle = FontStyles.Bold;

                if (chineseFontAsset)
                {
                    if (desiredTextField.font != chineseFontAsset)
                    {
                        desiredTextField.font = chineseFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageJapanese)
            {
                languageClassName = "GameStringsJapanese";
                desiredTextField.fontStyle = FontStyles.Bold;

                if (japaneseFontAsset)
                {
                    if (desiredTextField.font != japaneseFontAsset)
                    {
                        desiredTextField.font = japaneseFontAsset;
                    }
                }
            }
            else if (gameManager.scriptReferences.options.languageBrazilian)
            {
                languageClassName = "GameStringsBrazilian";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }
            else
            {
                languageClassName = "GameStringsEnglish";
                desiredTextField.fontStyle = FontStyles.Normal;

                if (defaultFontAsset)
                {
                    if (desiredTextField.font != defaultFontAsset)
                    {
                        desiredTextField.font = defaultFontAsset;
                    }
                }
            }

            if (languageClassName != null)
            {
                var gameStringsType = typeof(GameStrings);
                var languageType = gameStringsType.GetNestedType(languageClassName);

                if (languageType != null)
                {
                    var field = languageType.GetField(fieldName);
                    if (field != null)
                    {
                        string localizedText = field.GetValue(null) as string;
                        desiredTextField.text = localizedText;
                    }
                    else
                    {
                        Debug.LogError($"Field '{fieldName}' not found in '{languageClassName}'");
                    }
                }
                else
                {
                    Debug.LogError($"Language class '{languageClassName}' not found");
                }
            }
        }
        else
        {
            //Debug.Log("desiredTextField not active...");
        }
    }
    */

    /*if (Utils.CheckForOptionsSave(gameManager.saveManager.optionsFilePath))
     * {
     *    //SAVE EXISTS, DO SOMETHING
     * }*/
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

    // Utils.ClearMemoryThreaded();
    public static void ClearMemoryThreaded()
    {
        var memoryCleanupJob = new MemoryCleanupJob();
        JobHandle memoryCleanupHandle = memoryCleanupJob.Schedule();
    }

    public struct MemoryCleanupJob : IJob
    {
        public void Execute()
        {
            System.GC.Collect();
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
