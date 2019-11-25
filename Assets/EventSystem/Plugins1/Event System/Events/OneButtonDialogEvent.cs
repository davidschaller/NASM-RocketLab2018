
//#define EZ_GUI

using UnityEngine;
using System.Collections;
using System;

class OneButtonDialogRunner : IEvent
{
	public TextAsset xmlFile;
	public Transform trRenderer;

	public void OnEnter ()
	{
        if (trRenderer)
        {
            OneButtonDialogRendererBase renderer = trRenderer.GetComponent<OneButtonDialogRendererBase>();

            if (renderer)
            {
                renderer.Show(xmlFile, ClickCallback);
            }
            else
            {
                eventFinished = true;
                Debug.LogError("OneButtonDialogEvent can't find OneButtonDialogRendererBase component in '" + trRenderer.name + "' transform", trRenderer);
            }
        }
        else
        {
            eventFinished = true;
            Debug.LogError("Renderer Transform is not assigned");
        }
	}

    void ClickCallback()
    {
        eventFinished = true;
    }

    public void OnExecute ()
	{
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
        OnTerminate();
    }

    public void OnTerminate()
    {
        OneButtonDialogRendererBase renderer = trRenderer.GetComponent<OneButtonDialogRendererBase>();
        if (renderer)
        {
            renderer.Hide();
        }
    }
}

[System.Serializable]
public class OneButtonDialogEvent : EventBase
{
	public TextAsset xmlFile;
    public Transform trRenderer;
	
	public override IEvent CreateRunner ()
	{
		OneButtonDialogRunner runner = new OneButtonDialogRunner();
		runner.xmlFile = xmlFile;
        runner.trRenderer = trRenderer;

		return runner;
	}
	
	string eventName = "One Button Dialog Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}