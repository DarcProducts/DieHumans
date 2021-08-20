using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    public static UnityAction WeaponFired;
    [SerializeField] BoolVariable fireWeapon;
    [SerializeField] FloatVariable weaponDamage;
    [SerializeField] IntVariable maxWeaponDamage;
    [SerializeField] GameObject aimTarget;
    [SerializeField] FloatVariable aimTargetDistance;
    [SerializeField] FloatVariable gunFireRate;
    [SerializeField] FloatVariable aimRotateSpeed;
    [SerializeField] float projectileForce;
    [SerializeField] float projectileSize;
    [SerializeField] LayerMask ignoreDamageLayer;
    [SerializeField] Vector2Variable playerShipSpeed;
    [SerializeField] IntVariable maxShipSpeed;
    [SerializeField] MultiPooler objectPooler;
    GameObject player;
    float currentRate;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        currentRate = gunFireRate.value;
        InitializeAimTarget();
    }

    private void FixedUpdate()
    {
        if (fireWeapon != null)
            if (fireWeapon.value)
                FireProjectiles();
    }

    void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance.value);
    }

    void FireProjectiles()
    {
        if (objectPooler != null && player != null)
        {
            currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
            if (currentRate <= 0 && aimTarget != null)
            {
                GameObject p = objectPooler.GetObject(1);
                if (p != null)
                {
                    p.transform.localScale = Vector3.one * projectileSize;
                    p.layer = 14;
                    p.transform.position = player.transform.position + player.transform.forward * 6;
                    Projectile j = p.GetComponent<Projectile>();
                    Rigidbody r = p.GetComponent<Rigidbody>();
                    if (j != null)
                        j.currentDamage = weaponDamage.value;
                    p.SetActive(true);
                    if (r != null)
                        r.AddForce(projectileForce * (aimTarget.transform.position - player.transform.position + player.transform.forward).normalized, ForceMode.Impulse);
                    WeaponFired?.Invoke();
                }
                currentRate = gunFireRate.value;
            }
        }
    }

    public float GetCurrentWeaponDamage() => weaponDamage.value;

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

    public GameObject GetPlayer() => player;
}