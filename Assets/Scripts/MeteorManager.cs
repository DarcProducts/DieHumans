using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeteorManager : MonoBehaviour
{
    [SerializeField] private bool startMeteorStorm;
    [SerializeField] private Vector3 meteorSpawnMin;
    [SerializeField] private Vector3 meteorSpawnMax;
    [SerializeField] private GameObject meteor;
    [SerializeField] private GameObject brokenMeteor;
    [SerializeField] private int meteorInitialPoolSize;
    [SerializeField] private float meteorStormLength;
    [SerializeField] private float meteorDropRate;
    private float currentDrop;
    private readonly List<GameObject> meteorPool = new List<GameObject>();

    private void Start()
    {
        currentDrop = meteorDropRate;
        InitializeMeteorPool();
    }

    private void FixedUpdate()
    {
        if (startMeteorStorm)
            StartMeteorStorm();
    }

    private void InitializeMeteorPool()
    {
        if (meteor != null)
        {
            for (int i = 0; i < meteorInitialPoolSize; i++)
            {
                GameObject m = Instantiate(meteor, Vector3.down, Quaternion.identity, transform);
                m.transform.position = Vector3.down;
                m.SetActive(false);
                meteorPool.Add(m);
            }
        }
    }

    public GameObject GetAvailableMeteor()
    {
        for (int i = 0; i < meteorPool.Count; i++)
        {
            if (!meteorPool[i].activeSelf)
                return meteorPool[i];
        }
        if (meteor != null)
        {
            GameObject newMeteor = Instantiate(meteor, Vector3.down, Quaternion.identity, transform);
            newMeteor.transform.position = Vector3.down;
            newMeteor.SetActive(false);
            meteorPool.Add(newMeteor);
            return newMeteor;
        }
        return null;
    }

    public void StartMeteorStorm()
    {
        currentDrop = currentDrop < 0 ? 0 : currentDrop -= Time.fixedDeltaTime;
        if (currentDrop == 0)
        {
            GameObject meteor = GetAvailableMeteor();
            meteor.transform.position = new Vector3(Random.Range(meteorSpawnMin.x, meteorSpawnMax.x), Random.Range(meteorSpawnMin.y, meteorSpawnMax.y), Random.Range(meteorSpawnMin.z, meteorSpawnMax.z));
            meteor.SetActive(true);
            currentDrop = meteorDropRate;
        }
    }
}
