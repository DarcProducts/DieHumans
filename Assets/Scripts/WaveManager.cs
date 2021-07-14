using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static UnityAction<int> UpdateWave;
    ObjectPools objectPools;

    [Header("Game Stats")]
    [SerializeField] bool startWaves;
    bool canSpawnWave = true;
    bool isRunning = false;
    [SerializeField] int currentWave = 0;
    [SerializeField] float waveDelay = 1f;
    EnemyManager enemyManager;

    [Header("Spawn Areas")]
    [SerializeField] Vector3[] minSpawnAreas;

    [SerializeField] Vector3[] maxSpawnAreas;

    readonly List<GameObject> waveObjects = new List<GameObject>();

    void Awake()
    {
        objectPools = FindObjectOfType<ObjectPools>();
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager.Equals(null))
            Debug.LogWarning($"Cound not locate a GameObject with an EnemyManager component attached");
        if (objectPools.Equals(null))
            Debug.LogWarning($"Cound not locate a GameObject with an ObjectPools component attached");
    }

    void Start() => StartWaves();

    void OnEnable()
    {
        SimpleDrone.UpdateDroneCount += RemoveWaveObject;
        Bomber.UpdateBomber += RemoveWaveObject;
        Tank.UpdateTank += RemoveWaveObject;
    }

    void OnDisable()
    {
        SimpleDrone.UpdateDroneCount -= RemoveWaveObject;
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
        currentWave++;
        isRunning = true;
        waveObjects.Clear();
        yield return new WaitForSeconds(waveDelay);
        do
        {
            if (objectPools != null && enemyManager != null)
            {
                int ranSpawn = Random.Range(0, maxSpawnAreas.Length);
                Vector3 newLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), Random.Range(minSpawnAreas[ranSpawn].y, maxSpawnAreas[ranSpawn].y), Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));

                float ranVal = Random.value;

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
                    t.transform.position = new Vector3(newLoc.x, 50, newLoc.z);
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
    }

    void RemoveWaveObject(GameObject target)
    {
        if (waveObjects.Count > 0 && target != null)
        {
            if (waveObjects.Remove(target))
                Debug.Log($"Removed {target.name}");
            if (waveObjects.Count == 0)
                WaveCompleted();
        }
    }

    void WaveCompleted()
    {
        Debug.Log($"Wave Completed!");
        isRunning = false;
        canSpawnWave = true;
    }
}
