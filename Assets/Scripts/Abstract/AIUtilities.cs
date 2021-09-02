using UnityEngine;
using UnityEngine.Events;

public abstract class AIUtilities : MonoBehaviour
{
    public static GameObject Player;

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

    public GameObject FindRandomCloseTarget(LayerMask targetLayers, float checkDistance)
    {
        GameObject currentTarget;
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, checkDistance, targetLayers.value);
        if (closeObjects.Length > 0)
        {
            int randObj = Random.Range(0, closeObjects.Length);
            currentTarget = closeObjects[randObj].gameObject;
            return currentTarget;
        }
        return null;
    }

    public bool TryDamagingTarget(GameObject target, float damage, LayerMask ignoreLayer)
    {
        if (Utilities.IsInLayerMask(target, ignoreLayer))
            return false;
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            d.ApplyDamage(damage);
            return true;
        }
        return false;
    }

    [ContextMenu("Get Player")]
    public GameObject GetPlayer()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
            return Player;
        Debug.LogWarning($"No GameObject in scene with 'Player' tag on it!");
        return null;
    }
}