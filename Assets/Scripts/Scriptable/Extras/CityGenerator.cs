using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(menuName = "Generator/New City Generator")]
public class CityGenerator : ScriptableObject
{
    public static UnityAction AllBuildingsDestroyed;
    [Range(0.001f, .999f)] public float willBeDestroyed;
    public GameObject building = null;
    public int gridWidth = 0;
    public int gridHeight = 0;
    public int gridDepth = 0;
    public int gridCellSize = 0;
    public int minBuildingHeight;
    public int maxBuildingHeight;
    public float minBuildingWidth;
    public float maxBuildingWidth;
    public Material buildingMat;
    int _buildingsDestroyed = 0;
    public int yPlaneLevel = 0;
    readonly List<Vector3> attackTargets = new List<Vector3>();
    Vector3 attackVector;

    void OnEnable() => Building.UpdateBuildingCount += BuildingDestroyed;

    void OnDisable() => Building.UpdateBuildingCount -= BuildingDestroyed;

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

    public void ResetDestroyedBuildingCount() => _buildingsDestroyed = 0;

    public void SetLocalScale(GameObject scaleObject, Vector3 newSize = default)
    {
        if (scaleObject != null && newSize.x >= 0 && newSize.y >= 0 && newSize.z >= 0)
            scaleObject.transform.localScale = new Vector3(newSize.x, newSize.y, newSize.z);
    }

    public int GetNumberBuildings() => attackTargets.Count;

    public void RemoveAttackTarget(Vector3 target)
    {
        attackTargets.Remove(target);
        _buildingsDestroyed++;
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
        attackVector.x = Random.Range(10, 280);
        attackVector.y = yPlaneLevel;
        attackVector.z = Random.Range(10, 280);
        return attackVector;
    }

    public void BuildingDestroyed() => _buildingsDestroyed++;

    public int GetBuildingsDestroyed() => _buildingsDestroyed;
}