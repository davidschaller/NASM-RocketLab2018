using UnityEngine;
using System.Collections;
using System;

class LetterWritingRunner : IEvent
{
	//[HideInInspector]

	static bool letterFinished = false;
	public void OnEnter ()
	{
		letterFinished = false;
		if (NotebookManager.HasSomeInfoCards)
			NotebookManager.InitLetterToMa();
		else
			Debug.LogWarning("Tried to start letter writing minigame, but no info cards found");
	}
	public void OnExecute ()
	{
		if (letterFinished)
			eventFinished = true;
	}
	public void OnExit ()
	{
	}

	public static void FinishLetter ()
	{
		letterFinished = true;
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
public class LetterWritingEvent : EventBase
{
	//[HideInInspector]
	
	public override IEvent CreateRunner ()
	{
		LetterWritingRunner runner = new LetterWritingRunner();
		return runner;
	}
	
	string eventName = "Letter Writing";
	public override string GetEventName ()
	{
		return eventName;
	}
}