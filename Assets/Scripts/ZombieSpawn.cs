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
