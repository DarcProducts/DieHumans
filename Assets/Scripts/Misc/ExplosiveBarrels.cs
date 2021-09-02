using UnityEngine;
using UnityEngine.Events;

public class ExplosiveBarrels : MonoBehaviour
{
    public static UnityAction BarrelShotSound;
    public static UnityAction BarrelHitSound;
    public ObjectPooler explosionPool;
    [SerializeField] float explosionEffectSize;
    [SerializeField] GameObject parachute;
    [SerializeField] float explRad;
    [SerializeField] float explDmg;
    [SerializeField] LayerMask explHitLyr;
    [SerializeField] LayerMask enemyHitLyr;
    [SerializeField] LayerMask projectileLayer;
    Rigidbody rB;
    bool dragSet = false;
    bool hasPara = true;

    void Awake() => rB = GetComponent<Rigidbody>();

    void Start() => ResetParachute();

    void OnEnable() => ResetParachute();

    void OnDisable()
    {
        hasPara = true;
        if (parachute != null)
            parachute.SetActive(true);
    }

    void FixedUpdate()
    {
        if (!hasPara && !dragSet)
            ShotParachute();

        if (transform.position.y < -5)
            gameObject.SetActive(false);

        transform.rotation = Quaternion.identity;
    }

    void ShotParachute()
    {
        if (rB != null)
        {
            rB.drag = .1f;
            rB.angularDrag = .05f;
            dragSet = true;
        }
    }

    void ResetParachute()
    {
        if (rB != null)
        {
            rB.drag = 2f;
            rB.angularDrag = .5f;
            dragSet = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Utilities.IsInLayerMask(collision.gameObject, explHitLyr))
            Explode(explHitLyr, false);
        else if (Utilities.IsInLayerMask(collision.gameObject, projectileLayer))
            Explode(enemyHitLyr, true);
        else
        {
            BarrelHitSound?.Invoke();
            gameObject.SetActive(false);
        }
    }

    void Explode(LayerMask layer, bool wasShot)
    {
        Utilities.TryDamagingNearTargets(transform.position, explRad, layer, explDmg);
        if (wasShot)
            BarrelShotSound?.Invoke();
        else
            BarrelHitSound?.Invoke();
        GameObject e = explosionPool.GetObject();
        e.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        e.transform.localScale = Vector3.one * explosionEffectSize;
        e.SetActive(true);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, projectileLayer))
            DestroyParachute();
    }

    [ContextMenu("Destroy Parachute")]
    public void DestroyParachute()
    {
        if (parachute != null)
        {
            hasPara = false;
            parachute.SetActive(false);
        }
    }
}