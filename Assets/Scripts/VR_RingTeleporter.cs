using UnityEngine;
using System.Collections;

public class VR_RingTeleporter : MonoBehaviour
{
    public bool teleportOnClick = false;
    public Transform model;

    bool isTeleporting = false;
    float time = 0.0f;
    float inc = 0.01f;
    Quaternion slerpStart;
    Quaternion slerpStop;

    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    void Start()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.TriggerClicked += new ClickedEventHandler(DoClick);
    }

    void Update()
    {
        //interpolate around the ring
        if (isTeleporting)
        {
            model.rotation = Quaternion.Slerp(slerpStart, slerpStop, time);
            time += inc;
        }

        if (time >= 1.0f)
        {
            isTeleporting = false;
            time = 0.0f;
        }
    }

    void DoClick(object sender, ClickedEventArgs e)
    {
        if (teleportOnClick)
        {
            // First get the current Transform of the the reference space (i.e. the Play Area, e.g. CameraRig prefab)
            var t = reference;
            if (t == null)
                return;

            // Get the current Y position of the reference space
            float refY = t.position.y;

            // Create a plane at the Y position of the Play Area
            // Then create a Ray from the origin of the controller in the direction that the controller is pointing
            Plane plane = new Plane(Vector3.up, -refY);
            Ray ray = new Ray(this.transform.position, transform.forward);

            // Set defaults
            bool hasGroundTarget = false;
            float dist = 0f;

            // Intersect a ray with the plane that was created earlier
            // and output the distance along the ray that it intersects
            hasGroundTarget = plane.Raycast(ray, out dist);

            if (hasGroundTarget)
            {
                // Get the current Camera (head) position on the ground relative to the world
                Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.position.x, refY, SteamVR_Render.Top().head.position.z);
                
                // Calculate where to teleport
                Vector3 clickPos = t.position + (ray.origin + (ray.direction * dist)) - headPosOnGround;

                // Find angle difference between current position and teleport location
                float theta = Vector3.Angle(t.position, clickPos);
                if (clickPos.x < 0.0f)
                {
                    theta = -theta;
                }

                slerpStart = model.rotation;
                slerpStop = Quaternion.Euler(model.rotation.eulerAngles + Vector3.up * theta);
                isTeleporting = true;
            }
        }
    }
}

