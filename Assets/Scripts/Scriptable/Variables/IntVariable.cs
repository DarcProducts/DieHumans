using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Variables/New Int Variable")]
public class IntVariable : ScriptableObject 
{
    public UnityAction<int> OnValueChanged;
    [SerializeField] int value;
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

    public int Value
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
