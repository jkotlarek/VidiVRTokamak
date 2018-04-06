using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_ControllerInput : MonoBehaviour {
    
    protected SteamVR_TrackedController controller;
    protected PopulateParticles particleScript;

    // Use this for initialization
    protected void Start () {
        particleScript = FindObjectOfType<PopulateParticles>();
        controller = GetComponent<SteamVR_TrackedController>();
	}


    protected void DPadToggle(object sender, ClickedEventArgs e)
    {
        //bottom
        if (e.padY > 0.5f)
        {
            Debug.Log("HandlePadClicked:Top");
            particleScript.ToggleSpeedshift();
        }
        //top
        if (e.padY < -0.5f)
        {
            Debug.Log("HandlePadClicked:Bottom");
            particleScript.ToggleTrails();
        }
        //right
        if (e.padX > 0.5f)
        {
            Debug.Log("HandlePadClicked:Right");
            particleScript.FramesPerTimestep(-1);
        }
        //left
        if (e.padX < -0.5f)
        {
            Debug.Log("HandlePadClicked:Left");
            particleScript.FramesPerTimestep(1);
        }
    }

    protected void TogglePause(object sender, ClickedEventArgs e)
    {
        particleScript.paused = !particleScript.paused;
    }

    protected void DPadMode (object sender, ClickedEventArgs e)
    {
        //bottom
        if (e.padY > 0.5f)
        {
            Debug.Log("HandlePadClicked:Top");
            particleScript.FlushTrails();
            particleScript.currentMode = ParticleDisplayMode.Normal;
            particleScript.flushTrails = true;
        }
        //top
        if (e.padY < -0.5f)
        {
            Debug.Log("HandlePadClicked:Bottom");
            particleScript.FlushTrails();
            particleScript.currentMode = ParticleDisplayMode.Flat;
            particleScript.flushTrails = true;
        }
        //right
        if (e.padX > 0.5f)
        {
            Debug.Log("HandlePadClicked:Right");
        }
        //left
        if (e.padX < -0.5f)
        {
            Debug.Log("HandlePadClicked:Left");
        }
    }

    protected void ClearHighlight(object sender, ClickedEventArgs e)
    {
        particleScript.ClearHighlight();
    }
}
