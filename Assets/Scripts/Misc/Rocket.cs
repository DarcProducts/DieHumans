using UnityEngine;
using UnityEngine.Events;

public enum RocketType { Standard, Homing, Bomb, Mortar }

public class Rocket : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<GameObject> HitObject;
    public RocketType type = RocketType.Standard;
    public Vector3 currentTarget;
    public float rocketDamage;
    public float rocketSpeed;
    public float explosionRadius;
    public LayerMask damagableLayers;
    public LayerMask ignoreDropCollisionLayers;
    [SerializeField] float maxHealth = 2;
    [SerializeField] ObjectPooler explosionPool;
    [SerializeField] float explosionSize;
    float currentHealth = 1;
    bool forceApplied = false;
    TrailRenderer rocketTrail;
    Rigidbody rB;

    void Awake()
    {
        rB = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        rocketTrail = GetComponentInChildren<TrailRenderer>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        if (type.Equals(RocketType.Mortar))
            transform.forward = Vector3.up;
    }

    void OnDisable()
    {
        forceApplied = false;
        if (rocketTrail != null)
            rocketTrail.Clear();
        if (rB != null)
        {
            rB.velocity = Vector3.zero;
            rB.angularVelocity = Vector3.zero;
        }
    }

    void FixedUpdate() => ActivateRocket();

    void ActivateRocket()
    {
        switch (type)
        {
            case RocketType.Standard:
                if (rB != null && !forceApplied)
                {
                    rB.AddForce(rocketSpeed * Time.fixedDeltaTime * (currentTarget - transform.position).normalized, ForceMode.Impulse);
                    forceApplied = true;
                }
                break;

            case RocketType.Homing:
                transform.LookAt(currentTarget);
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.normalized, rocketSpeed * Time.fixedDeltaTime);
                break;

            case RocketType.Bomb:
                transform.forward = Vector3.down;
                if (rB != null)
                {
                    rB.drag = 1.2f;
                    rB.angularDrag = .3f;
                    rB.useGravity = true;
                }
                break;
            
            case RocketType.Mortar:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(currentTarget.x, 0, currentTarget.z) - transform.position), 64 * Time.fixedDeltaTime);
                if (rB != null && !forceApplied)
                {
                    rB.drag = .2f;
                    rB.angularDrag = .01f;
                    rB.useGravity = true;
                    rB.AddForce(rocketSpeed * Time.fixedDeltaTime * (currentTarget - transform.position).normalized, ForceMode.Impulse);
                    forceApplied = true;
                }
                break;
            
            default:
                break;
        }
        if (transform.position.Equals(currentTarget))
            DisableRocket();
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, ignoreDropCollisionLayers))
            return;
        else if (!other.CompareTag("Projectile"))
        {
            Utilities.TryDamagingNearTargets(transform.position, explosionRadius, damagableLayers, rocketDamage);
            DisableRocket();
        }
    }

    void DisableRocket()
    {
        if (explosionPool != null)
        {
            GameObject e = explosionPool.GetObject();
            e.transform.position = transform.position;
            e.transform.localScale = new Vector3(explosionSize, explosionSize,explosionSize);
            e.SetActive(true);
        }
        gameObject.SetActive(false);
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth * 2);
            DisableRocket();
        }
    }

    public void SetCurrentTarget(Vector3 value) => currentTarget = value;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}