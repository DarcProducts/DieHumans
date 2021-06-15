using UnityEngine;
using UnityEngine.Events;

public class Rocket : MonoBehaviour, IDamagable<float>
{
    [SerializeField] private float maxRocketDistance;
    public bool isHoming;
    float currentDamage = 1;
    [SerializeField] private float homingSpeed;
    [SerializeField] private float maxHealth = 2;
    private float currentHealth = 1;
    public static UnityAction<Vector3> ExplodeRocket;
    public static UnityAction<GameObject> RocketHit;
    public static UnityAction<Vector3> WasDamaged;
    private Vector3 startLoc;
    private TrailRenderer rocketTrail;
    private GameObject player;

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("PlayerShip");
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
            if (player != null && isHoming)
                transform.LookAt(player.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, homingSpeed * Time.fixedDeltaTime);
        }
        if (currentHealth <= 0)
        {
            ExplodeRocket?.Invoke(transform.position);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collider)
    {
        RocketHit?.Invoke(collider.gameObject);
        ExplodeRocket?.Invoke(transform.position);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        RocketHit?.Invoke(other.gameObject);
        ExplodeRocket?.Invoke(transform.position);
        gameObject.SetActive(false);
    }


    public void SetCurrentDamage(float amount) => currentDamage = amount;
    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        WasDamaged?.Invoke(transform.position);
    }

    public float GetCurrentHealth() => currentHealth;

    public float GetMaxHealth() => maxHealth;
}
