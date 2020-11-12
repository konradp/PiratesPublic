using System;
using TMPro;
using UnityEngine;

public class GlobalGameOptions : MonoBehaviour
{
    int fullScreenMode;
    string currentScreenmodeText;
    public TextMeshProUGUI FSinfoText, FSmodeText, VsyncText, fpsText, resolutionText, muteText;
    int curVsync = 0;
    int targetFPS;
    Resolution[] resolutions;
    int curRes = 1;
    bool isBGMMuted;
    private void Start()
    {
        DebugSetupResolutions();
        fullScreenMode = (int)Screen.fullScreenMode;
        FSinfoText.text = "Current FS mode: " + Screen.fullScreen.ToString();
        FSmodeText.text = "Current mode: " + Screen.fullScreenMode.ToString();
        curVsync = QualitySettings.vSyncCount;
        targetFPS = Application.targetFrameRate;
        isBGMMuted=false;
        ChangeMuteText();
        ChangeVsyncText();
        ChangeResolutionText();
        ChangeFPSText();

    }
    void DebugSetupResolutions()
    {
        resolutions = new Resolution[2];
        resolutions[0].height = 720;
        resolutions[0].width = 1280;
        resolutions[0].refreshRate = 60;
        resolutions[1].height = 1080;
        resolutions[1].width = 1920;
        resolutions[1].refreshRate = 60;
        switch (Screen.currentResolution.height)
        {
            case(720):
                curRes = 0;
                break;
            case(1080):
                curRes = 1;
                break;
            default:
                break;
        }
    }
    void ChangeFPSText()
    {
        fpsText.text = "Target FPS: " + targetFPS;
    }
    void ChangeVsyncText()
    {
        switch (curVsync)
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
        resolutionText.text = "Resolution: " + resolutions[curRes].width + "/"+ resolutions[curRes].height;
    }
    void ChangeMuteText()
    {
        if (isBGMMuted)
        {
            muteText.SetText("BGM: OFF");
        }
        else
        {
            muteText.SetText("BGM: ON");
        }
    }
    void ChangeFPS()
    {
        int fpsMin = 30;
        int fpsMax = 120;
        targetFPS+=15;
        if(targetFPS>fpsMax || targetFPS <fpsMin)
            targetFPS = fpsMin;
        Application.targetFrameRate = targetFPS;

    }
    void ChangeResolution()
    {
        curRes+=1;
        if(curRes>1)
            curRes=0;
        Screen.SetResolution(resolutions[curRes].width,resolutions[curRes].height,
            Screen.fullScreenMode,resolutions[curRes].refreshRate);
        
    }
    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        FSinfoText.text = "Current FS mode: " + Screen.fullScreen.ToString();
    }
    public void ToggleFullScreenMode()
    {
        fullScreenMode++;
        if (fullScreenMode > Enum.GetNames(typeof(FullScreenMode)).Length)
            fullScreenMode = 0;
        Screen.fullScreenMode = (FullScreenMode)fullScreenMode;
        currentScreenmodeText = Screen.fullScreenMode.ToString();
        FSmodeText.text = "Current mode: " + currentScreenmodeText;
    }
    public void ToggleVsync()
    {
        curVsync+=1;
        if(curVsync>1)
            curVsync=0;
        QualitySettings.vSyncCount = curVsync;
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
        isBGMMuted = !isBGMMuted;
        PlaylistManager.instance.ChangePlaybackStatus(isBGMMuted);
        ChangeMuteText();
    }
}
