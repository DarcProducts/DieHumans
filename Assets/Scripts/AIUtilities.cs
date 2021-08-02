using UnityEngine;
using UnityEngine.Events;

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

    public bool CheckPlayerWithinRange(GameObject target, float range)
    {
        if (Player != null)
        {
            if (Vector3.Distance(target.transform.position, Player.transform.position) < range)
                return true;
        }
        return false;
    }

    /// <param name="obj"> current object from class being called from </param>
    /// <param name="distance"> distance to check for player </param>
    /// <returns></returns>
    public bool CheckIfPathClearPlayer(GameObject obj, float distance)
    {
        if (Player != null)
        {
            if (Physics.Raycast(obj.transform.position, Player.transform.position - obj.transform.position, out RaycastHit hit, distance))
                if (hit.collider.CompareTag("Player"))
                    return true;
        }
        return false;
    }

    public bool CheckIfPathClear(GameObject obj, GameObject target, float distance)
    {
        if (Physics.Raycast(obj.transform.position, target.transform.position - obj.transform.position, out RaycastHit hit, distance))
            if (hit.collider.gameObject.Equals(target))
                return true;
        return false;
    }

    public ObjectPools GetObjectPools() => ObjectPools;

    public GameObject GetPlayer() => Player;
}