using UnityEngine;
[CreateAssetMenu(menuName = "Mods/New Float Decrease Mod")]
public class FloatDecreaseMod : Mod
{
    public FloatVariable valueToApply;
    public FloatVariable targetVariable;
    public override void ChangeValue()
    {
        if (valueToApply != null && targetVariable != null)
            targetVariable.value += -Mathf.Abs(valueToApply.value);
    }
}
