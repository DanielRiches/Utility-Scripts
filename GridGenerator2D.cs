using System;
using Unity.Mathematics;
using UnityEngine;

public class GridGenerator2D : MonoBehaviour
{
    public static GridGenerator2D Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // ------------------------------------------------------------
    // CONFIG
    // ------------------------------------------------------------

    [Serializable]
    public class GridProperties
    {
        [Tooltip("Total number of cells in the grid. X = width, Y = height. For example, (100, 100) creates a grid containing 10,000 cells.")]
        [SerializeField] private int2 gridSize = new int2(500, 500);

        [Tooltip("Number of cells stored per chunk along one axis. Larger values reduce chunk count but increase per-chunk memory usage. A value of 128 creates chunks containing 128×128 cells.")]
        [SerializeField] private int chunkSize = 128;

        [Tooltip("World-space width and depth of an individual cell. A value of 1 means each cell occupies a 1×1 unit area.")]
        [SerializeField] private float cellSize = 1f;

        [Tooltip("Visual height used for cell-related geometry, floor thickness, and debug gizmos. Does not affect cell width or depth.")]
        [SerializeField] private float cellHeight = 0.1f;

        [Tooltip("Additional spacing between adjacent cells. A value of 0 makes cells touch edge-to-edge. Positive values create gaps between cells.")]
        [SerializeField] private float cellGap = 0f;

        private float3 origin;

        public int2 GridSize => gridSize;
        public int GridWidth => gridSize.x;
        public int GridHeight => gridSize.y;

        public int ChunkSize => chunkSize;
        public int ChunkCountX => (gridSize.x + chunkSize - 1) / chunkSize;
        public int ChunkCountY => (gridSize.y + chunkSize - 1) / chunkSize;

        public float CellSize => cellSize;
        public float CellHeight => cellHeight;
        public float CellGap => cellGap;

        public float3 Origin
        {
            get => origin;
            set => origin = value;
        }
    }

    public GridProperties gridProperties = new GridProperties();

    // ------------------------------------------------------------
    // FLOOR
    // ------------------------------------------------------------

    [Header("Floor")]
    [SerializeField] private bool generateFloor = true;
    [SerializeField] private int floorLayer = 0;

    // ------------------------------------------------------------
    // ENUMS
    // ------------------------------------------------------------

    [Flags]
    public enum TileFlags : uint
    {
        None = 0,
        Walkable = 1 << 0,
        Diggable = 1 << 1,
        Claimable = 1 << 2,
        Claimed = 1 << 3,
        Reserved = 1 << 4,
        Occupied = 1 << 5
    }

    public enum TerrainType : byte { None, Tile, Corrupt, Water}
    public enum EffectType : byte { None, Frozen, Corrupted, Electrified, Trapped }
    public enum RoomType : byte { None, LandingPad, Admin, TrainingCourse, Barracks, MessHall, FiringRange, Generator, SituationRoom, Research, GamesRoom}

    // ------------------------------------------------------------
    // CHUNK STORAGE
    // ------------------------------------------------------------

    private class Chunk
    {
        public readonly uint[] flags;
        public readonly byte[] terrain;
        public readonly byte[] effects;
        public readonly byte[] rooms;

        public readonly float3[] world;
        public readonly float3[] center;

        public Chunk(int size)
        {
            int count = size * size;

            flags = new uint[count];
            terrain = new byte[count];
            effects = new byte[count];
            rooms = new byte[count];

            world = new float3[count];
            center = new float3[count];
        }
    }

    private Chunk[,] chunks;
    private bool gridGenerated;

    // ------------------------------------------------------------
    // INITIALIZATION
    // ------------------------------------------------------------

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridProperties.Origin = (float3)transform.position;

        int cx = gridProperties.ChunkCountX;
        int cy = gridProperties.ChunkCountY;
        int chunkSize = gridProperties.ChunkSize;

        chunks = new Chunk[cx, cy];

        float spacing = gridProperties.CellSize + gridProperties.CellGap;

