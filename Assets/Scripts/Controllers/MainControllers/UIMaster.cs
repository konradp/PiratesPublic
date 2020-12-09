using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class UIMaster : MonoBehaviour
{
    //ref to gm
    GameMaster gm;
    //ref to our player
    [SerializeField] PlayerController curPlayer;
    //reference to text objs in canvas
    [SerializeField] TextMeshProUGUI weaponText, FPStext, PPSText, ammoText, curEnemyText, playerSpeedText;
    //ref to menus
    [SerializeField] GameObject PauseMenuParent, winMenu, loseMenu;
    //ref to buttons
    [SerializeField] Button pauseSelectBut, winSelectBut, loseSelectBut;
    //ref to main canvas
    [SerializeField] GameObject mainCanvas;
    bool isUIActive;
    GameObject[] healthObjs;
    //fps timers
    float msec, fps, ppsmsec, pps;
    float fpstimer = 0;
    float fpsmaxtimer = 0.05f;

    //ref to cutout alpha delay timer
    [SerializeField] GameObject cutout;

    //static instance
    public static UIMaster instance;
    //cached value
    float cutoutValue =0f;

    private void Start()
    {
        gm = GameMaster.instance;
        if (gm == null)
            Debug.LogError("Couldn't find Game Master!");
        msec = fps = ppsmsec = pps = 0;
        isUIActive = true;
        StartCoroutine(FindHealths(1f));
        if (instance == null)
            instance = this;
        
    }
    IEnumerator FindHealths(float time)
    {
        yield return new WaitForSeconds(time);
        var tmp = GameObject.FindObjectsOfType<RectTransform>();
        int index = 0;
        List<GameObject> objss = new List<GameObject>();
        foreach (RectTransform item in tmp)
        {
            if (item.gameObject.name.Contains("HealthBG"))
            {
                index++;
                objss.Add(item.gameObject);
            }
        }
        healthObjs = new GameObject[index];
        for (int i = 0; i < index; i++)
        {
            healthObjs[i] = objss[i];
        }
    }
    private void Update()
    {
        fpstimer += TimeControl.deltaTime;
        if (fpstimer > fpsmaxtimer)
        {
            msec = TimeControl.deltaTime * 1000.0f;
            fps = 1.0f / TimeControl.deltaTime;
            ppsmsec = TimeControl.fixedDeltaTime * 1000.0f;
            pps = 1.0f / TimeControl.fixedDeltaTime;
            FPStext.SetText(string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps));
            PPSText.SetText(string.Format("{0:0.0} ms ({1:0.} pps)", ppsmsec, pps));
            fpstimer = 0;
        }
        playerSpeedText.SetText(string.Format("PlayerSpeed : {0}",curPlayer.GetCurSpeed.ToString("n2")));
    }
    private void LateUpdate()
    {
        weaponText.SetText(string.Format("Current Weapon: {0}", curPlayer.GetCurWeapon.ToString()));
        switch (curPlayer.GetCurWeapon)
        {
            case (Weapon.MainCannons):
                ammoText.SetText(string.Format("Ammo: {0}", curPlayer.GetAmmoCount.ToString()));
                break;
            case (Weapon.PreciseCannon):
                ammoText.SetText("not implemented");
                break;
            default:
                break;
        }
        curEnemyText.SetText(string.Format("Enemy Left: {0} ", gm.GetEnemyCount));

    }
    private void FixedUpdate()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            HideGUI();
        }
    }
    void HideGUI()
    {
        isUIActive = !isUIActive;
        mainCanvas.SetActive(isUIActive);
        for (int i = 0; i < healthObjs.Length; i++)
        {
            healthObjs[i].transform.parent.gameObject.SetActive(isUIActive);
        }
        if (isUIActive)
            Camera.main.nearClipPlane = 0.5f;
        else
            Camera.main.nearClipPlane = 0.001f;
    }
    public void GamePauseScreen(bool _isVisible)
    {
        if (_isVisible)
        {
            PauseMenuParent.SetActive(true);
            pauseSelectBut.Select();
        }
        else
        {
            PauseMenuParent.SetActive(false);
        }
    }
    public void GameWinScreen()
    {
        winMenu.SetActive(true);
        winSelectBut.Select();
    }
    public void GameLostScreen()
    {
        loseMenu.SetActive(true);
        loseSelectBut.Select();
    }
    public void ChangeCutout(float _time, float _maxTime)
    {
        cutoutValue = _time / _maxTime;
        cutout.GetComponent<Image>().fillAmount = cutoutValue;

    }
}
