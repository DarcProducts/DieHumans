using UnityEngine;

public class CreateObject : MonoBehaviour
{
    [SerializeField] ObjectPooler objectToSpawn;
    [SerializeField] float newScale;
    [Header("Scaling")]
    [SerializeField] bool usedDynamicScaling;
    [SerializeField] float dynamicScalingMultiplier;

    public void CreateNewObject(GameObject objLoc)
    {
        GameObject o = objectToSpawn.GetObject();
        if (o == null) return;
        o.transform.SetPositionAndRotation(objLoc.transform.position, Quaternion.Euler(0, 0, Random.Range(-180f, 180f)));
        if (usedDynamicScaling)
            o.transform.localScale = objLoc.transform.localScale * dynamicScalingMultiplier;
        else
            o.transform.localScale *= newScale;
        o.SetActive(true);
    }
}