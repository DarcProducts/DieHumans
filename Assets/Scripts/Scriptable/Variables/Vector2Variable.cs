using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Variables/New Vector2 Variable")]
public class Vector2Variable : ScriptableObject 
{
    [SerializeField] Vector2 currentValue;
    public UnityAction<Vector2> OnValueChanged;
    [SerializeField] bool resetOnEnable;
    [SerializeField] bool resetOnDisable;
    [SerializeField] bool broadcastActionOnValueChange = false;
    Vector2 value;

    void OnEnable()
    {
        if (resetOnEnable)
            value = Vector2.zero;
    }

    void OnDisable()
    {
        if (resetOnDisable)
            value = Vector2.zero;
    }

    public Vector2 Value
    {
        get { return value;  }
        set
        {
            currentValue = value;
            this.value = value; 
            if (broadcastActionOnValueChange)
                OnValueChanged?.Invoke(value);
        }
    }
}
