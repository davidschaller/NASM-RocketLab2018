using UnityEngine;
using System.Collections;
using System;

class TreasureDialogRunner : IEvent
{
	public string askText;
	public string text;
	public string okButton;

	public string askGUIStyle = "ask treasure button";
	public string boxGUIStyle = "Box";
	public string textGUIStyle = "Label";
	public string buttonGUIStyle = "Button";
	
	public bool forceCentered = false;
	public bool useFixedWH = false;
	public Rect dimensions;

	public NPCController npcController;
	public AudioClip introAudio;
	public EventPlayer introEventPlayer;
	public bool countsForTreasureHunt;
	public bool replaceNPCAudioAfterExit;
	
	TreasureDialogRenderer renderer;

	AudioClip savedClip;
	public void OnEnter ()
	{
		//MovementController.Main.LockPCControl();
		if (replaceNPCAudioAfterExit)
		{
			savedClip = npcController.GetComponent<AudioSource>().clip;
			npcController.GetComponent<AudioSource>().clip = null;
		}

        // Oleg: NPC reconstruction
		//npcController.IdleRevert();
		
		GameObject go = new GameObject();
		renderer = (TreasureDialogRenderer)go.AddComponent<TreasureDialogRenderer>();
		renderer.askText = askText;
		renderer.text = text;
		renderer.okButton = okButton;

		renderer.askGUIStyle = askGUIStyle;
		renderer.boxGUIStyle = boxGUIStyle;
		renderer.textGUIStyle = textGUIStyle;
		renderer.buttonGUIStyle = buttonGUIStyle;
		
		renderer.forceCentered = forceCentered;
		renderer.useFixedWH = useFixedWH;
		renderer.dimensions = dimensions;
		renderer.introAudio = introAudio;
		renderer.npcController = npcController;
		renderer.introEventPlayer = introEventPlayer;
		renderer.countsForTreasureHunt = countsForTreasureHunt;
		if (countsForTreasureHunt)
		{
			TreasureMysteryScrollGUI.UncheckNPC(npcController.npcName);
			TreasureHuntController.NPCTalkCount++;
			TreasureMiniMap.RemoveMarkerForNPC(npcController.npcName, false);
		}
	}
	
	public void OnExecute ()
	{
		if (renderer.ButtonClicked)
			eventFinished = true;
	}
	
	public void OnExit ()
	{
		Debug.Log("Unlock PC Control");
		
		MovementController.Main.UnlockPCControl(true);
        // Oleg: NPC reconstruction
		//npcController.canTalk = false;
		TreasureHuntController.RegisterNPCTalked(npcController);
        // Oleg: NPC reconstruction
		//npcController.DialogFinished();
		npcController.GetComponent<AudioSource>().clip = null;
		
		GameObject.Destroy(renderer.gameObject);
		DialogRenderer.DialogNPC = null;
		if (replaceNPCAudioAfterExit)
		{
			npcController.GetComponent<AudioSource>().clip = savedClip;
			if (npcController.GetComponent<AudioSource>().clip != null)
				npcController.GetComponent<AudioSource>().Play();
		}
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
        renderer.Terminate();
    }
}

[System.Serializable]
public class TreasureDialogEvent : EventBase
{
	public string askText;
	public string text;
	public string okButton;

	public string askGUIStyle = "ask treasure button";
	public string boxGUIStyle = "Box";
	public string textGUIStyle = "Label";
	public string buttonGUIStyle = "Button";
	
	public bool forceCentered = false;
	public bool useFixedWH = false;
	public Rect dimensions;
	public NPCController npcController;
	public AudioClip introAudio;
	public EventPlayer introEventPlayer;
	public bool countsForTreasureHunt = true;
	public bool replaceNPCAudioAfterExit = false;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		TreasureDialogRunner runner = new TreasureDialogRunner();
		runner.askText = askText;
		runner.text = text;
		runner.okButton = okButton;

		runner.askGUIStyle = askGUIStyle;
		runner.boxGUIStyle = boxGUIStyle;
		runner.textGUIStyle = textGUIStyle;
		runner.buttonGUIStyle = buttonGUIStyle;
		
		runner.forceCentered = forceCentered;
		runner.useFixedWH = useFixedWH;
		runner.dimensions = dimensions;
		runner.npcController = npcController;
		runner.introAudio = introAudio;
		runner.introEventPlayer = introEventPlayer;
		runner.countsForTreasureHunt = countsForTreasureHunt;
		runner.replaceNPCAudioAfterExit = replaceNPCAudioAfterExit;
		return runner;
	}
	
	string eventName = "Treasure Dialog Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}