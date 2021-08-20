using UnityEngine;
[CreateAssetMenu(menuName = "Data/New Kill Sheet")]
public class KillSheet : ScriptableObject
{
    public IntVariable dronesDestroyed;
    public IntVariable tanksDestroyed;
    public IntVariable bombersDestroyed;
}