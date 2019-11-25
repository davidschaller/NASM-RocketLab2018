using UnityEngine;
using System.Collections;
using System;

class MultipleChoiceSelectorRunner : IEvent
{
	public EventPlayer[] eventPlayers;
	
	public void OnEnter ()
	{
		if (MultipleChoiceQuestionRunner.savedAnswer < eventPlayers.Length && eventPlayers[MultipleChoiceQuestionRunner.savedAnswer] != null)
		{
			eventPlayers[MultipleChoiceQuestionRunner.savedAnswer].PlayerTriggered();
		}
		else
		{
			Debug.LogWarning("No event player found for choice " + MultipleChoiceQuestionRunner.savedAnswer + " have " + eventPlayers.Length + " players");
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
public class MultipleChoiceSelectorEvent : EventBase
{
	public EventPlayer[] eventPlayers;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		MultipleChoiceSelectorRunner r = new MultipleChoiceSelectorRunner();
		r.eventPlayers = eventPlayers;
		return r;
	}
	
	string eventName = "Multiple Choice Selector Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}