using UnityEngine;

public class TurnOffAfterTime : MonoBehaviour
{
    [SerializeField] private float timeToTurnOff;

    private void OnEnable() => Invoke(nameof(TurnOffObject), timeToTurnOff);
    private void OnDisable() => CancelInvoke(nameof(TurnOffObject));

    private void TurnOffObject() => gameObject.SetActive(false);

}