using System.Collections;
using UnityEngine;

public class Chopper : MonoBehaviour
{
    private EnemyManager enemyManager;
    [SerializeField] private GameObject chopperExplodedEffect;
    private bool isExploded = false;
    [SerializeField] private float checkDistancePlayer;
    [SerializeField] private float checkDistanceObject;
    [SerializeField] private float rocketFireTime;
    private float currentFireTime;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    [Range(0f, 1f)] [SerializeField] private float healthPercentToFlee;
    [SerializeField] private float distanceFromPlayer;
    [Range(0f, 10f)] [SerializeField] private float downForceOnDeath;
    [SerializeField] private Vector3 effectSize;
    private bool dieForceApplied = false;

    private void Start()
    {
        if (enemyManager == null)
            enemyManager = FindObjectOfType<EnemyManager>();
        currentHealth = maxHealth;
        currentFireTime = rocketFireTime;
    }

    private void LateUpdate()
    {
        if (currentHealth <= 0)
            Die();
    }

    public void Attack()
    {
        currentFireTime = currentFireTime <= 0 ? 0 : currentFireTime -= Time.deltaTime;

        if (currentFireTime <= 0 && enemyManager != null)
        {
            enemyManager.LaunchRocket(gameObject);
            currentFireTime = rocketFireTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Building"))
            gameObject.SetActive(false);
    }

    public void Die()
    {
        if (currentHealth <= 0)
        {
            transform.Rotate(transform.eulerAngles * Time.deltaTime);
            Rigidbody r = GetComponent<Rigidbody>();
            if (r != null)
            {
                r.useGravity = true;
                if (!dieForceApplied)
                {
                    r.AddForce(r.velocity + new Vector3(Random.Range(r.velocity.x - downForceOnDeath, r.velocity.x + downForceOnDeath), Random.Range(r.velocity.y - downForceOnDeath, r.velocity.y - downForceOnDeath * 2), Random.Range(r.velocity.z - downForceOnDeath, r.velocity.z + downForceOnDeath)), ForceMode.Impulse);
                    dieForceApplied = true;
                }
            }
        }
    }

    public void Explode()
    {
        if (!isExploded && chopperExplodedEffect != null)
        {
            GameObject e = Instantiate(chopperExplodedEffect, transform.position, Quaternion.identity);
            e.transform.localScale = new Vector3(effectSize.x, effectSize.y, effectSize.z);
            isExploded = true;
            Destroy(gameObject);
        }
    }

    public void OnDisable() => Explode();

    public float GetCurrentHealth() => currentHealth;

    public void ApplyDamage(float damage) => currentHealth -= damage;
}
