using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections; // requires Collections package from Package Manager
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using static GridGeneratorRectangle;
using UnityEngine.InputSystem;

public class GridGeneratorRectangle : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------------------------------------------
    // Multithreaded and BurstCompiled Grid creation script, no specific components needed as the script will add those automatically.
    // Primarily designed for games that have a large desired grid size with a lot of data per-tile.
    //----------------------------------------------------------------------------------------------------------------------------------------

    // - Each cell is added to a Dictionary that can then be used to store values for each grid cell, using the cell's gridLayout.CellToWorld as the key for each entry.

    // - Code has already been added which will save the EXACT center of the cell as a Vector3 (inside CellProperties Struct below), since gridLayout.CellToWorld actually returns
    // the bottom left position of the cell and GetCellCenter takes into account the Z axis of cellSize, the center position has been manually worked out for you.
    // these Vector3's can be used for A* pathfinding, as well as placement of objects on the grid.

    // - Although heavier operations inside GridDebug class are using jobs, it can take a while to debug large grids, use cautiously.

    // - The grid generation Memory Limiter will prevent you from generating grids if it estimated you dont have enough system memory to cater
    // for it but this can be overriden in the inspector, use cautiously.


    private bool ready;
    private bool gridDone;

    public enum GridOrientation
    {
        Vertical,
        Horizontal
    }
    [System.Serializable]
    public class GridProperties
    {
        [HideInInspector] public Grid grid;
        [HideInInspector] public GridLayout gridLayout;
        public Dictionary<Vector3, CellProperties> cellsDictionary = new Dictionary<Vector3, CellProperties>();
        [Tooltip("Override grid generation limiter - USE CAUTIOUSLY.")]
        public bool overrideMemoryLimiter;
        [Space(5)]
        public List<Vector3> cellsList = new List<Vector3>();
        public List<Vector3> guideCellList = new List<Vector3>();
        public List<Vector3> cornerCellList = new List<Vector3>();

        public Vector3 centerCellPosition;

        [Space(5)]
        [Header("Grid Settings")]
        [Tooltip("World Space grid orientation.")]
        public GridOrientation gridOrientation = GridOrientation.Vertical;
        [Tooltip("The desired grid size.")]
        public Vector2Int gridSize = new Vector2Int(20, 20);
        [Tooltip("The desired cell size.")]
        public Vector3 cellSize = new Vector3(1f, 1f, 0.1f);
        [Tooltip("The desired gap between each cell.")]
        public Vector2 cellGap = new Vector3(0f, 0f);
               
        [HideInInspector] public GameObject floorAnchor;
        [Header("Floor Settings")]
        public GameObject floor;
        public BoxCollider floorCollider;
        public Material floorMaterial;
    }
    public GridProperties gridProperties;


    [Serializable]
    public struct CellProperties // Add per-cell properties here
    {
        [SerializeField] public Vector3 cellCenterPosition;
        // Add more properties as needed
    }

    #region Inspector
    [Space(5)]
    [Header("Debug")]
    [Tooltip("Will add all the cells to the Cells List during initial grid generation to aid debugging.")]
    [SerializeField] private bool showCells = true;
    [Tooltip("Draws the cells visually, requires GridDebug class if not already in this script.")]
    [SerializeField] private bool drawCells = true;
    [Tooltip("Highlights important points on the grid by drawing them green.")]
    [SerializeField] private bool drawGuideCells = true;
    [Tooltip("World Space Position on the corner cells.")]
    [SerializeField] private bool debugCornerCells;
    [Tooltip("Information on the contents of the cells dictionary.")]
    [SerializeField] private bool debugDictionary;
    [Tooltip("Information on the contents of each CellProperties in the dictionary.")]
    [SerializeField] private bool debugCellProperties;
    [Tooltip("Estimate the runtime cost of the grid, given the properties you put in the CellProperties struct and the inspector values selected.")]
    [SerializeField] private bool debugMemoryRequirements;

    #endregion

    private void Awake()
    {
        ready = false;
        if (!TryGetComponent<Grid>(out gridProperties.grid))
        {
            gridProperties.grid = gameObject.AddComponent<Grid>();
            gridProperties.gridLayout = gridProperties.grid;
        }
        else
        {
            gridProperties.gridLayout = gridProperties.grid;
        }

        if (gridProperties.gridLayout)
        {
            ready = true;
        }

        // Create floor
        gridProperties.floorAnchor = new GameObject("GridFloorAnchor");
        gridProperties.floorAnchor.transform.position = this.gameObject.transform.position;
        gridProperties.floorAnchor.isStatic = true;
        gridProperties.floorAnchor.transform.parent = this.transform;

        gridProperties.floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gridProperties.floor.name = "GridFloor";

        MeshRenderer floorRenderer;
        gridProperties.floor.TryGetComponent(out floorRenderer);
        if (floorRenderer)
        {
            floorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            if (gridProperties.floorMaterial)
            {
                floorRenderer.material = gridProperties.floorMaterial;
            }
        }

        gridProperties.floor.transform.parent = gridProperties.floorAnchor.transform;
        MeshCollider mc = gridProperties.floor.GetComponent<MeshCollider>();
        if (mc != null) Destroy(mc);
        gridProperties.floorCollider = gridProperties.floor.AddComponent<BoxCollider>();

        Quaternion newRotation;
        switch (gridProperties.gridOrientation)
        {
            case GridOrientation.Vertical:
                newRotation = Quaternion.Euler(0f, 0f, 0f);
                gridProperties.floor.transform.SetLocalPositionAndRotation(new Vector3(0.5f, 0.5f, gridProperties.cellSize.z / 2), newRotation);

                gridProperties.floorAnchor.transform.localScale = new Vector3(gridProperties.gridSize.x * gridProperties.cellSize.x + (gridProperties.gridSize.x - 1) * gridProperties.cellGap.x, gridProperties.gridSize.y * gridProperties.cellSize.y + (gridProperties.gridSize.y - 1) * gridProperties.cellGap.y, 1f);
                break;

            case GridOrientation.Horizontal:
                newRotation = Quaternion.Euler(90f, 0f, 0f);
                gridProperties.floor.transform.SetLocalPositionAndRotation(new Vector3(0.5f, -gridProperties.cellSize.z / 2, 0.5f), newRotation);

                gridProperties.floorAnchor.transform.localScale = new Vector3(gridProperties.gridSize.x * gridProperties.cellSize.x + (gridProperties.gridSize.x - 1) * gridProperties.cellGap.x, 1f, gridProperties.gridSize.y * gridProperties.cellSize.y + (gridProperties.gridSize.y - 1) * gridProperties.cellGap.y);
                break;
        }
    }

    private void Start()
    {
        if (ready)
        {
            SetGrid();

            if (HasEnoughMemory(gridProperties.gridSize, typeof(CellProperties)))
            {
                StartCoroutine(GenerateGrid(gridProperties.cellsList, gridProperties.cornerCellList, gridProperties.guideCellList));
            }
            else
            {
                if (gridProperties.overrideMemoryLimiter)
                {
                    StartCoroutine(GenerateGrid(gridProperties.cellsList, gridProperties.cornerCellList, gridProperties.guideCellList));
                }
                else
                {
                    Debug.LogWarning("Not enough system memory to create grid given estimated memory cost!");
                }
            }
        }
    }

    private void Update()
    {
        DebugGrid();
    }

    IEnumerator GenerateGrid(List<Vector3> cellsList, List<Vector3> cornerCellList, List<Vector3> guideCellList)
    {
        for (int y = 0; y < gridProperties.gridSize.y; y++)
        {
            for (int x = 0; x < gridProperties.gridSize.x; x++)
            {
                Vector3 cell = gridProperties.gridLayout.CellToWorld(new Vector3Int(x, y, 0));

                cellsList.Add(cell);

                if ((x == gridProperties.gridSize.x / 2 && (y == 0 || y == gridProperties.gridSize.y - 1)) || (y == gridProperties.gridSize.y / 2 && (x == 0 || x == gridProperties.gridSize.x - 1))) // Check if the cell is at the halfway point along each edge
                {
                    guideCellList.Add(cell);
                }

                if (x == 0 || x == gridProperties.gridSize.x - 1)// Check if the cell is a corner cell
                {
                    if (y == 0 || y == gridProperties.gridSize.y - 1)
                    {
                        cornerCellList.Add(cell);
                    }
                }

                if (x == gridProperties.gridSize.x / 2 && y == gridProperties.gridSize.y / 2)// Check if the cell is the center of the grid
                {
                    gridProperties.centerCellPosition = cell;
                }
            }
        }

        yield return null;

        StartCoroutine(PopulateDictionary(cellsList, gridProperties.cellsDictionary));
    }
    void SetGrid()
    {
        if (gridProperties.cellSize.x < 0.1f)
        {
            gridProperties.cellSize.x = 0.1f;
        }
        if (gridProperties.cellSize.y < 0.1f)
        {
            gridProperties.cellSize.y = 0.1f;
        }

        //grid.cellLayout = (GridLayout.CellLayout)GridStyle.Rectangle;
        gridProperties.grid.cellGap = gridProperties.cellGap;

        switch (gridProperties.gridOrientation)
        {
            case GridOrientation.Vertical:
                gridProperties.grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
                gridProperties.grid.cellSize = gridProperties.cellSize;
                break;
            case GridOrientation.Horizontal:
                gridProperties.grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
                gridProperties.grid.cellSize = gridProperties.cellSize;
                break;
        }
    }
    void GridDone()
    {
        gridDone = true;
    }

    // To make sure you dont generate a grid too large
    public static bool HasEnoughMemory(Vector2Int gridSize, Type cellPropertiesType)
    {
        // Estimate the memory requirement in megabytes
        FieldInfo[] fields = cellPropertiesType.GetFields();
        int structSize = 0;
        foreach (FieldInfo field in fields)
        {
            structSize += Marshal.SizeOf(field.FieldType);
        }

        int totalCells = gridSize.x * gridSize.y;
        float memoryBytes = structSize * totalCells;
        float memoryMB = memoryBytes / (1024f * 1024f);

        // Total system memory in MB (not necessarily available memory!)
        float systemMemoryMB = SystemInfo.systemMemorySize;

        // Optional: Reserve headroom (e.g., 500MB buffer)
        const float safetyBufferMB = 500f;

        return memoryMB + safetyBufferMB < systemMemoryMB;
    }

    #region Jobs
    [BurstCompile]
    struct PopulateDictionaryJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> cellsList;
        [WriteOnly] public NativeParallelHashMap<Vector3, CellProperties>.ParallelWriter cellsDictionary;
        public GridOrientation gridOrientation;
        public float cellSizeX;
        public float cellSizeY;

        public void Execute(int index)
        {
            Vector3 key = cellsList[index];
            CellProperties cellProperties = new CellProperties();

            switch (gridOrientation)
            {
                case GridOrientation.Vertical:
                    cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0f);
                    break;
                case GridOrientation.Horizontal:
                    cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, 0f, cellSizeY * 0.5f);
                    break;
            }

            cellsDictionary.TryAdd(key, cellProperties);
        }
    }
    IEnumerator PopulateDictionary(List<Vector3> cellsList, Dictionary<Vector3, CellProperties> cellsDictionary)
    {
        NativeArray<Vector3> nativeCellsArray = new NativeArray<Vector3>(cellsList.ToArray(), Allocator.TempJob);
        NativeParallelHashMap<Vector3, CellProperties> nativeCellsDictionary = new NativeParallelHashMap<Vector3, CellProperties>(nativeCellsArray.Length, Allocator.TempJob);

        PopulateDictionaryJob populateJob = new PopulateDictionaryJob
        {
            cellsList = nativeCellsArray, // Pass nativeCellsArray to job
            cellsDictionary = nativeCellsDictionary.AsParallelWriter(),
            gridOrientation = gridProperties.gridOrientation,
            cellSizeX = gridProperties.gridLayout.cellSize.x,
            cellSizeY = gridProperties.gridLayout.cellSize.y
        };

        int numThreads = System.Environment.ProcessorCount;
        JobHandle jobHandle = populateJob.Schedule(cellsList.Count, numThreads);
        jobHandle.Complete();

        foreach (var kvp in nativeCellsDictionary)// Copy back the result to the managed dictionary
        {
            cellsDictionary[kvp.Key] = kvp.Value;
        }

        nativeCellsArray.Dispose();
        nativeCellsDictionary.Dispose();

        yield return null;

        StartCoroutine(SetDictionary(cellsDictionary));
    }

    [BurstCompile]
    struct SetDictionaryJob : IJob
    {
        public NativeParallelHashMap<Vector3, CellProperties> cellsDictionary;
        public NativeList<KeyValuePair<Vector3, CellProperties>> changesList;
        public GridOrientation gridOrientation;
        public float cellSizeX;
        public float cellSizeY;

        public void Execute()
        {
            foreach (var kvp in cellsDictionary)
            {
                Vector3 key = kvp.Key;
                CellProperties cellProperties = kvp.Value;

                switch (gridOrientation)
                {
                    case GridOrientation.Vertical:
                        cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0f);
                        changesList.Add(new KeyValuePair<Vector3, CellProperties>(key, cellProperties));
                        break;

                    case GridOrientation.Horizontal:
                        cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, 0f, cellSizeY * 0.5f);
                        changesList.Add(new KeyValuePair<Vector3, CellProperties>(key, cellProperties));
                        break;
                }
            }

            foreach (var change in changesList)
            {
                cellsDictionary[change.Key] = change.Value;
            }
        }
    }
    IEnumerator SetDictionary(Dictionary<Vector3, CellProperties> cellsDictionary)
    {
        NativeParallelHashMap<Vector3, CellProperties> nativeCellsDictionary = new NativeParallelHashMap<Vector3, CellProperties>(cellsDictionary.Count, Allocator.TempJob);
        NativeList<KeyValuePair<Vector3, CellProperties>> nativeChangesList = new NativeList<KeyValuePair<Vector3, CellProperties>>(Allocator.TempJob);

        foreach (var kvp in cellsDictionary)
        {
            nativeCellsDictionary.Add(kvp.Key, kvp.Value);
        }

        SetDictionaryJob setJob = new SetDictionaryJob
        {
            cellsDictionary = nativeCellsDictionary,
            changesList = nativeChangesList,
            gridOrientation = gridProperties.gridOrientation,
            cellSizeX = gridProperties.gridLayout.cellSize.x,
            cellSizeY = gridProperties.gridLayout.cellSize.y
        };

        JobHandle jobHandle = setJob.Schedule();
        jobHandle.Complete();

        foreach (var kvp in nativeCellsDictionary)// Copy back the result to the managed dictionary
        {
            cellsDictionary[kvp.Key] = kvp.Value;
        }

        nativeCellsDictionary.Dispose();
        nativeChangesList.Dispose();

        if (!showCells)
        {
            gridProperties.cellsList.Clear();
        }

        yield return null;

        GridDone();
    }
    #endregion

    #region GridDebug
    private void DebugGrid()
    {
        if (gridDone)
        {
            if (debugDictionary)
            {
                if (gridProperties.cellsDictionary.Count > 0)
                {
                    GridDebug.DebugGridCellsDictionary(this);
                    debugDictionary = false;
                }
                else
                {
                    Debug.Log("No cells in: " + gridProperties.cellsDictionary + " to debug, did you generate a grid yet?");
                    debugDictionary = false;
                }
            }
            if (debugCellProperties)
            {
                if (gridProperties.cellsDictionary.Count > 0)
                {
                    GridDebug.DebugCellPropertiesDictionary(this);
                    debugCellProperties = false;
                }
                else
                {
                    Debug.Log("No cells in: " + gridProperties.cellsDictionary + " to debug, did you generate a grid yet?");
                    debugCellProperties = false;
                }
            }
            if (debugCornerCells && gridProperties.cellsDictionary.Count > 0)
            {
                if (gridProperties.cornerCellList.Count > 0)
                {
                    foreach (Vector3 cornerCell in gridProperties.cornerCellList)
                    {
                        Debug.Log("Corner Cell World Space Position: " + cornerCell);
                    }
                }
                debugCornerCells = false;
            }
        }

        if (debugMemoryRequirements)
        {
            Debug.Log("Accounting for CellProperties struct and inspector properties, estimated memory cost : " + GridDebug.CalculateMemoryRequirement(gridProperties.gridSize, typeof(CellProperties)));
            debugMemoryRequirements = false;
        }
    }
    private void OnDrawGizmos()
    {
        if (debugMemoryRequirements)
        {
            Debug.Log("Accounting for CellProperties struct and inspector properties, estimated memory cost : " + GridDebug.CalculateMemoryRequirement(gridProperties.gridSize, typeof(CellProperties)));
            debugMemoryRequirements = false;
        }

        if (gridDone)
        {
            if (!drawCells) return;

            Color gizmoColor;
            Vector3 desiredCellSize;
            Vector3 offset;

            foreach (var kvp in gridProperties.cellsDictionary)
            {
                switch (gridProperties.gridOrientation)
                {
                    case GridOrientation.Vertical:
                        if (drawGuideCells)
                        {
                            if (kvp.Key == gridProperties.centerCellPosition)
                            {
                                desiredCellSize = new Vector3(gridProperties.grid.cellSize.x, gridProperties.grid.cellSize.y, gridProperties.cellSize.z + 3f);
                                gizmoColor = Color.red;
                            }
                            else if (gridProperties.guideCellList.Contains(kvp.Key) || gridProperties.cornerCellList.Contains(kvp.Key))
                            {
                                desiredCellSize = new Vector3(gridProperties.grid.cellSize.x, gridProperties.grid.cellSize.y, gridProperties.cellSize.z + 1f);
                                gizmoColor = Color.green;
                            }
                            else
                            {
                                desiredCellSize = gridProperties.cellSize;
                                gizmoColor = Color.white;
                            }
                        }
                        else
                        {
                            desiredCellSize = gridProperties.cellSize;
                            gizmoColor = Color.white;
                        }
                        GridDebug.AddToBatch(kvp.Key + (gridProperties.gridLayout.cellSize * 0.5f), gizmoColor, desiredCellSize);
                        break;

                    case GridOrientation.Horizontal:
                        if (drawGuideCells)
                        {
                            if (kvp.Key == gridProperties.centerCellPosition)
                            {
                                desiredCellSize = new Vector3(gridProperties.grid.cellSize.x, gridProperties.cellSize.z + 3f, gridProperties.cellSize.y);
                                gizmoColor = Color.red;
                            }
                            else if (gridProperties.guideCellList.Contains(kvp.Key) || gridProperties.cornerCellList.Contains(kvp.Key))
                            {
                                desiredCellSize = new Vector3(gridProperties.grid.cellSize.x, gridProperties.grid.cellSize.z + 1f, gridProperties.grid.cellSize.y);
                                gizmoColor = Color.green;
                            }
                            else
                            {
                                desiredCellSize = new Vector3(gridProperties.grid.cellSize.x, gridProperties.grid.cellSize.z, gridProperties.grid.cellSize.y);
                                gizmoColor = Color.white;
                            }
                        }
                        else
                        {
                            desiredCellSize = new Vector3(gridProperties.grid.cellSize.x, gridProperties.grid.cellSize.z, gridProperties.cellSize.y);
                            gizmoColor = Color.white;
                        }
                        offset = new Vector3(gridProperties.gridLayout.cellSize.x * 0.5f, -0.05f, gridProperties.gridLayout.cellSize.y * 0.5f);
                        GridDebug.AddToBatch(kvp.Key + offset, gizmoColor, desiredCellSize);
                        break;
                }
            }
            GridDebug.ExecuteGizmoBatch(gridProperties.gridOrientation); // Execute the batched Gizmos calls...they deserved it
        }
    }
    #endregion
}

