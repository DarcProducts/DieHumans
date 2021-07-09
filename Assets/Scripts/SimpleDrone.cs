using UnityEngine;
using UnityEngine.Events;

public enum DroneState { moving, attacking }
public class SimpleDrone : AIUtilities, IDamagable<float>
{
    public DroneState state = DroneState.moving;
    public static UnityAction<GameObject> UpdateDroneCount;
    public static UnityAction<Vector3, float, byte> DroneExploded;
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    [SerializeField] float maxHealth;
    float currentHealth;
    [SerializeField] float weaponDamage = 0f;
    [SerializeField] float fireRate;
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] float targetCheckDistance = 10f;
    [SerializeField] LineRenderer laser;
    [SerializeField] float laserWidth;
    [SerializeField] float explosionSize = 0f;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] AudioSource audioSource;
    float fireTime;
    EnemyManager enemyManager;
    GameObject attackTarget = null;
    Vector3 targetLocation = Vector3.zero;
    readonly bool isHittingTarget = false;

    void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager.Equals(null))
            Debug.LogWarning($"Cound not locate a GameObject with an EnemyManager component attached");
    }

    void OnEnable()
    {
        fireTime = fireRate;
        state = DroneState.moving;
        attackTarget = null;
        currentHealth = maxHealth;
        if (enemyManager != null)
            targetLocation = enemyManager.FindLocationWithinArea();
        if (laser != null)
        {
            laser.startWidth = laserWidth;
            laser.endWidth = laserWidth;
            laser.textureMode = LineTextureMode.Tile;
        }
    }

    void OnDisable()
    {
        DeactivateLaser(laser);
        CancelInvoke(nameof(LaserShot));
    }

    void LateUpdate()
    {
        switch (state)
        {
            case DroneState.moving:
                transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.fixedDeltaTime);
                transform.LookAt(targetLocation);
                if (transform.position.Equals(targetLocation))
                    state = DroneState.attacking;
                break;
            case DroneState.attacking:
                AttackTarget();
                break;
            default:
                break;
        }
    }

    GameObject FindNearestTarget()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, targetCheckDistance, targetLayers);
        if (closeObjects.Length > 0)
        {
            int randObj = Random.Range(0, closeObjects.Length);
            attackTarget = closeObjects[randObj].gameObject;
            return attackTarget;
        }
        attackTarget = null;
        return attackTarget;
    }

    void AttackTarget()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform.position);
            fireTime = fireTime < 0 ? 0 : fireTime -= Time.fixedDeltaTime;
            if (fireTime <= 0)
            {
                LaserShot();
                fireTime = fireRate;
            }
            if (!attackTarget.activeSelf)
                attackTarget = null;
            return;
        }
        if (attackTarget == null || !isHittingTarget)
        {
            attackTarget = FindNearestTarget();
            return;
        }
        else if (attackTarget == null && !isHittingTarget && enemyManager != null)
        {
            targetLocation = enemyManager.FindLocationWithinArea();
            state = DroneState.moving;
        }
    }

    void LaserShot() => ShootALaser(laser, transform.position + transform.forward * 6, attackTarget.transform.position - transform.position, weaponDamage, audioSource);

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(transform.position, 2, 0, 0);
            Die();
        }
    }

    void Die()
    {
        DroneExploded?.Invoke(transform.position, explosionSize, 0);
        UpdateDroneCount?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    [ContextMenu("Kill Drone")]
    public void KillDrone() => ApplyDamage(1000000);

    public float GetCurrentHealth() => currentHealth;
}