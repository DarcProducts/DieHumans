using UnityEngine;
using UnityEngine.Events;

public class EnterTriggerMod : MonoBehaviour
{
    public LayerMask detectedLayers;
    public UnityEvent Events;

    void OnTriggerEnter(Collider other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, detectedLayers))
            Events.Invoke();
    }
}