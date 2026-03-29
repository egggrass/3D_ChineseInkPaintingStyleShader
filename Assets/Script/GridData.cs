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

    private List<Vector3Int> CalculatePositions(
        Vector3Int gridPosition,
        Vector2Int objectSize,
        int rotation
    )
    {
        List<Vector3Int> result = new();

        Vector2Int size = objectSize;

        if (rotation == 90 || rotation == 270)
            size = new Vector2Int(objectSize.y, objectSize.x);

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