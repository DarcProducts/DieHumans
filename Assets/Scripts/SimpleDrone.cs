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
    [SerializeField] float rotateSpeed = 0f;
    [SerializeField] float targetCheckDistance = 40f;
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
    bool isHittingTarget = false;

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
        if (laser != null)
            laser.enabled = false;
    }

    void LateUpdate()
    {
        switch (state)
        {
            case DroneState.moving:
                transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.fixedDeltaTime);
                transform.LookAt(targetLocation);
                if (transform.position.Equals(targetLocation))
                {
                    if (FindNearestTarget())
                        state = DroneState.attacking;
                }
                break;
            case DroneState.attacking:
                AttackTarget();
                break;
            default:
                break;
        }
    }
    public void CreateLaserLine(LineRenderer line, Vector3 from, Vector3 dir)
    {
        if (line != null)
        {
            line.enabled = true;
            if (Physics.Raycast(from, dir, out RaycastHit hitInfo))
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, hitInfo.point);
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, dir);
            }
        }
    }

    bool FindNearestTarget()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, targetCheckDistance, targetLayers);
        if (closeObjects.Length > 0)
        {
            int randObj = Random.Range(0, closeObjects.Length);
            attackTarget = closeObjects[randObj].gameObject;
            return true;
        }
        attackTarget = null;
        return false;
    }

    void AttackTarget()
    {
        if (attackTarget != null)
        {
            if (!CheckIfPathClear(gameObject, attackTarget, targetCheckDistance))
            {
                isHittingTarget = false;
                FindNearestTarget();
                return;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackTarget.transform.position - transform.position), rotateSpeed * Time.fixedDeltaTime);
            fireTime = fireTime < 0 ? 0 : fireTime -= Time.fixedDeltaTime;
            if (isHittingTarget)
                CreateLaserLine(laser, transform.position + transform.forward * 6, attackTarget.transform.position - transform.position);
            else if (!isHittingTarget && laser != null)
                laser.enabled = false;
            if (fireTime <= 0)
            {
                TryDamagingTarget(attackTarget, weaponDamage);
                if (audioSource != null)
                    LaserActivated?.Invoke(audioSource);
                fireTime = fireRate;
            }
            if (!attackTarget.activeSelf)
            {
                FindNearestTarget();
                return;
            }
        }
        if (!isHittingTarget)
        {
            FindNearestTarget();
            return;
        }
        if (attackTarget == null)
        {
            if (enemyManager != null)
                targetLocation = enemyManager.FindLocationWithinArea();
            state = DroneState.moving;
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth);
            Die();
        }
    }

    void Die()
    {
        DroneExploded?.Invoke(transform.position, explosionSize, 0);
        UpdateDroneCount?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    void TryDamagingTarget(GameObject target, float damage)
    {
        if (IsInLayerMask(target, ignoreLayer))
            return;
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            isHittingTarget = true;
            d.ApplyDamage(damage);
        }
        else isHittingTarget = false;
    }

    bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    [ContextMenu("Kill Drone")]
    public void KillDrone() => ApplyDamage(maxHealth);

    public float GetCurrentHealth() => currentHealth;
}

