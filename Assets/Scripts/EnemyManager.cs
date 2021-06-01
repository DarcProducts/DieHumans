using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private CityGenerator cityGenerator;
    [SerializeField] private GameObject playerShip;
    [SerializeField] private bool debug;

    [Header("Game Stats")]
    [SerializeField] private byte threatLevel;

    [SerializeField] private int currentWave;

    [Header("Chopper Stats")]
    [SerializeField] private float maxChopperHealth;

    [SerializeField] private float maxWanderHeight;
    [SerializeField] private GameObject rocket;
    [SerializeField] private float rocketThrust;
    [SerializeField] private float rocketDamage;
    [SerializeField] private float choppperAttDist;

    private void Awake()
    {
        if (cityGenerator == null)
            cityGenerator = FindObjectOfType<CityGenerator>();
        if (playerShip == null)
            playerShip = GameObject.FindWithTag("PlayerShip");
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

    public bool SearchForPlayer(GameObject target, float range)
    {
        if (!CheckPlayerWithinRange(target, range))
        {
            
        }
        return true;
    }

    private bool ObjectLeft()
    {

        return false;
    }

    private bool ObjectRight()
    {

        return false;
    }

    private bool ObjectForward()
    {

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

    public float GetChopperAttackDistance() => choppperAttDist;

    public float GetMaxChopperHealth() => maxChopperHealth;

    public GameObject GetPlayerShip() => playerShip;

    public byte GetThreatLevel() => threatLevel;

    public int GetCurrentWave() => currentWave;
}