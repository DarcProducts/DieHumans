using UnityEngine;
using UnityEngine.Events;

public class GameOverManager : MonoBehaviour
{
    public GlobalBoolVariable gameOver;
    public UnityEvent gameOverEvents;

    void OnEnable() => gameOver.OnValueChanged += GameOver;

    void OnDisable() => gameOver.OnValueChanged -= GameOver;

    void GameOver(bool value)
    {
        if (value.Equals(true))
            gameOverEvents?.Invoke();
    }
}
