using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VR_UIItem : MonoBehaviour {

    BoxCollider boxCollider;
    RectTransform rectTransform;

    private void OnEnable()
    {
        ValidateCollider();
    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision: " + collision.gameObject.name);
    }

    private void ValidateCollider()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider>();
        if(boxCollider==null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        boxCollider.size = rectTransform.sizeDelta;
    }
}
