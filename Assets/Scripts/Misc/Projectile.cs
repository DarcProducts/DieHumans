using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameEvent projectileImpacted;
    public float currentDamage;
    [SerializeField] LayerMask hitLayers;
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] float explosionSize;
    TrailRenderer projectileTrail;
    Rigidbody pRigidbody;

    void Awake()
    {
        projectileTrail = GetComponentInChildren<TrailRenderer>();
        pRigidbody = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        if (projectileTrail != null)
            projectileTrail.Clear();
        if (pRigidbody != null)
        {
            pRigidbody.velocity = Vector3.zero;
            pRigidbody.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, ignoreLayers))
            return;
        if (Utilities.IsInLayerMask(other.gameObject, hitLayers))
        {
            projectileImpacted.Invoke(gameObject);
            TryDamagingTarget(other.gameObject);
        }
        gameObject.SetActive(false);
    }

    public void TryDamagingTarget(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(currentDamage);
    }
    public Rigidbody GetRigidbody() => pRigidbody;
}
