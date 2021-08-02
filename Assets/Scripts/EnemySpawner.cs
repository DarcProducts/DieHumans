using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    ObjectPools objectPools;

    [SerializeField] bool startSpawning;

    [SerializeField] float spawnDelay = 5f;
    [SerializeField] float cDelay = 0f;

    [Header("Spawn Areas")]
    [SerializeField] Vector3[] minSpawnAreas;

    [SerializeField] Vector3[] maxSpawnAreas;

    void Awake() => objectPools = FindObjectOfType<ObjectPools>();

    void Start() => cDelay = spawnDelay;

    void OnEnable() => cDelay = spawnDelay;

    void FixedUpdate()
    {
        if (startSpawning)
            StartSpawning();
    }

    void StartSpawning()
    {
        if (objectPools != null)
        {
            cDelay = cDelay < 0 ? 0 : cDelay -= Time.fixedDeltaTime;
            if (cDelay <= 0)
            {
                int ranSpawn = Random.Range(0, maxSpawnAreas.Length);
                Vector3 newLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), Random.Range(minSpawnAreas[ranSpawn].y, maxSpawnAreas[ranSpawn].y), Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));
                Vector3 tankLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), 50, Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));

                float ranVal = Random.value;

                if (ranVal > .9f)
                {
                    GameObject b = objectPools.GetBomber();
                    b.transform.position = newLoc;
                    b.SetActive(true);
                }
                else if (ranVal <= .9f && ranVal > .7f)
                {
                    GameObject t = objectPools.GetTank();
                    t.transform.position = tankLoc;
                    t.SetActive(true);
                }
                else
                {
                    GameObject d = objectPools.GetDrone();
                    d.transform.position = newLoc;
                    d.SetActive(true);
                }
                spawnDelay = spawnDelay < .1f ? .1f : spawnDelay -= .001f;
                cDelay = spawnDelay;
            }
        }
    }
}