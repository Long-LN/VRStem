using UnityEngine;

public class CupSpawner : MonoBehaviour
{
    [Header("Cau hinh tao coc")]
    [Tooltip("Keo Prefab cua cai coc tu thu muc Prefabs vao day")]
    public GameObject cupPrefab;
    
    [Tooltip("Vi tri coc se xuat hien (Tao 1 object rong de lam vi tri)")]
    public Transform spawnPoint;

    // Ham nay se duoc goi khi ban bam nut VR
    public void SpawnCup()
    {
        if (cupPrefab != null && spawnPoint != null)
        {
            // Tao ra mot cai coc moi tai vi tri va goc xoay cua spawnPoint
            Instantiate(cupPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Chua gan Prefab hoac Spawn Point cho Cup Spawner!");
        }
    }
}