using System.Collections;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 1;
    [SerializeField] bool TurnOff;

    void OnEnable() => StartCoroutine(nameof(DelayedDestroy));

    void OnDisable() => StopCoroutine(nameof(DelayedDestroy));

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSecondsRealtime(timeToDestroy);
        if (!TurnOff)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}