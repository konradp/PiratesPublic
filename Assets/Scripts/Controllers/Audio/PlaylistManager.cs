using System.Collections.Generic;
using UnityEngine;

public class PlaylistManager : MonoBehaviour
{
    public static PlaylistManager instance;
    List<string> bgms;
    int curSong, maxSongs;

    //timers
    float songCheckerTimer = 0;
    float songCheckerMaxTimer = 1f;
    bool playbackStopped;
    public bool GetPlaybackStatus { get { return playbackStopped; } }
    void Awake()
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
            DontDestroyOnLoad(this);
        }
        GetAllBGMs();
    }

    private void Start() 
    {
        maxSongs = bgms.Count - 1;
        curSong = bgms.IndexOf(StaticOptionsVals.GetBGMMusic);
        PlaySong(curSong);
    }
    void GetAllBGMs()
    {
        bgms = new List<string>();
        for (int i = 0; i < AudioManager.instance.GetSoundNames.Length; i++)
        {
            if(AudioManager.instance.GetSoundNames[i].Contains("bgm")){
                bgms.Add(AudioManager.instance.GetSoundNames[i]);
            }
        }
    }
    void PlaySong(int _index)
    {
        AudioManager.instance.PlayOnce(bgms[_index]);
    }
    void StopSong(int _index)
    {
        AudioManager.instance.StopSound(bgms[_index]);
    }
    private void Update() 
    {
        songCheckerTimer+=TimeControl.deltaTime;
        if (songCheckerTimer>songCheckerMaxTimer)
        {
            songCheckerTimer=0;
            if(!AudioManager.instance.IsPlaying(bgms[curSong]) && !playbackStopped)
            {
                curSong++;
                if(curSong>maxSongs)
                    curSong=0;
                PlaySong(curSong);
            }
        }
    }
    public void ChangePlaybackStatus(bool _shouldStop){
        playbackStopped = _shouldStop;
        if(playbackStopped)
        {
            StopSong(curSong);
        }
    }
}
