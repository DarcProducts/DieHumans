using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private CityGenerator cityGenerator;
    [SerializeField] private TMP_Text buildingsLeftText;
    [SerializeField] private TMP_Text currentWaveText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int currentPoints = 0;

    private void OnEnable()
    {
        Building.BreakBuilding += UpdateBuildingsLeft;
        WaveManager.UpdateWave += UpdateCurrentWave;
        MessagesManager.UpdateScore += UpdateScore;
    }

    private void OnDisable()
    {
        Building.BreakBuilding -= UpdateBuildingsLeft;
        WaveManager.UpdateWave -= UpdateCurrentWave;
        MessagesManager.UpdateScore -= UpdateScore;
    }

    private void Start()
    {
        if (cityGenerator != null && buildingsLeftText != null)
            buildingsLeftText.text = $"Buildings Left: \n{cityGenerator.numberOfBuildings}";
        if (scoreText != null)
            scoreText.text = "Score: \n0";
    }

    private void UpdateBuildingsLeft(GameObject notUsed)
    {
        if (cityGenerator != null && buildingsLeftText != null)
            buildingsLeftText.text = $"Buildings Left: \n{cityGenerator.numberOfBuildings}";
    }

    private void UpdateCurrentWave(int wave)
    {
        if (currentWaveText != null)
            currentWaveText.text = $"Current Wave: \n{wave}";
    }

    private void UpdateScore(int points, bool positive)
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