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

public class GridGeneratorFull : MonoBehaviour
{
    // Multithreaded and BurstCompiled Grid creation script, no specific components needed as the script will add those automatically.    
    // Each cell is then added to a Dictionary. that can then be used to store values for each grid cell, using the cell's gridLayout.CellToWorld as the key for each entry.    

    // Intended Purpose: A* Pathfinding if necessary, as Dictionary keys are grid cell positions, primarily designed for games that have a large desired grid size  with a lot of data per-tile.

    // PLEASE NOTE: Although heavier operations inside GridDebug class are using jobs, it can take a while to debug large grids, use cautiously.

    #region Hidden Properties / Saved Grid Values
    [Serializable]
    public struct CellProperties // You can set what values you wish each cell to have inside here.
    {
        [SerializeField] public int teamNumber;
    }
    [BurstCompile]
    private struct CellJob : IJobParallelFor
    {
        public NativeArray<Vector3> cellWorldPositions;

        public void Execute(int index)
        {
            // Do something to the array if you wish.
        }
    }
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
    public Dictionary<Vector3, CellProperties> cellsDictionary = new Dictionary<Vector3, CellProperties>();    
    private BoxCollider gridCollider;
    private bool ready;
    private bool generated;
    private GridStyle previousGridStyle;
    private GridOrientation previousGridOrientation;
    private Vector2Int previousGridSize;
    private Vector3 previousCellSize;
    private Vector2 previousCellGap;
    private JobHandle jobHandle;
    private NativeArray<Vector3> cellWorldPositions;
    private bool isGeneratingGrid;    
    private bool preparingGuideCells;
    private bool guideCellsDone;
    private bool stopGuideCellsCoroutine;
    private bool centerFound;
    #endregion

    #region Inspector
    public Grid grid;
    public GridLayout gridLayout;
    [Space(5)]
    [Header("Debug")]
    [SerializeField] private List<Vector3> guideCellList = new List<Vector3>();
    [SerializeField] private Vector3 centerCellPosition;
    [Tooltip("Draws the cells visually, requires GridDebug class if not already in this script.")]
    [SerializeField] private bool drawCells;    
    [Tooltip("Highlights important points on the grid by drawing them green.")]
    [SerializeField] private bool drawGuideCells;
    [Tooltip("Information on the position of the furthest cell from the start.")]
    [SerializeField] private bool debugFurthestCell;
    [Tooltip("Information on the contents of the cells dictionary.")]
    [SerializeField] private bool debugDictionary;
    [Tooltip("Estimate the runtime cost of the grid, given the properties you put in the CellProperties struct and the inspector values selected")]
    [SerializeField] private bool debugMemoryRequirements;
    [Space(5)]
    [Header("Runtime")]
    [Tooltip("Generate a grid the moment PlayMode is entered.")]
    [SerializeField] private bool generateGridOnStart;
    [Tooltip("Generate a grid, this will overwrite the previous grid.")]
    [SerializeField] private bool generateGrid;
    [Space(5)]
    [Header("Grid Settings")]
    [Tooltip("Style of grid")]
    [SerializeField] private GridStyle gridStyle = GridStyle.Rectangle;
    [Tooltip("Grid Orientation, Horizontal grids are perfect for 3D environments.")]
    [SerializeField] private GridOrientation gridOrientation = GridOrientation.Vertical;
    [Tooltip("The desired grid size.")]
    public Vector2Int gridSize = new Vector2Int(5, 5);
    [Tooltip("The desired cell size.")]
    [SerializeField] private Vector3 cellSize = new Vector3(1f, 1f, 0.1f);
    [Tooltip("The desired gap between each cell.")]
    [SerializeField] private Vector2 cellGap = new Vector3(0f, 0f);
    #endregion   

    private void Awake()
    {
        if (!TryGetComponent<Grid>(out grid))
        {
            ready = false;
            grid = gameObject.AddComponent<Grid>();
            gridLayout = grid;
        }
        else
        {
            gridLayout = grid;
        }

        if (!TryGetComponent<BoxCollider>(out gridCollider))
        {
            ready = false;
            gridCollider = gameObject.AddComponent<BoxCollider>();

            if (guideCellsDone)
            {
                gridCollider.center = centerCellPosition; // Set the center of the BoxCollider to the center of the grid
            }
        }
        else
        {
            if (guideCellsDone)
            {
                gridCollider.center = centerCellPosition;
            }
        }

        ready = true;
    }

    private void Start()
    {
        if (ready)
        {
            if (generateGridOnStart)
            {
                generateGridOnStart = false;

            }
            generated = false;
            preparingGuideCells = false;
            guideCellsDone = false;
            generateGrid = false;            
            SetGrid();
            SaveGridValues();
            StartCoroutine(GenerateGrid());
        }
    }

