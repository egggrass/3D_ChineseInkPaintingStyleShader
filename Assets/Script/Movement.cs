using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 获取水平和垂直输入
        float moveX = Input.GetAxis("Horizontal"); // A/D 或 ←/→
        float moveZ = Input.GetAxis("Vertical");   // W/S 或 ↑/↓

        // 构造移动向量（XZ平面）
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 移动
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}