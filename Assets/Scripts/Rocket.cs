using UnityEngine;
using UnityEngine.Events;

public class Rocket : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, float> ExplodeRocket;
    public static UnityAction<Vector3> WasDamaged;
    public bool isHoming;
    public Vector3 currentTarget;
    public float rocketDamage;
    public float maxRocketDistance;
    public float homingSpeed;
    public float explosionRadius;
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

    private void Update() => ActivateRocket();

    private void ActivateRocket()
    {
        if (Vector3.Distance(startLoc, transform.position) > maxRocketDistance)
        {
            ExplodeRocket?.Invoke(transform.position, explosionRadius);
            TryDamagingNearTargets();
            gameObject.SetActive(false);
        }
        else
        {
            if (isHoming)
            {
                transform.LookAt(currentTarget);
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, homingSpeed * Time.fixedDeltaTime);
            }
        }
        if (currentHealth <= 0)
        {
            ExplodeRocket?.Invoke(transform.position, explosionRadius);
            TryDamagingNearTargets();
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        ExplodeRocket?.Invoke(transform.position, explosionRadius);
        TryDamagingNearTargets();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        ExplodeRocket?.Invoke(transform.position, explosionRadius);
        TryDamagingNearTargets();
        gameObject.SetActive(false);
    }

    private void TryDamagingNearTargets()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, explosionRadius);
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
    }

    public void SetCurrentTarget(Vector3 value) => currentTarget = value;
   
    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}
