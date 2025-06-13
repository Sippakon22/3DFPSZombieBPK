using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Health Settings")] // กลุ่มสำหรับการตั้งค่าพลังชีวิตของผู้เล่น
    public int maxHealth = 100; // พลังชีวิตสูงสุดของผู้เล่น
    private int currentHealth;  // พลังชีวิตปัจจุบันของผู้เล่น

    [Header("Game Over")] // กลุ่มสำหรับการจัดการ UI เมื่อตัวละครตาย
    public GameObject gameOverUI; // อ้างอิงถึง GameObject ของหน้าจอ Game Over

    private bool isDead = false; // ใช้ตรวจสอบว่าผู้เล่นตายแล้วหรือยัง

    [Header("Kill")] // กลุ่มสำหรับการแสดงผลและเก็บข้อมูลจำนวนศัตรูที่ฆ่าได้
    public TextMeshProUGUI killUI; // UI ที่ใช้แสดงจำนวนศัตรูที่ผู้เล่นฆ่าได้
    private int kill; // ตัวแปรเก็บจำนวนศัตรูที่ฆ่าได้

    void Start()
    {
        currentHealth = maxHealth; // เริ่มเกมด้วยพลังชีวิตเต็ม

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false); // ซ่อน UI Game Over ตอนเริ่มเกม
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // ถ้าผู้เล่นตายแล้ว จะไม่รับดาเมจอีก

        currentHealth -= damage; // ลดพลังชีวิตตามค่าดาเมจ
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // จำกัดค่าพลังชีวิตไม่ให้ต่ำกว่า 0 หรือเกิน max

        Debug.Log("Player took damage. Current HP: " + currentHealth); // แสดงข้อความใน Console ว่าผู้เล่นโดนโจมตี

        if (currentHealth <= 0)
        {
            Die(); // ถ้าพลังชีวิตหมด เรียกฟังก์ชัน Die()
        }
    }

    void Die()
    {
        isDead = true; // ตั้งสถานะว่าผู้เล่นตายแล้ว

        Debug.Log("Player Died!"); // แสดงข้อความใน Console ว่าผู้เล่นตายแล้ว

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // แสดงหน้าจอ Game Over
        }

        Cursor.lockState = CursorLockMode.None; // ปลดล็อคเมาส์
        Cursor.visible = true; // แสดงเมาส์

        Camera cam = GetComponentInChildren<Camera>(); // ค้นหา Component กล้องที่เป็นลูกของ Player
        if (cam != null)
        {
            cam.transform.parent = null; // ถอดกล้องออกจาก Player เพื่อไม่ให้ถูกทำลายตาม
        }

        Destroy(gameObject); // ลบ GameObject ผู้เล่นออกจากฉาก
    }

    public void AddScore(int score)
    {
        kill += score; // เพิ่มจำนวนศัตรูที่ฆ่าได้
        killUI.text = "Kill: " + kill; // อัปเดตข้อความใน UI
    }

    void OnGUI()
    {
        // แสดงพลังชีวิตผู้เล่นที่มุมบนซ้ายของหน้าจอ
        GUI.Label(new Rect(10, 50, 200, 20), $"Player HP: {currentHealth} / {maxHealth}");
    }
}
