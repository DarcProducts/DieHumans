using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, float, byte> ProjectileExploded;
    public float currentDamage;
    [SerializeField] LayerMask hitLayers;
    [SerializeField] LayerMask ignoreLayers;
    Rigidbody pRigidbody;

    void Start()
    {
        pRigidbody = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        if (pRigidbody != null)
        {
            pRigidbody.velocity = Vector3.zero;
            pRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsInLayerMask(collision.gameObject, ignoreLayers))
            return;
        if (IsInLayerMask(collision.gameObject, hitLayers))
        {
            TryDamagingTarget(collision.gameObject);
            ProjectileExploded?.Invoke(transform.position, .3f, 1);
        }
        gameObject.SetActive(false);
    }

    public void TryDamagingTarget(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
        {
            TextInfo?.Invoke(transform.position, 5, 0, currentDamage);
            d.ApplyDamage(currentDamage);
        }
    }

    public Rigidbody GetRigidbody() => pRigidbody;
    public bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;
}
