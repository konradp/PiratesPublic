using UnityEngine;

public class TimeMaster : MonoBehaviour
{
    private void Update()
    {
        TimeControl.timeSinceLevelLoad += TimeControl.deltaTime;
    }
}
