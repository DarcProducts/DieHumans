using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Variables/New Float Variable")]
public class FloatVariable : ScriptableObject
{
    public UnityAction<float> OnValueChanged;
    [SerializeField] float value;
    [SerializeField] bool resetOnEnable;
    [SerializeField] bool resetOnDisable;
    public bool broadcastActionOnValueChange = false;

    void OnEnable()
    {
        if (resetOnEnable)
            value = 0;
    }

    void OnDisable()
    {
        if (resetOnDisable)
            value = 0;
    }

    public float Value
    {
        get { return value; }
        set
        {
            this.value = value;
            if (broadcastActionOnValueChange)
                OnValueChanged?.Invoke(value);
        }
    }
}