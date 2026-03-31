using UnityEngine;

public class CameraImageFollower : MonoBehaviour
{
    [Header("引用")]
    public Transform playerTransform; // 拖入你的玩家物体

    [Header("设置")]
    public bool followX = true;  // 是否跟随 X
    public bool followY = false; // 是否跟随 Y
    private float initialZ;      // 保持 Z 轴固定
    private float initialY;      // 保持 Y 轴固定

    void Start()
    {
        // 记录相机初始的 Z 和 Y，防止它飞走
        initialZ = transform.position.z;
        initialY = transform.position.y;
    }

    // 使用 LateUpdate 确保在玩家移动完之后，相机再跟上，防止画面抖动
    void LateUpdate()
    {
        if (playerTransform == null) return;

        // 获取玩家当前的 X
        float targetX = playerTransform.position.x;

        // 如果也想跟 Y，就获取玩家的 Y，否则用初始 Y
       float targetY = followY ? playerTransform.position.y : initialY;

        // 重新赋值：X 跟随，Y 和 Z 保持独立
        transform.position = new Vector3(targetX, targetY, initialZ);
    }
}