using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class OptionsData
{
    public bool appliedAutosaves;
    public int appliedAutosavesAmount;
    public int appliedQuicksavesAmount;
    public bool appliedGore;

    public bool appliedLanguageEnglish;
    public bool appliedLanguageFrench;
    public bool appliedLanguageGerman;
    public bool appliedLanguageItalian;
    public bool appliedLanguageSpanish;
    public bool appliedLanguageAmerican;
    public bool appliedLanguageRussian;
    public bool appliedLanguageChinese;
    public bool appliedLanguageJapanese;
    public bool appliedLanguageBrazilian;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    public Options options;
    public string optionsFilePath;

    // Semaphore to ensure only one async operation runs at a time
    private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.saveManager = this;

        if (gameManager.options)
        {
            if (options != gameManager.options)
            {
                options = gameManager.options;
            }
        }

        optionsFilePath = Path.Combine(Application.persistentDataPath, "TheLastDebugOptions.json");
    }

    public async Task SaveOptionsAsync()
    {
        // Wait for access to the semaphore
        await semaphore.WaitAsync();
        try
        {
            if (!gameManager.loadingOptionsSave)
            {
                gameManager.savingOptions = true;

                OptionsData optionsData = new OptionsData
                {
                    appliedAutosaves = gameManager.options.appliedAutosaves,
                    appliedAutosavesAmount = gameManager.options.appliedAutosavesAmount,
                    appliedQuicksavesAmount = gameManager.options.appliedQuicksavesAmount,
                    appliedGore = gameManager.options.appliedGore,
                    appliedLanguageEnglish = gameManager.options.appliedLanguageEnglish,
                    appliedLanguageFrench = gameManager.options.appliedLanguageFrench,
                    appliedLanguageGerman = gameManager.options.appliedLanguageGerman,
                    appliedLanguageItalian = gameManager.options.appliedLanguageItalian,
                    appliedLanguageSpanish = gameManager.options.appliedLanguageSpanish,
                    appliedLanguageAmerican = gameManager.options.appliedLanguageAmerican,
                    appliedLanguageRussian = gameManager.options.appliedLanguageRussian,
                    appliedLanguageChinese = gameManager.options.appliedLanguageChinese,
                    appliedLanguageJapanese = gameManager.options.appliedLanguageJapanese,
                    appliedLanguageBrazilian = gameManager.options.appliedLanguageBrazilian,
                };

                string json = JsonUtility.ToJson(optionsData, true);

                try
                {
                    await File.WriteAllTextAsync(optionsFilePath, json); // Asynchronous file writing
                    Debug.Log($"Options saved asynchronously to {optionsFilePath}");
                }
                catch (IOException ex)
                {
                    Debug.LogError($"Failed to save options: {ex.Message}");
                }
                finally
                {
                    gameManager.savingOptions = false;
                }
            }
            else
            {
                Debug.Log("Attempted to SAVE whilst LOADING");
            }
        }
        finally
        {
            // Release the semaphore
            semaphore.Release();
        }
    }

    public async Task LoadOptionsAsync()
    {
        // Wait for access to the semaphore
        await semaphore.WaitAsync();
        try
        {
            if (!gameManager.savingOptions)
            {
                if (File.Exists(optionsFilePath))
                {
                    gameManager.loadingOptionsSave = true;

                    try
                    {
                        string json = await File.ReadAllTextAsync(optionsFilePath); // Asynchronous file reading
                        OptionsData optionsData = JsonUtility.FromJson<OptionsData>(json);

                        gameManager.options.appliedAutosaves = optionsData.appliedAutosaves;
                        gameManager.options.appliedAutosavesAmount = optionsData.appliedAutosavesAmount;
                        gameManager.options.appliedQuicksavesAmount = optionsData.appliedQuicksavesAmount;
                        gameManager.options.appliedGore = optionsData.appliedGore;

                        if (optionsData.appliedLanguageEnglish) gameManager.options.ApplyEnglishLanguage();
                        if (optionsData.appliedLanguageFrench) gameManager.options.ApplyFrenchLanguage();
                        if (optionsData.appliedLanguageGerman) gameManager.options.ApplyGermanLanguage();
                        if (optionsData.appliedLanguageItalian) gameManager.options.ApplyItalianLanguage();
                        if (optionsData.appliedLanguageSpanish) gameManager.options.ApplySpanishLanguage();
                        if (optionsData.appliedLanguageAmerican) gameManager.options.ApplyAmericanLanguage();
                        if (optionsData.appliedLanguageRussian) gameManager.options.ApplyRussianLanguage();
                        if (optionsData.appliedLanguageChinese) gameManager.options.ApplyChineseLanguage();
                        if (optionsData.appliedLanguageJapanese) gameManager.options.ApplyJapaneseLanguage();
                        if (optionsData.appliedLanguageBrazilian) gameManager.options.ApplyBrazilianLanguage();

                        gameManager.options.OptionsRevertModifications();
                        gameManager.localizeLanguage = true;
                        Debug.Log("Options loaded asynchronously.");
                    }
                    catch (IOException ex)
                    {
                        Debug.LogError($"Failed to load options: {ex.Message}");
                    }
                    finally
                    {
                        gameManager.loadingOptionsSave = false;
                    }
                }
                else
                {
                    Debug.LogWarning("No savefile exists to load!");
                }
            }
            else
            {
                Debug.Log("tried to LOAD whilst SAVING");
            }
        }
        finally
        {
            // Release the semaphore
            semaphore.Release();
        }
    }
}
