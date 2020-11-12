using UnityEngine;

public class PowerUpController : MonoBehaviour
{

    [SerializeField] Transform icon;
    [SerializeField] Material healthMat, healthMat2,ammoMat;
    [SerializeField] GameObject IconParent;
    [SerializeField] GameObject LightObj;
    Quaternion rot;
    PowerUpType type;
    public PowerUpType GetPUType {get {return type;}}
    public int healthAmmount,ammoAmmount;
    private void Start() 
    {
        rot = icon.rotation;
    }
    public void SetUpPowerUp(PowerUpType _type, int _healthAmmount, int _ammoAmmount)
    {
        type = _type;
        healthAmmount = _healthAmmount;
        ammoAmmount = _ammoAmmount;
        ChangeTexture();
    }
    private void Update() 
    {
        IconRotator();
    }
    public void DeletePU()
    {
        GameObject.Destroy(this);
    }
    void IconRotator()
    {
        icon.Rotate(0f,0f,30f*TimeControl.deltaTime,Space.Self);
    }
    void ChangeTexture()
    {
        switch(type)
        {
            case PowerUpType.HEALTH:
                IconParent.GetComponent<MeshRenderer>().material = healthMat;
                break;
            case PowerUpType.HEALTH2:
                IconParent.GetComponent<MeshRenderer>().material = healthMat;
                LightObj.SetActive(true);
                break;
            case PowerUpType.AMMO1:
                IconParent.GetComponent<MeshRenderer>().material = ammoMat;
                break;
            default:
                break;
        }
    }
}
