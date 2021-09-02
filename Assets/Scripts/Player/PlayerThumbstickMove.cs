using UnityEngine;

public class PlayerThumbstickMove : MonoBehaviour
{
    public GlobalBoolVariable gamePaused;
    public GlobalVector2Variable leftThumbstick;
    public GlobalVector2Variable rightThumbstick;
    public GlobalBoolVariable leftHandTriggerActive;
    public GlobalBoolVariable leftHandGripActive;
    public GlobalBoolVariable rightHandGripActive;
    public Transform ship;
    public float moveSpeed;
    public float rotateSpeed;
    public Transform aimTarget;
    public float aimRotateSpeed;

    void Update()
    {
        if (!gamePaused.Value)
        {
            if (leftThumbstick.Value.y > .1f)
                MoveInDirection(ship.forward, false);
            if (leftThumbstick.Value.y < -.1f)
                MoveInDirection(-ship.forward, false);
            if (leftThumbstick.Value.x < -.1f)
                MoveInDirection(-ship.right, true);
            if (leftThumbstick.Value.x > .1f)
                MoveInDirection(ship.right, true);

            if (leftHandTriggerActive.Value)
                MoveInDirection(Vector3.up, true);
            if (leftHandGripActive.Value)
                MoveInDirection(Vector3.down, true);

            if (rightThumbstick.Value.x < -.1f)
                TurnShip(Vector3.down);
            if (rightThumbstick.Value.x > .1f)
                TurnShip(Vector3.up);
        }
    }

    private void FixedUpdate()
    {
        if (!gamePaused.Value)
            if (rightHandGripActive.Value)
                RotateShip();
    }

    void RotateShip() => ship.rotation = Quaternion.Lerp(ship.rotation, aimTarget.rotation, aimRotateSpeed * Time.fixedDeltaTime);

    void TurnShip(Vector3 dir) => ship.Rotate(rotateSpeed * Time.fixedDeltaTime * dir.normalized, Space.Self);

    void MoveInDirection(Vector3 dir, bool halfValue)
    {
        if (ship == null)
            return;
        float mS;
        if (halfValue)
            mS = moveSpeed * .5f;
        else
            mS = moveSpeed;
        ship.Translate(mS * Time.fixedDeltaTime * dir.normalized, Space.World);
    }
}