using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class TestRunner : IEvent
{
	bool debug = false;
	
	//[HideInInspector]
	public float timesToExecute = 10;
	public int intNumber = 3;
	public string testString = "Test";
	public Vector3 vector = new Vector3(3, 1, 10);
	public Vector2 vector2 = new Vector2(2, 10);
	public GameObject testGO;
	
	string eventName = "Test Event";
	
	public void OnEnter ()
	{
		timesToExecute = 10;
		if (debug) Debug.Log("Enter: " + GetNickname() + " (" + eventName + "), will execute " + timesToExecute + " times.");
	}
	public void OnExecute ()
	{
		if (debug) Debug.Log("Exec: " + GetNickname() + " (" + eventName + "), times to go: " + timesToExecute);
		timesToExecute--;
		
		if (timesToExecute == 0)
			eventFinished = true;
	}
	public void OnExit ()
	{
		if (debug) Debug.Log("Exit: " + GetNickname() + " (" + eventName + ")");
	}

	string GetNickname ()
	{
		return "TestRunner";
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
public class TestEvent : EventBase
{
	//[HideInInspector]
	public float timesToExecute = 10;
	public int intNumber = 3;
	public string testString = "Test";
	public Vector3 vector = new Vector3(3, 1, 10);
	public Vector2 vector2 = new Vector2(2, 10);
	public GameObject testGO;
	
	public override IEvent CreateRunner ()
	{
		TestRunner runner = new TestRunner();
		runner.timesToExecute = timesToExecute;
		runner.intNumber = intNumber;
		runner.testString = testString;
		runner.vector = vector;
		runner.vector2 = vector2;
		runner.testGO = testGO;
		return runner;
	}
	
	string eventName = "Test Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}