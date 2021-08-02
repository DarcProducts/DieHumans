using UnityEngine;
using UnityEngine.Events;

public enum BoxType { Time, Damage, Speed, Thrust }

public class DropBox : MonoBehaviour
{
    public static UnityAction PlayerAquired;
    public static UnityAction ShotBox;
    public static UnityAction<Vector3, float, byte> HitObject;
    public static UnityAction<byte, float> BoxAction;
    [SerializeField] BoxType boxType;
    [SerializeField] Material dmgMat;
    [SerializeField] Material spdMat;
    [SerializeField] Material timeMat;
    [SerializeField] Material thrustMat;
    [SerializeField] GameObject parachute;
    [SerializeField] float dmgIncr;
    [SerializeField] float spdIncr;
    [SerializeField] float rateDecr;
    [SerializeField] float thrustIncr;
    [SerializeField] float explRad;
    [SerializeField] float explDmg;
    [SerializeField] LayerMask explHitLyr;
    [SerializeField] GameObject locator;
    [SerializeField] LayerMask locHitLyr;
    Rigidbody rB;
    WeaponManager wpnMng;
    bool dragSet = false;
    bool hasPara = true;
    Renderer boxRend;

    void Awake()
    {
        boxRend = GetComponent<Renderer>();
        rB = GetComponent<Rigidbody>();
        wpnMng = FindObjectOfType<WeaponManager>();
    }

    void Start()
    {
        PickBox();
        ResetParachute();
    }

    void OnEnable()
    {
        PickBox();
        ResetParachute();
    }

    void SetUpDropLocator()
    {
        if (locator != null)
        {
            if (Physics.Raycast(transform.position + Vector3.down * 3, Vector3.down, out RaycastHit hit, 800, locHitLyr))
            {
                locator.transform.position = hit.point;
                locator.SetActive(true);
            }
            else
                locator.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (locator != null)
        {
            locator.transform.position = transform.position + Vector3.down * 6;
            locator.SetActive(false);
        }
        hasPara = true;
        if (parachute != null)
            parachute.SetActive(true);
    }

    void FixedUpdate()
    {
        if (!hasPara && !dragSet)
            ShotParachute();

        if (transform.position.y < -10)
            gameObject.SetActive(false);

        SetUpDropLocator();

        transform.rotation = Quaternion.identity;
    }

    void ShotParachute()
    {
        if (rB != null)
        {
            rB.drag = .1f;
            rB.angularDrag = .05f;
            dragSet = true;
        }
    }

    void ResetParachute()
    {
        if (rB != null)
        {
            rB.drag = 2f;
            rB.angularDrag = .5f;
            dragSet = false;
        }
    }

    void PickBox()
    {
        if (boxRend != null)
        {
            int ranNum = Random.Range(0, 4);
            if (ranNum == 0)
            {
                boxType = BoxType.Time;
                boxRend.material = timeMat;
            }
            else if (ranNum == 1)
            {
                boxType = BoxType.Damage;
                boxRend.material = dmgMat;
            }
            else if (ranNum == 2)
            {
                boxType = BoxType.Speed;
                boxRend.material = spdMat;
            }
            else
            {
                boxType = BoxType.Thrust;
                boxRend.material = thrustMat;
            }
        }
    }

    public void ActivateBox()
    {
        if (wpnMng != null)
        {
            switch (boxType)
            {
                case BoxType.Damage:
                    BoxAction?.Invoke(0, dmgIncr);
                    break;

                case BoxType.Speed:
                    BoxAction?.Invoke(1, spdIncr);
                    break;

                case BoxType.Time:
                    BoxAction?.Invoke(2, rateDecr);
                    break;

                case BoxType.Thrust:
                    BoxAction?.Invoke(3, thrustIncr);
                    break;

                default:
                    break;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerAquired?.Invoke();
            ActivateBox();
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            ShotBox?.Invoke();
            Utilities.TryDamagingNearTargets(transform.position + Vector3.down * 2, explRad, explHitLyr, explDmg);
            HitObject?.Invoke(transform.position, 10, 1);
            gameObject.SetActive(false);
        }
        else
        {
            HitObject?.Invoke(transform.position, 3, 0);
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
            DestroyParachute();
    }

    [ContextMenu("Destroy Parachute")]
    public void DestroyParachute()
    {
        if (parachute != null)
        {
            hasPara = false;
            parachute.SetActive(false);
        }
    }
}