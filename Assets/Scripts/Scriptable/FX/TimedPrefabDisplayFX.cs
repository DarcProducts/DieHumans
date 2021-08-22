using UnityEngine;
[CreateAssetMenu(menuName = "FX/New Prefab Display FX")]
public class TimedPrefabDisplayFX : FX
{
    public ObjectPooler objectPooler;
    public Vector3 scale;

    public override void PlayFX(Vector3 location)
    {
        if (objectPooler != null)
        {
            GameObject newObject = objectPooler.GetObject();
            if (newObject != null)
            {
                newObject.transform.localScale = scale;
                newObject.transform.position = location;
                newObject.SetActive(true);
            }
        }
    }
}
