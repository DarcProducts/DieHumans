public interface IEnemy
{
    void Attack();
    void Flee();
    void Die();
    void ApplyDamage(float amount);
    float GetCurrentHealth();
}
