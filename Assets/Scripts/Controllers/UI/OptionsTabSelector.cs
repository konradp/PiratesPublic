using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsTabSelector : MonoBehaviour
{
    public GameObject[] OptionsTabs;
    public Button[] activateButtons;
    /// <summary>
    /// 0 is Keyboard, 1 is Gamepad
    /// </summary>
    public GameObject[] ControlsHelper;
    private void Awake()
    {
        ActivateTab(0);
    }
    public void ActivateTab(int _index)
    {
        for (int i = 0; i < OptionsTabs.Length; i++)
        {
            if (i == _index)
            {
                OptionsTabs[i].SetActive(true);
                activateButtons[i].Select();
            }
            else
                OptionsTabs[i].SetActive(false);
            //hardcoded. value 2 means we set active controls panel, so we need to switch visual helper accordingly
            if (i == 2)
            {
                ChangeControlsHelperVisuals();
            }
        }
    }
    public void ChangeControlsHelperVisuals()
    {

        switch (InputHelper.instance.PlayerInputMethods[0])
        {
            case InputType.KEYBOARD:
                ControlsHelper[0].SetActive(true);
                ControlsHelper[1].SetActive(false);
                break;
            case InputType.GAMEPAD:
                ControlsHelper[0].SetActive(false);
                ControlsHelper[1].SetActive(true);
                break;
            default:
                break;
        }


    }
}
