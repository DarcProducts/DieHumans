using UnityEngine;
using UnityEngine.Events;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float maxRocketDistance;
    [SerializeField] private Vector3 explosionEffectScale;
    [SerializeField] public float currentRocketDamage;
    [SerializeField] private float explosionRadius;
    private bool damageApplied = false;
    public static UnityAction<Vector3> HitATarget;
    private Vector3 startLoc;

    private void Start() => startLoc = transform.position;

    private void Update() => ActivateRocket();

    private void ActivateRocket()
    {
        if (Vector3.Distance(startLoc, transform.position) > maxRocketDistance)
            CheckIfNearTarget();
    }

    private void CheckIfNearTarget()
    {
        RaycastHit[] targetsHit = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.forward);
        if (!damageApplied && targetsHit.Length > 0)
        {
            for (int i = 0; i < targetsHit.Length; i++)
            {
                if (!targetsHit[i].collider.gameObject.CompareTag("Rocket"))
                {
                    IDamagable<float> d = targetsHit[i].collider.gameObject.GetComponent<IDamagable<float>>();
                    if (d != null)
                        d.ApplyDamage(currentRocketDamage);
                }
            }
            HitATarget?.Invoke(transform.position);
            damageApplied = true;
        }
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collider)
    {
        if (!collider.gameObject.CompareTag("TVWall") && !collider.gameObject.CompareTag("Chopper")) // gotta fix, cant have this check on eveything!!
            CheckIfNearTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("TVWall") && !other.CompareTag("Chopper")) // gotta fix, cant have this check on eveything!!
            CheckIfNearTarget();
    }

    public void SetRocketDamage(float damage) => currentRocketDamage = damage;
}