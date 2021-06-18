using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private GameObject brokenBuilding;
    [SerializeField] private GameObject brokenBuildingEffect;
    [SerializeField] private int brokenEffectInitialPoolSize;
    [SerializeField] private GameObject collapseEffect;
    [SerializeField] private int collapseEffectInitialPoolSize;
    private readonly List<GameObject> brokenEffectPool = new List<GameObject>();
    private readonly List<GameObject> collapseEffectPool = new List<GameObject>();

    private void Start()
    {
        InitializeBrokenEffectPool();
        InitializeCollapseEffectPool();
    }

    private void OnEnable() => Building.BreakBuilding += BreakBuilding;

    private void OnDisable() => Building.BreakBuilding -= BreakBuilding;

    public void InitializeBrokenEffectPool()
    {
        brokenEffectPool.Clear();
        if (brokenBuildingEffect != null)
        {
            for (int i = 0; i < brokenEffectInitialPoolSize; i++)
            {
                GameObject e = Instantiate(brokenBuildingEffect, Vector3.down, Quaternion.identity, transform);
                e.SetActive(false);
                brokenEffectPool.Add(e);
            }
        }
    }

    public void InitializeCollapseEffectPool()
    {
        collapseEffectPool.Clear();
        if (collapseEffect != null)
        {
            for (int i = 0; i < collapseEffectInitialPoolSize; i++)
            {
                GameObject c = Instantiate(collapseEffect, Vector3.down, Quaternion.identity, transform);
                c.SetActive(false);
                collapseEffectPool.Add(c);
            }
        }
    }

    public void BreakBuilding(GameObject building)
    {
        if (brokenBuilding != null)
        {
            GameObject newBrokenBuilding = Instantiate(brokenBuilding, building.transform.position, Quaternion.identity, transform);
            newBrokenBuilding.transform.localScale = building.transform.localScale;
        }
    }

    public GameObject GetBrokenEffect()
    {
        for (int i = 0; i < brokenEffectPool.Count; i++)
            if (!brokenEffectPool[i].activeSelf)
                return brokenEffectPool[i];
        if (brokenBuildingEffect != null)
        {
            GameObject newBrokenEffect = Instantiate(brokenBuilding, Vector3.down, Quaternion.identity, transform);
            newBrokenEffect.SetActive(false);
            brokenEffectPool.Add(newBrokenEffect);
            return newBrokenEffect;
        }
        return null;
    }

    public GameObject GetCollapseEffect()
    {
        for (int i = 0; i < collapseEffectPool.Count; i++)
            if (!collapseEffectPool[i].activeSelf)
                return collapseEffectPool[i];
        if (collapseEffect != null)
        {
            GameObject newCollapseEffect = Instantiate(collapseEffect, Vector3.down, Quaternion.identity, transform);
            newCollapseEffect.SetActive(false);
            collapseEffectPool.Add(newCollapseEffect);
            return newCollapseEffect;
        }
        return null;
    }
}