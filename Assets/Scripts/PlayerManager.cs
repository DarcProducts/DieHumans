using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float maxYDestroy = 0f;
    [SerializeField] float shipSpeedForwardBack;
    [SerializeField] float shipSpeedUpDown;

    public void TransformShip(Vector3 direction, float buttonStrength, bool isForwardBack)
    {
        if (player == null) return;
        if (isForwardBack)
            player.transform.Translate(buttonStrength * shipSpeedForwardBack * Time.fixedDeltaTime * direction.normalized);
        else
            player.transform.Translate(buttonStrength * shipSpeedUpDown * Time.fixedDeltaTime * direction.normalized);
        CheckHeight();
    }

    public void RotateShip(Vector3 direction, float buttonStrength)
    {
        if (player != null)
            player.transform.Rotate(buttonStrength * Time.fixedDeltaTime * direction.normalized);
    }

    public void CheckHeight()
    {
        if (player == null) return;
        if (player.transform.position.y < maxYDestroy)
            player.SetActive(false);
    }

    public float GetShipSpeedFB() => shipSpeedForwardBack;
    public float GetShipSpeedUD() => shipSpeedUpDown;
    public void IncreaseShipSpeeds(float value)
    {
        shipSpeedForwardBack += value;
        shipSpeedUpDown += value * .5f;
    }
}