using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GridGenerator2DDebug : MonoBehaviour
{
    [Header("Grid Reference")]
    [Tooltip("Reference to the grid being visualized and inspected.")]
    public GridGenerator2D grid;

    [Header("Tile Debug")]
    [Tooltip("When enabled, prints tile information to the Console and then automatically resets.")]
    [SerializeField] private bool debugTileData;
    [Tooltip("Tile coordinate to inspect. Use (-1, -1) to iterate through every tile in the grid.")]
    [SerializeField] private int2 tileCoord = new int2(-1, -1);
    [Tooltip("Stops any active tile debugging operation and clears the current selection.")]
    [SerializeField] private bool cancelDebug;

    [Header("Gizmos")]
    [Tooltip("Draws debug gizmos for visible grid cells.")]
    [SerializeField] private bool drawCells;
    [Tooltip("Highlights the grid center, guide axes, and corner cells using different colors.")]
    [SerializeField] private bool drawGuideCells;
    [Tooltip("Additional world-space origins that draw grid gizmos around themselves. Useful for tracking moving objects such as creatures.")]
    [SerializeField] private Transform[] multiOrigin;
    [Tooltip("Draw radius around each Multi Origin transform.")]
    [SerializeField] private float multiDrawRadius = 50f;
    [Tooltip("Optional custom origin used as the primary gizmo draw center.")]
    [SerializeField] private Transform customOrigin;
    [Tooltip("Uses grid cell (0,0) as the primary gizmo draw center when no custom origin is assigned.")]
    [SerializeField] private bool useGridOrigin;

    [Tooltip("Draw radius around the primary origin (camera, custom origin, or grid origin).")]
    [SerializeField] private float drawRadius = 150f;

    private Coroutine routine;
    private bool cancel;

    private Vector2Int current = new Vector2Int(-1, -1);

    void Update()
    {
        if (debugTileData)
        {
            debugTileData = false;
            Sample(tileCoord);
        }

        if (cancelDebug)
        {
            cancelDebug = false;
            Cancel();
        }
    }

    public void Sample(int2 coord)
    {
        if (grid == null || !grid.IsGenerated) return;

        Cancel();
        cancel = false;
        routine = StartCoroutine(Run(coord));
    }

    public void Cancel()
    {
        cancel = true;
        current = new Vector2Int(-1, -1);

        if (routine != null) StopCoroutine(routine);
    }

    IEnumerator Run(int2 coord)
    {
        if (coord.x >= 0 && coord.y >= 0)
        {
            if (!grid.InBounds(coord.x, coord.y)) yield break;

            current = new Vector2Int(coord.x, coord.y);
            Print(coord.x, coord.y);
            current = new Vector2Int(-1, -1);
            yield break;
        }

        for (int y = 0; y < grid.gridProperties.GridHeight; y++)
            for (int x = 0; x < grid.gridProperties.GridWidth; x++)
            {
                if (cancel) yield break;
                current = new Vector2Int(x, y);
                Print(x, y);
                yield return null;
            }

        current = new Vector2Int(-1, -1);
    }

    void Print(int x, int y)
    {
        var flags = grid.GetFlags(x, y);
        var terrain = grid.GetTerrain(x, y);
        var effect = grid.GetEffect(x, y);
        var room = grid.GetRoom(x, y);

        Debug.Log(
            $"Tile ({x},{y}) | Corner: {grid.CellToWorldCorner(x, y)} | Center: {grid.CellToWorldCenter(x, y)}\n" +
            $"Flags: {FlagsToString(flags)} |    Terrain: {TerrainToString(terrain)} |    Effect: {EffectToString(effect)} |    Room: {RoomToString(room)}"
        );
    }

    void OnDrawGizmos()
    {
        if (!drawCells || grid == null || !grid.IsGenerated)
            return;

        Vector3 origin =
            customOrigin != null
                ? customOrigin.position
                : useGridOrigin
                    ? grid.CellToWorldCorner(0, 0)
                    : Camera.main.transform.position;

        float r2 = drawRadius * drawRadius;
        float multiR2 = multiDrawRadius * multiDrawRadius;

        int width = grid.gridProperties.GridWidth;
        int height = grid.gridProperties.GridHeight;

        int centerX = width / 2;
        int centerY = height / 2;

        int maxX = width - 1;
        int maxY = height - 1;

        float baseHeight = grid.gridProperties.CellHeight;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 p = grid.CellToWorldCenter(x, y);

                bool insideMainRadius =
                    (p - origin).sqrMagnitude <= r2;

                bool insideMultiRadius = false;

                if (multiOrigin != null && multiOrigin.Length > 0)
                {
                    for (int i = 0; i < multiOrigin.Length; i++)
                    {
                        Transform t = multiOrigin[i];

                        if (t == null)
                            continue;

                        if (!t.gameObject.activeInHierarchy)
                            continue;

                        if ((p - t.position).sqrMagnitude <= multiR2)
                        {
                            insideMultiRadius = true;
                            break;
                        }
                    }
                }

                if (!insideMainRadius && !insideMultiRadius)
                    continue;

                bool isSelected =
                    (x == current.x && y == current.y);

                bool isCenter =
                    (x == centerX && y == centerY);

                bool isCorner =
                    (x == 0 && y == 0) ||
                    (x == maxX && y == 0) ||
                    (x == 0 && y == maxY) ||
                    (x == maxX && y == maxY);

                bool isGuide =
                    (x == centerX || y == centerY);

                Color color;
                float heightScale;

                // ----------------------------------------------------
                // COLOR PRIORITY
                // 1. Yellow   = active scan tile
                // 2. Red    = center tile
                // 3. Green  = guide lines + corners
                // 4. Blue = multi-origin radius
                // 5. White  = normal cells
                // ----------------------------------------------------

                if (isSelected)
                {
                    color = Color.yellow;
                    heightScale = grid.gridProperties.CellHeight + 5f;
                }
                else if (drawGuideCells && isCenter)
                {
                    color = Color.red;
                    heightScale = grid.gridProperties.CellHeight + 3f;
                }
                else if (drawGuideCells && (isGuide || isCorner))
                {
                    color = new Color(0f, 1f, 0f, 1f);
                    heightScale = grid.gridProperties.CellHeight + 3f;
                }
                else if (insideMultiRadius)
                {
                    color = Color.midnightBlue;
                    heightScale = grid.gridProperties.CellHeight * 2f;
                }
                else
                {
                    color = Color.white;
                    heightScale = grid.gridProperties.CellHeight * 2f;
                }

                Gizmos.color = color;

                Gizmos.DrawWireCube(
                    p,
                    new Vector3(
                        grid.gridProperties.CellSize,
                        heightScale,
                        grid.gridProperties.CellSize
                    )
                );
            }
        }
    }

    public string FlagsToString(GridGenerator2D.TileFlags flags)
    {
        return
            $"Walkable({flags.HasFlag(GridGenerator2D.TileFlags.Walkable)}) | " +
            $"Diggable({flags.HasFlag(GridGenerator2D.TileFlags.Diggable)}) | " +
            $"Claimable({flags.HasFlag(GridGenerator2D.TileFlags.Claimable)}) | " +
            $"Claimed({flags.HasFlag(GridGenerator2D.TileFlags.Claimed)}) | " +
            $"Reserved({flags.HasFlag(GridGenerator2D.TileFlags.Reserved)}) | " +
            $"Occupied({flags.HasFlag(GridGenerator2D.TileFlags.Occupied)})";
    }

    public string TerrainToString(GridGenerator2D.TerrainType terrain)
    {
        return
            $"None({terrain == GridGenerator2D.TerrainType.None}) | " +
            $"Tile({terrain == GridGenerator2D.TerrainType.Tile}) | " +
            $"Corrupt({terrain == GridGenerator2D.TerrainType.Corrupt}) | " +
            $"Water({terrain == GridGenerator2D.TerrainType.Water})";
    }

    public string EffectToString(GridGenerator2D.EffectType effect)
    {
        return
            $"None({effect == GridGenerator2D.EffectType.None}) | " +
            $"Frozen({effect == GridGenerator2D.EffectType.Frozen}) | " +
            $"Corrupted({effect == GridGenerator2D.EffectType.Corrupted}) | " +
            $"Electrified({effect == GridGenerator2D.EffectType.Electrified}) | " +
            $"Trapped({effect == GridGenerator2D.EffectType.Trapped})";
    }

    public string RoomToString(GridGenerator2D.RoomType room)
    {
        return room.ToString();
    }
}