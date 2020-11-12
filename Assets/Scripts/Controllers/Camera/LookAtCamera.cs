using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Leave empty to look at main camera")]
    private GameObject Target = null;

    private Vector3 targetPos;
    private Canvas canv;
    bool isVisible;
    float maxDistance = 50f;

    void Start()
    {
        canv = GetComponent<Canvas>();
        isVisible = true;
        if (Target == null)
        {
            Target = Camera.main.gameObject;
        }
    }

    private void LateUpdate()
    {
        if (targetPos != Target.transform.position)
        {
            targetPos = Target.transform.position;
            transform.LookAt(targetPos);
        }
        //canvas disabler, maybe make it better
        if (isVisible && Vector3.Distance(transform.position, Target.transform.position) > maxDistance)
        {
            isVisible = false;
            canv.enabled = isVisible;
        }
        if(!isVisible && Vector3.Distance(transform.position, Target.transform.position) < maxDistance)
        {
            isVisible = true;
            canv.enabled = isVisible;
        }

    }

}
