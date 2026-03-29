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
    // PreviewSystem.cs
    public void UpdatePosition(Vector3 position, bool validity, Vector2Int rotatedSize, int rotation, Vector2Int originalSize)
    {
        if (previewObject != null)
        {
            // 🚩 核心逻辑：计算当前网格区域的中心点
            // position 是格子的左下角，我们加上 (rotatedSize / 2) 得到区域中心
            Vector3 centerOffset = new Vector3(rotatedSize.x / 2f, 0, rotatedSize.y / 2f);
            Vector3 targetPosition = position + centerOffset;

            // 🚩 处理 Pivot 不在中心的问题：
            // 我们假设 Prefab 的内部网格是相对于其 Pivot 摆放的。
            // 为了让“模型中心”对齐“网格中心”，我们需要减去“模型原始尺寸的一半”
            // 这里的 rotation 作用于模型，模型绕着自己的 Pivot 转
            previewObject.transform.rotation = Quaternion.Euler(0, rotation, 0);

            // 计算模型在当前旋转下的局部中心偏移
            // 如果 originalSize 是 (2, 5)，0度时中心在 (1, 2.5)；90度时中心在 (-2.5, 1) 等
            Vector3 localModelCenter = Quaternion.Euler(0, rotation, 0) * new Vector3(originalSize.x / 2f, 0, originalSize.y / 2f);

            previewObject.transform.position = targetPosition - localModelCenter + new Vector3(0, previewYOffset, 0);
        }

        // 指示器（黄色框）依然对齐左下角，这是标准的网格表现
        cellIndicator.transform.position = position;
        PrepareCursor(rotatedSize);
        ApplyFeedbackToCursor(validity);
        ApplyFeedbackToPreview(validity);
    }

    // 🚩 此时不需要原本那个复杂的 GetOffset 了，我们可以删掉它或改写
    public Vector3 GetModelPlacementOffset(Vector2Int originalSize, Vector2Int rotatedSize, int rotation)
    {
        Vector3 gridCenter = new Vector3(rotatedSize.x / 2f, 0, rotatedSize.y / 2f);
        Vector3 modelCenterInWorld = Quaternion.Euler(0, rotation, 0) * new Vector3(originalSize.x / 2f, 0, originalSize.y / 2f);
        return gridCenter - modelCenterInWorld;
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
    // PreviewSystem.cs 中的 GetOffset
    // PreviewSystem.cs
    public Vector3 GetOffset(Vector2Int originalSize, int rotation)
    {
        // 假设原始尺寸 x=2(宽), y=5(长)
        switch (rotation)
        {
            case 0:
                return Vector3.zero;
            case 90:
                // 模型向左甩出了 5 格，补偿回 +X 方向
                return new Vector3(originalSize.y, 0, 0);
            case 180:
                // 模型向左甩出 2 格，向下甩出 5 格，补偿回 +X, +Z
                return new Vector3(originalSize.x, 0, originalSize.y);
            case 270:
                // 模型向下甩出了 2 格，补偿回 +Z 方向
                return new Vector3(0, 0, originalSize.x);
            default:
                return Vector3.zero;
        }
    }

   
}