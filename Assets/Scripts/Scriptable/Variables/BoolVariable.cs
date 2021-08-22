using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Variables/New Bool Variable")]
public class BoolVariable : ScriptableObject
{
    [SerializeField] bool currentValue;
    public UnityAction<bool> OnValueChanged;
    public UnityAction ValueChangeTrue;
    public UnityAction ValueChangeFalse;
    [SerializeField] bool resetOnEnable;
    [SerializeField] bool resetOnDisable;
    [SerializeField] bool resetToTrueElseFalse;
    [SerializeField] bool broadcastActionOnValueChange = false;
    bool value;

    void OnEnable()
    {
        if (resetOnEnable)
            if (resetToTrueElseFalse)
                value = true;
            else
                value = false;
    }

    void OnDisable()
    {
        if (resetOnDisable)
            if (resetToTrueElseFalse)
                value = true;
            else
                value = false;
    }

    public bool Value 
    {
        get { return value; } 
        set
        {
            currentValue = value;
            this.value = value;
            if (broadcastActionOnValueChange)
                OnValueChanged?.Invoke(value);
            if (value.Equals(true))
                ValueChangeTrue?.Invoke();
            else
                ValueChangeFalse?.Invoke();
        }
    }
}
