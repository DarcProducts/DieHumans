using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPools : MonoBehaviour
{
    public static UnityAction<Vector3> ExplosionSound;

    [Header("Building Stuff")]
    [SerializeField] readonly GameObject brokenBuilding;

    [SerializeField] GameObject brokenBuildingEffect;
    [SerializeField] GameObject collapseEffect;
    readonly List<GameObject> brokenBuildingPool = new List<GameObject>();
    readonly List<GameObject> brokenEffectPool = new List<GameObject>();
    readonly List<GameObject> collapseEffectPool = new List<GameObject>();

    [Header("Drone Stuff")]
    [SerializeField] GameObject drone;

    readonly List<GameObject> dronePool = new List<GameObject>();

    [Header("Rocket Stuff")]
    [Tooltip("Rocket firing method called from this script")]
    [SerializeField] GameObject rocket;

    [SerializeField] GameObject explosionEffect;
    readonly List<GameObject> explosionEffectPool = new List<GameObject>();
    readonly List<GameObject> rocketGameObjectPool = new List<GameObject>();

    [Header("Meteor Stuff")]
    [SerializeField] GameObject meteor;

    [SerializeField] GameObject brokenMeteor;
    readonly List<GameObject> meteorPool = new List<GameObject>();
    readonly List<GameObject> brokenMeteorPool = new List<GameObject>();

    [Header("Effects Stuff")]
    [SerializeField] GameObject infoLetters;
    readonly List<GameObject> infoLettersPool = new List<GameObject>();

    void OnEnable()
    {
        Building.BreakBuilding += BreakBuilding;
        Rocket.ExplodeRocket += Explode;
        Meteor.MeteorExploded += Explode;
        Meteor.MeteorEvaded += DestroyMeteor;
        SimpleDrone.DroneExploded += Explode;
    }

    void OnDisable()
    {
        Building.BreakBuilding -= BreakBuilding;
        Rocket.ExplodeRocket -= Explode;
        Meteor.MeteorExploded -= Explode;
        Meteor.MeteorEvaded -= DestroyMeteor;
        SimpleDrone.DroneExploded -= Explode;
    }

    private void DestroyMeteor(GameObject previousMeteor)
    {
        GameObject m = GetAvailableBrokenMeteor();
        m.transform.localScale = previousMeteor.transform.localScale;
        m.transform.localPosition = previousMeteor.transform.position;
        m.SetActive(true);
        previousMeteor.SetActive(false);
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
        GameObject broken = GetAvailableBrokenBuilding();
        broken.transform.SetPositionAndRotation(previousBuilding.transform.position, previousBuilding.transform.rotation);
        broken.transform.localScale = previousBuilding.transform.localScale;
        broken.SetActive(true);
    }

    public GameObject GetAvailableBrokenBuilding()
    {
        for (int i = 0; i < brokenBuildingPool.Count; i++)
            if (!brokenBuildingPool[i].activeSelf)
                return brokenBuildingPool[i];
        if (brokenBuilding != null)
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
            newMeteor.SetActive(false);
            meteorPool.Add(newMeteor);
            return newMeteor;
        }
        return null;
    }

    public GameObject GetAvailableBrokenMeteor()
    {
        for (int i = 0; i < brokenMeteorPool.Count; i++)
        {
            if (!brokenMeteorPool[i].activeSelf)
                return brokenMeteorPool[i];
        }
        if (meteor != null)
        {
            GameObject newBrokenMeteor = Instantiate(brokenMeteor, Vector3.down, Quaternion.identity, transform);
            newBrokenMeteor.SetActive(false);
            brokenMeteorPool.Add(newBrokenMeteor);
            return newBrokenMeteor;
        }
        return null;
    }

    public GameObject GetAvailableInfoLetters()
    {
        for (int i = 0; i < infoLettersPool.Count; i++)
        {
            if (!infoLettersPool[i].activeSelf)
                return infoLettersPool[i];
        }
        if (infoLetters != null)
        {
            GameObject newInfo = Instantiate(infoLetters, Vector3.down, Quaternion.identity, transform);
            newInfo.gameObject.SetActive(false);
            infoLettersPool.Add(newInfo);
            return newInfo;
        }
        return null;
    }
}