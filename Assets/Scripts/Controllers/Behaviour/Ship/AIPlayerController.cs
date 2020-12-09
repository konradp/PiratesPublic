using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIPlayerController : MonoBehaviour
{
    //serializable vars
    [SerializeField] GameObject[] shipTemplates;
    [SerializeField] GameObject NodeChecker;
    [SerializeField] AnimationCurve SpeedToSteerAdjust;

    //failsafe timers
    float failSafePathTimer = 0;
    float maxFailSafetimer = 20f;
    //speed timer
    float speedTimer = 0;
    float maxTimer = 5;
    //avoid vars
    bool shouldAvoid = false;
    float avoidTimer = 0;
    float maxAvoidTimer = 3;
    float standingStillTimer = 0;
    float maxStandingStillTimer = 5;
    bool overrideAvoid;
    float overrideAvoidTimer = 0;
    float overrideAvoidMaxtimer = 5f;
    //initial shooting timers
    float initialDelayTimer = 0;
    float initialDelayTimerMax = 10f;
    bool hasInitTimerExpired = false;

    //Controler variables
    /// <summary>
    /// Colliders for wheels. Default order - FL,FR,RL,RR
    /// </summary>
    WheelCollider[] wCols;
    Transform centreOfMass;
    Transform forwardCanonPiv;
    Transform[] rightCanonsPiv, leftCanonsPiv;
    List<Vector3> nodes = new List<Vector3>();
    Vector3 relativeVector = Vector3.zero;
    Transform AI_PathTrans;
    Vector2 areaXconstraints, areaZconstraints;

    //behaviour and AI vars
    float maxSteerAngle; //def 30f
    float maxSpeed; //def 20f
    float torque; //def 300f
    float tempTorque = 0f;
    float curSpeed = 0;
    public int curPlayerID;
    int curNode = 0;
    int curShipTemplate;
    public int AIGroup;
    int allowedAreaMask;
    bool canMoveForward = true;
    bool isImmobilized;
    Transform failSafeTransform;

    //controllers refs
    HealthController healthCont;
    AttackController playAtackCont;
    Rigidbody rb;
    GameMaster gm;
    GameObject healthBarObj;

    /// <summary>
    /// Constructor called from GameMaster to create AI ship 
    /// </summary>
    /// <param name="_aiPathTrans">Transform of AI Nodes parent object</param>
    /// <param name="_id">ID of the ship</param>
    /// <param name="_areaXConst">Constraint of Path Getter in X axis</param>
    /// <param name="_areaZConst">Constraint of Path Getter in Z axis</param>
    /// <param name="_aiGroup">AI group. default 0. Various group can fight with each other</param>
    /// <param name="_shipTemplate">Ship template index.</param>
    /// <param name="_isPacifist">Is ship pacifist?</param>
    /// <param name="_failSafeTrans">Transform of the Failsafe Position</param>
    public void AIPlayerSetup(Transform _aiPathTrans, int _id, Vector2 _areaXConst,
    Vector2 _areaZConst, int _aiGroup, int _shipTemplate, bool _isPacifist, Transform _failSafeTrans)
    {
        curShipTemplate = _shipTemplate;
        GameObject go = GameObject.Instantiate(shipTemplates[curShipTemplate], gameObject.transform);
        ShipInfo info = go.GetComponent<ShipInfo>();
        wCols = info.GetwCols;
        centreOfMass = info.GetcentreOfMass;
        forwardCanonPiv = info.GetforwardCannonPiv;
        leftCanonsPiv = info.GetleftCannonPivs;
        rightCanonsPiv = info.GetrightCannonPiv;
        healthBarObj = info.GetHealthObj;
        ShipData data = PresetShipData.GetPresetFromIndex(curShipTemplate);
        GetComponent<ConeSystem>().SetUpConeSystem(info.GetconePivotForward,
            info.GetconePivotLeft, info.GetconePivotRight, _isPacifist, 
            data.GetMaxRaycasts,data.GetMaxHitScanRange,data.GetMaxHitScanShootRange);
        maxSpeed = data.GetMaxSpeed;
        maxSteerAngle = data.GetMaxSteerAngle;
        torque = data.GetMaxTorque;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.localPosition;
        AI_PathTrans = _aiPathTrans;
        curPlayerID = _id;
        areaXconstraints = _areaXConst;
        areaZconstraints = _areaZConst;
        AIGroup = _aiGroup;
        playAtackCont = GetComponent<AttackController>();
        playAtackCont.SetUpAttackController(forwardCanonPiv, leftCanonsPiv, rightCanonsPiv, data.GetAmmo, false);
        healthCont = GetComponent<HealthController>();
        healthCont.SetUpHealthController(data.GetHp, true, curPlayerID, info.GetHealthBarRect);
        failSafeTransform = _failSafeTrans;

    }
    private void Start()
    {
        gm = GameMaster.instance;
        if (gm == null)
        {
            Debug.LogError("Couldnt find GM!!!!");
        }

        if (AI_PathTrans == null)
        {
            AI_PathTrans = GameObject.FindGameObjectWithTag("AI_path_parent").transform;
        }
        StartCoroutine(WaitForInitalPath(3f));
        //determine our current mask
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position, out hit, 3f, NavMesh.AllAreas);
        allowedAreaMask = hit.mask;

    }
    private void Update()
    {
        curSpeed = rb.velocity.magnitude * 3.6f;
        //limit speed if we're too fast
        if (curSpeed > maxSpeed)
        {
            for (int i = 0; i < wCols.Length; i++)
            {
                wCols[i].motorTorque = 0f;
            }
        }
        if (overrideAvoid)
        {
            overrideAvoidTimer += TimeControl.deltaTime;
            if (overrideAvoidTimer > overrideAvoidMaxtimer)
            {
                overrideAvoidTimer = 0;
                overrideAvoid = false;
            }
        }
        if (!hasInitTimerExpired)
        {
            initialDelayTimer += TimeControl.deltaTime;
            if (initialDelayTimer > initialDelayTimerMax)
                hasInitTimerExpired = true;
        }
    }
    IEnumerator WaitForInitalPath(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(GetPath(Vector3.zero));
    }
    //path getting method. If we feed Vector3.Zero, we'll get random pos from PathGetter static script
    IEnumerator GetPath(Vector3 desPos)
    {
        nodes.Clear();
        curNode = 0;
        NavMeshPath path = new NavMeshPath();
        if (desPos == Vector3.zero)
            NavMesh.CalculatePath(transform.position, PathGetter.GetRandomPath(transform.position, allowedAreaMask, areaXconstraints, areaZconstraints), allowedAreaMask, path);
        else
        {
            NavMesh.CalculatePath(transform.position, desPos, allowedAreaMask, path);
            for (int i = 0; i < AI_PathTrans.childCount; i++)
            {
                Destroy(AI_PathTrans.GetChild(i).gameObject);
            }
        }
        yield return new WaitForSeconds(0.5f);
        //if path is too small or is basically non existent it won't have corners
        if (path.corners.Length == 0)
        {
            Debug.LogError(string.Format("Impossible to reach destination. Possibly AI {0} out of bounds of navmesh.", gameObject.name + "," + shipTemplates[curShipTemplate].name));
        }
        else
        {
            for (int i = 0; i < path.corners.Length; i++)
            {
                nodes.Add(path.corners[i]);
                GameObject go = Instantiate(NodeChecker, path.corners[i], Quaternion.identity, AI_PathTrans);
                go.name = string.Format("{0}_{1}",curPlayerID,(i).ToString());
            }
            canMoveForward = true;
            failSafePathTimer = 0;
        }
    }
    IEnumerator GetFailSafePath(Vector3 _despos)
    {
        nodes.Clear();
        curNode = 0;
        yield return new WaitForSeconds(0.5f);
        nodes.Add(_despos);
        GameObject go = Instantiate(NodeChecker, _despos, Quaternion.identity, AI_PathTrans);
        go.name = string.Format("{0}_{1}", curPlayerID, 0.ToString());
        canMoveForward = true;
        failSafePathTimer = 0;

    }
    private void FixedUpdate()
    {
        if (nodes.Count > 0 && !isImmobilized)
        {
            if (shouldAvoid)
            {
                //avoidance timers. If we're standing still for too long, we need to tell the player to move 
                //anyway after certain ammount of time
                standingStillTimer += TimeControl.fixedDeltaTime;
                avoidTimer += TimeControl.fixedDeltaTime;
                if (avoidTimer > maxAvoidTimer)
                {
                    avoidTimer = 0;
                    shouldAvoid = false;
                }
                else
                {
                    ApplyBrakes();
                }
                if (standingStillTimer > maxStandingStillTimer)
                {
                    standingStillTimer = 0;
                    overrideAvoid = true;
                }
            }
            else
            {
                ApplySteer();
                ApplySpeed();
            }
        }
        else
        {
            //we're waiting for new node, so we need to stop
            ApplyBrakes();
        }
        //failsafe checker. If we're standing still for too long, try to create new path 
        if (canMoveForward && nodes.Count < 1)
        {
            failSafePathTimer += TimeControl.fixedDeltaTime;
            if (failSafePathTimer > maxFailSafetimer && !isImmobilized)
            {
                failSafePathTimer = 0;
                //create failsafe path
                Debug.LogWarning(string.Format("We can't move forward, refering to failsafe trans at {0} ; {1} ; {2}",
                    failSafeTransform.position.x, failSafeTransform.position.y, failSafeTransform.position.z));
                StartCoroutine(GetFailSafePath(failSafeTransform.position));
            }

        }
    }
    void ApplySteer()
    {
        if (curNode <= nodes.Count && canMoveForward)
        {
            if (curNode < nodes.Count)
            {
                relativeVector = transform.InverseTransformPoint(nodes[curNode]);
                float speedSteerAdjust = SpeedToSteerAdjust.Evaluate(Mathf.Clamp(Mathf.Abs(curSpeed / maxSpeed), 0.0f, 1.0f));
                float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle * speedSteerAdjust;
                for (int i = 0; i < 2; i++)
                {
                    wCols[i].steerAngle = newSteer / 2;
                }
                for (int i = 2; i < 4; i++)
                {
                    wCols[i].steerAngle = -newSteer;
                }
            }
            else
            {
                //we can't move. should reset path 
                StartCoroutine(GetPath(Vector3.zero));
            }
        }
    }
    void ApplySpeed()
    {
        speedTimer += TimeControl.deltaTime;
        if (curNode <= nodes.Count && canMoveForward)
        {
            if (speedTimer > maxTimer)
            {
                tempTorque = torque;
                speedTimer = 0;
                for (int i = 0; i < 4; i++)
                {
                    wCols[i].motorTorque = torque;
                    wCols[i].brakeTorque = 0;
                }
            }
            else
            {
                tempTorque = Mathf.Lerp(tempTorque, 0, TimeControl.fixedDeltaTime);
                for (int i = 0; i < 4; i++)
                {
                    wCols[i].motorTorque = tempTorque;
                    wCols[i].brakeTorque = 0;
                }
            }
        }
    }
    void ApplyBrakes()
    {
        for (int i = 0; i < 4; i++)
        {
            wCols[i].motorTorque = 0f;
            wCols[i].brakeTorque = 200f;
        }
    }
    public void ChangeAvoidanceStatus()
    {
        if (!shouldAvoid && avoidTimer == 0 && !overrideAvoid)
        {
            shouldAvoid = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NodeChecker"))
        {
            string tmp = string.Format("{0}_{1}",curPlayerID,curNode.ToString());
            if (tmp == other.gameObject.name)
            {
                curNode++;
                Destroy(other.gameObject);
            }
        }
    }
    public void ShootForward()
    {
        if (playAtackCont.CanShoot() && hasInitTimerExpired && !isImmobilized)
        {
            playAtackCont.ShootForward(curPlayerID);
        }
    }
    /// <summary>
    /// Side Shoot
    /// </summary>
    /// <param name="_direction">0 for left, 1 for right, else for fuck my day up</param>
    public void ShootToSide(int _direction)
    {
        if (playAtackCont.CanShoot() && hasInitTimerExpired && !isImmobilized)
        {
            playAtackCont.ShootToSide(curPlayerID, _direction);
        }
    }
    public void ImmobilizePlayer()
    {
        if (!isImmobilized)
        {
            isImmobilized = true;
            gm.SetAIStatus(AIShipStatus.IMMOBILIZED, curPlayerID);
            wCols[0].gameObject.SetActive(false);
            wCols[2].gameObject.SetActive(false);
            rb.mass = 0.5f;
            rb.drag=3f;
            rb.angularDrag=2f;
            healthBarObj.SetActive(false);
        }
    }
    public void SlowDownAI()
    {
        //fancy particle goes here
        //TODO: because of complexicity of ai ship spawner I need to do this further in development
        //set speed down to 2/3
        maxSpeed = (2f / 3f) * maxSpeed;
        torque = (2f / 3f) * torque;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (isImmobilized && other.collider.CompareTag("Terrain"))
        {
            IgnoreCollisionOnDeath(other.collider);
        }
    }
    void IgnoreCollisionOnDeath(Collider col)
    {
        MeshCollider meshcol = transform.GetComponentInChildren<MeshCollider>();
        Physics.IgnoreCollision(meshcol, col, true);
        for (int i = 0; i < wCols.Length; i++)
        {
            Physics.IgnoreCollision(wCols[i],col,true);
        }
    }
}
