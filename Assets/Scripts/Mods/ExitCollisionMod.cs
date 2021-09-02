using UnityEngine;
using UnityEngine.Events;

public class ExitCollisionMod : MonoBehaviour
{
    public LayerMask detectedLayers;
    public UnityEvent Events;

    void OnCollisionExit(Collision collision)
    {
        if (Utilities.IsInLayerMask(collision.gameObject, detectedLayers))
            Events.Invoke();
    }
}