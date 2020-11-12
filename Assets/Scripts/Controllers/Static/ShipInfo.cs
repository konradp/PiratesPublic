using UnityEngine;

public class ShipInfo : MonoBehaviour
{
    [SerializeField] WheelCollider[] wCols;
    [SerializeField] Transform centreOfMass;
    [SerializeField] Transform forwardCannonPiv;
    [SerializeField] Transform[] rightCannonPivs, leftCannonPivs;
    [SerializeField] Transform conePivotForward, conePivotLeft, conePivotRight;
    [SerializeField] RectTransform healthBarRect;
    [SerializeField] GameObject healthBarObj;

    //getters
    public WheelCollider[] GetwCols { get { return wCols; } }
    public Transform GetcentreOfMass { get { return centreOfMass; } }
    public Transform GetforwardCannonPiv { get { return forwardCannonPiv; } }
    public Transform[] GetrightCannonPiv { get { return rightCannonPivs; } }
    public Transform[] GetleftCannonPivs { get { return leftCannonPivs; } }
    public Transform GetconePivotForward { get { return conePivotForward; } }
    public Transform GetconePivotLeft { get { return conePivotLeft; } }
    public Transform GetconePivotRight { get { return conePivotRight; } }
    public RectTransform GetHealthBarRect { get { return healthBarRect; } }
    public GameObject GetHealthObj {  get { return healthBarObj; } }
}
