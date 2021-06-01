using UnityEngine;

public class Building : MonoBehaviour, IDamagable<float>
{
    private float maxBuildingHealth;
    private int maxNumberOfPeopleInside;
    [SerializeField] private float _currentBuildingHealth;
    [SerializeField] private int _currentNumberOfPeopleInside;
    [SerializeField] private bool isCollapsing = false;
    private BuildingManager buildingManager;
    [SerializeField] private bool brokenEffectSet = false;
    [SerializeField] private bool collapsingEffectSet = false;
    private GameObject currentBrokenEffect;

    public void Start()
    {
        if (buildingManager == null)
            buildingManager = FindObjectOfType<BuildingManager>();
        if (buildingManager != null)
        {
            maxBuildingHealth = transform.localScale.y * buildingManager.GetBuildingHealthMultiplier();
            maxNumberOfPeopleInside = (int)transform.localScale.y * buildingManager.GetBuildingPeopleMultiplier();
        }
        _currentBuildingHealth = maxBuildingHealth;
        _currentNumberOfPeopleInside = maxNumberOfPeopleInside;
    }

    public void LateUpdate()
    {
        if (isCollapsing)
        {
            StartCollapsingEffect();
            CollapseBuilding();
        }
        if (_currentBuildingHealth <= 0)
        {
            StartBrokenBuildingEffect();
            isCollapsing = true;
        }
    }

    public void CollapseBuilding()
    {
        if (isCollapsing)
        {
            if (buildingManager != null)
                buildingManager.InitializeBrokenBuilding(this.gameObject, currentBrokenEffect);
        }
    }

    public void StartCollapsingEffect()
    {
        if (buildingManager != null && !collapsingEffectSet)
        {
            buildingManager.PlayBuildingCollapseEffect(this.gameObject);
            collapsingEffectSet = true;
        }
    }

    public void StartBrokenBuildingEffect()
    {
        if (buildingManager != null && !brokenEffectSet)
        {
            currentBrokenEffect = buildingManager.PlayBuildingBrokenEffect(this.gameObject);
            brokenEffectSet = true;
        }
    }


    public bool GetHasBrokenEffect() => brokenEffectSet;
    public bool GetHasCollapseEffect() => collapsingEffectSet;
    public bool GetIsCollapsing() => isCollapsing;

    public void SetIsCollapsing() => isCollapsing = true;

    public float GetMaxHealth() => maxBuildingHealth;

    public float GetCurrentHealth() => _currentBuildingHealth;

    public int GetNumberOfPeopleInside() => _currentNumberOfPeopleInside;

    public void AbductPeople(int amount) => _currentNumberOfPeopleInside -= amount;

    public void ApplyDamage(float damage) => _currentBuildingHealth -= damage;
}