        for (int cyi = 0; cyi < cy; cyi++)
        {
            for (int cxi = 0; cxi < cx; cxi++)
            {
                float3 chunkOrigin = gridProperties.Origin + new float3(cxi * chunkSize * spacing, 0f, cyi * chunkSize * spacing);

                Chunk chunk = new Chunk(chunkSize);
                chunks[cxi, cyi] = chunk;

                for (int y = 0; y < chunkSize; y++)
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int i = x + y * chunkSize;

                        chunk.terrain[i] = (byte)TerrainType.None;
                        chunk.effects[i] = (byte)EffectType.None;
                        chunk.rooms[i] = (byte)RoomType.None;

                        float3 worldPos = chunkOrigin + new float3(x * spacing, 0f, y * spacing);

                        chunk.world[i] = worldPos;

                        chunk.center[i] = worldPos + new float3(gridProperties.CellSize * 0.5f, 0f, gridProperties.CellSize * 0.5f);
                    }
            }
        }

        gridGenerated = true;

        if (generateFloor) CreateGridFloor();
    }

    // ------------------------------------------------------------
    // CORE CONVERSION API (SINGLE SOURCE OF TRUTH)
    // ------------------------------------------------------------

    public int2 WorldToCell(Vector3 world)
    {
        float spacing = gridProperties.CellSize + gridProperties.CellGap;
        float3 origin = gridProperties.Origin;

        float gx = (world.x - origin.x) / spacing;
        float gy = (world.z - origin.z) / spacing;

        return new int2(
            Mathf.FloorToInt(gx),
            Mathf.FloorToInt(gy)
        );
    }

    public Vector3 CellToWorldCorner(int x, int y)
    {
        float spacing = gridProperties.CellSize + gridProperties.CellGap;
        float3 origin = gridProperties.Origin;

        return new Vector3(
            origin.x + x * spacing,
            origin.y,
            origin.z + y * spacing
        );
    }

    public Vector3 CellToWorldCenter(int x, int y)
    {
        float half = gridProperties.CellSize * 0.5f;

        Vector3 corner = CellToWorldCorner(x, y);

        return new Vector3(
            corner.x + half,
            corner.y,
            corner.z + half
        );
    }

    public bool InBounds(int x, int y)
        => x >= 0 && y >= 0 &&
           x < gridProperties.GridWidth &&
           y < gridProperties.GridHeight;

    // ------------------------------------------------------------
    // FLOOR (PERFECT ALIGNMENT)
    // ------------------------------------------------------------

    private void CreateGridFloor()
    {
        Transform existing = transform.Find("GridFloor");
        if (existing != null)
            DestroyImmediate(existing.gameObject);

        GameObject floor = new GameObject("GridFloor");
        floor.transform.SetParent(transform, false);

        float spacing = gridProperties.CellSize + gridProperties.CellGap;

        float width = gridProperties.GridWidth * spacing;
        float height = gridProperties.GridHeight * spacing;

        float3 origin = gridProperties.Origin;

        floor.transform.position =
            (Vector3)(origin + new float3(width, 0f, height) * 0.5f);

        BoxCollider col = floor.AddComponent<BoxCollider>();
        col.center = Vector3.zero;

        col.size = new Vector3(
            width,
            gridProperties.CellHeight * 2f,
            height
        );

        floor.layer = floorLayer;
    }

    public bool IsGenerated => gridGenerated;

    // ------------------------------------------------------------
    // CHUNK ACCESS
    // ------------------------------------------------------------

    private Chunk GetChunk(int x, int y)
    {
        int cx = x / gridProperties.ChunkSize;
        int cy = y / gridProperties.ChunkSize;

        return chunks[cx, cy];
    }

    private int LocalIndex(int x, int y)
    {
        int lx = x % gridProperties.ChunkSize;
        int ly = y % gridProperties.ChunkSize;

        return lx + ly * gridProperties.ChunkSize;
    }

    // ------------------------------------------------------------
    // TILE DATA ACCESS
    // ------------------------------------------------------------

    public TileFlags GetFlags(int x, int y)
    {
        Chunk c = GetChunk(x, y);
        return (TileFlags)c.flags[LocalIndex(x, y)];
    }

    public TerrainType GetTerrain(int x, int y)
    {
        Chunk c = GetChunk(x, y);
        return (TerrainType)c.terrain[LocalIndex(x, y)];
    }

    public EffectType GetEffect(int x, int y)
    {
        Chunk c = GetChunk(x, y);
        return (EffectType)c.effects[LocalIndex(x, y)];
    }

    public RoomType GetRoom(int x, int y)
    {
        Chunk c = GetChunk(x, y);
        return (RoomType)c.rooms[LocalIndex(x, y)];
    }
}