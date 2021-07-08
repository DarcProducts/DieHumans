using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static UnityAction WaveComplete;
    public static UnityAction<int> UpdateWave;
    ObjectPools objectPools;

    [Header("Game Stats")]
    [SerializeField] int currentWave = 0;

    [SerializeField] int waveCountMultiplier;
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

    void Start() => StartCurrentWave();

    void OnEnable()
    {
        SimpleDrone.UpdateDroneCount += RemoveWaveObject;
        Bomber.UpdateBomber += RemoveWaveObject;
    }

    void OnDisable()
    {
        SimpleDrone.UpdateDroneCount -= RemoveWaveObject;
        Bomber.UpdateBomber -= RemoveWaveObject;
    }

    [ContextMenu("Start Wave")]
    void StartCurrentWave()
    {
        waveObjects.Clear();
        do
        {
            if (objectPools != null && enemyManager != null)
            {
                int ranSpawn = Random.Range(0, maxSpawnAreas.Length);
                Vector3 newLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), Random.Range(minSpawnAreas[ranSpawn].y, maxSpawnAreas[ranSpawn].y), Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));
                
                if (Random.value > .9f)
                {
                    GameObject b = objectPools.GetBomber();
                    b.transform.position = newLoc;
                    waveObjects.Add(b);
                    b.SetActive(true);
                }
                else
                {
                    GameObject d = objectPools.GetAvailableDrone();
                    d.transform.position = newLoc;
                    waveObjects.Add(d);
                    d.SetActive(true);
                }
            }
        }
        while (waveObjects.Count < currentWave * waveCountMultiplier);
        currentWave++;
        waveDelay++;
        UpdateWave?.Invoke(currentWave);
    }

    void RemoveWaveObject(GameObject target)
    {
        if (waveObjects.Count > 0)
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
        WaveComplete?.Invoke();
        Invoke(nameof(StartCurrentWave), waveDelay);
    }
}