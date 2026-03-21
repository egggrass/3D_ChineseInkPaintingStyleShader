using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private AudioClip correctPlacementClip, wrongPlacementClip;
    [SerializeField]
    private AudioSource source;

    private GridData floorData, furnitureData;

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer objectPlacer;

    IPlacementState buildingState;

    [SerializeField]
   // private SoundFeedback soundFeedback;

    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new();
        furnitureData = new();

        SpawnInitialObject(1, new Vector3Int(-7, 0, -4));
        SpawnInitialObject(2, new Vector3Int(2, 0, -2));
        SpawnInitialObject(3, new Vector3Int(8, 0, 0));
        SpawnInitialObject(4, new Vector3Int(-1, 0, -4));
        SpawnInitialObject(5, new Vector3Int(-3, 0, -5));
        SpawnInitialObject(6, new Vector3Int(-5, 0, -4));
     
      
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           objectPlacer);
                                         //  soundFeedback);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
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

        buildingState.OnAction(gridPosition);

    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    //{
    //    GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
    //        floorData : 
    //        furnitureData;

    //    return selectedData.CanPlaceObejctAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    //}

    private void StopPlacement()
    {
       // soundFeedback.PlaySound(SoundType.Click);
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }

    }

    private void SpawnInitialObject(int objectID, Vector3Int gridPosition)
    {
        // 1. 从数据库找到该物体的信息 (根据 ID)
        int selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == objectID);
        if (selectedObjectIndex == -1) return; // 没找到 ID

        var itemData = database.objectsData[selectedObjectIndex];

        // 2. 计算物理世界位置
        // 注意：如果你的 Pivot 在左下角且要用之前讨论的对齐中心预览逻辑，这里要加偏移
        Vector3 worldPosition = grid.GetCellCenterWorld(gridPosition);

        // 3. 通知 ObjectPlacer 生成模型，并获取它在列表中的索引
        int placedObjectIndex = objectPlacer.PlaceObject(itemData.Prefab, worldPosition);

        // 4. 根据物体的 ID 判断它是地板还是家具，并存入对应的 GridData
        GridData selectedData = itemData.ID == 0 ? floorData : furnitureData;

        // 5. 让逻辑层记录下这个物体
        selectedData.AddObjectAt(gridPosition, itemData.Size, itemData.ID, placedObjectIndex);
    }


}
