using UnityEngine;
using UnityEngine.Events;

public class Meteor : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, float> MeteorExploded;
    public static UnityAction<GameObject> MeteorEvaded;
    public static UnityAction<GameObject, byte, byte, float> TextInfo;
    public float explosionRadius;
    public LayerMask hitLayers;
    [SerializeField] Vector2 meteorMinMaxSize;
    [SerializeField] private float healthMultiplier;
    [SerializeField] private byte pointMultiplier;
    float newScale;
    float currentHealth;
    float currentSpeed;

    void OnEnable()
    {
        newScale = Random.Range(meteorMinMaxSize.x, meteorMinMaxSize.y);
        transform.localScale = new Vector3(newScale, newScale, newScale);
        currentHealth = newScale * healthMultiplier;
    }

    void FixedUpdate() => transform.Translate(currentSpeed * Time.fixedDeltaTime * Vector3.down, Space.World);

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(gameObject, 2, 0, 0);
            MeteorEvaded?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    void OnCollisionEnter(Collision collision)
    {
        if (!IsInLayerMask(collision.gameObject, hitLayers))
        {
            MeteorExploded?.Invoke(transform.position, newScale * 2);
            gameObject.SetActive(false);
            return;
        }
        else if (!collision.gameObject.CompareTag("Projectile"))
        {
            TextInfo?.Invoke(gameObject, 3, 1, currentHealth * pointMultiplier);
            MeteorExploded?.Invoke(transform.position, newScale * 2);
            TryDamagingNearTargets();
            gameObject.SetActive(false);
        }
    }

    void TryDamagingNearTargets()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, explosionRadius, hitLayers);
        foreach (Collider hit in closeObjects)
        {
            IDamagable<float> d = hit.gameObject.GetComponent<IDamagable<float>>();
            if (d != null)
                d.ApplyDamage(currentHealth);
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    public void SetMeteorSpeed(float value) => currentSpeed = value;
}