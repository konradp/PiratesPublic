public static class StaticOptionsVals
{
    public static string GetBGMMusic{ get { return "discovery-bgm"; } }
    public static string GetShootSFX{ get { return "shoot"; } }
    public static string GetShoot2SFX { get { return "shoot2"; } }
    public static string GetShoot3SFX { get { return "shoot3"; } }
    public static string GetSplashSound { get { return "splash"; } }
    public static string GetRandomShootSound 
    {   get 
        {
            int tmp = UnityEngine.Random.Range(0, 3);
            switch (tmp)
            {
                case (0):
                    return "shoot";
                case (1):
                    return "shoot2";
                case (2):
                    return "shoot3";
                default:
                    return "shoot";
            }
        } 
    }
}
