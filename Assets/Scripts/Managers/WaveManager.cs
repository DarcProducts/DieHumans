using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static UnityAction<int> UpdateWave;
    public static UnityAction WaveStarted;
    public static UnityAction WaveCompleted;
    [SerializeField] MultiPooler objectPooler;

    [Header("Game Stats")]
    [SerializeField] CityGenerator cityGenerator;

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

    void Start()
    {
        if (cityGenerator != null)
        {
            cityGenerator.CreateDarcGrid(Vector3.zero);
            if (startWaves)
                StartCoroutine(StartWaves());
        }
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        if (cityGenerator != null)
            cityGenerator.ResetDestroyedBuildingCount();
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
        if (objectPooler != null)
        {
            do
            {
                int ranSpawn = Random.Range(0, maxSpawnAreas.Length);
                Vector3 newLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), Random.Range(minSpawnAreas[ranSpawn].y, maxSpawnAreas[ranSpawn].y), Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));
                Vector3 tankLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), 50, Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));

                float ranVal = Random.value;

                yield return new WaitForSeconds(spawnDelay);
                if (ranVal > .9f)
                {
                    GameObject b = objectPooler.GetObject(7);
                    if (b != null)
                    {
                        b.transform.position = newLoc;
                        waveObjects.Add(b);
                        b.SetActive(true);
                    }
                }
                else if (ranVal <= .9f && ranVal > .7f)
                {
                    GameObject t = objectPooler.GetObject(6);
                    if (t != null)
                    {
                        t.transform.position = tankLoc;
                        waveObjects.Add(t);
                        t.SetActive(true);
                    }
                }
                else
                {
                    GameObject d = objectPooler.GetObject(5);
                    if (d != null)
                    {
                        d.transform.position = newLoc;
                        waveObjects.Add(d);
                        d.SetActive(true);
                    }
                }
            }
            while (waveObjects.Count < currentWave * currentWave);
        }
        waveDelay++;
        UpdateWave?.Invoke(currentWave);
        if (continuousWaveDelay > 0)
        {
            yield return new WaitForSeconds(continuousWaveDelay);
            continuousWaveDelay = waveObjects.Count;
            StartCoroutine(StartWaves());
        }
    }

    public void RemoveWaveObject(GameObject target)
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