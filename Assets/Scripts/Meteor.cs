using UnityEngine;
using UnityEngine.Events;

public class Meteor : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, float> MeteorExploded;
    public static UnityAction<GameObject> MeteorEvaded;
    public static UnityAction<GameObject, byte> TextInfo;
    [SerializeField] Vector2 meteorMinMaxSize;
    public float maxHealth;
    public float explosionRadius;
    float currentHealth;
    float currentSpeed;

    void OnEnable()
    {
        currentHealth = maxHealth;
        float newScale = Random.Range(meteorMinMaxSize.x, meteorMinMaxSize.y);
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    void FixedUpdate() => transform.Translate(currentSpeed * Time.fixedDeltaTime * Vector3.down, Space.World);

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(gameObject, 2);
            MeteorEvaded?.Invoke(gameObject);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    void OnCollisionEnter(Collision collision)
    {
        TextInfo?.Invoke(gameObject, 3);
        MeteorExploded?.Invoke(transform.position, explosionRadius * .5f);
        TryDamagingNearTargets();
        gameObject.SetActive(false);
    }

    void TryDamagingNearTargets()
    {
        float damage = transform.localScale.x + transform.localScale.y + transform.localScale.z * currentHealth;
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in closeObjects)
        {
            IDamagable<float> d = hit.gameObject.GetComponent<IDamagable<float>>();
            if (d != null && !hit.gameObject.Equals(gameObject))
                d.ApplyDamage(damage);
        }
    }

    public void SetMeteorSpeed(float value) => currentSpeed = value;
}