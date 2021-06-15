using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private bool debug;

    [Header("Game Stats")]
    [SerializeField] private byte threatLevel;

    [SerializeField] private int currentWave;

    [Header("Rocket Stuff")]
    [Tooltip("Rocket firing method called from this script")]
    [SerializeField] private GameObject rocket;

    [SerializeField] private float rocketDamage;
    [SerializeField] private float rocketThrust;
    [SerializeField] private GameObject explosionEffect;
    private CityGenerator cityGenerator;
    private GameObject playerShip;
    private readonly List<GameObject> explosionEffectPool = new List<GameObject>();
    private readonly List<GameObject> rocketGameObjectPool = new List<GameObject>();
    private int explosionPoolInitialSize;
    private int rocketPoolInitialSize;

    [Header("Chopper Stuff")]
    [SerializeField] private GameObject chopper;

    [SerializeField] private int chopperInitialPoolSize;
    private readonly List<GameObject> chopperPool = new List<GameObject>();

    private void Awake()
    {
        cityGenerator = FindObjectOfType<CityGenerator>();
        explosionPoolInitialSize = currentWave * 10;
        rocketPoolInitialSize = currentWave * 10;
    }

    private void Start()
    {
        if (playerShip == null)
            playerShip = GameObject.FindWithTag("PlayerShip");
        if (cityGenerator == null)
            Debug.LogError("No City Generator found in scene! Cannot get city bounds for Enemy Manager.");
        InitializeExplosionPool();
        InitializeRocketPool();
        InitializeChopperPool();
    }

    private void OnEnable()
    {
        Rocket.ExplodeRocket += Explode;
        Chopper.FiredRocket += LaunchRocket;
        Rocket.RocketHit += TryDamagingTargetRocket;
    }

    private void OnDisable()
    {
        Rocket.ExplodeRocket -= Explode;
        Chopper.FiredRocket -= LaunchRocket;
        Rocket.RocketHit -= TryDamagingTargetRocket;
    }

    public void InitializeChopperPool()
    {
        chopperPool.Clear();
        if (chopper != null)
        {
            for (int i = 0; i < chopperInitialPoolSize; i++)
            {
                GameObject c = Instantiate(chopper, Vector3.down, Quaternion.identity, transform);
                c.SetActive(false);
                chopperPool.Add(c);
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

    public Vector3 PickTargetLocationWithinCity(float maxWanderHeight)
    {
        if (cityGenerator != null)
        {
            Vector3[] cityBounds = GetCityBounds();
            return new Vector3(Random.Range(cityBounds[0].x, cityBounds[0].z), Random.Range(cityGenerator.GetMaxBuildingHeight() + cityGenerator.GetGridSize() + 2, cityGenerator.GetMaxBuildingHeight() + cityGenerator.GetGridSize() + maxWanderHeight), Random.Range(cityBounds[1].x, cityBounds[1].z));
        }
        return Vector3.zero;
    }

    private GameObject GetAvailableRocket()
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

    private GameObject GetAvailableExplosion()
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

    public void Explode(Vector3 position)
    {
        GameObject explosion = GetAvailableExplosion();
        if (explosion != null)
        {
            explosion.transform.position = position;
            explosion.SetActive(true);
        }
    }

    public Vector3[] GetCityBounds()
    {
        if (cityGenerator != null)
            return cityGenerator.GetCityBounds();
        return new Vector3[2];
    }

    public bool CheckPlayerWithinRange(GameObject target, float range)
    {
        if (playerShip != null)
        {
            if (Vector3.Distance(target.transform.position, playerShip.transform.position) < range)
                return true;
        }
        return false;
    }

    public bool CheckIfPathClear(GameObject target, float distance)
    {
        if (playerShip != null)
        {
            if (Physics.Raycast(target.transform.position + Vector3.down, playerShip.transform.position + Vector3.up - target.transform.position, out RaycastHit hit, distance))
            {
                if (debug)
                    Debug.DrawRay(target.transform.position + Vector3.down, playerShip.transform.position + Vector3.up - target.transform.position, Color.red, distance);
                if (hit.collider.CompareTag("PlayerShip"))
                    return true;
            }
        }
        return false;
    }

    private void TryDamagingTargetRocket(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(rocketDamage);
    }

    public void LaunchRocket(Vector3 firedFrom, bool isHoming)
    {
        if (playerShip != null && rocket != null)
        {
            Vector3 direction = playerShip.transform.position + Vector3.up - firedFrom + Vector3.down;
            GameObject r = GetAvailableRocket();
            r.transform.position = firedFrom + Vector3.down * .5f;
            Rocket rocket = r.GetComponent<Rocket>();
            r.SetActive(true);
            if (r != null)
            {
                if (!isHoming)
                {
                    rocket.isHoming = false;
                    r.GetComponent<Rigidbody>().AddForce(direction.normalized * rocketThrust, ForceMode.Impulse);
                }
                else
                    rocket.isHoming = true;
            }
        }
    }

    public GameObject GetPlayerShip() => playerShip;

    public byte GetThreatLevel() => threatLevel;

    public int GetCurrentWave() => currentWave;
}