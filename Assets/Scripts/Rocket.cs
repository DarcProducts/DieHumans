using UnityEngine;
using UnityEngine.Events;

public enum RocketType { Standard, Homing, Bomb, Mortar }

public class Rocket : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, float, byte> ExplodeRocket;
    public static UnityAction<GameObject> HitObject;
    public RocketType type = RocketType.Standard;
    public Vector3 currentTarget;
    public float rocketDamage;
    public float rocketSpeed;
    public float explosionRadius;
    public LayerMask damagableLayers;
    public LayerMask ignoreLayers;
    [SerializeField] float maxHealth = 2;
    float currentHealth = 1;
    bool forceApplied = false;
    TrailRenderer rocketTrail;
    Rigidbody rB;

    private void Awake()
    {
        rB = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        rocketTrail = GetComponentInChildren<TrailRenderer>();
        if (rB == null)
            Debug.LogWarning($"No Rigidbody located on {gameObject.name}");
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void OnDisable()
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

    private void FixedUpdate() => ActivateRocket();

    private void ActivateRocket()
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
                transform.forward = Vector3.up;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(currentTarget - transform.position), 32 * Time.fixedDeltaTime);
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
        {
            ExplodeRocket?.Invoke(transform.position, explosionRadius * .5f, 1);
            gameObject.SetActive(false);
        }
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, ignoreLayers))
            return;
        TryDamagingNearTargets();
        ExplodeRocket?.Invoke(transform.position, explosionRadius * .5f, 0);
        gameObject.SetActive(false);
    }

    private void TryDamagingNearTargets()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, explosionRadius, damagableLayers);
        foreach (Collider hit in closeObjects)
        {
            if (IsInLayerMask(hit.gameObject, damagableLayers))
            {
                IDamagable<float> d = hit.gameObject.GetComponent<IDamagable<float>>();
                if (d != null)
                    d.ApplyDamage(rocketDamage);
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth);
            ExplodeRocket?.Invoke(transform.position, explosionRadius * .5f, 1);
            gameObject.SetActive(false);
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    public void SetCurrentTarget(Vector3 value) => currentTarget = value;

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}