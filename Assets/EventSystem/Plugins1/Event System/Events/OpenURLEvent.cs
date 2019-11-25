using UnityEngine;
using System.Collections;
using System;

class OpenURLRunner : IEvent
{
	public string url;
	
	public void OnEnter ()
	{
		Application.OpenURL(url);
	}
	public void OnExecute ()
	{
		eventFinished = true;
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
public class OpenURLEvent : EventBase
{
	public string url;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		OpenURLRunner runner = new OpenURLRunner();
		runner.url = url;
		return runner;
	}
	
	string eventName = "Open URL Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}