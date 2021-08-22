using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable<float>
{
    [SerializeField] IntVariable maxHealth;
    [SerializeField] FloatVariable currentHealth;

    public void ApplyDamage(float amount)
    {
        if (currentHealth != null)
            currentHealth.Value -= amount;
    }

    public float GetCurrentHealth()
    {
        if (currentHealth != null)
            return currentHealth.Value;
        return 0;
    }

    public float GetMaxHealth() => maxHealth.Value;

    void Start()
    {
        if (maxHealth != null)
            currentHealth.Value = maxHealth.Value;
    }
}