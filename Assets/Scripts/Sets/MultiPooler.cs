using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPooler : MonoBehaviour
{
    [SerializeField] List<ObjectPooler> objectPools = new List<ObjectPooler>();

    public GameObject GetObject(byte index)
    {
        if (index <= objectPools.Count && objectPools[index] != null)
            return objectPools[index].GetObject();
        return null;
    }
}
