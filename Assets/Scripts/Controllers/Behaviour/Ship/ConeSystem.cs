using UnityEngine;

public class ConeSystem : MonoBehaviour
{
    
    [SerializeField]
    private Vector3[] frontWorldDir,leftWorldDir,rightWorldDir;
    [SerializeField]
    private Vector2 dispersionClamp;
    public Vector2 setDispersionClamp { set { dispersionClamp = value; } }

    public RaycastHit[] hits;
    RaycastHit hit;

    //settable vals
    Transform conePivotForward, conePivotLeft, conePivotRight;
    bool isPacifist;
    private int maxRaycasts = 25;
    private Vector3[] frontRandomDir, leftRandomDir, rightRandomDir;
    private float hitScanMaxRange, hitScanShootingRange;

    //refs
    AIPlayerController AIcont;
    GameMaster gm;

    //timers
    private float timeSinceLastCalled;
    private float delay;

    void Start()
    {
        frontRandomDir = new Vector3[maxRaycasts];
        leftRandomDir = new Vector3[maxRaycasts];
        rightRandomDir = new Vector3[maxRaycasts];
        frontWorldDir = new Vector3[maxRaycasts];
        leftWorldDir = new Vector3[maxRaycasts];
        rightWorldDir = new Vector3[maxRaycasts];
        hits = new RaycastHit[maxRaycasts];
        //randomize delay so not all raycast will be updated at once
        delay = UnityEngine.Random.Range(0.1f, 0.3f);
        AIcont = GetComponent<AIPlayerController>();
        gm = GameMaster.instance;
        if (gm == null)
        {
            Debug.LogError("Couldn't find Game Master!");
        }
        VectorRefresh();
    }
    public void SetUpConeSystem(Transform _coneForward, Transform _coneLeft, 
    Transform _coneRight, bool _isPacifist, int _maxRaycasts, float _maxHitScanRange, float _maxHitScanShootRange)
    {
        conePivotForward = _coneForward;
        conePivotLeft = _coneLeft;
        conePivotRight = _coneRight;
        isPacifist = _isPacifist;
        maxRaycasts = _maxRaycasts;
        hitScanMaxRange = _maxHitScanRange;
        hitScanShootingRange = _maxHitScanShootRange;
    }
    // Update is called once per frame
    void Update()
    {
        DebugVectors();     
        InverseToWorld();
        ConeUpdate();
    }
    void ConeUpdate()
    {
        timeSinceLastCalled += TimeControl.deltaTime;
        if (timeSinceLastCalled > delay)
        {
            if(conePivotForward!= null)
                HitScan();
            else{
                Debug.LogError("Dont have main pivot, bail");
            }
            timeSinceLastCalled = 0f;
        }
    }
    void DebugVectors()
    {
#if UNITY_EDITOR
        for (int i = 0; i < maxRaycasts; i++)
        {
            Debug.DrawRay(conePivotForward.position, frontWorldDir[i] * 7f, Color.yellow);
            Debug.DrawRay(conePivotLeft.position, leftWorldDir[i] * 7f, Color.yellow);
            Debug.DrawRay(conePivotRight.position, rightWorldDir[i] * 7f, Color.yellow);
        }
#endif
    }
    void VectorRefresh()
    {
        for (int i = 0; i < maxRaycasts; i++)
        {
            float x = UnityEngine.Random.Range(-dispersionClamp.x,  dispersionClamp.x);
            float y = UnityEngine.Random.Range(-dispersionClamp.y,  dispersionClamp.y);
            frontRandomDir[i] = new Vector3(x,y,0);
            rightRandomDir[i] = new Vector3(x,y,0);
            leftRandomDir[i] = new Vector3(x,y,0);
        }
    }
    void InverseToWorld()
    {
        for (int i = 0; i < maxRaycasts; i++)
        {
            frontWorldDir[i] = Quaternion.Euler(frontRandomDir[i]) * conePivotForward.forward;
            leftWorldDir[i] = Quaternion.Euler(leftRandomDir[i]) * conePivotLeft.forward;
            rightWorldDir[i] = Quaternion.Euler(rightRandomDir[i]) * conePivotRight.forward;
        }      
    }
    void HitScan()
    {
        /// <summary>
        /// Raycast based hitscan. 
        /// </summary>
        for (int i = 0; i < maxRaycasts; i++)
        {
            if (Physics.Raycast(conePivotForward.position, frontWorldDir[i], out hit, hitScanMaxRange))
            {
                //avoidance setup
                if (IsTargetaShip(hit.collider.tag))
                {
                    //if target is within range, first thing we do is to avoid contact
                    if (hit.distance < 7f) //hardcoded val for now
                    {
                    #if UNITY_EDITOR
                        Debug.DrawRay(conePivotForward.position, frontWorldDir[i] * hit.distance, Color.blue);
                    #endif
                        AIcont.ChangeAvoidanceStatus();
                    }
                }
                if (CanShootTarget(hit)&& !isPacifist)
                {
                    //Debug attack detection
                    if (hit.distance < hitScanShootingRange)
                    {
                    #if UNITY_EDITOR
                        Debug.DrawRay(conePivotForward.position, frontWorldDir[i] * hit.distance, Color.red);
                    #endif
                        AIcont.ShootForward();
                    }
                }
            }
            if (Physics.Raycast(conePivotLeft.position, leftWorldDir[i], out hit, hitScanMaxRange))
            {
                if (CanShootTarget(hit)&&!isPacifist)
                {
                    if (hit.distance < hitScanShootingRange)
                    {
                    #if UNITY_EDITOR
                        Debug.DrawRay(conePivotLeft.position, leftWorldDir[i] * hit.distance, Color.red);
                    #endif
                        AIcont.ShootToSide(0);
                    }
                }
            }
            if(Physics.Raycast(conePivotRight.position, rightWorldDir[i], out hit, hitScanMaxRange))
            {
                if (CanShootTarget(hit)&&!isPacifist)
                {
                    if (hit.distance < hitScanShootingRange)
                    {
                    #if UNITY_EDITOR
                        Debug.DrawRay(conePivotRight.position, rightWorldDir[i] * hit.distance, Color.red);
                    #endif
                        AIcont.ShootToSide(1);

                    }
                }
            }
        }
    }
    bool CanShootTarget(RaycastHit _hit)
    {
        bool ret = false;
        if (IsTargetaShip(_hit.collider.tag))
        {
            int hitID = 0;
            if (int.TryParse(_hit.collider.attachedRigidbody.gameObject.name, out hitID))
            {
                //we succeded parsing name from GO, so our hitscan detected AI.
                if(gm.GetAIGroup(hitID)!= AIcont.AIGroup && gm.GetAIStatus(hitID) == AIShipStatus.WORKING)
                {
                    ret = true;
                }
            }
            else
            {
                //we have detected player
                ret = true;
            }
        }
        return ret;
    }
    bool IsTargetaShip(string _tag)
    {
        bool ret = false;
        switch (_tag)
        {
            case ("Enemy"):
                ret = true;
                break;
            case ("Player0"):
                ret = true;
                break;
            default:
                break;
        }
        return ret;
    }
}
