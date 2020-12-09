using UnityEngine;

public class Options
{
    public bool hasBeenAssigned { get; set; }
    public Resolution[] resolutions { get; set; }
    public int curResolution { get; set; }
    public bool fullScreen { get; set; }
    public FullScreenMode fullScreenMode { get; set; }
    public int vSyncCount { get; set; }
    public int targetFPS { get; set; }
    public bool bgmMute { get; set; }
    public InputType inputType { get; set; }

    //getters

    public Resolution[] GetResolutions { get => resolutions; }
    public int GetCurResolution { get => curResolution; }
    public bool GetFullScreen { get => fullScreen; }
    public FullScreenMode GetFullScreenMode { get => fullScreenMode; }
    public int GetVsyncCount { get => vSyncCount; }
    public int GetTargetFPS { get => targetFPS; }
    public bool GetBGMMute { get => bgmMute; }
    public InputType GetInputType { get => inputType; }
    /// <summary>
    /// Options Constructor
    /// </summary>
    /// <param name="_resolutions">Array of accepted and tested resolutions</param>
    /// <param name="_curResolution">Current resolution index</param>
    /// <param name="_fullScreen">Is game in full screen?</param>
    /// <param name="_fullScreenMode">What full screen mode do we have? Default Exclusive FullScreen</param>
    /// <param name="_vsyncCount">what's vsync status? 0 for ignore, 1 for vsync, 2 for half etc. Default 0</param>
    /// <param name="_targetFPS">Target FPS. Default 60</param>
    /// <param name="_bgmMute">is BGM muted? Default false</param>
    /// <param name="_inputType">selected input type</param>
    public Options(Resolution[] _resolutions, int _curResolution, bool _fullScreen, FullScreenMode _fullScreenMode, 
        int _vsyncCount, int _targetFPS, bool _bgmMute, InputType _inputType)
    {
        resolutions = _resolutions;
        curResolution = _curResolution;
        fullScreen = _fullScreen;
        fullScreenMode = _fullScreenMode;
        vSyncCount = _vsyncCount;
        targetFPS = _targetFPS;
        bgmMute = _bgmMute;
        inputType = _inputType;
        hasBeenAssigned = true;
    }
    public Options()
    {
        
    }
}
