using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugMaster : MonoBehaviour
{



    GameMaster gm;
    int curScene;
    private float enemyCheckTimer;
    private int bonusEnemyCount;
    private float enemyCheckTimerMax = 0.6f;
    [SerializeField] GameObject player, bonusWinCanv;
    bool alreadySetBonusCond = false;
    [SerializeField] bool isBonusEnabled;

    private void Start()
    {
        gm = GameMaster.instance;
        if (gm == null)
            Debug.LogError("Couldn't find Game Master!");
        curScene = SceneManager.GetActiveScene().buildIndex;
#if UNITY_EDITOR
        //this will delay my mental breakdown
        if (PlaylistManager.instance.GetPlaybackStatus != true)
            Invoke("KillMusic", 1f);
#endif

    }
    void KillMusic()
    {
        PlaylistManager.instance.ChangePlaybackStatus(true);
    }
    private void Update()
    {
        enemyCheckTimer += TimeControl.deltaTime;
        if (enemyCheckTimer > enemyCheckTimerMax && isBonusEnabled)
        {
            enemyCheckTimer = 0;
            bonusEnemyCount = 0;
            for (int i = 0; i < gm.AIs.Length; i++)
            {
                if (gm.AIs[i].Status == AIShipStatus.WORKING && gm.AIs[i].Group == 1)
                    bonusEnemyCount++;
            }
            if (bonusEnemyCount == 0)
            {
                //bonus conditions
                BonusWinCondition();
            }
        }
    }
    void BonusWinCondition()
    {
        if (!alreadySetBonusCond && isBonusEnabled)
        {
            alreadySetBonusCond = true;
            //activate win canvas for few seconds
            StartCoroutine(BonusWinCanvas(6f));
            //set extra health and ammo
            player.GetComponent<HealthController>().SetMaxHealth(900,true);
            player.GetComponent<AttackController>().AddAmmo(1000);
        }
    }
    IEnumerator BonusWinCanvas(float _time)
    {
        bonusWinCanv.SetActive(true);
        yield return new WaitForSeconds(_time);
        bonusWinCanv.SetActive(false);
    }

    public void ResumeGameButton()
    {
        GlobalInput.instance.SetPauseStatus(false);
    }
    public void QuitGameButton()
    {
        TimeControl.isPaused=false;
        Physics.autoSimulation = true;
        SceneManager.LoadScene(0);
    }
    public void GameWonResumeButton()
    {
        curScene++;
        if (curScene >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(curScene);
        }
    }

}
