using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    public static UnityAction<GameObject> LaserHitObject;
    [SerializeField] private float weaponDamage;
    [SerializeField] private GameObject ship;
    [SerializeField] private GameObject weaponLocation;
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private float aimTargetDistance;
    [SerializeField] private float laserWidth;
    [SerializeField] private LineRenderer shipLaser;
    [SerializeField] private LayerMask laserHitLayers;
    [SerializeField] private Material laserMat;
    [SerializeField] private GameObject laserHitEffect;
    private GameObject currentLaserEffect;

    private void Start()
    {
        InitializeAimTarget();
        InitializeLaser();
        DeactivateLaser();
    }

    public void ActivateLaser()
    {
        if (shipLaser != null && weaponLocation != null && aimTarget != null)
        {
            shipLaser.enabled = true;

            if (!GetTargetHitLocation())
            {
                shipLaser.positionCount = 2;
                shipLaser.SetPosition(0, weaponLocation.transform.position);
                shipLaser.SetPosition(1, aimTarget.transform.position);
                if (currentLaserEffect != null)
                    currentLaserEffect.SetActive(false);
            }
        }
    }

    private void EnableLaserEffect(Vector3 loc)
    {
        if (currentLaserEffect == null)
            currentLaserEffect = Instantiate(laserHitEffect, Vector3.down, Quaternion.identity);
        if (currentLaserEffect != null)
        {
            currentLaserEffect.SetActive(true);
            currentLaserEffect.transform.position = loc;
        }
    }

    public void DeactivateLaser()
    {
        if (shipLaser != null)
        {
            shipLaser.enabled = false;

            if (currentLaserEffect != null)
            {
                currentLaserEffect.SetActive(false);
                currentLaserEffect.transform.position = weaponLocation.transform.position;
            }

            if (laserHitEffect != null)
                laserHitEffect.SetActive(false);
        }
    }

    private bool GetTargetHitLocation()
    {
        Debug.DrawRay(weaponLocation.transform.position, aimTarget.transform.position - weaponLocation.transform.position, Color.red);
        if (Physics.Raycast(weaponLocation.transform.position, aimTarget.transform.position - weaponLocation.transform.position,
            out RaycastHit hitInfo, Mathf.Infinity, laserHitLayers))
        {
            shipLaser.positionCount = 2;
            shipLaser.SetPosition(0, weaponLocation.transform.position);
            shipLaser.SetPosition(1, hitInfo.point);
            EnableLaserEffect(hitInfo.point);
            TryDamagingTarget(hitInfo.collider.gameObject);
            LaserHitObject?.Invoke(hitInfo.collider.gameObject);
            return true;
        }
        return false;
    }

    private void TryDamagingTarget(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(weaponDamage);
    }

     public void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);
    }

    public void InitializeLaser()
    {
        if (shipLaser != null)
        {
            shipLaser.material = laserMat;
            shipLaser.startWidth = laserWidth;
            shipLaser.endWidth = laserWidth;
        }
    }

    public float GetWeaponDamage() => weaponDamage;
}