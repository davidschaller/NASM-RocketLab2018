using UnityEngine;
using System.Collections;
using System;

/*
	Use this event as a test bed for making sure 
	all variable types get saved and used correctly 
	by the event editor/system.
*/

class BlendSkyboxRunner : IEvent
{
    public Material material;
    public float fadeFrom = 0,
                 fadeTo = 1;
    public float fadeTimeInSec = 3;

    float timer = 0;

    public void OnEnter()
    {
        if (!material)
        {
            Debug.LogError("BlendSkybox Event: Material to fade is not set");
            eventFinished = true;
            timer = 0;
        }
    }

    public void OnExecute()
    {
        if (Time.deltaTime == 0)
            return;

        if (timer > 1)
        {
            eventFinished = true;
        }
        else
        {
            material.SetFloat("_Blend", Mathf.Lerp(fadeFrom, fadeTo, timer));
            timer += Time.deltaTime / fadeTimeInSec;
        }
    }

    public void OnExit()
    {
    }

    string GetNickname()
    {
        return "BlendSkyboxRunner";
    }

    bool eventFinished = false;
    public bool EventIsFinished()
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
public class BlendSkyboxEvent : EventBase
{
    public Material material;
    public float fadeFrom = 0,
                 fadeTo = 1;
    public float fadeTimeInSec = 3;

    public override IEvent CreateRunner()
    {
        BlendSkyboxRunner runner = new BlendSkyboxRunner();
        runner.material = material;
        runner.fadeFrom = fadeFrom;
        runner.fadeTo = fadeTo;
        runner.fadeTimeInSec = fadeTimeInSec;

        return runner;
    }

    string eventName = "Blend Skybox Event";
    public override string GetEventName()
    {
        return eventName;
    }
}