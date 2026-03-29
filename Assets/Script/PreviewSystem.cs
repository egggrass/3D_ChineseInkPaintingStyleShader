using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField] private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 && size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void UpdateSize(Vector2Int size)
    {
        PrepareCursor(size);
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(
        Vector3 position,
        bool validity,
        Vector2Int rotatedSize,
        int rotation
   )
    {
        // 1️⃣ 更新 previewObject
        if (previewObject != null)
        {
            MovePreview(position);
            UpdateRotation(rotation);
            PrepareCursor(rotatedSize); // scale 用旋转后的大小
            ApplyFeedbackToPreview(validity);
        }

        // 2️⃣ 更新 cellIndicator
        MoveCursor(position, rotation); // offset 用原始大小
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    public void MoveCursor(Vector3 position,  int rotation)
    {
    
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    public void UpdateRotation(int rotation)
    {
        if (previewObject != null)
            previewObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
           
    }

    /// <summary>
    /// 计算左下角 pivot 对齐偏移
    /// </summary>
    private Vector3 GetOffset(Vector2Int size, int rotation)
    {
        switch (rotation)
        {
            case 0: return Vector3.zero;
            case 90: return new Vector3(size.y - 1, 0, 0);
            case 180: return new Vector3(size.x - 1, 0, size.y - 1);
            case 270: return new Vector3(0, 0, size.x - 1);
            default: return Vector3.zero;
        }
    }
}