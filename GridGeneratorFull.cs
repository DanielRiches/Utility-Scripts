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
using static GridGeneratorFull;
using UnityEngine.InputSystem;

public class GridGeneratorFull : MonoBehaviour
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
    // for it,this can be overriden in the inspector, use cautiously.

    [Serializable]
    public struct CellProperties // Add per-cell properties here
    {
        [SerializeField] public Vector3 cellCenterPosition;
        // Add more properties as needed
    }

    public Grid grid;
    public GridLayout gridLayout;
    public Dictionary<Vector3, CellProperties> cellsDictionary = new Dictionary<Vector3, CellProperties>();
    [field: SerializeField, Header("Cells")] private List<Vector3> cellsList = new List<Vector3>();
    [SerializeField] private List<Vector3> guideCellList = new List<Vector3>();
    [SerializeField] private List<Vector3> cornerCellList = new List<Vector3>();
    [SerializeField] private Vector3 centerCellPosition;

    [SerializeField] private GameObject floor;
    [SerializeField] private Material floorMaterial;

    #region Hidden Properties
    public enum GridOrientation
    {
        Vertical,
        Horizontal
    }
    public enum GridStyle
    {
        Rectangle,
        Hexagon
    }

    private BoxCollider gridCollider;
    private bool ready;
    private bool gridDone;

    private GameObject floorAnchor;
    #endregion

    #region Inspector
    [Space(5)]
    [Header("Debug")]
    [Tooltip("Will add all the cells to the Cells List during initial grid generation to aid debugging.")]
    [SerializeField] private bool showCells = true;
    [Tooltip("Draws the cells visually, requires GridDebug class if not already in this script.")]
    [SerializeField] private bool drawCells;
    [Tooltip("Highlights important points on the grid by drawing them green.")]
    [SerializeField] private bool drawGuideCells;
    [Tooltip("World Space Position on the corner cells.")]
    [SerializeField] private bool debugCornerCells;
    [Tooltip("Information on the contents of the cells dictionary.")]
    [SerializeField] private bool debugDictionary;
    [Tooltip("Information on the contents of each CellProperties in the dictionary.")]
    [SerializeField] private bool debugCellProperties;
    [Tooltip("Estimate the runtime cost of the grid, given the properties you put in the CellProperties struct and the inspector values selected.")]
    [SerializeField] private bool debugMemoryRequirements;
    [Tooltip("Override grid generation limiter - USE CAUTIOUSLY.")]
    [SerializeField] private bool overrideMemoryLimiter;
    [Space(5)]
    [Header("Grid Settings")]
    [Tooltip("Style of grid")]
    [SerializeField] private GridStyle gridStyle = GridStyle.Rectangle;
    [Tooltip("World Space grid orientation.")]
    [SerializeField] private GridOrientation gridOrientation = GridOrientation.Vertical;
    [Tooltip("The desired grid size.")]
    public Vector2Int gridSize = new Vector2Int(11, 11);
    [Tooltip("The desired cell size.")]
    [SerializeField] private Vector3 cellSize = new Vector3(1f, 1f, 0.1f);
    [Tooltip("The desired gap between each cell.")]
    [SerializeField] private Vector2 cellGap = new Vector3(0f, 0f);
    #endregion   
    
    private void Awake()
    {
        ready = false;
        if (!TryGetComponent<Grid>(out grid))
        {
            grid = gameObject.AddComponent<Grid>();
            gridLayout = grid;
        }
        else
        {
            gridLayout = grid;
        }

        if (gridLayout)
        {
            ready = true;
        }

        // Create floor
        floorAnchor = new GameObject("GridFloorAnchor");
        floorAnchor.transform.position = this.gameObject.transform.position;
        floorAnchor.isStatic = true;        
        floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "GridFloor";
        if (floor.TryGetComponent(out Renderer r)) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        floor.transform.parent = floorAnchor.transform;
        Quaternion newRotation;
        switch (gridOrientation)
        {
            case GridOrientation.Vertical:
                newRotation = Quaternion.Euler(0f, 0f, 0f);
                floor.transform.SetLocalPositionAndRotation(new Vector3(0.5f, 0.5f, cellSize.z / 2), newRotation);
                floorAnchor.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
                break;
            case GridOrientation.Horizontal:
                newRotation = Quaternion.Euler(90f, 0f, 0f);
                floor.transform.SetLocalPositionAndRotation(new Vector3(0.5f, -cellSize.z / 2, 0.5f), newRotation);
                floorAnchor.transform.localScale = new Vector3(gridSize.x, 1, gridSize.y);
                break;
        }
        floor.isStatic = true;        
        floor.AddComponent<BoxCollider>();
        if (floor.TryGetComponent(out MeshCollider collider)) Destroy(collider);
    }

    private void Start()
    {
        if (ready)
        {
            SetGrid();

            if (HasEnoughMemory(gridSize, typeof(CellProperties)))
            {
                StartCoroutine(GenerateGrid(cellsList, cornerCellList, guideCellList));
            }
            else
            {
                if (overrideMemoryLimiter)
                {
                    StartCoroutine(GenerateGrid(cellsList, cornerCellList, guideCellList));
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
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 cell = gridLayout.CellToWorld(new Vector3Int(x, y, 0));

                cellsList.Add(cell);

                if ((x == gridSize.x / 2 && (y == 0 || y == gridSize.y - 1)) || (y == gridSize.y / 2 && (x == 0 || x == gridSize.x - 1))) // Check if the cell is at the halfway point along each edge
                {
                    guideCellList.Add(cell);
                }

                if (x == 0 || x == gridSize.x - 1)// Check if the cell is a corner cell
                {
                    if (y == 0 || y == gridSize.y - 1)
                    {
                        cornerCellList.Add(cell);
                    }
                }

                if (x == gridSize.x / 2 && y == gridSize.y / 2)// Check if the cell is the center of the grid
                {
                    centerCellPosition = cell;
                }
            }
        }

        yield return null;

        StartCoroutine(PopulateDictionary(cellsList, cellsDictionary));
    }
    void SetGrid()
    {
        if (cellSize.x < 0.1f)
        {
            cellSize.x = 0.1f;
        }
        if (cellSize.y < 0.1f)
        {
            cellSize.y = 0.1f;
        }

        switch (gridStyle)
        {
            case GridStyle.Rectangle:
                grid.cellLayout = (GridLayout.CellLayout)GridStyle.Rectangle;
                grid.cellGap = cellGap;
                break;
            case GridStyle.Hexagon:
                grid.cellLayout = (GridLayout.CellLayout)GridStyle.Hexagon;
                break;
        }
        switch (gridOrientation)
        {
            case GridOrientation.Vertical:
                grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
                grid.cellSize = cellSize;
                break;
            case GridOrientation.Horizontal:
                grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
                grid.cellSize = cellSize;
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
        public GridStyle gridStyle;
        public GridOrientation gridOrientation;
        public float cellSizeX;
        public float cellSizeY;

        public void Execute(int index)
        {
            Vector3 key = cellsList[index];
            CellProperties cellProperties = new CellProperties();

            switch (gridStyle)
            {
                case GridStyle.Rectangle:
                    switch (gridOrientation)
                    {
                        case GridOrientation.Vertical:
                            cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0f);
                            break;
                        case GridOrientation.Horizontal:
                            cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, 0f, cellSizeY * 0.5f);
                            break;
                    }
                    break;
                case GridStyle.Hexagon:
                    cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0f);
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
            gridStyle = gridStyle,
            gridOrientation = gridOrientation,
            cellSizeX = gridLayout.cellSize.x,
            cellSizeY = gridLayout.cellSize.y
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
        public GridStyle gridStyle;
        public GridOrientation gridOrientation;
        public float cellSizeX;
        public float cellSizeY;

        public void Execute()
        {
            foreach (var kvp in cellsDictionary)
            {
                Vector3 key = kvp.Key;
                CellProperties cellProperties = kvp.Value;

                switch (gridStyle)
                {
                    case GridStyle.Rectangle:
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
                        break;

                    case GridStyle.Hexagon:
                        cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, cellSizeY * 0.5f, 0f);
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
            gridStyle = gridStyle,
            gridOrientation = gridOrientation,
            cellSizeX = gridLayout.cellSize.x,
            cellSizeY = gridLayout.cellSize.y
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
            cellsList.Clear();
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
                if (cellsDictionary.Count > 0)
                {
                    GridDebug.DebugGridCellsDictionary(this);
                    debugDictionary = false;
                }
                else
                {
                    Debug.Log("No cells in: " + cellsDictionary + " to debug, did you generate a grid yet?");
                    debugDictionary = false;
                }
            }
            if (debugCellProperties)
            {
                if (cellsDictionary.Count > 0)
                {
                    GridDebug.DebugCellPropertiesDictionary(this);
                    debugCellProperties = false;
                }
                else
                {
                    Debug.Log("No cells in: " + cellsDictionary + " to debug, did you generate a grid yet?");
                    debugCellProperties = false;
                }
            }
            if (debugCornerCells && cellsDictionary.Count > 0)
            {
                if (cornerCellList.Count > 0)
                {
                    foreach (Vector3 cornerCell in cornerCellList)
                    {
                        Debug.Log("Corner Cell World Space Position: " + cornerCell);
                    }
                }
                debugCornerCells = false;
            }
        }

        if (debugMemoryRequirements)
        {
            Debug.Log("Accounting for CellProperties struct and inspector properties, estimated memory cost : " + GridDebug.CalculateMemoryRequirement(gridSize, typeof(CellProperties)));
            debugMemoryRequirements = false;
        }
    }
    private void OnDrawGizmos()
    {
        if (debugMemoryRequirements)
        {
            Debug.Log("Accounting for CellProperties struct and inspector properties, estimated memory cost : " + GridDebug.CalculateMemoryRequirement(gridSize, typeof(CellProperties)));
            debugMemoryRequirements = false;
        }

        if (gridDone)
        {
            if (!drawCells) return;

            Color gizmoColor;
            Vector3 desiredCellSize;
            Vector3 offset;

            foreach (var kvp in cellsDictionary)
            {
                switch (gridStyle)
                {
                    case GridStyle.Rectangle:
                        switch (gridOrientation)
                        {
                            case GridOrientation.Vertical:
                                if (drawGuideCells)
                                {
                                    if (kvp.Key == centerCellPosition)
                                    {
                                        desiredCellSize = new Vector3(grid.cellSize.x, grid.cellSize.y, cellSize.z + 3f);
                                        gizmoColor = Color.red;
                                    }
                                    else if (guideCellList.Contains(kvp.Key) || cornerCellList.Contains(kvp.Key))
                                    {
                                        desiredCellSize = new Vector3(grid.cellSize.x, grid.cellSize.y, cellSize.z + 1f);
                                        gizmoColor = Color.green;
                                    }
                                    else
                                    {
                                        desiredCellSize = cellSize;
                                        gizmoColor = Color.white;
                                    }
                                }
                                else
                                {
                                    desiredCellSize = cellSize;
                                    gizmoColor = Color.white;
                                }
                                GridDebug.AddToBatch(kvp.Key + (gridLayout.cellSize * 0.5f), gizmoColor, GridDebug.GizmoType.Cube, desiredCellSize);
                                break;

                            case GridOrientation.Horizontal:
                                if (drawGuideCells)
                                {
                                    if (kvp.Key == centerCellPosition)
                                    {
                                        desiredCellSize = new Vector3(grid.cellSize.x, cellSize.z + 3f, cellSize.y);
                                        gizmoColor = Color.red;
                                    }
                                    else if (guideCellList.Contains(kvp.Key) || cornerCellList.Contains(kvp.Key))
                                    {
                                        desiredCellSize = new Vector3(grid.cellSize.x, grid.cellSize.z + 1f, grid.cellSize.y);
                                        gizmoColor = Color.green;
                                    }
                                    else
                                    {
                                        desiredCellSize = new Vector3(grid.cellSize.x, grid.cellSize.z, grid.cellSize.y);
                                        gizmoColor = Color.white;
                                    }
                                }
                                else
                                {
                                    desiredCellSize = new Vector3(grid.cellSize.x, grid.cellSize.z, cellSize.y);
                                    gizmoColor = Color.white;
                                }
                                offset = new Vector3(gridLayout.cellSize.x * 0.5f, -0.05f, gridLayout.cellSize.y * 0.5f);
                                GridDebug.AddToBatch(kvp.Key + offset, gizmoColor, GridDebug.GizmoType.Cube, desiredCellSize);
                                break;
                        }
                        break;

                    case GridStyle.Hexagon:
                        if (kvp.Key == centerCellPosition)
                        {
                            gizmoColor = Color.red;
                        }
                        else if (guideCellList.Contains(kvp.Key))
                        {
                            gizmoColor = Color.green;
                        }
                        else
                        {
                            gizmoColor = Color.white;
                        }
                        GridDebug.AddToBatch(kvp.Key, gizmoColor, GridDebug.GizmoType.Hexagon, grid.cellSize);
                        break;
                }
            }
            GridDebug.ExecuteGizmoBatch(gridOrientation); // Execute the batched Gizmos calls...they deserved it
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
    public enum GizmoType
    {
        Cube,
        Hexagon
    }
    private struct GizmoBatchData
    {
        public Vector3 position;
        public Color color;
        public GizmoType type;
        public Vector3 cellSize;
    }
    private static List<GizmoBatchData> gizmoBatch = new List<GizmoBatchData>();
    public static void AddToBatch(Vector3 position, Color color, GizmoType type, Vector3 cellSize)
    {
        gizmoBatch.Add(new GizmoBatchData { position = position, color = color, type = type, cellSize = cellSize });
    }
    public static void ExecuteGizmoBatch(GridGeneratorFull.GridOrientation gridOrientation)
    {
        foreach (var data in gizmoBatch)
        {
            Gizmos.color = data.color;

            switch (data.type)
            {
                case GizmoType.Cube:
                    Gizmos.DrawWireCube(data.position, data.cellSize);
                    break;

                case GizmoType.Hexagon:
                    DrawHexagon(data.position, data.cellSize, gridOrientation);
                    break;
            }
        }
        gizmoBatch.Clear();
    }
    #endregion

    #region Jobs
    [BurstCompile]
    public struct DebugGridCellsDictionaryJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> Keys;
        [ReadOnly] public NativeArray<GridGeneratorFull.CellProperties> Values;

        public void Execute(int index)
        {
            Debug.Log($"Key: {Keys[index]}, Value: {Values[index]}");
        }
    }
    public static void DebugGridCellsDictionary(GridGeneratorFull gridGenerator)
    {
        NativeArray<Vector3> keysArray = new NativeArray<Vector3>(gridGenerator.cellsDictionary.Keys.ToArray(), Allocator.TempJob);
        NativeArray<GridGeneratorFull.CellProperties> valuesArray = new NativeArray<GridGeneratorFull.CellProperties>(gridGenerator.cellsDictionary.Values.ToArray(), Allocator.TempJob);

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

        Debug.Log($"Total Dictionary Entry Count: {gridGenerator.cellsDictionary.Count}");
    }

    // ----------------
    [BurstCompile]
    private struct DrawHexagonJob : IJobParallelFor
    {
        public float HexagonSizeX;
        public float HexagonSizeY;
        public Vector3 Position;
        public GridGeneratorFull.GridOrientation GridOrientation;
        public NativeArray<Vector3> HexagonVertices;

        public void Execute(int i)
        {
            float angle = 60 * i;
            float angleRad = Mathf.PI / 180 * angle;
            float x = Mathf.Cos(angleRad) * HexagonSizeX / 2f;
            float z = Mathf.Sin(angleRad) * HexagonSizeY / 2f;

            HexagonVertices[i] = Position + new Vector3(x, 0f, z);
            HexagonVertices[i] = RotateAroundY(HexagonVertices[i], Position, 90);

            if (GridOrientation == GridGeneratorFull.GridOrientation.Vertical)
            {
                HexagonVertices[i] = ApplyRotationOffset(HexagonVertices[i], new Vector3(90, 0, 0), Position);
            }
        }

        private static Vector3 RotateAroundY(Vector3 point, Vector3 pivot, float angleY)
        {
            Vector3 direction = point - pivot;
            direction = Quaternion.Euler(0, angleY, 0) * direction;
            return pivot + direction;
        }

        private static Vector3 ApplyRotationOffset(Vector3 point, Vector3 offset, Vector3 pivot)
        {
            return Quaternion.Euler(offset) * (point - pivot) + pivot;
        }
    }
    public static void DrawHexagon(Vector3 position, Vector2 hexagonSize, GridGeneratorFull.GridOrientation gridOrientation)
    {
        Vector3[] hexagonVertices = new Vector3[6];
        NativeArray<Vector3> hexagonVerticesNative = new NativeArray<Vector3>(hexagonVertices, Allocator.TempJob);

        DrawHexagonJob jobData = new DrawHexagonJob
        {
            HexagonSizeX = hexagonSize.x,
            HexagonSizeY = hexagonSize.y,
            Position = position,
            GridOrientation = gridOrientation,
            HexagonVertices = hexagonVerticesNative
        };

        int numThreads = System.Environment.ProcessorCount;
        JobHandle jobHandle = jobData.Schedule(6, numThreads);

        jobHandle.Complete();

        for (int i = 0; i < 6; i++)
        {
            hexagonVertices[i] = hexagonVerticesNative[i];
        }

        hexagonVerticesNative.Dispose();

        for (int i = 0; i < 6; i++)
        {
            Gizmos.DrawLine(hexagonVertices[i], hexagonVertices[(i + 1) % 6]);
        }
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
    public static void DebugCellPropertiesDictionary(GridGeneratorFull gridGenerator)
    {
        var keysArray = new NativeArray<Vector3>(gridGenerator.cellsDictionary.Keys.ToArray(), Allocator.TempJob);
        var valuesArray = new NativeArray<CellProperties>(gridGenerator.cellsDictionary.Values.ToArray(), Allocator.TempJob);

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