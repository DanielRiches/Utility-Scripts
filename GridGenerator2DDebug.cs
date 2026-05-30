using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GridGenerator2DDebug : MonoBehaviour
{
    [Header("Grid Reference")]
    public GridGenerator2D grid;

    [Header("Tile Debug")]
    [SerializeField] private bool debugTileData = false;

    [Tooltip("-1 = full grid, otherwise specific world space position")]
    [SerializeField] private int2 tileCoord = new int2(-1, -1);

    [SerializeField] private bool cancelDebug;

    // ------------------------------------------------------------
    // GIZMOS
    // ------------------------------------------------------------

    [Header("Gizmos")]
    [SerializeField] private bool drawCells = false;
    [SerializeField] private bool drawGuideCells = false;

    [SerializeField, Range(0f, 20000f)]
    private float drawRadius = 50f;

    [Header("Radius Origin")]
    [SerializeField] private bool useFirstCellAsOrigin = false;

    // ------------------------------------------------------------
    // INTERNAL
    // ------------------------------------------------------------

    private Coroutine sampleCoroutine;
    private bool cancelRequested;

    private Plane[] frustumPlanes = new Plane[6];

    private Vector2Int currentSampledTile = new Vector2Int(-1, -1);

    private void Update()
    {
        if (debugTileData)
        {
            debugTileData = false;
            SampleTileFlags(tileCoord);
        }

        if (cancelDebug)
        {
            cancelDebug = false;
            CancelSampling();
        }
    }

    // ------------------------------------------------------------
    // TILE DEBUG
    // ------------------------------------------------------------

    public void SampleTileFlags(int2 coord)
    {
        if (grid == null || !grid.IsGenerated)
            return;

        CancelSampling();

        cancelRequested = false;
        sampleCoroutine = StartCoroutine(SampleCoroutine(coord));
    }

    public void CancelSampling()
    {
        cancelRequested = true;
        currentSampledTile = new Vector2Int(-1, -1);

        if (sampleCoroutine != null)
        {
            StopCoroutine(sampleCoroutine);
            sampleCoroutine = null;
        }

        Debug.Log("Tile sampling cancelled.");
    }

    private IEnumerator SampleCoroutine(int2 coord)
    {
        int width = grid.gridProperties.GridWidth;
        int height = grid.gridProperties.GridHeight;

        if (coord.x >= 0 && coord.y >= 0)
        {
            if (!grid.InBounds(coord.x, coord.y))
                yield break;

            currentSampledTile = new Vector2Int(coord.x, coord.y);
            PrintTile(coord.x, coord.y);
            currentSampledTile = new Vector2Int(-1, -1);
            yield break;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cancelRequested)
                {
                    currentSampledTile = new Vector2Int(-1, -1);
                    yield break;
                }

                currentSampledTile = new Vector2Int(x, y);
                PrintTile(x, y);

                yield return null;
            }
        }

        currentSampledTile = new Vector2Int(-1, -1);
    }

    private void PrintTile(int x, int y)
    {
        float3 world = grid.GetWorldPosition(x, y);
        float3 center = grid.GetCenterPosition(x, y);

        Debug.Log(
            $"Tile ({x},{y}) | World: {world} | Center: {center} | " +
            $"Flags: {grid.GetFlags(x, y)} | Terrain: {grid.GetTerrain(x, y)} | " +
            $"Effect: {grid.GetEffect(x, y)} | Room: {grid.GetRoom(x, y)}"
        );
    }

    // ------------------------------------------------------------
    // GIZMOS
    // ------------------------------------------------------------

    private void OnDrawGizmos()
    {
        if (!drawCells || grid == null || !grid.IsGenerated)
            return;

        Camera cam = Camera.main;
        if (cam == null)
            return;

        GeometryUtility.CalculateFrustumPlanes(cam, frustumPlanes);

        Vector3 origin = cam.transform.position;

        if (useFirstCellAsOrigin)
        {
            float3 first = grid.GetWorldPosition(0, 0);
            origin = (Vector3)first;
        }

        float radiusSqr = drawRadius * drawRadius;

        int width = grid.gridProperties.GridWidth;
        int height = grid.gridProperties.GridHeight;

        float cellSize = grid.gridProperties.CellSize;
        float baseHeight = Mathf.Max(0.1f, grid.gridProperties.CellHeight);

        int maxX = width - 1;
        int maxY = height - 1;

        int centerX = width / 2;
        int centerY = height / 2;

        bool useFrustumCulling = !useFirstCellAsOrigin;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float3 world = grid.GetWorldPosition(x, y);
                Vector3 worldPos = (Vector3)world;

                if ((worldPos - origin).sqrMagnitude > radiusSqr)
                    continue;

                if (useFrustumCulling)
                {
                    Bounds b = new Bounds(worldPos, Vector3.one * cellSize);
                    if (!GeometryUtility.TestPlanesAABB(frustumPlanes, b))
                        continue;
                }

                bool isCenter = (x == centerX && y == centerY);

                bool isCorner =
                    (x == 0 && y == 0) ||
                    (x == maxX && y == 0) ||
                    (x == 0 && y == maxY) ||
                    (x == maxX && y == maxY);

                bool isGuide = (x == centerX || y == centerY);

                bool isSamplingThisTile =
                    (x == currentSampledTile.x && y == currentSampledTile.y);

                float drawHeight = baseHeight;
                Color color = Color.white;

                if (isSamplingThisTile)
                {
                    color = Color.blue;
                    drawHeight *= 15f;
                }
                else if (drawGuideCells)
                {
                    if (isCenter)
                        color = Color.red;
                    else if (isCorner)
                        color = Color.green;
                    else if (isGuide)
                        color = new Color(0f, 1f, 0f, 0.35f);
                }

                if (isCenter || isCorner)
                    drawHeight *= 5f;

                Gizmos.color = color;

                Gizmos.DrawWireCube(
                    new Vector3(
                        worldPos.x,
                        worldPos.y + drawHeight * 0.5f,
                        worldPos.z
                    ),
                    new Vector3(cellSize, drawHeight, cellSize)
                );
            }
        }
    }
}