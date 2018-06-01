using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMiniMap : MonoBehaviour {

    public Transform followPosition;
    public Transform trackedPosition;
    public Transform trackerPosition;
    public Transform tokamakForScale;
    public Transform pointerPosition;
    public Transform trackedPointerPosition;

    public bool updateTeleport = true;

    Vector3 FollowPosition { get { return followPosition.position; } }
    Vector3 TrackedPosition { get { return trackedPosition.position; } }
    Vector3 TrackerPosition { get { return trackerPosition.localPosition; } set { trackerPosition.localPosition = value; } }
    Vector3 TokamakPosition { get { return tokamakForScale.position; } }
    Vector3 TokamakScale { get { return tokamakForScale.localScale; } }
    Vector3 PointerPosition { get { return pointerPosition.position; } set { pointerPosition.localPosition = value; } }
    Vector3 TrackedPointerPosition { get { return trackedPointerPosition.position; } }

    LineRenderer teleportHistory;
    LineRenderer pointerLine;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Lerp minimap towards controller
        transform.position = Vector3.MoveTowards(transform.position, FollowPosition, 1.0f);
        TrackerPosition = Vector3ComponentDivide(TrackedPosition - TokamakPosition, TokamakScale);

        //Update PointerPosition and its line renderer
        if (pointerLine == null) pointerLine = pointerPosition.GetComponent<LineRenderer>();
        if (pointerLine.positionCount != 2) pointerLine.positionCount = 2;

        if (trackedPointerPosition.gameObject.activeInHierarchy)
        {
            if (!pointerPosition.gameObject.activeSelf) pointerPosition.gameObject.SetActive(true);
            PointerPosition = Vector3ComponentDivide(TrackedPointerPosition - TokamakPosition, TokamakScale);
            pointerLine.SetPosition(0, pointerPosition.position);
            pointerLine.SetPosition(1, trackerPosition.position);
        }
        else if (pointerPosition.gameObject.activeSelf)
        {
            pointerPosition.gameObject.SetActive(false);
        }

        //Update positions if we've teleported
        if (updateTeleport)
        {
            if (teleportHistory == null)
            {
                teleportHistory = transform.GetComponent<LineRenderer>();
                teleportHistory.positionCount = 0;
            }
            if (teleportHistory.positionCount == 0 ||
                Vector3.Distance(teleportHistory.GetPosition(teleportHistory.positionCount - 1), TrackerPosition) >= 0.05)
            {
                teleportHistory.positionCount++;
                teleportHistory.SetPosition(teleportHistory.positionCount - 1, TrackerPosition);
            }
        }
    }

    Vector3 Vector3ComponentDivide(Vector3 a, Vector3 b) {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
