using UnityEngine;

public class RemovingState : IPlacementState
{
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData floorData;
    private GridData furnitureData;
    private ObjectPlacer objectPlacer;

    public RemovingState(
        Grid grid,
        PreviewSystem previewSystem,
        GridData floorData,
        GridData furnitureData,
        ObjectPlacer objectPlacer)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = GetDataAtPosition(gridPosition);

        if (selectedData == null)
            return;

        int index = selectedData.GetRepresentationIndex(gridPosition);

        if (index == -1)
            return;

        // 移除逻辑：GridData 会根据该位置存储的 data.occupiedPositions 自动清空所有占用格子
        selectedData.RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(index);
    }

    private GridData GetDataAtPosition(Vector3Int gridPosition)
    {
        if (furnitureData.GetRepresentationIndex(gridPosition) != -1)
            return furnitureData;

        if (floorData.GetRepresentationIndex(gridPosition) != -1)
            return floorData;

        return null;
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        return GetDataAtPosition(gridPosition) != null;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool isValid = CheckIfSelectionIsValid(gridPosition);
        Vector3 worldPos = grid.CellToWorld(gridPosition);

        // 🚩 修正：匹配 PreviewSystem 新的参数签名
        // 移除预览不需要旋转，大小固定为 1x1
        previewSystem.UpdatePosition(
            worldPos,
            isValid,
            Vector2Int.one, // rotatedSize
            0,              // rotation
            Vector2Int.one  // originalSize
        );
    }

    public void Rotate() { } // 移除模式下无需旋转
}