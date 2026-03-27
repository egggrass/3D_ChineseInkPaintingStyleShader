using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CharacterController controller;

    public bool isMoving = true;

    public GameObject BuildUI;
    public GameObject head;
    public PreviewSystem preview;

    [Header("设置")]
    public float sensitivity = 200f; // 灵敏度

    [Header("视野限制（角度）")]
    public float yMin = -60f; // 向下看极限
    public float yMax = 60f;  // 向上看极限
    public float xMin = -70f; // 向左看极限
    public float xMax = 70f;  // 向右看极限
    private float rotationX = 0f; // 垂直角度
    private float rotationY = 0f; // 水平角度
   
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
       
            isMoving = !isMoving;
            BuildUI.SetActive(!isMoving);
            
        if (!isMoving) { return; }
        preview.StopShowingPreview();
        VisionMovement();
        // 获取水平和垂直输入
        float moveX = Input.GetAxis("Horizontal"); // A/D 或 ←/→
        float moveZ = Input.GetAxis("Vertical");   // W/S 或 ↑/↓

        // 构造移动向量（XZ平面）
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 cameramove = transform.up * moveX + transform.right * moveZ;
        // 移动
        controller.Move(move * moveSpeed * Time.deltaTime);
       
    }

    public void VisionMovement()
    {
        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // 1. 计算水平旋转 (左右)
        rotationY += mouseX;
        rotationY = Mathf.Clamp(rotationY, xMin, xMax);

        // 2. 计算垂直旋转 (上下)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, yMin, yMax);

        // 3. 一键应用旋转
        head.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}