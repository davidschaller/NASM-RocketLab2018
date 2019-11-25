using UnityEngine;
using System.Collections;
using System;

class RequirementRunner : IEvent
{
	public TextAsset xmlFile;
	public bool isChecked;
	
	public delegate void AddCallback (RequirementRunner r);
	public static AddCallback register;
	 
	public void OnEnter ()
	{
		eventFinished = true;
		
		if (register != null)
		{
			register(this);
		}
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
public class RequirementEvent : EventBase
{
	public TextAsset xmlFile;
	public bool isChecked;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		RequirementRunner r = new RequirementRunner();
		
		r.xmlFile = xmlFile;
		r.isChecked = isChecked;
		
		return r;
	}
	
	string eventName = "Null Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}