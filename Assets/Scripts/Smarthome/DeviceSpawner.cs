using UnityEngine;

public class DeviceSpawner : MonoBehaviour
{
    [Header("Cấu hình tạo thiết bị")]
    public GameObject wallLampPrefab; // Kéo Prefab Bóng Đèn vào đây
    public GameObject ceillingFanPrefab;   // Kéo Prefab Quạt vào đây
    public Transform spawnPoint;   // Một vị trí lơ lửng trên bàn để đồ vật rơi ra

    public void SpawnLight()
    {
        Instantiate(wallLampPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void SpawnFan()
    {
        Instantiate(ceillingFanPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}