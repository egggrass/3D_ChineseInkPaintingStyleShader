using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(
        Vector3Int gridPosition,
        Vector2Int objectSize,
        int ID,
        int placedObjectIndex,
        int rotation
    )
    {
        List<Vector3Int> positions =
            CalculatePositions(gridPosition, objectSize, rotation);

        PlacementData data =
            new PlacementData(positions, ID, placedObjectIndex, rotation);

        foreach (var pos in positions)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Cell occupied {pos}");

            placedObjects[pos] = data;
        }
    }

    public bool CanPlaceObjectAt(
        Vector3Int gridPosition,
        Vector2Int objectSize,
        int rotation
    )
    {
        List<Vector3Int> positions =
            CalculatePositions(gridPosition, objectSize, rotation);

        foreach (var pos in positions)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    // 在 GridData.cs 中
    // GridData.cs
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int size, int rotation)
    {
        List<Vector3Int> result = new();

        // 🚩 核心：size 已经是 rotatedSize。
        // 我们只负责从起点开始，向右填 X 个，向上填 Y 个。
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                result.Add(gridPosition + new Vector3Int(x, 0, z));
            }
        }
        return result;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
            return -1;

        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (!placedObjects.ContainsKey(gridPosition))
            return;

        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }
    public int Rotation { get; private set; }

    public PlacementData(
        List<Vector3Int> occupiedPositions,
        int iD,
        int placedObjectIndex,
        int rotation
    )
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        Rotation = rotation;
    }

}