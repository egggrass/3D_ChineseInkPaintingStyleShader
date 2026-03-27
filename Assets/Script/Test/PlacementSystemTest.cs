using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class PlacementSystemTest : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]

    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData floorData , furnitureData;

    private List<GameObject> placedGameObjects = new();
    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

   

    private void Start()
    {
        StopPlacement();
        floorData = new ();
        furnitureData = new();
       
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data  => data.ID == ID);
        if(selectedObjectIndex < 0 )
        {
            Debug.LogError($"no id found");
        }
        gridVisualization.SetActive( true );
        preview.StartShowingPlacementPreview(
            database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
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

        bool placementValidity = Checkplacementvalidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        placedGameObjects.Add( newObject );
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;
        selectedData.AddObjectAt(gridPosition, 
                database.objectsData[selectedObjectIndex].Size, 
                database.objectsData[selectedObjectIndex].ID,
                placedGameObjects.Count -1);
        preview.UpdatePosition(grid.CellToWorld(gridPosition),false);
    }

    private bool Checkplacementvalidity(Vector3Int gridposition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;

        return selectedData.CanPlaceObjectAt(gridposition, database.objectsData[selectedObjectIndex].Size);
    }
    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive( false );
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= PlaceStructure;
        lastDetectedPosition = Vector3Int.zero;

    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
        return; 
        
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if(lastDetectedPosition != gridPosition)
        {
            bool placementValidity = Checkplacementvalidity(gridPosition, selectedObjectIndex);

            mouseIndicator.transform.position = mousePosition;
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
       
    }
}
