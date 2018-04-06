using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_LeftControllerInput : VR_ControllerInput {

    VR_ColliderTeleporter teleportScript;
    float incrementTPDistance = 0;

    // Use this for initialization
    void Start()
    {
        base.Start();
        teleportScript = GetComponent<VR_ColliderTeleporter>();
        controller.PadClicked += ChangeTPDistance;
        controller.Gripped += TogglePause;
        //controller.PadClicked += ClearHighlight;
    }

    private void Update()
    {
        if (controller.padPressed)
        {
            teleportScript.maxDist = Mathf.Clamp(teleportScript.maxDist + incrementTPDistance, 1f, 10f);
        }
    }

    void ChangeTPDistance(object sender, ClickedEventArgs e)
    {
        if (e.padY > 0)
        {
            Debug.Log("Increase TP Distance");
            incrementTPDistance = 0.1f;
        }
        else if (e.padY < 0)
        {
            Debug.Log("Decrease TP Distance");
            incrementTPDistance = -0.1f;
        }
    }

}
