using UnityEngine;
public class AudioFX : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    [SerializeField] Vector2 minMaxVolume;
    public Vector2 minMaxPitch;
    public void PlayFX() => Utilities.PlayAtSourceWithVPRange(source, clip, minMaxVolume.x, minMaxVolume.y, minMaxPitch.x, minMaxPitch.y);
}
