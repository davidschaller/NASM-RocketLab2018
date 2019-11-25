using UnityEngine;
using System.Collections;
using System;

class TriggerEventScenarioRunner : IEvent
{
	public EventPlayer player;

	public void OnEnter ()
	{
        if (player)
		    player.PlayerTriggered();
		
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
public class TriggerEventScenarioEvent : EventBase
{
	public EventPlayer player;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		TriggerEventScenarioRunner r = new TriggerEventScenarioRunner();
		r.player = player;
		return r;
	}
	
	string eventName = "Trigger Event Scenario Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}