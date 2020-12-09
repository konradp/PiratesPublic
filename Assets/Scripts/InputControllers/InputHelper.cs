using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;


public class InputHelper : MonoBehaviour
{
    public static InputHelper instance;
    public int humanLocalPlayers;
    public InputType[] PlayerInputMethods;
    int[] curInputIndex;
    //TODO:
    //Make method to find out which controller is for which player
    private void Awake()
    {
        //create instance
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
        DontDestroyOnLoad(this);
        curInputIndex = new int[humanLocalPlayers];
        PlayerInputMethods = new InputType[humanLocalPlayers];
    }
    private void Start()
    {
    }
    public void ChangeInputTypeForPlayer(int playerIndex)
    {
        curInputIndex[playerIndex]++;
        if (curInputIndex[playerIndex] >= Enum.GetNames(typeof(InputType)).Length)
            curInputIndex[playerIndex] = 0;
        PlayerInputMethods[playerIndex] = (InputType)curInputIndex[playerIndex];
    }
    public void ChangeInputTypeForPlayer(int playerIndex, InputType type)
    {
        switch (type)
        {
            case InputType.KEYBOARD:
                curInputIndex[playerIndex] = 0;
                PlayerInputMethods[playerIndex] = type;
                break;
            case InputType.GAMEPAD:
                curInputIndex[playerIndex] = 1;
                PlayerInputMethods[playerIndex] = type;
                break;
            default:
                break;
        }
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(InputHelper))]
public class InputHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        InputHelper helper = (InputHelper)target;
        if (GUILayout.Button("Change Input P1"))
        {
            helper.ChangeInputTypeForPlayer(0);
        }
        if (GUILayout.Button("Change Input P2"))
        {
            helper.ChangeInputTypeForPlayer(1);
        }

    }
}
#endif