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
    [SerializeField] TMP_Text currentWaveText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] int currentPoints = 0;

    void Awake()
    {
        if (cityGenerator == null)
            Debug.LogWarning($"Cannot find a GameObject with the CityGenerator component attached");
        if (killSheet == null)
            Debug.LogWarning($"Cannot find a GameObject with the KillSheet component attached");
    }

    void OnEnable()
    {
        Building.BuildingDestroyed += UpdateBuildingsLeft;
        Drone.UpdateDroneKillCount += UpdateDroneDestroyedText;
        WaveManager.UpdateWave += UpdateCurrentWave;
        MessagesManager.UpdateScore += UpdateScore;
    }

    void OnDisable()
    {
        Building.BuildingDestroyed -= UpdateBuildingsLeft;
        Drone.UpdateDroneKillCount -= UpdateDroneDestroyedText;
        WaveManager.UpdateWave -= UpdateCurrentWave;
        MessagesManager.UpdateScore -= UpdateScore;
    }

    void Start()
    {
        if (cityGenerator != null && buildingsText != null)
            buildingsText.text = $"Buildings: \n{cityGenerator.GetNumberBuildingsLeft()}";
        if (cityGenerator != null && buildingsDestroyedText != null)
            buildingsDestroyedText.text = $"Buildings Destroyed: \n{cityGenerator.GetBuildingsDestroyed()}";
        if (killSheet != null && dronesDestroyedText != null)
            dronesDestroyedText.text = $"Drones Destroyed: \n{killSheet.dronesDestroyed}";
        if (killSheet != null && tanksDestroyedText != null)
            tanksDestroyedText.text = $"Tanks Destroyed: \n{killSheet.tanksDestroyed}";
        if (killSheet != null && bombersDestroyedText != null)
            bombersDestroyedText.text = $"Bombers Destroyed: \n{killSheet.bombersDestroyed}";
        if (scoreText != null)
            scoreText.text = "Score: \n0";
    }

    void UpdateBuildingsLeft(GameObject notUsed)
    {
        if (cityGenerator != null && buildingsText != null)
            buildingsText.text = $"Buildings: \n{cityGenerator.GetNumberBuildingsLeft()}";
        UpdateBuildingsDestroyedText();
    }

    void UpdateBuildingsDestroyedText()
    {
        if (cityGenerator != null && buildingsDestroyedText != null)
            buildingsDestroyedText.text = $"Buildings Destroyed: \n{cityGenerator.GetBuildingsDestroyed()}";
    }

    void UpdateDroneDestroyedText()
    {
        if (killSheet != null && dronesDestroyedText != null)
            dronesDestroyedText.text = $"Drones Destroyed: \n{killSheet.dronesDestroyed}";
    }

    void UpdateTankDestroyedText()
    {
        if (killSheet != null && tanksDestroyedText != null)
            tanksDestroyedText.text = $"Tanks Destroyed: \n{killSheet.tanksDestroyed}";
    }

    void UpdateBomberDestroyedText()
    {
        if (killSheet != null && bombersDestroyedText != null)
            bombersDestroyedText.text = $"Bombers Destroyed: \n{killSheet.bombersDestroyed}";
    }

    void UpdateCurrentWave(int wave)
    {
        if (currentWaveText != null)
            currentWaveText.text = $"Current Wave: \n{wave}";
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