using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Vector3 minAreaToTraverse;
    [SerializeField] private Vector3 maxAreaToTraverse;

    public Vector3 FindLocationWithinArea() => new Vector3(Random.Range(minAreaToTraverse.x, maxAreaToTraverse.x), Random.Range(minAreaToTraverse.y, maxAreaToTraverse.y), Random.Range(minAreaToTraverse.z, maxAreaToTraverse.z));
}
