using UnityEngine;
using UnityEditor;

public class DebugAudio : MonoBehaviour
{

    [SerializeField] AudioSource tmpAudio;
    public void PlayAudio(){
        tmpAudio.Play();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(DebugAudio))]
public class DebugAudioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DebugAudio deb = (DebugAudio)target;
        if (GUILayout.Button("DebugPlay"))
        {
            deb.PlayAudio();
        }

    }
}
#endif
