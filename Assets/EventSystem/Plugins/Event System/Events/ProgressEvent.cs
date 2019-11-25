using UnityEngine;
using System.Collections;
using System;

class ProgressRunner : IEvent
{
	public static int stage = 0;
	public void OnEnter ()
	{
		Debug.Log("Increment stage to " + stage);
		stage++;
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
public class ProgressEvent : EventBase
{
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		return new ProgressRunner();
	}
	
	string eventName = "Progress Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}