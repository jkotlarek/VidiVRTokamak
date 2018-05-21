using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMiniMap : MonoBehaviour {

    public Transform followPosition;
    public Transform trackedPosition;
    public Transform trackerPosition;
    public Transform tokamakForScale;

    public bool updateTeleport = true;

    Vector3 FollowPosition { get { return followPosition.position; } }
    Vector3 TrackedPosition { get { return trackedPosition.position; } }
    Vector3 TrackerPosition { get { return trackerPosition.localPosition; } set { trackerPosition.localPosition = value; } }
    Vector3 TokamakPosition { get { return tokamakForScale.position; } }
    Vector3 TokamakScale { get { return tokamakForScale.localScale; } }

    LineRenderer teleportHistory;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, FollowPosition, 1.0f) + Vector3.up * 0.2f;
        TrackerPosition = Vector3ComponentDivide(TrackedPosition - TokamakPosition, TokamakScale);

        if (updateTeleport)
        {
            if (teleportHistory == null)
            {
                teleportHistory = transform.GetComponent<LineRenderer>();
                teleportHistory.positionCount = 0;
            }
            teleportHistory.positionCount++;
            teleportHistory.SetPosition(teleportHistory.positionCount - 1, TrackerPosition);
        }
    }

    Vector3 Vector3ComponentDivide(Vector3 a, Vector3 b) {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }
}
