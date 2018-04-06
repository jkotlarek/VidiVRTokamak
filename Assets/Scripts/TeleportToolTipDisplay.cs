using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToolTipDisplay : MonoBehaviour {

    SteamVR_TrackedController controller;
    public bool enablePad = true;

    GameObject up, down;

    // Use this for initialization
    void Start () {
        controller = GetComponent<SteamVR_TrackedController>();

        Transform controllerUI = transform.Find("ControllerUI");

        up = controllerUI.Find("UpArrow").gameObject;
        down = controllerUI.Find("DownArrow").gameObject;

        if (enablePad)
        {
            controller.PadTouched += OnPadTouched;
            controller.PadUntouched += OnPadUntouched;
        }
    }

    private void OnPadTouched(object sender, ClickedEventArgs e)
    {
        Debug.Log("PadTouched");
        up.SetActive(true);
        down.SetActive(true);
    }

    private void OnPadUntouched(object sender, ClickedEventArgs e)
    {
        Debug.Log("PadUntouched");
        up.SetActive(false);
        down.SetActive(false);
    }
}
