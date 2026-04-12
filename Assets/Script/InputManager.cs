using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    public CameraSwitcher cameraSwicher;
    public Camera mainCamera;
    public Camera paintingCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
             OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUI()
    => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition ()
    {
        if (cameraSwicher.isMainView)
        {
            sceneCamera = mainCamera;
        }
        else sceneCamera = paintingCamera;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }


}
