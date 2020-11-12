public class ShipData
{
    int hp;
    int ammo;
    int coneMaxRaycasts;
    float maxSpeed;
    float maxSteerAngle;
    float maxTorque;
    float maxHitScanRange;
    float maxHitScanShootRange;
    public int GetHp { get { return hp; } }
    public int GetAmmo { get { return ammo; } }
    public int GetMaxRaycasts { get { return coneMaxRaycasts; } }
    public float GetMaxSpeed { get { return maxSpeed; } }
    public float GetMaxSteerAngle { get { return maxSteerAngle; } }
    public float GetMaxTorque { get { return maxTorque; } }
    public float GetMaxHitScanRange {  get { return maxHitScanRange; } }
    public float GetMaxHitScanShootRange { get { return maxHitScanShootRange; } }

    public ShipData(int _hp, int _ammo, float _maxSpeed, float _maxSteerAngle, float _maxTorque, int _maxRaycasts, float _maxHitScanRange, float _maxHitScanShootRange)
    {
        hp = _hp;
        ammo = _ammo;
        maxSpeed = _maxSpeed;
        maxSteerAngle = _maxSteerAngle;
        maxTorque = _maxTorque;
        coneMaxRaycasts = _maxRaycasts;
        maxHitScanRange = _maxHitScanRange;
        maxHitScanShootRange = _maxHitScanShootRange;
    }
}
public static class PresetShipData
{
    public static ShipData GetPresetBaseAI()
    {
        return new ShipData(50, 75, 20, 50, 300,25,20f,10f);
    }
    public static ShipData GetPresetBossAI()
    {
        return new ShipData(150, 750, 20, 50, 300,25,25f,15f);
    }
    public static ShipData GetPresetFishingAI()
    {
        return new ShipData(20, 10, 4, 40, 70,10,20f,10f);
    }
    public static ShipData GetPresetFromIndex(int _index)
    {
        switch (_index)
        {
            case (0):
                return GetPresetBaseAI();
            case (1):
                return GetPresetBossAI();
            case (9):
                return GetPresetFishingAI();
            default:
                return null;
        }
    }
}
