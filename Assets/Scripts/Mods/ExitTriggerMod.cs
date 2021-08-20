using UnityEngine;
public class ExitTriggerMod : MonoBehaviour
{
    public LayerMask detectedLayers;
    public Mod[] mods = new Mod[0];
    public FXInitializer effects;
    public GameEvent[] Events = new GameEvent[0];
    void OnTriggerExit(Collider other) 
    {
        if (IsInLayerMask(other.gameObject, detectedLayers))
        {
            for (int i = 0; i < mods.Length; i++) if (mods[i] != null)
                    mods[i].ChangeValue();
            if (effects != null)
            effects.PlayAllFX(transform.position);
            for (int i = 0; i < Events.Length; i++) if (Events[i] != null)
                    Events[i].Raise();
        }
    }
    bool IsInLayerMask(GameObject obj, LayerMask layerMask) => (layerMask.value & (1 << obj.layer)) > 0;
}
