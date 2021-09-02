using UnityEngine;
using UnityEngine.Events;

public class Meteor : MonoBehaviour, IDamagable<float>
{
    public static UnityAction MeteorExplodedSound;
    public static UnityAction MeteorEvadedSound;
    public float explosionRadius;
    public LayerMask hitLayers;
    public Vector2 meteorMinMaxSize;
    public float healthMultiplier;
    public byte pointMultiplier;
    public ObjectPooler explosionPool;
    public ObjectPooler meteorbrokenPool;
    float maxHealth;
    float newScale;
    float currentHealth;
    float currentSpeed;

    void OnEnable()
    {
        newScale = Random.Range(meteorMinMaxSize.x, meteorMinMaxSize.y);
        transform.localScale = new Vector3(newScale, newScale, newScale);
        currentHealth = newScale * healthMultiplier;
        maxHealth = currentHealth;
    }

    void FixedUpdate() => transform.Translate(currentSpeed * Time.fixedDeltaTime * Vector3.down, Space.World);

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            MeteorEvadedSound?.Invoke();
            GameObject e = meteorbrokenPool.GetObject();
            e.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            e.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Projectile"))
        {
            if (explosionPool != null)
            {
                GameObject e = explosionPool.GetObject();
                e.transform.position = transform.position;
                float newSize = newScale;
                e.transform.localScale = new Vector3(newSize, newSize, newSize);
                e.SetActive(true);
            }
            TryDamagingNearTargets();
            MeteorExplodedSound?.Invoke();
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
                d.ApplyDamage(Mathf.RoundToInt(currentHealth / closeObjects.Length));
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    public void SetMeteorSpeed(float value) => currentSpeed = value;
}