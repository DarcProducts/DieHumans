using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private bool objectFront;
    [SerializeField] private bool objectBack;
    [SerializeField] private bool objectLeft;
    [SerializeField] private bool objectRight;
    [SerializeField] private bool objectBelow;
    [SerializeField] private float[] distancesToCheck = new float[5];
}
