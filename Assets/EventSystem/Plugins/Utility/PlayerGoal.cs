using UnityEngine;
using System.Collections;
//using System;

public abstract class PlayerGoal : MonoBehaviour
{
	public string goalTargetName = "";
	public abstract string GoalTargetName
	{
		get;
	}
	
	public PlayerGoal activateGoalOnCompletion;
	
	GoalManager.GoalStates goalState;
	public GoalManager.GoalStates GoalState
	{
		get
		{
			return goalState;
		}
		private set {}
	}
	
	IGoalAssigner assignedBy;
	
	public GoalIndicator goalIndicator;
	public float indicatorHeight = 2;
	
	void Awake ()
	{
		goalState = GoalManager.GoalStates.PreActivate;
		if (goalTargetName == "")
			goalTargetName = name;
	}
	
	protected GoalIndicator indicator;
	public Transform Indicator
	{
		get
		{
			return indicator == null ? null : indicator.transform;
		}
	}
	
	public void ActivateGoal ()
	{
		if (goalIndicator)
			indicator = (GoalIndicator)Instantiate(goalIndicator, transform.position + Vector3.up*indicatorHeight, transform.rotation);
		goalState = GoalManager.GoalStates.Active;
		OnActivate();
	}
	
	protected abstract void OnActivate ();
	
	void DestroyIndicator ()
	{
		if (indicator)
		{
			indicator.Destruct();
		}
	}
	
	
	public void MarkGoalCompleted ()
	{
		goalState = GoalManager.GoalStates.Completed;
		DestroyIndicator();
		GoalManager.GoalCompleted(this);
		OnCompleted();
	}
	
	protected abstract void OnCompleted ();
	
	public void MarkGoalAsMissed ()
	{
		goalState = GoalManager.GoalStates.Missed;
		DestroyIndicator();
	}
}
