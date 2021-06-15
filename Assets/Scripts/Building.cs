using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3> BuildingDamaged;
    public static UnityAction<GameObject> BreakBuilding;
    [SerializeField] private float buildingHealthMultiplier;
    [SerializeField] private int buildingPeopleMultiplier;
    [SerializeField] private float _currentBuildingHealth;
    [SerializeField] private int _currentNumberOfPeopleInside;
    [SerializeField] private float shakeIntensity = .01f;
    [SerializeField] private float duration = .1f;
    private BuildingManager buildingManager;
    private float maxBuildingHealth;
    private int maxNumberOfPeopleInside;
    private bool brokenEffectSet = false;
    private bool collapsingEffectSet = false;

    public void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        maxBuildingHealth = transform.localScale.y * buildingHealthMultiplier;
        maxNumberOfPeopleInside = (int)transform.localScale.y * buildingPeopleMultiplier;
        _currentBuildingHealth = maxBuildingHealth;
        _currentNumberOfPeopleInside = maxNumberOfPeopleInside;
    }

    public void LateUpdate()
    {
        if (_currentBuildingHealth <= 0)
        {
            InitializeBrokenBuildingEffect();
            InitializeCollapsingEffect();
        }
    }

    private void InitializeCollapsingEffect()
    {
        if (!collapsingEffectSet && buildingManager != null)
        {
            GameObject effect = buildingManager.GetCollapseEffect();
            if (effect != null)
            {
                effect.transform.SetPositionAndRotation(transform.position, transform.rotation);
                effect.SetActive(true);
            }
            BreakBuilding?.Invoke(gameObject);
            collapsingEffectSet = true;
            gameObject.SetActive(false);
        }
    }

    public void InitializeBrokenBuildingEffect()
    {
        if (!brokenEffectSet && buildingManager != null)
        {
            GameObject e = buildingManager.GetBrokenEffect();
            if (e != null)
            {
                e.transform.SetPositionAndRotation(transform.position, transform.rotation);
                e.SetActive(true);
                brokenEffectSet = true;
            }
        }
    }

    private IEnumerator ShakeBuilding()
    {
        if (duration > 0)
        {
            var randPos = Random.insideUnitSphere * shakeIntensity;
            transform.Translate(new Vector3(randPos.x, transform.position.y, randPos.z));
            yield return new WaitForSeconds(duration);
        }
        StopCoroutine(ShakeBuilding());
    }

    public float GetMaxHealth() => maxBuildingHealth;

    public float GetCurrentHealth() => _currentBuildingHealth;

    public int GetNumberOfPeopleInside() => _currentNumberOfPeopleInside;

    public void AbductPeople(int amount) => _currentNumberOfPeopleInside -= amount;

    public void ApplyDamage(float damage)
    {
        _currentBuildingHealth -= damage;
        StartCoroutine(ShakeBuilding());
    }

    
    [ContextMenu("Damage building")]
    public void TestDamage() => ApplyDamage(10000);
  
}