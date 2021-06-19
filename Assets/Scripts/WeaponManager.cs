using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private float weaponDamage;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private float aimTargetDistance;
    [SerializeField] private float rocketThrust;
    

    private void Start()
    {
        if (enemyManager == null)
            enemyManager = FindObjectOfType<EnemyManager>();
        InitializeAimTarget();
    }

    public void LaunchRocket(Vector3 firedFrom, bool isHoming)
    {
        if (player != null && enemyManager != null)
        {
            Vector3 direction = player.transform.position + Vector3.up - firedFrom + Vector3.down;
            GameObject r = enemyManager.GetAvailableRocket();
            r.transform.position = firedFrom + Vector3.down * .5f;
            Rocket rocket = r.GetComponent<Rocket>();
            r.SetActive(true);
            if (r != null)
            {
                if (!isHoming)
                {
                    rocket.isHoming = false;
                    r.GetComponent<Rigidbody>().AddForce(direction.normalized * rocketThrust, ForceMode.Impulse);
                }
                else
                    rocket.isHoming = true;
            }
        }
    }

    public void TryDamagingTarget(GameObject target)
    {
        IDamagable<float> d = target.GetComponent<IDamagable<float>>();
        if (d != null)
            d.ApplyDamage(weaponDamage);
    }

     public void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);
    }

    public float GetWeaponDamage() => weaponDamage;
}