using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamagable<float>
{
    public static UnityAction UpdateBuildingCount;
    [SerializeField] GameEvent buildingDestroyed;
    public Color damageColor;
    public float damageColorDuration;
    [SerializeField] float buildingHealthMultiplier;
    float _currentBuildingHealth;
    [SerializeField] float collapseRate = .4f;
    Renderer buildingRend;
    float maxBuildingHealth;
    bool isCollapsing = false;
    [SerializeField] Color originalColor;

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


    void SlowlyCollapseBuilding()
    {
        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(transform.localScale.x, 0, transform.localScale.z), collapseRate * Time.fixedDeltaTime);
        if (transform.localScale.y <= 0)
        {
            buildingDestroyed.Invoke(gameObject);
            UpdateBuildingCount?.Invoke();
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => _currentBuildingHealth;

    public void ApplyDamage(float damage)
    {
        _currentBuildingHealth -= damage;
        StartCoroutine(nameof(ChangeMaterialColor));
        if (_currentBuildingHealth <= 0)
            isCollapsing = true;
    }

    [ContextMenu("Destroy building")]
    public void DestroyBuilding() => ApplyDamage(maxBuildingHealth);
}