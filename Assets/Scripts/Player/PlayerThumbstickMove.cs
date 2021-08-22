using UnityEngine;

public class PlayerThumbstickMove : MonoBehaviour
{
    [SerializeField] Transform ship;
    [SerializeField] Vector2Variable leftThumbstick;
    [SerializeField] Vector2Variable rightThumbstick;
    [SerializeField] FloatVariable moveSpeed;
    [SerializeField] Transform aimTarget;
    [SerializeField] FloatVariable rotateSpeed;
    [Range(0f, 1f)] [SerializeField] float deadZone;
    [SerializeField] BoolVariable rollYaw;

    void Update()
    {
        if (leftThumbstick.Value.y > deadZone)
            MoveInDirection(ship.forward, false);
        if (leftThumbstick.Value.y < -deadZone)
            MoveInDirection(-ship.forward, false);
        if (leftThumbstick.Value.x < -deadZone)
            MoveInDirection(-ship.right, false);
        if (leftThumbstick.Value.x > deadZone)
            MoveInDirection(ship.right, false);

        if (rightThumbstick.Value.y > deadZone)
            MoveInDirection(Vector3.up, true);
        if (rightThumbstick.Value.y < -deadZone)
            MoveInDirection(Vector3.down, true);
    }

    private void FixedUpdate()
    {
        if (rollYaw.Value)
            RotateShip();
    }

    void RotateShip() => ship.rotation = Quaternion.Lerp(ship.rotation, aimTarget.rotation, rotateSpeed.Value * Time.fixedDeltaTime);

    void MoveInDirection(Vector3 direction, bool halfValue)
    {
        if (moveSpeed == null || ship == null)
            return;
        float mS;
        if (halfValue)
            mS = moveSpeed.Value * .5f;
        else
            mS = moveSpeed.Value;

        ship.Translate(mS * Time.fixedDeltaTime * direction.normalized, Space.World);
    }
}