    private void Update()
    {
        if (generated)
        {
            if (!preparingGuideCells)
            {                
                StartCoroutine(GetGuideCells());
                preparingGuideCells = true;
            }

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

            if (debugFurthestCell)
            {
                if (cellsDictionary.Count > 0)
                {
                    GridDebug.DebugFurthestCellKey(this);
                }
                debugFurthestCell = false;
            }
        }

        if (debugMemoryRequirements)
        {
            Debug.Log("Accounting for CellProperties struct and inspector properties, estimated memory cost : " + GridDebug.CalculateMemoryRequirement(gridSize, typeof(CellProperties)));
            debugMemoryRequirements = false;
        }

        if (generateGrid || gridStyle != previousGridStyle || gridOrientation != previousGridOrientation || gridSize.x != previousGridSize.x || gridSize.y != previousGridSize.y || cellSize != previousCellSize || cellGap.x != previousCellGap.x || cellGap.y != previousCellGap.y)
        {
            if (isGeneratingGrid)
            {
                isGeneratingGrid = false;
                jobHandle.Complete();
                cellWorldPositions.Dispose();
                Start();
            }
            else
            {
                Start();
            }
            generateGrid = false;
        }

        if (guideCellsDone && gridSize.x > 2 && gridSize.y > 2 && guideCellList.Count < 9)
        {
            guideCellsDone = false;
            StartCoroutine(GetGuideCells());
        }
    }

