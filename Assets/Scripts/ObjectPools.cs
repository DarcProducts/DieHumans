using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPools : MonoBehaviour
{
    public static UnityAction<Vector3> ExplosionSound;

    [Header("Building Stuff")]
    [SerializeField] private GameObject brokenBuilding;

    [SerializeField] private int brokenBuildingInitialPoolSize;
    [SerializeField] private GameObject brokenBuildingEffect;
    [SerializeField] private int brokenEffectInitialPoolSize;
    [SerializeField] private GameObject collapseEffect;
    [SerializeField] private int collapseEffectInitialPoolSize;
    private readonly List<GameObject> brokenBuildingPool = new List<GameObject>();
    private readonly List<GameObject> brokenEffectPool = new List<GameObject>();
    private readonly List<GameObject> collapseEffectPool = new List<GameObject>();

    [Header("Drone Stuff")]
    [SerializeField] private GameObject drone;

    [SerializeField] private int droneInitialPoolSize;
    private readonly List<GameObject> dronePool = new List<GameObject>();

    [Header("Rocket Stuff")]
    [Tooltip("Rocket firing method called from this script")]
    [SerializeField] private GameObject rocket;

    [SerializeField] private GameObject explosionEffect;
    private readonly List<GameObject> explosionEffectPool = new List<GameObject>();
    private readonly List<GameObject> rocketGameObjectPool = new List<GameObject>();
    private int explosionPoolInitialSize;
    private int rocketPoolInitialSize;

    [Header("Meteor Stuff")]
    [SerializeField] private GameObject meteor;

    [SerializeField] private GameObject brokenMeteor;
    [SerializeField] private int meteorInitialPoolSize;
    private readonly List<GameObject> meteorPool = new List<GameObject>();

    private void Start()
    {
        InitializeBrokenBuildingPool();
        InitializeBrokenEffectPool();
        InitializeCollapseEffectPool();
        InitializeExplosionPool();
        InitializeRocketPool();
        InitializeChopperPool();
        InitializeMeteorPool();
    }

    private void OnEnable()
    {
        Building.BreakBuilding += BreakBuilding;
        Rocket.ExplodeRocket += Explode;
        Meteor.MeteorExploded += Explode;
    }

    private void OnDisable()
    {
        Building.BreakBuilding -= BreakBuilding;
        Rocket.ExplodeRocket -= Explode;
        Meteor.MeteorExploded -= Explode;
    }

    public void Explode(Vector3 position, float size)
    {
        ExplosionSound?.Invoke(position);
        GameObject explosion = GetAvailableExplosion();
        if (explosion != null)
        {
            explosion.transform.localScale = new Vector3(size * .5f, size * .5f, size * .5f);
            explosion.transform.position = position;
            explosion.SetActive(true);
        }
    }

    public void BreakBuilding(GameObject previousBuilding)
    {
        GameObject broken = GetBrokenBuilding();
        broken.transform.SetPositionAndRotation(previousBuilding.transform.position, previousBuilding.transform.rotation);
        broken.transform.localScale = previousBuilding.transform.localScale;
        broken.SetActive(true);
    }

    public void InitializeBrokenEffectPool()
    {
        brokenEffectPool.Clear();
        if (brokenBuildingEffect != null)
        {
            for (int i = 0; i < brokenEffectInitialPoolSize; i++)
            {
                GameObject e = Instantiate(brokenBuildingEffect, Vector3.down, Quaternion.identity, transform);
                e.SetActive(false);
                brokenEffectPool.Add(e);
            }
        }
    }

    public void InitializeCollapseEffectPool()
    {
        collapseEffectPool.Clear();
        if (collapseEffect != null)
        {
            for (int i = 0; i < collapseEffectInitialPoolSize; i++)
            {
                GameObject c = Instantiate(collapseEffect, Vector3.down, Quaternion.identity, transform);
                c.SetActive(false);
                collapseEffectPool.Add(c);
            }
        }
    }

    public void InitializeBrokenBuildingPool()
    {
        brokenBuildingPool.Clear();
        if (brokenBuilding != null)
        {
            for (int i = 0; i < brokenBuildingInitialPoolSize; i++)
            {
                GameObject b = Instantiate(brokenBuilding, Vector3.down, Quaternion.identity, transform);
                b.SetActive(false);
                brokenBuildingPool.Add(b);
            }
        }
    }

    public GameObject GetBrokenBuilding()
    {
        for (int i = 0; i < brokenBuildingPool.Count; i++)
            if (!brokenBuildingPool[i].activeSelf)
                return brokenBuildingPool[i];
        if (brokenBuildingPool != null)
        {
            GameObject newBrokenBuilding = Instantiate(brokenBuilding, Vector3.down, Quaternion.identity, transform);
            newBrokenBuilding.SetActive(false);
            brokenEffectPool.Add(newBrokenBuilding);
            return newBrokenBuilding;
        }
        return null;
    }

    public GameObject GetBrokenEffect()
    {
        for (int i = 0; i < brokenEffectPool.Count; i++)
            if (!brokenEffectPool[i].activeSelf)
                return brokenEffectPool[i];
        if (brokenBuildingEffect != null)
        {
            GameObject newBrokenEffect = Instantiate(brokenBuildingEffect, Vector3.down, Quaternion.identity, transform);
            newBrokenEffect.SetActive(false);
            brokenEffectPool.Add(newBrokenEffect);
            return newBrokenEffect;
        }
        return null;
    }

    public GameObject GetCollapseEffect()
    {
        for (int i = 0; i < collapseEffectPool.Count; i++)
            if (!collapseEffectPool[i].activeSelf)
                return collapseEffectPool[i];
        if (collapseEffect != null)
        {
            GameObject newCollapseEffect = Instantiate(collapseEffect, Vector3.down, Quaternion.identity, transform);
            newCollapseEffect.SetActive(false);
            collapseEffectPool.Add(newCollapseEffect);
            return newCollapseEffect;
        }
        return null;
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
}