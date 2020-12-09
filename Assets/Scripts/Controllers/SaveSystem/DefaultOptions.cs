using UnityEngine;
public static class DefaultOptions
{
    public static Options GetDefaultOptions()
    {
        Resolution[] defaultResolutions = new Resolution[3];
        defaultResolutions[0].width = 1280;
        defaultResolutions[0].height = 720;
        defaultResolutions[0].refreshRate = 60;
        defaultResolutions[1].width = 1440;
        defaultResolutions[1].height = 900;
        defaultResolutions[1].refreshRate = 60;
        defaultResolutions[2].width = 1920;
        defaultResolutions[2].height = 1080;
        defaultResolutions[2].refreshRate = 60;

        return new Options(defaultResolutions, 0, true, FullScreenMode.ExclusiveFullScreen, 0, 60, false,InputType.KEYBOARD);

    }
}
