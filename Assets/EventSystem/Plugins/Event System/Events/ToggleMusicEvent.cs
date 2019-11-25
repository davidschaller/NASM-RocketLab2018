using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class ToggleMusicRunner : IEvent
{
    public bool enableMainMusic = false;

    public AudioClip audioClip;
	
    public void OnEnter ()
	{
	}

	public void OnExecute ()
	{
        if (MusicManager.Main)
        {
            MusicManager.Main.Toggle(enableMainMusic);

            eventFinished = true;
        }
	}

	public void OnExit ()
	{
	}

	string GetNickname ()
	{
        return "ToggleMusicRunner";
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
public class ToggleMusicEvent : EventBase
{
    public bool enableMainMusic = false;
	
	public override IEvent CreateRunner ()
	{
        ToggleMusicRunner runner = new ToggleMusicRunner();
        runner.enableMainMusic = enableMainMusic;
		return runner;
	}

    string eventName = "Toggle Music Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}