using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable<float>
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public void ApplyDamage(float amount) => currentHealth -= amount;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
