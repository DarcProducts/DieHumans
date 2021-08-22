using System.Collections;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] Vector3 dropSpawnMin;
    [SerializeField] Vector3 dropSpawnMax;
    [SerializeField] bool startDrops;
    [SerializeField] ObjectPooler meteorPool;
    [SerializeField] ObjectPooler dropPool;
    [SerializeField] float meteorDropRate;
    [SerializeField] float meteorSpeed;
    [Range(0f, 1f)] [SerializeField] float boxSpawnRate;
    bool isRunning = false;
    bool canStartStorms = true;

    private void OnEnable()
    {
        WaveManager.WaveStarted += StartDrop;
        WaveManager.WaveCompleted += StopDrop;
    }

    private void OnDisable()
    {
        WaveManager.WaveStarted -= StartDrop;
        WaveManager.WaveCompleted -= StopDrop;
    }

    void FixedUpdate()
    {
        if (startDrops)
        {
            if (canStartStorms)
            {
                StartCoroutine(nameof(StartMeteorStorm));
                canStartStorms = false;
            }
        }
        else if (!startDrops && isRunning)
        {
            StopCoroutine(nameof(StartMeteorStorm));
            isRunning = false;
        }
    }

    public IEnumerator StartMeteorStorm()
    {
        isRunning = true;
        yield return new WaitForSeconds(meteorDropRate);
        Vector3 ranPos = new Vector3(Random.Range(dropSpawnMin.x, dropSpawnMax.x), Random.Range(dropSpawnMin.y, dropSpawnMax.y), Random.Range(dropSpawnMin.z, dropSpawnMax.z));

        if (Random.value < boxSpawnRate)
        {
            GameObject box = dropPool.GetObject();
            if (box != null)
            {
                box.transform.SetPositionAndRotation(ranPos, Quaternion.identity);
                box.SetActive(true);
            }
        }
        else
        {
            GameObject meteor = meteorPool.GetObject();
            Meteor m = meteor.GetComponent<Meteor>();
            if (m != null)
                m.SetMeteorSpeed(meteorSpeed);
            meteor.transform.position = ranPos;
            meteor.SetActive(true);
        }
        canStartStorms = true;
    }

    public void StartDrop() => startDrops = true;

    public void StopDrop() => startDrops = false;
}