using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GamePauser : MonoBehaviour
{
    public GlobalBoolVariable buttonPressed;
    public GlobalBoolVariable gamePaused;
    public UnityEvent GamePausedExtra;
    public UnityEvent GameContinuedExtra;

    void OnEnable() => buttonPressed.OnValueChanged += PauseGame;

    void OnDisable() => buttonPressed.OnValueChanged -= PauseGame;

    public void PauseGame(bool value)
    {
        if (value.Equals(true))
        {
            if (Time.timeScale > 0)
            {
                GamePausedExtra?.Invoke();
                gamePaused.Value = true;
                Time.timeScale = 0;
            }
            else
            {
                GameContinuedExtra?.Invoke();
                gamePaused.Value = false;
                Time.timeScale = 1;
            }
        }
    }
}
