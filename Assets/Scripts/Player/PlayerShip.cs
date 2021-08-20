using UnityEngine;

public class PlayerShip : MonoBehaviour, IDamagable<float>
{
    [SerializeField] IntVariable maxHealth;
    [SerializeField] FloatVariable currentHealth;

    public void ApplyDamage(float amount)
    {
        if (currentHealth != null)
            currentHealth.value -= amount;
    }

    public float GetCurrentHealth()
    {
        if (currentHealth != null)
            return currentHealth.value;
        return 0;
    }

    public float GetMaxHealth() => maxHealth.value;

    void Start()
    {
        if (maxHealth != null)
            currentHealth.value = maxHealth.value;
    }
}