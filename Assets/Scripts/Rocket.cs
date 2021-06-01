using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float maxRocketLife;
    [SerializeField] private Vector3 explosionScale;
    private float currentRocketDamage;
    [SerializeField] private float checkRadius;
    private GameObject hitObject;
    private bool damageApplied = false;

    private void Start() => Destroy(gameObject, maxRocketLife);

    private void Update() => ActivateRocket();

    private void ActivateRocket()
    {
        hitObject = CheckIfNearTarget();
        if (hitObject != null)
        {
            IDamagable<float> d = hitObject.GetComponent<IDamagable<float>>();
            if (d != null && !damageApplied)
            {
                d.ApplyDamage(currentRocketDamage);
                Debug.Log($"Applying {currentRocketDamage} damage to {hitObject.name}");
            }
            damageApplied = true;
            Explode();
        }
    }

    private GameObject CheckIfNearTarget()
    {
        if(Physics.SphereCast(transform.position, checkRadius, transform.forward, out RaycastHit hit, 1))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (!hitObject.CompareTag("Rocket") | !hitObject.CompareTag("Chopper"))
                return hitObject;
        }
        return null;
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject e = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            e.transform.localScale = explosionScale;
            Destroy(gameObject);
        }
    }

    public void SetRocketDamage(float damage) => currentRocketDamage = damage;
}