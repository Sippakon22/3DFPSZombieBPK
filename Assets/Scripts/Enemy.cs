using TMPro; // ใช้สำหรับระบบ UI ที่ใช้ TextMeshPro
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")] // กลุ่มตัวแปรเกี่ยวกับการเคลื่อนที่ของศัตรู
    public float minSpeed = 1.5f; // ความเร็วขั้นต่ำของศัตรู
    public float maxSpeed = 3f;   // ความเร็วสูงสุดของศัตรู
    private float moveSpeed;      // ความเร็วที่สุ่มได้จริง ๆ สำหรับศัตรูตัวนี้

    private Transform player;     // อ้างอิงตำแหน่งของผู้เล่น

    [Header("Health")] // กลุ่มตัวแปรเกี่ยวกับพลังชีวิตของศัตรู
    public int maxHP = 100;       // พลังชีวิตสูงสุดของศัตรู
    private int currentHP;        // พลังชีวิตปัจจุบันของศัตรู

    [Header("Attack")] // กลุ่มตัวแปรเกี่ยวกับการโจมตีของศัตรู
    public float attackRange = 1.5f; // ระยะโจมตีของศัตรู
    public int damage = 10;          // ดาเมจที่ศัตรูทำได้เมื่อโจมตี
    public float hitCooldown = 1f;   // คูลดาวน์ระหว่างการโจมตีแต่ละครั้ง
    private float lastHitTime;       // เวลาที่ศัตรูโจมตีครั้งล่าสุด

    private PlayerManager playerManager; // อ้างอิงไปยัง PlayerManager เพื่อให้ศัตรูสามารถทำดาเมจผู้เล่นได้
    private Renderer rend;               // ใช้ในการเปลี่ยนสีของวัสดุของศัตรู

    void Start()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed); // กำหนดความเร็วของศัตรูแบบสุ่มภายในช่วงที่กำหนด

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); // ค้นหา GameObject ที่มีแท็กว่า Player
        if (playerObj)
        {
            player = playerObj.transform;                         // กำหนดตำแหน่งของผู้เล่น
            playerManager = playerObj.GetComponent<PlayerManager>(); // อ้างอิงสคริปต์ PlayerManager จากผู้เล่น
        }

        currentHP = maxHP; // ตั้งค่าพลังชีวิตเริ่มต้นของศัตรู

        // ตรวจสอบว่ามี CapsuleCollider หรือยัง ถ้ายังไม่มี ให้เพิ่มเข้าไป
        if (GetComponent<CapsuleCollider>() == null)
        {
            CapsuleCollider capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.height = 2f;
            capsule.radius = 0.5f;
            capsule.center = new Vector3(0, 1, 0);
        }

        rend = GetComponent<Renderer>(); // อ้างอิง Renderer ของศัตรู
        if (rend != null)
        {
            // สุ่มสีของศัตรูให้ไม่ซ้ำกัน
            rend.material.color = new Color(Random.value, Random.value, Random.value);
        }
    }

    void Update()
    {
        if (player != null)
        {
            MoveTowardsPlayer(); // เคลื่อนที่เข้าหาผู้เล่น
            TryAttack();         // พยายามโจมตีผู้เล่นหากอยู่ในระยะ
        }
    }

    void MoveTowardsPlayer()
    {
        // คำนวณทิศทางการเคลื่อนที่เข้าหาผู้เล่น
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime; // เคลื่อนที่
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z)); // หันหน้าหาผู้เล่น
    }

    void TryAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position); // คำนวณระยะห่างกับผู้เล่น
        if (distance <= attackRange && Time.time >= lastHitTime + hitCooldown)
        {
            lastHitTime = Time.time; // อัปเดตเวลาที่โจมตีล่าสุด
            if (playerManager != null)
            {
                playerManager.TakeDamage(damage); // ทำดาเมจให้กับผู้เล่น
                Debug.Log("Enemy hit player for " + damage + " damage"); // แสดงผลใน Console
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount; // ลดพลังชีวิตของศัตรู

        if (currentHP <= 0)
        {
            Die(); // ถ้าพลังชีวิตหมด เรียกฟังก์ชัน Die()
        }
    }

    void Die()
    {
        playerManager.AddScore(1); // เพิ่มคะแนนให้กับผู้เล่นเมื่อฆ่าศัตรูได้ 1 ตัว
        Destroy(gameObject);       // ทำลาย GameObject ศัตรู
    }
}
