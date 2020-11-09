using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    private Transform defaultParent;

    private void Awake()
    {
        gameObject.SetActive(false);

        defaultParent = transform.parent;
    }

    public void Show(Transform parent = null)
    {
        gameObject.SetActive(true);

        if (parent != null)
        {
            transform.parent = parent;

            transform.localPosition = Vector3.zero;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        transform.parent = defaultParent;
    }
}
