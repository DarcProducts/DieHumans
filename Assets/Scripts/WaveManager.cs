using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static UnityAction<int> UpdateWave;
    public static UnityAction WaveStarted;
    public static UnityAction WaveCompleted;
    ObjectPools objectPools;

    [Header("Game Stats")]
    [SerializeField] bool startWaves;
    bool canSpawnWave = true;
    bool isRunning = false;
    [SerializeField] int currentWave = 0;
    [SerializeField] float waveDelay = 1f;
    [SerializeField] float spawnDelay = 1f;
    [SerializeField] float continuousWaveDelay;
    bool sentWaveBegun = false;

    [Header("Spawn Areas")]
    [SerializeField] Vector3[] minSpawnAreas;

    [SerializeField] Vector3[] maxSpawnAreas;

    readonly List<GameObject> waveObjects = new List<GameObject>();

    void Awake() => objectPools = FindObjectOfType<ObjectPools>();

    void Start() => StartWaves();

    void OnEnable()
    {
        Drone.UpdateDrone += RemoveWaveObject;
        Bomber.UpdateBomber += RemoveWaveObject;
        Tank.UpdateTank += RemoveWaveObject;
    }

    void OnDisable()
    {
        Drone.UpdateDrone -= RemoveWaveObject;
        Bomber.UpdateBomber -= RemoveWaveObject;
        Tank.UpdateTank += RemoveWaveObject;
        StopAllCoroutines();
    }

    void LateUpdate()
    {
        if (startWaves)
        {
            if (canSpawnWave)
            {
                StartCoroutine(nameof(StartWaves));
                canSpawnWave = false;
            }
            else if (!canSpawnWave && !isRunning)
                StopCoroutine(nameof(StartWaves));
        }
    }

    [ContextMenu("Start Wave")]
    IEnumerator StartWaves()
    {
        if (!sentWaveBegun)
        {
            WaveStarted?.Invoke();
            sentWaveBegun = true;
        }
        currentWave++;
        isRunning = true;
        waveObjects.Clear();
        yield return new WaitForSeconds(waveDelay);
        do
        {
            if (objectPools != null)
            {
                int ranSpawn = Random.Range(0, maxSpawnAreas.Length);
                Vector3 newLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), Random.Range(minSpawnAreas[ranSpawn].y, maxSpawnAreas[ranSpawn].y), Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));
                Vector3 tankLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), 50, Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));

                float ranVal = Random.value;

                yield return new WaitForSeconds(spawnDelay);
                if (ranVal > .9f)
                {
                    GameObject b = objectPools.GetBomber();
                    b.transform.position = newLoc;
                    waveObjects.Add(b);
                    b.SetActive(true);
                }
                else if (ranVal <= .9f && ranVal > .7f)
                {
                    GameObject t = objectPools.GetTank();
                    t.transform.position = tankLoc;
                    waveObjects.Add(t);
                    t.SetActive(true);
                }
                else
                {
                    GameObject d = objectPools.GetDrone();
                    d.transform.position = newLoc;
                    waveObjects.Add(d);
                    d.SetActive(true);
                }
            }
        }
        while (waveObjects.Count < currentWave * currentWave);
        waveDelay++;
        UpdateWave?.Invoke(currentWave);
        if (continuousWaveDelay > 0)
        {
            yield return new WaitForSeconds(continuousWaveDelay);
            continuousWaveDelay = waveObjects.Count;
            StartCoroutine(StartWaves());
        }
    }

    void RemoveWaveObject(GameObject target)
    {
        if (waveObjects.Count > 0 && target != null)
        {
            if (waveObjects.Remove(target))
                Debug.Log($"Removed {target.name}");
            if (waveObjects.Count == 0)
                WaveFinished();
        }
    }

    void WaveFinished()
    {
        Debug.Log($"Wave Completed!");
        isRunning = false;
        canSpawnWave = true;
        sentWaveBegun = false;
        WaveCompleted?.Invoke();
    }
}
