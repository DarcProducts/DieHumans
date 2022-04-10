using UnityEngine;

public class ExplodeNearObjects : MonoBehaviour
{
    [SerializeField] float explosionForce;
    [SerializeField, Range(0f, 1f)] float upwardsModifier = .1f;
    [SerializeField] float explosionRadius;
    [SerializeField] LayerMask explodeLayers;
    Collider[] objectsToExplode;
    Vector3 pos;
    
    public void Explode(GameObject obj)
    {
        pos = obj.transform.position;
        objectsToExplode = Physics.OverlapSphere(pos, explosionForce, explodeLayers);
        foreach (Collider c in objectsToExplode)
        {
            if (c.TryGetComponent(out Rigidbody rigid))
                rigid.AddExplosionForce(explosionForce, pos, explosionRadius, upwardsModifier, ForceMode.Impulse);
        }
    }
}
 