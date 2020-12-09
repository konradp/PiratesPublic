using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerController[] players;
    InputType[] playersInputMethods;
    CustomPlayerInput[] desiredInputs;
    int playerCount;
    //cached input values
    float[] steeringAxis, accelAxis, brakeAxis, cameraXAxis, cameraYaxis;
    bool[] attackButton, leftAttackButton, rightAttackButton, weaponChangeButton;

    private void Start()
    {
        //debug for now
        players = new PlayerController[1];
        players[0] = GetComponent<DebugMaster>().GetPlayer.GetComponent<PlayerController>();
        //end debug hardcoding
        //get desired players input methods
        playersInputMethods = InputHelper.instance.PlayerInputMethods;
        playerCount = InputHelper.instance.humanLocalPlayers;
        desiredInputs = new CustomPlayerInput[playerCount];

        //init arrays for controls
        steeringAxis = new float[playerCount];
        accelAxis = new float[playerCount]; 
        brakeAxis = new float[playerCount]; 
        cameraXAxis = new float[playerCount]; 
        cameraYaxis = new float[playerCount];
        attackButton = new bool[playerCount]; 
        leftAttackButton = new bool[playerCount]; 
        rightAttackButton = new bool[playerCount]; 
        weaponChangeButton = new bool[playerCount];

        //create input methods
        //find a way to check who has what input method. Maybe include proper steering method in input helper?
        for (int i = 0; i < playerCount; i++)
        {
            switch (playersInputMethods[i])
            {
                case(InputType.GAMEPAD):
                    desiredInputs[i] = InputPreset.GetDefaultGamepadPreset(Gamepad.current);
                    break;
                case (InputType.KEYBOARD):
                    desiredInputs[i] = InputPreset.GetDefaultKeyboardPreset(Keyboard.current);
                    break;
            }
        }
    }
    private void Update()
    {
        for (int i = 0; i < playerCount; i++)
        {
            switch (playersInputMethods[i])
            {
                case (InputType.GAMEPAD):
                    AssignGamepadControls(i);
                    break;
                case (InputType.KEYBOARD):
                    AssignKeyboardControls(i);
                    break;
                default:
                    break;
            }
            CallPlayerController(i);
        }
    }
    void AssignGamepadControls(int _playerIndex)
    {
        steeringAxis[_playerIndex] = desiredInputs[_playerIndex].getLeftStick.x.ReadValue();
        accelAxis[_playerIndex] = desiredInputs[_playerIndex].getRightTrigger.ReadValue();
        brakeAxis[_playerIndex] = desiredInputs[_playerIndex].getLeftTrigger.ReadValue();
        cameraXAxis[_playerIndex] = desiredInputs[_playerIndex].getRightStick.x.ReadValue();
        cameraYaxis[_playerIndex] = desiredInputs[_playerIndex].getRightStick.y.ReadValue();
        attackButton[_playerIndex] = desiredInputs[_playerIndex].getAttackButton.isPressed;
        leftAttackButton[_playerIndex] = desiredInputs[_playerIndex].getLeftAttackButton.isPressed;
        rightAttackButton[_playerIndex] = desiredInputs[_playerIndex].getRightAttackButton.isPressed;
        weaponChangeButton[_playerIndex] = desiredInputs[_playerIndex].getWeaponChangeButton.wasPressedThisFrame;
    }
    void AssignKeyboardControls(int _playerIndex)
    {
        float steerAngle;
        if (desiredInputs[_playerIndex].getLeftButton.isPressed)
            steerAngle = -1f;
        else if (desiredInputs[_playerIndex].getLeftButton.isPressed && desiredInputs[_playerIndex].getRightButton.isPressed)
            steerAngle = 0f;
        else if (desiredInputs[_playerIndex].getRightButton.isPressed)
            steerAngle = 1f;
        else
            steerAngle = 0f;
        steeringAxis[_playerIndex] = steerAngle;
        accelAxis[_playerIndex] = desiredInputs[_playerIndex].getAccelButton.isPressed ? 1f : 0f;
        brakeAxis[_playerIndex] = desiredInputs[_playerIndex].getBrakeButton.isPressed ? 1f : 0f;
        attackButton[_playerIndex] = desiredInputs[_playerIndex].getAttackButton.isPressed;
        leftAttackButton[_playerIndex] = desiredInputs[_playerIndex].getLeftAttackButton.isPressed;
        rightAttackButton[_playerIndex] = desiredInputs[_playerIndex].getRightAttackButton.isPressed;
        weaponChangeButton[_playerIndex] = desiredInputs[_playerIndex].getWeaponChangeButton.wasPressedThisFrame;
    }
    void CallPlayerController(int _playerIndex)
    {
        players[_playerIndex].CameraRotation(cameraXAxis[_playerIndex], cameraYaxis[_playerIndex]);
        players[_playerIndex].PlayerBrake(brakeAxis[_playerIndex], steeringAxis[_playerIndex]);
        players[_playerIndex].PlayerSteering(steeringAxis[_playerIndex]);
        players[_playerIndex].PlayerAccel(accelAxis[_playerIndex]);
        players[_playerIndex].PlayerAttack(attackButton[_playerIndex], leftAttackButton[_playerIndex], rightAttackButton[_playerIndex]);
        players[_playerIndex].PlayerChangeWeapon(weaponChangeButton[_playerIndex]);
    }

}
