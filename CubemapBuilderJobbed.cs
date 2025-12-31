using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using System.IO;

public class CubemapBuilderJobbed : EditorWindow
{
    const string left = "Left (-X / nX)";
    const string right = "Right (+X / pX)";
    const string bottom = "Bottom (-Y / nY)";
    const string top = "Top (+Y/ pY)";
    const string back = "Back (-Z / nZ)";
    const string front = "Front (+Z / pZ)";

    const string menu = "Tools/Cubemap Builder";
    const string assignTextures = "Assign Textures for Each Face";
    const string buildCubemapToggle = "Generate HDR Cubemap";
    const string buildCubemapMaxFaceSize = "Max Face Size";
    const string buildCubemap = "Build Cubemap";
    const string buildCubemapError = "Error";
    const string buildCubemapErrorDesc = "Please assign all 6 textures";
    const string buildCubemapOk = "Ok";
    const string newCubemap = "NewCubemap";
    const string cubemapExr = ".exr";
    const string cubemapPng = ".png";
    const string success = "Success";
    const string cubemapBuilt = "Cubemap built successfully! Face size:";
    const string exposure = "Exposure (EV)";
    const string selectFolder = "Select Folder";
    const string saveFolderl = "Save Folder";
    const string selectSaveFolder = "Select Save Folder";
    const string generatePreview = "Generate Preview";
    const string preview = "Preview";
    const string settings = "Settings";
    const string noPreview = "No preview generated yet.";
    const string memorySafety = "(clamped for memory safety)";
    const string textureSafety = "(clamped to input texture size)";

    [MenuItem(menu)]
    public static void OpenWindow() => GetWindow<CubemapBuilderJobbed>();

    public Texture2D nx, px, ny, py, nz, pz;
    public bool generateHDR = true;
    public int maxFaceSize = 1024; // maximum face size to budget memory
    public float exposureEV = 1f; // exposure in EV steps (logarithmic)

    // Save folder
    public string saveFolder = "Assets";

    // Preview fields
    private Texture2D previewTexture;
    private int previewSize = 128; // size of one face in preview

    // Tab fields
    private int currentTab = 0;
    private int lastTab = 0;
    private readonly string[] tabs = { settings, preview };

    // Safe max face size to avoid crashing
    private const int safeMaxFaceSizeHDR = 2048;
    private const int safeMaxFaceSizeLDR = 4096;

    private void OnGUI()
    {
        currentTab = GUILayout.Toolbar(currentTab, tabs);

        if (lastTab == 1 && currentTab != 1)
        {
            // left preview tab, free memory
            if (previewTexture != null)
            {
                DestroyImmediate(previewTexture);
                previewTexture = null;
            }
        }
        lastTab = currentTab;

        if (currentTab == 0)
        {
            DrawSettingsTab();
        }
        else if (currentTab == 1)
        {
            DrawPreviewTab();
        }
    }

