using System.Collections;
using UnityEngine;

public class Chopper : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject rocket;
    [SerializeField] private int rocketsToFire;
    [SerializeField] private float rocketThrust;
    [SerializeField] private float rocketFireTime;
    [SerializeField] private float currentFireTime;
    [SerializeField] private int currentRockets;
    private bool hasFired = false;
    [SerializeField] private float maxHealth;
    private float currentHealth;
    [SerializeField] private float maxSpeed;
    [Range(0f, 1f)] [SerializeField] private float healthPercentToFlee;
    [SerializeField] private float distanceFromPlayer;

    private void Awake()
    {
        player = GameObject.FindWithTag("PlayerShip");
        currentHealth = maxHealth;
        currentFireTime = rocketFireTime;
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < distanceFromPlayer && currentHealth > currentHealth * healthPercentToFlee)
            {
                Attack();
            }
        }
    }

    public void Attack()
    {
        currentFireTime = currentFireTime <= 0 ? 0 : currentFireTime -= Time.deltaTime;

        if (currentFireTime <= 0)
        {
            LaunchRockets();
            ResetRockets();
            currentFireTime = rocketFireTime;
        }
    }

    private void LaunchRockets()
    {
        if (!hasFired && player != null && rocket != null)
        {
            if (currentRockets < rocketsToFire)
            {
                GameObject r = Instantiate(rocket, transform.position + Vector3.down, Quaternion.identity);
                Vector3 direction = player.transform.position + player.transform.forward - transform.position;
                Rigidbody rR = r.GetComponent<Rigidbody>();
                if (rR != null)
                {
                    rR.useGravity = false;
                    rR.AddForce(direction.normalized * rocketThrust, ForceMode.Impulse);
                }
                currentRockets++;
                LaunchRockets();
            }
        }
    }

    private void ResetRockets()
    {
        currentRockets = 0;
        hasFired = false;
    }

    public void Flee()
    {

    }

    public void Die()
    {

    }

    
    
    public float GetCurrentHealth() => currentHealth;

    public void ApplyDamage(float damage) => currentHealth -= damage;
}