using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public GameObject PlayerToFollow;
    Transform target;
    Vector3 pos;
    private void Start()
    {
        target = PlayerToFollow.transform;
        pos = transform.position;
    }
    private void LateUpdate()
    {
        pos = new Vector3(target.position.x, pos.y,target.position.z);
        transform.position = pos;
    }
}
