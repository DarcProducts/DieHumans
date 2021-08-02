using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPools : MonoBehaviour
{
    public static UnityAction<Vector3> ExplosionSound;
    public static UnityAction DropBoxCollected;

    [Header("Building Stuff")]

    [SerializeField] GameObject brokenBuildingEffect;
    [SerializeField] GameObject collapseEffect;
    readonly List<GameObject> brokenEffectPool = new List<GameObject>();
    readonly List<GameObject> collapseEffectPool = new List<GameObject>();

    [Header("Drone Stuff")]
    [SerializeField] GameObject drone;

    readonly List<GameObject> dronePool = new List<GameObject>();
    [Header("Bomber Stuff")]
    [SerializeField] GameObject bomber;
    readonly List<GameObject> bomberPool = new List<GameObject>();
    [Header("Tank Stuff")]
    [SerializeField] GameObject tank;
    readonly List<GameObject> tankPool = new List<GameObject>();
    [Header("Rocket Stuff")]
    [SerializeField] GameObject rocket;
    readonly List<GameObject> rocketGameObjectPool = new List<GameObject>();

    [Header("Meteor Stuff")]
    [SerializeField] GameObject meteor;

    [SerializeField] GameObject brokenMeteor;
    readonly List<GameObject> meteorPool = new List<GameObject>();
    readonly List<GameObject> brokenMeteorPool = new List<GameObject>();

    [Header("Effects Stuff")]
    [SerializeField] GameObject infoLetters;
    readonly List<GameObject> infoLettersPool = new List<GameObject>();
    [SerializeField] GameObject explosionEffect;
    readonly List<GameObject> explosionEffectPool = new List<GameObject>();
    [SerializeField] GameObject explosionEffect2;
    readonly List<GameObject> explosionEffect2Pool = new List<GameObject>();
    [SerializeField] GameObject explosionEffect3;
    readonly List<GameObject> explosionEffect3Pool = new List<GameObject>();
    [Header("Projectile Stuff")]
    [SerializeField] GameObject projectile;
    readonly List<GameObject> projectilePool = new List<GameObject>();
    [Header("DropBox Stuff")]
    [SerializeField] GameObject dropBox;
    readonly List<GameObject> dropBoxPool = new List<GameObject>();
    [Header("Turret Stuff")]
    [SerializeField] GameObject turret;
    readonly List<GameObject> turretPool = new List<GameObject>();

    void OnEnable()
    {
        Rocket.ExplodeRocket += Explode;
        Meteor.MeteorExploded += Explode;
        Meteor.MeteorEvaded += DestroyMeteor;
        Drone.DroneExploded += Explode;
        Bomber.BomberExploded += Explode;
        Projectile.ProjectileExploded += Explode;
        DropBox.HitObject += Explode;
    }

    void OnDisable()
    {
        Rocket.ExplodeRocket -= Explode;
        Meteor.MeteorExploded -= Explode;
        Meteor.MeteorEvaded -= DestroyMeteor;
        Drone.DroneExploded -= Explode;
        Bomber.BomberExploded -= Explode;
        Projectile.ProjectileExploded -= Explode;
        DropBox.HitObject -= Explode;
    }

    private void DestroyMeteor(GameObject previousMeteor)
    {
        GameObject m = GetBrokenMeteor();
        m.transform.localScale = previousMeteor.transform.localScale;
        m.transform.localPosition = previousMeteor.transform.position;
        m.SetActive(true);
        previousMeteor.SetActive(false);
    }

    public void Explode(Vector3 pos, float size, byte type)
    {
        ExplosionSound?.Invoke(pos);
        GameObject explosion = GetExplosion(type);
        if (explosion != null)
        {
            explosion.transform.localScale = new Vector3(size * .5f, size * .5f, size * .5f);
            explosion.transform.position = pos;
            explosion.SetActive(true);
        }
    }

    public GameObject GetDropBox()
    {
        for (int i = 0; i < dropBoxPool.Count; i++)
        {
            if (!dropBoxPool[i].activeSelf)
                return dropBoxPool[i];
        }
        if (dropBox != null)
        {
            GameObject newBox = Instantiate(dropBox, Vector3.down, Quaternion.identity);
            newBox.SetActive(false);
            dropBoxPool.Add(newBox);
            return newBox;
        }
        return null;
    }

    public GameObject GetTurret()
    {
        for (int i = 0; i < turretPool.Count; i++)
        {
            if (!turretPool[i].activeSelf)
                return turretPool[i];
        }
        if (dropBox != null)
        {
            GameObject newTurret = Instantiate(turret, Vector3.down, Quaternion.identity);
            newTurret.SetActive(false);
            turretPool.Add(newTurret);
            return newTurret;
        }
        return null;
    }

    public GameObject GetTank()
    {
        for (int i = 0; i < tankPool.Count; i++)
        {
            if (!tankPool[i].activeSelf)
                return tankPool[i];
        }
        if (tank != null)
        {
            GameObject newTank = Instantiate(tank, Vector3.down, Quaternion.identity);
            newTank.SetActive(false);
            tankPool.Add(newTank);
            return newTank;
        }
        return null;
    }

    public GameObject GetProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeSelf)
                return projectilePool[i];
        }
        if (projectile != null)
        {
            GameObject newProjectile = Instantiate(projectile, Vector3.down, Quaternion.identity);
            newProjectile.SetActive(false);
            projectilePool.Add(newProjectile);
            return newProjectile;
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
            GameObject newBrokenEffect = Instantiate(brokenBuildingEffect, Vector3.down, Quaternion.identity);
            newBrokenEffect.SetActive(false);
            brokenEffectPool.Add(newBrokenEffect);
            return newBrokenEffect;
        }
        return null;
    }

    public GameObject GetBomber()
    {
        for (int i = 0; i < bomberPool.Count; i++)
            if (!bomberPool[i].activeSelf)
                return bomberPool[i];
        if (bomber != null)
        {
            GameObject newBomber = Instantiate(bomber, Vector3.down, Quaternion.identity);
            newBomber.SetActive(false);
            bomberPool.Add(newBomber);
            return newBomber;
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
            GameObject newCollapseEffect = Instantiate(collapseEffect, Vector3.down, Quaternion.identity);
            newCollapseEffect.SetActive(false);
            collapseEffectPool.Add(newCollapseEffect);
            return newCollapseEffect;
        }
        return null;
    }

    public GameObject GetRocket()
    {
        for (int i = 0; i < rocketGameObjectPool.Count; i++)
            if (!rocketGameObjectPool[i].activeSelf)
                return rocketGameObjectPool[i];
        if (rocket != null)
        {
            GameObject newRocket = Instantiate(rocket, Vector3.down, Quaternion.identity);
            newRocket.SetActive(false);
            rocketGameObjectPool.Add(newRocket);
            return newRocket;
        }
        return null;
    }

    public GameObject GetExplosion(byte type)
    {
        switch (type)
        {
            case 0:
                for (int i = 0; i < explosionEffectPool.Count; i++)
                    if (!explosionEffectPool[i].activeSelf)
                        return explosionEffectPool[i];
                if (explosionEffect != null)
                {
                    GameObject newExplosion = Instantiate(explosionEffect, Vector3.down, Quaternion.identity);
                    newExplosion.SetActive(false);
                    explosionEffectPool.Add(newExplosion);
                    return newExplosion;
                }
                break;
            case 1:
                for (int i = 0; i < explosionEffect2Pool.Count; i++)
                    if (!explosionEffect2Pool[i].activeSelf)
                        return explosionEffect2Pool[i];
                if (explosionEffect2 != null)
                {
                    GameObject newExplosion2 = Instantiate(explosionEffect2, Vector3.down, Quaternion.identity);
                    newExplosion2.SetActive(false);
                    explosionEffect2Pool.Add(newExplosion2);
                    return newExplosion2;
                }
                break;
            case 2:
                for (int i = 0; i < explosionEffect3Pool.Count; i++)
                    if (!explosionEffect3Pool[i].activeSelf)
                        return explosionEffect3Pool[i];
                if (explosionEffect3 != null)
                {
                    GameObject newExplosion3 = Instantiate(explosionEffect3, Vector3.down, Quaternion.identity);
                    newExplosion3.SetActive(false);
                    explosionEffect3Pool.Add(newExplosion3);
                    return newExplosion3;
                }
                break;
            default:
                break;
        }
        return null;
    }

    public GameObject GetDrone()
    {
        for (int i = 0; i < dronePool.Count; i++)
        {
            if (!dronePool[i].activeSelf)
                return dronePool[i];
        }
        if (drone != null)
        {
            GameObject newDrone = Instantiate(drone, Vector3.down, Quaternion.identity);
            newDrone.SetActive(false);
            dronePool.Add(newDrone);
            return newDrone;
        }
        return null;
    }

    public GameObject GetMeteor()
    {
        for (int i = 0; i < meteorPool.Count; i++)
        {
            if (!meteorPool[i].activeSelf)
                return meteorPool[i];
        }
        if (meteor != null)
        {
            GameObject newMeteor = Instantiate(meteor, Vector3.down, Quaternion.identity);
            newMeteor.SetActive(false);
            meteorPool.Add(newMeteor);
            return newMeteor;
        }
        return null;
    }

    public GameObject GetBrokenMeteor()
    {
        for (int i = 0; i < brokenMeteorPool.Count; i++)
        {
            if (!brokenMeteorPool[i].activeSelf)
                return brokenMeteorPool[i];
        }
        if (meteor != null)
        {
            GameObject newBrokenMeteor = Instantiate(brokenMeteor, Vector3.down, Quaternion.identity);
            newBrokenMeteor.SetActive(false);
            brokenMeteorPool.Add(newBrokenMeteor);
            return newBrokenMeteor;
        }
        return null;
    }

    public GameObject GetInfoLetters()
    {
        for (int i = 0; i < infoLettersPool.Count; i++)
        {
            if (!infoLettersPool[i].activeSelf)
                return infoLettersPool[i];
        }
        if (infoLetters != null)
        {
            GameObject newInfo = Instantiate(infoLetters, Vector3.down, Quaternion.identity);
            newInfo.SetActive(false);
            infoLettersPool.Add(newInfo);
            return newInfo;
        }
        return null;
    }
}