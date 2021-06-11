using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnExit : MonoBehaviour
{
    private void OnTriggerExit(Collider other) => Destroy(gameObject);

    private void OnCollisionExit(Collision collision) => Destroy(gameObject);
}
