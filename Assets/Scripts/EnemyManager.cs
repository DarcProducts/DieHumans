using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public static UnityAction WaveComplete;

    [Header("Game Stats")]
    [SerializeField] private int currentWave = 1;

    [SerializeField] private int waveCountMultiplier;

    [Header("Rocket Stuff")]
    [Tooltip("Rocket firing method called from this script")]
    [SerializeField] private GameObject rocket;

    [SerializeField] private float rocketDamage;
    [SerializeField] private float rocketThrust;
    [SerializeField] private GameObject explosionEffect;
    private readonly List<GameObject> explosionEffectPool = new List<GameObject>();
    private readonly List<GameObject> rocketGameObjectPool = new List<GameObject>();
    private int explosionPoolInitialSize;
    private int rocketPoolInitialSize;

    [Header("Drone Stuff")]
    [SerializeField] private GameObject drone;

    [SerializeField] private int droneInitialPoolSize;
    private readonly List<GameObject> dronePool = new List<GameObject>();
    private readonly List<GameObject> waveObjects = new List<GameObject>();

    private void Awake()
    {
        explosionPoolInitialSize = currentWave * 10;
        rocketPoolInitialSize = currentWave * 10;
    }

    private void Start()
    {
        InitializeExplosionPool();
        InitializeRocketPool();
        InitializeChopperPool();
    }

    private void OnEnable()
    {
        WaveComplete += WaveCompleted;
        Rocket.ExplodeRocket += Explode;
    }

    private void OnDisable()
    {
        WaveComplete -= WaveCompleted;
        Rocket.ExplodeRocket -= Explode;
    }

    public void InitializeChopperPool()
    {
        dronePool.Clear();
        if (drone != null)
        {
            for (int i = 0; i < droneInitialPoolSize; i++)
            {
                GameObject d = Instantiate(drone, Vector3.down, Quaternion.identity, transform);
                d.SetActive(false);
                dronePool.Add(d);
            }
        }
    }

    public void InitializeExplosionPool()
    {
        explosionEffectPool.Clear();
        if (explosionEffect != null)
        {
            for (int i = 0; i < explosionPoolInitialSize; i++)
            {
                GameObject e = Instantiate(explosionEffect, Vector3.down, Quaternion.identity, transform);
                e.SetActive(false);
                explosionEffectPool.Add(e);
            }
        }
    }

    public void InitializeRocketPool()
    {
        rocketGameObjectPool.Clear();
        if (rocket != null)
        {
            for (int i = 0; i < rocketPoolInitialSize; i++)
            {
                GameObject e = Instantiate(rocket, Vector3.down, Quaternion.identity, transform);
                e.SetActive(false);
                rocketGameObjectPool.Add(e);
            }
        }
    }

    public GameObject GetAvailableRocket()
    {
        for (int i = 0; i < rocketGameObjectPool.Count; i++)
            if (!rocketGameObjectPool[i].activeSelf)
                return rocketGameObjectPool[i];
        if (rocket != null)
        {
            GameObject newRocket = Instantiate(rocket, Vector3.down, Quaternion.identity, transform);
            newRocket.SetActive(false);
            rocketGameObjectPool.Add(newRocket);
            return newRocket;
        }
        return null;
    }

    public GameObject GetAvailableExplosion()
    {
        for (int i = 0; i < explosionEffectPool.Count; i++)
            if (!explosionEffectPool[i].activeSelf)
                return explosionEffectPool[i];
        if (explosionEffect != null)
        {
            GameObject newExplosion = Instantiate(explosionEffect, Vector3.down, Quaternion.identity, transform);
            newExplosion.SetActive(false);
            explosionEffectPool.Add(newExplosion);
            return newExplosion;
        }
        return null;
    }

    public GameObject GetAvailableDrone()
    {
        for (int i = 0; i < dronePool.Count; i++)
        {
            if (!dronePool[i].activeSelf)
                return dronePool[i];
        }
        if (drone != null)
        {
            GameObject newDrone = Instantiate(drone, Vector3.down, Quaternion.identity, transform);
            newDrone.SetActive(false);
            dronePool.Add(newDrone);
            return newDrone;
        }
        return null;
    }

    [ContextMenu("Start Wave")]
    public void StartCurrentWave()
    {
        waveObjects.Clear();
        do
        {
            GameObject d = GetAvailableDrone();
            d.transform.position = d.GetComponent<EnemyAI>().PickTargetLocationWithinCity(d.GetComponent<Drone>().GetMaxWanderHeight());
            waveObjects.Add(d);
            d.SetActive(true);
            Debug.Log($"Activated: {d.name}");
        }
        while (waveObjects.Count < currentWave * waveCountMultiplier);
        currentWave++;
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

    public void Explode(Vector3 position)
    {
        GameObject explosion = GetAvailableExplosion();
        if (explosion != null)
        {
            explosion.transform.position = position;
            explosion.SetActive(true);
        }
    }

    public void WaveCompleted()
    {
        Debug.Log($"Wave Completed!");
    }

    public float GetRocketThrust() => rocketThrust;

    public int GetCurrentWave() => currentWave;
}