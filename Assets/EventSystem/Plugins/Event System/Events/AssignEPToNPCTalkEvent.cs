using UnityEngine;
using System.Collections;
using System;

class AssignEPToNPCTalkRunner : IEvent
{
	//[HideInInspector]
	public NPCController npc;
	public EventPlayer eventPlayer;
	
	public void OnEnter ()
	{
		if (npc == null)
		{
			Debug.LogWarning("No NPC has has been specified to reassign a talk event player.");
		}
		else
		{
            // Oleg: NPC reconstruction
			//npc.talkEventPlayer = eventPlayer;
            //npc.canTalk = false;
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
public class AssignEPToNPCTalkEvent : EventBase
{
	//[HideInInspector]
	public NPCController npc;
	public EventPlayer eventPlayer;
	

	public override IEvent CreateRunner ()
	{
		AssignEPToNPCTalkRunner runner = new AssignEPToNPCTalkRunner();

		runner.npc = npc;
		runner.eventPlayer = eventPlayer;
		return runner;
	}
	
	
	string eventName = "Assign EP To NPC Talk Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}