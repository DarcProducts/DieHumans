using UnityEngine;
using UnityEngine.Events;

public class EnterCollisionMod : MonoBehaviour
{
    public LayerMask detectedLayers;
    public UnityEvent Events;

    void OnCollisionEnter(Collision collision)
    {
        if (Utilities.IsInLayerMask(collision.gameObject, detectedLayers))
            Events.Invoke();
    }
}