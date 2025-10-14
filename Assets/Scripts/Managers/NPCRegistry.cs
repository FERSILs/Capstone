using UnityEngine;

[CreateAssetMenu(fileName = "NPCRegistry", menuName = "NPC/NPC Registry")]
public class NPCRegistry : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string npcID;
        public GameObject npcPrefab;
    }

    public Entry[] entries;

    public GameObject GetPrefab(string id)
    {
        if (entries == null) return null;
        foreach (var e in entries)
        {
            if (e != null && e.npcID == id) return e.npcPrefab;
        }
        return null;
    }
}
