using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

class ActorWaypointRunner : IEvent
{
    //TODO [HideInInspector]
    public WaypointGroup waypointGroup;
    public NPCController npc;

    Waypoint currWaypoint, lastWayPoint;
    public float defaultMoveSpeed = 1;
    float currSpeed;
    float totalDistance;
    ActorBase actor;
    Vector3 startPosition;
    bool eventFinished = false;
    public void OnEnter()
    {
        npc.ActivateNPC();
        actor = npc.Actor;
        if (actor == null)
        {
            eventFinished = true;
            Log("Actor waypoint group exiting (actor is NULL).  " +
                                     "Check to make sure the NPC actor assigned to this event has not been deactivated.  " +
                                     "If it has intentionally, deactivate this player");
        }
        //actor.forwardMoveSpeed = 1f;
        /*if (!npc.enabled || !npc.active)
        {
            if (Application.isEditor && npc.gameObject.name.Contains("group "))
                Debug.Log("(AnimateNPC Event) NPC Controller is not enabled.  (its either out of LOD or has not been assigned)", npc.gameObject);
            eventFinished = true;
            return;
            }*/

        if (waypointGroup == null)
            Log("Waypoint Group is null for Actor Waypoint Event");

        currSpeed = defaultMoveSpeed;

        if (waypointGroup == null)
        {
            eventFinished = true;
            throw new EventException("Actor waypoint group exiting (waypoint group is NULL) - " +
                                     "check to make sure the waypoint group exists and has a valid SV.  " +
                                     "if you've removed this group from the scene, deactivate this player, or remove the event that uses this group.");
        }
        //waypointGroup.GroundAll();
        currWaypoint = waypointGroup.FirstWaypoint();
        //Debug.Log("================= Starting waypoint " + currWaypoint.gameObject.name, currWaypoint.gameObject);
        //Debug.Log("GROUP: ", waypointGroup);
        SetStart();
    }

    void Log(string msg)
    {
        Debug.Log("======== " + msg, npc.gameObject);
    }

    float DistanceToWP()
    {
        if (currWaypoint == null)
        {
            throw new EventException("current waypoint is null...why?");
        }

        return (actor.transform.position - currWaypoint.transform.position).magnitude;
    }

    float timeLeft;
    void SetStart()
    {
        totalDistance = DistanceToWP();
        startSpeed = currSpeed;
        timeLeft = totalDistance / defaultMoveSpeed;
        if (currWaypoint == null)
            Debug.LogWarning("CURR WAYPOINT IS NULL", waypointGroup);

        currWaypoint.Ground();
        startPosition = actor.transform.position;
    }

    bool PassedWaypoint(Vector3 startPoint, Waypoint w)
    {
        Vector3 forward = w.transform.position - startPoint;
        Vector3 toOther = w.transform.position - actor.transform.position;
        if (Vector3.Dot(forward, toOther) < 0)
        {
            return true;
        }

        return false;
    }

    float startSpeed;
    public void OnExecute()
    {
        if (!actor)
            return;

        if (!actor.CanMove)
            return;

        if (currWaypoint && !currWaypoint.Grounded)
        {
            Debug.Log("Having trouble grounding waypoint", currWaypoint);

            currWaypoint.Ground();
        }

        if (currWaypoint && currWaypoint.PositionIsClose(actor.transform.position) ||
            (currWaypoint && PassedWaypoint(startPosition, currWaypoint)))
        {
            currWaypoint = waypointGroup.GetNextWaypoint(currWaypoint);
            if (currWaypoint == null)
            {
                //Log("----->ActorWaypointEvent finishing due to " + (currWaypoint==null ? "no more waypoints" : "space interrupt"));
                eventFinished = true;
            }
            else
            {
                //float angle = Vector3.Angle(actor.transform.TransformDirection(Vector3.forward), currWaypoint.transform.position - actor.transform.position);
                //Debug.Log("Switching to " + currWaypoint.gameObject.name + " angle is " + angle, currWaypoint.gameObject);
                if (currWaypoint != null)
                {
                    lastWayPoint = currWaypoint;
                    SetStart();
                }
            }
        }

        if (currWaypoint != null)
        {
            Vector3 vecTo = currWaypoint.transform.position - actor.transform.position;
            float angle = Vector3.Angle(actor.transform.TransformDirection(Vector3.forward), vecTo);

            if (currWaypoint.speedHint != 0)
            {
                currSpeed = Mathf.Lerp(startSpeed, currWaypoint.speedHint, 1f - DistanceToWP() / totalDistance);
            }
            else
            {
                currSpeed = Mathf.Lerp(startSpeed, defaultMoveSpeed, 1f - DistanceToWP() / totalDistance);
            }

            float rightAngle = Vector3.Angle(actor.transform.TransformDirection(Vector3.right), vecTo);
            float leftAngle = Vector3.Angle(actor.transform.TransformDirection(-Vector3.right), vecTo);

            actor.MoveForward(currSpeed, false);

            /*
             * Commented due to npc turns back to passed waypoint sometimes 
             * (NPC bounces off nodes bug) 
             */
            //if (Mathf.Abs(angle) < 2)
            //{
            //    actor.transform.LookAt(currWaypoint.transform.position);
            //}

            if (Mathf.Abs(angle) > 2)
            {
                if (rightAngle < leftAngle)
                {
                    actor.TurnRight(rightAngle, currWaypoint.transform.position);
                }
                else if (rightAngle > leftAngle)
                {
                    actor.TurnLeft(leftAngle, currWaypoint.transform.position);
                }
            }
        }
        else if (lastWayPoint != null)
        {
            //npc.IdleRevert();
            npc.transform.rotation = lastWayPoint.transform.rotation;
        }

        timeLeft -= Time.deltaTime;
    }

    public void OnExit()
    {
    }

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
public class ActorWaypointEvent : EventBase
{
    //TODO [HideInInspector]
    public WaypointGroup waypointGroup;
    public NPCController npc;
    public float defaultMoveSpeed = 1;

    public override IEvent CreateRunner()
    {
        ActorWaypointRunner runner = new ActorWaypointRunner();
        runner.waypointGroup = waypointGroup;
        runner.npc = npc;
        runner.defaultMoveSpeed = defaultMoveSpeed;
        return runner;
    }

    string eventName = "Actor Waypoint";
    public override string GetEventName()
    {
        return eventName;
    }
}