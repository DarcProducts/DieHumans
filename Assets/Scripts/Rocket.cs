using UnityEngine;
using UnityEngine.Events;


public enum RocketType { standard, homing, bomb }
public class Rocket : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<GameObject, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, float> ExplodeRocket;
    public static UnityAction<Vector3> WasDamaged;
    public RocketType type = RocketType.standard;
    public Vector3 currentTarget;
    public float rocketDamage;
    public float maxRocketDistance;
    public float rocketSpeed;
    public float explosionRadius;
    public LayerMask damagableLayers;
    [SerializeField] private float maxHealth = 2;
    private float currentHealth = 1;
    private Vector3 startLoc;
    private TrailRenderer rocketTrail;

    private void Awake()
    {
        currentHealth = maxHealth;
        rocketTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
        startLoc = transform.position;
    }

    private void OnDisable()
    {
        if (rocketTrail != null)
            rocketTrail.Clear();
    }

    private void FixedUpdate() => ActivateRocket();

    private void ActivateRocket()
    {
        if (Vector3.Distance(startLoc, transform.position) > maxRocketDistance)
        {
            ExplodeRocket?.Invoke(transform.position, explosionRadius);
            TryDamagingNearTargets();
            gameObject.SetActive(false);
        }
        switch (type)
        {
            case RocketType.standard:
                transform.LookAt(transform.position + transform.forward * 2);
                transform.position = Vector3.MoveTowards(transform.position, (transform.position + transform.forward * 10).normalized, rocketSpeed * Time.fixedDeltaTime);
                break;
            case RocketType.homing:
                transform.LookAt(currentTarget);
                transform.position = Vector3.MoveTowards(transform.position, currentTarget.normalized, rocketSpeed * Time.fixedDeltaTime);
                break;
            case RocketType.bomb:
                transform.forward = Vector3.down;
                Rigidbody r = GetComponent<Rigidbody>();
                if (r != null)
                    r.useGravity = true;
                break;
            default:
                break;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bomber"))
            return;
        if (other.CompareTag("Building"))
            TextInfo?.Invoke(gameObject, 3, 1, rocketDamage);

        ExplodeRocket?.Invoke(transform.position, explosionRadius);
        TryDamagingNearTargets();
        gameObject.SetActive(false);
    }

    private void TryDamagingNearTargets()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, explosionRadius, damagableLayers);
        foreach (Collider hit in closeObjects)
        {

            IDamagable<float> d = hit.gameObject.GetComponent<IDamagable<float>>();
            if (d != null)
                d.ApplyDamage(rocketDamage);
        }
    }

    public void ApplyDamage(float amount)
    { 
        currentHealth -= amount;
        WasDamaged?.Invoke(transform.position);
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(gameObject, 2, 1, 10);
            ExplodeRocket?.Invoke(transform.position, explosionRadius);
            gameObject.SetActive(false);
        }
    }

    public void SetCurrentTarget(Vector3 value) => currentTarget = value;
   
    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}
