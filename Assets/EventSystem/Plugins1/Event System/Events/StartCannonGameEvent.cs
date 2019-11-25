using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class StartCannonGameRunner : IEvent
{
	public CannonController cannon;
	
	public void OnEnter ()
	{
		Debug.Log("Starting Cannon Game Event");
		
		cannon.StartGame();
	}
	public void OnExecute ()
	{
		if (cannon.Finished)
		{
			//Debug.LogWarning("StartFlashbackRunner thinks FBM said flashback is done");
			
			eventFinished = true;
		}
		
	}
	public void OnExit ()
	{
		Debug.Log("Exiting Cannon Game Event");
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
public class StartCannonGameEvent : EventBase
{
	public CannonController cannon;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		StartCannonGameRunner runner = new StartCannonGameRunner();
		runner.cannon = cannon;
		return runner;
	}
	
	string eventName = "Start Cannon Game Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}