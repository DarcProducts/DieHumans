using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayFX : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip clip;
    [SerializeField] Vector2 minMaxVol;
    [SerializeField] Vector2 minMaxPitch;
    [SerializeField] UnityEvent EffectActivated;

    public void ActivateFX()
    {
        if (source != null && clip != null)
            Utilities.PlayAtSourceWithVPRange(source, clip, minMaxVol.x, minMaxVol.y, minMaxPitch.x, minMaxPitch.y);
        EffectActivated?.Invoke();
    }
}
