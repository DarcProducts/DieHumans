using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    [SerializeField] private string tagName;
    [SerializeField] private float destroyTime;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagName))
            Destroy(this.gameObject, destroyTime);
    }
}
