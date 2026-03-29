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

    public PlacementState(
        int ID,
        Grid grid,
        PreviewSystem preview,
        ObjectsDatabaseSO database,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.preview = preview;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex =
            database.objectsData.FindIndex(data => data.ID == ID);

        var data = database.objectsData[selectedObjectIndex];

        preview.StartShowingPlacementPreview(
            data.Prefab,
            GetRotatedSize(data.Size)
        );
    }

    public void Rotate()
    {
        rotation += 90;
        if (rotation >= 360)
            rotation = 0;
    }

    public void EndState()
    {
        preview.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        var data = database.objectsData[selectedObjectIndex];

        GridData selectedData =
            data.ID == 0 ? floorData : furnitureData;

        if (!selectedData.CanPlaceObjectAt(
            gridPosition,
            data.Size,
            rotation))
            return;

        int index = objectPlacer.PlaceObject(
            data.Prefab,
            grid.CellToWorld(gridPosition),
            rotation);

        selectedData.AddObjectAt(
            gridPosition,
            data.Size,
            data.ID,
            index,
            rotation);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        var data = database.objectsData[selectedObjectIndex];

        GridData selectedData =
            data.ID == 0 ? floorData : furnitureData;

        bool canPlace = selectedData.CanPlaceObjectAt(
            gridPosition,
            data.Size,
            rotation);

        Vector2Int originalSize = data.Size;
        Vector2Int rotatedSize = GetRotatedSize(data.Size);

        preview.UpdatePosition(
            grid.CellToWorld(gridPosition),
            canPlace,
            rotatedSize,   // 给显示
            rotation
        );
    

    }

    private Vector2Int GetRotatedSize(Vector2Int size)
    {
        if (rotation == 90 || rotation == 270)
            return new Vector2Int(size.y, size.x);

        return size;
    }
}