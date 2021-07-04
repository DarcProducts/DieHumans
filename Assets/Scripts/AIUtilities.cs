using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIUtilities : MonoBehaviour
{
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
    public Vector3 PickTargetLocationWithinCity(float maxWanderHeight)
    {
        if (CityGenerator != null)
        {

            Vector3[] cityBounds = GetCityBounds();
            return new Vector3(Random.Range(cityBounds[0].x, cityBounds[0].z), Random.Range(CityGenerator.GetMaxBuildingHeight() + CityGenerator.GetGridSize() + 2, CityGenerator.GetMaxBuildingHeight() + CityGenerator.GetGridSize() + maxWanderHeight), Random.Range(cityBounds[1].x, cityBounds[1].z));
        }
        return Vector3.zero;
    }

    public Vector3[] GetCityBounds()

    {
        if (CityGenerator != null)
            return CityGenerator.GetCityBounds();
        return new Vector3[2];
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
                    rocket.isHoming = false;
                    r.GetComponent<Rigidbody>().AddForce(direction.normalized * thrust, ForceMode.Impulse);
                }
                else
                {
                    rocket.isHoming = true;
                    rocket.homingSpeed = homingSpeed;
                }
            }
        }
    }

    public void ShootALaser(LineRenderer line, Vector3 from, Vector3 dir, float damage, float duration) => StartCoroutine(InitializeLaserbeam(line, from, dir, damage, duration));

    private IEnumerator InitializeLaserbeam(LineRenderer line, Vector3 from, Vector3 dir, float damage, float duration)
    {
        line.enabled = true;
        if (line != null)
        {
            if(Physics.Raycast(from, dir, out RaycastHit hitInfo))
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, hitInfo.point);
                TryDamagingTarget(hitInfo.collider.gameObject, damage);
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, dir);
            }
        }
        yield return new WaitForSeconds(duration);
        line.enabled = false;
        StopCoroutine(InitializeLaserbeam(line, from, dir, damage, duration));
    }

    public virtual void TryDamagingTarget(GameObject target, float damage)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(damage);
    }

    public ObjectPools GetObjectPools() => ObjectPools;

    public GameObject GetPlayer() => Player;
}
