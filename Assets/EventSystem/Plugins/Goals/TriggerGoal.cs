using UnityEngine;
using System.Collections;

public class TriggerGoal : PlayerGoal
{
	void OnTriggerEnter (Collider coll)
	{
		MarkGoalCompleted();
	}
	
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
