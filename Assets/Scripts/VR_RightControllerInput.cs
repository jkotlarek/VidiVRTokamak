using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_RightControllerInput : VR_ControllerInput {

    SteamVR_LaserPointer lp;
    Transform target;

    // Use this for initialization
    void Start () {
        base.Start();
        lp = GetComponent<SteamVR_LaserPointer>();
        lp.PointerIn += OnPointerIn;
        lp.PointerOut += OnPointerOut;
        controller.TriggerUnclicked += HighlightParticle;
        controller.PadClicked += DPadToggle;
        controller.Gripped += TogglePause;
	}

    void OnPointerIn(object sender, PointerEventArgs e)
    {
        Debug.Log("laser hit");
        if (e.target.gameObject.CompareTag("Particle"))
        {
            target = e.target;
        }
    }

    void HighlightParticle(object sender, ClickedEventArgs e)
    {
        Debug.Log("right trigger clicked");
        if (target != null)
        {
            particleScript.HighlightParticle(target.gameObject);
        }
    }

    void OnPointerOut(object sender, PointerEventArgs e)
    {
        target = null;
    }
}
