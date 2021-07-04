using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource weaponSource;
    [SerializeField] private AudioClip meteorDestroyed;
    [SerializeField] private AudioClip laserShoot;
    [SerializeField] private AudioClip laserHit;
    [SerializeField] private AudioClip[] explosions;
    [SerializeField] private AudioClip machinegun;
    [SerializeField] private AudioClip[] backgroundMusic;

    private void OnEnable()
    {
        ObjectPools.ExplosionSound += PlayExplosionSound;
        WeaponManager.MachinegunFired += PlayMachinegunClip;
        GameManager.FireWeaponButtonUp += StopWeaponSounds;
        Meteor.MeteorEvaded += PlayMeteorDestroyed;
    }

    private void OnDisable()
    {
        ObjectPools.ExplosionSound -= PlayExplosionSound;
        WeaponManager.MachinegunFired -= PlayMachinegunClip;
        GameManager.FireWeaponButtonUp -= StopWeaponSounds;
        Meteor.MeteorEvaded -= PlayMeteorDestroyed;
    }

    public void PlayLaserShot()
    {
        if (mainSource != null && laserShoot != null)
        {
            
        }
    }

    public void StopWeaponSounds()
    {
        if (weaponSource != null)
        {
            weaponSource.Stop();
            weaponSource.volume = 1;
        }
    }

    public void PlayMachinegunClip()
    {
        if (weaponSource != null && machinegun != null)
                PlayAtSourceWithVPRange(weaponSource, machinegun, .8f, 1f, .8f, 1.2f);
    }

    public void PlayMachinegunHit()
    {
        
    }

    public void PlayLaserHit(GameObject targetHit)
    {
        
    }

    public void PlayMeteorDestroyed(GameObject obj)
    {
        if (mainSource != null && meteorDestroyed != null)
            PlayAtSourceWithVPRange(mainSource, meteorDestroyed, .6f, 1f, .8f, 1.2f);
    }

    public void PlayExplosionSound(Vector3 loc)
    {
        int ranNum = Random.Range(0, explosions.Length);
        if (mainSource != null && explosions[ranNum] != null)
            PlayAtSourceWithVPRange(mainSource, explosions[ranNum], .8f, 1f, .2f, 1.2f);
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
}