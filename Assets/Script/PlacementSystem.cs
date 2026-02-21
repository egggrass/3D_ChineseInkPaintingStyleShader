using System;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioSource audio;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.ObjectsData.FindIndex(data => data.ID == ID);
        if(selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found{ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStrcture;
        inputManager.OnExit += StopPlacement;
       
    }

    private void PlaceStrcture()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }

       // audio.Play();

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
       
        GameObject newObject = Instantiate(database.ObjectsData[selectedObjectIndex].Prefab);
        //cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        
        newObject.transform.position = grid.GetCellCenterWorld(gridPosition);
     //   Vector3 worldPosition = grid.CellToWorld(gridPosition);
      //  worldPosition.y += 0.5f;

      //  newObject.transform.position = worldPosition;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStrcture;
        inputManager.OnExit -= StopPlacement;

    }
    private void Update()
    {
        if(selectedObjectIndex <0)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;

       //cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        cellIndicator.transform.position = grid.GetCellCenterWorld(gridPosition);
    }

}
