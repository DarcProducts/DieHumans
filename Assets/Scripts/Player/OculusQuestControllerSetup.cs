using UnityEngine;
using UnityEngine.InputSystem;

public class OculusQuestControllerSetup : MonoBehaviour
{
    [Header("Left Hand")]
    public GlobalVector2Variable leftThumbstick;

    public GlobalBoolVariable leftHandTriggerActive;
    public GlobalBoolVariable leftHandGripActive;
    public bool xButtonActivated;
    public bool yButtonActivated;
    public GlobalBoolVariable menuButtonActivated;

    [Header("Right Hand")]
    public GlobalVector2Variable rightThumbstick;

    public GlobalBoolVariable rightHandTriggerActive;
    public GlobalBoolVariable rightHandGripActive;
    public bool aButtonActivated;
    public bool bButtonActivated;

    [Header("Left Hand Action References")]
    [SerializeField] InputActionReference leftHandThumbstick;

    [SerializeField] InputActionReference leftHandTriggerAction;
    [SerializeField] InputActionReference leftHandGripAction;
    [SerializeField] InputActionReference xButtonAction;
    [SerializeField] InputActionReference yButtonAction;
    public InputActionReference menuButton;

    [Header("Right Hand Action References")]
    [SerializeField] InputActionReference rightHandThumbstick;

    [SerializeField] InputActionReference rightHandTriggerAction;
    [SerializeField] InputActionReference rightHandGripAction;
    [SerializeField] InputActionReference aButtonAction;
    [SerializeField] InputActionReference bButtonAction;

    void OnEnable()
    {
        leftHandThumbstick.action.performed += LeftHandThumbPress;
        leftHandTriggerAction.action.performed += LeftTriggerPress;
        leftHandGripAction.action.performed += LeftGripPress;
        leftHandTriggerAction.action.canceled += LeftTriggerPress;
        leftHandGripAction.action.canceled += LeftGripPress;
        rightHandThumbstick.action.performed += RightHandThumbstick;
        rightHandTriggerAction.action.performed += RightTriggerPress;
        rightHandGripAction.action.performed += RightGripPress;
        rightHandTriggerAction.action.canceled += RightTriggerPress;
        rightHandGripAction.action.canceled += RightGripPress;
        aButtonAction.action.performed += AButtonPress;
        aButtonAction.action.canceled += AButtonPress;
        bButtonAction.action.performed += BButtonPress;
        bButtonAction.action.canceled += BButtonPress;
        xButtonAction.action.performed += XButtonPress;
        xButtonAction.action.canceled += XButtonPress;
        yButtonAction.action.performed += YButtonPress;
        yButtonAction.action.canceled += YButtonPress;
        menuButton.action.performed += MenuButtonPressed;
        menuButton.action.canceled += MenuButtonPressed;
    }

    void OnDisable()
    {
        leftHandThumbstick.action.performed -= LeftHandThumbPress;
        leftHandTriggerAction.action.performed -= LeftTriggerPress;
        leftHandGripAction.action.performed -= LeftGripPress;
        leftHandTriggerAction.action.canceled -= LeftTriggerPress;
        leftHandGripAction.action.canceled -= LeftGripPress;
        rightHandThumbstick.action.performed -= RightHandThumbstick;
        rightHandTriggerAction.action.performed -= RightTriggerPress;
        rightHandGripAction.action.performed -= RightGripPress;
        rightHandTriggerAction.action.canceled -= RightTriggerPress;
        rightHandGripAction.action.canceled -= RightGripPress;
        aButtonAction.action.performed -= AButtonPress;
        aButtonAction.action.canceled -= AButtonPress;
        bButtonAction.action.performed -= BButtonPress;
        bButtonAction.action.canceled -= BButtonPress;
        xButtonAction.action.performed -= XButtonPress;
        xButtonAction.action.canceled -= XButtonPress;
        yButtonAction.action.performed -= YButtonPress;
        yButtonAction.action.canceled -= YButtonPress;
        menuButton.action.performed -= MenuButtonPressed;
        menuButton.action.canceled -= MenuButtonPressed;
    }

    void MenuButtonPressed(InputAction.CallbackContext obj) => menuButtonActivated.Value = obj.ReadValueAsButton();


    void XButtonPress(InputAction.CallbackContext obj) => xButtonActivated = obj.ReadValueAsButton();

    void YButtonPress(InputAction.CallbackContext obj) => yButtonActivated = obj.ReadValueAsButton();


    void AButtonPress(InputAction.CallbackContext obj) => aButtonActivated = obj.ReadValueAsButton();


    void BButtonPress(InputAction.CallbackContext obj) => bButtonActivated = obj.ReadValueAsButton();


    void LeftHandThumbPress(InputAction.CallbackContext obj) => leftThumbstick.Value = obj.ReadValue<Vector2>();

    void RightHandThumbstick(InputAction.CallbackContext obj) => rightThumbstick.Value = obj.ReadValue<Vector2>();

    void LeftTriggerPress(InputAction.CallbackContext obj) => leftHandTriggerActive.Value = obj.ReadValueAsButton();

    void LeftGripPress(InputAction.CallbackContext obj) => leftHandGripActive.Value = obj.ReadValueAsButton();

    void RightTriggerPress(InputAction.CallbackContext obj) => rightHandTriggerActive.Value = obj.ReadValueAsButton();

    void RightGripPress(InputAction.CallbackContext obj) => rightHandGripActive.Value = obj.ReadValueAsButton();

}