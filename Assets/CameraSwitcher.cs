using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [Header("相机列表")]
    public Camera firstPersonCamera;
    public Camera topViewCamera;

    public bool isMainView = true;

    void Start()
    {
        // 游戏开始时，确保视角状态正确
        EnableCamera(true);
    }

    // 该方法绑定到按钮的 OnClick 事件
    public void SwitchView()
    {
        isMainView = !isMainView;
        EnableCamera(isMainView);
    }

    private void EnableCamera(bool mainActive)
    {
        // WebGL 优化：直接开关组件，避免频繁 Instantiate/Destroy
        firstPersonCamera.enabled = mainActive;
        topViewCamera.enabled = !mainActive;

        // 如果你的相机上有额外的控制脚本（比如旋转、缩放），也在这里同步开关
        // firstPersonCamera.GetComponent<MonoBehaviour>().enabled = mainActive;
    }
}