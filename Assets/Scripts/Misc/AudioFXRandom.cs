using UnityEngine;

public class AudioFXRandom : MonoBehaviour
{
    [SerializeField] AudioSource audioSouce;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] Vector2 minMaxVolume;
    [SerializeField] Vector2 minMaxPitch;
    [Tooltip("Max distance to hear sound if calling PlayRandomFXDistance")]
    [SerializeField] float maxDistance = 20f;
    [SerializeField] bool stopSourceIfPlaying;
    [SerializeField] bool playOneShot;

    public void PlayRandomFX()
    {
        SetRandomVolume(minMaxVolume);
        SetRandomPitch(minMaxPitch);
        InitializeFX(audioClips[Random.Range(0, audioClips.Length)]);
    }

    public void PlayFXAtIndex(int index)
    {
        if (index < audioClips.Length)
            InitializeFX(audioClips[index]);
    }

    public void PlayRandomFXDistance(GameObject obj)
    {
        float newVolume = Utilities.Remap(Vector3.Distance(obj.transform.position, Vector3.zero), 0.01f, maxDistance, 1, 0);
        SetRandomVolume(new Vector2(newVolume, newVolume));
        SetRandomPitch(minMaxPitch);
        InitializeFX(audioClips[Random.Range(0, audioClips.Length)]);
    }

    void SetRandomVolume(Vector2 minMax) => audioSouce.volume =
        Mathf.Clamp01(Random.Range(Mathf.Min(minMax.x, minMax.y), Mathf.Max(minMax.x, minMax.y)));

    void SetRandomPitch(Vector2 minMax) => audioSouce.pitch =
        audioSouce.pitch = Mathf.Clamp(Random.Range(Mathf.Min(minMax.x, minMax.y), Mathf.Max(minMax.x, minMax.y)), -3, 3);

    void InitializeFX(AudioClip clip)
    {
        if (audioSouce == null || audioClips.Length == 0) return;
        audioSouce.clip = clip;
        if (!playOneShot)
        {
            if (stopSourceIfPlaying)
            {
                audioSouce.Stop();
                audioSouce.Play();
            }
            else
            {
                if (!audioSouce.isPlaying)
                    audioSouce.Play();
            }
        }
        else
            audioSouce.PlayOneShot(clip);
    }

    public void SetAudioSource(AudioSource newSource) => audioSouce = newSource;
}