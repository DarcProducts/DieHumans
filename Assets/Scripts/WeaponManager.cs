using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum WeaponState { machinegun, rocket, special }

public class WeaponManager : MonoBehaviour
{
    public WeaponState currentWeapon = WeaponState.machinegun;
    public static UnityAction WeaponFired;
    [SerializeField] float gunDamage;
    [SerializeField] float rocketDamage;
    [SerializeField] float specialDamage;
    [SerializeField] GameObject aimTarget;
    [SerializeField] float aimTargetDistance;
    [SerializeField] float rocketHomingSpeed;
    [SerializeField] int currentRocketAmount;
    [SerializeField] float gunFireRate;
    [SerializeField] float rotateSpeed;
    [SerializeField] float projectileForce;
    [SerializeField] LayerMask ignoreDamageLayer;
    [SerializeField] GameObject pickupEffect;
    [SerializeField] float pickupEffectDuration;
    [SerializeField] TMP_Text pickupText;
    PlayerManager playerManager;
    readonly bool isRotatingShip;
    GameObject currentRocket = null;
    bool isHoldingRocketButton = false;
    bool canLaunchRocket = true;
    ObjectPools objectPools;
    GameObject player;
    float currentRate;

    void OnEnable()
    {
        GameManager.FireWeaponButtonHold += FireCurrentWeapon;
        GameManager.FireWeaponButtonDown += RocketButtonTrue;
        GameManager.FireWeaponButtonUp += RocketButtonFalse;
        DropBox.BoxAction += AquiredBox;
    }

    void OnDisable()
    {
        GameManager.FireWeaponButtonHold -= FireCurrentWeapon;
        GameManager.FireWeaponButtonDown -= RocketButtonTrue;
        GameManager.FireWeaponButtonUp -= RocketButtonFalse;
        DropBox.BoxAction -= AquiredBox;
    }

    void RocketButtonFalse() => isHoldingRocketButton = false;

    void RocketButtonTrue() => isHoldingRocketButton = true;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        objectPools = FindObjectOfType<ObjectPools>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (objectPools == null)
            Debug.LogWarning("An object with a EnemyManager component could not be found in scene");
        if (player == null)
            Debug.LogWarning("No GameObject with Player tag could be found in scene");
        if (playerManager == null)
            Debug.LogWarning("Could not find a GameObject with PlayerManager component attached");
    }

    void Start()
    {
        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
        if (pickupEffect != null)
            pickupEffect.SetActive(false);
        currentRate = gunFireRate;
        InitializeAimTarget();
    }

    private void LateUpdate()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && player != null && aimTarget != null)
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, aimTarget.transform.rotation, rotateSpeed * Time.smoothDeltaTime);
    }

    private void LaunchRocket()
    {
        canLaunchRocket = false;
        if (player != null && objectPools != null)
        {
            if (currentRocketAmount > 0)
            {
                currentRocket = objectPools.GetRocket();
                Rocket r = currentRocket.GetComponent<Rocket>();
                if (currentRocket != null && r != null)
                {
                    currentRocket.transform.position = player.transform.position + Vector3.down;
                    r.rocketDamage = rocketDamage;
                    r.type = RocketType.Homing;
                    currentRocket.SetActive(true);
                    if (isHoldingRocketButton)
                        currentRocket.transform.position = Vector3.MoveTowards(currentRocket.transform.position, aimTarget.transform.position, rocketHomingSpeed * Time.fixedDeltaTime);
                    if (!isHoldingRocketButton)
                    {
                        r.type = RocketType.Standard;
                        currentRocketAmount--;
                        canLaunchRocket = true;
                        currentRocket = null;
                    }
                }
            }
        }
    }

    private void FireCurrentWeapon()
    {
        switch (currentWeapon)
        {
            case WeaponState.machinegun:
                FireProjectiles();
                break;

            case WeaponState.rocket:
                if (canLaunchRocket)
                    LaunchRocket();
                break;

            case WeaponState.special:

                break;
        }
    }

    public void StartPickUpBoxEffect()
    {
        if (pickupEffect != null)
        {
            pickupEffect.SetActive(true);
            Invoke(nameof(DeactivatePickupEffect), pickupEffectDuration);
        }
    }

    void DisplayPickupText(string message)
    {
        if (pickupText != null)
        {
            pickupText.text = message;
            pickupText.gameObject.SetActive(true);
            Invoke(nameof(DeactivatePickupText), pickupEffectDuration);
        }
    }

    private void DeactivatePickupEffect()
    {
        if (pickupEffect != null)
            pickupEffect.SetActive(false);
    }

    void DeactivatePickupText()
    {
        if (pickupText != null)
            pickupText.gameObject.SetActive(false);
    }

    private void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);
    }

    private void FireProjectiles()
    {
        if (objectPools != null && player != null)
        {
            GameObject p = objectPools.GetProjectile();

            currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
            if (currentRate <= 0 && p != null && aimTarget != null)
            {
                p.transform.position = player.transform.position + player.transform.forward * 6;
                Projectile j = p.GetComponent<Projectile>();
                Rigidbody r = p.GetComponent<Rigidbody>();
                if (j != null)
                    j.currentDamage = gunDamage;
                p.SetActive(true);
                if (r != null)
                    r.AddForce(projectileForce * (aimTarget.transform.position - player.transform.position + player.transform.forward).normalized, ForceMode.Impulse);
                WeaponFired?.Invoke();
                currentRate = gunFireRate;
            }
        }
    }

    public float GetCurrentWeaponDamage()
    {
        switch (currentWeapon)
        {
            case WeaponState.machinegun:
                return gunDamage;

            case WeaponState.rocket:
                return rocketDamage;

            case WeaponState.special:
                return specialDamage;

            default:
                break;
        }
        return 0;
    }

    public void TryDamagingTarget(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            if (IsInLayerMask(target, ignoreDamageLayer))
                return;
            d.ApplyDamage(GetCurrentWeaponDamage());
        }
    }

    public void AquiredBox(byte type, float value)
    {
        StartPickUpBoxEffect();
        switch (type)
        {
            case 0:
                IncreaseDamage(value);
                DisplayPickupText($"+ { value } Damage");
                break;

            case 1:
                InceaseProjectileSpeed(value);
                DisplayPickupText($"+ { value } Projectile Speed");
                break;

            case 2:
                DecreaseFireRate(value);
                DisplayPickupText($"- { value } Fire Rate");
                break;

            case 3:
                IncreaseShipThrust(value);
                DisplayPickupText($"+ { value } Ship Speed");
                break;

            default:
                break;
        }
    }

    void IncreaseShipThrust(float value)
    {
        if (playerManager != null)
        {
            if (playerManager.GetShipSpeedFB() >= 32)
            {
                DisplayPickupText("Max Ship Speed");
                return;
            }
            else
                playerManager.IncreaseShipSpeeds(value);
        }
    }

    void DecreaseFireRate(float value)
    {
        gunFireRate = gunFireRate <= .01f ? .01f : gunFireRate -= value;
        if (gunFireRate.Equals(.01f))
            DisplayPickupText("Max Fire Rate");
    }

    void IncreaseDamage(float value) => gunDamage += value;

    void InceaseProjectileSpeed(float value)
    {
        projectileForce = projectileForce > 500 ? 500 : projectileForce += value;
        if (projectileForce.Equals(500))
            DisplayPickupText("Max Projectile Speed");
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => ((layerMask.value & (1 << obj.layer)) > 0);

    public bool GetIsRotatingShip() => isRotatingShip;
}