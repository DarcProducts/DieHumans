using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource mainSource;
    [SerializeField] AudioSource weaponSource;
    [SerializeField] AudioClip[] meteorDestroyed;
    [SerializeField] AudioClip laserShoot;
    [SerializeField] AudioClip[] explosions;
    [SerializeField] AudioClip machinegun;
    [SerializeField] AudioClip[] backgroundMusic;
    [SerializeField] AudioClip buildingCollapse;
    [SerializeField] AudioClip bombDrop;
    [SerializeField] AudioClip pickupDropBox;
    [SerializeField] AudioClip wrongDestroy;

    void Start()
    {
        PlayBackgroundMusic();
    }

    void OnEnable()
    {
        ObjectPools.ExplosionSound += PlayExplosionSound;
        WeaponManager.WeaponFired += PlayMachinegunClip;
        GameManager.FireWeaponButtonUp += StopWeaponSounds;
        Meteor.MeteorEvaded += PlayMeteorDestroyed;
        AIUtilities.LaserActivated += PlayLaserShot;
        Bomber.BombDropped += PlayBombDrop;
        DropBox.PlayerAquired += PlayPickUpDropBox;
        DropBox.ShotBox += PlayWrongDestroy;
    }

    void OnDisable()
    {
        ObjectPools.ExplosionSound -= PlayExplosionSound;
        WeaponManager.WeaponFired -= PlayMachinegunClip;
        GameManager.FireWeaponButtonUp -= StopWeaponSounds;
        Meteor.MeteorEvaded -= PlayMeteorDestroyed;
        AIUtilities.LaserActivated -= PlayLaserShot;
        Bomber.BombDropped -= PlayBombDrop;
        DropBox.PlayerAquired -= PlayPickUpDropBox;
        DropBox.ShotBox -= PlayWrongDestroy;
    }

    public void StopWeaponSounds()
    {
        if (weaponSource != null)
        {
            weaponSource.Stop();
            weaponSource.volume = 1;
        }
    }

    public void PlayLaserShot(AudioSource source)
    {
        if (laserShoot != null && !source.isPlaying)
            PlayAtSourceWithVPRange(source, laserShoot, .2f, .4f, .8f, 1.2f);
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null)
        {
            int ranIndex = Random.Range(0, backgroundMusic.Length);
            if (backgroundMusic[ranIndex] != null)
            { 
                musicSource.clip = backgroundMusic[ranIndex];
                musicSource.Play();
            }
        }
    }

    public void PlayBombDrop()
    {
        if (mainSource != null && bombDrop != null)
            PlayAtSourceWithVPRange(mainSource, bombDrop, .6f, .88f, .8f, 1.2f);
    }

    public void PlayPickUpDropBox()
    {
        if (mainSource != null && pickupDropBox != null)
            PlayAtSourceWithVPRange(mainSource, pickupDropBox, .6f, .88f, .8f, 1.2f);
    }

    public void PlayWrongDestroy()
    {
        if (mainSource != null && wrongDestroy != null)
            PlayAtSourceWithVPRange(mainSource, wrongDestroy, .8f, 1f, .8f, 1f);
    }

    public void PlayMachinegunClip()
    {
        if (weaponSource != null && machinegun != null)
                PlayAtSourceWithVPRange(weaponSource, machinegun, .6f, .88f, .8f, 1.2f);
    }

    public void PlayBuildingCollapse()
    {
        if (mainSource != null && buildingCollapse != null)
            PlayAtSourceWithVPRange(mainSource, buildingCollapse, .6f, 1f, .2f, .8f);
    }

    public void PlayMeteorDestroyed(GameObject obj)
    {
        if (mainSource != null && meteorDestroyed.Length > 0)
        {
            int ranNum = Random.Range(0, meteorDestroyed.Length);
            if (meteorDestroyed[ranNum] != null)
                PlayAtSourceWithVPRange(mainSource, meteorDestroyed[ranNum], .6f, 1f, .8f, 1.2f);
        }
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