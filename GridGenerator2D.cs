using System;
using Unity.Mathematics;
using UnityEngine;

public class GridGenerator2D : MonoBehaviour
{
    // ------------------------------------------------------------
    // CONFIG
    // ------------------------------------------------------------

    [Serializable]
    public class GridProperties
    {
        [SerializeField] private int2 gridSize = new int2(20000, 20000);
        [SerializeField] private int chunkSize = 128;

        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float cellHeight = 0.1f;
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

        public float3 Origin { get => origin; set => origin = value; }
    }

    public GridProperties gridProperties = new GridProperties();

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

    public enum TerrainType : byte { Empty, Dirt, Rock, Lava, Water }
    public enum EffectType : byte { None, Frozen, Poisoned, Electrified, Trapped }
    public enum RoomType : byte { None, Corridor, Lair, Storage, Workshop }

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
    // INDEX HELPERS
    // ------------------------------------------------------------

    public int2 GetChunkCoord(int x, int y)
        => new int2(x / gridProperties.ChunkSize, y / gridProperties.ChunkSize);

    public int2 GetLocalCoord(int x, int y)
        => new int2(x % gridProperties.ChunkSize, y % gridProperties.ChunkSize);

    public int IndexLocal(int x, int y)
        => x + y * gridProperties.ChunkSize;

    public bool InBounds(int x, int y)
        => x >= 0 && y >= 0 &&
           x < gridProperties.GridWidth &&
           y < gridProperties.GridHeight;

    // ------------------------------------------------------------
    // GENERATION
    // ------------------------------------------------------------

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        gridProperties.Origin = transform.position;

        int cx = gridProperties.ChunkCountX;
        int cy = gridProperties.ChunkCountY;
        int chunkSize = gridProperties.ChunkSize;

        chunks = new Chunk[cx, cy];

        float spacing = gridProperties.CellSize + gridProperties.CellGap;

        for (int cyi = 0; cyi < cy; cyi++)
        {
            for (int cxi = 0; cxi < cx; cxi++)
            {
                float3 chunkOrigin = new float3(
                    gridProperties.Origin.x + cxi * chunkSize * spacing,
                    gridProperties.Origin.y,
                    gridProperties.Origin.z + cyi * chunkSize * spacing
                );

                Chunk chunk = new Chunk(chunkSize);
                chunks[cxi, cyi] = chunk;

                for (int y = 0; y < chunkSize; y++)
                {
                    for (int x = 0; x < chunkSize; x++)
                    {
                        int i = IndexLocal(x, y);

                        chunk.flags[i] =
                            (uint)(TileFlags.Walkable |
                                   TileFlags.Diggable |
                                   TileFlags.Claimable);

                        chunk.terrain[i] = (byte)TerrainType.Dirt;
                        chunk.effects[i] = (byte)EffectType.None;
                        chunk.rooms[i] = (byte)RoomType.None;

                        float3 worldPos =
                            chunkOrigin + new float3(x * spacing, 0, y * spacing);

                        chunk.world[i] = worldPos;

                        chunk.center[i] =
                            worldPos + new float3(
                                gridProperties.CellSize * 0.5f,
                                0,
                                gridProperties.CellSize * 0.5f
                            );
                    }
                }
            }
        }

        gridGenerated = true;
    }

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
    // PUBLIC API (A* FRIENDLY)
    // ------------------------------------------------------------

    public TileFlags GetFlags(int x, int y)
    {
        var c = GetChunk(x, y);
        return (TileFlags)c.flags[LocalIndex(x, y)];
    }

    public TerrainType GetTerrain(int x, int y)
    {
        var c = GetChunk(x, y);
        return (TerrainType)c.terrain[LocalIndex(x, y)];
    }

    public EffectType GetEffect(int x, int y)
    {
        var c = GetChunk(x, y);
        return (EffectType)c.effects[LocalIndex(x, y)];
    }

    public RoomType GetRoom(int x, int y)
    {
        var c = GetChunk(x, y);
        return (RoomType)c.rooms[LocalIndex(x, y)];
    }

    public float3 GetWorldPosition(int x, int y)
    {
        var c = GetChunk(x, y);
        return c.world[LocalIndex(x, y)];
    }

    public float3 GetCenterPosition(int x, int y)
    {
        var c = GetChunk(x, y);
        return c.center[LocalIndex(x, y)];
    }

    // ------------------------------------------------------------
    // STATE
    // ------------------------------------------------------------

    public bool IsGenerated => gridGenerated;
}