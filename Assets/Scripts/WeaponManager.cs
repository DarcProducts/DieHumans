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
    [SerializeField] GameObject pickupEffect;
    [SerializeField] float pickupEffectDuration;
    [SerializeField] TMP_Text pickupText;
    [SerializeField] Vector2Variable playerShipSpeed;
    [SerializeField] IntVariable maxShipSpeed;
    ObjectPools objectPools;
    GameObject player;
    float currentRate;

    void OnEnable()
    {
        DropBox.BoxAction += AquiredBox;
    }

    void OnDisable()
    {
        DropBox.BoxAction -= AquiredBox;
    }

    private void Awake()
    {
        objectPools = FindObjectOfType<ObjectPools>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
        if (pickupEffect != null)
            pickupEffect.SetActive(false);
        currentRate = gunFireRate.value;
        InitializeAimTarget();
    }

    private void FixedUpdate()
    {
        if (fireWeapon != null)
            if (fireWeapon.value)
                FireProjectiles();
    }

    public IEnumerator StartPickUpBoxEffect()
    {
        if (pickupEffect != null)
        {
            pickupEffect.SetActive(true);
            yield return new WaitForSeconds(pickupEffectDuration);
            pickupEffect.SetActive(false);
        }
    }

    IEnumerator DisplayPickupText(string message)
    {
        if (pickupText != null)
        {
            pickupText.text = message;
            pickupText.gameObject.SetActive(true);
            yield return new WaitForSeconds(pickupEffectDuration);
            pickupText.gameObject.SetActive(false);
        }
    }

    void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance.value);
    }

    void FireProjectiles()
    {
        if (objectPools != null && player != null)
        {
            currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
            if (currentRate <= 0 && aimTarget != null)
            {
                GameObject p = objectPools.GetProjectile();
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

    public void AquiredBox(byte type, float value)
    {
        StartCoroutine(StartPickUpBoxEffect());
        switch (type)
        {
            case 0:
                IncreaseDamage(value);
                StartCoroutine(DisplayPickupText($"+ { value } Damage"));
                break;

            case 1:
                InceaseProjectileSpeed(value);
                StartCoroutine(DisplayPickupText($"+ { value } Projectile Speed"));
                break;

            case 2:
                DecreaseFireRate(value);
                StartCoroutine(DisplayPickupText($"- { value } Fire Rate"));
                break;

            case 3:
                IncreaseShipThrust(value);
                StartCoroutine(DisplayPickupText($"+ { value } Ship Speed"));
                break;

            default:
                break;
        }
    }

    void IncreaseShipThrust(float value)
    {
        if (playerShipSpeed != null)
        {
            if (playerShipSpeed.value.x >= maxShipSpeed.value)
            {
                playerShipSpeed.value.x = maxShipSpeed.value;
                playerShipSpeed.value.y = playerShipSpeed.value.x * .5f;
                StartCoroutine(DisplayPickupText("Max Ship Speed"));
            }
            else playerShipSpeed.value += new Vector2(value, value * .5f);
        }
    }

    void DecreaseFireRate(float value)
    {
        gunFireRate.value = gunFireRate.value <= .1f ? .1f : gunFireRate.value -= value;
        if (gunFireRate.value > .1f)
        {
            gunFireRate.value = .1f;
            StartCoroutine(DisplayPickupText("Max Fire Rate"));
        }
        else
            gunFireRate.value += value;
    }

    void IncreaseDamage(float value)
    {
        weaponDamage.value = weaponDamage.value > maxWeaponDamage.value ? maxWeaponDamage.value : weaponDamage.value += value;
        if (weaponDamage.value > maxWeaponDamage.value)
        {
            weaponDamage.value = maxWeaponDamage.value;
            StartCoroutine(DisplayPickupText("Max Damage"));
        }
        else
            weaponDamage.value += value;
    }

    void InceaseProjectileSpeed(float value)
    {
        projectileForce = projectileForce > 200 ? 200 : projectileForce += value;
        if (projectileForce > 200)
        {
            projectileForce = 200;
            StartCoroutine(DisplayPickupText("Max Projectile Speed"));
        }
        else projectileForce += value;
    }

    public GameObject GetPlayer() => player;
}