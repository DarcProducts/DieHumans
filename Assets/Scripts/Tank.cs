using UnityEngine;
using UnityEngine.Events;

public enum TankState { moving, attacking }

public class Tank : AIUtilities, IDamagable<float>
{
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, float, byte> TankExploded;
    public static UnityAction<GameObject> UpdateTank;
    public static UnityAction UpdateTankKillCount;
    public static UnityAction TankShot;
    [SerializeField] TankState state;
    [SerializeField] float targetCheckDistance;
    [SerializeField] Vector3 minCenterArea;
    [SerializeField] Vector3 maxCenterArea;
    [SerializeField] float checkDistance;
    [SerializeField] float maxHealth;
    [SerializeField] float tankSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform barrelTip;
    [SerializeField] float mortarDamage = 500f;
    [SerializeField] float mortarRadius = 10f;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] float fireRate;
    CityGenerator cityGenerator;
    ObjectPools objectPools;
    float currentFireRate = 0f;
    Vector3 targetLocation;
    GameObject currentTarget = null;
    float currentHealth;

    void Awake()
    {
        cityGenerator = FindObjectOfType<CityGenerator>();
        objectPools = FindObjectOfType<ObjectPools>();
    }

    void Start() => InitialSetup();

    void OnEnable() => InitialSetup();

    void InitialSetup()
    {
        currentTarget = null;
        currentFireRate = fireRate;
        if (cityGenerator != null)
            targetLocation = cityGenerator.GetAttackVector();
        transform.LookAt(targetLocation);
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case TankState.moving:
                if (currentTarget != null)
                    state = TankState.attacking;
                if (currentTarget == null)
                    FindRandomCloseTarget();
                if (transform.position == targetLocation)
                {
                    if (cityGenerator != null)
                        targetLocation = cityGenerator.GetAttackVector();
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetLocation, tankSpeed * Time.fixedDeltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position), rotateSpeed * Time.fixedDeltaTime);
                }
                break;

            case TankState.attacking:
                AttackTarget();
                break;

            default:
                break;
        }
    }

    void AttackTarget()
    {
        if (currentTarget != null)
        {
            if (currentTarget.activeSelf)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position), rotateSpeed * Time.fixedDeltaTime);
                targetLocation = currentTarget.transform.position;
                currentFireRate = currentFireRate < 0 ? 0 : currentFireRate -= Time.fixedDeltaTime;
                if (currentFireRate.Equals(0))
                {
                    LaunchMortar();
                    TankShot?.Invoke();
                    currentFireRate = fireRate;
                }
            }
            if (!currentTarget.activeSelf)
                FindRandomCloseTarget();
        }
        if (currentTarget == null)
        {
            state = TankState.moving;
            return;
        }
    }

    void FindRandomCloseTarget()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, targetCheckDistance, targetLayers);
        if (closeObjects.Length > 0)
        {
            int randObj = Random.Range(0, closeObjects.Length);
            currentTarget = closeObjects[randObj].gameObject;
            return;
        }
        currentTarget = null;
    }

    void LaunchMortar()
    {
        if (objectPools != null)
        {
            GameObject r = objectPools.GetRocket();
            Rocket rocket = r.GetComponent<Rocket>();
            if (r != null && rocket != null && barrelTip != null)
            {
                r.transform.position = barrelTip.position;
                rocket.type = RocketType.Mortar;
                rocket.currentTarget = currentTarget.transform.position + Vector3.up * 45f;
                rocket.rocketSpeed = 1200f;
                rocket.rocketDamage = mortarDamage;
                rocket.explosionRadius = mortarRadius;
                r.SetActive(true);
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TankExploded?.Invoke(transform.position, 3, 2);
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth * 2);
            UpdateTank?.Invoke(gameObject);
            UpdateTankKillCount?.Invoke();
            gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, targetLayers))
        {
            if (currentTarget == null)
                FindRandomCloseTarget();
        }
    }

    [ContextMenu("Kill Tank")]
    public void KillTank() => ApplyDamage(maxHealth);

    public float GetCurrentHealth() => currentHealth;
}