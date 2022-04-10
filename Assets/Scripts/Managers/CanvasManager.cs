using System.Text;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] CityGenerator cityGenerator;
    [SerializeField] KillSheet killSheet;
    [SerializeField] TMP_Text buildingsText;
    [SerializeField] TMP_Text buildingsDestroyedText;
    [SerializeField] TMP_Text dronesDestroyedText;
    [SerializeField] TMP_Text tanksDestroyedText;
    [SerializeField] TMP_Text bombersDestroyedText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] int currentPoints = 0;
    int currentBuildingsDestroyed;
    int currentDronesDestroyed;
    int currentTanksDestroyed;
    int currentBombersDestroyed;

    void OnEnable()
    {
        Building.UpdateBuildingCount += UpdateBuildingsLeft;
        Drone.UpdateDroneKillCount += UpdateDroneDestroyedText;
        Bomber.UpdateBomberKillCount += UpdateBomberDestroyedText;
        Tank.UpdateTankKillCount += UpdateTankDestroyedText;
    }

    void OnDisable()
    {
        Building.UpdateBuildingCount -= UpdateBuildingsLeft;
        Drone.UpdateDroneKillCount -= UpdateDroneDestroyedText;
        Bomber.UpdateBomberKillCount -= UpdateBomberDestroyedText;
        Tank.UpdateTankKillCount -= UpdateTankDestroyedText;
    }

    void Start()
    {
        if (cityGenerator != null && buildingsText != null)
            buildingsText.text = $"Buildings: \n{cityGenerator.GetNumberBuildings()}";
        if (cityGenerator != null && buildingsDestroyedText != null)
            buildingsDestroyedText.text = $"Buildings Destroyed: \n{cityGenerator.GetBuildingsDestroyed()}";
        if (killSheet != null && dronesDestroyedText != null)
            dronesDestroyedText.text = $"Drones Destroyed: \n{0}";
        if (killSheet != null && tanksDestroyedText != null)
            tanksDestroyedText.text = $"Tanks Destroyed: \n{0}";
        if (killSheet != null && bombersDestroyedText != null)
            bombersDestroyedText.text = $"Bombers Destroyed: \n{0}";
        if (scoreText != null)
            scoreText.text = "Score: \n0";
    }

    void UpdateBuildingsLeft()
    {
        if (cityGenerator == null || buildingsText == null) return;
        buildingsText.text = $"Buildings: \n{""}";
        UpdateBuildingsDestroyedText();
    }

    void UpdateBuildingsDestroyedText()
    {
        if (cityGenerator == null || buildingsDestroyedText == null) return;
        currentBuildingsDestroyed++;
        buildingsDestroyedText.text = $"Buildings Destroyed: \n{cityGenerator.GetBuildingsDestroyed()}";
    }

    void UpdateDroneDestroyedText()
    {
        if (killSheet == null || dronesDestroyedText == null) return;
        killSheet.dronesDestroyed.Value++;
        currentDronesDestroyed++;
        dronesDestroyedText.text = $"Drones Destroyed: \n{currentDronesDestroyed}";
    }

    void UpdateTankDestroyedText()
    {
        if (killSheet == null || tanksDestroyedText == null) return;
        killSheet.tanksDestroyed.Value++;
        currentTanksDestroyed++;
        tanksDestroyedText.text = $"Tanks Destroyed: \n{currentTanksDestroyed}";
    }

    void UpdateBomberDestroyedText()
    {
        if (killSheet == null || bombersDestroyedText == null) return;
        killSheet.bombersDestroyed.Value++;
        currentBombersDestroyed++;
        bombersDestroyedText.text = $"Bombers Destroyed: \n{currentBombersDestroyed}";
    }

    void UpdateScore(int points, bool positive)
    {
        if (positive.Equals(true))
            currentPoints += points;
        else
            currentPoints -= points;
        if (scoreText != null)
        {
            scoreText.text = $"Score: \n{currentPoints}";
            if (currentPoints < 0)
                scoreText.color = Color.red;
            else
                scoreText.color = Color.green;
        }
    }
}