using System.Linq;
using UnityEngine;

public class BrokenObjectFixer : MonoBehaviour
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
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (childrenPostions != null)
                transform.GetChild(i).transform.localPosition = childrenPostions[i];
            if (childrenRotations != null)
                transform.GetChild(i).transform.localRotation = childrenRotations[i];
            if (childrenScales != null)
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

    public GameObject[] GetChildren() => children.ToArray();
}