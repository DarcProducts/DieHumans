using UnityEngine;
[CreateAssetMenu(menuName = "FX/New Audio FX")]
public class AudioFX : FX
{
    public AudioSource source;
    public AudioClip clip;
    public override void PlayFX(Vector3 location)
    {
        
    }
}
