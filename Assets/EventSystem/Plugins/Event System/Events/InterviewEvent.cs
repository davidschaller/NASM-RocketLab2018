using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;


class InterviewRunner : IEvent
{
    public EventPlayer eventPlayer;
    public NPCInformator forcedNPC;

    #region IEvent Members

    public void OnEnter()
    {
        if (forcedNPC != null)
        {
            InterviewRenderer.ForcedNPC = forcedNPC;
            InterviewRenderer.Init(forcedNPC.location, eventPlayer);
            
            MovementController.Main.ClearWalkTarget();

            Vector3 lookAtPlayerPos = PCCamera.Main.FirstPersonView();

            forcedNPC.Lock("Interview has started");
            forcedNPC.LookAt(lookAtPlayerPos);
            MovementController.Main.LockPCControl();
        }
        else
            Debug.Log("forcedNPC is NULL");
    }

    public void OnExecute()
    {
        if (InterviewRenderer.IsFinished())
        {
            eventFinished = true;
        }
    }

    public void OnExit()
    {
        Debug.Log("Setting PC Control on dialog exit");
		
		//if(eventPlayer.playEventOnExit == null)
		{
	        forcedNPC.RestoreRotation();
	        PCCamera.Main.ThirdPersonView();
	        MovementController.Main.UnlockPCControl(false);
	        forcedNPC.Unlock("Interview has finished");
		}
        InterviewRenderer.Finish();
    }

    bool eventFinished = false;
    public bool EventIsFinished()
    {
        return eventFinished;
    }

    public bool EventFinished
    {
        get
        {
            Debug.LogError("Disabled. Use EventIsFinished insead if it");
            throw new NotImplementedException();
        }
        set
        {
            Debug.LogError("Disabled. Can't set this property");
            throw new NotImplementedException();
        }
    }

    public void OnReset()
    {
    }

    public void OnTerminate()
    {
    }

    #endregion
}

[System.Serializable]
public class InterviewEvent : EventBase
{
    public NPCInformator forcedNPC;

    //[HideInInspector]
    public override IEvent CreateRunner()
    {
        InterviewRunner runner = new InterviewRunner();
        runner.eventPlayer = calledBy;
        runner.forcedNPC = forcedNPC;
        return runner;
    }

    string eventName = "Interview Event";
    public override string GetEventName()
    {
        return eventName;
    }
}

