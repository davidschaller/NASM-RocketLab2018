using UnityEngine;
using System.Collections;
using System;

class CameraWaypointRunner : IEvent
{
    public static string CAMERA_WAYPOINT_TAG = "CameraWaypoint";

    private static int inProccess;

    public static bool InProccess
    {
        get
        {
            return inProccess > 0;
        }
        private set { }
    }

    //[HideInInspector]
    public WaypointGroup waypointGroup;
    public float defaultMoveSpeed = 1;
    public bool jumpToFirstWaypoint = false;
    public bool restoreToOrigPosition = false;
    public Transform optionalCamera;

    Vector3 origPosition,
            origLocalPosition;

    Quaternion origRotation,
               origLocalRotation;

    Waypoint currWaypoint;
    float currSpeed;
    float totalDistance;
    Quaternion startRotation;
    Transform cam;
    Vector3 startPosition;
    public void OnEnter()
    {
        if (waypointGroup == null)
            throw new EventException("Waypoint Group is null for Camera Waypoint Event");

        /*if (!CameraController.RequestCameraLock(GetNickname()))
        {
            Debug.Log("Could not acquire camera lock, locked by " + CameraController.LockedBy());
            eventFinished = true;
        }
        else
        {*/
        if (optionalCamera == null)
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("Optional camera not specified and main camera is deactivated or couldn't be found, deactivating camera waypoint event");
                eventFinished = true;
                return;
            }

        }
        else
        {
            cam = optionalCamera;
        }
        Debug.Log("Using camera: " + cam.name);
        if (restoreToOrigPosition)
        {
            origPosition = cam.position;
            origLocalPosition = cam.localPosition;
            origRotation = cam.rotation;
            origLocalPosition = cam.localPosition;
        }

        currSpeed = defaultMoveSpeed;
        currWaypoint = waypointGroup.FirstWaypoint();
        if (jumpToFirstWaypoint)
        {
            cam.position = currWaypoint.transform.position;
            cam.rotation = currWaypoint.transform.rotation;
        }
        SetStart();
        Debug.Log("Starting waypoint " + currWaypoint.gameObject.name + " of " + currWaypoint.transform.parent.name, currWaypoint);
        //}
    }

    void Log(string msg)
    {
        //Debug.Log("======== " + msg);
    }

    float DistanceToWP()
    {
        return (cam.position - currWaypoint.transform.position).magnitude;
    }

    float timeLeft;
    void SetStart()
    {
        totalDistance = DistanceToWP();
        startSpeed = currWaypoint.speedHint;
        startRotation = cam.rotation;
        timeLeft = totalDistance / defaultMoveSpeed;
        startPosition = cam.position;
    }

    bool PassedWaypoint(Vector3 startPoint, Waypoint w)
    {
        Vector3 forward = w.transform.position - startPoint;
        Vector3 toOther = w.transform.position - cam.position;
        if (Vector3.Dot(forward, toOther) < 0)
        {
            return true;
        }

        return false;
    }

    float startSpeed;
    public void OnExecute()
    {
        /*Debug.Log("Waypoint: " + currWaypoint.name + 
                  ", close? " + currWaypoint.PositionIsClose(cam.position) + 
                  " (" + (currWaypoint.transform.position - cam.position).magnitude.ToString("#.0") +  ")" +
                  ", passed? " + PassedWaypoint(startPosition, currWaypoint) + 
                  ", cam: " + cam.position + 
                  ", wp: " + currWaypoint.transform.position +
                  " at " + Time.time.ToString("#.0"), currWaypoint);
                  */


        if (currWaypoint.PositionIsClose(cam.position) || PassedWaypoint(startPosition, currWaypoint))
        {
            currWaypoint = waypointGroup.GetNextWaypoint(currWaypoint);
            if (currWaypoint == null || Input.GetKeyDown("space"))
            {
                Waypoint lastPoint = waypointGroup.LastWaypoint;
                cam.position = lastPoint.transform.position;
                cam.rotation = lastPoint.transform.rotation;
                eventFinished = true;
                return;
            }
            else
            {
                SetStart();
            }
        }

        inProccess++;

        if (currWaypoint != null)
        {
            if (currWaypoint.speedHint != 0)
            {
                currSpeed = Mathf.Lerp(startSpeed, currWaypoint.speedHint, 1f - DistanceToWP() / totalDistance);
            }
            else
            {
                currSpeed = Mathf.Lerp(startSpeed, defaultMoveSpeed, 1f - DistanceToWP() / totalDistance);
            }

            Vector3 vecTo = (currWaypoint.transform.position - cam.position);
            float dist = vecTo.magnitude;
            if (dist != 0)
                cam.position += (currWaypoint.transform.position - cam.position).normalized * currSpeed * Time.deltaTime;

            if (waypointGroup.cameraWatchTarget == null && dist != 0)
            {
                float smooth = 1 - DistanceToWP() / totalDistance;
                cam.rotation = Quaternion.Slerp(startRotation, currWaypoint.transform.rotation, smooth);
                //Debug.Log("Smooth: " + smooth);
            }
            else
                cam.LookAt(waypointGroup.cameraWatchTarget);
        }

        /*if (!CameraController.Locked())
        {
            Waypoint lastPoint = waypointGroup.LastWaypoint;
            cam.position = lastPoint.transform.position;
            cam.rotation = lastPoint.transform.rotation;

            Debug.Log("Exiting due to camera lock being broken (last broken by " + CameraController.LastBreaker() + ").");
            eventFinished = true;
        }*/

        timeLeft -= Time.deltaTime;
    }
    string GetNickname()
    {
        return CAMERA_WAYPOINT_TAG + " " + waypointGroup.name;
    }

    public void OnExit()
    {
        inProccess--;
        CameraController.ReleaseLock(GetNickname());

        if (cam != null && restoreToOrigPosition)
        {
            cam.position = origPosition;
            cam.localPosition = origLocalPosition;
            cam.rotation = origRotation;
            cam.localPosition = origLocalPosition;
        }
    }

    bool eventFinished = false;
    public bool EventIsFinished()
    {
        return eventFinished;
    }

    public void OnReset()
    {
    }

    public void OnTerminate()
    {
    }
}

[System.Serializable]
public class CameraWaypointEvent : EventBase
{
    //[HideInInspector]
    public WaypointGroup waypointGroup;
    public float defaultMoveSpeed = 1;
    public bool jumpToFirstWaypoint = false;
    public bool restoreToOrigPosition = false;
    public Transform optionalCamera;

    public override IEvent CreateRunner()
    {
        CameraWaypointRunner runner = new CameraWaypointRunner();
        runner.waypointGroup = waypointGroup;
        runner.defaultMoveSpeed = defaultMoveSpeed;
        runner.jumpToFirstWaypoint = jumpToFirstWaypoint;
        runner.restoreToOrigPosition = restoreToOrigPosition;
        runner.optionalCamera = optionalCamera;
        return runner;
    }

    string eventName = "Camera Waypoint";
    public override string GetEventName()
    {
        return eventName;
    }
}