public static class GridDebug
{
    public static string CalculateMemoryRequirement(Vector2Int gridSize, Type cellPropertiesType)
    {
        FieldInfo[] fields = cellPropertiesType.GetFields();

        int structSize = 0;
        foreach (FieldInfo field in fields)
        {
            structSize += Marshal.SizeOf(field.FieldType);
        }

        int totalCells = gridSize.x * gridSize.y;
        float memoryRequirementBytes = structSize * totalCells;

        const long gigabyte = 1024 * 1024 * 1024;
        const long megabyte = 1024 * 1024;
        const long kilobyte = 1024;

        if (memoryRequirementBytes > gigabyte)
        {
            return $"{memoryRequirementBytes / gigabyte:F1}GB";
        }
        else if (memoryRequirementBytes > megabyte)
        {
            return $"{memoryRequirementBytes / megabyte:F1}MB";
        }
        else if (memoryRequirementBytes > kilobyte)
        {
            return $"{memoryRequirementBytes / kilobyte:F1}KB";
        }
        else
        {
            return $"{memoryRequirementBytes}Bytes";
        }
    }

    #region Gizmo Batching

    private struct GizmoBatchData
    {
        public Vector3 position;
        public Color color;
        public Vector3 cellSize;
    }
    private static List<GizmoBatchData> gizmoBatch = new List<GizmoBatchData>();
    public static void AddToBatch(Vector3 position, Color color, Vector3 cellSize)
    {
        gizmoBatch.Add(new GizmoBatchData { position = position, color = color, cellSize = cellSize });
    }
    public static void ExecuteGizmoBatch(GridGeneratorRectangle.GridOrientation gridOrientation)
    {
        foreach (var data in gizmoBatch)
        {
            Gizmos.color = data.color;

            Gizmos.DrawWireCube(data.position, data.cellSize);
        }
        gizmoBatch.Clear();
    }
    #endregion

