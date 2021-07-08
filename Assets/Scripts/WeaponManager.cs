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

    void Start()
    {
        currentRate = gunFireRate;
        objectPools = FindObjectOfType<ObjectPools>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (objectPools == null)
            Debug.LogWarning("An object with a EnemyManager component could not be found in scene");
        if (player == null)
            Debug.LogWarning("No GameObject with Player tag could be found in scene");
        InitializeAimTarget();
    }

    void LateUpdate()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && player != null && aimTarget != null)
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, aimTarget.transform.rotation, rotateSpeed * Time.smoothDeltaTime);
    }

    void LaunchRocket()
    {
        canLaunchRocket = false;
        if (player != null && objectPools != null)
        {
            if (currentRocketAmount > 0)
            {
                currentRocket = objectPools.GetAvailableRocket();
                Rocket r = currentRocket.GetComponent<Rocket>();
                if (currentRocket != null && r != null)
                {
                    currentRocket.transform.position = player.transform.position + Vector3.down;
                    r.rocketDamage = rocketDamage;
                    r.type = RocketType.homing;
                    currentRocket.SetActive(true);
                    if (isHoldingRocketButton)
                        currentRocket.transform.position = Vector3.MoveTowards(currentRocket.transform.position, aimTarget.transform.position, rocketHomingSpeed * Time.fixedDeltaTime);
                    if (!isHoldingRocketButton)
                    {
                        r.type = RocketType.standard;
                        currentRocketAmount--;
                        canLaunchRocket = true;
                        currentRocket = null;
                    }
                }
            }
        }
    }

    void FireCurrentWeapon()
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

    void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);
    }

    void FireProjectiles()
    {
        if (objectPools != null && player != null)
        {
            GameObject p = objectPools.GetProjectile();

            currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
            if (currentRate <= 0 && p != null && aimTarget != null)
            {
                p.transform.position = player.transform.position + player.transform.forward * 6;
                p.GetComponent<Projectile>().currentDamage = gunDamage;
                p.SetActive(true);
                p.GetComponent<Rigidbody>().AddForce(projectileForce * (aimTarget.transform.position - player.transform.position + player.transform.forward).normalized, ForceMode.Impulse);
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
        if (type == 0)
            IncreaseDamage(value);
        else if (type == 1)
            InceaseProjectileSpeed(value);
        else
            DecreaseFireRate(value);
    }

    public void DecreaseFireRate(float value) => gunFireRate = gunFireRate <= .01f ? .01f : gunFireRate -= value;

    public void IncreaseDamage(float value) => gunDamage += value;

    public void InceaseProjectileSpeed(float value) => projectileForce = projectileForce > 500 ? 500 : projectileForce += value;

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => ((layerMask.value & (1 << obj.layer)) > 0);

    public bool GetIsRotatingShip() => isRotatingShip;
}