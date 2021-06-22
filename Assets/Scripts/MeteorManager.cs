using UnityEngine;

public class MeteorManager : MonoBehaviour
{
    [SerializeField] private Vector3 meteorSpawnMin;
    [SerializeField] private Vector3 meteorSpawnMax;
    [SerializeField] private bool startMeteorStorm;
    [SerializeField] private ObjectPools objectPools;
    [SerializeField] private float meteorStormLength;
    [SerializeField] private float meteorDropRate;
    [SerializeField] private float meteorSpeed;
    private float currentDrop;

    private void Start()
    {
        currentDrop = meteorDropRate;
    }

    private void FixedUpdate()
    {
        if (startMeteorStorm)
            StartMeteorStorm();
    }

    public void StartMeteorStorm()
    {
        currentDrop = currentDrop < 0 ? 0 : currentDrop -= Time.fixedDeltaTime;
        if (currentDrop == 0 && objectPools != null)
        {
            GameObject meteor = objectPools.GetAvailableMeteor();
            meteor.GetComponent<Meteor>().SetMeteorSpeed(meteorSpeed);
            meteor.transform.position = new Vector3(Random.Range(meteorSpawnMin.x, meteorSpawnMax.x), Random.Range(meteorSpawnMin.y, meteorSpawnMax.y), Random.Range(meteorSpawnMin.z, meteorSpawnMax.z));
            meteor.SetActive(true);
            currentDrop = meteorDropRate;
        }
    }
}