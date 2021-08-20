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
        leftThumbstick.value.x = obj.ReadValue<Vector2>().x;
        leftThumbstick.value.y = obj.ReadValue<Vector2>().y;
    }
    void RightHandThumbstick(InputAction.CallbackContext obj)
    {
        rightThumbstick.value.x = obj.ReadValue<Vector2>().x;
        rightThumbstick.value.y = obj.ReadValue<Vector2>().y;
    }
    void LeftTriggerPress(InputAction.CallbackContext obj) => leftHandTriggerActive.value = true;
    void StopLeftTriggerPress(InputAction.CallbackContext obj) => leftHandTriggerActive.value = false;
    void LeftGripPress(InputAction.CallbackContext obj) => leftHandGripActive.value = true;
    void StopLeftGripPress(InputAction.CallbackContext obj) => leftHandGripActive.value = false;
    void RightTriggerPress(InputAction.CallbackContext obj) => rightHandTriggerActive.value = true;
    void StopRightTriggerPress(InputAction.CallbackContext obj) => rightHandTriggerActive.value = false;
    void RightGripPress(InputAction.CallbackContext obj) => rightHandGripActive.value = true;
    void StopRightGripPress(InputAction.CallbackContext obj) => rightHandGripActive.value = false;
}
