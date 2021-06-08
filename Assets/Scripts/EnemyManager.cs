using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private CityGenerator cityGenerator;
    [SerializeField] private GameObject playerShip;
    [SerializeField] private bool debug;

    [Header("Game Stats")]
    [SerializeField] private byte threatLevel;

    [SerializeField] private int currentWave;

    [Header("Rocket Stats")]

    [SerializeField] private GameObject rocket;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float rocketThrust;
    [SerializeField] private float rocketDamage;
    private List<GameObject> explosionEffectPool = new List<GameObject>();
    [SerializeField] private int initialExplosionPool = 10;

    [Header("Arial Enemies")]
    [SerializeField] private float maxWanderHeight;

    private void Awake() => cityGenerator = FindObjectOfType<CityGenerator>();

    private void Start()
    {
        if (playerShip == null)
            playerShip = GameObject.FindWithTag("PlayerShip");
        if (cityGenerator == null)
            Debug.LogError("No City Generator found in scene! Cannot get city bounds for Enemy Manager.");
        if (explosionEffect != null)
        {
            for (int i = 0; i < initialExplosionPool; i++)
            {
                GameObject e = Instantiate(explosionEffect, Vector3.down, Quaternion.identity);
                e.SetActive(false);
                explosionEffectPool.Add(e);
            }
        }
    }

    public Vector3 PickTargetLocation()
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
        if (playerShip != null)
        {
            if (Vector3.Distance(target.transform.position, playerShip.transform.position) < range)
                return true;
        }
        return false;
    }

    public bool CheckIfPathClear(GameObject target, float distance)
    {
        if (Physics.Raycast(target.transform.position + Vector3.down, playerShip.transform.position + Vector3.up - target.transform.position, out RaycastHit hit, distance))
        {
            if (debug)
                Debug.DrawRay(target.transform.position + Vector3.down, playerShip.transform.position - target.transform.position, Color.red, distance);
            if (hit.collider.CompareTag("PlayerShip"))
                return true;
        }
        return false;
    }

    public void LaunchRocket(GameObject targetLocation)
    {
        if (playerShip != null && rocket != null)
        {
            Vector3 direction = playerShip.transform.position + playerShip.transform.forward * .8f - targetLocation.transform.position;
            GameObject r = Instantiate(rocket, targetLocation.transform.position + Vector3.down * .5f, Quaternion.LookRotation(direction, Vector3.up));
            r.GetComponent<Rocket>().SetRocketDamage(rocketDamage);
            Rigidbody rR = r.GetComponent<Rigidbody>();
            if (rR != null)
            {
                rR.useGravity = false;
                rR.AddForce(direction.normalized * rocketThrust, ForceMode.Impulse);
            }
        }
    }

    public GameObject GetPlayerShip() => playerShip;

    public byte GetThreatLevel() => threatLevel;

    public int GetCurrentWave() => currentWave;
}