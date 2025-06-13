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
