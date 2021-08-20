using UnityEngine;

public class TurnOffOnTouchAfterSeconds : MonoBehaviour
{
    [SerializeField] private string tagName;
    [SerializeField] private string otherTagName;
    [SerializeField] private float turnOffTime;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagName) || collision.gameObject.CompareTag(otherTagName))
            Invoke(nameof(TurnOffGameObject), turnOffTime);
    }

    private void TurnOffGameObject() => gameObject.SetActive(false);
}