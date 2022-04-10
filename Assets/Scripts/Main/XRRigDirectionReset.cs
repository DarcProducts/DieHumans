using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRRigDirectionReset : MonoBehaviour
{
    [SerializeField] XRRig xRRig;
    [SerializeField] Transform forwardTarget;
    void Start() => ResetXRView();

    public void ResetXRView() => StartCoroutine(nameof(ResetView));

    // crap
    IEnumerator ResetView()
    {
        yield return new WaitForEndOfFrame();
        xRRig.MatchRigUpCameraForward(Vector3.up, forwardTarget.forward);
    }
}
