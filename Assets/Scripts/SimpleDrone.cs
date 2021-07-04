using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleDrone : AIUtilities, IDamagable<float>
{
    public bool isActivated = false;
    public static UnityAction<GameObject> UpdateDroneCount;
    public static UnityAction<Vector3, float> DroneExploded;
    public static UnityAction<GameObject, byte> TextInfo;
    [SerializeField] float maxHealth;
    float currentHealth;
    [SerializeField] float weaponDamage = 0f;
    [SerializeField] float moveSpeed = 0f;
    [SerializeField] Vector2 minMaxHeight = Vector2.zero;
    [SerializeField] float maxSpreadDistance = 10f;
    [SerializeField] float targetCheckDistance = 10f;
    [SerializeField] LineRenderer laser;
    [SerializeField] float laserWidth;
    [SerializeField] float explosionSize = 0f;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LayerMask targetLayers;
    Vector3 dropLocation = Vector3.zero;
    GameObject attackTarget = null;
    bool isAtLocation = false;
    Vector3 targetLocation = Vector3.zero;
    bool isHittingTarget = false;

    void OnEnable()
    {
        attackTarget = null;
        currentHealth = maxHealth;
        dropLocation = transform.position;
        targetLocation = FindLocationFromDrop();
        if (laser != null)
        { 
            laser.startWidth = laserWidth;
            laser.endWidth = laserWidth;
        }
    }

    void LateUpdate()
    {
        if (isActivated && !isAtLocation)
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, moveSpeed * Time.fixedDeltaTime);
        if (transform.position.Equals(targetLocation) && !isAtLocation)
            isAtLocation = true;
        if (isAtLocation)
            AttackTarget();
        if (attackTarget != null)
            transform.LookAt(attackTarget.transform.position);
        else
            transform.LookAt(targetLocation);
    }

    Vector3 FindLocationFromDrop() => new Vector3(Random.Range(dropLocation.x - maxSpreadDistance, dropLocation.x + maxSpreadDistance), Random.Range(minMaxHeight.x, minMaxHeight.y), Random.Range(dropLocation.z - maxSpreadDistance, dropLocation.z + maxSpreadDistance));

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
        if (attackTarget == null || !isHittingTarget)
            attackTarget = FindNearestTarget();
        if (attackTarget != null)
            ShootALaser(laser, transform.position + transform.forward * 3f, attackTarget.transform.position - transform.position, weaponDamage);
    }

    void ShootALaser(LineRenderer line, Vector3 from, Vector3 dir, float damage)
    {
        if (line != null)
        {
            if (Physics.Raycast(from, dir, out RaycastHit hitInfo))
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, hitInfo.point);
                TryDamagingTarget(hitInfo.collider.gameObject, damage);
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(0, from);
                line.SetPosition(1, dir);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(ignoreLayer))
            return;
        else
        {
            ApplyDamage(maxHealth);
            base.TryDamagingTarget(collision.gameObject, currentHealth);
            TextInfo?.Invoke(gameObject, 4);
            Die();
        }
    }

    public override void TryDamagingTarget(GameObject target, float damage)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            d.ApplyDamage(damage);
            isHittingTarget = true;
        } else
            isHittingTarget = false;
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(gameObject, 2);
            Die();
        }
    }

    void Die()
    {
        DroneExploded?.Invoke(transform.position, explosionSize);
        UpdateDroneCount?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    [ContextMenu("Kill Drone")]
    public void KillDrone() => ApplyDamage(1000000);

    public float GetCurrentHealth() => currentHealth;
}
