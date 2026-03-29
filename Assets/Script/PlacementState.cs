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

    public void OnAction(Vector3Int gridPosition)
    {
        var data = database.objectsData[selectedObjectIndex];
        GridData selectedData = data.ID == 0 ? floorData : furnitureData;

        // 🚩 修正：使用旋转后的尺寸判断合法性
        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        if (!selectedData.CanPlaceObjectAt(gridPosition, rotatedSize, rotation))
            return;

        // 🚩 修正：获取偏移量并应用到放置位置
        Vector3 offset = preview.GetOffset(data.Size, rotation);
        Vector3 worldPos = grid.CellToWorld(gridPosition) + offset;

        int index = objectPlacer.PlaceObject(data.Prefab, worldPos, rotation);

        // 🚩 修正：在 GridData 中记录旋转后的实际占用面积
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