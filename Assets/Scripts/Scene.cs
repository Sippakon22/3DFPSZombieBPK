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
