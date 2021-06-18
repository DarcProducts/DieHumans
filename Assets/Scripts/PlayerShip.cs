using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable<float>
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    public void ApplyDamage(float amount) => currentHealth -= amount;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;

    private void Start() => currentHealth = maxHealth;
}