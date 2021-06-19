using UnityEngine;
using UnityEngine.Events;

public class Rocket : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3> ExplodeRocket;
    public static UnityAction<Vector3> WasDamaged;
    public bool isHoming;
    [SerializeField] private float rocketDamage;
    [SerializeField] private float maxRocketDistance;
    [SerializeField] private float homingSpeed;
    [SerializeField] private float maxHealth = 2;
    private Vector3 currentTarget;
    private float currentHealth = 1;
    private Vector3 startLoc;
    private TrailRenderer rocketTrail;
    private GameObject player;

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
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
            ExplodeRocket?.Invoke(transform.position);
            gameObject.SetActive(false);
        }
        else
        {
            if (isHoming)
                transform.LookAt(currentTarget);
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, homingSpeed * Time.fixedDeltaTime);
        }
        if (currentHealth <= 0)
        {
            ExplodeRocket?.Invoke(transform.position);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        ExplodeRocket?.Invoke(transform.position);
        TryDamagingTargetRocket(collider.gameObject);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        ExplodeRocket?.Invoke(transform.position);
        TryDamagingTargetRocket(other.gameObject);
        gameObject.SetActive(false);
    }

    private void TryDamagingTargetRocket(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(rocketDamage);
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        WasDamaged?.Invoke(transform.position);
    }

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}
