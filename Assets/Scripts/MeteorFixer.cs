using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorFixer : MonoBehaviour
{
    private GameObject[] children;
    private Vector3[] childrenPostions;
    private Quaternion[] childrenRotations;
    private Vector3[] childrenScales;

    private void Start()
    {
        children = new GameObject[transform.childCount];
        childrenPostions = new Vector3[transform.childCount];
        childrenRotations = new Quaternion[transform.childCount];
        childrenScales = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            childrenPostions[i] = transform.GetChild(i).transform.localPosition;
            childrenRotations[i] = transform.GetChild(i).transform.localRotation;
            childrenScales[i] = transform.GetChild(i).transform.localScale;
            Debug.Log($"Added transform to array: {childrenPostions[i]} {childrenRotations[i]} {childrenScales[i]}");
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).transform.localPosition = childrenPostions[i];
            transform.GetChild(i).transform.localRotation = childrenRotations[i];
            transform.GetChild(i).transform.localScale = childrenScales[i];
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (AllChildrenDisabled())
            gameObject.SetActive(false);
    }

    private bool AllChildrenDisabled()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].activeSelf)
                return false;
        }
        return true;
    }
}
