using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerToolTipDisplay : MonoBehaviour {

    SteamVR_TrackedController controller;

    GameObject TrailTooltip;
    GameObject FasterTooltip;
    GameObject SlowerTooltip;
    GameObject HueshiftTooltip;
    //GameObject PlayPauseRight;
    //GameObject PlayPauseLeft;

    public bool enablePad = true;

    // Use this for initialization
    void Start()
    {
        Debug.Log("startup tooltip");
        controller = GetComponent<SteamVR_TrackedController>();

        Transform controllerUI = transform.Find("ControllerUI");

        TrailTooltip = controllerUI.Find("TrailTooltip").gameObject;
        FasterTooltip = controllerUI.Find("FasterTooltip").gameObject;
        SlowerTooltip = controllerUI.Find("SlowerTooltip").gameObject;
        HueshiftTooltip = controllerUI.Find("HueshiftTooltip").gameObject;
        //PlayPauseLeft = controllerUI.Find("PlayPause Left").gameObject;
        //PlayPauseRight = controllerUI.Find("PlayPause Right").gameObject;

        if (enablePad)
        {
            controller.PadTouched += OnPadTouched;
            controller.PadUntouched += OnPadUntouched;
        }
    }

    private void OnEnable()
    {

    }

    private void OnPadTouched(object sender, ClickedEventArgs e)
    {
        Debug.Log("PadTouched");
        TrailTooltip.SetActive(true);
        FasterTooltip.SetActive(true);
        SlowerTooltip.SetActive(true);
        HueshiftTooltip.SetActive(true);
        //PlayPauseLeft.SetActive(true);
        //PlayPauseRight.SetActive(true);
    }

    private void OnPadUntouched(object sender, ClickedEventArgs e)
    {
        Debug.Log("PadUntouched");
        TrailTooltip.SetActive(false);
        FasterTooltip.SetActive(false);
        SlowerTooltip.SetActive(false);
        HueshiftTooltip.SetActive(false);
        //PlayPauseLeft.SetActive(false);
        //PlayPauseRight.SetActive(false);
    }



}
