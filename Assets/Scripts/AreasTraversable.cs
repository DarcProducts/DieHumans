using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AreasTraversable
{
    [SerializeField] private static List<Vector3> safePoints = new List<Vector3>();
    public static void AddPoint(Vector3 point) => safePoints.Add(point);
    public static Vector3[] GetAllPoints() => safePoints.ToArray();

    public static Vector3 GetRandomSafePoint()
    {
        if (safePoints.Count > 0)
        {
            int ranNum = Random.Range(0, safePoints.Count);
            return safePoints[ranNum];
        }
        return Vector3.zero;
    }
}
