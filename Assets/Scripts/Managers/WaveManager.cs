using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static UnityAction WaveStarted;
    public static UnityAction WaveCompleted;
    public CanvasManager canvasManager;
    public ObjectPooler dronePooler;
    public ObjectPooler tankPooler;
    public ObjectPooler bomberPooler;
    public TMP_Text waveDisplayScreen;
    public TMP_Text currentWaveText;

    [Header("Game Stats")]
    public CityGenerator cityGenerator;

    public bool startWaves;
    bool canSpawnWave = true;
    bool isRunning = false;
    [SerializeField] int currentWave = 0;
    public float waveDelay = 1f;
    public float spawnDelay = 1f;
    bool sentWaveBegun = false;
    bool updatedWaveNumber = false;

    [Header("Spawning")]
    [SerializeField] List<Transform> spawnPoints;

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
        Drone.RemoveFromWaveList += RemoveWaveObject;
        Tank.RemoveFromWaveList += RemoveWaveObject;
        Bomber.RemoveFromWaveList += RemoveWaveObject;
    }

    void OnDisable()
    {
        if (cityGenerator != null)
            cityGenerator.ResetDestroyedBuildingCount();
        StopAllCoroutines();
        Drone.RemoveFromWaveList -= RemoveWaveObject;
        Tank.RemoveFromWaveList -= RemoveWaveObject;
        Bomber.RemoveFromWaveList -= RemoveWaveObject;
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
        if (!updatedWaveNumber)
        {
            currentWave++;
            currentWaveText.text = $"Wave {currentWave}";
            waveDisplayScreen.text = $"Wave {currentWave}";
            waveDisplayScreen.transform.parent.gameObject.SetActive(true);
            updatedWaveNumber = true;
        }
        isRunning = true;
        waveObjects.Clear();
        yield return new WaitForSeconds(waveDelay);
        do
        {
            Vector3 newLoc = Vector3.zero;
            
            if (spawnPoints.Count > 0)
            {
                int ranSpawn = Random.Range(0, spawnPoints.Count);
                newLoc = spawnPoints[ranSpawn].position;
            }

            float ranVal = Random.value;

            yield return new WaitForSeconds(spawnDelay);
            if (ranVal > .9f)
            {
                GameObject b = bomberPooler.GetObject();
                if (b != null)
                {
                    b.transform.position = newLoc + Vector3.up * 100;
                    waveObjects.Add(b);
                    b.SetActive(true);
                }
            }
            else if (ranVal <= .9f && ranVal > .7f)
            {
                GameObject t = tankPooler.GetObject();
                if (t != null)
                {
                    t.transform.position = newLoc;
                    waveObjects.Add(t);
                    t.SetActive(true);
                }
            }
            else
            {
                GameObject d = dronePooler.GetObject();
                if (d != null)
                {
                    d.transform.position = newLoc + Vector3.up * 60;
                    waveObjects.Add(d);
                    d.SetActive(true);
                }
            }
        }
        while (waveObjects.Count < currentWave * currentWave);
        waveDelay++;
        updatedWaveNumber = false;
    }

    public void RemoveWaveObject(GameObject target)
    {
        waveObjects.Remove(target);
        if (waveObjects.Count.Equals(0))
            WaveFinished();
    }

    void WaveFinished()
    {
        Debug.Log($"Wave Completed!");
        isRunning = false;
        canSpawnWave = true;
        sentWaveBegun = false;
        WaveCompleted?.Invoke();
        StartCoroutine(nameof(StartWaves));
    }
}