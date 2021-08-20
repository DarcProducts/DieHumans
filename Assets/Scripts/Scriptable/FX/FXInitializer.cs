using UnityEngine;
[CreateAssetMenu(menuName = "FX/New FX Initializer")]
public class FXInitializer : ScriptableObject
{
    public FX[] allFX = new FX[0];
    public void PlayAllFX(Vector3 location) { for (int i = 0; i < allFX.Length; i++) if (allFX[i] != null) allFX[i].PlayFX(location); }
}
