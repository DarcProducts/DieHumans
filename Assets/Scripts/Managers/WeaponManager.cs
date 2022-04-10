using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    public GlobalBoolVariable fireButton;
    public GlobalBoolVariable bombButton;
    public UnityEvent WeaponFired;
    public Transform weaponLocation;
    public float weaponDamage;
    public GameObject aimTarget;
    public float aimTargetDistance;
    public float gunFireRate;
    public float projectileForce;
    public float projectileSize;
    public LayerMask ignoreDamageLayer;
    public ObjectPooler projectilePool;
    public ObjectPooler turretPool;
    public ObjectPooler bombPool;
    int _currentTurrets = 0;
    float _currentRate;
    bool _hasDroppedBomb;

    void Start()
    {
        _currentRate = gunFireRate;
        InitializeAimTarget();
    }

    void FixedUpdate()
    {
        if (fireButton.Value.Equals(true))
            FireProjectiles();
        //if (bombButton.Value.Equals(true))
        //{
            //TryDroppingBomb();
        //}
    }

    void TryDroppingBomb()
    {
        if (!_hasDroppedBomb)
        {
            _hasDroppedBomb = true;
            GameObject bomb = bombPool.GetObject();
        }
    }

    void InitializeAimTarget() => aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);

    bool IsTurretAvailable() => _currentTurrets > 0;

    void FireProjectiles()
    {
        _currentRate = _currentRate <= 0 ? 0 : _currentRate -= Time.fixedDeltaTime;
        if (_currentRate <= 0 && aimTarget != null && Vector3.Dot(weaponLocation.forward, aimTarget.transform.forward) > .6f)
        {
            if (IsTurretAvailable())
                DropTurret();
            else
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
                        j.currentDamage = weaponDamage;
                    p.SetActive(true);
                    if (r != null)
                        r.AddForce(projectileForce * (aimTarget.transform.position - weaponLocation.position + weaponLocation.forward).normalized, ForceMode.Impulse);
                    WeaponFired?.Invoke();
                }
            }
            _currentRate = gunFireRate;
        }
    }

    public void DropTurret()
    {
        GameObject newTurret = turretPool.GetObject();
        newTurret.transform.SetPositionAndRotation(weaponLocation.position, Quaternion.identity);
        newTurret.SetActive(true);
        _currentTurrets--;
    }

    public float GetCurrentWeaponDamage() => weaponDamage;

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