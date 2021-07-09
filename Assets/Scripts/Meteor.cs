using UnityEngine;
using UnityEngine.Events;

public class Meteor : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, float, byte> MeteorExploded;
    public static UnityAction<GameObject> MeteorEvaded;
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, string, float, Color> DamageInfo;
    public float explosionRadius;
    public LayerMask hitLayers;
    [SerializeField] Vector2 meteorMinMaxSize;
    [SerializeField] float healthMultiplier;
    [SerializeField] byte pointMultiplier;
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
            TextInfo?.Invoke(transform.position, 2, 0, 0);
            MeteorEvaded?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => currentHealth;

    void OnCollisionEnter(Collision collision)
    {
        if (!IsInLayerMask(collision.gameObject, hitLayers))
        {
            MeteorExploded?.Invoke(transform.position, newScale * 2, 0);
            gameObject.SetActive(false);
            return;
        }
        else
        {
            if (!collision.gameObject.CompareTag("Projectile"))
            {
                if (!collision.gameObject.CompareTag("Building"))
                    currentHealth %= 2;
                MeteorExploded?.Invoke(transform.position, newScale * 2, 0);
                TryDamagingNearTargets();
                gameObject.SetActive(false);
            }
        }
    }

    void TryDamagingNearTargets()
    {
        Collider[] closeObjects = Physics.OverlapSphere(transform.position, explosionRadius, hitLayers);
        foreach (Collider hit in closeObjects)
        {
            IDamagable<float> d = hit.gameObject.GetComponent<IDamagable<float>>();
            if (d != null)
                d.ApplyDamage(Mathf.RoundToInt(currentHealth));
        }
    }

    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;

    public void SetMeteorSpeed(float value) => currentSpeed = value;
}