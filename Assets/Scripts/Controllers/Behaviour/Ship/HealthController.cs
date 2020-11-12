using UnityEngine;
using UnityEditor;


public class HealthController : MonoBehaviour, IDamageable, IKillable
{
    GameMaster gm;
    int maxHealth = 100;
    int curHealth, playerIndex, healthDivisor;
    public float GetCurHealth { get { return curHealth; } }
    bool isAI, isSpeedHalved;
    // UI
    [SerializeField] RectTransform healthBar;
    float maxHealthBarWidth;
    private void Start()
    {
        gm = GameMaster.instance;
        if (gm == null)
            Debug.LogError("Couldn't find Game Master!");
        curHealth = maxHealth;
        isSpeedHalved = false;
        healthDivisor = 2;
    }
    public void SetUpHealthController(int _maxHealth, bool _isAI, int _playerIndex, RectTransform _healthBar)
    {
        isAI = _isAI;
        playerIndex = _playerIndex;
        if(isAI && healthBar==null)
            healthBar=_healthBar;
        maxHealthBarWidth = healthBar.sizeDelta.x;
        SetMaxHealth(_maxHealth, false);

    }
    public void SetMaxHealth(int _maxHealth, bool _isBoosted)
    {
        maxHealth = _maxHealth;
        curHealth = _maxHealth;
        if(healthBar!=null)
            UpdateHealthBar(-maxHealth);
        if(_isBoosted)
            healthDivisor = 100;
    }
    public void TakeDamage(int _dmg)
    {
        curHealth -= _dmg;
        UpdateHealthBar(_dmg);
        if(!isSpeedHalved && curHealth < maxHealth/healthDivisor)
        {
            isSpeedHalved = true;
            SlowDownShip();
        }
        if (curHealth <= 0f)
        {
            Kill();
        }
    }

    void SlowDownShip()
    {
        if (isAI)
        {
            GetComponent<AIPlayerController>().SlowDownAI();
        }
        else
        {
            GetComponent<PlayerController>().SlowDownPlayer();
        }
    }
    public void AddHealth(int _health)
    {
        curHealth+=_health;
        if(curHealth>maxHealth)
        {
            curHealth=maxHealth;
        }
        UpdateHealthBar(-_health);
        if (isSpeedHalved && curHealth > maxHealth / 2)
        {
            isSpeedHalved = false;
            //we can assume only player can pickup/regen health, so no checking for ai needed
            GetComponent<PlayerController>().SpeedUpPlayer();
        }
    }
    void UpdateHealthBar(int _dmg)
    {
        if(healthBar != null)
        {
            float tmp = ((float)_dmg * maxHealthBarWidth) / maxHealth;
            float tmp2 = healthBar.sizeDelta.x - tmp;
            if(tmp2>maxHealthBarWidth)
                tmp2 = maxHealthBarWidth;
            Vector2 v2 = new Vector2(tmp2, healthBar.sizeDelta.y);
            healthBar.sizeDelta = v2;
        }
        else
        {
            Debug.LogError("We don't have helth indicator");
        }

    }
    public void Kill()
    {
        Debug.Log("kaboom. make it more dramatic later");
        if (isAI)
        {
            GetComponent<AIPlayerController>().ImmobilizePlayer();
        }
        else 
        { 
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<PlayerController>().isImmobilized = true; 
            gm.PlayerKilled();
        }
    }
}
#if UNITY_EDITOR

/// <summary>
/// Debug Damage editor override
/// </summary>
[CustomEditor(typeof(HealthController))]
public class HealthControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HealthController cont = (HealthController)target;
        if (GUILayout.Button("Debug Damage"))
        {
            cont.TakeDamage(10);
        }
    }
}
#endif