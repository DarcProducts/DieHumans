using UnityEngine;

public class TurnOffOnTouchAfterSeconds : MonoBehaviour
{
    [SerializeField] private string tagName;
    [SerializeField] private float turnOffTime;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagName))
            Invoke(nameof(TurnOffGameObject), turnOffTime);
    }

    private void TurnOffGameObject() => gameObject.SetActive(false);
}