using UnityEngine;
using System.Collections;
using System;

class MultipleChoiceQuestionRunner : IEvent
{
	public TextAsset xmlFile;
	
	public bool center;
	public Rect choiceDimensions;
	public bool saveForLater;

	
	public delegate void ActivateGUICallback(MultipleChoiceQuestionRunner runner);
	public static ActivateGUICallback activateGUI;
	
	public static int savedAnswer = 0;
	
	public void OnEnter ()
	{
		Debug.Log("Activate multiple choice GUI...");
		activateGUI(this);
	}
	public void OnExecute ()
	{
	}
	public void OnExit ()
	{
		// hack to allow Pictorial event to have no button and be exited via another event's button
		PictorialRunner.exitFromAnotherEvent = true;
		Pictorial3Runner.exitFromAnotherEvent = true;
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
public class MultipleChoiceQuestionEvent : EventBase
{
	public TextAsset xmlFile;
	
	public bool center;
	public Rect choiceDimensions;
	
	public bool saveForLater;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		MultipleChoiceQuestionRunner r = new MultipleChoiceQuestionRunner();
		r.xmlFile = xmlFile;
		r.center = center;
		r.choiceDimensions = choiceDimensions;
		r.saveForLater = saveForLater;
		
		return r;
	}
	
	string eventName = "Multiple Choice Question Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}