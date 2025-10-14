using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("ID→Prefab 매핑")]
    public NPCRegistry registry;

    [Header("스폰 위치들(필요 시 다수)")]
    public Transform[] spawnPoints;

    public GameObject SpawnByID(string npcID, int spawnPointIndex = 0)
    {
        if (registry == null)
        {
            Debug.LogError("[NPCSpawner] Registry not assigned");
            return null;
        }

        var prefab = registry.GetPrefab(npcID);
        if (prefab == null)
        {
            Debug.LogError($"[NPCSpawner] Prefab not found for id={npcID}");
            return null;
        }

        var point = (spawnPoints != null && spawnPointIndex < spawnPoints.Length)
            ? spawnPoints[spawnPointIndex] : transform;

        return Instantiate(prefab, point.position, point.rotation);
    }
}
