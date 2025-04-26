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
        
        public static string optionsDescInitial = "A <color=" + Colors.textRed + "><b>description</b></color> of each setting will appear here.";
        public static string optionsDescInitialAdditional = "<color=" + Colors.textRed + "><b>Additional Information</b></color> for each setting will appear here.";        
        
        public static string optionsModifiedSetting = "This setting has been <color=" + Colors.textRed + "><b>Modified</b></color>. Click the button at the <color=" + Colors.textRed + "><b>bottom left</b></color> of this screen to revert all modified settings.";

        public static string optionsActivateSetting = "<color=" + Colors.textRed + "><b>Activate</b></color> this setting.";
        public static string optionsRevertSetting = "<color=" + Colors.textRed + "><b>Revert</b></color> modified settings to previous values.";
        public static string optionsInvertSetting = "<color=" + Colors.textRed + "><b>Invert</b></color> (reverse) this setting.";
        
        public static string optionsTextInputDesc = "Click the <color=" + Colors.textRed + "><b>number</b></color> to manually enter a value.";
        
        public static string optionsNextPage = "Go to <color=" + Colors.textRed + "><b>next</b></color> page.";
        public static string optionsPreviousPage = "Go to <color=" + Colors.textRed + "><b>previous</b></color> page.";
        public static string optionsPage1Desc = "Go to <color=" + Colors.textRed + "><b>Page 1</b></color> of this section.";        
        public static string optionsPage2Desc = "Go to <color=" + Colors.textRed + "><b>Page 2</b></color> of this section.";
        
        public static string raytracingOffDesc = "Raytracing Availability:\r\n\r\n<color=" + Colors.textRed + "><b>Unavailable</b></color>";
        public static string raytracingOffDescAdditional = "Disabled:\r\n\r\n- <color=" + Colors.textRed + "><b>Ray-Traced Global Illumination</b></color>(<color=" + Colors.textRed + "><b>RTGI</b></color>)\r\n\r\n\r\n- <color=" + Colors.textRed + "><b>Ray-Traced Reflections</b></color>(<color=" + Colors.textRed + "><b>RTR</b></color>)";
        
        public static string raytracingOnDesc = "Raytracing Availability:\r\n\r\n<color=" + Colors.textGreen + "><b>Available</b></color>";
        public static string raytracingOnDescAdditional = "Enabled:\r\n\r\n- <color=" + Colors.textRed + "><b>Ray-Traced Global Illumination</b></color>(<color=" + Colors.textRed + "><b>RTGI</b></color>)\r\n\r\n\r\n- <color=" + Colors.textRed + "><b>Ray-Traced Reflections</b></color>(<color=" + Colors.textRed + "><b>RTR</b></color>)";
        
        public static string optionsAutosavesDesc = "Allow the game to <color=" + Colors.textRed + "><b>save automatically</b></color> at certain points.";
        
        public static string optionsMaximumAutosavesDesc = "The maximum amount of times the game will create a new <color=" + Colors.textRed + "><b>autosave</b></color> file, it will then overwite the oldest <color=" + Colors.textRed + "><b>autosave</b></color> each time.";
        
        public static string optionsMaximumQuicksavesDesc = "The maximum amount of times the game will create a new <color=" + Colors.textRed + "><b>quicksave</b></color> file, it will then overwite the oldest <color=" + Colors.textRed + "><b>quicksave/<b></color> each time.";
        
        public static string optionsGoreDesc = "Allow <color=" + Colors.textRed + "><b>blood</b></color> effects and <color=" + Colors.textRed + "><b>dismemberment</b></color>, this will also effect the <color=" + Colors.textOrange + "><b>Fragment</b></color> property if it is taken.";

        public static string optionsDestructionDesc = "Enable <color=" + Colors.textRed + "><b>rigidbodies</b></color> so objects fly around, <color=" + Colors.textRed + "><b>destruction</b></color> effects so objects shatter or both.";

        public static string optionsCrowdsDesc = "The density of <color=" + Colors.textRed + "><b>crowds</b></color> and <color=" + Colors.textRed + "><b>non interactable NPCs</b></color> in the game.";

        public static string optionsTrafficDesc = "The density of <color=" + Colors.textRed + "><b>traffic</b></color> in the game.";

        public static string optionsWildlifeDesc = "The density of <color=" + Colors.textRed + "><b>wildlife</b></color> in the game.";

        public static string optionsDisplayDeviceDesc = "Which <color=" + Colors.textRed + "><b>monitor</b></color> you wish to use.";
        public static string optionsDisplayAdapterDesc = "The <color=" + Colors.textRed + "><b>GPU</b></color> the game is using.";
        
        public static string optionsResolutionDesc = "Which <color=" + Colors.textRed + "><b>resolution</b></color> you wish to play at.";
        
        public static string optionsDisplayModeDesc = "Play <color=" + Colors.textRed + "><b>fullscreen</b></color> or in a seperate <color=" + Colors.textRed + "><b>Window</b></color>.";
        
        public static string optionsVSyncDesc = "<color=" + Colors.textRed + "><b>VSync</b></color> synchronizes the frame rate of a game with the refresh rate of the monitor to prevent <color=" + Colors.textRed + "><b>screen tearing</b></color>.\r\n\r\n<color=" + Colors.textRed + "><b>Screen tearing</b></color> happens when your GPU outputs frames faster than your monitor can display them.";
        public static string optionsVSyncDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\nFrames limited to <color=" + Colors.textRed + "><b>Framerate Cap</b></color>.\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>V<color=" + Colors.textRed + "><b>-</b></color>Blank<color=" + Colors.textRed + "><b>]</b></color>\r\nFrames limited to monitors <color=" + Colors.textRed + "><b>maximum</b></color> refresh rate.\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Second V<color=" + Colors.textRed + "><b>-</b></color>Blank<color=" + Colors.textRed + "><b>]</b></color>\r\nFrames limited to <color=" + Colors.textRed + "><b>HALF</b></color> monitors <color=" + Colors.textRed + "><b>maximum</b></color> refresh rate.";
        
        public static string optionsFramerateCapDesc = "The target <color=" + Colors.textRed + "><b>framerate</b></color> the game should aim for assuming <color=" + Colors.textRed + "><b>VSync</b></color> is off and this setting is active. If you experience <color=" + Colors.textRed + "><b>screen tearing</b></color>, consider activating <color=" + Colors.textRed + ">V<b>Sync</b></color>.";
        public static string optionsFramerateCapDescAdditional = "<color=" + Colors.textRed + "><b>Warning</b></color>: This will be capped at your monitors <color=" + Colors.textRed + "><b>max refresh rate</b></color> if you attempt to go higher.";
        public static string optionsFramerateCapInfoDesc = "Your currently used display has a <color=" + Colors.textRed + "><b>maximum refresh rate</b></color> of ";
        
        public static string optionsQualityDesc = "A <color=" + Colors.textRed + "><b>global</b></color> quality setting.";
        public static string optionsQualityDescAdditional = "Affects the following:\r\n----------------------------------\r\n\r\n- <color=" + Colors.textRed + "><b>Bloom</b></color>\r\n- <color=" + Colors.textRed + "><b>Shadow Quality</b></color>\r\n- <color=" + Colors.textRed + "><b>Global Illumination<b></color>\r\n- <color=" + Colors.textRed + "><b>GI Resolution</b></color>\r\n- <color=" + Colors.textRed + "><b>Anti-Alias</b></color>\r\n- <color=" + Colors.textRed + "><b>TAA Sharpening</b></color>\r\n- <color=" + Colors.textRed + "><b>Reflections</b></color>\r\n- <color=" + Colors.textRed + "><b>Planar Reflections</b></color>\r\n- <color=" + Colors.textRed + "><b>Ansio Filtering</b></color>\r\n- <color=" + Colors.textRed + "><b>Weather Effects</b></color>\r\n- <color=" + Colors.textRed + "><b>Volumetric Fog</b></color>\r\n- <color=" + Colors.textRed + "><b>Line Rendering</b></color>\r\n- <color=" + Colors.textRed + "><b>Crevices</b></color>\r\n- <color=" + Colors.textRed + "><b>Tonemapping</b></color>\r\n- <color=" + Colors.textRed + "><b>Tonemapping Quality</b></color>\r\n";
        
        public static string optionsAntiAliasDesc = "<color=" + Colors.textRed + "><b>Anti-aliasing</b></color> is a technique to reduce jagged edges. \r\n\r\n- <color=" + Colors.textRed + "><b>FXAA</b></color> (<color=" + Colors.textRed + "><b>Fast Approximate</b></color>)</color>\r\nDetects high-contrast edges.\r\n\r\n- <color=" + Colors.textRed + "><b>SMAA</b></color> (<color=" + Colors.textRed + "><b>Subpixel Morphological</b></color>)\r\nUses post-processing / pattern detection.\r\n\r\n- <color=" + Colors.textRed + "><b>MSAA</b></color> (<color=" + Colors.textRed + "><b>Multisample</b></color>)\r\nTakes multiple samples per pixel.\r\n\r\n- <color=" + Colors.textRed + "><b>TAA</b></color> (<color=" + Colors.textRed + "><b>Temporal</b></color>)\r\nBlends frames together.";
        public static string optionsAntiAliasDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>FXAA<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>SMAA<color=" + Colors.textRed + "><b>]</b></color> <color=" + Colors.textRed + "><b>[</b></color>MSAA<color=" + Colors.textRed + "><b>]</b></color> <color=" + Colors.textRed + "><b>[</b></color>TAA<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>2x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><</b> </color>Performance\r\n<color=" + Colors.textGreen + ">></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>4x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>8x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality";
        
        public static string optionsTaaQualityDesc = "When using <color=" + Colors.textRed + "><b>Temporal Anti Aliasing</b></color> (<color=" + Colors.textRed + "><b>TAA</b></color>) you can set what type of <color=" + Colors.textRed + "><b>sharpening</b></color> to use.\r\n\r\n- <color=" + Colors.textRed + "><b>Low Quality</b></color>\r\nBasic filter.\r\n\r\n- <color=" + Colors.textRed + "><b>Post Sharpen</b></color>\r\nBetter quality filter.\r\n\r\n- <color=" + Colors.textRed + "><b>Contrast Adapt</b></color>\r\nUses <color=" + Colors.textOrange + "><b>AMD Fidelity-FX</b></color>, a more advanced sharpening method.";
        public static string optionsTaaQualityDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Low Quality<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Post Sharpen<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b><</b></color> Performance\r\n<color=" + Colors.textGreen + ">></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Contrast Adapt<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><<<</color> Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality";
        
        public static string optionsVolumetricFogDesc = "The quality of the <color=" + Colors.textRed + "><b>Volumetric Fog</b></color>.";
        public static string optionsVolumetricFogDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>On<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>Low<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><</b></color> Performance\r\n<color=" + Colors.textGreen + ">></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Medium<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>High<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b>  </color>Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality";
        
        public static string optionsFOVDesc = "The <color=" + Colors.textRed + "><b>Field of View</b></color> of cameras.";
        public static string optionsFOVDescAdditional = "<color=" + Colors.textRed + "><b>Warning</b></color>: This may be capped in certain circumstances, if the game releases on a handheld for example.";
        
        public static string optionsRenderDistanceDesc = "The <color=" + Colors.textRed + "><b>distance</b></color> at which objects will be rendered in <color=" + Colors.textRed + "><b>meters</b></color>.";
        
        public static string optionsBloomDesc = "The intensity of the <color=" + Colors.textRed + "><b>Bloom</b></color> (glow) of emissive objects.";
        public static string optionsBloomDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>On<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>Low<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><</b></color> Performance <color=" + Colors.textGreen + ">\r\n></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Medium<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>High<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality";
        
        public static string optionsHDRDesc = "<color=" + Colors.textRed + "><b>HDR</b></color> (<color=" + Colors.textRed + "><b>High Dynamic Range</b></color>) is a rendering technique that improves brightness, contrast, and color depth, allowing for more realistic and visually striking images.";
        public static string optionsHDRDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>On<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">></color> Lighting and Shadow Quality\r\n\r\n<color=" + Colors.textGreen + ">></color> Colour Realism\r\n\r\n<color=" + Colors.textGreen + ">>></color> Reduced detail loss in dark / bright areas\r\n\r\n<color=" + Colors.textRed + "><b><<</b></color> Performance";
        
        public static string optionsAnsioDesc = "<color=" + Colors.textRed + "><b>Anisotropic Filtering</b></color> (<color=" + Colors.textRed + "><b>AF</b></color>) enhances texture quality, especially for surfaces that are far away from the camera, making them appear sharper and more detailed without excessive blurring.";
        public static string optionsAnsioDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>On<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>2x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textOrange + "><b>-</b></color> Performance\r\n<color=" + Colors.textGreen + ">></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>4x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><</b> </color>Performance\r\n<color=" + Colors.textGreen + ">></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>8x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>16x<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality";

        public static string optionsWeatherEffectsDesc = "The amount of <color=" + Colors.textRed + "><b>weather effects</b></color> in the game.\r\n\r\n- <color=" + Colors.textRed + "><b>Half</b></color>\r\nRain, Snow etc.\r\n\r\n- <color=" + Colors.textRed + "><b>Full</b></color>\r\nAdds blowing leaves, droplets etc.";
        public static string optionsWeatherEffectsDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Half<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + "><b>>></b></color> Performance <color=" + Colors.textRed + ">\r\n<</color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Full<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b><</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality";

        public static string optionsLineRenderingDesc = "<color=" + Colors.textRed + "><b>High Quality Line Rendering</b></color> draws lines with <color=" + Colors.textRed + "><b>analytic anti-aliasing</b></color> and <color=" + Colors.textRed + "><b>transparent sorting</b></color> for better quality geometry.";
        public static string optionsLineRenderingDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Standard<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">></color> Performance\r\n<color=" + Colors.textRed + "><b><</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>High Quality<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b><<</b></color> Performance <color=" + Colors.textGreen + ">\r\n>></color> Quality";

        public static string optionsCrevicesDesc = "<color=" + Colors.textRed + "><b>Crevices</b></color> adds additional visual depth to geometry.";
        public static string optionsCrevicesDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">></color> Performance\r\n<color=" + Colors.textRed + "><b><</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>On<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b><</b></color> Performance <color=" + Colors.textGreen + ">\r\n></color> Quality";

        public static string optionsTonemappingDesc = "<color=" + Colors.textRed + "><b>Tonemapping</b></color> converts <color=" + Colors.textRed + "><b>High Dynamic Range</b></color> (<color=" + Colors.textRed + "><b>HDR</b></color>) colors into a displayable range for <color=" + Colors.textRed + "><b>SDR</b></color> / <color=" + Colors.textRed + "><b>HDR</b></color> screens. It helps maintain details in bright and dark areas.";
        public static string optionsTonemappingDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Neutral<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\nMinimal impact on color hue & saturation.\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>ACES<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\nCinematic look, contrasted and effects color hue & saturation.";

        public static string optionsTonemappingQualityDesc = "<color=" + Colors.textRed + "><b>Tonemapping</b></color> converts <color=" + Colors.textRed + "><b>High Dynamic Range</b></color> (<color=" + Colors.textRed + "><b>HDR</b></color>) colors into a displayable range for <color=" + Colors.textRed + "><b>SDR</b></color> / <color=" + Colors.textRed + "><b>HDR</b></color> screens. It helps maintain details in bright and dark areas.";
        public static string optionsTonemappingQualityDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Neutral<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>Reinhard<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>BT2390<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textGreen + ">>> </color>Performance\r\n<color=" + Colors.textRed + "><b><<</b> </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>ACES + HDR<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>1000 Nits<color=" + Colors.textRed + "><b>]</b></color> </color>\r\n<color=" + Colors.textRed + "><b><</b> </color>Performance\r\n<color=" + Colors.textGreen + ">> </color>Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>2000 Nits<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>> </color>Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>4000 Nits<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b></color> Performance\r\n<color=" + Colors.textGreen + ">>>> </color>Quality";

        public static string optionsGlobalIlluminationDesc = "<color=" + Colors.textRed + "><b>Global Illumination</b></color> (<color=" + Colors.textRed + "><b>GI</b></color>) is used to simulate the way light interacts with surfaces in a scene.\r\n\r\n<color=" + Colors.textRed + "><b>Raymarching</b></color> is done using the <color=" + Colors.textRed + "><b>CPU</b></color> and <color=" + Colors.textRed + "><b>Raytracing</b></color> is done using your <color=" + Colors.textRed + "><b>Display Adapter</b></color> if it is capable.\r\n\r\nIf you choose a <color=" + Colors.textRed + "><b>Raytracing</b></color> mode and your system does not support it, the game will default to <color=" + Colors.textRed + "><b>SSGI Medium</b></color>.";
        public static string optionsGlobalIlluminationDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Raymarching - SSGI<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b>[</b></color>Raytracing - RTGI<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>Low<color=" + Colors.textRed + "><b>]</b></color>\r\n</color><color=" + Colors.textRed + "><b><</b></color> Performance\r\n<color=" + Colors.textGreen + ">> </color>Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Medium<color=" + Colors.textRed + "><b>]</b></color>\r\n</color><color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>> </color>Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>High<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b></color> Performance\r\n<color=" + Colors.textGreen + ">>>> </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Realtime<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n</color><color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>> </color>Quality";

        public static string optionsGlobalIlluminationResDesc = "When using <color=red><b>SSGI</b></color> or <color=red><b>RTGI</b></color> the resolution can be set to <color=red><b>Full</b></color> quality or <color=red><b>Half</b></color>.";
        public static string optionsGlobalIlluminationResDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Half<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>> </color>Performance\r\n<color=" + Colors.textRed + ">< </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Full<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><</color> Performance\r\n<color=" + Colors.textGreen + ">>> </color>Quality";
                
        public static string optionsReflectionsDesc = "What type of <color=" + Colors.textRed + "><b>Reflections</b></color> you wish to use.";
        public static string optionsReflectionsDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Raymarching - SSR<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b>[</b></color>Raytracing - RTR<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>Low<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><</b> </color>Performance\r\n<color=" + Colors.textGreen + ">> </color>Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Medium<color=" + Colors.textRed + "><b>]</b></color>\r\n</color><color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>High<color=" + Colors.textRed + "><b>]</b></color> </color>\r\n<color=" + Colors.textRed + "><b><<<</b></color> Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Realtime<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>> </color>Quality";
        
        public static string optionsPlanarReflectionsDesc = "<color=red>Planar Reflections</color> can be used in addition to other reflection methods to increase quality.";
        public static string optionsPlanarReflectionsDescAdditional = "<color=" + Colors.textRed + "><b>[</b></color>Off<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textGreen + ">>>></color> Performance\r\n<color=" + Colors.textRed + "><b><<<</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>On<color=" + Colors.textRed + "><b>]</b></color>\r\n----------------------------------\r\n<color=" + Colors.textRed + "><b>[</b></color>Low<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><</b></color> Performance <color=" + Colors.textGreen + ">\r\n></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>Medium<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>></color> Quality\r\n\r\n<color=" + Colors.textRed + "><b>[</b></color>High<color=" + Colors.textRed + "><b>]</b></color>\r\n<color=" + Colors.textRed + "><b><<<</b> </color>Performance\r\n<color=" + Colors.textGreen + ">>>></color> Quality";
        
        public static string optionsShadowQualityDesc = "The quality of <color=" + Colors.textRed + "><b>shadows</b></color>.";
        public static string optionsShadowQualityDescAdditional = "<color=red><b>[</b></color>Low<color=red><b>]</b></color>\r\n<color=green><b>>>></b></color> Performance\r\n<color=red><b><<<</b></color> Quality\r\n\r\n<color=red><b>[</b></color>Medium<color=red><b>]</b></color>\r\n<color=red><b><</b> </color>Performance\r\n<color=green><b>></b></color> Quality\r\n\r\n<color=red><b>[</b></color>High<color=red><b>]</b></color>\r\n<color=red><b><<</b> </color>Performance\r\n<color=green><b>>></b></color> Quality\r\n\r\n<color=red><b>[</b></color>Ultra<color=red><b>]</b></color>\r\n<color=red><b><<<</b> </color>Performance\r\n<color=green><b>>>></b></color> Quality";
        
        public static string optionsShadowDistanceDesc = "The distance at which <color=" + Colors.textRed + "><b>shadows</b></color> will be rendered.";



        public static string optionsDiagnosticsDesc = "Diagnostics to show the <color=" + Colors.textRed + ">Resources</color> the game is using.";
        
        public static string optionsFPSDesc = "Activate to display the <color=" + Colors.textRed + ">Frames Per Second</color> the game is using.";
        
        public static string optionsMemoryDesc = "Activate to display the <color=" + Colors.textRed + ">Memory</color> the game is using.";        
    }
    #endregion

    public class GameStringsFrench : MonoBehaviour
    {
        public static bool available = false;
    }

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
