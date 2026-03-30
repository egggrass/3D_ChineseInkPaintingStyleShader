using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class PlacementSystemTest : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]

    private ObjectsDatabaseSO database;
   
    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData , furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IPlacementState placementState;

    public Movement movement;

    private void Start()
    {
        StopPlacement();
        floorData = new ();
        furnitureData = new();
        LoadSceneView();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        placementState = new PlacementState(ID,grid,preview,database,floorData,furnitureData,objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;


    }
    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        placementState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI() || movement.isMoving)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        placementState.OnAction(gridPosition);
    }

    //private bool Checkplacementvalidity(Vector3Int gridposition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
    //        floorData :
    //        furnitureData;

    //    return selectedData.CanPlaceObjectAt(gridposition, database.objectsData[selectedObjectIndex].Size);
    //}
    private void StopPlacement()
    {
        if (placementState == null)
            return;
        gridVisualization.SetActive( false );
        placementState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= PlaceStructure;
        lastDetectedPosition = Vector3Int.zero;
        placementState = null;  

    }

    private void Update()
    {
        if (placementState == null)
            return;

       
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (Input.GetKeyDown(KeyCode.R))
        {
            placementState.Rotate();
            placementState.UpdateState(gridPosition);
        }

        if (lastDetectedPosition != gridPosition)
        {
            placementState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    // 在 PlacementSystemTest.cs 中修正这个方法
    private void SpawnInitialObject(int objectID, Vector3Int gridPosition)
    {
        int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == objectID);
        if (selectedObjectIndex == -1) return;

        var itemData = database.objectsData[selectedObjectIndex];

        // 🚩 关键点：初始生成默认为 0 度旋转，偏移量通常为 Vector3.zero
        // 但如果以后你想初始生成旋转后的物体，这里必须调用 preview.GetOffset
        int initialRotation = 0;
        Vector3 offset = preview.GetOffset(itemData.Size, initialRotation);
        Vector3 worldPosition = grid.CellToWorld(gridPosition) + offset;

        int placedObjectIndex = objectPlacer.PlaceObject(itemData.Prefab, worldPosition, initialRotation);

        GridData selectedData = itemData.ID == 0 ? floorData : furnitureData;

        // 初始生成默认不旋转，所以直接用 itemData.Size
        selectedData.AddObjectAt(gridPosition, itemData.Size, itemData.ID, placedObjectIndex, initialRotation);
    }
    public void LoadSceneView()
    {
        SpawnInitialObject(1, new Vector3Int(-6, 0, -4));
        SpawnInitialObject(2, new Vector3Int(0, 0, -2));
        SpawnInitialObject(3, new Vector3Int(8, 0, -4));
        SpawnInitialObject(4, new Vector3Int(0, 0, -4));
        SpawnInitialObject(5, new Vector3Int(-2, 0, -5));
        SpawnInitialObject(6, new Vector3Int(-4, 0, -4));
        SpawnInitialObject(7, new Vector3Int(1, 0, -5));
        SpawnInitialObject(8, new Vector3Int(4, 0, 0));
        SpawnInitialObject(9, new Vector3Int(4, 0, 4));
        SpawnInitialObject(10, new Vector3Int(6, 0, 4));
        SpawnInitialObject(11, new Vector3Int(8, 0, 0));
        SpawnInitialObject(12, new Vector3Int(0, 0, 0));
        SpawnInitialObject(13, new Vector3Int(-4, 0, 0));
        SpawnInitialObject(14, new Vector3Int(-2, 0, -2));
        SpawnInitialObject(15, new Vector3Int(2, 0, -5));
        SpawnInitialObject(16, new Vector3Int(1, 0, 3));
    }
}
