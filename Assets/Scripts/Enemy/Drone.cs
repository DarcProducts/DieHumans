using UnityEngine;
using UnityEngine.Events;

public enum DroneState { moving, attacking }

public class Drone : AIUtilities, IDamagable<float>
{
    public DroneState state = DroneState.moving;
    [SerializeField] GameEvent objectExploded;
    public static UnityAction UpdateDroneKillCount;
    public Vector2 minMaxHeight;
    public int maxHealth;
    float currentHealth;
    public float weaponDamage;
    public float fireRate;
    public float moveSpeed;
    public float rotateSpeed = 0f;
    public float targetCheckDistance;
    public LineRenderer laser1;
    public LineRenderer laser2;
    public float laserWidth;
    public float explosionSize = 0f;
    public LayerMask ignoreLayer;
    public LayerMask targetLayers;
    float fireTime;
    [SerializeField] bool ignoreLineOfSight;
    [SerializeField] CityGenerator cityGenerator;
    GameObject attackTarget = null;
    [SerializeField] UnityEvent<GameObject> firedLaser;

    Vector3 targetLocation = Vector3.zero;
    bool isHittingTarget = false;


    void OnEnable()
    {
        fireTime = fireRate;
        state = DroneState.moving;
        attackTarget = null;
        currentHealth = maxHealth;
        if (cityGenerator != null)
        {
            Vector3 newAttackVector = cityGenerator.GetAttackVector();
            float ranHeight = Random.Range(minMaxHeight.x, minMaxHeight.y);
            targetLocation = newAttackVector + Vector3.up * ranHeight;
        }
        if (laser1 != null)
        {
            laser2.startWidth = laserWidth;
            laser2.endWidth = laserWidth;
            laser2.textureMode = LineTextureMode.Tile;
        }
        if (laser2 != null)
        {
            laser2.startWidth = laserWidth;
            laser2.endWidth = laserWidth;
            laser2.textureMode = LineTextureMode.Tile;
        }
    }

    void OnDisable()
    {
        if (laser1 != null)
            laser1.enabled = false;
        if (laser2 != null)
            laser2.enabled = false;
        UpdateDroneKillCount?.Invoke();
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case DroneState.moving:
                transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.fixedDeltaTime);
                transform.forward = Vector3.Lerp(transform.forward, targetLocation - transform.position, .2f);
                if (transform.position == targetLocation)
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
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, targetCheckDistance, targetLayers.value);
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
            if (!ignoreLineOfSight)
            {
                if (!CheckIfPathClear(gameObject, attackTarget, targetCheckDistance))
                {
                    isHittingTarget = false;
                    FindNearestTarget();
                    return;
                }
            }
            bool isFacing = Vector3.Dot(transform.forward, attackTarget.transform.position.normalized) > .6f;
            fireTime = fireTime < 0 ? 0 : fireTime -= Time.fixedDeltaTime;
            if (isHittingTarget)
            {
                CreateLaserLine(laser1, laser1.transform.position, attackTarget.transform.position - laser1.transform.position);
                CreateLaserLine(laser2, laser2.transform.position, attackTarget.transform.position - laser2.transform.position);
            }
            else if (!isHittingTarget && laser1 != null && laser2 != null)
            {
                laser1.enabled = false;
                laser2.enabled = false;
            }
            if (fireTime <= 0 && isFacing)
            {
                transform.forward = Vector3.Lerp(transform.forward, attackTarget.transform.position - transform.position, .2f);
                TryDamagingTarget(attackTarget, weaponDamage);
                firedLaser.Invoke(gameObject);
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
        if (currentHealth <= 0)
            ObjectDestroyed();
    }

    void ObjectDestroyed()
    {
        objectExploded.Invoke(gameObject);
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
    public void KillDrone() => ApplyDamage(maxHealth);

    public float GetCurrentHealth() => currentHealth;
}