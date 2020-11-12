using UnityEngine;

public static class TimeControl
{
    public static bool isPaused = false;
    public static float deltaTime { get { return isPaused ? 0 : Time.deltaTime; } }
    public static float fixedDeltaTime { get { return isPaused ? 0 : Time.fixedDeltaTime; } }
    public static float timeSinceLevelLoad { get; set; }
}

