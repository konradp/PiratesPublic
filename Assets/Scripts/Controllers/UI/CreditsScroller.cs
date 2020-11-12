using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] RectTransform _trans;
    Vector2 rect, defRect;
    [SerializeField] float scrollTime = 20f;
    [SerializeField] float maxScroll = 3500f;
    bool canScroll;

    private void OnEnable() 
    {
        rect = _trans.sizeDelta;
        defRect = rect;
        canScroll=true;
    }
    private void Update() 
    {
        if(canScroll)
        {
            rect.y += TimeControl.deltaTime * scrollTime;
            _trans.sizeDelta = new Vector2(rect.x, rect.y);
        }
        if(rect.y>maxScroll)
            canScroll=false;
        
    }
    private void OnDisable() 
    {
        _trans.sizeDelta = defRect;
    }
}
