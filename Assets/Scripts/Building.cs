using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour, IDamagable<float>
{
    public static UnityAction<Vector3> BuildingDamaged;
    public static UnityAction<GameObject> BuildingDestroyed;
    public static UnityAction<Vector3, byte, byte, float> TextInfo;
    public static UnityAction<Vector3, string, float, Color> DamagedInfo;
    [SerializeField] float buildingHealthMultiplier;
    [SerializeField] int buildingPeopleMultiplier;
    [SerializeField] Material brokenMaterial;
    float _currentBuildingHealth;
    int _currentNumberOfPeopleInside;
    [SerializeField] float collapseRate = .4f;
    [SerializeField] float damageDiplayDuration;
    GameObject player;
    ObjectPools objectPools;
    float maxBuildingHealth;
    int maxNumberOfPeopleInside;
    bool brokenEffectSet = false;
    bool collapsingEffectSet = false;
    bool isCollapsing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        objectPools = FindObjectOfType<ObjectPools>();
        if (objectPools == null)
            Debug.LogWarning($"{gameObject.name} could not find an object with a ObjectPools component attached");
        maxBuildingHealth = transform.localScale.y * buildingHealthMultiplier;
        maxNumberOfPeopleInside = (int)transform.localScale.y * buildingPeopleMultiplier;
        _currentBuildingHealth = maxBuildingHealth;
        _currentNumberOfPeopleInside = maxNumberOfPeopleInside;
    }

    void LateUpdate()
    {
        if (isCollapsing)
            SlowlyCollapseBuilding();
    }

    void InitializeCollapsingEffect()
    {
        if (!collapsingEffectSet && objectPools != null)
        {
            GameObject effect = objectPools.GetCollapseEffect();
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
        if (!brokenEffectSet && objectPools != null)
        {
            GameObject e = objectPools.GetBrokenEffect();
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
        GetComponent<Renderer>().material = brokenMaterial;
        transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(transform.localScale.x, 0, transform.localScale.z), collapseRate * Time.fixedDeltaTime);
        if (transform.localScale.y <= 0)
        {
            InitializeBrokenBuildingEffect();
            TextInfo?.Invoke(transform.position, 3, 1, maxBuildingHealth);
            BuildingDestroyed?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }

    public float GetCurrentHealth() => _currentBuildingHealth;

    public int GetNumberOfPeopleInside() => _currentNumberOfPeopleInside;

    public void AbductPeople(int amount) => _currentNumberOfPeopleInside -= amount;

    public void ApplyDamage(float damage)
    {
        _currentBuildingHealth -= damage;
        BuildingDamaged?.Invoke(transform.position);
        DamagedInfo?.Invoke(transform.position + Vector3.up * 30, damage.ToString(), 64, Color.red);
        if (_currentBuildingHealth <= 0)
        {
            InitializeCollapsingEffect();
            isCollapsing = true;
        }
    }


    [ContextMenu("Damage building")]
    public void TestDamage() => ApplyDamage(10000);
}