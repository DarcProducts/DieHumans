using System.Collections.Generic;
using UnityEngine;
public class ObjectPooler : MonoBehaviour
{
    public GameObject objectToPool = null;
    readonly List<GameObject> objects = new List<GameObject>();

    public GameObject GetObject()
    {
        if (objects.Count > 0)
        {
            for (int i = 0; i < objects.Count; i++)
                if (!objects[i].activeSelf)
                    return objects[i];
        }
        if (objectToPool != null)
        {
            GameObject newObject = Instantiate(objectToPool, Vector3.down, Quaternion.identity, transform);
            objects.Add(newObject);
            newObject.SetActive(false);
            return newObject;
        }
        return null;
    }
}