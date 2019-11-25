using UnityEngine;
using System.Collections;
using System;

class SuspendPlayerControlRunner : IEvent
{
	public void OnEnter ()
	{
		MovementController mc = MovementController.GetMovementController();
		mc.LockPCControl();
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
public class SuspendPlayerControlEvent : EventBase
{
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		return new SuspendPlayerControlRunner();
	}
	
	string eventName = "Suspend Player Control Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}