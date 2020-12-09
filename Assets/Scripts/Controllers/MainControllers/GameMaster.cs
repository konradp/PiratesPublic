using UnityEngine;

public enum AIShipStatus { IMMOBILIZED, WORKING, UNKNOWN, PACIFIST }
public enum PowerUpType {HEALTH, HEALTH2, AMMO1, AMMO2, BLANK, SPECIAL}
public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

    /// <summary>
    /// All of these variables represents AI Players and their spawns, as well as PowerUP
    /// </summary>
    [SerializeField] GameObject AIPrefab, PUPrefab;
    [SerializeField] Transform AICheckPointTrans, AIPlayerTrans;
    [SerializeField] Transform AISpawnParent, PUSpawnParent;
    Transform[] AISpawns, PUSpawns;
    /// <summary>
    /// So far only method I found to properly constraint AI Path Getter search for new path
    /// Every group of AIs have their own constrainable area where they can look for new path
    /// This will avoid searching forever within bigger area as well as provide some quicker results
    /// </summary>
    [SerializeField] Vector2[] AreaXConstraints, AreaZConstraints;
    GameObject[] AIPlayers;
    int[] IDs;

    public struct AIInfo
    {
        public int ID;
        public int Group;
        public AIShipStatus Status;
        public AIInfo(int _id, int _group, AIShipStatus _stat)
        {
            ID = _id;
            Group = _group;
            Status = _stat;
        }
    }
    public AIInfo[] AIs;

    public int defaultHealthAmmount, defaultAmmoAmmount, defaultHealth2Ammount;
    int enemyCount;
    public int GetEnemyCount {get {return enemyCount;}}
    float enemyCheckTimer;
    float enemyCheckTimerMax = 0.6f;
    UIMaster uIMaster;

    private void Awake()
    {
        //create instance
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        uIMaster = FindObjectOfType<UIMaster>();
        System.Random rand = new System.Random();
        AISpawns = new Transform[AISpawnParent.childCount];
        AIPlayers = new GameObject[AISpawnParent.childCount];
        AIs = new AIInfo[AISpawnParent.childCount];
        for (int i = 0; i < AISpawnParent.childCount; i++)
        {
             AISpawns[i] = AISpawnParent.GetChild(i);
        }
        IDs = new int[AISpawns.Length];
        SpawnController[] spawnConts = new SpawnController[AISpawns.Length];
        for (int i = 0; i < IDs.Length; i++)
        {
            IDs[i] = rand.Next(10, 2137);
            spawnConts[i] = AISpawns[i].gameObject.GetComponent<SpawnController>();
        }
        for (int i = 0; i < IDs.Length; i++)
        {
            AIPlayers[i] = GameObject.Instantiate(AIPrefab, AISpawns[i].position, Quaternion.identity, AIPlayerTrans);
            AIPlayers[i].name = IDs[i].ToString();
            AIPlayers[i].GetComponent<AIPlayerController>().AIPlayerSetup(AICheckPointTrans,IDs[i],
                AreaXConstraints[spawnConts[i].spawnIndex],AreaZConstraints[spawnConts[i].spawnIndex],
                spawnConts[i].AIGroup, spawnConts[i].shipIndex, spawnConts[i].isPacifist, spawnConts[i].failSafeTransform);
            AIs[i] = new AIInfo(IDs[i], spawnConts[i].AIGroup, 
                spawnConts[i].isPacifist ? AIShipStatus.PACIFIST : AIShipStatus.WORKING);
        }
        PUSpawns = new Transform[PUSpawnParent.childCount];
        PowerUpType[] types = new PowerUpType[PUSpawns.Length];
        for (int i = 0; i < PUSpawns.Length; i++)
        {
            PUSpawns[i] = PUSpawnParent.GetChild(i);
            GameObject _spawn = GameObject.Instantiate(PUPrefab,PUSpawns[i].position,Quaternion.identity,PUSpawns[i]);
            types[i] = PUSpawns[i].GetComponent<PowerUPSpawnCont>().PUType;
            switch (types[i])
            {
                case(PowerUpType.HEALTH):
                    _spawn.GetComponent<PowerUpController>().SetUpPowerUp(PowerUpType.HEALTH,defaultHealthAmmount,0);
                    break;
                case (PowerUpType.HEALTH2):
                    _spawn.GetComponent<PowerUpController>().SetUpPowerUp(PowerUpType.HEALTH2, defaultHealth2Ammount, 0);
                    break;
                case(PowerUpType.AMMO1):
                    _spawn.GetComponent<PowerUpController>().SetUpPowerUp(PowerUpType.AMMO1,0,defaultAmmoAmmount);
                    break;
                default:
                    break;
            }
        }
    }
    public int GetAIGroup(int _index)
    {
        int ret = -1;
        for (int i = 0; i < AIs.Length; i++)
        {
            if(AIs[i].ID == _index)
            {
                ret = AIs[i].Group;
            }
        }
        return ret;
    }
    public void SetAIStatus (AIShipStatus _status, int _index)
    {
        int checker = 0;
        for (int i = 0; i < AIs.Length; i++)
        {
            checker++;
            if (AIs[i].ID == _index)
            {
                checker -= 2;
                AIs[i].Status = _status;
            }
        }
        if (checker == AIs.Length)
            Debug.LogError("Couldn't set AI status for " + _index);
    }
    public AIShipStatus GetAIStatus(int _index)
    {
        for (int i = 0; i < AIs.Length; i++)
        {
            if (AIs[i].ID == _index)
            {
                return AIs[i].Status;
            }
        }
        return AIShipStatus.UNKNOWN;
    }
    private void Update() 
    {
        enemyCheckTimer+=TimeControl.deltaTime;
        if (enemyCheckTimer > enemyCheckTimerMax)
        {
            enemyCheckTimer=0;
            enemyCount=0;
            for (int i = 0; i < AIs.Length; i++)
            {
                if(AIs[i].Status==AIShipStatus.WORKING && AIs[i].Group == 0)
                {
                    enemyCount++;
                }
            }
            if (enemyCount==0)
            {
                //game win conditions
                GameWin();
            }
        }
    }
    void GameWin()
    {
        uIMaster.GameWinScreen();
    }
    public void PlayerKilled()
    {
        uIMaster.GameLostScreen();
    }
}
