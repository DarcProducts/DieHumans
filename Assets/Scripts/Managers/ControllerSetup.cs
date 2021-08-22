using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class ControllerSetup : MonoBehaviour
{
    [Header("Left Hand")]
    public Vector2Variable leftThumbstick;
    public BoolVariable leftHandTriggerActive;
    public BoolVariable leftHandGripActive;
    public BoolVariable xButtonActivated;
    public BoolVariable yButtonActivated;
    [Space(10)]
    [SerializeField] InputActionReference leftHandThumbstick;
    [SerializeField] InputActionReference leftHandTriggerAction;
    [SerializeField] InputActionReference leftHandGripAction;
    [Header("Right Hand")]
    public Vector2Variable rightThumbstick;
    public BoolVariable rightHandTriggerActive;
    public BoolVariable rightHandGripActive;
    public BoolVariable aButtonActivated;
    public BoolVariable bButtonActivated;
    [Space(10)]
    [SerializeField] InputActionReference rightHandThumbstick; 
    [SerializeField] InputActionReference rightHandTriggerAction;
    [SerializeField] InputActionReference rightHandGripAction;

    void OnEnable()
    {
        leftHandThumbstick.action.performed += LeftHandThumbPress;
        leftHandTriggerAction.action.performed += LeftTriggerPress;
        leftHandGripAction.action.performed += LeftGripPress;
        leftHandTriggerAction.action.canceled += StopLeftTriggerPress;
        leftHandGripAction.action.canceled += StopLeftGripPress;
        rightHandThumbstick.action.performed += RightHandThumbstick;
        rightHandTriggerAction.action.performed += RightTriggerPress;
        rightHandGripAction.action.performed += RightGripPress;
        rightHandTriggerAction.action.canceled += StopRightTriggerPress;
        rightHandGripAction.action.canceled += StopRightGripPress;
    }

    void OnDisable()
    {
        leftHandThumbstick.action.performed -= LeftHandThumbPress;
        leftHandTriggerAction.action.performed -= LeftTriggerPress;
        leftHandGripAction.action.performed -= LeftGripPress;
        leftHandTriggerAction.action.canceled -= StopLeftTriggerPress;
        leftHandGripAction.action.canceled -= StopLeftGripPress;
        rightHandThumbstick.action.performed -= RightHandThumbstick;
        rightHandTriggerAction.action.performed -= RightTriggerPress;
        rightHandGripAction.action.performed -= RightGripPress;
        rightHandTriggerAction.action.canceled -= StopRightTriggerPress;
        rightHandGripAction.action.canceled -= StopRightGripPress;
    }

    void LeftHandThumbPress(InputAction.CallbackContext obj)
    {
        leftThumbstick.Value = obj.ReadValue<Vector2>();
        leftThumbstick.Value = obj.ReadValue<Vector2>();
    }
    void RightHandThumbstick(InputAction.CallbackContext obj)
    {
        rightThumbstick.Value = obj.ReadValue<Vector2>();
        rightThumbstick.Value = obj.ReadValue<Vector2>();
    }
    void LeftTriggerPress(InputAction.CallbackContext obj) => leftHandTriggerActive.Value = true;
    void StopLeftTriggerPress(InputAction.CallbackContext obj) => leftHandTriggerActive.Value = false;
    void LeftGripPress(InputAction.CallbackContext obj) => leftHandGripActive.Value = true;
    void StopLeftGripPress(InputAction.CallbackContext obj) => leftHandGripActive.Value = false;
    void RightTriggerPress(InputAction.CallbackContext obj) => rightHandTriggerActive.Value = true;
    void StopRightTriggerPress(InputAction.CallbackContext obj) => rightHandTriggerActive.Value = false;
    void RightGripPress(InputAction.CallbackContext obj) => rightHandGripActive.Value = true;
    void StopRightGripPress(InputAction.CallbackContext obj) => rightHandGripActive.Value = false;
}
