using UnityEngine;
[CreateAssetMenu(menuName = "Mods/New Float Multiply Mod")]
public class FloatMultiplyMod : Mod
{
    public FloatVariable valueToApply;
    public FloatVariable targetVariable;
    public override void ChangeValue()
    {
        if (valueToApply != null && targetVariable != null)
            targetVariable.Value *= valueToApply.Value;
    }
}
