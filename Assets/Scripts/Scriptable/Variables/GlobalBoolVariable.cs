using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Global Variables/New Bool Variable")]
public class GlobalBoolVariable : ScriptableObject
{
    [Tooltip("Invokes UnityAction and returns new value")] public UnityAction<bool> OnValueChanged;
    [Tooltip("Set to true to broadcast UnityActions on values changed")] [SerializeField] bool broadcast;
    [SerializeField] bool resetValue;
    [SerializeField] bool resetTrueElseFalse;
    [Tooltip("If reset true")] [SerializeField] bool resetOnEnableElseDisable;

    bool _v;

    public bool Value
    {
        get { return _v; }
        set
        {
            _v = value;
            if (broadcast)
                OnValueChanged?.Invoke(_v);
        }
    }

    void OnEnable()
    {
        if (resetValue && resetOnEnableElseDisable)
        {
            if (resetTrueElseFalse)
                _v = true;
            else
                _v = false;
        }
    }

    void OnDisable()
    {
        if (resetValue && !resetOnEnableElseDisable)
        {
            if (resetTrueElseFalse)
                _v = true;
            else
                _v = false;
        }
    }
}