using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource mainSource;
    [SerializeField] AudioSource weaponSource;
    [SerializeField] AudioSource droneSource;
    [SerializeField] AudioSource tankSource;
    [SerializeField] AudioSource bomberSource;
    [SerializeField] AudioSource fxSource;
    [SerializeField] AudioClip[] meteorDestroyed;
    [SerializeField] AudioClip laserShoot;
    [SerializeField] AudioClip[] explosions;
    [SerializeField] AudioClip machinegun;
    [SerializeField] AudioClip[] backgroundMusic;
    [SerializeField] AudioClip buildingCollapse;
    [SerializeField] AudioClip bombDrop;
    [SerializeField] AudioClip pickupDropBox;
    [SerializeField] AudioClip wrongDestroy;
    [SerializeField] AudioClip scorePoints;
    [SerializeField] AudioClip decreasedPoints;
    [SerializeField] AudioClip tankShotSound;

    void Start()
    {
        StartCoroutine(PlayBackgroundMusic());
    }

    void OnEnable()
    {
        WeaponManager.WeaponFired += PlayMachinegunClip;
        Meteor.MeteorEvaded += PlayMeteorDestroyed;
        MessagesManager.GainPoints += PlayScoredSound;
        MessagesManager.DecreasePoints += PlayDecreasedPointSound;
        Drone.FiredLaser += PlayLaserSound;
        Building.BuildingCollapseSound += PlayBuildingCollapse;
    }

    void OnDisable()
    {
        WeaponManager.WeaponFired -= PlayMachinegunClip;
        Meteor.MeteorEvaded -= PlayMeteorDestroyed;
        MessagesManager.GainPoints -= PlayScoredSound;
        MessagesManager.DecreasePoints -= PlayDecreasedPointSound;
        Drone.FiredLaser -= PlayLaserSound;
        Building.BuildingCollapseSound -= PlayBuildingCollapse;
    }

    public void StopWeaponSounds()
    {
        if (weaponSource != null)
        {
            weaponSource.Stop();
            weaponSource.volume = 1;
        }
    }

    public void PlayLaserShotFromSource(AudioSource source)
    {
        if (laserShoot != null && !source.isPlaying)
            Utilities.PlayAtSourceWithVPRange(source, laserShoot, .2f, .4f, .8f, 1.2f);
    }

    public IEnumerator PlayBackgroundMusic()
    {
        if (musicSource != null)
        {
            int ranIndex = Random.Range(0, backgroundMusic.Length);
            if (backgroundMusic[ranIndex] != null)
            { 
                musicSource.clip = backgroundMusic[ranIndex];
                musicSource.Play();
                yield return new WaitForSeconds(backgroundMusic[ranIndex].length);
                StartCoroutine(PlayBackgroundMusic());
            }
        }
    }

    public void PlayLaserSound()
    {
        if (droneSource != null && laserShoot != null)
            Utilities.PlayAtSourceWithVPRange(droneSource, laserShoot, .1f, .2f, .8f, 1.2f);
    }

    public void PlayBombDrop()
    {
        if (bomberSource != null && bombDrop != null)
            Utilities.PlayAtSourceWithVPRange(bomberSource, bombDrop, .6f, .88f, .8f, 1.2f);
    }

    public void PlayScoredSound()
    {
        if (mainSource != null && scorePoints != null)
            Utilities.PlayAtSourceWithVPRange(mainSource, scorePoints, 1f, 1f, 1f, 1f);
    }

    public void PlayTankShotSound()
    {
        if (tankSource != null && tankShotSound != null)
            Utilities.PlayAtSourceWithVPRange(tankSource, tankShotSound, .6f, .8f, .8f, 1.2f);
    }

    public void PlayDecreasedPointSound()
    {
        if (mainSource != null && decreasedPoints != null)
            Utilities.PlayAtSourceWithVPRange(mainSource, decreasedPoints, 1f, 1f, 1f, 1f);
    }

    public void PlayPickUpDropBox()
    {
        if (mainSource != null && pickupDropBox != null)
            Utilities.PlayAtSourceWithVPRange(mainSource, pickupDropBox, .6f, .88f, .8f, 1.2f);
    }

    public void PlayWrongDestroy()
    {
        if (fxSource != null && wrongDestroy != null)
            Utilities.PlayAtSourceWithVPRange(fxSource, wrongDestroy, .8f, 1f, .8f, 1f);
    }

    public void PlayMachinegunClip()
    {
        if (weaponSource != null && machinegun != null)
            Utilities.PlayAtSourceWithVPRange(weaponSource, machinegun, .6f, .88f, .8f, 1.2f);
    }

    public void PlayBuildingCollapse()
    {
        if (mainSource != null && buildingCollapse != null)
        {
            Utilities.PlayAtSourceWithVPRange(mainSource, buildingCollapse, .8f, 1f, .8f, 1f);
        }
    }

    public void PlayMeteorDestroyed(GameObject obj)
    {
        if (fxSource != null && meteorDestroyed.Length > 0)
        {
            int ranNum = Random.Range(0, meteorDestroyed.Length);
            if (meteorDestroyed[ranNum] != null)
                Utilities.PlayAtSourceWithVPRange(fxSource, meteorDestroyed[ranNum], .6f, 1f, .8f, 1.2f);
        }
    }

    public void PlayExplosionSound(Vector3 loc)
    {
        int ranNum = Random.Range(0, explosions.Length);
        if (fxSource != null && explosions[ranNum] != null)
            Utilities.PlayAtSourceWithVPRange(fxSource, explosions[ranNum], .8f, 1f, .2f, 1.2f);
    }
}