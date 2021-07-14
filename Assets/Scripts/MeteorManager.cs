using System.Collections;
using UnityEngine;

public class MeteorManager : MonoBehaviour
{
    [SerializeField] Vector3 meteorSpawnMin;
    [SerializeField] Vector3 meteorSpawnMax;
    [SerializeField] bool startMeteorStorm;
    [SerializeField] ObjectPools objectPools;
    [SerializeField] float meteorDropRate;
    [SerializeField] float meteorSpeed;
    [Range(0f, 1f)] [SerializeField] float boxSpawnRate;
    bool isRunning = false;
    bool canStartStorms = true;

    void FixedUpdate()
    {
        if (startMeteorStorm)
        {
            if (canStartStorms)
            {
                StartCoroutine(nameof(StartMeteorStorm));
                canStartStorms = false;
            }
        }
        else if (!startMeteorStorm && isRunning)
        {
            StopCoroutine(nameof(StartMeteorStorm));
            isRunning = false;
        }
    }

    public IEnumerator StartMeteorStorm()
    {
        if (objectPools != null)
        {
            isRunning = true;
            yield return new WaitForSeconds(meteorDropRate);
            GameObject meteor = objectPools.GetMeteor();
            if (meteor != null)
            {
                Vector3 ranPos = new Vector3(Random.Range(meteorSpawnMin.x, meteorSpawnMax.x), Random.Range(meteorSpawnMin.y, meteorSpawnMax.y), Random.Range(meteorSpawnMin.z, meteorSpawnMax.z));

                if (Random.value < boxSpawnRate)
                {
                    GameObject box = objectPools.GetDropBox();
                    box.transform.SetPositionAndRotation(ranPos, Quaternion.identity);
                    box.SetActive(true);
                }
                else
                {
                    Meteor m = meteor.GetComponent<Meteor>();
                    if (m != null)
                        m.SetMeteorSpeed(meteorSpeed);
                    meteor.transform.position = ranPos;
                    meteor.SetActive(true);
                }
            }
        }
    }
}