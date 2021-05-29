using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private CityGenerator cityGenerator;
    [SerializeField] private GameObject playerShip;

    [Header("Game Stats")]
    [SerializeField] private byte threatLevel;

    [SerializeField] private int currentWave;
    [Header("Chopper Stats")]
    [SerializeField] private float maxChopperHealth;
    [SerializeField] private float maxWanderHeight;

    private void Start()
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

    public float GetMaxChopperHealth() => maxChopperHealth;
    public GameObject GetPlayerShip() => playerShip; 
    public byte GetThreatLevel() => threatLevel;
    public int GetCurrentWave() => currentWave;
}