    #region Jobs
    [BurstCompile]
    public struct DebugGridCellsDictionaryJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Keys;
        [ReadOnly] public NativeArray<GridGeneratorRectangle.CellProperties> Values;

        public void Execute(int index)
        {
            Debug.Log($"Key: {Keys[index]}, Value: {Values[index]}");
        }
    }
    public static void DebugGridCellsDictionary(GridGeneratorRectangle gridGenerator)
    {
        NativeArray<Vector3> keysArray = new NativeArray<Vector3>(gridGenerator.gridProperties.cellsDictionary.Keys.ToArray(), Allocator.TempJob);
        NativeArray<GridGeneratorRectangle.CellProperties> valuesArray = new NativeArray<GridGeneratorRectangle.CellProperties>(gridGenerator.gridProperties.cellsDictionary.Values.ToArray(), Allocator.TempJob);

        DebugGridCellsDictionaryJob jobData = new DebugGridCellsDictionaryJob
        {
            Keys = keysArray,
            Values = valuesArray
        };

        int numThreads = System.Environment.ProcessorCount;
        JobHandle jobHandle = jobData.Schedule(keysArray.Length, numThreads);

        jobHandle.Complete();

        keysArray.Dispose();
        valuesArray.Dispose();

        Debug.Log($"Total Dictionary Entry Count: {gridGenerator.gridProperties.cellsDictionary.Count}");
    }

    // ---------------
    // Do NOT BurstCompile, values will not show properly.
    private static List<string> logMessages;
    public struct DebugCellPropertiesJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Keys;
        [ReadOnly] public NativeArray<CellProperties> CellPropertiesArray;

        public void Execute(int index)
        {
            Vector3 key = Keys[index];
            CellProperties cellProperties = CellPropertiesArray[index];

            logMessages.Add($"Key: {key}");
            logMessages.Add($"cellCenterPosition: {cellProperties.cellCenterPosition}");
            logMessages.Add("-----------------------");
        }
    }
    public static void DebugCellPropertiesDictionary(GridGeneratorRectangle gridGenerator)
    {
        var keysArray = new NativeArray<Vector3>(gridGenerator.gridProperties.cellsDictionary.Keys.ToArray(), Allocator.TempJob);
        var valuesArray = new NativeArray<CellProperties>(gridGenerator.gridProperties.cellsDictionary.Values.ToArray(), Allocator.TempJob);

        logMessages = new List<string>();

        DebugCellPropertiesJob jobData = new DebugCellPropertiesJob
        {
            Keys = keysArray,
            CellPropertiesArray = valuesArray,
        };

        int numThreads = System.Environment.ProcessorCount;
        JobHandle jobHandle = jobData.Schedule(keysArray.Length, numThreads);
        jobHandle.Complete();

        // Display log messages in order
        foreach (var message in logMessages)
        {
            Debug.Log(message);
        }

        keysArray.Dispose();
        valuesArray.Dispose();
        logMessages.Clear();
    }
    #endregion
}
