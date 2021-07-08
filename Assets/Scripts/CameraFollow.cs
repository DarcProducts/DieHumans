using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject targetToFollow = null;
    [SerializeField] private Vector3 cameraOffset = Vector3.zero;
    [SerializeField] private float followSpeed = 1f;
    [SerializeField] private float rotationSpeed = 2f;
    [Range(0f, 1f)] [SerializeField] private float cameraHeightBias;
    [SerializeField] private float targetCameraDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float cameraHeightAdjustmentSpeed;

    private void Awake()
    {
        targetToFollow = GameObject.FindGameObjectWithTag("Player");
        if (targetToFollow == null)
            Debug.LogWarning($"Could not find a GameObject with Player tag");
    }

    private void LateUpdate()
    {
        float distanceFromGround = CheckDistanceWithRay(this.gameObject, Vector3.down, Color.blue, groundLayer);

        transform.position = Vector3.Lerp(transform.position, targetToFollow.transform.position - targetToFollow.transform.forward * -cameraOffset.z, followSpeed * Time.smoothDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetToFollow.transform.rotation, rotationSpeed * Time.smoothDeltaTime);

        if (distanceFromGround > targetCameraDistance + cameraHeightBias)
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down, cameraHeightAdjustmentSpeed * Time.smoothDeltaTime);
        if (distanceFromGround < targetCameraDistance - cameraHeightBias)
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up, cameraHeightAdjustmentSpeed * Time.smoothDeltaTime);
        if (distanceFromGround < 1)
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up, cameraHeightAdjustmentSpeed * 10 * Time.smoothDeltaTime);
    }

    public float CheckDistanceWithRay(GameObject rayObject, Vector3 rayDirection, Color color = default, LayerMask layerMask = default)
    {
        if (rayObject != null)
        {
            if (Physics.Raycast(rayObject.transform.position, rayObject.transform.TransformDirection(rayDirection), out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(rayObject.transform.position, rayObject.transform.TransformDirection(rayDirection) * hit.distance, color);
                return hit.distance;
            }
        }
        return 0;
    }
}