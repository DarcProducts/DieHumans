using UnityEngine;

public class TurnOffAfterTime : MonoBehaviour
{
    [SerializeField] float timeToTurnOff;

    void OnEnable() => Invoke(nameof(TurnOffObject), timeToTurnOff);
    void OnDisable()
    {
        this.enabled = true;
        CancelInvoke(nameof(TurnOffObject));
    }

    void TurnOffObject() => gameObject.SetActive(false);

}