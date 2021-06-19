using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public CityGenerator cityGenerator;
    public EnemyManager enemyManager;
    public GameObject player;
    public float rocketThrust = 32;
    public float rocketDamage = 100;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cityGenerator = FindObjectOfType<CityGenerator>();
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
            rocketThrust = enemyManager.GetRocketThrust();
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

    public void LaunchRocket(Vector3 firedFrom, bool isHoming)
    {
        if (player != null)
        {
            Vector3 direction = player.transform.position + Vector3.up - firedFrom + Vector3.down;
            GameObject r = enemyManager.GetAvailableRocket();
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

    public void ShootLaser(LineRenderer line, Vector3 from, Vector3 to)
    {

    }
}
