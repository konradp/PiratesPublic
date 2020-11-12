using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class AutoPlacer : MonoBehaviour
{
    [SerializeField] GameObject[] helpObjs;
    [SerializeField] Terrain ter;
    [SerializeField] float randomDeviation = 1f;
    [SerializeField] float maxHeight = 99f;
    public bool isManuallActivated;

    Vector2[] xBoundaries, zBoundaries;
    public int maxSpace = 5;
    public int minSpace = 2;
    Transform[] desPlace;
    List<float>[] xPositions, zPositions;
    List<int> objOverBoundsIndexes;
    int maxObjects;
    public int GetMaxObjects { get { return maxObjects; } }
    GameObject[] objsCache;
    PropsPlacer placer;
    private void Start()
    {
        if (!isManuallActivated)
        {
            //convert helpers to world boundaries
            ConvertHelpersToBoundaries();
            //make random x and z positions
            RandomXZPosGen();
            //create GameObjects and assign Correct Transforms and Positions
            CreateTransforms();
            //feed data to prop placer and do magic
            FeedPlacer();
        }
    }
    
    public void ConvertHelpersToBoundaries()
    {
        xBoundaries = new Vector2[helpObjs.Length];
        zBoundaries = new Vector2[helpObjs.Length];
        for (int i = 0; i < helpObjs.Length; i++)
        {
            Bounds bb = helpObjs[i].GetComponent<MeshCollider>().bounds;
            xBoundaries[i].x = bb.min.x;
            xBoundaries[i].y = bb.max.x;
            zBoundaries[i].x = bb.min.z;
            zBoundaries[i].y = bb.max.z;
        }
    }

    public void RandomXZPosGen()
    {
        xPositions = new List<float>[xBoundaries.Length];
        zPositions = new List<float>[xBoundaries.Length];
        System.Random rand = new System.Random();
        for (int j = 0; j < xBoundaries.Length; j++)
        {
            xPositions[j] = new List<float>();
            zPositions[j] = new List<float>();
            for (float i = xBoundaries[j].x; i < xBoundaries[j].y; i += rand.Next(minSpace, maxSpace))
            {
                xPositions[j].Add(i);
            }
            for (float i = zBoundaries[j].x; i < zBoundaries[j].y; i += rand.Next(minSpace, maxSpace))
            {
                zPositions[j].Add(i);
            }
        }
    }
    public void CreateTransforms()
    {
        int count = 0;
        for (int i = 0; i < xBoundaries.Length; i++)
        {
            count += xPositions[i].Count * zPositions[i].Count;
        }
        maxObjects = count;
        objsCache = new GameObject[maxObjects];
        desPlace = new Transform[maxObjects];
        for (int i = 0; i < maxObjects; i++)
        {
            objsCache[i] = new GameObject("Trans" + i);
            objsCache[i].transform.SetParent(this.transform);
            desPlace[i] = objsCache[i].GetComponent<Transform>();
            
        }
        objOverBoundsIndexes = new List<int>();
        int index = 0;
        for (int k = 0; k < xBoundaries.Length; k++)
        {
            for (int i = 0; i < xPositions[k].Count; i++)
            {
                for (int j = 0; j < zPositions[k].Count; j++)
                {
                    //make minimal random offset to make it look more natural
                    xPositions[k][i] += UnityEngine.Random.Range(-randomDeviation, randomDeviation);
                    zPositions[k][j] += UnityEngine.Random.Range(-randomDeviation, randomDeviation);
                    desPlace[index].position = new Vector3(xPositions[k][i], 0f, zPositions[k][j]);
                    //fix y pos according to terrain
                    //for some bizzare reason this offsets y pos by 5 so we need to fix it with hardcode
                    float y = ter.SampleHeight(desPlace[index].position) - 5f;
                    if (y > maxHeight)
                        objOverBoundsIndexes.Add(index);
                    desPlace[index].position = new Vector3(desPlace[index].position.x, y, desPlace[index].position.z);
                    index++;
                }
            }
        }
    }
    public void FeedPlacer()
    {
        placer = GetComponent<PropsPlacer>();
        placer.SetObjTrans = desPlace;
        placer.GetRandomScaleAndRotation();
        placer.PlaceObjects();
        //delete objects over desired height
        if (objOverBoundsIndexes.Count>1)
        {
            for (int i = 0; i < objOverBoundsIndexes.Count; i++)
            {
                GameObject.Destroy(desPlace[objOverBoundsIndexes[i]].gameObject);
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AutoPlacer))]
public class AutoPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AutoPlacer deb = (AutoPlacer)target;
        if (GUILayout.Button("Auto Place Objects"))
        {
            deb.ConvertHelpersToBoundaries();
            //make random x and z positions
            deb.RandomXZPosGen();
            //create GameObjects and assign Correct Transforms and Positions
            deb.CreateTransforms();

            if (EditorUtility.DisplayDialog("Place Objects?",
            "Are you sure you want to place " + deb.GetMaxObjects +
            " objects in scene?", "Yes", "No"))
            {
                //feed data to prop placer and do magic
                deb.FeedPlacer();
            }
        }
    }
}
#endif

