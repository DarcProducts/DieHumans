using UnityEngine;

public class Chopper : MonoBehaviour, IDamagable<float>
{
    private EnemyManager enemyManager;
    [SerializeField] private GameObject chopperExplodedEffect;
    private bool isExploded = false;
    [SerializeField] private float maxWanderHeight;
    [SerializeField] private float checkDistancePlayer;
    [SerializeField] private float checkDistanceObject;
    [SerializeField] private float rocketFireTime;
    [SerializeField] private float moveSpeed;
    private float currentFireTime;
    [SerializeField] private float maxHealth;
    private float currentHealth = 1;
    [Range(0f, 1f)] [SerializeField] private float healthPercentToFlee;
    [SerializeField] private float distanceFromPlayer;
    [Range(0f, 10f)] [SerializeField] private float downForceOnDeath;
    [SerializeField] private Vector3 effectSize;
    private bool dieForceApplied = false;
    private GameObject playerShip;
    private Vector3 nextTargetLocation;
    private bool hasFoundPlayer = false;

    private void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            nextTargetLocation = enemyManager.PickTargetLocationWithinCity(maxWanderHeight);
            playerShip = enemyManager.GetPlayerShip();
        }
        if (playerShip == null)
            playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        currentHealth = maxHealth;
        currentFireTime = rocketFireTime;
    }

    private void LateUpdate()
    {
        if (currentHealth <= 0)
            Die();
        if (enemyManager != null)
            if (enemyManager.CheckIfPathClear(gameObject, checkDistancePlayer))
            {
                hasFoundPlayer = true;
                Attack();
            }
        if (hasFoundPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerShip.transform.position.x, Random.Range(maxWanderHeight - 10, maxWanderHeight), playerShip.transform.position.z), moveSpeed * .01f * Time.smoothDeltaTime);
            return;
        }

        if (!transform.position.Equals(nextTargetLocation))
            transform.position = Vector3.MoveTowards(transform.position, nextTargetLocation, moveSpeed * Time.smoothDeltaTime);
        if (transform.position.Equals(nextTargetLocation))
            nextTargetLocation = ChangeNextTargetLocation();
        transform.forward = Vector3.Lerp(transform.forward, nextTargetLocation - transform.position, moveSpeed * .001f * Time.smoothDeltaTime);
    }

    public void OnEnable() => BuildingManager.BuildingDamaged += MoveToDamagedTarget;

    private void OnDisable()
    {
        BuildingManager.BuildingDamaged -= MoveToDamagedTarget;
        Explode();
    }

    public Vector3 ChangeNextTargetLocation()
    {
        if (enemyManager != null)
            return enemyManager.PickTargetLocationWithinCity(maxWanderHeight);
        return transform.position;
    }

    public void MoveToDamagedTarget(Vector3 newLoc)
    {
        if (!newLoc.Equals(nextTargetLocation))
            nextTargetLocation = new Vector3(Random.Range(newLoc.x - 10, newLoc.x + 10), transform.position.y, Random.Range(newLoc.z - 10, newLoc.z + 10));
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
        if (!collision.gameObject.CompareTag("TVWall") && !collision.gameObject.CompareTag("Chopper"))
        {
            IDamagable<float> damagable = collision.gameObject.GetComponent<IDamagable<float>>();
            if (damagable != null)
                damagable.ApplyDamage(maxHealth);
            Debug.Log($"HIT: {collision.collider.gameObject.name}");
            gameObject.SetActive(false);
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
        if (!isExploded && chopperExplodedEffect != null)
        {
            GameObject e = Instantiate(chopperExplodedEffect, transform.position, Quaternion.identity);
            e.transform.localScale = new Vector3(effectSize.x, effectSize.y, effectSize.z);
            isExploded = true;
            Destroy(gameObject);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    public void ApplyDamage(float damage) => currentHealth -= damage;

    public float GetMaxHealth() => maxHealth;
}