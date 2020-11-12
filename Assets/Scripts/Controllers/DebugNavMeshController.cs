using UnityEngine;
using UnityEngine.AI;
public class DebugNavMeshController : MonoBehaviour
{
    NavMeshAgent agent;
    public Transform target;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void DebugNavMesh()
    {
        agent.SetDestination(target.transform.position);
    }
}