    private void DrawSettingsTab()
    {
        GUILayout.Label(assignTextures, EditorStyles.boldLabel);
        nx = EditorGUILayout.ObjectField(left, nx, typeof(Texture2D), false) as Texture2D;
        px = EditorGUILayout.ObjectField(right, px, typeof(Texture2D), false) as Texture2D;
        ny = EditorGUILayout.ObjectField(bottom, ny, typeof(Texture2D), false) as Texture2D;
        py = EditorGUILayout.ObjectField(top, py, typeof(Texture2D), false) as Texture2D;
        nz = EditorGUILayout.ObjectField(back, nz, typeof(Texture2D), false) as Texture2D;
        pz = EditorGUILayout.ObjectField(front, pz, typeof(Texture2D), false) as Texture2D;

        generateHDR = EditorGUILayout.Toggle(buildCubemapToggle, generateHDR);
        maxFaceSize = EditorGUILayout.IntField(buildCubemapMaxFaceSize, maxFaceSize);
        exposureEV = EditorGUILayout.Slider(exposure, exposureEV, -5f, 5f);

        EditorGUILayout.Space(20);
        // Save folder selection
        GUILayout.Label(saveFolderl, EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        saveFolder = EditorGUILayout.TextField(saveFolder);
        if (GUILayout.Button(selectFolder, GUILayout.Width(120)))
        {
            string selected = EditorUtility.OpenFolderPanel(selectSaveFolder, saveFolder, "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                {
                    saveFolder = "Assets" + selected.Substring(Application.dataPath.Length);
                }
                else
                {
                    saveFolder = selected;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button(generatePreview))
        {
            GeneratePreview();
            currentTab = 1; // auto-switch to preview tab
        }

        if (GUILayout.Button(buildCubemap))
        {
            if (!nx || !px || !ny || !py || !nz || !pz)
            {
                EditorUtility.DisplayDialog(buildCubemapError, buildCubemapErrorDesc, buildCubemapOk);
                return;
            }

            string fileName = newCubemap + (generateHDR ? cubemapExr : cubemapPng);
            string path = Path.Combine(saveFolder, fileName);
            BuildCubemap(path);
        }
    }

    private void DrawPreviewTab()
    {
        GUILayout.Label(preview, EditorStyles.boldLabel);
        if (previewTexture != null)
        {
            Rect previewRect = GUILayoutUtility.GetRect(previewTexture.width, previewTexture.height, GUILayout.ExpandWidth(false));
            GUI.DrawTexture(previewRect, previewTexture, ScaleMode.ScaleToFit, false);
        }
        else
        {
            GUILayout.Label(noPreview, EditorStyles.helpBox);
        }
    }

    private void GeneratePreview()
    {
        if (!nx || !px || !ny || !py || !nz || !pz) return;

        Texture2D[] faces = { px, nx, py, ny, pz, nz };
        int size = previewSize;

        Texture2D cross = new Texture2D(size * 4, size * 3, TextureFormat.RGBA32, false);

        Vector2Int[] positions = new Vector2Int[]
        {
            new Vector2Int(2*size, 1*size), // Left (-X)
            new Vector2Int(0*size, 1*size), // Right (+X)
            new Vector2Int(1*size, 2*size), // Top (+Y)
            new Vector2Int(1*size, 0*size), // Bottom (-Y)
            new Vector2Int(1*size, 1*size), // Front (+Z)
            new Vector2Int(3*size, 1*size), // Back (-Z)
        };

        float exposureMultiplier = Mathf.Pow(2f, exposureEV);

        for (int i = 0; i < 6; i++)
        {
            Texture2D downsampled = Downsample(faces[i], size, TextureFormat.RGBA32);
            Color[] pixels = downsampled.GetPixels();
            for (int p = 0; p < pixels.Length; p++) pixels[p] *= exposureMultiplier;
            cross.SetPixels(positions[i].x, positions[i].y, size, size, pixels);
            DestroyImmediate(downsampled);
        }

        cross.Apply();
        previewTexture = cross;
    }

    private void BuildCubemap(string assetPath)
    {
        Texture2D[] faces = { px, nx, py, ny, pz, nz }; // +X,-X,+Y,-Y,+Z,-Z

        int smallestWidth = Mathf.Min(nx.width, px.width, ny.width, py.width, nz.width, pz.width);
        int smallestHeight = Mathf.Min(nx.height, px.height, ny.height, py.height, nz.height, pz.height);
        int requestedSize = maxFaceSize;

        int safeMax = generateHDR ? safeMaxFaceSizeHDR : safeMaxFaceSizeLDR;
        int clampedSize = Mathf.Min(requestedSize, smallestWidth, smallestHeight, safeMax);

        string reason = "";
        if (clampedSize < requestedSize)
        {
            if (clampedSize < requestedSize && clampedSize < smallestWidth || clampedSize < smallestHeight) reason = textureSafety;
            else reason = memorySafety;
        }

        int crossWidth = clampedSize * 4;
        int crossHeight = clampedSize * 3;

        TextureFormat tmpFormat = generateHDR ? TextureFormat.RGBAFloat : TextureFormat.RGBA32;
        Texture2D cross = new Texture2D(crossWidth, crossHeight, tmpFormat, false);

        Vector2Int[] positions = new Vector2Int[]
        {
            new Vector2Int(2*clampedSize, 1*clampedSize), // Left (-X)
            new Vector2Int(0*clampedSize, 1*clampedSize), // Right (+X)
            new Vector2Int(1*clampedSize, 2*clampedSize), // Top (+Y)
            new Vector2Int(1*clampedSize, 0*clampedSize), // Bottom (-Y)
            new Vector2Int(1*clampedSize, 1*clampedSize), // Front (+Z)
            new Vector2Int(3*clampedSize, 1*clampedSize), // Back (-Z)
        };

        float exposureMultiplier = Mathf.Pow(2f, exposureEV);

        for (int i = 0; i < 6; i++)
        {
            Texture2D downsampled = Downsample(faces[i], clampedSize, tmpFormat);
            Color[] pixels = downsampled.GetPixels();

            NativeArray<Color> nativePixels = new NativeArray<Color>(pixels.Length, Allocator.TempJob);
            nativePixels.CopyFrom(pixels);

            ProcessPixelsJob job = new ProcessPixelsJob
            {
                pixels = nativePixels,
                exposure = exposureMultiplier
            };
            JobHandle handle = job.Schedule(pixels.Length, 64);
            handle.Complete();

            nativePixels.CopyTo(pixels);
            nativePixels.Dispose();
            DestroyImmediate(downsampled);

            cross.SetPixels(positions[i].x, positions[i].y, clampedSize, clampedSize, pixels);
        }

        cross.Apply();

        byte[] bytes = generateHDR ? cross.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat) : cross.EncodeToPNG();
        File.WriteAllBytes(assetPath, bytes);
        DestroyImmediate(cross);

        AssetDatabase.ImportAsset(assetPath);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        importer.textureShape = TextureImporterShape.TextureCube;
        importer.generateCubemap = TextureImporterGenerateCubemap.FullCubemap;
        importer.sRGBTexture = false;
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        string fileName = Path.GetFileNameWithoutExtension(assetPath);
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
        if (tex != null) tex.name = fileName;

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(success, cubemapBuilt + clampedSize + reason, buildCubemapOk);
    }

    private Texture2D Downsample(Texture2D source, int size, TextureFormat format)
    {
        RenderTexture rt = RenderTexture.GetTemporary(size, size, 0, generateHDR ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGB32);
        Graphics.Blit(source, rt);

        Texture2D result = new Texture2D(size, size, format, false);
        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    [BurstCompile]
    struct ProcessPixelsJob : IJobParallelFor
    {
        public NativeArray<Color> pixels;
        public float exposure; // multiplier

        public void Execute(int index)
        {
            pixels[index] *= exposure;
        }
    }
}
