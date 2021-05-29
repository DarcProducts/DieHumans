using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float maxRocketLife;

    private void Start() => Destroy(gameObject, maxRocketLife);

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Rocket"))
        {
            if (explosionEffect != null)
            {
                GameObject e = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(e, maxRocketLife * .8f);
                Debug.Log($"Hit: {collision.gameObject.name}");
                gameObject.SetActive(false);
            }
        }
    }
}