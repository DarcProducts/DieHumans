using UnityEngine;
using UnityEngine.Events;

public class Bomber : MonoBehaviour, IDamagable<float>
{
    public static UnityAction BombDropSound;
    public static UnityAction BomberExploded;
    public static UnityAction UpdateBomberKillCount;
    public static UnityAction<GameObject> RemoveFromWaveList;
    public KillSheet killSheet;
    public int maxHealth;
    public Vector2 minMaxBombHeight;
    public float bombDamage;
    public float bombRadius;
    public float bombStartTime;
    public float rotateSpeed;
    float currentStartTime;
    public CityGenerator cityGenerator;
    bool startDroppingBombs = false;
    public float explosionSize;
    float currentHealth;
    public float shipSpeed;
    public float bombDropRate;
    public ObjectPooler explosionPool;
    public ObjectPooler rocketPool;
    float currentDrop;
    Vector3 targetLocation;

    void Start() => currentStartTime = bombStartTime;

    void OnEnable()
    {
        currentHealth = maxHealth;
        currentStartTime = bombStartTime;
        startDroppingBombs = false;
        SetNewLocation();
        transform.LookAt(targetLocation);
    }

    void SetNewLocation()
    {
        if (cityGenerator != null && minMaxBombHeight != null)
        {
            float ranHeight = Random.Range(minMaxBombHeight.x, minMaxBombHeight.y);
            Vector3 newLoc = cityGenerator.GetAttackVector();
            targetLocation = newLoc + Vector3.up * ranHeight;
        }
    }

    void FixedUpdate()
    {
        if (!startDroppingBombs)
        {
            currentStartTime = currentStartTime < 0 ? 0 : currentStartTime -= Time.fixedDeltaTime;
            if (currentStartTime.Equals(0))
                startDroppingBombs = true;
        }
        if (!transform.position.Equals(targetLocation))
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, shipSpeed * Time.fixedDeltaTime);
        if (transform.position.Equals(targetLocation))
            SetNewLocation();
        if (startDroppingBombs)
        {
            currentDrop = currentDrop <= 0 ? 0 : currentDrop -= Time.fixedDeltaTime;
            if (currentDrop <= 0)
            {
                GameObject bomb = rocketPool.GetObject();
                Rocket r = bomb.GetComponent<Rocket>();

                if (r != null)
                {
                    r.type = RocketType.Bomb;
                    r.explosionRadius = bombRadius;
                    r.rocketDamage = bombDamage;
                    currentDrop = bombDropRate;
                }
                if (bomb != null)
                {
                    bomb.transform.position = transform.position + Vector3.down * 5;
                    bomb.SetActive(true);
                    BombDropSound?.Invoke();
                }
                currentDrop = bombDropRate;
            }
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position), rotateSpeed * Time.fixedDeltaTime);
    }

    public void ApplyDamage(float amount)

    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            GameObject e = explosionPool.GetObject();
            e.transform.position = transform.position;
            e.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
            e.SetActive(true);
            killSheet.bombersDestroyed++;
            UpdateBomberKillCount?.Invoke();
            RemoveFromWaveList?.Invoke(this.gameObject);
            BomberExploded?.Invoke();
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Kill Bomber")]
    public void KillBomber() => ApplyDamage(maxHealth);

    public float GetCurrentHealth() => currentHealth;
}