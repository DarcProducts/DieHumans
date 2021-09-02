using UnityEngine;
using UnityEngine.Events;

public enum TankState { moving, attacking }

public class Tank : AIUtilities, IDamagable<float>
{
    public static UnityAction UpdateTankKillCount;
    public static UnityAction<GameObject> RemoveFromWaveList;
    public static UnityAction TankShotSound;
    public static UnityAction TankExploded;
    public KillSheet killSheet;
    public TankState state;
    public float checkDistance;
    public int maxHealth;
    public float tankSpeed;
    public float rotateSpeed;
    public Transform barrelTip;
    public float mortarDamage;
    public float mortarRadius;
    public LayerMask targetLayers;
    public float fireRate;
    public CityGenerator cityGenerator;
    public ObjectPooler explosionPool;
    public ObjectPooler rocketPool;
    public float explosionSize;
    float currentFireRate = 0f;
    Vector3 targetLocation;
    GameObject currentTarget = null;
    float currentHealth;

    public void Start() => InitialSetup();

    public void OnEnable() => InitialSetup();

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
                    TankShotSound?.Invoke();
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
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, checkDistance, targetLayers);
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
        GameObject r = rocketPool.GetObject();
        Rocket rocket = r.GetComponent<Rocket>();
        if (r != null && rocket != null && barrelTip != null)
        {
            r.transform.position = barrelTip.position;
            rocket.type = RocketType.Mortar;
            rocket.currentTarget = currentTarget.transform.position + Vector3.up * 45f;
            rocket.rocketSpeed = 1400f;
            rocket.rocketDamage = mortarDamage;
            rocket.explosionRadius = mortarRadius;
            r.SetActive(true);
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            GameObject e = explosionPool.GetObject();
            if (e != null)
            {
                float eSize = explosionSize;
                e.transform.position = transform.position;
                e.transform.localScale = new Vector3(eSize, eSize, eSize);
                e.SetActive(true);
            }

            killSheet.tanksDestroyed++;
            UpdateTankKillCount?.Invoke();
            RemoveFromWaveList?.Invoke(this.gameObject);
            TankExploded?.Invoke();
            gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, targetLayers))
            if (currentTarget == null)
                FindRandomCloseTarget();
    }

    [ContextMenu("Kill Tank")]
    public void KillTank() => ApplyDamage(maxHealth);

    public float GetCurrentHealth() => currentHealth;
}