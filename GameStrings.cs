using UnityEngine;

public static class GameStrings
{
    #region English
    public static class GameStringsEnglish
    {
        public static bool available = true;
        public static string gameTitle = "The Last Debug";

        public static string splashSelectLanguageText = "Select Language";
        public static string splashSelectedLanguageText = "English";
        public static string splashSelectLanguageNoticeText = "This can be changed in the Accessibility section of the options menu.";
        public static string splashSelectLanguageConfirmButtonText = "Confirm";
        public static string splashEpilepsyWarningText = "<color=red><b>Photosensitive Epilepsy Warning</b></color>\r\n\r\nSome people experience a seizure when exposed to certain visual images, like flashing lights or patterns that may appear in video games. Even people who have no history of seizures.\r\n\r\nImmediately stop playing and consult a doctor if you experience any symptoms.";
        public static string splashSaveWarningText = "<color=red><b>Warning</b></color>\r\n\r\nThis game uses an auto-save feature. Please do not turn off your game\r\n\r\nor hardware while the icon below is displayed.";

        public static string mainMenuContinueButtonText = "Continue";

        public static string optionsGameplaySectionTitle = "Gameplay";
        public static string optionsVideoSectionTitle = "Video";
        public static string optionsAudioSectionTitle = "Audio";
        public static string optionsKeyboardSectionTitle = "Keyboard";
        public static string optionsGamepadSectionTitle = "Gamepad";
        public static string optionsInterfaceSectionTitle = "Interface";
        public static string optionsAccessibilitySectionTitle = "Accessibility";

        public static string optionsDescClear = "";
        public static string optionsActivateSetting = "<color=red>Activate</color> this setting.";
        public static string optionsRevertSetting = "<color=red>Revert</color> this setting to it's previous value.";
        public static string optionsInvertSetting = "<color=red>Invert</color> (reverse) this setting.";

        public static string optionsAutosavesDesc = "Allow the game to <color=red>save automatically</color> at certain points.";
        public static string optionsMaximumAutosavesDesc = "The maximum amount of times the game will create a new <color=red>autosave</color> file, it will then overwite the oldest <color=red>autosave</color> each time.";
        public static string optionsMaximumQuicksavesDesc = "The maximum amount of times the game will create a new <color=red>quicksave</color> file, it will then overwite the oldest <color=red>quicksave</color> each time.";
        public static string optionsGoreDesc = "Allow <color=red>blood</color> effects and <color=red>dismemberment</color>, this will effect the <color=orange>Fragment</color> perk if it is taken.";

