using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float maxYDestroy = 0f;
    [SerializeField] private float thrustPower;
    [SerializeField] private float rotateSpeed;

    public void TransformShip(Vector3 direction, float buttonStrength)
    {
        if (player == null) return;
        if (player != null)
        {
            player.transform.Translate(buttonStrength * thrustPower * Time.deltaTime * direction.normalized);
            CheckHeight();
        }
    }

    public void RotateShip(Vector3 direction, float buttonStrength)
    {
        if (player != null)
            player.transform.Rotate(buttonStrength * rotateSpeed * Time.deltaTime * direction.normalized);
    }

    public void CheckHeight()
    {
        if (player == null) return;
        if (player.transform.position.y < maxYDestroy)
            DestroyPlayer();
    }

    public void DestroyPlayer()
    {
        if (player == null) return;
        player.SetActive(false);
    }
}