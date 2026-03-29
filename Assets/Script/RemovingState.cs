using System.Drawing;
using UnityEngine;

public class RemovingState : IPlacementState
{
    Grid grid;
    PreviewSystem previewSystem;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;
    

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

        selectedData.RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(index);
    }

    // ✅ 核心：统一查询逻辑
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

      
        // 👇 基础版（1x1）
        previewSystem.UpdatePosition(
            worldPos,
            isValid,
            Vector2Int.one,
            0

        );
    }

    public void Rotate() { }
}