using UnityEngine;
using UnityEngine.Events;

public enum DroneState { moving, attacking }

public class Drone : AIUtilities, IDamagable<float>
{
    public DroneState state = DroneState.moving;
    public static UnityAction<GameObject> UpdateDrone;
    public static UnityAction<Vector3, float, byte> DroneExploded;
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction FiredLaser;
    public static UnityAction UpdateDroneKillCount;
    [SerializeField] Vector2 minMaxHeight;
    [SerializeField] IntVariable maxHealth;
    float currentHealth;
    [SerializeField] FloatVariable weaponDamage;
    [SerializeField] FloatVariable fireRate;
    [SerializeField] FloatVariable moveSpeed;
    [SerializeField] float rotateSpeed = 0f;
    [SerializeField] FloatVariable targetCheckDistance;
    [SerializeField] LineRenderer laser;
    [SerializeField] float laserWidth;
    [SerializeField] float explosionSize = 0f;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LayerMaskVariable targetLayers;
    float fireTime;
    CityGenerator cityGenerator;
    GameObject attackTarget = null;
    Vector3 targetLocation = Vector3.zero;
    bool isHittingTarget = false;

    void Awake() => cityGenerator = FindObjectOfType<CityGenerator>();

    void OnEnable()
    {
        if (fireRate != null)
            fireTime = fireRate.value;
        state = DroneState.moving;
        attackTarget = null;
        if (maxHealth != null)
            currentHealth = maxHealth.value;
        if (cityGenerator != null)
        {
            Vector3 newAttackVector = cityGenerator.GetAttackVector();
            float ranHeight = Random.Range(minMaxHeight.x, minMaxHeight.y);
            targetLocation = newAttackVector + Vector3.up * ranHeight;
        }
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

    void FixedUpdate()
    {
        if (moveSpeed != null)
        {
            switch (state)
            {
                case DroneState.moving:
                    transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed.value * Time.fixedDeltaTime);
                    transform.LookAt(targetLocation);
                    if (transform.position.Equals(targetLocation))
                        if (FindNearestTarget())
                            state = DroneState.attacking;
                    break;

                case DroneState.attacking:
                    AttackTarget();
                    break;

                default:
                    break;
            }
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
        if (targetCheckDistance != null && targetLayers != null)
        {
            Collider[] closeObjects = Physics.OverlapSphere(transform.position, targetCheckDistance.value, targetLayers.value);
            if (closeObjects.Length > 0)
            {
                int randObj = Random.Range(0, closeObjects.Length);
                attackTarget = closeObjects[randObj].gameObject;
                return true;
            }
            attackTarget = null;
        }
        return false;
    }

    void AttackTarget()
    {
        if (attackTarget != null && targetCheckDistance != null)
        {
            if (!CheckIfPathClear(gameObject, attackTarget, targetCheckDistance.value))
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
            if (fireTime <= 0 && weaponDamage != null && fireRate != null)
            {
                TryDamagingTarget(attackTarget, weaponDamage.value);
                FiredLaser?.Invoke();
                fireTime = fireRate.value;
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
            if (cityGenerator != null)
            {
                Vector3 newAttackVector = cityGenerator.GetAttackVector();
                float ranHeight = Random.Range(minMaxHeight.x, minMaxHeight.y);
                targetLocation = newAttackVector + Vector3.up * ranHeight;
            }
            state = DroneState.moving;
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0 && maxHealth != null)
        {
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth.value * 2);
            UpdateDrone?.Invoke(gameObject);
            UpdateDroneKillCount?.Invoke();
            DisableObject();
        }
    }

    void DisableObject()
    {
        DroneExploded?.Invoke(transform.position, explosionSize, 2);
        gameObject.SetActive(false);
    }

    void TryDamagingTarget(GameObject target, float damage)
    {
        if (Utilities.IsInLayerMask(target, ignoreLayer))
            return;
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            isHittingTarget = true;
            d.ApplyDamage(damage);
        }
        else isHittingTarget = false;
    }

    [ContextMenu("Kill Drone")]
    public void KillDrone()
    {
        if (maxHealth != null)
            ApplyDamage(maxHealth.value);
    }

    public float GetCurrentHealth() => currentHealth;
}