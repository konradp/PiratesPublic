using UnityEngine;

public class SpawnController : MonoBehaviour
{
    //TODO: Make proper getters and setters
    public int spawnIndex;
    public int AIGroup;
    /// <summary>
    /// 0 represents base AI model, 1 for boss, 2-8 reserved for future ships
    /// 9 represents npc pacifist fishing ship
    /// </summary>
    public int shipIndex;
    public bool isPacifist;
    public Transform failSafeTransform;
}
