using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CityGenerator cityGenerator;
    [SerializeField] private GameObject spaceShipTarget;
    [SerializeField] private GameObject ship;
    [SerializeField] private float maxYDestroy = 0f;
    [SerializeField] private float thrustPower;
    [SerializeField] private float rotateSpeed;
    private Vector3[] cityBounds = new Vector3[2];

    private void Start()
    {
        if (cityGenerator != null)
            cityBounds = cityGenerator.GetCityBounds();
    }

    public void TransformShip(Vector3 direction, float buttonStrength)
    {
        if (spaceShipTarget == null) return;
        if (spaceShipTarget != null)
        {
            spaceShipTarget.transform.Translate(direction.normalized * thrustPower * buttonStrength * Time.deltaTime);
            CheckHeight();
        }
    }

    public void RotateShip(Vector3 direction, float buttonStrength)
    {
        if (spaceShipTarget != null)
            spaceShipTarget.transform.Rotate(direction.normalized * rotateSpeed * buttonStrength * Time.deltaTime);
    }

    public void CheckHeight()
    {
        if (spaceShipTarget == null) return;
        if (spaceShipTarget.transform.position.y < maxYDestroy)
            DestroyShip();
    }

    public void DestroyShip()
    {
        if (spaceShipTarget == null) return;
        spaceShipTarget.SetActive(false);
    }
}