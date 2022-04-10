using UnityEngine;

public class AnimatedBounce : MonoBehaviour
{
    [SerializeField, Space(10)] bool isActive;
    [SerializeField] float frequency;
    [SerializeField] float magnitude;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 lookAtPosition;
    float cT;
    Vector3 pos;

    void FixedUpdate()
    {
        if (!isActive) return;
        cT = cT > Utilities.TAU ? 0 : cT += Time.fixedDeltaTime;
        pos = transform.localPosition;
        pos.y = magnitude * Mathf.Sin(cT * frequency);
        transform.localPosition = pos + offset;
        transform.forward = transform.position + (transform.position - lookAtPosition).normalized;
    }
}
