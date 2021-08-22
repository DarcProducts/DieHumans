using UnityEngine;
[CreateAssetMenu(menuName = "Mods/New Bool Switch Mod")]
public class BoolSwitchMod : Mod
{
    public BoolVariable targetVariable;
    public override void ChangeValue()
    {
        if (targetVariable != null)
            targetVariable.Value = !targetVariable.Value;
    }
}
