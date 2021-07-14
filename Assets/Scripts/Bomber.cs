using UnityEngine;
using UnityEngine.Events;

public class Bomber : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, float, byte> BomberExploded;
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<GameObject> UpdateBomber;
    public static UnityAction BombDropped;
    [SerializeField] float maxHealth;
    [SerializeField] Vector2 minMaxBombHeight;
    [SerializeField] float bombDamage;
    [SerializeField] float bombRadius;
    [SerializeField] float bombDropSpeed;
    [SerializeField] float bombStartTime;
    [SerializeField] float rotateSpeed;
    float currentStartTime;
    EnemyManager enemyManager;
    bool startDroppingBombs = false;
    ObjectPools objectPools;
    float currentHealth;
    [SerializeField] float shipSpeed;
    [SerializeField] float bombDropRate;
    float currentDrop;
    Vector3 targetLocation;

    void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager == null)
            Debug.LogWarning($"Could not find GameObject with EnemyManager component on it");
        objectPools = FindObjectOfType<ObjectPools>();
        if (objectPools == null)
            Debug.LogWarning($"Could not find GameObject with ObjectPools component on it");
    }

    void Start() => currentStartTime = bombStartTime;

    void OnEnable()
    {
        currentHealth = maxHealth;
        currentStartTime = bombStartTime;
        startDroppingBombs = false;
        if (enemyManager != null)
        {
            Vector3 newLoc = enemyManager.FindLocationWithinArea();
            targetLocation = new Vector3(newLoc.x, Random.Range(minMaxBombHeight.x, minMaxBombHeight.y), newLoc.z);
            transform.LookAt(targetLocation);
        }
    }

    void FixedUpdate()
    {
        if (!startDroppingBombs)
        {
            currentStartTime = currentStartTime < 0 ? 0 : currentStartTime -= Time.fixedDeltaTime;
            if (currentStartTime.Equals(0))
                startDroppingBombs = true;
        }
        if (!transform.position.Equals(targetLocation))
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, shipSpeed * Time.fixedDeltaTime);
        if (transform.position.Equals(targetLocation) && enemyManager != null)
        {
            Vector3 newLoc = enemyManager.FindLocationWithinArea();
            targetLocation = new Vector3(newLoc.x, Random.Range(minMaxBombHeight.x, minMaxBombHeight.y), newLoc.z);
        }
        if (startDroppingBombs && objectPools != null)
        {
            currentDrop = currentDrop <= 0 ? 0 : currentDrop -= Time.fixedDeltaTime;
            if (currentDrop <= 0)
            {
                GameObject bomb = objectPools.GetRocket();
                Rocket r = bomb.GetComponent<Rocket>();
                Rigidbody rB = bomb.GetComponent<Rigidbody>();
                
                if (r != null)
                {
                    r.type = RocketType.Bomb;
                    r.rocketSpeed = bombDropSpeed;
                    r.explosionRadius = bombRadius;
                    r.rocketDamage = bombDamage;
                    currentDrop = bombDropRate;
                }
                if (bomb != null)
                {
                    bomb.transform.position = transform.position + Vector3.down * 5;
                    bomb.SetActive(true);
                    BombDropped?.Invoke();
                }
                currentDrop = bombDropRate;
            }
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position), rotateSpeed * Time.fixedDeltaTime);
    }

    public void ApplyDamage(float amount)

    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth);
            BomberExploded?.Invoke(transform.position, 10, 0);
            UpdateBomber?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Kill Bomber")]
    public void KillBomber() => ApplyDamage(maxHealth);
    public float GetCurrentHealth() => currentHealth;
}
