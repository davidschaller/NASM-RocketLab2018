using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class StartTreasureHuntRunner : IEvent
{
	public TreasureHuntController treasure;
	
	public void OnEnter ()
	{
		treasure.StartGame();
	}
	public void OnExecute ()
	{
		if (treasure.Finished)
		{
			eventFinished = true;
		}
		
	}
	public void OnExit ()
	{
		Debug.Log("Exiting treasure hunt event");
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
public class StartTreasureHuntEvent : EventBase
{
	public TreasureHuntController treasure;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		StartTreasureHuntRunner runner = new StartTreasureHuntRunner();
		runner.treasure = treasure;
		return runner;
	}
	
	string eventName = "Start Treasure Hunt Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}