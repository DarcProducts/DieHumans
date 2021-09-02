using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Global Variables/New Vector2 Variable")]
public class GlobalVector2Variable : ScriptableObject
{
    [Tooltip("Invokes UnityAction and returns new value")] public UnityAction<Vector2> OnValueChanged;
    [Tooltip("Set to true to broadcast UnityActions on values changed")] [SerializeField] bool broadcast;
    [SerializeField] bool resetValue;
    [Tooltip("If reset true")] [SerializeField] bool resetOnEnableElseDisable;

    Vector2 _v;

    public Vector2 Value
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
            _v = Vector2.zero;
    }

    void OnDisable()
    {
        if (resetValue && !resetOnEnableElseDisable)
            _v = Vector2.zero;
    }
}