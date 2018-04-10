using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public enum ControllerMode
{
    Undefined, Teleport, Highlight, Visibility, Options
}

public class VR_ControllerInput : MonoBehaviour {
    
    VRTK_ControllerEvents controller;
    VRTK_Pointer pointer;
    VRTK_BasePointerRenderer pointerRenderer;
    VR_ColliderTeleporter teleporter;
    PopulateParticles particleScript;
    VisibilitySegments visibilitySegments;

    Transform target;

    public ControllerMode mode = ControllerMode.Undefined;

    // Use this for initialization
    protected void Start () {
        visibilitySegments = FindObjectOfType<VisibilitySegments>();
        particleScript = FindObjectOfType<PopulateParticles>();
        controller = GetComponent<VRTK_ControllerEvents>();
        teleporter = GetComponent<VR_ColliderTeleporter>();
        pointer = GetComponent<VRTK_Pointer>();
        pointerRenderer = GetComponent<VRTK_BasePointerRenderer>();

        controller.GripPressed += TogglePause;
        controller.TriggerClicked += HandleTriggerClick;
        controller.TriggerUnclicked += HandleTriggerUnclick;
        pointer.DestinationMarkerEnter += HandlePointerEnter;
        pointer.DestinationMarkerExit += HandlePointerExit;

        //if (mode != ControllerMode.Highlight) { pointerRenderer.enabled = false; }
    }

    /*
    private void OnEnable()
    {
        controller = GetComponent<VRTK_ControllerEvents>();
        controller.GripPressed -= TogglePause;
        controller.GripPressed += TogglePause;
        controller.TriggerClicked -= HandleTriggerClick;
        controller.TriggerClicked += HandleTriggerClick;
        controller.TriggerUnclicked -= HandleTriggerUnclick;
        controller.TriggerUnclicked += HandleTriggerUnclick;

        pointer = GetComponent<VRTK_Pointer>();
        pointer.DestinationMarkerEnter -= HandlePointerEnter;
        pointer.DestinationMarkerEnter += HandlePointerEnter;
        pointer.DestinationMarkerExit -= HandlePointerExit;
        pointer.DestinationMarkerExit += HandlePointerExit;
    }
    */

    private void TogglePause(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("GripClicked");
        particleScript.paused = !particleScript.paused;
    }

    private void ToggleVisibility()
    {
        foreach (GameObject go in visibilitySegments.list)
        {
            go.SetActive(!go.activeSelf);
        }
    }

    private void HandleTriggerClick(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("TriggerClicked");
        switch (mode)
        {
            case ControllerMode.Highlight:
                if(target != null)
                    particleScript.HighlightParticle(target.gameObject);
                break;

            case ControllerMode.Options:
                break;

            case ControllerMode.Teleport:
                teleporter.ProjectReference();
                break;

            case ControllerMode.Visibility:
                break;

            case ControllerMode.Undefined:
                break;
        }
    }

    private void HandleTriggerUnclick(object sender, ControllerInteractionEventArgs e)
    {
        Debug.Log("TriggerUnclicked");
        switch (mode)
        {
            case ControllerMode.Teleport:
                teleporter.Teleport();
                break;
        }
    }

    private void HandlePointerEnter(object sender, DestinationMarkerEventArgs e)
    {
        Debug.Log("pointer enter: " + e.target);
        if (e.target.CompareTag("Particle"))
        {
            Debug.Log("hit particle");
            target = e.target;
        }
    }

    private void HandlePointerExit(object sender, DestinationMarkerEventArgs e)
    {
        Debug.Log("pointer exit");
        target = null;
    }

    public void RadialSelectHighlight()
    {
        mode = ControllerMode.Highlight;
        pointerRenderer.enabled = true;
    }

    public void RadialSelectVisibility()
    {
        //mode = ControllerMode.Visibility;
        ToggleVisibility();
        pointerRenderer.enabled = false;
    }

    public void RadialSelectTeleport()
    {
        mode = ControllerMode.Teleport;
        pointerRenderer.enabled = false;
    }

    public void RadialSelectOptions()
    {
        //mode = ControllerMode.Options;
        particleScript.ToggleTrails();
        pointerRenderer.enabled = false;
    }

    /*
    public void DPadToggle(object sender, ClickedEventArgs e)
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
    */
}
