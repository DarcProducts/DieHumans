using UnityEngine;

public class MeteorManager : MonoBehaviour
{
    [SerializeField] Vector3 meteorSpawnMin;
    [SerializeField] Vector3 meteorSpawnMax;
    [SerializeField] bool startMeteorStorm;
    [SerializeField] ObjectPools objectPools;
    [SerializeField] float meteorDropRate;
    [SerializeField] float meteorSpeed;
    [Range(0f, 1f)] [SerializeField] float boxSpawnRate;
    float currentDrop;

    void Start()
    {
        currentDrop = meteorDropRate;
    }

    void FixedUpdate()
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
            if (meteor != null)
            {
                meteor.GetComponent<Meteor>().SetMeteorSpeed(meteorSpeed);
                Vector3 ranPos = new Vector3(Random.Range(meteorSpawnMin.x, meteorSpawnMax.x), Random.Range(meteorSpawnMin.y, meteorSpawnMax.y), Random.Range(meteorSpawnMin.z, meteorSpawnMax.z));
                meteor.transform.position = ranPos;
                
                if (Random.value < boxSpawnRate)
                {
                    GameObject box = objectPools.GetDropBox();
                    box.transform.position = ranPos;
                    box.SetActive(true);
                } 
                else
                    meteor.SetActive(true);
            }
            currentDrop = meteorDropRate;
        }
    }
}