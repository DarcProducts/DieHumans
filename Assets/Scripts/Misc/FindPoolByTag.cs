using UnityEngine;

public class FindPoolByTag : MonoBehaviour
{
    [SerializeField] string poolTag;
    ObjectPooler foundPool;

    private void Awake()
    {
        GameObject pool = GameObject.FindWithTag(poolTag);
        if (pool != null)
            foundPool = pool.GetComponent<ObjectPooler>();
    }

    public ObjectPooler GetFoundPool() => foundPool;
}
