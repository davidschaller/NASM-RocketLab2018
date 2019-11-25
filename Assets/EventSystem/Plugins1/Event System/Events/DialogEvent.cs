using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

class DialogRunner : IEvent
{
	public DialogDescriptor dialogDescriptor;
	public EventPlayer eventPlayer;
	public NPCController forceNPC;
    public bool cameraStayOnExit = false;

	public void OnEnter ()
	{
		DialogRenderer.Init(dialogDescriptor, eventPlayer);
		if (forceNPC != null)
		{
			DialogRenderer.DialogNPC = forceNPC;
			MovementController.Main.ClearWalkTarget();
			
			PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
            if (cam.Target && !CameraController.Locked())
            {
                if (cam.Target.name == "CameraTarget")
                    cam.transform.position = cam.Target.position;
                else
                    cam.transform.position = cam.Target.position + Vector3.up * cam.WatchTargetHeight;

                if (cam.WatchTarget == null)
                    cam.transform.rotation = cam.Target.rotation;
                else
                {
                    // players is in dialog inside building
                    cam.transform.LookAt(cam.WatchTarget.position + Vector3.up * cam.WatchTargetHeight);
                }
            }

			cam.enabled = true;
            cam.FirstPersonView();
			MovementController.Main.ClearWalkTarget();
		}

		MovementController.Main.LockPCControl();
	}

	public void OnExecute ()
	{
		if (DialogRenderer.IsFinished(dialogDescriptor))
		{
			eventFinished = true;
		}
	}

	public void OnExit ()
	{
		Debug.Log("Setting PC Control on dialog exit");
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
public class DialogEvent : EventBase
{
	public DialogDescriptor dialogDescriptor;
	public EventPlayer eventPlayer;
	public NPCController forceNPC;
    public bool cameraStayOnExit = false;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		DialogRunner runner = new DialogRunner();
		runner.dialogDescriptor = dialogDescriptor;
		runner.eventPlayer = calledBy;
		runner.forceNPC = forceNPC;
        runner.cameraStayOnExit = cameraStayOnExit;
		return runner;
	}
	
	string eventName = "Dialog Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}