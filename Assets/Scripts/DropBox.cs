using UnityEngine;
using UnityEngine.Events;

public enum BoxType { Time, Damage, Speed }

public class DropBox : MonoBehaviour
{
    public static UnityAction PlayerAquired;
    public static UnityAction<Vector3, float> HitOtherObject;
    public static UnityAction<byte, float> BoxAction;
    [SerializeField] BoxType boxType;
    [SerializeField] Material damageMaterial;
    [SerializeField] Material speedMaterial;
    [SerializeField] Material timeMaterial;
    [SerializeField] float dropSpeed;
    [SerializeField] int speedMultiplier;
    [SerializeField] GameObject parachute;
    [SerializeField] float damageIncrease;
    [SerializeField] float speedIncrease;
    [SerializeField] float rateDecrease;
    WeaponManager weaponManager;
    bool hasParachute = true;
    Renderer boxRenderer;

    private void Awake()
    {
        weaponManager = FindObjectOfType<WeaponManager>();
        if (weaponManager == null)
            Debug.LogWarning($"Cannot find a GameObject with the WeaponManager component attached");
    }

    void Start()
    {
        boxRenderer = GetComponent<Renderer>();
        PickBox();
    }

    private void FixedUpdate()
    {
        if (hasParachute)
            transform.Translate(dropSpeed * Time.fixedDeltaTime * Vector3.down.normalized);
        else
            transform.Translate(dropSpeed * speedMultiplier * Time.fixedDeltaTime * Vector3.down.normalized);

        if (transform.position.y < -10)
            gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (boxRenderer != null)
        {
            int ranNum = Random.Range(0, 3);
            if (ranNum == 0)
            {
                boxType = BoxType.Speed;
                boxRenderer.material = speedMaterial;
            }
            if (ranNum == 1)
            {
                boxType = BoxType.Damage;
                boxRenderer.material = damageMaterial;
            }
            else if (ranNum == 2)
            {
                boxType = BoxType.Time;
                boxRenderer.material = timeMaterial;
            }
        }
    }

    void PickBox()
    {
        int ranNum = Random.Range(0, 3);
        if (ranNum == 0)
        {
            boxType = BoxType.Time;
            boxRenderer.material = timeMaterial;
        }
        else if (ranNum == 1)
        {
            boxType = BoxType.Damage;
            boxRenderer.material = damageMaterial;
        }
        else
        {
            boxType = BoxType.Speed;
            boxRenderer.material = speedMaterial;
        }
    }

    void OnDisable()
    {
        transform.position = Vector3.down;
    }

    public void ActivateBox()
    {
        if (weaponManager != null)
        {
            switch (boxType)
            {
                case BoxType.Damage:
                    BoxAction?.Invoke(0, damageIncrease);
                    break;

                case BoxType.Speed:
                    BoxAction?.Invoke(1, speedIncrease);
                    break;

                case BoxType.Time:
                    BoxAction?.Invoke(2, rateDecrease);
                    break;

                default:
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerAquired?.Invoke();
            ActivateBox();
        }
        else
            HitOtherObject?.Invoke(transform.position, 3);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
            DestroyParachute();
    }

    [ContextMenu("Destroy Parachute")]
    public void DestroyParachute()
    {
        if (parachute != null)
        {
            hasParachute = false;
            parachute.SetActive(false);
        }
    }
}