using UnityEngine;
[CreateAssetMenu(menuName = "Mods/New Float Increase Mod")]
public class FloatIncreaseMod : Mod
{
    public FloatVariable valueToApply;
    public FloatVariable targetVariable;
    public override void ChangeValue()
    {
        if (valueToApply != null && targetVariable != null)
            targetVariable.Value += Mathf.Abs(valueToApply.Value);
    }
}
