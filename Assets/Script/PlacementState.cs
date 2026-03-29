using UnityEngine;

public class PlacementState : IPlacementState
{
    private int selectedObjectIndex;
    private Grid grid;
    private PreviewSystem preview;
    private ObjectsDatabaseSO database;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;

    private int rotation = 0;

    public PlacementState(int ID, Grid grid, PreviewSystem preview, ObjectsDatabaseSO database, GridData floorData, GridData furnitureData, ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.preview = preview;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        var data = database.objectsData[selectedObjectIndex];

        preview.StartShowingPlacementPreview(data.Prefab, data.Size);
    }

    public void Rotate()
    {
        rotation = (rotation + 90) % 360;
    }

    public void EndState()
    {
        preview.StopShowingPreview();
    }

    // PlacementState.cs
    // PlacementState.cs
    public void OnAction(Vector3Int gridPosition)
    {
        var data = database.objectsData[selectedObjectIndex];
        GridData selectedData = data.ID == 0 ? floorData : furnitureData;

        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        if (!selectedData.CanPlaceObjectAt(gridPosition, rotatedSize, rotation))
            return;

        // 🚩 使用中心对齐逻辑计算最终物理位置
        Vector3 worldPosOrigin = grid.CellToWorld(gridPosition);
        Vector3 finalOffset = preview.GetModelPlacementOffset(data.Size, rotatedSize, rotation);
        Vector3 finalWorldPos = worldPosOrigin + finalOffset;

        int index = objectPlacer.PlaceObject(data.Prefab, finalWorldPos, rotation);

        selectedData.AddObjectAt(gridPosition, rotatedSize, data.ID, index, rotation);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        var data = database.objectsData[selectedObjectIndex];
        GridData selectedData = data.ID == 0 ? floorData : furnitureData;

        Vector2Int rotatedSize = GetRotatedSize(data.Size);
        bool canPlace = selectedData.CanPlaceObjectAt(gridPosition, rotatedSize, rotation);

        // 🚩 修正：传递原始尺寸给预览系统计算偏移
        preview.UpdatePosition(
            grid.CellToWorld(gridPosition),
            canPlace,
            rotatedSize,
            rotation,
            data.Size
        );
    }

    private Vector2Int GetRotatedSize(Vector2Int size)
    {
        if (rotation == 90 || rotation == 270)
            return new Vector2Int(size.y, size.x);
        return size;
    }
}