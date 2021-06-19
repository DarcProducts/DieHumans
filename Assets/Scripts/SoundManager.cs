using UnityEngine;

public enum MaterialType { concrete, glass, metal, dirt, all }

public class SoundManager : MonoBehaviour
{
    [SerializeField] private float maxDistanceToPlay;
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource laserHitSource;
    [SerializeField] private AudioClip laserShoot;
    [SerializeField] private AudioClip laserHitConcrete;
    [SerializeField] private AudioClip laserHitDirt;
    [SerializeField] private AudioClip laserHitMetal;
    [SerializeField] private AudioClip laserHitGlass;
    [SerializeField] private AudioClip laserHitAll;
    [SerializeField] private AudioClip explosionSound;
    private GameObject player;

    private void OnEnable()
    {
        Rocket.ExplodeRocket += PlayExplosionSound;
    }

    private void OnDisable()
    {
        Rocket.ExplodeRocket -= PlayExplosionSound;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void PlayLaserShoot()
    {
        if (mainSource != null && laserShoot != null)
        {
            if (!mainSource.isPlaying)
            {
                mainSource.clip = laserShoot;
                mainSource.Play();
            }
        }
    }

    public void StopLaserShoot()
    {
        if (mainSource != null)
            if (mainSource.isPlaying)
                mainSource.Stop();
    }

    public void PlayLaserHit(GameObject targetHit, MaterialType type)
    {
        if (laserHitSource != null)
        {
            switch (type)
            {
                case MaterialType.dirt:
                    if (laserHitDirt != null && laserHitSource.clip != laserHitDirt)
                        PlayAtSourceWithVPRange(laserHitSource, laserHitDirt, .2f, .6f, .5f, 1.5f);
                    break;

                case MaterialType.concrete:
                    if (laserHitConcrete != null && laserHitSource.clip != laserHitConcrete)
                        PlayAtSourceWithVPRange(laserHitSource, laserHitConcrete, .2f, .6f, .5f, 1.5f);
                    break;

                case MaterialType.metal:
                    if (laserHitMetal != null && laserHitSource.clip != laserHitMetal)
                        PlayAtSourceWithVPRange(laserHitSource, laserHitMetal, .2f, .6f, .5f, 1.5f);
                    break;

                case MaterialType.glass:
                    if (laserHitGlass != null && laserHitSource.clip != laserHitGlass)
                        PlayAtSourceWithVPRange(laserHitSource, laserHitGlass, .2f, .6f, .5f, 1.5f);
                    break;

                case MaterialType.all:
                    if (laserHitAll != null && laserHitSource.clip != laserHitAll)
                        PlayAtSourceWithVPRange(laserHitSource, laserHitAll, .2f, .6f, .5f, 1.5f);
                    break;

                default:
                    break;
            }
        }
    }

    private float GetVolumeBasedOnDistance(Vector3 loc)
    {
        if (player != null)
        {
            float distance = Vector3.Distance(loc, player.transform.position);
            if (distance < maxDistanceToPlay)
                return Mathf.InverseLerp(distance, 0, 1);
        }
        return 0;
    }

    public void PlayExplosionSound(Vector3 loc)
    {
        if (mainSource != null && explosionSound != null)
        {
            float vol = GetVolumeBasedOnDistance(loc);
            if (vol != 0)
            {
                mainSource.volume = vol;
                mainSource.PlayOneShot(explosionSound);
            }
        }
    }

    public static void PlayAtSourceWithVPRange(AudioSource source, AudioClip clip, float minVol = 0, float maxVol = 1, float minPitch = -3, float maxPitch = 3)
    {
        minVol = minVol < 0 ? 0 : minVol > 1 ? 1 : minVol;
        maxVol = maxVol < minVol ? minVol : maxVol > 1 ? 1 : maxVol;
        minPitch = minPitch < -3 ? -3 : minPitch > 3 ? 3 : minPitch;
        maxPitch = maxPitch < minPitch ? minPitch : maxPitch > 3 ? 3 : maxPitch;
        source.volume = Random.Range(minVol, maxVol);
        source.pitch = Random.Range(minPitch, maxPitch);
        source.PlayOneShot(clip);
    }

    public AudioSource GetLaserHitSource() => laserHitSource;
}