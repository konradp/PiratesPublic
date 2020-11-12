using UnityEngine;

public class WaveCreator : MonoBehaviour
{
    [SerializeField] float perlinScale = 4.56f;
    [SerializeField] float waveSpeed = 1f;
    [SerializeField] float waveHeight = 2f;
    [SerializeField] GameObject MeshObj;

    Mesh mesh;
    MeshCollider meshCol;
    private void Start()
    {
        mesh = MeshObj.GetComponent<MeshFilter>().mesh;
        meshCol = MeshObj.GetComponent<MeshCollider>();
    }

    void Update()
    {
        AnimateMesh();
        UpdateMeshCollider();
    }

    void AnimateMesh()
    {
        if (!mesh)
        {
            Debug.LogError("Can't find mesh");
            return;
        }

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float pX = (vertices[i].x * perlinScale) + (TimeControl.timeSinceLevelLoad * waveSpeed);
            float pZ = (vertices[i].z * perlinScale) + (TimeControl.timeSinceLevelLoad * waveSpeed);

            vertices[i].y = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * waveHeight;
        }

        mesh.vertices = vertices;
    }
    void UpdateMeshCollider()
    {
        if(meshCol!=null)
            meshCol.sharedMesh = mesh;
    }
}
