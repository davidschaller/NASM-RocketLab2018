using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class NullRunner : IEvent
{
	public void OnEnter ()
	{
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
public class NullEvent : EventBase
{
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		return new NullRunner();
	}
	
	string eventName = "Null Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}