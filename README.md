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


# Gun.cs
สคริปต์ที่ทำให้ผู้เล่นยิงปืนได้

```csharp
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")] // กลุ่มตัวแปรการตั้งค่าปืน
    public float range = 100f;       // ระยะยิงของปืน
    public int damage = 25;          // ดาเมจที่ปืนทำได้ต่อการยิงหนึ่งครั้ง
    public float fireRate = 0.1f;    // ความเร็วในการยิง (ยิงได้ทุกๆ กี่วินาที)
    public int maxAmmo = 30;         // กระสุนสูงสุดในแมกกาซีน
    public float reloadTime = 1.5f;  // เวลาในการรีโหลด
    public Camera fpsCamera;         // กล้องที่ใช้สำหรับยิง (กล้องมุมมอง FPS)

    [Header("Effects")] // กลุ่มตัวแปรเกี่ยวกับเอฟเฟกต์ของปืน
    public ParticleSystem muzzleFlash; // เอฟเฟกต์ไฟแลบที่ปลายปืน

    private int currentAmmo;         // กระสุนที่เหลืออยู่ในแมกกาซีนขณะนี้
    private bool isReloading = false;// สถานะว่าอยู่ในช่วงรีโหลดหรือไม่
    private float nextTimeToFire = 0f; // เวลาที่สามารถยิงได้ครั้งถัดไป

    void Start()
    {
        currentAmmo = maxAmmo; // ตั้งค่ากระสุนให้เต็มในตอนเริ่ม
    }

    void Update()
    {
        if (isReloading) // ถ้าอยู่ในสถานะรีโหลด ให้หยุดทำอย่างอื่นก่อน
            return;

        if (currentAmmo <= 0) // ถ้ากระสุนหมด ให้เริ่มรีโหลดอัตโนมัติ
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) // ถ้าผู้เล่นกดปุ่ม R ให้เริ่มรีโหลด
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire) // ถ้าผู้เล่นกดคลิกซ้าย และถึงเวลายิงครั้งต่อไปแล้ว
        {
            nextTimeToFire = Time.time + fireRate; // ตั้งเวลาใหม่สำหรับยิงครั้งถัดไป
            Shoot(); // เรียกฟังก์ชันยิง
        }
    }

    // ฟังก์ชันสำหรับรีโหลดกระสุน
    System.Collections.IEnumerator Reload()
    {
        isReloading = true; // ตั้งสถานะว่ากำลังรีโหลด
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime); // รอเวลารีโหลดตามที่กำหนด
        currentAmmo = maxAmmo; // เติมกระสุนให้เต็ม
        isReloading = false; // รีโหลดเสร็จแล้ว
    }

    // ฟังก์ชันสำหรับการยิงกระสุน
    void Shoot()
    {
        if (muzzleFlash) muzzleFlash.Play(); // เล่นเอฟเฟกต์ไฟแลบที่ปลายปืน
        currentAmmo--; // ลดจำนวนกระสุนลง 1 นัด

        // ยิง Ray ออกจากจุดกลางของหน้าจอผ่านกล้อง
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hit, range)) // ตรวจสอบว่ามีวัตถุถูกยิงหรือไม่
        {
            Debug.Log("Hit: " + hit.collider.name); // แสดงชื่อของวัตถุที่โดนยิง

            // ตรวจสอบว่าวัตถุที่โดนคือ Enemy หรือไม่
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // ถ้าใช่ ให้ศัตรูรับดาเมจ
            }
        }
    }

    // ฟังก์ชันแสดงผล HUD ของกระสุน
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), $"Ammo: {currentAmmo} / {maxAmmo}"); // แสดงจำนวนกระสุน
        if (isReloading)
        {
            GUI.Label(new Rect(10, 30, 200, 20), "Reloading..."); // แสดงข้อความรีโหลดถ้ากำลังรีโหลดอยู่
        }
    }

}
```


# Enemy.cs
สคริปต์ AI ศัตรู ทำให้ศัตรูพุ่งมาโจมตีผู้เล่น และอื่นๆ
```csharp
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
```


# PlayerManager.cs
สคริปต์ควบคุมเลือด การตายและสถานะอื่นๆของผู้เล่น
```csharp
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
```

# ZombieSpawn.cs
สคริปต์สุ่มการเกิดซอมบี้
```csharp
using UnityEngine;
using System.Collections;

public class ZombieSpawn : MonoBehaviour
{
    [Header("Zombie Prefab")]
    // ตัว prefab ของซอมบี้ที่จะถูกสร้าง
    public GameObject zombiePrefab;

    [Header("Spawn Area")]
    // ขนาดพื้นที่สำหรับสุ่มเกิดซอมบี้ (กว้าง x สูง x ยาว)
    public Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);

    [Header("Spawn Settings")]
    // เวลาที่สุ่มรอระหว่างการเกิดซอมบี้แต่ละครั้ง (หน่วย: วินาที)
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 5f;

    // จำนวนซอมบี้ที่จะเกิดสุ่มในแต่ละครั้ง (ตั้งแต่ขั้นต่ำ ถึง ขั้นสูงสุด)
    public int minZombiesPerSpawn = 1;
    public int maxZombiesPerSpawn = 3;

    void Start()
    {
        // เริ่ม coroutine เพื่อสุ่มเกิดซอมบี้แบบต่อเนื่อง
        StartCoroutine(SpawnZombiesRandomly());
    }

    // Coroutine สำหรับสุ่มระยะเวลารอและจำนวนซอมบี้ในแต่ละรอบ
    IEnumerator SpawnZombiesRandomly()
    {
        while (true)
        {
            // สุ่มเวลารอระหว่างการเกิดซอมบี้
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            // สุ่มจำนวนซอมบี้ที่จะเกิดในรอบนี้
            int zombieCount = Random.Range(minZombiesPerSpawn, maxZombiesPerSpawn + 1);
            for (int i = 0; i < zombieCount; i++)
            {
                SpawnZombie();
            }
        }
    }

    // ฟังก์ชันสำหรับสุ่มตำแหน่งและสร้างซอมบี้
    void SpawnZombie()
    {
        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),  // สุ่มตำแหน่งในแกน X
            0,                                                      // ตำแหน่งแกน Y คงที่
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)   // สุ่มตำแหน่งในแกน Z
        );

        // สร้างซอมบี้ที่ตำแหน่งสุ่มและใช้การหมุนเริ่มต้น
        Instantiate(zombiePrefab, randomPos, Quaternion.identity);
    }

    // ฟังก์ชันสำหรับแสดงกรอบพื้นที่เกิดซอมบี้ใน Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
```


# SceneManagement.cs
สคริปต์ควบคุมการเปลี่ยนด่าน การเริ่มเกมใหม่
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    // ฟังก์ชันสำหรับเริ่มฉากใหม่ (เล่นซ้ำ)
    public void playAgain()
    {
        SceneManager.LoadScene(0); // โหลดฉากที่มี index เป็น 0 จาก Build Settings
    }
}
```
