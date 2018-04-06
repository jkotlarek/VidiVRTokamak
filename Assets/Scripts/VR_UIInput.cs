using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VR_UIInput : MonoBehaviour {

    SteamVR_LaserPointer laserPointer;
    SteamVR_TrackedController trackedController;

    Canvas canvas;

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;
        
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        trackedController.TriggerClicked -= HandleTriggerClicked;
        trackedController.TriggerClicked += HandleTriggerClicked;
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Debug.Log("click");
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
            
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        Debug.Log(e.target.name);
        var button = e.target.GetComponent<Button>();
        Debug.Log(button);
        if (button != null)
        {
            button.Select();
        }
    }
    
    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
