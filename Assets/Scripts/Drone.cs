using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Drone : EnemyAI, IDamagable<float>
{
    public static UnityAction<Vector3> DroneDamaged;
    public static UnityAction<Vector3> DroneExploded;
    [SerializeField] private float maxWanderHeight, checkDistancePlayer, attackDistance, fireTime, moveSpeed, maxHealth, currentFireTime, currentHealth = 1, laserDuration;
    [Range(0f, 10f)] [SerializeField] private float downForceOnDeath;
    [SerializeField] private AudioSource mainSource, laserHitSource;
    private bool dieForceApplied = false, hasFoundPlayer = false, hasDied = false, isNearPlayer, canFire = true;
    private Vector3 nextTargetLocation;
    private LineRenderer laser;

    private void Start()
    {
        laser = GetComponent<LineRenderer>();
        if (objectPools != null)
            nextTargetLocation = PickTargetLocationWithinCity(maxWanderHeight);
        currentHealth = maxHealth;
        currentFireTime = fireTime;
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
            if (objectPools != null && player != null)
            {
                transform.LookAt(player.transform.position, Vector3.up);
                if (CheckIfPathClear(gameObject, checkDistancePlayer))
                {
                    hasFoundPlayer = true;
                    if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
                    {
                        isNearPlayer = true;
                        Attack();
                    }
                    else isNearPlayer = false;
                }
                if (hasFoundPlayer && !isNearPlayer)
                {
                    transform.position = Vector3.MoveTowards(transform.position, new Vector3(Random.Range(player.transform.position.x - attackDistance, player.transform.position.x + attackDistance), Random.Range(maxWanderHeight - 10, maxWanderHeight), Random.Range(player.transform.position.z - attackDistance, player.transform.position.z + attackDistance)), moveSpeed * Time.smoothDeltaTime);
                    return;
                }
            }

            if (!transform.position.Equals(nextTargetLocation))
            {
                transform.position = Vector3.MoveTowards(transform.position, nextTargetLocation, moveSpeed * Time.smoothDeltaTime);
                if (canFire)
                    FireRandomLaserShot(false);
                return;
            }
            if (transform.position.Equals(nextTargetLocation))
                nextTargetLocation = ChangeNextTargetLocation();
        }
    }

    private void FireRandomLaserShot(bool isPlayer)
    {
        if (isPlayer)
        {

        }
        else
        {

        }
    }

    public Vector3 ChangeNextTargetLocation()
    {
        if (objectPools != null)
            return PickTargetLocationWithinCity(maxWanderHeight);
        return transform.position;
    }

    public void Attack()
    {
        currentFireTime = currentFireTime <= 0 ? 0 : currentFireTime -= Time.deltaTime;

        if (currentFireTime <= 0 && objectPools != null && canFire)
            StartCoroutine(FireLasers());
    }

    private IEnumerator FireLasers()
    {
        canFire = false;
        FireRandomLaserShot(true);
        yield return new WaitForSeconds(laserDuration);
        currentFireTime = fireTime;
        canFire = true;
        StopCoroutine(FireLasers());
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
            DroneExploded?.Invoke(transform.position);
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

    public float GetMaxWanderHeight() => maxWanderHeight;

    public float GetCurrentHealth() => currentHealth;

    public void ApplyDamage(float damage)
    {
        currentHealth -= damage;
        DroneDamaged?.Invoke(transform.position);
    }

    [ContextMenu("Kill Drone")]
    public void KillDrone() => ApplyDamage(1000000);
}