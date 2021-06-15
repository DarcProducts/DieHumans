using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject spaceShipTarget;
    [SerializeField] private float maxYDestroy = 0f;
    [SerializeField] private float thrustPower;
    [SerializeField] private float rotateSpeed;

    public void TransformShip(Vector3 direction, float buttonStrength)
    {
        if (spaceShipTarget == null) return;
        if (spaceShipTarget != null)
        {
            spaceShipTarget.transform.Translate(buttonStrength * thrustPower * Time.deltaTime * direction.normalized);
            CheckHeight();
        }
    }

    public void RotateShip(Vector3 direction, float buttonStrength)
    {
        if (spaceShipTarget != null)
            spaceShipTarget.transform.Rotate(buttonStrength * rotateSpeed * Time.deltaTime * direction.normalized);
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