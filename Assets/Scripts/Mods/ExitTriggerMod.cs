using UnityEngine;
using UnityEngine.Events;

public class ExitTriggerMod : MonoBehaviour
{
    public LayerMask detectedLayers;
    public UnityEvent Events;

    void OnTriggerExit(Collider other)
    {
        if (Utilities.IsInLayerMask(other.gameObject, detectedLayers))
            Events.Invoke();
    }
}