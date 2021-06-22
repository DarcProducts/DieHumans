using System.Collections;
using UnityEngine;

public enum WeaponState { machinegun, rocket, special }

public class WeaponManager : MonoBehaviour
{
    public WeaponState currentWeapon = WeaponState.machinegun;
    [SerializeField] private float machinegunDamage;
    [SerializeField] private float rocketDamage;
    [SerializeField] private float specialDamage;
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private float aimTargetDistance;
    [SerializeField] private float rocketThrust;
    [SerializeField] private float rocketHomingSpeed;
    [SerializeField] private LineRenderer machineGunShot;
    [SerializeField] private int currentRocketAmount;
    private GameObject currentRocket;
    private bool isHoldingRocketButton = false;
    private bool readyForRocket = false;
    private ObjectPools objectPools;
    private GameObject player;
    private float gunFireRate = .3f;
    private float currentRate;

    private void OnEnable()
    {
        GameManager.FireWeaponButtonHold += FireCurrentWeapon;
        GameManager.FireWeaponButtonDown += RocketButtonTrue;
        GameManager.FireWeaponButtonUp += RocketButtonFalse;
    }

    private void OnDisable()
    {
        GameManager.FireWeaponButtonHold -= FireCurrentWeapon;
        GameManager.FireWeaponButtonDown -= RocketButtonTrue;
        GameManager.FireWeaponButtonUp -= RocketButtonFalse;
    }

    public void RocketButtonFalse() => isHoldingRocketButton = false;
    public void RocketButtonTrue() => isHoldingRocketButton = true;

    private void Start()
    {
        currentRate = gunFireRate;
        objectPools = FindObjectOfType<ObjectPools>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (objectPools == null)
            Debug.LogError("An object with a EnemyManager component could not be found in scene");
        if (player == null)
            Debug.LogWarning("No GameObject with Player tag could be found in scene");
        InitializeAimTarget();
    }

    public IEnumerator LaunchRocket()
    {
        if (player != null && objectPools != null)
        {
            if (currentRocketAmount > 0)
            {
                currentRocket = objectPools.GetAvailableRocket();
                currentRocket.transform.position = transform.position + Vector3.down;
                currentRocket.GetComponent<Rocket>().rocketDamage = rocketDamage;
                currentRocket.SetActive(true);
                currentRocket.transform.position = Vector3.MoveTowards(currentRocket.transform.position, aimTarget.transform.position, rocketHomingSpeed * Time.fixedDeltaTime);
                yield return isHoldingRocketButton;
                SetHoming(false);
                currentRocketAmount--;
                readyForRocket = true;
            }
        }
        StopCoroutine(LaunchRocket());
    }

    public void FireCurrentWeapon()
    {
        switch (currentWeapon)
        {
            case WeaponState.machinegun:
                FireMachineguns();
                break;

            case WeaponState.rocket:
                if (readyForRocket)
                {
                    StartCoroutine(LaunchRocket());
                    readyForRocket = false;
                }
                break;

            case WeaponState.special:

                break;
        }
    }

    public void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);
    }

    private void FireMachineguns()
    {
        if (machineGunShot != null)
        {
            machineGunShot.enabled = true;
            if (Physics.Raycast(transform.position + Vector3.down, aimTarget.transform.position - transform.position + Vector3.down, out RaycastHit hitInfo))
            {
                currentRate = currentRate <= 0 ? 0 : currentRate -= Time.fixedDeltaTime;
                machineGunShot.positionCount = 2;
                machineGunShot.SetPosition(0, player.transform.position + Vector3.down);
                machineGunShot.SetPosition(1, hitInfo.point);
                if (currentRate <= 0)
                {
                    machineGunShot.enabled = false;
                    currentRate = gunFireRate;
                }
            }
        }
    }

    public float GetCurrentWeaponDamage()
    {
        float damage = 0;
        switch (currentWeapon)
        {
            case WeaponState.machinegun:
                damage = machinegunDamage;
                break;

            case WeaponState.rocket:
                damage = rocketDamage;
                break;

            case WeaponState.special:
                damage = specialDamage;
                break;
        }
        return damage;
    }

    public void SetHoming(bool value)
    {
        if (currentRocket != null)
            currentRocket.GetComponent<Rocket>().isHoming = value;
    }
}