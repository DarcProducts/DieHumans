using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static UnityAction FireWeaponButtonHold;
    public static UnityAction FireWeaponButtonUp;
    public static UnityAction FireWeaponButtonDown;
    [Header("Managers")]
    [SerializeField] PlayerManager playerManager;
    [SerializeField] CityGenerator cityGenerator;
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] SoundManager soundManager;
    [SerializeField] Vector3 cityStartLoc = Vector3.zero;

    void Awake()
    {
        if (cityGenerator != null)
        {
            cityGenerator.transform.position = cityStartLoc;
            cityGenerator.CreateDarcGrid(cityGenerator.transform.position);
        }
    }

    void Update()
    {
        if (weaponManager != null)
            CheckWeapons();
        if (playerManager != null)
            CheckController();
    }

    void CheckWeapons()
    {
        
    }

    void CheckController()
    {
        if (playerManager != null)
        {
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > .1f)
                playerManager.TransformShip(Vector3.forward, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y);
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < -.1f)
                playerManager.TransformShip(Vector3.forward, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y);

            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > .1f)
                playerManager.RotateShip(Vector3.up, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x);
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -.1f)
                playerManager.RotateShip(Vector3.down, -OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x);
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                playerManager.TransformShip(Vector3.up, 1);
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
                playerManager.TransformShip(Vector3.down, 1);
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
            FireWeaponButtonDown?.Invoke();
        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
            FireWeaponButtonUp?.Invoke();
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            FireWeaponButtonHold?.Invoke();
    }
}