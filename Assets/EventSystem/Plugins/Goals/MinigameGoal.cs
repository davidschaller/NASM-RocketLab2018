using UnityEngine;
using System.Collections;

public class MinigameGoal : PlayerGoal
{
	// TODO fill in completion logic for Minigames here...


	public override string GoalTargetName
	{
		get
		{
			return goalTargetName;
		}
	}
	
	protected override void OnActivate ()
	{
	}
	
	protected override void OnCompleted ()
	{
	}
}
