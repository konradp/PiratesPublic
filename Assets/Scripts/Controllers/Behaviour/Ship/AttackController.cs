using UnityEngine;

public class AttackController : MonoBehaviour
{
    public GameObject bulletPref;
    public GameObject smokeParticle;

    //positions of shoot vectors
    Transform shootForwardPiv, ShootBackPiv;
    Transform[] shootLeftPiv, shootRightPiv;

    //ammo count
    int curAmmo;
    public int GetCurAmmo{get {return curAmmo;}}

    //timers
    [SerializeField] float basicAttackDelay = 2f;
    float basicTimer = 0;
    bool isBasicAttackPossible = false;

    //shooting mechanics vars
    [SerializeField] float forceStrength = 150f;

    //if we're player
    bool isPlayer;
    private void Update()
    {
        basicTimer += TimeControl.deltaTime;
        if (basicTimer > basicAttackDelay)
        {
            isBasicAttackPossible = true;
        }
        if (isPlayer && basicTimer < (basicAttackDelay * 1.5f))
        {
            if (UIMaster.instance != null)
                UIMaster.instance.ChangeCutout(basicTimer, basicAttackDelay);
        }
    }

    public void SetUpAttackController(Transform _shootForward, 
    Transform[] _leftPivs, Transform[] _rightPivs, int _ammoAmmount, bool _isPlayer)
    {
        shootForwardPiv = _shootForward;
        shootLeftPiv = _leftPivs;
        shootRightPiv = _rightPivs;
        AddAmmo(_ammoAmmount);
        isPlayer = _isPlayer;
    }
    public void ShootForward(int _playerIndex)
    {
        if (isBasicAttackPossible && curAmmo>=1)
        {
            basicTimer = 0;
            isBasicAttackPossible = false;
            ShootBullet(shootForwardPiv, _playerIndex, 40, true);
        }
        else if(isBasicAttackPossible && curAmmo <1)
        {
            //player or ai can shoot but don't have enough ammo. 
            //todo: visual clue for player if they can't shoot
        }
    }
    /// <summary>
    /// Side shooting mechanics
    /// </summary>
    /// <param name="_direction">0 for left cannons, 1 for right</param>
    public void ShootToSide(int _playerIndex,int _direction)
    {
        if (isBasicAttackPossible && curAmmo>=3)
        {
            basicTimer = 0;
            isBasicAttackPossible = false;
            for (int i = 0; i < shootLeftPiv.Length; i++)
            {
                switch (_direction)
                {
                    case 0:
                        ShootBullet(shootLeftPiv[i], _playerIndex, 10, false);
                        break;
                    case 1:
                        ShootBullet(shootRightPiv[i], _playerIndex, 10, false);
                        break;
                    default:
                        break;
                }
            }
        }else if(isBasicAttackPossible && curAmmo<3)
        {
            //player or ai can shoot but don't have enough ammo. 
        }
    }
    void ShootBullet(Transform _pivot, int _playerIndex, int _damage, bool _isFront )
    {
        GameObject bullet = GameObject.Instantiate(bulletPref, _pivot.position, _pivot.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * forceStrength, ForceMode.Impulse);
        bullet.GetComponent<BulletController>().UpdateBulletData(_playerIndex, _damage, _isFront);
        GameObject.Instantiate(smokeParticle, _pivot.position, Quaternion.identity);
        curAmmo -= 1;
        AudioManager.instance.PlayCopy(StaticOptionsVals.GetShoot3SFX, transform);
    }
    public bool CanShoot()
    {
        return isBasicAttackPossible;
    }
    public void AddAmmo(int _ammount)
    {
        curAmmo+=_ammount;
    }


}
