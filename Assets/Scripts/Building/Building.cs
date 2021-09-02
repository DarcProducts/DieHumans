using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamagable<float>
{
    public static UnityAction UpdateBuildingCount;
    public static UnityAction BuildingCollapseSound;
    public static UnityAction<GameObject> BuildingDestroyed;
    public static UnityAction<Vector3> RemoveBuildingVector;
    public Color damageColor;
    public float damageColorDuration;
    [SerializeField] float buildingHealthMultiplier;
    float _currentBuildingHealth;
    [SerializeField] float collapseRate = .4f;
    bool hasPlayedSound = false;
    [SerializeField] ObjectPooler collapseEffectPool;
    [SerializeField] GameObject brokenBuildingEffect;
    Renderer buildingRend;
    float maxBuildingHealth;
    bool brokenEffectSet = false;
    bool collapsingEffectSet = false;
    bool isCollapsing = false;
    [SerializeField] Color originalColor;

    void OnEnable() => hasPlayedSound = false;

    void Awake()
    {
        buildingRend = GetComponent<Renderer>();
        originalColor = buildingRend.material.GetColor("_Color");
    }

    void Start()
    {
        maxBuildingHealth = transform.localScale.y * buildingHealthMultiplier;
        _currentBuildingHealth = maxBuildingHealth;
    }

    void LateUpdate()
    {
        if (isCollapsing)
            SlowlyCollapseBuilding();
    }

    IEnumerator ChangeMaterialColor()
    {
        buildingRend.material.SetColor("_Color", damageColor);
        yield return new WaitForSeconds(damageColorDuration);
        buildingRend.material.SetColor("_Color", originalColor);
    }

    void InitializeCollapsingEffect()
    {
        if (collapseEffectPool != null && !collapsingEffectSet)
        {
            GameObject effect = collapseEffectPool.GetObject();
            if (effect != null)
            {
                effect.transform.SetPositionAndRotation(transform.position, transform.rotation);
                effect.SetActive(true);
            }
            collapsingEffectSet = true;
        }
    }

    void InitializeBrokenBuildingEffect()
    {
        if (!brokenEffectSet && brokenBuildingEffect != null)
        {
            GameObject e = Instantiate(brokenBuildingEffect, transform.position, Quaternion.identity);
            if (e != null)
            {
                e.transform.SetPositionAndRotation(transform.position, transform.rotation);
                e.transform.localScale = new Vector3(transform.localScale.x, Random.Range(.4f, 3f), transform.localScale.z);
                e.SetActive(true);
                brokenEffectSet = true;
            }
        }
    }

    void SlowlyCollapseBuilding()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(transform.localScale.x, 0, transform.localScale.z), collapseRate * Time.fixedDeltaTime);
        if (!hasPlayedSound)
        {
            BuildingCollapseSound?.Invoke();
            hasPlayedSound = true;
        }
        if (transform.localScale.y <= 0)
        {
            InitializeBrokenBuildingEffect();
            UpdateBuildingCount?.Invoke();
            BuildingDestroyed?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => _currentBuildingHealth;

    public void ApplyDamage(float damage)
    {
        _currentBuildingHealth -= damage;
        StartCoroutine(nameof(ChangeMaterialColor));
        if (_currentBuildingHealth <= 0)
        {
            InitializeCollapsingEffect();
            isCollapsing = true;
        }
    }

    [ContextMenu("Destroy building")]
    public void DestroyBuilding() => ApplyDamage(maxBuildingHealth);
}