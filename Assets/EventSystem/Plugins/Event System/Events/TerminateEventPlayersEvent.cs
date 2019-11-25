using UnityEngine;
using System.Collections;
using System;

class TerminateEventPlayersRunner : IEvent
{
    public EventPlayer[] eventPlayersToTerminate;

	public void OnEnter ()
	{
        if (eventPlayersToTerminate != null)
        {
            foreach (EventPlayer ep in eventPlayersToTerminate)
            {
                ep.ForceTerminate(null);
            }
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
public class TerminateEventPlayersEvent : EventBase
{
	public EventPlayer[] eventPlayersToTerminate;
	
	public override IEvent CreateRunner ()
	{
        TerminateEventPlayersRunner runner = new TerminateEventPlayersRunner();
        runner.eventPlayersToTerminate = eventPlayersToTerminate;

		return runner;
	}

    string eventName = "Terminate EventPlayers Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}