    IEnumerator GenerateGrid()
    {        
        stopGuideCellsCoroutine = true;// Set the flag to stop the GetGuideCells coroutine
        cellsDictionary.Clear();
        cellSize = grid.cellSize;
        cellWorldPositions = new NativeArray<Vector3>(gridSize.x * gridSize.y, Allocator.TempJob);
        isGeneratingGrid = true;

        int numThreads = System.Environment.ProcessorCount;
        CellJob cellJob = new CellJob
        {
            cellWorldPositions = cellWorldPositions
        };
        jobHandle = cellJob.Schedule(gridSize.x * gridSize.y, numThreads);
        
        yield return new WaitUntil(() => jobHandle.IsCompleted || !isGeneratingGrid); // Wait for the job to complete or be canceled
        jobHandle.Complete();

        if (isGeneratingGrid)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector3 cellCenter = gridLayout.CellToWorld(new Vector3Int(x, y, 0));

                    if (!cellsDictionary.ContainsKey(cellCenter))
                    {
                        cellsDictionary.Add(cellCenter, new CellProperties());
                    }
                    else
                    {
                        Debug.Log($"Key already exists for position: {cellCenter}");
                    }
                }
            }
            generated = true;
            cellWorldPositions.Dispose();
            isGeneratingGrid = false;
        }        
        stopGuideCellsCoroutine = false;
    }

    #region Save + Set Grid
    private void SaveGridValues()
    {
        previousGridStyle = gridStyle;
        previousGridOrientation = gridOrientation;
        previousGridSize = gridSize;
        previousCellSize = cellSize;
        previousCellGap = cellGap;
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
    #endregion

    #region GridDebug
    private void OnDrawGizmos()
    {
        if (!generated || !drawCells) return;

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
                            if (guideCellsDone && drawGuideCells)
                            {
                                if (guideCellList.Contains(kvp.Key))
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
                            if (guideCellsDone && drawGuideCells)
                            {
                                if (guideCellList.Contains(kvp.Key))
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
                    if (guideCellList.Contains(kvp.Key))
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

    IEnumerator GetGuideCells()
    {
        HashSet<Vector3> uniqueKeys = new HashSet<Vector3>(); // Use a HashSet to store unique keys
        centerFound = false;
        Vector3 localPosition;

        if (stopGuideCellsCoroutine)
        {
            yield break;
        }

        List<Vector3> cellKeys = new List<Vector3>(cellsDictionary.Keys);

        foreach (var key in cellKeys)
        {
            bool isGuide = GridDebug.GetGuideCellColor(this, key);

            if (isGuide && !uniqueKeys.Contains(key))
            {
                int index = cellsDictionary.Keys.ToList().IndexOf(key);

                if (!centerFound && GridDebug.IsCellAtCenter(index, gridSize))
                {
                    centerFound = true;
                    localPosition = transform.InverseTransformPoint(key);
                    centerCellPosition = localPosition;

                    switch (gridStyle)
                    {
                        case GridStyle.Rectangle:
                            switch (gridOrientation) // Set the center of the BoxCollider to the middle of the grids center cell
                            {
                                case GridOrientation.Vertical:
                                    gridCollider.center = centerCellPosition + new Vector3(gridLayout.cellSize.x * 0.5f, gridLayout.cellSize.y * 0.5f, 0f);
                                    break;

                                case GridOrientation.Horizontal:
                                    gridCollider.center = centerCellPosition + new Vector3(gridLayout.cellSize.x * 0.5f, 0f, gridLayout.cellSize.y * 0.5f);
                                    break;
                            }
                            break;

                        case GridStyle.Hexagon:
                            gridCollider.center = centerCellPosition;
                            break;
                    }
                }
                uniqueKeys.Add(key); // Add the key to the HashSet to track uniqueness
            }
            yield return null;
        }
        guideCellList = new List<Vector3>(uniqueKeys); // Convert the HashSet back to the List
        guideCellsDone = true;
    }
    #endregion
}

public static class GridDebug
{
    #region Debug
    public static bool IsCellHalfwayBetweenCorners(int index, Vector2Int gridSize)
    {
        int maxX = gridSize.x - 1;
        int maxY = gridSize.y - 1;
        int cellX = index % gridSize.x;
        int cellY = index / gridSize.x;
        int halfwayX = gridSize.x / 2;
        int halfwayY = gridSize.y / 2;

        return (cellX == halfwayX && (cellY == 0 || cellY == maxY)) ||
               (cellY == halfwayY && (cellX == 0 || cellX == maxX));
    }
    public static bool IsCellAtCenter(int index, Vector2Int gridSize)
    {
        int centerX = gridSize.x / 2;
        int centerY = gridSize.y / 2;

        return index == (centerX + centerY * gridSize.x);
    }
    public static bool IsCellAtCorner(int index, Vector2Int gridSize)
    {
        int maxX = gridSize.x - 1;
        int maxY = gridSize.y - 1;

        return index == 0 ||
               index == maxX ||
               index == maxY * gridSize.x ||
               index == (maxY * gridSize.x) + maxX;
    }
    public static bool GetGuideCellColor(GridGeneratorFull gridGenerator, Vector3 cellPosition)
    {
        bool isCorner = IsCellAtCorner(gridGenerator.cellsDictionary.Keys.ToList().IndexOf(cellPosition), gridGenerator.gridSize);
        bool isCenter = IsCellAtCenter(gridGenerator.cellsDictionary.Keys.ToList().IndexOf(cellPosition), gridGenerator.gridSize);
        bool isHalfway = IsCellHalfwayBetweenCorners(gridGenerator.cellsDictionary.Keys.ToList().IndexOf(cellPosition), gridGenerator.gridSize);

        return isCorner || isCenter || isHalfway;
    }

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
    #endregion

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
    public struct FindFurthestCellJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> cellKeys;

        [NativeDisableParallelForRestriction]
        public NativeArray<float> maxMagnitudes;

        [NativeDisableParallelForRestriction]
        public NativeArray<Vector3> furthestCellKeys;

        public void Execute(int index)
        {
            float magnitude = cellKeys[index].magnitude;

            // Use local variables to avoid race conditions
            float localMaxMagnitude = maxMagnitudes[0];
            Vector3 localFurthestCellKey = furthestCellKeys[0];

            if (magnitude > localMaxMagnitude)
            {
                localMaxMagnitude = magnitude;
                localFurthestCellKey = cellKeys[index];
            }

            // Write back the local variables
            maxMagnitudes[0] = localMaxMagnitude;
            furthestCellKeys[0] = localFurthestCellKey;
        }
    }
    public static void DebugFurthestCellKey(GridGeneratorFull gridGenerator)
    {
        if (gridGenerator.cellsDictionary.Count == 0)
        {
            Debug.LogWarning("Grid is empty.");
            return;
        }
        
        NativeArray<Vector3> cellKeys = new NativeArray<Vector3>(gridGenerator.cellsDictionary.Keys.ToArray(), Allocator.TempJob);// Extract the keys from the dictionary        
        NativeArray<float> maxMagnitudes = new NativeArray<float>(1, Allocator.TempJob);// Allocate native arrays for job results
        NativeArray<Vector3> furthestCellKeys = new NativeArray<Vector3>(1, Allocator.TempJob);        
        maxMagnitudes[0] = float.MinValue;// Set initial values for the max magnitude and furthest cell
        furthestCellKeys[0] = Vector3.zero;
        
        FindFurthestCellJob job = new FindFurthestCellJob
        {
            cellKeys = cellKeys,
            maxMagnitudes = maxMagnitudes,
            furthestCellKeys = furthestCellKeys
        };

        int numThreads = System.Environment.ProcessorCount;
        JobHandle jobHandle = job.Schedule(cellKeys.Length, numThreads);
        jobHandle.Complete();

        float maxMagnitudeResult = maxMagnitudes[0];        // Retrieve results from native arrays
        Vector3 furthestCellKeyResult = furthestCellKeys[0];

        cellKeys.Dispose();
        maxMagnitudes.Dispose();
        furthestCellKeys.Dispose();
        Debug.Log($"World Position of furthest cell in the grid from origin (bottom left of grid): {furthestCellKeyResult}");
    }

    // ----------------

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

    #endregion
}
