using UnityEngine;
using System.Collections;
using System;

class MountRunner : IEvent
{
    //[HideInInspector]
    public NPCController horse;
    public NPCController rider;
    public bool skipMountingAnimation;

    private int oldChildsCount = 0;
    private MovementController oMovementController = null;
    private GameObject playerGO = null;

    public void OnEnter()
    {
        if (horse)
        {
            oMovementController = MovementController.GetMovementController();

            if (oMovementController)
            {
                oldChildsCount = horse.transform.childCount;

                if (rider)
                {
                    oMovementController.Mount(rider.transform, horse.transform, skipMountingAnimation);
                }
                else
                {
                    if (!skipMountingAnimation)
                    {
                        oMovementController.Mount(GameObject.FindWithTag("Player").transform, horse.transform, skipMountingAnimation);
                    }
                }
            }
            else
            {
                Debug.Log("Mount failed MovementController obj is null");
            }
        }
    }

    public void OnExecute()
    {
        if (oldChildsCount < horse.transform.childCount)
        {
            eventFinished = true;
        }

        if (!eventFinished && rider == null && ActorUtility.PlayerFound)
        {
            if (!oMovementController)
            {
                oMovementController = MovementController.GetMovementController();
            }
            else
            {
                if (!playerGO)
                {
                    playerGO = (GameObject)GameObject.FindWithTag("Player");
                }
                else
                {
                    oMovementController.Mount(playerGO.transform, horse.transform, skipMountingAnimation);
                    eventFinished = true;
                }
            }
        }
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
public class MountEvent : EventBase
{
    //[HideInInspector]
    public NPCController horse;
    public NPCController rider;
    public bool skipMountingAnimation;

    public override IEvent CreateRunner()
    {
        MountRunner runner = new MountRunner();
        runner.horse = horse;
        runner.rider = rider;
        runner.skipMountingAnimation = skipMountingAnimation;

        return runner;
    }


    string eventName = "Mount Event";
    public override string GetEventName()
    {
        return eventName;
    }
}