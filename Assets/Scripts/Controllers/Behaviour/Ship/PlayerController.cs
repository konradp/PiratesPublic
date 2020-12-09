using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityStandardAssets.Utility;


public enum Weapon { MainCannons, PreciseCannon }
public class PlayerController : MonoBehaviour
{

    //references
    Gamepad curPad;
    Rigidbody rb;
    AttackController playAtackCont;
    HealthController healthCont;

    //player logic vals

    [SerializeField] int playerIndex;
    /// <summary>
    /// Colliders for wheels. Default order - FL,FR,RL,RR
    /// </summary>
    [SerializeField] WheelCollider[] wCols;
    /// <summary>
    /// Centre of Mass for Boat. Crucial for making a playable vehicle. In this example is set slightly lower and towards 
    /// the back of the ship
    /// </summary>
    [SerializeField] Transform centreOfMass;
    [SerializeField] float maxSteerAngle; //def 25
    [SerializeField] float maxSpeed; //def 20
    [SerializeField] float torque; //def 200
    [SerializeField] int startingAmmo; //def 150
    [SerializeField] float stillRotHelper; //def 2
    [SerializeField] float stillMaxSpeed; //def 5
    [SerializeField] float brakeTorque; //def 100
    [SerializeField] Transform forwardCanonPiv;
    [SerializeField] Transform[] rightCanonsPiv, leftCanonsPiv;
    [SerializeField] Transform cameraPiv;
    [SerializeField] AnimationCurve speedToSteerAdjust;
    [SerializeField] GameObject SmokeParticle;
    [SerializeField] Transform[] smokePartTrans;
    List<GameObject> cachedSmokeParticles;


    Weapon curWeapon;
    public Weapon GetCurWeapon { get => curWeapon;  }

    //player behaviour helper vals
    public bool isImmobilized = false;
    float desMaxSpeed, desMaxTorque;
    float curSpeed = 0;
    public float GetCurSpeed { get => curSpeed; }
    bool isStill = false;
    float turningSpeedHelper = 150f;
    bool turningWhileStill = false;

    //animations
    Animator playerAnimCont;
    //animation timer
    float animTimer = 0f;
    float zagielMaxTimer = 0.5f;

    //rot helper
    float rotAngleY, rotAngleX, defRot;

    //camera adjust vals
    CameraSmoothFollow camFollow;
    Vector2 camHeightConstraints = new Vector2(2f, 12f);
    float rotSpeed = 2f;

    public int GetAmmoCount { get { return playAtackCont.GetCurAmmo; } }

    private void Start()
    {
        //components getters
        camFollow = Camera.main.GetComponent<CameraSmoothFollow>();
        rb = GetComponent<Rigidbody>();
        playAtackCont = GetComponent<AttackController>();
        playerAnimCont = GetComponent<Animator>();
        healthCont = GetComponent<HealthController>();
        curPad = Gamepad.current;

        //Setting up controllers
        //debug health ammount, should be more fluid
        healthCont.SetUpHealthController(150, false, playerIndex, null);
        playAtackCont.SetUpAttackController(forwardCanonPiv, leftCanonsPiv, rightCanonsPiv, startingAmmo, true);

        //set local variables
        defRot = camFollow.GetHeight;
        rb.centerOfMass = centreOfMass.localPosition;
        if (curPad == null)
            Debug.LogWarning("No gamepads detected");
        curWeapon = Weapon.MainCannons;

        desMaxSpeed = maxSpeed;
        desMaxTorque = torque;
    }
    private void Update()
    {
        //CameraRotation();
        curSpeed = rb.velocity.magnitude * 3.6f;
        isStill = (curSpeed < stillMaxSpeed) ? true : false;
        DebugAnim();
    }
    void DebugAnim()
    {
        animTimer += TimeControl.deltaTime;
        if (animTimer > zagielMaxTimer)
        {
            animTimer = 0;
            if (curSpeed > 7f && curSpeed < 15f)
            {
                playerAnimCont.SetTrigger("GetBigger");
                playerAnimCont.ResetTrigger("GetSmaller");
            }
            else if (curSpeed < 5f && curSpeed > 2f)
            {
                playerAnimCont.SetTrigger("GetSmaller");
                playerAnimCont.ResetTrigger("GetBigger");
            }
        }
    }

