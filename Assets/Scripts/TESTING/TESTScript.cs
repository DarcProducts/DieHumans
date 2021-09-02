using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTScript : MonoBehaviour
{
    public float bias;
    public float currentHealth;
    public float maxHealth;

    [ContextMenu("Calculate Values")]
    public void CalculateValues()
    {
        float newValue = maxHealth - currentHealth;
        Debug.Log($"value = {newValue}");
    }
}
