using UnityEngine;
using UnityEngine.Events;

public enum TankState { moving, attacking }
public class Tank : AIUtilities, IDamagable<float>
{
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, float, byte> TankExploded;
    public static UnityAction<GameObject> UpdateTank;
    [SerializeField] TankState state;
    [SerializeField] float targetCheckDistance;
    [SerializeField] Vector3 minCenterArea;
    [SerializeField] Vector3 maxCenterArea;
    [SerializeField] float checkDistance;
    [SerializeField] float maxHealth;
    [SerializeField] float tankSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform barrelTip;
    [SerializeField] float mortarDamage = 250f;
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
        if (objectPools == null)
            Debug.LogWarning($"Cannot find a GameObject with the ObjectPools component attached");
        if (cityGenerator == null)
            Debug.LogWarning($"Cannot find a GameObject with the CityGenerator component attached");
    }

    void Start() => InitialSetup();

    void OnEnable() => InitialSetup();

    void OnDisable() => UpdateTank?.Invoke(gameObject);

    void InitialSetup()
    {
        currentFireRate = fireRate;
        targetLocation = GetLocationWithinArea();
        transform.LookAt(targetLocation);
        currentHealth = maxHealth;
    }

    void LateUpdate()
    {
        switch (state)
        {
            case TankState.moving:
                if (currentTarget != null)
                    state = TankState.attacking;
                if (currentTarget == null)
                    FindNearestTarget();
                if (transform.position.Equals(targetLocation))
                    targetLocation = GetLocationWithinArea();
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
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitDown, 20))
            transform.position = hitDown.point;
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
                    currentFireRate = fireRate;
                }
            }
            if (!currentTarget.activeSelf)
                FindNearestTarget();
        }
        if (currentTarget == null)
        {
            state = TankState.moving;
            return;
        }
    }

    void FindNearestTarget()
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

    Vector3 GetLocationWithinArea() => new Vector3(UnityEngine.Random.Range(minCenterArea.x, maxCenterArea.x), UnityEngine.Random.Range(minCenterArea.y, maxCenterArea.y), UnityEngine.Random.Range(minCenterArea.z, maxCenterArea.z));


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
                rocket.rocketSpeed = 1100f;
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
            TankExploded?.Invoke(transform.position, 3, 1);
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth);
            gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (IsInLayerMask(other.gameObject, targetLayers))
        {
            if (currentTarget == null)
                FindNearestTarget();
        }
    }

    bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    [ContextMenu("Kill Tank")]
    public void KillTank() => ApplyDamage(maxHealth);

    public float GetCurrentHealth() => currentHealth;
}