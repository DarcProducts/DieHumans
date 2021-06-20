using UnityEngine;

public enum WeaponState { machinegun, rocket, special }
public class WeaponManager : MonoBehaviour
{
    public WeaponState currentWeapon = WeaponState.machinegun;
    [SerializeField] private float machinegunDamage;
    [SerializeField] private float rocketDamage;
    [SerializeField] private float specialDamage;
    [SerializeField] private GameObject aimTarget;
    [SerializeField] private float aimTargetDistance;
    [SerializeField] private float rocketThrust;
    [SerializeField] private float rocketHomingSpeed;
    private EnemyManager enemyManager;
    private GameObject player;

    private void Start()
    { 
        enemyManager = FindObjectOfType<EnemyManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        if (enemyManager == null)
            Debug.LogError("An object with a EnemyManager component could not be found in scene");
        if (player == null)
            Debug.LogWarning("No GameObject with Player tag could be found in scene");
        InitializeAimTarget();
    }

    public void LaunchRocketPlayer(Vector3 firedFrom, bool isHoming)
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
                {
                    rocket.isHoming = true;
                    rocket.homingSpeed = rocketHomingSpeed;
                    rocket.currentTarget = aimTarget.transform.position;
                }
            }
        }
    }

    public void InitializeAimTarget()
    {
        if (aimTarget != null)
            aimTarget.transform.localPosition = new Vector3(0, 0, aimTargetDistance);
    }

    public float GetCurrentWeaponDamage()
    {
        float damage = 0;
        switch (currentWeapon)
        {
            case WeaponState.machinegun: damage = machinegunDamage;
                break;
            case WeaponState.rocket: damage = rocketDamage;
                break;
            case WeaponState.special: damage = specialDamage;
                break;
        }
        return damage;
    }
}