using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class StartFlashbackRunner : IEvent
{
	public string flashbackScene;
	
	public void OnEnter ()
	{
        AudioRunner.TerminateSoundTrack();
		MovementController.Main.LockPCControl();
		FlashbackManager.SaveSceneState(flashbackScene);
		Debug.Log("Loading flashback scene: " + flashbackScene);
		Application.LoadLevelAdditive(flashbackScene);
	}
	public void OnExecute ()
	{
		if (FlashbackManager.CompletelyFinished)
		{
			//Debug.LogWarning("StartFlashbackRunner thinks FBM said flashback is done");

			eventFinished = true;
		}
		
	}
	public void OnExit ()
	{
        if (MovementController.Main.RememberMyHorse != null)
        {
            MovementController.Main.RememberMyHorse.gameObject.active = true;
        }
		MovementController.Main.UnlockPCControl(false);
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
public class StartFlashbackEvent : EventBase
{
	public string flashbackScene;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		StartFlashbackRunner runner = new StartFlashbackRunner();
		runner.flashbackScene = flashbackScene;
		return runner;
	}
	
	string eventName = "Start Flashback Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}