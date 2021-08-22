using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    public GameEvent WeaponFired;
    [SerializeField] Transform weaponLocation;
    [SerializeField] BoolVariable fireWeapon;
    [SerializeField] FloatVariable weaponDamage;
    [SerializeField] GameObject aimTarget;
    [SerializeField] FloatVariable aimTargetDistance;
    [SerializeField] FloatVariable gunFireRate;
    [SerializeField] FloatVariable projectileForce;
    [SerializeField] float projectileSize;
    [SerializeField] LayerMask ignoreDamageLayer;
    [SerializeField] ObjectPooler projectilePool;
    float currentRate;

    void Start()
    {
        currentRate = gunFireRate.Value;
        InitializeAimTarget();
    }

    private void FixedUpdate()
    {
        if (fireWeapon.Value)
            FireProjectiles();
    }

    void InitializeAimTarget()
    {
        aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance.Value);
    }

    void FireProjectiles()
    {
        currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
        if (currentRate <= 0 && aimTarget != null && Vector3.Dot(weaponLocation.localPosition, aimTarget.transform.localPosition) > .3f)
        {
            GameObject p = projectilePool.GetObject();
            if (p != null)
            {
                p.transform.localScale = Vector3.one * projectileSize;
                p.layer = 14;
                p.transform.position = weaponLocation.position;
                Projectile j = p.GetComponent<Projectile>();
                Rigidbody r = p.GetComponent<Rigidbody>();
                if (j != null)
                    j.currentDamage = weaponDamage.Value;
                p.SetActive(true);
                if (r != null)
                    r.AddForce(projectileForce.Value * (aimTarget.transform.position - weaponLocation.position + weaponLocation.forward).normalized, ForceMode.Impulse);
                WeaponFired?.Raise();
            }
            currentRate = gunFireRate.Value;
        }
    }

    public float GetCurrentWeaponDamage() => weaponDamage.Value;

    public void TryDamagingTarget(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            if (Utilities.IsInLayerMask(target, ignoreDamageLayer))
                return;
            d.ApplyDamage(GetCurrentWeaponDamage());
        }
    }
}