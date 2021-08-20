using UnityEngine;
using UnityEngine.Events;

public class Bomber : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    [SerializeField] IntVariable maxHealth;
    [SerializeField] Vector2Variable minMaxBombHeight;
    [SerializeField] FloatVariable bombDamage;
    [SerializeField] FloatVariable bombRadius;
    [SerializeField] FloatVariable bombStartTime;
    [SerializeField] FloatVariable rotateSpeed;
    float currentStartTime;
    [SerializeField] CityGenerator cityGenerator;
    bool startDroppingBombs = false;
    [SerializeField] float explosionSize;
    [SerializeField] ByteVariable explosionIndex;
    float currentHealth;
    [SerializeField] FloatVariable shipSpeed;
    [SerializeField] FloatVariable bombDropRate;
    [SerializeField] FXInitializer bombDropFX;
    MultiPooler objectPooler;
    float currentDrop;
    Vector3 targetLocation;

    void Start()
    {
        objectPooler = FindObjectOfType < MultiPooler>();
        if (bombStartTime != null)
        currentStartTime = bombStartTime.value;
    }

    void OnEnable()
    {
        if (maxHealth != null)
            currentHealth = maxHealth.value;
        if (bombStartTime != null)
        currentStartTime = bombStartTime.value;
        startDroppingBombs = false;
        SetNewLocation();
        transform.LookAt(targetLocation);
    }

    void SetNewLocation()
    {
        if (cityGenerator != null && minMaxBombHeight != null)
        {
            float ranHeight = Random.Range(minMaxBombHeight.value.x, minMaxBombHeight.value.y);
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
        if (!transform.position.Equals(targetLocation) && shipSpeed != null)
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, shipSpeed.value * Time.fixedDeltaTime);
        if (transform.position.Equals(targetLocation))
            SetNewLocation();
        if (startDroppingBombs && objectPooler != null)
        {
            currentDrop = currentDrop <= 0 ? 0 : currentDrop -= Time.fixedDeltaTime;
            if (currentDrop <= 0)
            {
                GameObject bomb = objectPooler.GetObject(8);
                Rocket r = bomb.GetComponent<Rocket>();
                Rigidbody rB = bomb.GetComponent<Rigidbody>();

                if (r != null && bombRadius != null && bombDamage != null && bombDropRate != null)
                {
                    r.type = RocketType.Bomb;
                    r.explosionRadius = bombRadius.value;
                    r.rocketDamage = bombDamage.value;
                    currentDrop = bombDropRate.value;
                }
                if (bomb != null)
                {
                    bomb.transform.position = transform.position + Vector3.down * 5;
                    bomb.SetActive(true);
                    if (bombDropFX != null)
                        bombDropFX.PlayAllFX(transform.position);
                }
                if (bombDropRate != null)
                    currentDrop = bombDropRate.value;
            }
        }
        if (rotateSpeed != null)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetLocation - transform.position), rotateSpeed.value * Time.fixedDeltaTime);
    }

    public void ApplyDamage(float amount)

    {
        currentHealth -= amount;
        if (currentHealth <= 0 && maxHealth != null)
        {
            TextInfo?.Invoke(transform.position, 2, 1, maxHealth.value * 2);
            if (objectPooler != null && explosionIndex != null)
            {
                GameObject e = objectPooler.GetObject(explosionIndex.value);
                e.transform.position = transform.position;
                e.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
                e.SetActive(true);
            }
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Kill Bomber")]
    public void KillBomber()
    {
        if (maxHealth != null)
            ApplyDamage(maxHealth.value);
    }

    public float GetCurrentHealth() => currentHealth;
}