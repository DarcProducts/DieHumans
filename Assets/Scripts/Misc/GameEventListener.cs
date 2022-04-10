using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] GameEvent Event;
    [SerializeField] UnityEvent<GameObject> Response;

    void OnEnable() => Event.RegisterListener(this);
    void OnDisable() => Event.UnregisterListener(this);
    public void OnEventRaised(GameObject obj) => Response?.Invoke(obj);
}
