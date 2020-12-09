using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public enum InputType {KEYBOARD, GAMEPAD}

public class CustomPlayerInput
{
    /// <summary>
    /// Use this constructor when creating binary player input, such as Keyboard
    /// </summary>
    public CustomPlayerInput(
        ButtonControl _attackButton, ButtonControl _leftAttackButton, ButtonControl _rightAttackButton,
        ButtonControl _weaponChangeButton, ButtonControl _accelButton, ButtonControl _brakeButton,
        ButtonControl _leftButton, ButtonControl _rightButton, ButtonControl _pauseButton, InputType _inputType
        )
    {
        attackButton = _attackButton;
        leftAttackButton = _leftAttackButton;
        rightAttackButton = _rightAttackButton;
        weaponChangeButton = _weaponChangeButton;
        accelButton = _accelButton;
        brakeButton = _brakeButton;
        leftButton = _leftButton;
        rightButton = _rightButton;
        pauseButton = _pauseButton;
        inputType = _inputType;
    }
    /// <summary>
    /// Use this constructor, when creating analog-based input, such as with GamePads
    /// </summary>

    public CustomPlayerInput(
        ButtonControl _attButton, ButtonControl _leftAttButton, ButtonControl _rightAttButton, ButtonControl _weaponChangeButton,
        ButtonControl _rightTrigger, ButtonControl _leftTrigger, StickControl _leftStick, StickControl _rightStick, InputType _inputType
        )
    {
        attackButton = _attButton;
        leftAttackButton = _leftAttButton;
        rightAttackButton = _rightAttButton;
        weaponChangeButton = _weaponChangeButton;
        rightTrigger = _rightTrigger;
        leftTrigger = _leftTrigger;
        leftStick = _leftStick;
        rightStick = _rightStick;
        inputType = _inputType;

    }

    private InputType inputType;
    private ButtonControl attackButton;
    private ButtonControl leftAttackButton;
    private ButtonControl rightAttackButton;
    private ButtonControl weaponChangeButton;
    private ButtonControl rightTrigger;
    private ButtonControl leftTrigger;
    private StickControl leftStick;
    private StickControl rightStick;
    private ButtonControl accelButton;
    private ButtonControl brakeButton;
    private ButtonControl leftButton;
    private ButtonControl rightButton;
    private ButtonControl pauseButton;

    public InputType getInputType { get => inputType; }
    public ButtonControl getAttackButton { get => attackButton; }
    public ButtonControl getLeftAttackButton { get => leftAttackButton; }
    public ButtonControl getRightAttackButton { get => rightAttackButton; }
    public ButtonControl getWeaponChangeButton { get => weaponChangeButton; }
    public ButtonControl getRightTrigger { get => rightTrigger; }
    public ButtonControl getLeftTrigger { get => leftTrigger; }
    public StickControl getLeftStick { get => leftStick; }
    public StickControl getRightStick { get => rightStick; }
    public ButtonControl getAccelButton { get => accelButton; }
    public ButtonControl getBrakeButton { get => brakeButton; }
    public ButtonControl getLeftButton { get => leftButton; }
    public ButtonControl getRightButton { get => rightButton; }
    public ButtonControl getPauseButton { get => pauseButton; }

}
public static class InputPreset
{
    public static CustomPlayerInput GetDefaultGamepadPreset (Gamepad _desiredGamePad)
    {
        return new CustomPlayerInput(
            _desiredGamePad.aButton,_desiredGamePad.leftShoulder,_desiredGamePad.rightShoulder,
            _desiredGamePad.bButton,_desiredGamePad.rightTrigger,_desiredGamePad.leftTrigger,
            _desiredGamePad.leftStick,_desiredGamePad.rightStick,InputType.GAMEPAD
            );
    }
    public static CustomPlayerInput GetDefaultKeyboardPreset (Keyboard _desiredKeyboard)
    {
        return new CustomPlayerInput(
            _desiredKeyboard.spaceKey, _desiredKeyboard.qKey, _desiredKeyboard.eKey, _desiredKeyboard.leftShiftKey, 
            _desiredKeyboard.wKey, _desiredKeyboard.sKey,_desiredKeyboard.aKey,_desiredKeyboard.dKey,
            _desiredKeyboard.escapeKey,InputType.KEYBOARD
            );
    }
}


