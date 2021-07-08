using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bomber : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3, float> BomberExploded;
    public static UnityAction<GameObject, byte, byte, float> TextInfo;
    public static UnityAction<GameObject> UpdateBomber;
    public static UnityAction BombDropped;
    [SerializeField] float maxHealth;
    [SerializeField] Vector2 minMaxBombHeight;
    [SerializeField] float bombDamage;
    EnemyManager enemyManager;
    bool startDroppingBombs = false;
    ObjectPools objectPools;
    float currentHealth;
    [SerializeField] float shipSpeed;
    [SerializeField] float bombDropRate;
    float currentDrop;
    Vector3 targetLocation;

    void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager == null)
            Debug.LogWarning($"Could not find GameObject with EnemyManager component on it");
        objectPools = FindObjectOfType<ObjectPools>();
        if (objectPools == null)
            Debug.LogWarning($"Could not find GameObject with ObjectPools component on it");
    }
    void OnEnable()
    {
        if (enemyManager != null)
        {
            Vector3 newLoc = enemyManager.FindLocationWithinArea();
            targetLocation = new Vector3(newLoc.x, Random.Range(minMaxBombHeight.x, minMaxBombHeight.y), newLoc.z);
        }
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (!transform.position.Equals(targetLocation))
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, shipSpeed * Time.fixedDeltaTime);
        if (transform.position.Equals(targetLocation) && enemyManager != null)
        {
            Vector3 newLoc = enemyManager.FindLocationWithinArea();
            targetLocation = new Vector3(newLoc.x, Random.Range(minMaxBombHeight.x, minMaxBombHeight.y), newLoc.z);
            startDroppingBombs = true;
        }
        if (startDroppingBombs && objectPools != null)
        {
            currentDrop = currentDrop <= 0 ? 0 : currentDrop -= Time.fixedDeltaTime;
            if (currentDrop <= 0)
            {
                GameObject bomb = objectPools.GetAvailableRocket();
                bomb.transform.position = transform.position + Vector3.down * 5;
                Rocket r = bomb.GetComponent<Rocket>();
                if (r != null)
                {
                    r.type = RocketType.bomb;
                    r.rocketDamage = bombDamage;
                    r.maxRocketDistance = 1000;
                    r.gameObject.SetActive(true);
                    BombDropped?.Invoke();
                    currentDrop = bombDropRate;
                }
            }
        }
        transform.LookAt(targetLocation);
    }

    public void ApplyDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            TextInfo?.Invoke(gameObject, 2, 1, maxHealth);
            BomberExploded?.Invoke(transform.position, 10);
            UpdateBomber?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => currentHealth;
}
