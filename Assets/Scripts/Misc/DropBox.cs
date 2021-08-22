using UnityEngine;

public class DropBox : MonoBehaviour
{
    [SerializeField] ObjectPooler infoLettersPool;
    [SerializeField] Drop[] dropData = new Drop[0];
    [SerializeField] FXInitializer shotBoxFX;
    [SerializeField] FXInitializer hitObjectFX;
    [SerializeField] GameObject parachute;
    [SerializeField] bool willExplodeWhenShot;
    [SerializeField] float explRad;
    [SerializeField] float explDmg;
    [SerializeField] LayerMask explHitLyr;
    [SerializeField] GameObject locator;
    [SerializeField] LayerMask locHitLyr;
    [SerializeField] float pickUpDisplayDuration;
    Rigidbody rB;
    bool dragSet = false;
    bool hasPara = true;
    int ranIndex;
    Renderer boxRend;

    void Awake()
    {
        boxRend = GetComponent<Renderer>();
        rB = GetComponent<Rigidbody>();
    }

    void Start()
    {
        ResetParachute();
        SetData();
    }

    void OnEnable()
    {
        ResetParachute();
        SetData();
        ranIndex = Random.Range(0, dropData.Length);
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

    public void Activate()
    {
        if (dropData[ranIndex].pickupMods != null)
            dropData[ranIndex].pickupMods.ActivateMods();
        if (dropData[ranIndex].pickupFX != null)
            dropData[ranIndex].pickupFX.PlayAllFX(transform.position);
        dropData[ranIndex].DisplayPickupText(pickUpDisplayDuration, infoLettersPool.GetObject());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Activate();
            gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            if (shotBoxFX != null)
                shotBoxFX.PlayAllFX(transform.position);
            if (willExplodeWhenShot)
                Utilities.TryDamagingNearTargets(transform.position + Vector3.down * 2, explRad, explHitLyr, explDmg);
            gameObject.SetActive(false);
        }
        else
        {
            if (hitObjectFX != null)
                hitObjectFX.PlayAllFX(transform.position);
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

    public void SetData()
    {
        if (boxRend != null)
            boxRend.material = dropData[ranIndex].material;
    }
}