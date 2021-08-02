using UnityEngine;

public class KillSheet : MonoBehaviour
{
    int dronesDestroyed = 0;
    int tanksDestroyed = 0;
    int bombersDestroyed = 0;

    void OnEnable()
    {
        Drone.UpdateDroneKillCount += UpdateDroneDestroyed;
        Bomber.UpdateBomberKillCount += UpdateBomberDestroyed;
        Tank.UpdateTankKillCount += UpdateTankDestroyed;
    }

    void OnDisable()
    {
        Drone.UpdateDroneKillCount -= UpdateDroneDestroyed;
        Bomber.UpdateBomberKillCount -= UpdateBomberDestroyed;
        Tank.UpdateTankKillCount -= UpdateTankDestroyed;
    }

    public void UpdateDroneDestroyed() => dronesDestroyed++;

    public void UpdateTankDestroyed() => tanksDestroyed++;

    public void UpdateBomberDestroyed() => bombersDestroyed++;

    public int GetDronesDestroyed() => dronesDestroyed;

    public int GetTanksDestroyed() => tanksDestroyed;

    public int GetBombersDestroyed() => bombersDestroyed;
}