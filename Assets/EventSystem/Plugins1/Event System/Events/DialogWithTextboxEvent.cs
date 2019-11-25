
#define EZ_GUI

using UnityEngine;
using System.Collections;
using System;

class DialogWithTextboxRunner : IEvent
{
    public string text,
                  buttonText,
                  boxStyleName,
                  labelStyleName,
                  buttonStyleName,
                  textboxStyleName;
	
    public GUISkin skin;
    DialogWithTextboxRenderer renderer;

    public void OnEnter()
    {
        if (MovementController.Main)
            MovementController.Main.LockPCControl();

        GameObject go = new GameObject("Dialog with Textbox Renderer");
        renderer = (DialogWithTextboxRenderer)go.AddComponent<DialogWithTextboxRenderer>();
        renderer.SetStyleNames(boxStyleName, labelStyleName, buttonStyleName, textboxStyleName);

        renderer.labelText = text;
        renderer.buttonText = buttonText;
        renderer.Callback = OnClose;
        renderer.skin = skin;
    }

    void OnClose()
    {
        eventFinished = true;
    }

    public void OnExecute ()
	{
        
    }
	
	public void OnExit ()
	{
		GameObject.Destroy(renderer.gameObject);

		if (MovementController.Main)
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
        if (renderer)
        {
            renderer.Terminate();
        }
    }
}

[System.Serializable]
public class DialogWithTextboxEvent : EventBase
{
    public string text,
                  buttonText,
                  boxStyleName = "dialog-with-textbox-box",
                  labelStyleName = "dialog-with-textbox-label",
                  buttonStyleName = "dialog-with-textbox-button",
                  textboxStyleName = "dialog-with-textbox-textbox";

    public GUISkin skin;
	
	public override IEvent CreateRunner ()
	{
        DialogWithTextboxRunner runner = new DialogWithTextboxRunner();
        runner.text = text;
        runner.buttonText = buttonText;
        runner.skin = skin;
        runner.boxStyleName = boxStyleName;
        runner.labelStyleName = labelStyleName;
        runner.buttonStyleName = buttonStyleName;
        runner.textboxStyleName = textboxStyleName;

		return runner;
	}

    string eventName = "Dialog With Textbox Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}