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

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    // 🚩 修正：增加 originalSize 参数，内部处理 Offset
    public void UpdatePosition(Vector3 position, bool validity, Vector2Int rotatedSize, int rotation, Vector2Int originalSize)
    {
        if (previewObject != null)
        {
            Vector3 offset = GetOffset(originalSize, rotation);
            previewObject.transform.position = position + offset + new Vector3(0, previewYOffset, 0);
            previewObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        cellIndicator.transform.position = position;
        PrepareCursor(rotatedSize);
        ApplyFeedbackToCursor(validity);
        ApplyFeedbackToPreview(validity);
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

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    // 🚩 修正：计算左下角 Pivot 旋转后的位移补偿
    public Vector3 GetOffset(Vector2Int size, int rotation)
    {
        switch (rotation)
        {
            case 0: return Vector3.zero;
            case 90: return new Vector3(size.y, 0, 0);
            case 180: return new Vector3(size.x, 0, size.y);
            case 270: return new Vector3(0, 0, size.x);
            default: return Vector3.zero;
        }
    }
}