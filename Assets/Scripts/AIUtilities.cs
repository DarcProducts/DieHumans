using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AIUtilities : MonoBehaviour
{
    public static UnityAction<AudioSource> LaserActivated;
    CityGenerator CityGenerator { get; set; }
    ObjectPools ObjectPools { get; set; }
    GameObject Player { get; set; }


    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        CityGenerator = FindObjectOfType<CityGenerator>();
        ObjectPools = FindObjectOfType<ObjectPools>();
        if (Player == null)
            Debug.LogWarning("No GameObject with Player tag could be found in scene");
        if (CityGenerator == null)
            Debug.LogWarning("An object with a CityGenerator component could not be found in scene");
        if (ObjectPools == null)
            Debug.LogWarning("An object with a EnemyManager component could not be found in scene");
    }

    public bool CheckPlayerWithinRange(GameObject target, float range)
    {
        if (Player != null)
        {
            if (Vector3.Distance(target.transform.position, Player.transform.position) < range)
                return true;
        }
        return false;
    }

    /// <param name="target"> current object from class being called from </param>
    /// <param name="distance"> distance to check for player </param>
    /// <returns></returns>
    public bool CheckIfPathClear(GameObject target, float distance)
    {
        if (Player != null)
        {
            if (Physics.Raycast(target.transform.position, Player.transform.position - target.transform.position, out RaycastHit hit, distance))
                if (hit.collider.CompareTag("Player"))
                    return true;
        }
        return false;
    }

    public void LaunchRocket(Vector3 firedFrom, bool isHoming, float damage, float thrust = 32, float homingSpeed = 8)
    {
        if (Player != null)
        {
            Vector3 direction = Player.transform.position + Vector3.up - firedFrom + Vector3.down;
            GameObject r = ObjectPools.GetAvailableRocket();
            r.transform.position = firedFrom + Vector3.down * .5f;
            Rocket rocket = r.GetComponent<Rocket>();
            if (r != null && rocket != null)
            {
                rocket.rocketDamage = damage;
                r.SetActive(true);
                if (!isHoming)
                {
                    rocket.type = RocketType.standard;
                    r.GetComponent<Rigidbody>().AddForce(direction.normalized * thrust, ForceMode.Impulse);
                }
                else
                {
                    rocket.type = RocketType.homing;
                    rocket.rocketSpeed = homingSpeed;
                }
            }
        }
    }

    public void ShootALaser(LineRenderer line, Vector3 from, Vector3 dir, float damage, AudioSource source = null)
    {
        if (line != null)
        {
            line.enabled = true;
            if (Physics.Raycast(from, dir, out RaycastHit hitInfo))
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, hitInfo.point);
                TryDamagingTarget(hitInfo.collider.gameObject, damage);
                if (source != null)
                    LaserActivated?.Invoke(source);
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, dir);
            }
        }
    }

    public void DeactivateLaser(LineRenderer laser) => laser.enabled = false;

    public virtual void TryDamagingTarget(GameObject target, float damage)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(damage);
    }

    public ObjectPools GetObjectPools() => ObjectPools;

    public GameObject GetPlayer() => Player;
}
