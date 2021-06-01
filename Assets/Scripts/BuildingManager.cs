using System.Collections;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private GameObject brokenBuilding;
    [SerializeField] private GameObject brokenBuildingEffect;
    [SerializeField] private GameObject collapsingBuildingEffect;
    [SerializeField] private float buildingHealthMultiplier;
    [SerializeField] private int buildingPeopleMultiplier;
    [Range(0f, .1f)] [SerializeField] private float shakeIntensity;
    [SerializeField] private float buildingCollapseRate;
    [SerializeField] private float collapseEffectDuration;
    [SerializeField] private float damageShakeDuration;

    private IEnumerator ShakeBuilding(GameObject building, float duration)
    {
        if (building != null && duration > 0)
        {
            var randPos = Random.insideUnitSphere * shakeIntensity;
            building.transform.Translate(new Vector3(randPos.x, building.transform.position.y, randPos.z));
            yield return new WaitForSeconds(duration);
        }
    }

    public GameObject PlayBuildingBrokenEffect(GameObject building)
    {
        var bScript = building.GetComponent<Building>();
        if (brokenBuildingEffect != null && bScript != null)
        {
            if (bScript.GetHasBrokenEffect()) return null;
            {
                var brokenEffect = Instantiate(brokenBuildingEffect, building.transform.position, Quaternion.identity, building.transform);
                var rubble = brokenEffect.transform.GetChild(0).transform.GetChild(0).gameObject;
                if (rubble != null)
                {
                    rubble.transform.localScale = new Vector3(building.transform.localScale.x, rubble.transform.localScale.y, building.transform.localScale.z);
                    //rubble.transform.rotation = Quaternion.Euler(0, Random.Range(-360f, 360f), 0);
                }
                return brokenEffect;
            }
        }
        return null;
    }

    public GameObject PlayBuildingCollapseEffect(GameObject building)
    {
        var bScript = building.GetComponent<Building>();
        if (brokenBuildingEffect != null && collapseEffectDuration > 0 && bScript != null)
        {
            if (bScript.GetHasCollapseEffect()) return null;
            var myCollapseEffect = Instantiate(collapsingBuildingEffect, building.transform.position, Quaternion.identity);
            Destroy(myCollapseEffect, collapseEffectDuration);
            return myCollapseEffect;
        }
        return null;
    }

    public void ApplyBuildingDamage(Building building, float amount)
    {
        if (building != null && amount > 0)
        {
            if (!building.GetIsCollapsing())
            {
                ShakeThisBuilding(building.gameObject, damageShakeDuration);
                building.ApplyDamage(amount);
                if (building.GetCurrentHealth() <= 0)
                    building.SetIsCollapsing();
            }
        }
    }

    public void InitializeBrokenBuilding(GameObject building, GameObject effect)
    {
        var buildingSize = new Vector3(building.transform.localScale.x, building.transform.localScale.y,
            building.transform.localScale.z);
        {
            if (brokenBuilding != null && effect != null)
            {
                brokenBuilding.transform.localScale = buildingSize;
                var iB = Instantiate(brokenBuilding, building.transform.position, Quaternion.identity);
                effect.transform.SetParent(iB.transform);
                Destroy(building);
            }
        }
    }

    public void TryDamageBuilding(GameObject target)
    {
        float damage = 0;
        if (weaponManager != null)
            damage = weaponManager.GetWeaponDamage();
        var building = target.GetComponent<Building>();
        if (building != null)
            ApplyBuildingDamage(building, damage);
    }

    public void ShakeThisBuilding(GameObject building, float duration) => StartCoroutine(ShakeBuilding(building, duration));

    public int GetBuildingPeopleMultiplier() => buildingPeopleMultiplier;

    public float GetBuildingHealthMultiplier() => buildingHealthMultiplier;
}