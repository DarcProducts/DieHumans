using UnityEngine;
using UnityEngine.Events;

public class Drone : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, bool> FiredRocket;
    public static UnityAction<Vector3> DroneDamaged;
    [SerializeField] private float maxWanderHeight;
    [SerializeField] private float checkDistancePlayer;
    [SerializeField] private float attackDistance;
    [SerializeField] private float rocketFireTime;
    [SerializeField] private bool shootHomingRockets;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxHealth;
    [Range(0f, 10f)] [SerializeField] private float downForceOnDeath;
    private EnemyManager enemyManager;
    private bool isExploded = false;
    private float currentFireTime;
    private float currentHealth = 1;
    private bool dieForceApplied = false;
    private GameObject Player;
    private Vector3 nextTargetLocation;
    private bool hasFoundPlayer = false;
    private bool hasDied = false;
    private bool isNearPlayer = false;

    private void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            nextTargetLocation = enemyManager.PickTargetLocationWithinCity(maxWanderHeight);
            Player = enemyManager.GetPlayerShip();
        }
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
        currentFireTime = rocketFireTime;
    }

    private void LateUpdate()
    {
        if (currentHealth <= 0)
        {
            hasDied = true;
            Die();
        }
        if (!hasDied)
        {
            if (enemyManager != null && Player != null)
            {
                //transform.LookAt(new Vector3(transform.rotation.x, transform.position.y - Player.transform.position.y, transform.rotation.z));
                if (enemyManager.CheckIfPathClear(gameObject, checkDistancePlayer))
                {
                    hasFoundPlayer = true;
                    if (Vector3.Distance(transform.position, Player.transform.position) < attackDistance)
                    {
                        isNearPlayer = true;
                        Attack();
                    }
                    else isNearPlayer = false;
                }
                if (hasFoundPlayer && !isNearPlayer)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(Random.Range(Player.transform.position.x - attackDistance, Player.transform.position.x + attackDistance), Random.Range(maxWanderHeight - 10, maxWanderHeight), Random.Range(Player.transform.position.z - attackDistance, Player.transform.position.z + attackDistance)), moveSpeed * Time.smoothDeltaTime);
                    return;
                }
            }

            if (!transform.position.Equals(nextTargetLocation))
            {
                transform.position = Vector3.MoveTowards(transform.position, nextTargetLocation, moveSpeed * Time.smoothDeltaTime);
                return;
            }
            if (transform.position.Equals(nextTargetLocation))
                nextTargetLocation = ChangeNextTargetLocation();
            
        }
    }

    public void OnEnable() => DroneDamaged += MoveToDamagedTarget;

    private void OnDisable() => DroneDamaged -= MoveToDamagedTarget;

    public Vector3 ChangeNextTargetLocation()
    {
        if (enemyManager != null)
            return enemyManager.PickTargetLocationWithinCity(maxWanderHeight);
        return transform.position;
    }

    public void MoveToDamagedTarget(Vector3 newLoc)
    {
        if (!newLoc.Equals(nextTargetLocation))
            nextTargetLocation = new Vector3(Random.Range(newLoc.x - attackDistance, newLoc.x + attackDistance), transform.position.y, Random.Range(newLoc.z - attackDistance, newLoc.z + attackDistance));
    }

    public void Attack()
    {
        currentFireTime = currentFireTime <= 0 ? 0 : currentFireTime -= Time.deltaTime;

        if (currentFireTime <= 0 && enemyManager != null)
        {
            FiredRocket?.Invoke(transform.position + Vector3.down, shootHomingRockets);
            currentFireTime = rocketFireTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("TVWall") || collision.gameObject.CompareTag("Drone"))
            return;
        else
        {
            IDamagable<float> damagable = collision.gameObject.GetComponent<IDamagable<float>>();
            if (damagable != null)
                damagable.ApplyDamage(Random.Range(maxHealth, maxHealth * 2));
            Explode();
        }
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
        if (!isExploded && enemyManager != null)
        {
            enemyManager.Explode(transform.position);
            isExploded = true;
            gameObject.SetActive(false);
        }
    }

    public float GetMaxWanderHeight() => maxWanderHeight;
    public float GetCurrentHealth() => currentHealth;

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        DroneDamaged?.Invoke(transform.position);
    }

    [ContextMenu("Damage Drone")]
    public void DamageDrone() => ApplyDamage(10000);

    public float GetMaxHealth() => maxHealth;
}