using UnityEngine;
using UnityEngine.Events;

public class ExplosiveBarrels : MonoBehaviour
{
    [SerializeField] GameEvent barrelExploded;
    [SerializeField] GameEvent barrelWasShot;
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
            barrelExploded.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    void Explode(LayerMask layer, bool wasShot)
    {
        Utilities.TryDamagingNearTargets(transform.position, explRad, layer, explDmg);
        if (wasShot)
            barrelWasShot.Invoke(gameObject);
        barrelExploded.Invoke(gameObject);
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