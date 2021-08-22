using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Variables/New Byte Variable")]
public class ByteVariable : ScriptableObject
{
    public UnityAction<byte> OnValueChanged;
    byte value;
    [SerializeField] bool resetOnEnable;
    [SerializeField] bool resetOnDisable;
    [SerializeField] bool broadcastActionOnValueChange = false;

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

    public byte Value
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