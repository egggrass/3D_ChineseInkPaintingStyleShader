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


    private void Start()
    {
        StopPlacement();
        floorData = new ();
        furnitureData = new();
       
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
        if (inputManager.IsPointerOverUI())
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

        if(lastDetectedPosition != gridPosition)
        {
            placementState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
       
    }
}
