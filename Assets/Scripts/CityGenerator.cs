using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CityGenerator : MonoBehaviour
{
    public static UnityAction AllBuildingsDestroyed;
    [Range(0.001f, .999f)] [SerializeField] float willBeDestroyed;
    [SerializeField] GameObject building = null;
    [SerializeField] int gridWidth = 0;
    [SerializeField] int gridHeight = 0;
    [SerializeField] int gridDepth = 0;
    [SerializeField] int gridCellSize = 0;
    [SerializeField] int minBuildingHeight;
    [SerializeField] int maxBuildingHeight;
    [SerializeField] float minBuildingWidth;
    [SerializeField] float maxBuildingWidth;
    [SerializeField] Material buildingMat;
    int buildingsDestroyed = 0;
    int yPlaneLevel = 0;
    readonly List<Vector3> attackTargets = new List<Vector3>();

    void OnEnable()
    {
        Building.RemoveBuildingVector += RemoveAttackTarget;
        Building.UpdateBuildingCount += BuildingDestroyed;
    }

    void OnDisable()
    {
        Building.RemoveBuildingVector -= RemoveAttackTarget;
        Building.UpdateBuildingCount -= BuildingDestroyed;
    }

    private void Start()
    {
        CreateDarcGrid(Vector3.zero);
    }

    Vector3 GetWorldPosition(int x, int y, int z, float cellSize) => new Vector3(x, y, z) * cellSize;

    public void CreateDarcGrid(Vector3 origin)
    {
        yPlaneLevel = Mathf.FloorToInt(origin.y / gridCellSize);
        int[,,] gridArray = new int[gridWidth, gridHeight, gridDepth];
        GameObject buildings = new GameObject("Buildings");
        if (buildings != null)
            buildings.transform.position = origin;

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    if (building != null)
                    {
                        if (y.Equals(yPlaneLevel))
                        {
                            if (Random.value > willBeDestroyed)
                            {
                                SetLocalScale(building, new Vector3(Random.Range(Mathf.RoundToInt(minBuildingWidth), Mathf.RoundToInt(maxBuildingWidth)), Random.Range(Mathf.RoundToInt(minBuildingHeight), Mathf.RoundToInt(maxBuildingHeight)), Random.Range(Mathf.RoundToInt(minBuildingWidth), Mathf.RoundToInt(maxBuildingWidth))));
                                GameObject newBuilding = GameObject.Instantiate(building, GetWorldPosition(x, yPlaneLevel, z, gridCellSize) + new Vector3(gridCellSize, yPlaneLevel, gridCellSize) * .5f, Quaternion.identity, buildings.transform);
                                newBuilding.name = $"Building: {newBuilding.transform.position.x} {newBuilding.transform.position.z}";
                                Renderer rend = newBuilding.GetComponent<Renderer>();
                                if (rend != null)
                                   rend.material = buildingMat;
                                attackTargets.Add(newBuilding.transform.position);
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetLocalScale(GameObject scaleObject, Vector3 newSize = default)
    {
        if (scaleObject != null && newSize.x >= 0 && newSize.y >= 0 && newSize.z >= 0)
            scaleObject.transform.localScale = new Vector3(newSize.x, newSize.y, newSize.z);
    }

    public int GetNumberBuildingsLeft() => attackTargets.Count;

    public void RemoveAttackTarget(Vector3 target)
    {
        attackTargets.Remove(target);
        if (attackTargets.Count.Equals(0))
            AllBuildingsDestroyed?.Invoke();
    }

    public Vector3 GetAttackVector()
    {
        if (attackTargets.Count > 0)
        {
            int ranNum = Random.Range(0, attackTargets.Count);
            return attackTargets[ranNum];
        }
        return new Vector3(Random.Range(0, 310), 0, Random.Range(0, 310));
    }

    public void BuildingDestroyed() => buildingsDestroyed++;

    public int GetBuildingsDestroyed() => buildingsDestroyed;
}