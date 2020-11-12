using System.Collections;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [HideInInspector] public AudioSource source;

    [Range(0f, 1f)] public float volume = 0.7f;
    [Range(0.5f, 4f)] public float pitch = 1f;
    [Range(0f, 1f)] public float originalVolume = 1f;
    [Range(0f,1f)] public float spatialBlend = 1f;
    [Range(50f,500f)] public float soundDistance = 200f;
    [Range(0f, 0.5f)] public float randomVolume = 0.1f;
    [Range(0f, 0.5f)] public float randomPitch = 0.1f;
    public bool isRandom = false;
    public bool loop = false;

    public Sound(float _vol, float _pitch, float _originalVol, float _spatialBlend, float _soundDistance, float _ranVol, float _ranPitch, bool _isRandom, bool _loop, AudioClip _clip)
    {
        volume = _vol;
        pitch = _pitch;
        originalVolume = _originalVol;
        spatialBlend = _spatialBlend;
        soundDistance = _soundDistance;
        randomVolume = _ranVol;
        randomPitch = _ranPitch;
        isRandom = _isRandom;
        loop = _loop;
        clip = _clip;
    }
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
        source.spatialBlend = spatialBlend;
        source.maxDistance = soundDistance;
    }

    public void Play()
    {
        if (isRandom)
        {
            source.volume = volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
            source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        }
        else
        {
            source.volume = volume;
            source.pitch = pitch;
        }
        source.Play();
    }
    public void ChangeTransform(Transform _transform)
    {
        source.gameObject.transform.position = _transform.position;
    }
    public void Stop()
    {
        source.Stop();
    }
    public bool IsPlaying()
    {
        return source.isPlaying;
    }
    public void ChangeMute()
    {
        source.mute = !source.mute;
    }
    public void ChangePitch()
    {
        source.pitch = pitch;
    }
    public bool IsMuted()
    {
        return source.mute;
    }
    public void ChangeVolume(float desVolume)
    {
        volume = originalVolume * desVolume;
        source.volume = volume;
    }
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]Sound[] sounds;
    public string[] GetSoundNames
    {
        get
        {
            string[] ret = new string[sounds.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = sounds[i].name;
            }
            return ret;
        }
    }
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
    }
    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            CreateSound(sounds[i], i);
        }
        
    }
    void CreateSound(Sound _sound, int _index)
    {
        GameObject go = new GameObject("Sound_" + _index + "_" + _sound.name);
        go.transform.SetParent(this.transform);
        _sound.SetSource(go.AddComponent<AudioSource>());
        _sound.originalVolume = _sound.volume;
    }
    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play();
                return;
            }
        }
        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list: " + _name);
    }
    /// <summary>
    /// EXPERIMENTAL
    /// Useful when trying to play multiple overlaping sounds, such as bullets
    /// </summary>
    /// <param name="_name">Name of sound</param>
    /// <param name="_pos">Global Position of where sound should be</param>
    public void PlayCopy(string _name, Transform _pos)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                GameObject go = new GameObject("Sound_" + Random.Range(10, 2137) + sounds[i].name);
                go.transform.SetParent(sounds[i].source.transform.parent);
                go.transform.position = _pos.position;
                Sound _s = new Sound(
                    sounds[i].volume,sounds[i].pitch,sounds[i].originalVolume, sounds[i].spatialBlend, sounds[i].soundDistance, 
                    sounds[i].randomVolume, sounds[i].randomPitch, sounds[i].isRandom, sounds[i].loop, sounds[i].clip);
                _s.SetSource(go.AddComponent<AudioSource>());
                _s.Play();
                StartCoroutine(DestroyCopiedSound(go, 10f));
            }
        }
    }
    IEnumerator DestroyCopiedSound(GameObject _go, float _time)
    {
        yield return new WaitForSeconds(_time);
        GameObject.Destroy(_go);
    }
    public void PlayOnce(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if (!sounds[i].source.isPlaying)
                {
                    sounds[i].Play();
                    return;
                }
            }
        }
    }
    public bool IsPlaying(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                return sounds[i].IsPlaying();
            }
        }
        Debug.LogWarning("AudioManager: Sound not found in list: " + _name);
        return false;
    }
    public void PitchControl(string _name, float _pitch)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].pitch = _pitch;
                sounds[i].ChangePitch();
                return;
            }
        }
    }
    public bool IsMuted(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
            if (sounds[i].name == _name)
                return sounds[i].IsMuted();
        //at this point all else failed
        Debug.LogError("Can't find desired sound name or the sound doesnt know if it's muted");
        return false;
    }
    public void ChangeMute(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].ChangeMute();
                return;
            }
        }
    }
    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }

        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list: " + _name);
    }
    public void ChangeVolume(float desVolume)
    {
        AudioListener.volume = desVolume;
    }
}
