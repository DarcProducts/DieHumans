using UnityEngine;
using UnityEngine.Events;

public enum WeaponState { machinegun, rocket, special }

public class WeaponManager : MonoBehaviour
{
    public WeaponState currentWeapon = WeaponState.machinegun;
    public static UnityAction MachinegunFired;
    [SerializeField] float machinegunDamage;
    [SerializeField] float rocketDamage;
    [SerializeField] float specialDamage;
    [SerializeField] GameObject aimTarget;
    [SerializeField] float aimTargetDistance;
    [SerializeField] float rocketHomingSpeed;
    [SerializeField] LineRenderer machineGunShot;
    [SerializeField] int currentRocketAmount;
    [SerializeField] LayerMask ignoreDamageLayer;
    Rocket currentRocket = null;
    bool isHoldingRocketButton = false;
    bool canLaunchRocket = true;
    ObjectPools objectPools;
    GameObject player;
    float gunFireRate = .3f;
    float currentRate;

    void OnEnable()
    {
        GameManager.FireWeaponButtonHold += FireCurrentWeapon;
        GameManager.FireWeaponButtonDown += RocketButtonTrue;
        GameManager.FireWeaponButtonUp += RocketButtonFalse;
        GameManager.FireWeaponButtonUp += DisableMachinegunBullet;
    }

    void OnDisable()
    {
        GameManager.FireWeaponButtonHold -= FireCurrentWeapon;
        GameManager.FireWeaponButtonDown -= RocketButtonTrue;
        GameManager.FireWeaponButtonUp -= RocketButtonFalse;
        GameManager.FireWeaponButtonUp -= DisableMachinegunBullet;
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

    void LaunchRocket()
    {
        canLaunchRocket = false;
        if (player != null && objectPools != null)
        {
            if (currentRocketAmount > 0)
            {
                currentRocket = objectPools.GetAvailableRocket().GetComponent<Rocket>();
                if (currentRocket != null)
                {
                    currentRocket.transform.position = player.transform.position + Vector3.down;
                    currentRocket.gameObject.SetActive(true);
                    if (isHoldingRocketButton)
                    {
                        currentRocket.rocketDamage = rocketDamage;
                        currentRocket.transform.position = Vector3.MoveTowards(currentRocket.transform.position, aimTarget.transform.position, rocketHomingSpeed * Time.fixedDeltaTime);
                    }
                    if (!isHoldingRocketButton)
                    {
                        SetCurrentRocketHoming(false);
                        currentRocketAmount--;
                        canLaunchRocket = true;
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
                FireMachineguns();
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

    void FireMachineguns()
    {
        if (machineGunShot != null && player != null)
        {
            machineGunShot.enabled = false;
            machineGunShot.positionCount = 2;
            machineGunShot.SetPosition(0, player.transform.position + player.transform.forward);
            if (Physics.Raycast(player.transform.position + player.transform.forward, aimTarget.transform.position - player.transform.position + player.transform.forward, out RaycastHit hitInfo))
            {
                machineGunShot.SetPosition(1, hitInfo.point);
                TryDamagingTarget(hitInfo.collider.gameObject);
            }
            else
                machineGunShot.SetPosition(1, aimTarget.transform.position);
            currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
            if (currentRate <= 0)
            {
                MachinegunFired?.Invoke();
                machineGunShot.enabled = true;
                currentRate = gunFireRate;
            }
        }
    }

    void DisableMachinegunBullet()
    {
        if (machineGunShot != null)
            machineGunShot.enabled = false;
    }

    public float GetCurrentWeaponDamage()
    {
        switch (currentWeapon)
        {
            case WeaponState.machinegun:
                return machinegunDamage;

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
            if (target.layer.Equals(ignoreDamageLayer))
                return;
            d.ApplyDamage(GetCurrentWeaponDamage());
        }
    }

    void SetCurrentRocketHoming(bool value)
    {
        if (currentRocket != null)
            currentRocket.GetComponent<Rocket>().isHoming = value;
    }
}