    private void FixedUpdate()
    {
        //check if we're not exceeding speed. if so, cut the power
        if (curSpeed > maxSpeed)
        {
            for (int i = 0; i < wCols.Length; i++)
            {
                wCols[i].motorTorque = 0f;
            }
        }
    }
    public void CameraRotation(float _camXaxis, float _camYaxis)
    {
        //counter intuitive, because we're assigning Y value of campiv rotation, and we're moving right analog on x axis
        rotAngleY = _camXaxis * 90f;
        rotAngleX += _camYaxis * TimeControl.deltaTime * rotSpeed;
        rotAngleX = Mathf.Clamp(rotAngleX, camHeightConstraints.x, camHeightConstraints.y);
        if (_camYaxis == 0f)
            rotAngleX = defRot;
        camFollow.SetHeight = rotAngleX;
        cameraPiv.localEulerAngles = new Vector3(0, -rotAngleY, 0);
    }
    public void PlayerBrake(float _brakeAxis, float _steerAxis)
    {
        if (_brakeAxis > 0)
        {
            for (int i = 0; i < wCols.Length; i++)
            {
                if (isStill && _steerAxis != 0)
                {
                    wCols[i].brakeTorque = 0f;
                }
                else
                {
                    wCols[i].brakeTorque = brakeTorque * _brakeAxis;
                }
            }
        }
        else
        {
            for (int i = 0; i < wCols.Length; i++)
            {
                wCols[i].brakeTorque = 0f;
            }
        }

    }
    public void PlayerSteering(float _steerAxis)
    {
        if (_steerAxis != 0)
        {
            //speed ajdust helper
            float speedHelper = speedToSteerAdjust.Evaluate(Mathf.Clamp(Mathf.Abs(curSpeed / maxSpeed), 0.0f, 1.0f));

            //front wheels
            for (int i = 0; i < 2; i++)
            {
                if (isStill)
                {
                    turningWhileStill = true;
                    wCols[i].motorTorque = turningSpeedHelper;
                    float tmpAngle = maxSteerAngle * (_steerAxis * speedHelper) * stillRotHelper;
                    wCols[i].steerAngle = tmpAngle;
                }
                else
                {
                    turningWhileStill = false;
                    //turning with back wheels to give more authenticy, but allow front to help a bit
                    wCols[i].steerAngle = maxSteerAngle / 2 * _steerAxis * speedHelper;
                }
            }
            //back wheels
            for (int i = 2; i < 4; i++)
            {
                if (isStill)
                {
                    turningWhileStill = true;
                    wCols[i].motorTorque = turningSpeedHelper;
                    float tmpAngle = maxSteerAngle * (_steerAxis * speedHelper) * stillRotHelper;
                    wCols[i].steerAngle = -tmpAngle;
                }
                else
                {
                    turningWhileStill = false;
                    wCols[i].steerAngle = maxSteerAngle * -_steerAxis * speedHelper;
                }
            }
        }
        else
        {
            turningWhileStill = false;
            for (int i = 0; i < wCols.Length; i++)
            {
                wCols[i].steerAngle = 0;
            }
        }
    }
    public void PlayerAccel(float _accelAxis)
    {
        if (_accelAxis > 0)
        {
            for (int i = 0; i < wCols.Length; i++)
            {
                wCols[i].motorTorque = torque * _accelAxis;
            }
        }
        else
        {
            if (!turningWhileStill)
            {
                for (int i = 0; i < wCols.Length; i++)
                {
                    wCols[i].motorTorque = 0;
                }
            }
        }

    }
    public void PlayerAttack(bool _attackFront, bool _attackLeft, bool _attackRight)
    {
        //attack up front
        if (_attackFront)
        {
            if (playAtackCont.CanShoot() && !isImmobilized)
            {
                if (curWeapon == Weapon.MainCannons)
                {
                    playAtackCont.ShootForward(playerIndex);
                }
            }
        }
        //attack to left side
        if (_attackLeft)
        {
            if (playAtackCont.CanShoot() && !isImmobilized)
            {
                if (curWeapon == Weapon.MainCannons)
                {
                    playAtackCont.ShootToSide(playerIndex, 0);
                }
            }
        }
        //attack to right side
        if (_attackRight)
        {
            if (playAtackCont.CanShoot() && !isImmobilized)
            {
                if (curWeapon == Weapon.MainCannons)
                {
                    playAtackCont.ShootToSide(playerIndex, 1);
                }
            }
        }
    }
    public void PlayerChangeWeapon(bool _weaponChange)
    {
        if (_weaponChange)
        {
            curWeapon++;
            if ((int)curWeapon >= Enum.GetValues(typeof(Weapon)).Length)
            {
                curWeapon = Weapon.MainCannons;
            }
        }
    }
    public void SlowDownPlayer()
    {
        cachedSmokeParticles = new List<GameObject>();
        //fancy particle
        for (int i = 0; i < smokePartTrans.Length; i++)
        {
            GameObject go = GameObject.Instantiate(SmokeParticle, smokePartTrans[i].position, 
                Quaternion.identity, smokePartTrans[i]);
            cachedSmokeParticles.Add(go);
        }

        //set speed down to 4/5
        maxSpeed = (4f / 5f) * desMaxSpeed;
        torque = (4f / 5f) * desMaxTorque;
    }
    public void SpeedUpPlayer()
    {
        foreach (GameObject part in cachedSmokeParticles)
        {
            GameObject.Destroy(part);
        }
        maxSpeed = desMaxSpeed;
        torque = desMaxTorque;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("PowerUp"))
        {
            PowerUpController tmpCont = other.gameObject.GetComponent<PowerUpController>();
            PowerUpType _type = tmpCont.GetPUType;
            switch (_type)
            {
                case (PowerUpType.HEALTH):
                    healthCont.AddHealth(tmpCont.healthAmmount);
                    GameObject.Destroy(other.gameObject);
                    break;
                case (PowerUpType.HEALTH2):
                    healthCont.AddHealth(tmpCont.healthAmmount);
                    GameObject.Destroy(other.gameObject);
                    break;
                case (PowerUpType.AMMO1):
                    playAtackCont.AddAmmo(tmpCont.ammoAmmount);
                    GameObject.Destroy(other.gameObject);
                    break;
                default:
                    break;
            }
        }
    }

}
