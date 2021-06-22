using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public CityGenerator cityGenerator;
    public ObjectPools objectPools;
    public GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cityGenerator = FindObjectOfType<CityGenerator>();
        objectPools = FindObjectOfType<ObjectPools>();
        if (player == null)
            Debug.LogWarning("No GameObject with Player tag could be found in scene");
        if (cityGenerator == null)
            Debug.LogWarning("An object with a CityGenerator component could not be found in scene");
        if (objectPools == null)
            Debug.LogWarning("An object with a EnemyManager component could not be found in scene");
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

    public Vector3[] GetCityBounds()
    {
        if (cityGenerator != null)
            return cityGenerator.GetCityBounds();
        return new Vector3[2];
    }

    public bool CheckPlayerWithinRange(GameObject target, float range)
    {
        if (player != null)
        {
            if (Vector3.Distance(target.transform.position, player.transform.position) < range)
                return true;
        }
        return false;
    }

    public bool CheckIfPathClear(GameObject target, float distance)
    {
        if (player != null)
        {
            if (Physics.Raycast(target.transform.position + Vector3.down, player.transform.position + Vector3.up - target.transform.position, out RaycastHit hit, distance))
                if (hit.collider.CompareTag("Player"))
                    return true;
        }
        return false;
    }

    public void LaunchRocket(Vector3 firedFrom, bool isHoming, float damage, float thrust = 32, float homingSpeed = 8)
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position + Vector3.up - firedFrom + Vector3.down;
            GameObject r = objectPools.GetAvailableRocket();
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

    public void ShootLaser(LineRenderer line, Vector3 from, Vector3 to)
    {

    }
}
