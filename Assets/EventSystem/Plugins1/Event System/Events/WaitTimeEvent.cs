using UnityEngine;
using System.Collections;
using System;

/// <summary>
///   I'd like a new event-- a simple timer that I can insert between
///   other events in an Event Scenario and specify a wait time before
///   triggering the next event. I think the only SV necessary would be
///   "Timer" or "Wait Time" or something like that, a float variable so 
///   I can assign a duration to it in the Inspector.
/// </summary>
class WaitTimeRunner : IEvent
{
    public float waitTime;

    float targetTime = 0;

	public void OnEnter ()
	{
        targetTime = Time.time + waitTime;
	}

	public void OnExecute ()
	{
        if (Time.time > targetTime)
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
        targetTime = 0;
    }

    public void OnTerminate()
    {

    }
}

[System.Serializable]
public class WaitTimeEvent : EventBase
{
	//[HideInInspector]
    public float waitTime;

	public override IEvent CreateRunner ()
	{
        WaitTimeRunner runner = new WaitTimeRunner();
        runner.waitTime = waitTime;

        return runner;
	}
	
	string eventName = "Wait Time Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}