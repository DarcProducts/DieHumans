using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Mods/New Vector2 Increase Mod")]
public class Vector2IncreaseMod : Mod
{
    public Vector2Variable valueToApply;
    public Vector2Variable targetVariable;
    public override void ChangeValue()
    {
        if (valueToApply != null && targetVariable != null)
            targetVariable.Value += valueToApply.Value;
    }
}
