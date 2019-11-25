using UnityEngine;
using System.Collections;
using System;

class ResumePlayerControlRunner : IEvent
{
	public void OnEnter ()
	{
		MovementController mc = MovementController.GetMovementController();
        if (mc != null)
        {
            mc.UnlockPCControl(false);
        }
		eventFinished = true;
	}
	public void OnExecute ()
	{
	}
	public void OnExit ()
	{
	}

	bool eventFinished = false;
	public bool EventIsFinished ()
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
public class ResumePlayerControlEvent : EventBase
{
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		return new ResumePlayerControlRunner();
	}
	
	string eventName = "Resume Player Control Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}