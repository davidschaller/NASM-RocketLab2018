using UnityEngine;
using System.Collections;
using System;

class LoadLevelRunner : IEvent
{
	public string sceneName;
	public bool addScene = false;
	public bool showProgress;
    public Transform trloadLevelDialog;
    public TextAsset xmlFile;
	
	public void OnEnter ()
	{
		if (addScene)
		{
			Debug.Log("Adding new scene: " + sceneName);
			Application.LoadLevelAdditive(sceneName);
		}
		else
		{
			Debug.Log("Loading new scene: " + sceneName);
			if (showProgress)
                LevelLoader.LoadLevelProgress(sceneName, trloadLevelDialog, xmlFile);
			else
				Application.LoadLevel(sceneName);
		}
	}
	public void OnExecute ()
	{
        AudioRunner.TerminateSoundTrack();

		eventFinished = true;
	}
	public void OnExit ()
	{
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
        if (showProgress)
            LevelLoader.Terminate();
    }
}

[System.Serializable]
public class LoadLevelEvent : EventBase
{
	public string sceneName;
	public bool addScene = false;
	public bool showProgress = false;
    public Transform trloadLevelDialog;
    public TextAsset xmlFile;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		LoadLevelRunner runner = new LoadLevelRunner();
		runner.sceneName = sceneName;
		runner.addScene = addScene;
		runner.showProgress = showProgress;
        runner.trloadLevelDialog = trloadLevelDialog;
        runner.xmlFile = xmlFile;

		return runner;
	}
	
	string eventName = "Load Level Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}