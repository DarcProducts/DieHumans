using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    [Range(0f, 1f)] [SerializeField] private float willBeDestroyed;
    [SerializeField] private GameObject house = null;
    [SerializeField] private Material[] niceMaterials;
    private float yPlaneLevel = 0;
    [SerializeField] private int gridWidth = 0;
    [SerializeField] private int gridHeight = 0;
    [SerializeField] private int gridCellSize = 0;
    [SerializeField] private int minHouseHeight;
    [SerializeField] private int maxHouseHeight;
    [SerializeField] private float minHouseWidth;
    [SerializeField] private float maxHouseWidth;
    [SerializeField] private bool showNumbers = true;
    [SerializeField] private bool showLines = true;
    [SerializeField] private Color gridNumberColor = Color.white;
    [SerializeField] private Color gridXColor = Color.white;
    [SerializeField] private Color gridZColor = Color.white;
    GameObject cityOrigin;

    private Vector3 GetWorldPosition(int x, float y, int z, float cellSize) => new Vector3(x, y, z) * cellSize;

    public void CreateDarcGrid(Vector3 origin)
    {
        yPlaneLevel = origin.y / gridCellSize;
        int currentNumber = 0;
        int[,] gridArray = new int[gridWidth, gridHeight];
        GameObject buildings = new GameObject("Buildings");
        cityOrigin = buildings;
        cityOrigin.transform.position = new Vector3(origin.x, yPlaneLevel, origin.z);

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                if (house != null)
                {
                    if (Random.value > willBeDestroyed)
                    {
                        SetLocalScale(house, new Vector3(Random.Range(minHouseWidth, maxHouseWidth), Random.Range(minHouseHeight, maxHouseHeight), Random.Range(minHouseWidth, maxHouseWidth)));
                        GameObject newHouse = GameObject.Instantiate(house, GetWorldPosition(x, yPlaneLevel, z, gridCellSize) + new Vector3(gridCellSize, yPlaneLevel, gridCellSize) * .5f, Quaternion.identity, buildings.transform);
                        newHouse.name = $"Building {x} {z}";

                        if (newHouse.transform.localScale.y > maxHouseHeight / 1.5f && niceMaterials.Length > 2)
                            newHouse.GetComponent<Renderer>().material = niceMaterials[2];
                        else if (newHouse.transform.localScale.y <= maxHouseHeight / 2 && newHouse.transform.localScale.y >= maxHouseHeight / 4 && niceMaterials.Length > 1)
                            newHouse.GetComponent<Renderer>().material = niceMaterials[1];
                        else if (niceMaterials.Length > 0)
                            newHouse.GetComponent<Renderer>().material = niceMaterials[0];
                    }


                    if (showNumbers)
                    {
                        CreateWorldText(currentNumber.ToString(), TextAnchor.MiddleCenter, TextAlignment.Center, gridNumberColor,
                            GetWorldPosition(x, yPlaneLevel, z, gridCellSize) + new Vector3(gridCellSize, gridCellSize, gridCellSize) * .5f);
                        currentNumber++;
                    }
                }

                if (showLines)
                {
                    Debug.DrawLine(GetWorldPosition(x, yPlaneLevel, z, gridCellSize), GetWorldPosition(x, yPlaneLevel, z + 1, gridCellSize), gridZColor, Mathf.Infinity);
                    Debug.DrawLine(GetWorldPosition(x, yPlaneLevel, z, gridCellSize), GetWorldPosition(x + 1, yPlaneLevel, z, gridCellSize), gridXColor, Mathf.Infinity);
                }
            }
        }
        if (showLines)
        {
            Debug.DrawLine(GetWorldPosition(0, yPlaneLevel, gridHeight, gridCellSize), GetWorldPosition(gridWidth, yPlaneLevel, gridHeight, gridCellSize), Color.white, Mathf.Infinity);
            Debug.DrawLine(GetWorldPosition(gridWidth, yPlaneLevel, 0, gridCellSize), GetWorldPosition(gridWidth, yPlaneLevel, gridHeight, gridCellSize), Color.white, Mathf.Infinity);
        }
        house.transform.localScale = new Vector3(1, 1, 1);
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
            cB[0] = new Vector3(-cityOrigin.transform.position.x, yPlaneLevel * gridCellSize, gridWidth * gridCellSize - cityOrigin.transform.position.x);
            cB[1] = new Vector3(-cityOrigin.transform.position.z, yPlaneLevel * gridCellSize, gridHeight * gridCellSize - cityOrigin.transform.position.z);
            Debug.Log($"City Bounds: " + cB[0] + " " + cB[1]);
            return cB;
        }
        return new Vector3[2];
    }

    public void SetCityOrigin(Vector3 newOrigin)
    {
        if (cityOrigin != null)
            cityOrigin.transform.position = newOrigin;
    }

    public TextMesh CreateWorldText(string text, TextAnchor textAnchor, TextAlignment textAlignment, Color fontColor = default,
        Vector3 localPosition = default, int fontSize = 40, int sortingOrder = 0, Transform parent = null)
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

    public float GetGridSize() => gridCellSize;
    public float GetMaxBuildingHeight() => maxHouseHeight;
}