using System;
using TMPro;
using UnityEngine;
using UnityEditor;
using System.Collections;

public class GlobalGameOptions : MonoBehaviour
{
    string currentScreenmodeText;
    public TextMeshProUGUI FSinfoText, FSmodeText, VsyncText, fpsText, resolutionText, muteText, inputTypeText;

    Options loadedOptions;

    public static GlobalGameOptions instance;
    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                //Destroy(this.gameObject);
                this.enabled = false;
            }
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {

    }

    public void SetGameOptions(Options _opts)
    {
        loadedOptions = _opts;
        SetOptionValues();
        SetTextOnceOptionsLoaded();

    }
    void SetOptionValues()
    {
        Application.targetFrameRate = loadedOptions.targetFPS;
        Screen.fullScreen = loadedOptions.fullScreen;
        Screen.fullScreenMode = loadedOptions.fullScreenMode;
        Screen.SetResolution(loadedOptions.resolutions[loadedOptions.curResolution].width,
            loadedOptions.resolutions[loadedOptions.curResolution].height,
            Screen.fullScreenMode, loadedOptions.resolutions[loadedOptions.curResolution].refreshRate);
        
        QualitySettings.vSyncCount = loadedOptions.vSyncCount;
        InputHelper.instance.ChangeInputTypeForPlayer(0,loadedOptions.inputType);
        PlaylistManager.instance.ChangePlaybackStatus(loadedOptions.bgmMute);

    }
    void SetTextOnceOptionsLoaded()
    {
        FSinfoText.text = "Current FS mode: " + Screen.fullScreen.ToString();
        FSmodeText.text = "Current mode: " + Screen.fullScreenMode.ToString();
        ChangeMuteText();
        ChangeVsyncText();
        ChangeResolutionText();
        ChangeFPSText();
        ChangeControlText();
    }
    void ChangeFPSText()
    {
        fpsText.text = "Target FPS: " + loadedOptions.GetTargetFPS;
    }
    void ChangeVsyncText()
    {
        switch (loadedOptions.GetVsyncCount)
        {
            case(0):
                VsyncText.text = "Vsync: Off";
                break;
            case(1):
                VsyncText.text = "Vsync: On";
                break;
            default:
                break;
        }
    }
    void ChangeResolutionText()
    {
        resolutionText.text = "Resolution: " + loadedOptions.resolutions[loadedOptions.GetCurResolution].width + "/"
            + loadedOptions.resolutions[loadedOptions.GetCurResolution].height;
    }
    void ChangeMuteText()
    {
        if (loadedOptions.GetBGMMute)
        {
            muteText.SetText("BGM: OFF");
        }
        else
        {
            muteText.SetText("BGM: ON");
        }
    }
    void ChangeControlText()
    {
        switch (loadedOptions.GetInputType)
        {
            case InputType.KEYBOARD:
                inputTypeText.SetText("Input: Keyboard");
                break;
            case InputType.GAMEPAD:
                inputTypeText.SetText("Input: Xbox Gamepad");
                break;
            default:
                break;
        }
    }
    void ChangeFPS()
    {
        int fpsMin = 30;
        int fpsMax = 120;
        loadedOptions.targetFPS+=15;
        if(loadedOptions.targetFPS > fpsMax || loadedOptions.targetFPS < fpsMin)
            loadedOptions.targetFPS = fpsMin;
        Application.targetFrameRate = loadedOptions.targetFPS;

    }
    void ChangeResolution()
    {
        loadedOptions.curResolution+=1;
        if(loadedOptions.curResolution >= loadedOptions.GetResolutions.Length)
            loadedOptions.curResolution = 0;
        Screen.SetResolution(loadedOptions.resolutions[loadedOptions.curResolution].width, 
            loadedOptions.resolutions[loadedOptions.curResolution].height,
            Screen.fullScreenMode, loadedOptions.resolutions[loadedOptions.curResolution].refreshRate);
        
    }
    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        loadedOptions.fullScreen = Screen.fullScreen;
        FSinfoText.text = "Current FS mode: " + Screen.fullScreen.ToString();
    }
    public void ToggleFullScreenMode()
    {
        loadedOptions.fullScreenMode++;
        if ((int)loadedOptions.fullScreenMode >= Enum.GetNames(typeof(FullScreenMode)).Length)
            loadedOptions.fullScreenMode = 0;
        Screen.fullScreenMode = loadedOptions.fullScreenMode;
        currentScreenmodeText = Screen.fullScreenMode.ToString();
        FSmodeText.text = "Current mode: " + currentScreenmodeText;
    }
    public void ToggleVsync()
    {
        loadedOptions.vSyncCount+=1;
        if(loadedOptions.vSyncCount > 1)
            loadedOptions.vSyncCount = 0;
        QualitySettings.vSyncCount = loadedOptions.vSyncCount;
        ChangeVsyncText();
    }
    public void ChangeTargetFPSBut()
    {
        ChangeFPS();
        ChangeFPSText();
    }
    public void ChangeResolutionBut()
    {
        ChangeResolution();
        ChangeResolutionText();
    }
    public void ChangeMute()
    {
        loadedOptions.bgmMute = !loadedOptions.bgmMute;
        PlaylistManager.instance.ChangePlaybackStatus(loadedOptions.bgmMute);
        ChangeMuteText();
    }
    public void ChangeInputType()
    {
        InputHelper.instance.ChangeInputTypeForPlayer(0);
        loadedOptions.inputType = InputHelper.instance.PlayerInputMethods[0];
        ChangeControlText();
    }
    public void SaveOptions()
    {
        XMLManipulator.SaveOptions(loadedOptions);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(GlobalGameOptions))]
public class GlobalGameOptionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GlobalGameOptions options = (GlobalGameOptions)target;
        if (GUILayout.Button("Toggle FPS"))
        {
            options.ChangeTargetFPSBut();
        }
        if (GUILayout.Button("Toggle FullScreenMode"))
        {
            options.ToggleFullScreenMode();
        }
    }
}
#endif