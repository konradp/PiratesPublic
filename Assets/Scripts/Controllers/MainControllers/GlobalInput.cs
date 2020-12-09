using UnityStandardAssets.Utility;
using UnityEngine;
using UnityEngine.InputSystem;


public class GlobalInput : MonoBehaviour
{
    public static GlobalInput instance;

    CameraFreeFlyController cameraFreeFlyController;
    CameraSmoothFollow cameraSmoothFollow;

    bool isPaused;
    bool isFreeCam;

    Gamepad curPad;
    UIMaster uIMaster;

    private void Awake()
    {
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
        curPad = Gamepad.current;
        isFreeCam = false;
        cameraFreeFlyController = Camera.main.GetComponent<CameraFreeFlyController>();
        cameraSmoothFollow = Camera.main.GetComponent<CameraSmoothFollow>();
        uIMaster = UIMaster.instance;
    }
    private void FixedUpdate()
    {
        if (curPad != null)
        {
            if (curPad.startButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetPauseStatus(!isPaused);
            }
        }
        else
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetPauseStatus(!isPaused);
            }
        }
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            isFreeCam = !isFreeCam;
            if (isFreeCam)
            {
                cameraSmoothFollow.enabled = false;
                cameraFreeFlyController.enabled = true;
            }
            else
            {
                cameraSmoothFollow.enabled = true;
                cameraFreeFlyController.enabled = false;
            }
        }
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            PlaylistManager.instance.ChangePlaybackStatus(!PlaylistManager.instance.GetPlaybackStatus);
        }
        //bit of a debug cheat, remove later
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            FindObjectOfType<PlayerController>().gameObject.GetComponent<HealthController>().AddHealth(50);
        }

    }

    public void SetPauseStatus(bool _shouldPause)
    {
        Physics.autoSimulation = !_shouldPause;
        TimeControl.isPaused = _shouldPause;
        uIMaster.GamePauseScreen(_shouldPause);
        isPaused = _shouldPause;
    }


}
