using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static UnityAction WaveComplete;
    public static UnityAction<int> UpdateWave;
    [SerializeField] private ObjectPools objectPools;

    [Header("Game Stats")]
    [SerializeField] private int currentWave = 0;

    [SerializeField] private int waveCountMultiplier;

    [SerializeField] private List<GameObject> waveObjects = new List<GameObject>();

    private void Start() => StartCurrentWave();

    private void OnEnable()
    {
        SimpleDrone.UpdateDroneCount += RemoveWaveObject;
    }

    private void OnDisable()
    {
        SimpleDrone.UpdateDroneCount -= RemoveWaveObject;
    }

    [ContextMenu("Start Wave")]
    public void StartCurrentWave()
    {
        waveObjects.Clear();
        do
        {
            GameObject d = objectPools.GetAvailableDrone();
            d.transform.position = new Vector3(Random.Range(0, 200), 300, Random.Range(0, 200));
            waveObjects.Add(d);
            d.GetComponent<SimpleDrone>().isActivated = true;
            d.SetActive(true);
        }
        while (waveObjects.Count < currentWave * waveCountMultiplier);
        currentWave++;
        UpdateWave?.Invoke(currentWave);
    }

    private void RemoveWaveObject(GameObject target)
    {
        if (waveObjects.Count > 0)
        {
            if (waveObjects.Remove(target))
                Debug.Log($"Removed {target.name}");
            if (waveObjects.Count == 0)
                WaveCompleted();
        }
    }

    private void WaveCompleted()
    {
        Debug.Log($"Wave Completed!");
        WaveComplete?.Invoke();
        StartCurrentWave();
    }
}