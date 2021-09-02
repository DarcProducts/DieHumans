using UnityEngine;
using UnityEngine.Events;

public enum DroneState { moving, attacking }

public class Drone : AIUtilities, IDamagable<float>
{
    public DroneState state = DroneState.moving;
    public static UnityAction<GameObject> RemoveFromWaveList;
    public static UnityAction FiredLaserSound;
    public static UnityAction DroneExploded;
    public static UnityAction UpdateDroneKillCount;
    public KillSheet killSheet;
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
    [SerializeField] CityGenerator cityGenerator;
    [SerializeField] ObjectPooler explosionPool;
    GameObject attackTarget = null;

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
            if (!CheckIfPathClear(gameObject, attackTarget, targetCheckDistance))
            {
                isHittingTarget = false;
                FindNearestTarget();
                return;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackTarget.transform.position - transform.position), rotateSpeed * Time.fixedDeltaTime);
            bool isFacing = Vector3.Dot(transform.forward, attackTarget.transform.position) > .6f;
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
                TryDamagingTarget(attackTarget, weaponDamage);
                FiredLaserSound?.Invoke();
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
        {
            killSheet.dronesDestroyed++;
            DisableObject();
        }
    }

    void DisableObject()
    {
        GameObject e = explosionPool.GetObject();
        e.transform.position = transform.position;
        e.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
        e.SetActive(true);
        RemoveFromWaveList?.Invoke(this.gameObject);
        DroneExploded?.Invoke();
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