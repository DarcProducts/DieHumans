using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Global Variables/New Int Variable")]
public class GlobalIntVariable : ScriptableObject
{
    [Tooltip("Invokes UnityAction and returns new value")] public UnityAction<int> OnValueChanged;
    [Tooltip("Set to true to broadcast UnityActions on values changed")] [SerializeField] bool broadcast;
    [SerializeField] bool resetValue;
    [Tooltip("If reset true")] [SerializeField] bool resetOnEnableElseDisable;

    int _v;

    public int Value
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