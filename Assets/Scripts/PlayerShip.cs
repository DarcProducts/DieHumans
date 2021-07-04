using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable<float>
{
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;

    public void ApplyDamage(float amount) => currentHealth -= amount;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;

    void Start() => currentHealth = maxHealth;
}