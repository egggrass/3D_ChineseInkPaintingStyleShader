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
    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int size, int rotation)
    {
        List<Vector3Int> result = new();
        // 既然外面已经传进来了 rotatedSize，这里直接循环即可
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                result.Add(gridPosition + new Vector3Int(x, 0, y));
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