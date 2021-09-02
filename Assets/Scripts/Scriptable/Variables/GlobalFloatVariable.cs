using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Global Variables/New Float Variable")]
public class GlobalFloatVariable : ScriptableObject
{
    [Tooltip("Invokes UnityAction and returns new value")] public UnityAction<float> OnValueChanged;
    [Tooltip("Set to true to broadcast UnityActions on values changed")] [SerializeField] bool broadcast;
    [SerializeField] bool resetValue;
    [Tooltip("If reset true")] [SerializeField] bool resetOnEnableElseDisable;

    float _v;

    public float Value
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
            _v = 0;
    }

    void OnDisable()
    {
        if (resetValue && !resetOnEnableElseDisable)
            _v = 0;
    }
}
