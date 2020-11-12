using UnityEngine;
using UnityEngine.AI;

public static class PathGetter
{
    public static Vector3 GetRandomPath(Vector3 initPos, int areaLayer, Vector2 xConstraints, Vector2 zConstraints)
    {
        Vector3 ret = Vector3.zero;
        float x, z;
        float y = -0.7f;
        NavMeshPath path = new NavMeshPath();
        //Potential infinite loop constraint
        int iterations = 0;
        int maxIterations = 512;
        do
        {
            x = UnityEngine.Random.Range(xConstraints.x, xConstraints.y);
            z = UnityEngine.Random.Range(zConstraints.x, zConstraints.y);
            ret = new Vector3(x, y, z);
            
            NavMesh.CalculatePath(initPos, ret, areaLayer, path);
            iterations++;
            if (iterations > maxIterations)
            {
                //possible infinite loop, bail
                Debug.LogError("Infinite loop");
                break;
            }
            
        } while (path.status == NavMeshPathStatus.PathInvalid || path.status == NavMeshPathStatus.PathPartial);
        return ret;
    }
}
