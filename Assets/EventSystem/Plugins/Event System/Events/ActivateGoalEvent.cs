using UnityEngine;
using System.Collections;
using System;

class ActivateGoalRunner : IEvent
{
	public PlayerGoal goal;
	
	public void OnEnter ()
	{
		GoalManager.AddGoal(goal);
		eventFinished = true;
	}
	public void OnExecute ()
	{
	}
	public void OnExit ()
	{
		MovementController.Main.SetPCControl();
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
public class ActivateGoalEvent : EventBase
{
	public PlayerGoal goal;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		ActivateGoalRunner runner = new ActivateGoalRunner();
		runner.goal = goal;
		return runner;
	}
	
    // Oleg: It was
	// string eventName = "Dialog Event";
    // Should be "Activate Goal Event", right?
    string eventName = "Activate Goal Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}