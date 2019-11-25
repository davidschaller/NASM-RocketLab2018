using UnityEngine;
using System.Collections;
using System;

class CheckboxQuizQuestionRunner : IEvent
{
	public TextAsset xmlFile;
	public bool center;
	public Rect dimensions;
	
	public delegate void ActivateCallback(CheckboxQuizQuestionRunner r);
	public static ActivateCallback activate;
	
	public void OnEnter ()
	{
		activate(this);
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
public class CheckboxQuizQuestionEvent : EventBase
{
	public TextAsset xmlFile;
	public bool center;
	public Rect dimensions;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		CheckboxQuizQuestionRunner r = new CheckboxQuizQuestionRunner();
		r.xmlFile = xmlFile;
		r.center = center;
		r.dimensions = dimensions;
		return r;
	}
	
	string eventName = "Checkbox Quiz Question Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}