using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public int numberOfBuildings = 0;
    [Range(0f, 1f)] [SerializeField] float willBeDestroyed;
    [SerializeField] GameObject building = null;
    [SerializeField] int gridWidth = 0;
    [SerializeField] int gridHeight = 0;
    [SerializeField] int gridDepth = 0;
    [SerializeField] int gridCellSize = 0;
    [SerializeField] int minBuildingHeight;
    [SerializeField] int maxBuildingHeight;
    [SerializeField] float minBuildingWidth;
    [SerializeField] float maxBuildingWidth;
    [SerializeField] bool showNumbers = true;
    [SerializeField] bool showLines = true;
    [SerializeField] Color gridNumberColor = Color.white;
    [SerializeField] Color gridXColor = Color.white;
    [SerializeField] Color gridZColor = Color.white;
    [SerializeField] Material buildingMat;
    [SerializeField] Material building1Mat;
    [SerializeField] Material building2Mat; 
    int yPlaneLevel = 0;
    readonly List<Vector3> groundUnitSafeLocations = new List<Vector3>();
    Vector3 cityOrigin;

    void OnEnable()
    {
        Building.BreakBuilding += RemoveABuilding;
    }

    void OnDisable()
    {
        Building.BreakBuilding -= RemoveABuilding;
    }

    Vector3 GetWorldPosition(int x, int y, int z, float cellSize) => new Vector3(x, y, z) * cellSize;

    public void CreateDarcGrid(Vector3 origin)
    {
        cityOrigin = origin; 
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
                                if (newBuilding.transform.localScale.y > maxBuildingHeight * .8f && building2Mat != null)
                                    newBuilding.GetComponent<Renderer>().material = building2Mat;
                                else if (newBuilding.transform.localScale.y <= maxBuildingHeight *.8f && newBuilding.transform.localScale.y >= maxBuildingHeight / 4 && building1Mat != null)
                                    newBuilding.GetComponent<Renderer>().material = building1Mat;
                                else if (buildingMat != null)
                                    newBuilding.GetComponent<Renderer>().material = buildingMat;
                                if (showNumbers)
                                {
                                    CreateWorldText(".", TextAnchor.MiddleCenter, TextAlignment.Center, gridNumberColor,
                                        GetWorldPosition(x, yPlaneLevel, z, gridCellSize) + new Vector3(gridCellSize, gridCellSize, gridCellSize) * .5f);
                                }
                                numberOfBuildings++;
                            }
                            else
                                groundUnitSafeLocations.Add(new Vector3(x, y, z));
                        }

                        if (showLines)
                        {
                            Debug.DrawLine(GetWorldPosition(x, y, z, gridCellSize), GetWorldPosition(x, y, z + 1, gridCellSize), gridZColor, Mathf.Infinity);
                            Debug.DrawLine(GetWorldPosition(x, y, z, gridCellSize), GetWorldPosition(x + 1, y, z, gridCellSize), gridXColor, Mathf.Infinity);
                        }
                    }
                }
            }
        }
        if (building != null)
            building.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetLocalScale(GameObject scaleObject, Vector3 newSize = default)
    {
        if (scaleObject != null && newSize.x >= 0 && newSize.y >= 0 && newSize.z >= 0)
            scaleObject.transform.localScale = new Vector3(newSize.x, newSize.y, newSize.z);
    }

    [ContextMenu("Get City Bounds")]
    public Vector3[] GetCityBounds()
    {
        if (cityOrigin != null)
        {
            Vector3[] cB = new Vector3[2];
            cB[0] = new Vector3(-cityOrigin.x, yPlaneLevel * gridCellSize, gridWidth * gridCellSize - cityOrigin.x);
            cB[1] = new Vector3(-cityOrigin.z, gridHeight * gridCellSize, gridDepth * gridCellSize - cityOrigin.z);
            return cB;
        }
        return new Vector3[2];
    }

    public void SetCityOrigin(Vector3 newOrigin)
    {
        if (cityOrigin != null)
            cityOrigin = newOrigin;
    }

    public void RemoveABuilding(GameObject notUsed) => numberOfBuildings--;

    public TextMesh CreateWorldText(string text, TextAnchor textAnchor, TextAlignment textAlignment, Color fontColor = default,
        Vector3 localPosition = default, int fontSize = 20, int sortingOrder = 0, Transform parent = null)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = fontColor;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    public Vector3[] GetTankSafePoints() => groundUnitSafeLocations.ToArray();

    public float GetGridSize() => gridCellSize;

    public float GetMaxBuildingHeight() => maxBuildingHeight;
}