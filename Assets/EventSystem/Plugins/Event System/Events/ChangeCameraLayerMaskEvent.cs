using UnityEngine;
using System.Collections;
using System;

class ChangeCameraLayerMaskRunner : IEvent
{
	public LayerMask layerMask;
	public Camera camera;
	
	public void OnEnter ()
	{
		camera.cullingMask = layerMask;
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
    }

    public void OnTerminate()
    {
    }
}

[System.Serializable]
public class ChangeCameraLayerMaskEvent : EventBase
{
	public LayerMask layerMask;
	public Camera camera;
	
	//[HideInInspector]
	public override IEvent CreateRunner ()
	{
		ChangeCameraLayerMaskRunner r = new ChangeCameraLayerMaskRunner();
		r.layerMask = layerMask;
		r.camera = camera;
		return r;
	}
	
	string eventName = "Change Camera Layer Mask Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}