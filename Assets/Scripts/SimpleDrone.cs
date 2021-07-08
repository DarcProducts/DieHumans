using UnityEngine;
using UnityEngine.Events;

public enum DroneState { moving, attacking }
public class SimpleDrone : AIUtilities, IDamagable<float>
{
    public DroneState state = DroneState.moving;
    public static UnityAction<GameObject> UpdateDroneCount;
    public static UnityAction<Vector3, float> DroneExploded;
    public static UnityAction<GameObject, byte, byte, float> TextInfo;
    [SerializeField] private float maxHealth;
    private float currentHealth;
    [SerializeField] private float weaponDamage = 0f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float targetCheckDistance = 10f;
    [SerializeField] private LineRenderer laser;
    [SerializeField] private float laserWidth;
    [SerializeField] private float explosionSize = 0f;
    [SerializeField] private LayerMask ignoreLayer;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private AudioSource audioSource;
    private EnemyManager enemyManager;
    private GameObject attackTarget = null;
    private Vector3 targetLocation = Vector3.zero;
    private bool isHittingTarget = false;

    void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager.Equals(null))
            Debug.LogWarning($"Cound not locate a GameObject with an EnemyManager component attached");
    }

    void OnEnable()
    {
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

    void OnDisable() => DeactivateLaser(laser);

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
            ShootALaser(laser, transform.position + transform.forward * 6, attackTarget.transform.position - transform.position, weaponDamage, audioSource);
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

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(gameObject, 2, 0, 0);
            Die();
        }
    }

    void Die()
    {
        DroneExploded?.Invoke(transform.position, explosionSize);
        UpdateDroneCount?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    [ContextMenu("Kill Drone")]
    public void KillDrone() => ApplyDamage(1000000);

    public float GetCurrentHealth() => currentHealth;
}