using UnityEngine;
using System.Collections;
using System;

class DismountRunner : IEvent
{
    //[HideInInspector]
    public NPCController horse;
    public NPCController rider;

    public void OnEnter()
    {
        if (horse)
        {
            MovementController oMovementController = MovementController.GetMovementController();

            if (oMovementController)
            {
                if (rider)
                {
                    oMovementController.Dismount(rider.transform, horse.transform);
                }
                else
                {
                    oMovementController.Dismount(GameObject.FindWithTag("Player").transform, horse.transform);
                }
            }
            else
            {
                Debug.Log("Mount failed MovementController obj is null");
            }
        }
    }

    float delayEvent = 1.6f;
    public void OnExecute()
    {
        if (rider != null && delayEvent > 0)
        {
            if (rider.transform.parent == null)
            {
                eventFinished = true;
            }
        }
        delayEvent -= Time.deltaTime;
    }

    
    public void OnExit()
    {

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
public class DismountEvent : EventBase
{
    //[HideInInspector]
    public NPCController horse;
    public NPCController rider;

    public override IEvent CreateRunner()
    {
        DismountRunner runner = new DismountRunner();
        runner.horse = horse;
        runner.rider = rider;

        return runner;
    }


    string eventName = "Dismount Event";
    public override string GetEventName()
    {
        return eventName;
    }
}