using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PropsPlacer : MonoBehaviour
{
    [SerializeField] GameObject[] objsToPlace;
    Transform[] objTrans;
    [SerializeField] Vector2 rotationRange, scaleRange;
    public bool isManualyActivated;
    public bool disableShadows;
    float[] randomRotationY;
    Vector3[] randomScale;
    List<GameObject> placedObjs;
    
    public int GetObjectCount { get { return objTrans.Length; } }
    public Transform[] SetObjTrans { set { objTrans = value; } }

    private void Start()
    {
        if (!isManualyActivated)
        {
            GetRandomScaleAndRotation();
            PlaceObjects();
            if (disableShadows)
                DisableShadows();
        }
    }

    private void DisableShadows()
    {
        foreach (GameObject go in placedObjs)
        {
            foreach (MeshRenderer rend in go.GetComponentsInChildren<MeshRenderer>())
            {
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }
    public void GetRandomScaleAndRotation()
    {
        objTrans = transform.GetComponentsInChildren<Transform>();
        randomRotationY = new float[objTrans.Length];
        randomScale = new Vector3[objTrans.Length];
        for (int i = 0; i < objTrans.Length; i++)
        {
            randomRotationY[i] = UnityEngine.Random.Range(rotationRange.x, rotationRange.y);
            randomScale[i].x = UnityEngine.Random.Range(scaleRange.x, scaleRange.y);
            randomScale[i].y = UnityEngine.Random.Range(scaleRange.x, scaleRange.y);
            randomScale[i].z = UnityEngine.Random.Range(scaleRange.x, scaleRange.y);
        }
    }
    public void PlaceObjects()
    {
        placedObjs = new List<GameObject>();
        for (int i = 0; i < objTrans.Length; i++)
        {
            if (objTrans[i].gameObject.name.Contains("Trans"))
            {
                GameObject go = GameObject.Instantiate(objsToPlace[UnityEngine.Random.Range(0, objsToPlace.Length)], objTrans[i]);
                go.transform.localScale = randomScale[i];
                go.transform.Rotate(new Vector3(0, randomRotationY[i], 0), Space.Self);
                placedObjs.Add(go);

            }
        }
    }
    public void RemovePlacedObjects()
    {
        for (int i = 0; i < placedObjs.Count; i++)
        {
            GameObject.DestroyImmediate(placedObjs[i]);
        }
        placedObjs = new List<GameObject>();
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(PropsPlacer))]
public class PropsPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PropsPlacer deb = (PropsPlacer)target;
        if (GUILayout.Button("Place Objects"))
        {
            deb.GetRandomScaleAndRotation();
            if (EditorUtility.DisplayDialog("Place Objects?",
            "Are you sure you want to place " + deb.GetObjectCount +
            " objects in scene?", "Yes", "No"))
            {
                deb.PlaceObjects();
            }
        }
        if (GUILayout.Button("Remove Objects"))
        {
            if (EditorUtility.DisplayDialog("Remove Objects?", "Are you sure you want to delete all placed objects?",
            "Yes", "No"))
            {
                deb.RemovePlacedObjects();
            }
        }

    }
}
#endif
