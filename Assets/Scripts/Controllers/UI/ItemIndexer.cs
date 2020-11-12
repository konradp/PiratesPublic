using UnityEngine;

public class ItemIndexer : MonoBehaviour
{
    private void Start() 
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ItemSelectHelper>().SetIndex(i+1);
        }
    }

}
