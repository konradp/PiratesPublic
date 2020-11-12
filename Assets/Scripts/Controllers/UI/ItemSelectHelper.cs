using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSelectHelper : MonoBehaviour, ISelectHandler
{
    [SerializeField] GameObject itemScrollbar;
    [SerializeField] Transform parentContent;

    public int index;
    float totalItems;
    private void Start()
    {
        totalItems = parentContent.childCount;
    }
    public void SetIndex(int _index)
    {
        index = _index;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (index != 1)
            itemScrollbar.GetComponent<Scrollbar>().value = 1.0f - (index / totalItems);
        else
        {
            itemScrollbar.GetComponent<Scrollbar>().value = 1.0f;
        }
    }
}
