using UnityEngine;
using System.Collections;

public class DialogGoal : PlayerGoal
{
	public NPCController npcForDialog;
	// TODO fill in completion logic for dialogs here...



	public override string GoalTargetName
	{
		get
		{
			// TODO the goal target name will probably be the NPCs name in this case?
			return goalTargetName;
		}
	}
	
	EventPlayerBase savedTalkEP;
	protected override void OnActivate ()
	{
        // Oleg: NPC reconstruction
		Debug.Log("Dialog Goal: " + name + " activated");
		//savedTalkEP = npcForDialog.talkEventPlayer;
		//npcForDialog.talkEventPlayer = (EventPlayer)GetComponent(typeof(EventPlayer));
		
		// need to get a reference to the DialogRenderer and add "DialogFinished" to its list of callbacks.
		// Can we do this?  DialogRenderer won't be active til the PC clicks "Talk" button
		//DialogRenderer.AddCallbackForEventPlayer(npcForDialog.talkEventPlayer, DialogFinished);
	}
	
	void DialogFinished()
	{
		Debug.Log("Dialog has been completed...");
		// TODO possibly modify this function to receive number of questions answered, or whether all questions were answered

        // Oleg: NPC reconstruction
		Debug.Log("DialogFinished... Restoring saved dialog via " + name);
		//npcForDialog.talkEventPlayer = savedTalkEP;
		MarkGoalCompleted();
	}
	
	protected override void OnCompleted ()
	{
		Debug.Log("Dialog goal OnCompleted... Restoring saved dialog via " + name);
        // Oleg: NPC reconstruction
        //npcForDialog.talkEventPlayer = savedTalkEP;
	}
	
	void Update ()
	{
		if (GoalState == GoalManager.GoalStates.Active && indicator != null && npcForDialog != null)
		{
			indicator.transform.position = npcForDialog.transform.position + Vector3.up * indicatorHeight;
		}
	}
}
