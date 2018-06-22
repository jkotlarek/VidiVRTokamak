using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ParticleData : MonoBehaviour {

    public bool touched = false;

    void Start()
    {
        GetComponent<VRTK_InteractableObject>().InteractableObjectTouched += Touch;
    }

    void Touch(object sender, InteractableObjectEventArgs e)
    {
        touched = true;
    }
    
}
