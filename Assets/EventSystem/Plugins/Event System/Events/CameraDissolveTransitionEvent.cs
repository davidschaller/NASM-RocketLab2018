using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class CameraDissolveTransitionRunner : IEvent
{
    public Camera camera1,
                  camera2,
                  cameraKeepRendering;


    public float fadeTime;

    public Mesh mesh;
    public bool grow;
    public float rotateAmount;

    float currTime = 0;

	public void OnEnter ()
	{
        if (mesh)
        {
            ScreenWipeCustom.CrossFadeWipe(camera1, camera2, fadeTime, grow, mesh, rotateAmount);
        }
        else
        {
            CameraFade.Fade(camera1, camera2, cameraKeepRendering, fadeTime);
        }
	}

	public void OnExecute ()
	{
        currTime += Time.deltaTime;

        if (currTime > fadeTime)
            eventFinished = true;
        else
        {
            camera1.transform.localScale = Vector3.one;
            camera2.transform.localScale = Vector3.one;
        }
	}

	public void OnExit ()
	{
	}

	string GetNickname ()
	{
        return "CameraDissolveTransitionRunner";
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
public class CameraDissolveTransitionEvent : EventBase
{
    public Camera camera1,
                  camera2,
                  cameraKeepRendering;

    public float fadeTime;

    public Mesh mesh;
    public bool grow;
    public float rotateAmount;
    public bool fadeIn = false;

	public override IEvent CreateRunner ()
	{
        CameraDissolveTransitionRunner runner = new CameraDissolveTransitionRunner();
        runner.camera1 = camera1;
        runner.camera2 = camera2;
        runner.fadeTime = fadeTime;
        runner.mesh = mesh;
        runner.grow = grow;
        runner.rotateAmount = rotateAmount;
        runner.cameraKeepRendering = cameraKeepRendering;

		return runner;
	}

    string eventName = "Camera Dissolve Transition Event";
	public override string GetEventName ()
	{
		return eventName;
	}
}