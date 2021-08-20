using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] MultiPooler objectPooler;
    [SerializeField] bool startSpawning;
    [SerializeField] float spawnDelay = 5f;
    [SerializeField] float cDelay = 0f;

    [Header("Spawn Areas")]
    [SerializeField] Vector3[] minSpawnAreas;

    [SerializeField] Vector3[] maxSpawnAreas;

    void Start() => cDelay = spawnDelay;

    void OnEnable() => cDelay = spawnDelay;

    void FixedUpdate()
    {
        if (startSpawning)
            StartSpawning();
    }

    void StartSpawning()
    {
        cDelay = cDelay < 0 ? 0 : cDelay -= Time.fixedDeltaTime;
        if (cDelay <= 0)
        {
            int ranSpawn = Random.Range(0, maxSpawnAreas.Length);
            Vector3 newLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), Random.Range(minSpawnAreas[ranSpawn].y, maxSpawnAreas[ranSpawn].y), Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));
            Vector3 tankLoc = new Vector3(Random.Range(minSpawnAreas[ranSpawn].x, maxSpawnAreas[ranSpawn].x), 50, Random.Range(minSpawnAreas[ranSpawn].z, maxSpawnAreas[ranSpawn].z));

            float ranVal = Random.value;

            if (objectPooler != null)
            {
                if (ranVal > .9f)
                {
                    GameObject b = objectPooler.GetObject(7);
                    b.transform.position = newLoc;
                    b.SetActive(true);
                }
                else if (ranVal <= .9f && ranVal > .7f)
                {
                    GameObject t = objectPooler.GetObject(6);
                    t.transform.position = tankLoc;
                    t.SetActive(true);
                }
                else
                {
                    GameObject d = objectPooler.GetObject(5);
                    d.transform.position = newLoc;
                    d.SetActive(true);
                }
            }
            spawnDelay = spawnDelay < .1f ? .1f : spawnDelay -= .001f;
            cDelay = spawnDelay;
        }
    }
}