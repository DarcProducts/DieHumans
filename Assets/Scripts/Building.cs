using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamagable<float>
{
    [SerializeField] private float buildingHealthMultiplier;
    [SerializeField] private int buildingPeopleMultiplier;
    [SerializeField] private float _currentBuildingHealth;
    [SerializeField] private int _currentNumberOfPeopleInside;
    private float maxBuildingHealth;
    private int maxNumberOfPeopleInside;
    private bool isCollapsing = false;
    private bool brokenEffectSet = false;
    private bool collapsingEffectSet = false;
    [SerializeField] private float shakeIntensity = .01f;
    [SerializeField] private float duration = .1f;
    public static UnityAction<Vector3> BuildingDamaged;

    public void Start()
    {
        maxBuildingHealth = transform.localScale.y * buildingHealthMultiplier;
        maxNumberOfPeopleInside = (int)transform.localScale.y * buildingPeopleMultiplier;
        _currentBuildingHealth = maxBuildingHealth;
        _currentNumberOfPeopleInside = maxNumberOfPeopleInside;
    }

    public void LateUpdate()
    {
        if (isCollapsing)
            StartCollapsingEffect();
        if (_currentBuildingHealth <= 0)
        {
            StartBrokenBuildingEffect();
            isCollapsing = true;
        }
    }

    public void StartCollapsingEffect()
    {
        
    }

    public void StartBrokenBuildingEffect()
    {
        
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