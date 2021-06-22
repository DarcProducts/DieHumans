using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static UnityAction WaveComplete;
    [SerializeField] private ObjectPools objectPools;

    [Header("Game Stats")]
    public int viewableCurrentWave = 1;

    public static int currentWave = 1;

    [SerializeField] private int waveCountMultiplier;

    private readonly List<GameObject> waveObjects = new List<GameObject>();

    private void OnEnable()
    {
        WaveComplete += WaveCompleted;
    }

    private void OnDisable()
    {
        WaveComplete -= WaveCompleted;
    }

    [ContextMenu("Start Wave")]
    public void StartCurrentWave()
    {
        waveObjects.Clear();
        do
        {
            GameObject d = objectPools.GetAvailableDrone();
            d.transform.position = d.GetComponent<EnemyAI>().PickTargetLocationWithinCity(d.GetComponent<Drone>().GetMaxWanderHeight());
            waveObjects.Add(d);
            d.SetActive(true);
            Debug.Log($"Activated: {d.name}");
        }
        while (waveObjects.Count < currentWave * waveCountMultiplier);
        currentWave++;
        viewableCurrentWave++;
    }

    private void RemoveNonActive()
    {
        if (waveObjects.Count > 0)
            for (int i = 0; i < waveObjects.Count; i++)
                if (!waveObjects[i].activeSelf)
                {
                    waveObjects.Remove(waveObjects[i]);
                    Debug.Log($"Removed: {waveObjects[i].name}");
                    RemoveNonActive();
                }
    }

    public void WaveCompleted()
    {
        Debug.Log($"Wave Completed!");
    }

    public int GetCurrentWave() => currentWave;
}