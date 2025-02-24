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
        public static string splashEpilepsyWarningText = "<color=" + Colors.textRed + "><b>Photosensitive Epilepsy Warning</b></color>\r\n\r\nSome people experience a seizure when exposed to certain visual images, like flashing lights or patterns that may appear in video games. Even people who have no history of seizures.\r\n\r\nImmediately stop playing and consult a doctor if you experience any symptoms.";
        public static string splashSaveWarningText = "<color=" + Colors.textRed + "><b>Warning</b></color>\r\n\r\nThis game uses an auto-save feature. Please do not turn off your game\r\n\r\nor hardware while the icon below is displayed.";

        public static string mainMenuContinueButtonText = "Continue";

        public static string optionsGameplaySectionTitle = "Gameplay";
        public static string optionsVideoSectionTitle = "Video";
        public static string optionsAudioSectionTitle = "Audio";
        public static string optionsKeyboardSectionTitle = "Keyboard";
        public static string optionsGamepadSectionTitle = "Gamepad";
        public static string optionsInterfaceSectionTitle = "Interface";
        public static string optionsAccessibilitySectionTitle = "Accessibility";

        public static string optionsDescClear = "";
        public static string optionsDescInitial = "A <color=" + Colors.textRed + ">description</color> of each setting will appear here.";
        public static string optionsDescInitialAdditional = "<color=" + Colors.textRed + ">Additional Information</color> for each setting will appear here.";
        public static string optionsActivateSetting = "<color=" + Colors.textRed + ">Activate</color> this setting.";
        public static string optionsRevertSetting = "<color=" + Colors.textRed + ">Revert</color> this setting to it's previous value.";
        public static string optionsInvertSetting = "<color=" + Colors.textRed + ">Invert</color> (reverse) this setting.";
        public static string optionsNextPage = "Go to <color=" + Colors.textRed + ">next</color> page.";
        public static string optionsPreviousPage = "Go to <color=" + Colors.textRed + ">previous</color> page.";
        public static string optionsPage1Desc = "Go to<color=" + Colors.textRed + ">Page 1</color>.";
        public static string optionsPage2Desc = "Go to <color=" + Colors.textRed + ">Page 2</color>.";

        public static string optionsAutosavesDesc = "Allow the game to <color=" + Colors.textRed + ">save automatically</color> at certain points.";
        public static string optionsMaximumAutosavesDesc = "The maximum amount of times the game will create a new <color=" + Colors.textRed + ">autosave</color> file, it will then overwite the oldest <color=" + Colors.textRed + ">autosave</color> each time.";
        public static string optionsMaximumQuicksavesDesc = "The maximum amount of times the game will create a new <color=" + Colors.textRed + ">quicksave</color> file, it will then overwite the oldest <color=" + Colors.textRed + ">quicksave</color> each time.";
        public static string optionsGoreDesc = "Allow <color=" + Colors.textRed + ">blood</color> effects and <color=" + Colors.textRed + ">dismemberment</color>, this will effect the <color=orange>Fragment</color> perk if it is taken.";

        public static string optionsDisplayDeviceDesc = "Which <color=" + Colors.textRed + ">monitor</color> you wish to use.";
        public static string optionsDisplayAdapterDesc = "The <color=" + Colors.textRed + ">GPU</color> the game is using.";
        public static string optionsResolutionDesc = "Which <color=" + Colors.textRed + ">resolution</color> you wish to play at.";
        public static string optionsDisplayModeDesc = "Play <color=" + Colors.textRed + ">fullscreen</color> or in a seperate <color=" + Colors.textRed + ">Window</color>.";
        public static string optionsVSyncDesc = "<color=" + Colors.textRed + ">VSync</color> synchronizes the frame rate of a game with the refresh rate of the monitor to prevent <color=" + Colors.textRed + ">screen tearing</color>.\r\n\r\n<color=" + Colors.textRed + ">Screen tearing</color> happens when your GPU outputs frames faster than your monitor can display them.";
        public static string optionsVSyncDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------\r\nFrames limited to <color=" + Colors.textRed + ">Framerate Cap</color>.\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>V<color=" + Colors.textRed + ">-</color>Blank<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------\r\nFrames limited to monitors <color=" + Colors.textRed + ">maximum</color> refresh rate.\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Second V<color=" + Colors.textRed + ">-</color>Blank<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------\r\nFrames limited to <color=" + Colors.textRed + ">HALF</color> monitors maximum refresh rate.";
        public static string optionsFramerateCapDesc = "The target <color=" + Colors.textRed + ">framerate</color> the game should aim for assuming <color=" + Colors.textRed + ">VSync</color> is off and this setting is active. If you experience <color=" + Colors.textRed + ">screen tearing</color>, consider activating <color=" + Colors.textRed + ">VSync</color>.";
        public static string optionsFramerateCapDescAdditional = "<color=" + Colors.textRed + ">Warning</color>: This will be capped at your monitors <color=" + Colors.textRed + ">max refresh rate</color> if you attempt to go higher.";
        public static string optionsFramerateCapInfoDesc = "Your currently used display has a <color=" + Colors.textRed + ">maximum refresh rate</color> of ";
        public static string optionsQualityDesc = "A <color=" + Colors.textRed + ">global</color> quality setting, affects several settings.";
        public static string optionsQualityDescAdditional = "Affects the following:\r\n\r\n- <color=" + Colors.textRed + ">Anti-Alias</color>\r\n- <color=" + Colors.textRed + ">Volumetric Fog</color>\r\n- <color=" + Colors.textRed + ">Bloom</color>";
        public static string optionsAntiAliasDesc = "<color=" + Colors.textRed + ">Anti-aliasing</color> is a technique used to reduce the appearance of jagged edges. \r\n\r\n<color=" + Colors.textRed + ">FXAA</color> is the most performant, <color=" + Colors.textRed + ">TAA</color> is the highest quality and least performant.";
        public static string optionsAntiAliasDescAdditional = "<color=" + Colors.textRed + ">[</color>FXAA<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+++</b></color> Performance<color=" + Colors.textRed + ">]</color>\\r\\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>FXAA<color=" + Colors.textRed + ">]</color>           <color=" + Colors.textGreen + "><b>+++</b></color> Performance       /      <color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>SMAA<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+</b></color> Quality<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + ">[</color>MSAA<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>++</b></color> Quality<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + ">[</color>TAA<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+++</b></color> Quality<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>2x<color=" + Colors.textRed + ">]</color>                <color=" + Colors.textRed + "><b>-</b> </color>Performance          /         <color=" + Colors.textGreen + "><b>+</b></color> Quality\\r\\n<color=" + Colors.textRed + ">[</color>4x<color=" + Colors.textRed + ">]</color>            <color=" + Colors.textRed + "> <b>--</b>  </color>Performance          /       <color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n<color=" + Colors.textRed + ">[</color>8x<color=" + Colors.textRed + ">]</color>           <color=" + Colors.textRed + "><b>---</b> </color>Performance          /      <color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        public static string optionsVolumetricFogDesc = "The quality of the <color=" + Colors.textRed + ">Volumetric Fog</color>.";
        public static string optionsVolumetricFogDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+++</b></color> Performance<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>           <color=" + Colors.textGreen + "><b>+++</b></color> Performance         /     <color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+</b></color> Quality<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Low<color=" + Colors.textRed + ">]</color>               <color=" + Colors.textGreen + "><b>+</b></color> Performance         /         <color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n<color=" + Colors.textRed + ">[</color>Medium<color=" + Colors.textRed + ">]</color>      <color=" + Colors.textRed + "><b>--</b> </color>Performance         /        <color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n<color=" + Colors.textRed + ">[</color>High<color=" + Colors.textRed + ">]</color>       <color=" + Colors.textRed + "><b>---</b>  </color>Performance         /      <color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        public static string optionsFOVDesc = "The <color=" + Colors.textRed + ">Field of View</color> of cameras.";
        public static string optionsFOVDescAdditional = "<color=" + Colors.textRed + ">Warning</color>: This may be capped in certain circumstances, if the game releases on a handheld for example.";
        public static string optionsBloomDesc = "The intensity of the <color=" + Colors.textRed + ">Bloom</color> (glow) of emissive objects.";
        public static string optionsBloomDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+++</b></color> Performance<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>           <color=" + Colors.textGreen + "><b>+++</b></color> Performance         /     <color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+</b></color> Quality<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Low<color=" + Colors.textRed + ">]</color>               <color=" + Colors.textRed + "><b>-</b></color> Performance         /         <color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n<color=" + Colors.textRed + ">[</color>Medium<color=" + Colors.textRed + ">]</color>      <color=" + Colors.textRed + "><b>--</b> </color>Performance         /        <color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n<color=" + Colors.textRed + ">[</color>High<color=" + Colors.textRed + ">]</color>        <color=" + Colors.textRed + "><b>---</b>  </color>Performance         /      <color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        public static string optionsHDRDesc = "<color=" + Colors.textRed + ">HDR</color> (<color=" + Colors.textRed + ">High Dynamic Range</color>) is a rendering technique that improves brightness, contrast, and color depth, allowing for more realistic and visually striking images..";
        public static string optionsHDRDescAdditional = "<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color>\r\n---------------------------------------------\r\n<color=" + Colors.textGreen + ">+</color> Lighting and Shadow Quality\r\n<color=" + Colors.textGreen + ">+</color> Color Realism\r\n<color=" + Colors.textGreen + ">+</color> Reduced detail loss in dark / bright areas\r\n<color=" + Colors.textRed + ">--</color> Performance";
        public static string optionsAnsioDesc = "<color=" + Colors.textRed + ">Anisotropic Filtering</color> (<color=" + Colors.textRed + ">AF</color>) enhances texture quality, especially for surfaces that are far away from the camera, making them appear sharper and more detailed without excessive blurring.";
        public static string optionsAnsioDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+++</b></color> Performance<color=" + Colors.textRed + ">]</color>\\r\\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>             <color=" + Colors.textGreen + "><b>+++</b></color> Performance       /      <color=" + Colors.textRed + "><b>---</b></color> Quality\\r\\n\\r\\n\\r\\n\\r\\n<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>+</b></color> Quality<color=" + Colors.textRed + ">]</color>\\r\\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>2x<color=" + Colors.textRed + ">]</color>                   <color=" + Colors.textGreen + "><b>+</b></color> Performance        /        <color=" + Colors.textGreen + "><b>+</b></color> Quality\\r\\n<color=" + Colors.textRed + ">[</color>4x<color=" + Colors.textRed + ">]</color>                   <color=" + Colors.textRed + "><b>-</b> </color>Performance        /        <color=" + Colors.textGreen + "><b>+</b></color> Quality\\r\\n<color=" + Colors.textRed + ">[</color>8x<color=" + Colors.textRed + ">]</color>                <color=" + Colors.textRed + "><b>--</b> </color>Performance        /      <color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n<color=" + Colors.textRed + ">[</color>16x<color=" + Colors.textRed + ">]</color>            <color=" + Colors.textRed + "><b>---</b> </color>Performance        /     <color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsTonemappingDesc = "<color=" + Colors.textRed + ">Tonemapping</color> converts <color=" + Colors.textRed + ">High Dynamic Range</color> (<color=" + Colors.textRed + ">HDR</color>) colors into a displayable range for <color=" + Colors.textRed + ">SDR</color> / <color=" + Colors.textRed + ">HDR</color> screens. It helps maintain details in bright and dark areas.";
        public static string optionsTonemappingDescAdditional = "<color=" + Colors.textRed + ">Warning</color>: will apply <color=" + Colors.textRed + ">Neutral</color> or <color=" + Colors.textRed + ">Aces</color> but will not apply specific tiers unless <color=" + Colors.textRed + ">HDR</color> is active.";
        
        
        public static string optionsGlobalIlluminationDesc = "<color=" + Colors.textRed + ">Global Illumination</color> (<color=" + Colors.textRed + ">GI</color>) is used to simulate the way light interacts with surfaces in a scene.\r\n\r\nIf you choose a <color=" + Colors.textRed + ">Raytracing</color> mode and your system does not support it, the game will default to <color=" + Colors.textRed + ">BT2390</color>.";
        public static string optionsGlobalIlluminationDescAdditional = "<color=" + Colors.textRed + ">[</color>Realtime<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + ">++ </color>Quality<color=" + Colors.textRed + ">]</color>\\r\\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Realtime<color=" + Colors.textRed + ">]</color>              </color><color=" + Colors.textRed + ">-- </color> Performance    /   <color=" + Colors.textGreen + ">++ </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Raymarching - SSGI<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + ">++ </color>Performance<color=" + Colors.textRed + ">]</color>\\r\\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Reinhard<color=" + Colors.textRed + ">]</color>          </color><color=" + Colors.textGreen + ">+++ </color> Performance    /   <color=" + Colors.textRed + ">--- </color>Quality\r\n<color=" + Colors.textRed + ">[</color>BT2390<color=" + Colors.textRed + ">]</color></color>              <color=" + Colors.textGreen + ">++ </color> Performance    /     <color=" + Colors.textRed + ">-- </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Raytracing - RTGI<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + ">+++ </color>Quality<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>ACES1000<color=" + Colors.textRed + ">]</color> </color>\\t   <color=" + Colors.textRed + ">- </color> Performance     /      <color=" + Colors.textGreen + ">+ </color>Quality\r\n<color=" + Colors.textRed + ">[</color>ACES2000<color=" + Colors.textRed + ">]</color> </color>\\t <color=" + Colors.textRed + ">-- </color> Performance     /     <color=" + Colors.textGreen + ">++ </color>Quality\r\n<color=" + Colors.textRed + ">[</color>ACES4000<color=" + Colors.textRed + ">]</color> </color>       <color=" + Colors.textRed + ">---</color> Performance     /   <color=" + Colors.textGreen + ">+++ </color>Quality";
        public static string optionsGlobalIlluminationDescAdditional2 = "Raytracing Availability: <color=" + Colors.textRed + ">Unavailable</color>";
        public static string optionsGlobalIlluminationDescAdditional3 = "Raytracing Availability: <color=" + Colors.textGreen + ">Available</color>";
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
        public static string splashEpilepsyWarningText = "<color=" + Colors.textRed + "><b>Avertissement d'Épilepsie Photosensible</b></color>\r\n\r\nCertaines personnes peuvent avoir une crise lorsqu'elles sont exposées à certaines images visuelles, comme des lumières clignotantes ou des motifs pouvant apparaître dans les jeux vidéo. Même les personnes sans antécédents de crises.\r\n\r\nArrêtez immédiatement de jouer et consultez un médecin si vous ressentez des symptômes.";
        public static string splashSaveWarningText = "<color=" + Colors.textRed + "><b>Attention</b></color>\r\n\r\nCe jeu utilise une fonctionnalité de sauvegarde automatique. Veuillez ne pas éteindre votre jeu\r\n\r\nou votre matériel pendant que l'icône ci-dessous est affichée.";

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
