using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float secondsToBeDestroyed;
    void Start() => Destroy(gameObject, secondsToBeDestroyed);
}
