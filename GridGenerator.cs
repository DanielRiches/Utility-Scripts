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
using static GridGenerator;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GridGenerator : MonoBehaviour
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

    [SerializeField] GameManager gameManager;
    private bool ready;
    private bool gridDone;

    [System.Serializable]
    public class GridProperties
    {
        [HideInInspector] public Grid grid;
        [HideInInspector] public GridLayout gridLayout;
        public Dictionary<Vector3, CellProperties> cellsDictionary = new Dictionary<Vector3, CellProperties>();
        public List<Vector3> cellsList = new List<Vector3>();
        public List<Vector3> edgeCellList = new List<Vector3>();
        public List<Vector3> guideCellList = new List<Vector3>();
        public List<Vector3> cornerCellList = new List<Vector3>();
        public Vector3 centerCellPosition;
        [Space(10)]
        [Header("Grid Settings")]
        [Tooltip("The desired grid size.")]
        public Vector2Int gridSize = new Vector2Int(20, 20);
        [Tooltip("The desired cell size.")]
        public Vector3 cellSize = new Vector3(1f, 1f, 0.1f);
        [Tooltip("The desired gap between each cell.")]
        public Vector2 cellGap = new Vector3(0f, 0f);
        [Space(10)]
        [Header("Floor Settings")]
        public bool createFloor;
        [HideInInspector] public GameObject floorAnchor;
        [HideInInspector] public GameObject floor;
        public int desiredFloorLayerIndex;
        public BoxCollider floorCollider;
        [Space(10)]
        [Tooltip("Tile the material to match the grid size.")]
        public bool tileMaterial;
        [Tooltip("If not assigned, will use Unity Default Material.")]
        public Material floorMaterial;
        [Space(10)]
        [Range(0.1f, 10000)] public float probeThickness = 6.5f;
        public PlanarReflectionProbe floorPlanarReflectionProbe;
        [Header("NavMesh Settings")]
        public bool createNavMesh;
        public bool createNavMeshGridEdgeBlockers;
        [HideInInspector] public NavMeshSurface navMeshSurface;
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
    [Tooltip("Adds all cells to Cells List during grid generation to enable debugging.")]
    [SerializeField] private bool debugCells;
    [Space(5)]
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
    #endregion

    private void Awake()
    {
        gameManager = GameObject.FindWithTag(Strings.gameManagerTag).GetComponent<GameManager>();
        gameManager.scripts.gridGenerator = this;
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
    }

    private void Start()
    {
        if (ready)
        {
            SetGrid();
            StartCoroutine(GenerateGrid(gridProperties.cellsList, gridProperties.cornerCellList, gridProperties.guideCellList));
        }
    }

    private void Update()
    {
        DebugGrid();
    }

    void CreateFloor()
    {
        gridProperties.floorAnchor = new GameObject("GridFloorAnchor");
        gridProperties.floorAnchor.transform.position = this.gameObject.transform.position;
        gridProperties.floorAnchor.isStatic = true;
        gridProperties.floorAnchor.transform.parent = this.transform;

        gridProperties.floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gridProperties.floor.name = "GridFloor";

        MeshRenderer floorRenderer;
        gridProperties.floor.TryGetComponent(out floorRenderer);

        if (gridProperties.createFloor)
        {
            if (floorRenderer)
            {
                floorRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                if (gridProperties.floorMaterial)
                {
                    floorRenderer.material = gridProperties.floorMaterial;
                    floorRenderer.material.mainTextureScale = new Vector2(gridProperties.gridSize.x, gridProperties.gridSize.y);
                    floorRenderer.material.SetTextureScale("_NormalMap", new Vector2(gridProperties.gridSize.x, gridProperties.gridSize.y));
                    floorRenderer.material.SetTextureScale("_EmissiveColorMap", new Vector2(gridProperties.gridSize.x, gridProperties.gridSize.y));
                }   
            }
        }
        else
        {
            if (floorRenderer)
            {
                Destroy(floorRenderer);
                floorRenderer = null;
            }
        }

        MeshCollider mc = gridProperties.floor.GetComponent<MeshCollider>();
        if (mc != null) Destroy(mc);
        gridProperties.floor.transform.parent = gridProperties.floorAnchor.transform;
        gridProperties.floorCollider = gridProperties.floor.AddComponent<BoxCollider>();   
        gridProperties.floor.layer = gridProperties.desiredFloorLayerIndex;
        gridProperties.floorCollider.center = new Vector3(0, 0, (gridProperties.cellSize.z / 2) / 2);
        gridProperties.floorCollider.size = new Vector3(1, 1, gridProperties.cellSize.z);

        Quaternion newRotation = Quaternion.Euler(90f, 0f, 0f);
        gridProperties.floor.transform.SetLocalPositionAndRotation(new Vector3(0.5f, gridProperties.floorAnchor.transform.position.y, 0.5f), newRotation);
        gridProperties.floorAnchor.transform.localScale = new Vector3(gridProperties.gridSize.x * gridProperties.cellSize.x + (gridProperties.gridSize.x - 1) * gridProperties.cellGap.x, 1f, gridProperties.gridSize.y * gridProperties.cellSize.y + (gridProperties.gridSize.y - 1) * gridProperties.cellGap.y);

        if (gridProperties.floorPlanarReflectionProbe)
        {
            float worldWidth = gridProperties.gridSize.x * (gridProperties.cellSize.x + gridProperties.cellGap.x);
            float worldDepth = gridProperties.gridSize.y * (gridProperties.cellSize.y + gridProperties.cellGap.y);
            //float verticalThickness = 6.5f;
            gridProperties.floorPlanarReflectionProbe.influenceVolume.boxSize = new Vector3(worldWidth, gridProperties.probeThickness, worldDepth);
            gridProperties.floorPlanarReflectionProbe.transform.position = new Vector3(gridProperties.centerCellPosition.x, 0, gridProperties.centerCellPosition.z);
        }

        if (gridProperties.createNavMesh)
        {
            gridProperties.navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
            gridProperties.navMeshSurface.buildHeightMesh = true;
            gridProperties.navMeshSurface.overrideVoxelSize = true;
            gridProperties.navMeshSurface.voxelSize = 0.125f;
            gridProperties.navMeshSurface.overrideTileSize = true;
            gridProperties.navMeshSurface.tileSize = 24;
            gridProperties.navMeshSurface.layerMask = 1 << gridProperties.desiredFloorLayerIndex;
            gridProperties.navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            if (gridProperties.createNavMeshGridEdgeBlockers && gridProperties.guideCellList != null)
            {
                foreach (Vector3 key in gridProperties.guideCellList)
                {
                    GameObject blocker = new GameObject("NavMeshEdgeBlocker");
                    Vector3 blockerPos = gridProperties.cellsDictionary[key].cellCenterPosition;
                    blocker.transform.position = blockerPos;

                    // Get direction to center on the XZ plane only
                    Vector3 toCenter = gridProperties.centerCellPosition - blockerPos;
                    toCenter.y = 0f; // flatten

                    Vector3 forward;

                    // Snap to the closest world axis (X or Z)
                    if (Mathf.Abs(toCenter.x) > Mathf.Abs(toCenter.z))
                    {
                        forward = toCenter.x > 0 ? Vector3.right : Vector3.left;
                    }
                    else
                    {
                        forward = toCenter.z > 0 ? Vector3.forward : Vector3.back;
                    }

                    // Rotate the blocker so its forward points toward the grid center
                    blocker.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

                    NavMeshObstacle blockerObstacle = blocker.AddComponent<NavMeshObstacle>();
                    blockerObstacle.size = new Vector3(100000, gridProperties.cellSize.y - 1, gridProperties.cellSize.x - 1); // Swapped Y/Z size if needed
                    blockerObstacle.carving = true;
                    blockerObstacle.carveOnlyStationary = true;
                    blockerObstacle.gameObject.layer = gridProperties.desiredFloorLayerIndex;
                }
            }
            gridProperties.navMeshSurface.BuildNavMesh();
        }
    }

    IEnumerator GenerateGrid(List<Vector3> cellsList, List<Vector3> cornerCellList, List<Vector3> guideCellList)
    {
        for (int y = 0; y < gridProperties.gridSize.y; y++)
        {
            for (int x = 0; x < gridProperties.gridSize.x; x++)
            {
                Vector3 cell = gridProperties.gridLayout.CellToWorld(new Vector3Int(x, y, 0));

                cellsList.Add(cell);

                // Guide cells (midpoints on edges)
                if ((x == gridProperties.gridSize.x / 2 && (y == 0 || y == gridProperties.gridSize.y - 1)) ||
                    (y == gridProperties.gridSize.y / 2 && (x == 0 || x == gridProperties.gridSize.x - 1)))
                {
                    guideCellList.Add(cell);
                }

                // Corner cells
                if ((x == 0 || x == gridProperties.gridSize.x - 1) &&
                    (y == 0 || y == gridProperties.gridSize.y - 1))
                {
                    cornerCellList.Add(cell);
                }

                // Center cell
                if (x == gridProperties.gridSize.x / 2 && y == gridProperties.gridSize.y / 2)
                {
                    gridProperties.centerCellPosition = cell;
                }

                // Edge cells (perimeter, includes corners and guide cells)
                if (x == 0 || x == gridProperties.gridSize.x - 1 ||
                    y == 0 || y == gridProperties.gridSize.y - 1)
                {
                    gridProperties.edgeCellList.Add(cell);
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

        if (gridProperties.gridSize.x < 3)
        {
            if (debugCells)
            {
                Debug.Log("Must use minimum 3x3 grid size, adjusting....");
            }            
            gridProperties.gridSize.x = 3;
        }
        if (gridProperties.gridSize.y < 3)
        {
            if (debugCells)
            {
                Debug.Log("Must use minimum 3x3 grid size, adjusting....");
            }
            gridProperties.gridSize.y = 3;
        }

        gridProperties.grid.cellGap = gridProperties.cellGap;
        gridProperties.grid.cellSwizzle = GridLayout.CellSwizzle.XZY;
        gridProperties.grid.cellSize = gridProperties.cellSize;
    }
    void GridDone()
    {
        gridDone = true;
    }

    #region Jobs
    [BurstCompile]
    struct PopulateDictionaryJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> cellsList;
        [WriteOnly] public NativeParallelHashMap<Vector3, CellProperties>.ParallelWriter cellsDictionary;
        public float cellSizeX;
        public float cellSizeY;

        public void Execute(int index)
        {
            Vector3 key = cellsList[index];
            CellProperties cellProperties = new CellProperties();
            cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, 0f, cellSizeY * 0.5f);
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
        public float cellSizeX;
        public float cellSizeY;

        public void Execute()
        {
            foreach (var kvp in cellsDictionary)
            {
                Vector3 key = kvp.Key;
                CellProperties cellProperties = kvp.Value;
                cellProperties.cellCenterPosition = key + new Vector3(cellSizeX * 0.5f, 0f, cellSizeY * 0.5f);
                changesList.Add(new KeyValuePair<Vector3, CellProperties>(key, cellProperties));
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

        if (!debugCells)
        {
            gridProperties.cellsList.Clear();
        }

        yield return null;

        GridDone();
        CreateFloor();
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
    }
    private void OnDrawGizmos()
    {
        if (gridDone)
        {
            if (!drawCells) return;

            Color gizmoColor;
            Vector3 desiredCellSize;
            Vector3 offset;

            foreach (var kvp in gridProperties.cellsDictionary)
            {
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
            }
            GridDebug.ExecuteGizmoBatch(); // Execute the batched Gizmos calls...they deserved it
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
    public static void ExecuteGizmoBatch()
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
        [ReadOnly] public NativeArray<GridGenerator.CellProperties> Values;

        public void Execute(int index)
        {
            Debug.Log($"Key: {Keys[index]}, Value: {Values[index]}");
        }
    }
    public static void DebugGridCellsDictionary(GridGenerator gridGenerator)
    {
        NativeArray<Vector3> keysArray = new NativeArray<Vector3>(gridGenerator.gridProperties.cellsDictionary.Keys.ToArray(), Allocator.TempJob);
        NativeArray<GridGenerator.CellProperties> valuesArray = new NativeArray<GridGenerator.CellProperties>(gridGenerator.gridProperties.cellsDictionary.Values.ToArray(), Allocator.TempJob);

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
    public static void DebugCellPropertiesDictionary(GridGenerator gridGenerator)
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

