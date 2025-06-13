# Movement.cs
สคริปต์นี้ใช้ควบคุมการเคลื่อนที่ของตัวละครในเกมแบบมุมมองบุคคลที่หนึ่ง
```csharp
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")] // กลุ่มการตั้งค่าเกี่ยวกับการเคลื่อนที่
    public float moveSpeed = 5f; // ความเร็วในการเคลื่อนที่
    public float jumpForce = 5f; // แรงกระโดด

    [Header("Mouse Look")] // กลุ่มการตั้งค่าเกี่ยวกับการหมุนกล้องด้วยเมาส์
    public float mouseSensitivity = 2f; // ความไวของเมาส์
    public Transform cameraTransform; // อ้างอิงตำแหน่งกล้องที่ต้องหมุน
    private float pitch = 0f; // มุมกล้องในแนวตั้ง (ก้ม/เงย)

    private Rigidbody rb; // Rigidbody ของตัวละคร
    private bool isGrounded; // ตรวจสอบว่าตัวละครอยู่บนพื้นหรือไม่

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // ดึง Rigidbody ของตัวเอง
        Cursor.lockState = CursorLockMode.Locked; // ล็อกเคอร์เซอร์ให้อยู่กลางจอ
    }

    void Update()
    {
        LookAround(); // เรียกการควบคุมเมาส์เพื่อหมุนมุมกล้อง
        Jump();       // ตรวจสอบและสั่งกระโดดหากผู้เล่นกด
    }

    void FixedUpdate()
    {
        Move(); // เคลื่อนที่ด้วยฟิสิกส์ใน FixedUpdate เพื่อความเสถียร
    }

    // ฟังก์ชันควบคุมการหมุนกล้องด้วยเมาส์
    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // การหมุนซ้ายขวา
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity; // การหมุนขึ้นลง

        pitch -= mouseY; // ลด pitch ตามการขยับเมาส์แนวตั้ง
        pitch = Mathf.Clamp(pitch, -90f, 90f); // จำกัดมุมไม่ให้หมุนเกินไป

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0); // หมุนกล้องแนวตั้ง
        transform.Rotate(Vector3.up * mouseX); // หมุนตัวละครในแนวนอน (แกน Y)
    }

    // ฟังก์ชันควบคุมการเคลื่อนที่
    void Move()
    {
        float x = Input.GetAxis("Horizontal"); // รับค่าการกดปุ่ม A/D หรือ ซ้าย/ขวา
        float z = Input.GetAxis("Vertical");   // รับค่าการกดปุ่ม W/S หรือ หน้า/หลัง

        Vector3 move = transform.right * x + transform.forward * z; // ทิศทางการเคลื่อนที่
        Vector3 velocity = move * moveSpeed; // คำนวณความเร็วจากทิศทางและค่าความเร็ว
        velocity.y = rb.velocity.y; // รักษาความเร็วแนวแกน Y เดิม (กันตกหรือกระโดดผิดพลาด)
        rb.velocity = velocity; // ใช้ความเร็วใหม่กับ Rigidbody
    }

    // ฟังก์ชันควบคุมการกระโดด
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // เมื่อกดปุ่ม Jump และอยู่บนพื้น
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // เพิ่มแรงในแนวขึ้น
        }
    }

    // ตรวจสอบว่าตัวละครกำลังสัมผัสพื้น
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // ถ้าแท็กของวัตถุที่สัมผัสคือ "Ground"
        {
            isGrounded = true; // อยู่บนพื้น
        }
    }

    // ตรวจสอบว่าตัวละครไม่ได้สัมผัสพื้นแล้ว
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // ถ้าหลุดออกจากพื้น
        {
            isGrounded = false; // ไม่อยู่บนพื้นแล้ว
        }
    }

    // ฟังก์ชันแสดงเป้าเล็ง (Crosshair) ตรงกลางจอ
    void OnGUI()
    {
        float size = 10f;
        float xMin = (Screen.width / 2) - (size / 2);
        float yMin = (Screen.height / 2) - (size / 2);
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(xMin, yMin, size, size), Texture2D.whiteTexture); // วาดสี่เหลี่ยมสีขาวขนาดเล็กตรงกลางจอ
    }
}
```