        public static string optionsDisplayDeviceDesc = "Which <color=red>monitor</color> you wish to use.";
        public static string optionsDisplayAdapterDesc = "The <color=red>GPU</color> the game is using.";
        public static string optionsResolutionDesc = "Which <color=red>resolution</color> you wish to play at.";
        public static string optionsDisplayModeDesc = "Play <color=red>fullscreen</color> or in a seperate <color=red>Window</color>.";
        public static string optionsVSyncDesc = "<color=red>VSync</color> synchronizes the frame rate of a game with the refresh rate of the monitor to prevent <color=red>screen tearing</color>.\r\n\r\n<color=red>Screen tearing</color> happens when your GPU outputs frames faster than your monitor can display them.";
        public static string optionsVSyncDescAdditional = "<color=red>[</color>Off<color=red>]</color>\r\n------------------------------------------\r\nFrames limited to <color=red>Framerate Cap</color>.\r\n\r\n\r\n<color=red>[</color>V<color=red>-</color>Blank<color=red>]</color>\r\n------------------------------------------\r\nFrames limited to monitors <color=red>maximum</color> refresh rate.\r\n\r\n\r\n<color=red>[</color>Second V<color=red>-</color>Blank<color=red>]</color>\r\n------------------------------------------\r\nFrames limited to <color=red>HALF</color> monitors maximum refresh rate.";
        public static string optionsFramerateCapDesc = "The target <color=red>framerate</color> the game should aim for assuming <color=red>VSync</color> is off and this setting is active. If you experience <color=red>screen tearing</color>, consider activating <color=red>VSync</color>.";
        public static string optionsFramerateCapDescAdditional = "<color=red>Warning</color>: This will be capped at your monitors <color=red>max refresh rate</color> if you attempt to go higher.";
        public static string optionsFramerateCapInfoDesc = "Your currently used display has a <color=red>maximum refresh rate</color> of ";
        public static string optionsQualityDesc = "A <color=red>global</color> quality setting, affects several settings.";
        public static string optionsQualityDescAdditional = "Affects the following:\r\n---------------------------------------------\r\n\r\n- <color=red>Anti-Alias</color>\r\n- <color=red>Volumetric Fog</color>\r\n- <color=red>Bloom</color>";
        public static string optionsAntiAliasDesc = "<color=red>Anti-aliasing</color> is a technique used to reduce the appearance of jagged edges. \r\n\r\n<color=red>FXAA</color> is the most performant, <color=red>TAA</color> is the highest quality and least performant.";
        public static string optionsAntiAliasDescAdditional = "<color=red>[</color>FXAA<color=red>]</color> </color><color=red>[</color><color=green>++</color> Performance<color=red>]</color>\r\n------------------------------------------\r\n<color=green><b>++</b></color> performance\\t\\t\\t| <color=red><b>--</b></color> Quality\r\n<color=red>\r\n\r\n[</color>SMAA<color=red>] [</color><color=green>+</color> Performance<color=red>]</color>\r\n<color=red>[</color>MSAA<color=red>]</color> </color><color=red>[</color><color=green>+</color> Quality<color=red>]</color>\r\n<color=red>[</color>TAA<color=red>]</color> </color><color=red>[</color><color=green>++</color> Quality<color=red>]</color>\r\n------------------------------------------ \\r\\n2x\\t| <color=green><b>++</b></color> Performance\\t| <color=red><b>--</b></color> Quality\\r\\n4x\\t| <color=green><b>+</b></color> Performance \\t| <color=green><b>+</b></color> Quality\\r\\n8x\\t| <color=red><b>--</b></color> Performance\\t| <color=green><b>++</b></color> Quality";
        public static string optionsVolumetricFogDesc = "The quality of the <color=red>Volumetric Fog</color>.";
        public static string optionsFOVDesc = "The <color=red>Field of View</color> of cameras.";
        public static string optionsFOVDescAdditional = "<color=red>Warning</color>: This may be capped in certain circumstances, if the game releases on a handheld for example.";
        public static string optionsBloomDesc = "The intensity of the <color=red>Bloom</color> (glow) of emissive objects.";
        public static string optionsHDRDesc = "<color=red>HDR</color> (High Dynamic Range) is a rendering technique that improves brightness, contrast, and color depth, allowing for more realistic and visually striking images..";
        public static string optionsHDRDescAdditional = "<color=red>[</color>On<color=red>]</color>\r\n---------------------------------------------\r\n<color=green>+</color> Lighting and Shadow Quality\r\n<color=green>+</color> Color Realism\r\n<color=green>+</color> Reduced detail loss in dark / bright areas\r\n<color=red>--</color> Performance";
        public static string optionsAnsioDesc = "<color=red>Anisotropic Filtering</color> (<color=red>AF</color>) enhances texture quality, especially for surfaces that are far away from the camera, making them appear sharper and more detailed without excessive blurring.";
        public static string optionsTonemappingDesc = "Tonemapping converts High Dynamic Range (HDR) colors into a displayable range for SDR / HDR screens. It helps maintain details in bright and dark areas.";
        public static string optionsTonemappingDescAdditional = "<color=red>Warning</color>: will apply Neutral or Aces but will not apply specific tiers unless HDR is active.";
    }
    #endregion

    #region French
    public class GameStringsFrench : MonoBehaviour
    {
        public static bool available = true;
        public static string gameTitle = "Le Dernier Débogage";

        public static string splashSelectLanguageText = "Sélectionner la Langue";
        public static string splashSelectedLanguageText = "Français";
        public static string splashSelectLanguageNoticeText = "Cela peut être modifié dans la section Accessibilité du menu des options.";
        public static string splashSelectLanguageConfirmButtonText = "Confirmer";
        public static string splashEpilepsyWarningText = "<color=red><b>Avertissement d'Épilepsie Photosensible</b></color>\r\n\r\nCertaines personnes peuvent avoir une crise lorsqu'elles sont exposées à certaines images visuelles, comme des lumières clignotantes ou des motifs pouvant apparaître dans les jeux vidéo. Même les personnes sans antécédents de crises.\r\n\r\nArrêtez immédiatement de jouer et consultez un médecin si vous ressentez des symptômes.";
        public static string splashSaveWarningText = "<color=red><b>Attention</b></color>\r\n\r\nCe jeu utilise une fonctionnalité de sauvegarde automatique. Veuillez ne pas éteindre votre jeu\r\n\r\nou votre matériel pendant que l'icône ci-dessous est affichée.";

        public static string mainMenuContinueButtonText = "Continuer";

        public static string optionsGameplaySectionTitle = "Jouabilité";
        public static string optionsVideoSectionTitle = "Vidéo";
    }
    #endregion


    public static class GameStringsGerman
    {
        public static bool available = false;
    }

    public static class GameStringsItalian
    {
        public static bool available = false;

    }

    public static class GameStringsSpanish
    {
        public static bool available = false;
    }

    public static class GameStringsAmerican
    {
        public static bool available = false;
    }

    public static class GameStringsRussian
    {
        public static bool available = false;
    }

    public static class GameStringsChinese
    {
        public static bool available = false;
    }

    public static class GameStringsJapanese
    {
        public static bool available = false;
    }

    public static class GameStringsBrazilian
    {
        public static bool available = false;
    }
}
