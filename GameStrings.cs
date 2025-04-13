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
        public static string optionsModifiedSetting = "This setting has been <color=" + Colors.textRed + ">Modified</color>. Click the Gear icon at the <color=" + Colors.textRed + ">top left</color> of this screen to revert all modified settings.";
        public static string optionsRevertSetting = "<color=" + Colors.textRed + ">Revert</color> modified settings to previous values.";
        public static string optionsInvertSetting = "<color=" + Colors.textRed + ">Invert</color> (reverse) this setting.";
        public static string optionsTextInputDesc = "Click the <color=" + Colors.textRed +">number</color> to manually enter a value.";
        public static string optionsNextPage = "Go to <color=" + Colors.textRed + ">next</color> page.";
        public static string optionsPreviousPage = "Go to <color=" + Colors.textRed + ">previous</color> page.";
        public static string optionsPage1Desc = "Go to <color=" + Colors.textRed + ">Page 1</color> of this section.";
        public static string optionsPage2Desc = "Go to <color=" + Colors.textRed + ">Page 2</color> of this section.";
        public static string raytracingOffDesc = "Raytracing Availability: <color=" + Colors.textRed + ">Unavailable</color>";
        public static string raytracingOnDesc = "Raytracing Availability: <color=" + Colors.textGreen + ">Available</color>";

        public static string optionsAutosavesDesc = "Allow the game to <color=" + Colors.textRed + ">save automatically</color> at certain points.";
        public static string optionsMaximumAutosavesDesc = "The maximum amount of times the game will create a new <color=" + Colors.textRed + ">autosave</color> file, it will then overwite the oldest <color=" + Colors.textRed + ">autosave</color> each time.";
        public static string optionsMaximumQuicksavesDesc = "The maximum amount of times the game will create a new <color=" + Colors.textRed + ">quicksave</color> file, it will then overwite the oldest <color=" + Colors.textRed + ">quicksave</color> each time.";
        public static string optionsGoreDesc = "Allow <color=" + Colors.textRed + ">blood</color> effects and <color=" + Colors.textRed + ">dismemberment</color>, this will effect the <color=orange>Fragment</color> perk if it is taken.";
        
        public static string optionsDisplayDeviceDesc = "Which <color=" + Colors.textRed + ">monitor</color> you wish to use.";
        public static string optionsDisplayAdapterDesc = "The <color=" + Colors.textRed + ">GPU</color> the game is using.";
        
        public static string optionsResolutionDesc = "Which <color=" + Colors.textRed + ">resolution</color> you wish to play at.";
        
        public static string optionsDisplayModeDesc = "Play <color=" + Colors.textRed + ">fullscreen</color> or in a seperate <color=" + Colors.textRed + ">Window</color>.";
        
        public static string optionsVSyncDesc = "<color=" + Colors.textRed + ">VSync</color> synchronizes the frame rate of a game with the refresh rate of the monitor to prevent <color=" + Colors.textRed + ">screen tearing</color>.\r\n\r\n<color=" + Colors.textRed + ">Screen tearing</color> happens when your GPU outputs frames faster than your monitor can display them.";
        public static string optionsVSyncDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>\r\nFrames limited to <color=" + Colors.textRed + ">Framerate Cap</color>.\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>V<color=" + Colors.textRed + ">-</color>Blank<color=" + Colors.textRed + ">]</color>\r\nFrames limited to monitors <color=" + Colors.textRed + ">maximum</color> refresh rate.\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Second V<color=" + Colors.textRed + ">-</color>Blank<color=" + Colors.textRed + ">]</color>\r\nFrames limited to <color=" + Colors.textRed + ">HALF</color> monitors maximum refresh rate.";
        
        public static string optionsFramerateCapDesc = "The target <color=" + Colors.textRed + ">framerate</color> the game should aim for assuming <color=" + Colors.textRed + ">VSync</color> is off and this setting is active. If you experience <color=" + Colors.textRed + ">screen tearing</color>, consider activating <color=" + Colors.textRed + ">VSync</color>.";
        public static string optionsFramerateCapDescAdditional = "<color=" + Colors.textRed + ">Warning</color>: This will be capped at your monitors <color=" + Colors.textRed + ">max refresh rate</color> if you attempt to go higher.";
        public static string optionsFramerateCapInfoDesc = "Your currently used display has a <color=" + Colors.textRed + ">maximum refresh rate</color> of ";
        
        public static string optionsQualityDesc = "A <color=" + Colors.textRed + ">global</color> quality setting, affects several settings.";
        public static string optionsQualityDescAdditional = "Affects the following:\r\n\r\n- <color=" + Colors.textRed + ">Anti-Alias</color>\r\n- <color=" + Colors.textRed + ">Volumetric Fog</color>\r\n- <color=" + Colors.textRed + ">Bloom</color>";
        
        public static string optionsAntiAliasDesc = "<color=" + Colors.textRed + ">Anti-aliasing</color> is a technique used to reduce the appearance of jagged edges. \r\n\r\n<color=" + Colors.textRed + ">FXAA</color> (<color=" + Colors.textRed + ">Fast Approximate Anti-Aliasing</color>)</color>\r\nDetects high-contrast edges.\r\n\r\n<color=" + Colors.textRed + ">SMAA</color> (<color=" + Colors.textRed + ">Subpixel Morphological Anti-Aliasing</color>)\r\nUses post-processing / pattern detection.\r\n\r\n<color=" + Colors.textRed + ">MSAA</color> (<color=" + Colors.textRed + ">Multisample Anti-Aliasing</color>)\r\nTakes multiple samples per pixel.\r\n\r\n<color=" + Colors.textRed + ">TAA</color> (<color=" + Colors.textRed + ">Temporal Anti-Aliasing</color>)\r\nAdvanced technique that blends frames together.";
        public static string optionsAntiAliasDescAdditional = "<color=" + Colors.textRed + ">[</color>FXAA<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Performance\r\n<color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>SMAA<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color>MSAA<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color>TAA<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>2x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>-</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>4x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>8x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>---</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsTaaQualityDesc = "When using <color=" + Colors.textRed + ">Temporal Anti Aliasing</color> (<color=" + Colors.textRed + ">TAA</color>) you can set what type of <color=" + Colors.textRed + ">sharpening</color> to use.\r\n\r\n<color=" + Colors.textRed + ">Low Quality</color>\r\nBasic filter.\r\n\r\n<color=" + Colors.textRed + ">Post Sharpen</color>\r\nBetter quality filter.\r\n\r\n<color=" + Colors.textRed + ">Contrast Adapt</color>\r\nUses <color=" + Colors.textOrange + ">AMD Fidelity-FX</color>, a more advanced sharpening method.";
        public static string optionsTaaQualityDescAdditional = "<color=" + Colors.textRed + ">[</color>Low Quality<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Performance\r\n<color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Post Sharpen<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + "><b>-</b></color> Performance\r\n<color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Contrast Adapt<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + "><b>---</b></color> Performance\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsVolumetricFogDesc = "The quality of the <color=" + Colors.textRed + ">Volumetric Fog</color>.";
        public static string optionsVolumetricFogDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textGreen + "><b>++++</b></color> Performance\r\n<color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>Low<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>-</b></color> Performance\r\n<color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>Medium<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>High<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>---</b>  </color>Performance\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsFOVDesc = "The <color=" + Colors.textRed + ">Field of View</color> of cameras.";
        public static string optionsFOVDescAdditional = "<color=" + Colors.textRed + ">Warning</color>: This may be capped in certain circumstances, if the game releases on a handheld for example.";
        
        public static string optionsRenderDistanceDesc = "The <color=" + Colors.textRed + ">distance</color> at which objects will be rendered in <color=" + Colors.textRed + ">meters</color>.";
        
        public static string optionsBloomDesc = "The intensity of the <color=" + Colors.textRed + ">Bloom</color> (glow) of emissive objects.";
        public static string optionsBloomDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Performance\r\n<color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>Low<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>-</b></color> Performance <color=" + Colors.textGreen + ">\r\n<b>+</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>Medium<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>High<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>---</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsHDRDesc = "<color=" + Colors.textRed + ">HDR</color> (<color=" + Colors.textRed + ">High Dynamic Range</color>) is a rendering technique that improves brightness, contrast, and color depth, allowing for more realistic and visually striking images.";
        public static string optionsHDRDescAdditional = "<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textGreen + "><b>+</b></color> Lighting and Shadow Quality\r\n\r\n<color=" + Colors.textGreen + "><b>+</b></color> Colour Realism\r\n\r\n<color=" + Colors.textGreen + ">+<b>+</b></color> Reduced detail loss in dark / bright areas\r\n\r\n<color=" + Colors.textRed + "><b>--</b></color> Performance";
        
        public static string optionsAnsioDesc = "<color=" + Colors.textRed + ">Anisotropic Filtering</color> (<color=" + Colors.textRed + ">AF</color>) enhances texture quality, especially for surfaces that are far away from the camera, making them appear sharper and more detailed without excessive blurring.";
        public static string optionsAnsioDescAdditional = "<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Performance\r\n<color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>2x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textGreen + "><b>+</b></color> Performance\r\n<color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>4x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>-</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>8x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>16x<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>---</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsTonemappingDesc = "<color=" + Colors.textRed + ">Tonemapping</color> converts <color=" + Colors.textRed + ">High Dynamic Range</color> (<color=" + Colors.textRed + ">HDR</color>) colors into a displayable range for <color=" + Colors.textRed + ">SDR</color> / <color=" + Colors.textRed + ">HDR</color> screens. It helps maintain details in bright and dark areas.\r\n\r\n<color=" + Colors.textRed + ">Warning</color>: will apply <color=" + Colors.textRed + ">Neutral</color> or <color=" + Colors.textRed + ">Aces</color> but will not apply specific tiers unless <color=" + Colors.textRed + ">HDR</color> is active.";
        public static string optionsTonemappingDescAdditional = "<color=" + Colors.textRed + ">[</color>Raymarching<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>Neutral - Reinhard<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Performance\r\n<color=" + Colors.textRed + "><b>---</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>Neutral - BT2390<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textGreen + "><b>++</b> </color>Performance\r\n<color=" + Colors.textRed + "><b>--</b> </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Raytracing<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>ACES - 1000<color=" + Colors.textRed + ">]</color> </color>\r\n<color=" + Colors.textRed + "><b>-</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+</b> </color>Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>ACES - 2000<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b> </color>Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>ACES - 4000<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>---</b></color> Performance\r\n<color=" + Colors.textGreen + "><b>+++</b> </color>Quality";
        
        public static string optionsGlobalIlluminationDesc = "<color=" + Colors.textRed + ">Global Illumination</color> (<color=" + Colors.textRed + ">GI</color>) is used to simulate the way light interacts with surfaces in a scene.\r\n\r\n<color=" + Colors.textRed + ">Raymarching</color> is done using the <color=" + Colors.textRed + ">CPU</color> and <color=" + Colors.textRed + ">Raytracing</color> is done using your <color=" + Colors.textRed + ">Graphics Card</color> if it is capable.\r\n\r\nIf you choose a <color=" + Colors.textRed + ">Raytracing</color> mode and your system does not support it, the game will default to <color=" + Colors.textRed + ">SSGI Medium</color>.";
        public static string optionsGlobalIlluminationDescAdditional = "<color=" + Colors.textRed + ">[</color>Raymarching - SSGI<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color>Raytracing - RTGI<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>Low<color=" + Colors.textRed + ">]</color>\r\n</color><color=" + Colors.textRed + ">-</color> Performance\r\n<color=" + Colors.textGreen + ">+ </color>Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>Medium<color=" + Colors.textRed + ">]</color>\r\n</color><color=" + Colors.textRed + ">-- </color>Performance\r\n<color=" + Colors.textGreen + ">++ </color>Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>High<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + ">---</color> Performance\r\n<color=" + Colors.textGreen + ">+++ </color>Quality\r\n\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Realtime<color=" + Colors.textRed + ">]</color>\r\n------\r\n</color><color=" + Colors.textRed + ">-- </color>Performance\r\n<color=" + Colors.textGreen + ">++ </color>Quality<color=" + Colors.textRed + ">";
        public static string optionsGlobalIlluminationResDesc = "Choose <color=" + Colors.textGreen + ">Full</color> or <color=" + Colors.textRed + ">Half</color> resolution for the <color=" + Colors.textRed + ">Raymarch</color> / <color=" + Colors.textRed + ">Raycast</color>.\r\n\r\n\r\nSwitch using the toggle.";
        public static string optionsGlobalIlluminationResDescAdditional = "<color=" + Colors.textRed + ">[</color>On<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + "><b>++</b> </color>Quality<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Full<color=" + Colors.textRed + ">]</color>                         </color><color=" + Colors.textRed + ">- </color>Performance    /   <color=" + Colors.textGreen + ">++ </color>Quality\r\n\r\n\r\n<color=" + Colors.textRed + ">[</color>Off<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color><color=" + Colors.textGreen + ">+++ </color>Performance<color=" + Colors.textRed + ">]</color>\r\n-------------------------------------------------<color=" + Colors.textRed + ">[</color>Half<color=" + Colors.textRed + ">]                   </color> </color><color=" + Colors.textGreen + ">+++ </color> Performance    /   <color=" + Colors.textRed + ">-- </color>Quality";
        public static string optionsGlobalIlluminationResStatusDesc = "When using <color=red>SSGI</color> or <color=red>RTGI</color> the resolution can be set to <color=red>Full</color> quality or <color=red>Half</color>.\r\n\r\n\r\n\r\nChange using the toggle at the top of this section.";
        public static string optionsGlobalIlluminationResStatusFullDesc = "Current Setting: On (<color=" + Colors.textGreen + ">Full</color>)";
        public static string optionsGlobalIlluminationResStatusHalfDesc = "Current Setting: Off (<color=" + Colors.textRed + ">Half</color>)";
        
        public static string optionsReflectionsDesc = "What type of <color=" + Colors.textRed + ">Reflections</color> you wish to use.\r\n\r\nIf you choose a <color=" + Colors.textRed + ">Raytracing</color> mode and your system does not support it, the game will default to <color=" + Colors.textRed + ">SSR Medium</color>.";
        public static string optionsReflectionsDescAdditional = "<color=" + Colors.textRed + ">[</color>Realtime<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>Realtime<color=" + Colors.textRed + ">]</color>\r\n</color><color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b> </color>Quality<color=" + Colors.textRed + ">\r\n\r\n\r\n[</color>Raymarching - SSR<color=" + Colors.textRed + ">]</color> <color=" + Colors.textRed + ">[</color>Raytracing - RTR<color=" + Colors.textRed + ">]</color>\r\n------\r\n<color=" + Colors.textRed + ">[</color>Low<color=" + Colors.textRed + ">]</color>\r\n<color=" + Colors.textRed + "><b>-</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>+</b> </color>Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>Medium<color=" + Colors.textRed + ">]</color>\r\n</color><color=" + Colors.textRed + "><b>--</b> </color>Performance\r\n<color=" + Colors.textGreen + "><b>++</b></color> Quality\r\n\r\n<color=" + Colors.textRed + ">[</color>High<color=" + Colors.textRed + ">]</color> </color>\r\n<color=" + Colors.textRed + "><b>---</b></color> Performance\r\n<color=" + Colors.textGreen + "><b>+++</b></color> Quality";
        
        public static string optionsPlanarReflectionsDesc = "<color=red>Planar Reflections</color> can be used in addition to other reflection methods to increase quality.\r\n\r\n\r\nActivate using the toggle.";
        public static string optionsPlanarReflectionsDescAdditional = "<color=red>[</color>Off<color=red>]</color> <color=red>[</color><color=green><b>+++</b> </color>Performance<color=red>]</color>\\r\\n-------------------------------------------------<color=red>[</color>Off<color=red>]</color>                     </color><color=green>+++ </color>Performance    /    <color=red>-- </color>Quality\r\n\r\n\r\n\r\n<color=red>[</color>On<color=red>]</color> <color=red>[</color><color=green>+++ </color>Quality<color=red>]</color>\r\n-------------------------------------------------<color=red>[</color>On<color=red>]</color>                      </color><color=red>-- </color>Performance    /    <color=green>+++ </color>Quality";
        
        public static string optionsShadowQualityDesc = "The quality of <color=" + Colors.textRed + ">shadows</color>.";
        public static string optionsShadowQualityDescAdditional = "<color=red>[</color>Low<color=red>]</color>\r\n<color=green><b>+++</b></color> Performance\r\n<color=red><b>---</b></color> Quality\r\n\r\n<color=red>[</color>Medium<color=red>]</color>\r\n<color=red><b>-</b> </color>Performance\r\n<color=green><b>+</b></color> Quality\r\n\r\n<color=red>[</color>High<color=red>]</color>\r\n<color=red><b>--</b> </color>Performance\r\n<color=green><b>++</b></color> Quality\r\n\r\n<color=red>[</color>Very High<color=red>]</color>\r\n<color=red><b>---</b> </color>Performance\r\n<color=green><b>+++</b></color> Quality";
        
        public static string optionsShadowDistanceDesc = "The distance at which <color=" + Colors.textRed + ">shadows</color> will be rendered.";
        
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
