using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] SphereCollider blastRadius;
    [SerializeField] GameObject hitParticle;
    int playerIndex;
    int damage;
    private void Start()
    {
        //run failsafe destroyer in case something bad happens
        StartCoroutine(DestroyOverTime(7f));
    }
    IEnumerator DestroyOverTime(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyBullet(null);
    }
    public void UpdateBulletData(int _playerIndex, int _damage, bool _isFront)
    {
        playerIndex = _playerIndex;
        damage = _damage;
        //if bullet is front bullet increase the radius
        if (_isFront)
        {
            blastRadius.radius = blastRadius.radius * 1.5f;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IDamageable>() != null)
        {
            IDamageable IDam = collision.gameObject.GetComponent<IDamageable>();
            if (collision.collider.CompareTag("Player0"))
            {
                //TODO: When making local multiplayer, check whether index of hit player is not our index. 
                if (playerIndex != 0)
                {
                    DamagePlayer(IDam, collision.transform);
                }
            }
            if (collision.collider.CompareTag("Enemy"))
            {
                
                if (playerIndex != collision.gameObject.GetComponent<AIPlayerController>().curPlayerID)
                {
                    DamagePlayer(IDam, collision.transform);
                }
            }
        }
        
        if (collision.collider.CompareTag("Terrain"))
        {
            DestroyBullet(null);
        }
        if (collision.collider.CompareTag("Water"))
        {
            DestroyOnWater();
        }
    }
    void DamagePlayer(IDamageable _ID, Transform _hitParent)
    {
        _ID.TakeDamage(damage);
        DestroyBullet(_hitParent);
    }
    void DestroyBullet(Transform _hitTrans)
    {
        AudioManager.instance.PlayCopy(StaticOptionsVals.GetShootSFX, transform);
        if (_hitTrans != null)
            GameObject.Instantiate(hitParticle, _hitTrans);
        Destroy(this.gameObject);
        
    }
    void DestroyOnWater()
    {
        AudioManager.instance.PlayCopy(StaticOptionsVals.GetSplashSound, transform);
        //make nice particle here
        Destroy(this.gameObject);
    }
}
