using UnityEngine;
using System.Collections;

public class GoalManager : MonoBehaviour
{
	public enum GoalStates
	{
		PreActivate,
		Active,
		Completed,
		Missed
	}
	
	public PlayerGoal sceneStartupGoal;
	
	PlayerGoal currentGoal;
	static GoalManager main;
	public static PlayerGoal CurrentGoal
	{
		get
		{
			if (main != null)
				return main.currentGoal;

			return null;
		}
		private set
		{
		}
	}

	public static PlayerGoal FindLastGoal ()
	{
		// FIXME: this needs to walk the goal list if multiple goals
		// are added  (ie: via ActivateGoalEvent) before the current
		// goal is completed.
		return main.currentGoal;
	}

	public static void AddGoal(PlayerGoal goal)
	{
		if (main.currentGoal == null)
		{
			main.currentGoal = goal;
			main.ActivateNextGoal();
		}
		else
		{
			PlayerGoal lastGoal = FindLastGoal();
			if (lastGoal != null && goal != null && lastGoal.transform != goal.transform)
				lastGoal.activateGoalOnCompletion = goal;
		}
	}
	
	void Awake ()
	{
		main = this;
		if (sceneStartupGoal)
		{
			sceneStartupGoal.ActivateGoal();
			currentGoal = sceneStartupGoal;
		}
		Terrain terrain = Terrain.activeTerrain;
		terrain.gameObject.layer = LayerMask.NameToLayer("Terrain");
	}
	
	void ActivateNextGoal()
	{
		currentGoal.ActivateGoal();
	}
	
	public static void GoalCompleted(PlayerGoal goal)
	{
		if (goal.activateGoalOnCompletion)
		{
			Debug.Log("Goal " + goal.name + " completed, activating " + goal.activateGoalOnCompletion.name, goal.activateGoalOnCompletion.gameObject);
			main.currentGoal = goal.activateGoalOnCompletion;
			main.Invoke("ActivateNextGoal", 0.5f);
		}
	}
}
