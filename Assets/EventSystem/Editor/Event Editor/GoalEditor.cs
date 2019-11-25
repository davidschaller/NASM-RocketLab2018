using UnityEngine;
using System.Collections;
using UnityEditor;

public class GoalEditor
{
	[MenuItem("Event System/Goals/Add Trigger Goal")]
	[MenuItem("Component/Goals/Add Trigger Goal")]
	public static void AddTriggerGoal()
	{
		if (Selection.activeGameObject != null)
			Selection.activeGameObject.AddComponent(typeof(TriggerGoal));
	}

	[MenuItem("Event System/Goals/Add Dialog Goal")]
	[MenuItem("Component/Goals/Add Dialog Goal")]
	public static void AddDialogGoal()
	{
		if (Selection.activeGameObject != null)
			Selection.activeGameObject.AddComponent(typeof(DialogGoal));
	}
	
	[MenuItem("Event System/Goals/Add Minigame Goal")]
	[MenuItem("Component/Goals/Add Minigame Goal")]
	public static void AddMinigameGoal()
	{
		if (Selection.activeGameObject != null)
			Selection.activeGameObject.AddComponent(typeof(MinigameGoal));
	}
}
