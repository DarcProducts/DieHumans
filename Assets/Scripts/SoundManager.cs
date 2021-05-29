using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioClip laserShoot;
    [SerializeField] private AudioClip laserHitConcrete;
    [SerializeField] private AudioClip laserHitDirt;
    [SerializeField] private AudioClip laserHitMetal;

    public void PlayLaserShoot()
    {
        if (mainSource != null)
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

    public void PlayLaserHitConcrete()
    {
    }

    public void PlayLaserHitDirt()
    {
    }

    public void PlayLaserHitMetal()
    {
    }
}