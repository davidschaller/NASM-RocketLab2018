using UnityEngine;
using System.Collections;
using System;

class UpdateSkyBoxRunner : IEvent
{
    public float blendFrom;
    public float blendTo;
    public float time;
    public Material skybox;

    private bool processing = false;
    private float timer = 0;

    public void OnEnter()
    {
    }

    public void OnExecute()
    {
        if (RenderSettings.skybox != skybox)
        {
            RenderSettings.skybox = skybox;
        }

        if (!eventFinished)
        {
            if (!processing)
            {
                processing = true;
                skybox.SetFloat("_Blend", blendFrom);
                timer = blendFrom;
            }

            timer += (blendTo - blendFrom) * (Time.deltaTime / time);
            skybox.SetFloat("_Blend", Mathf.Clamp01(timer));

            if (timer >= blendTo)
            {
                processing = false;
                eventFinished = true;
            }
        }
    }

    public void OnExit()
    {
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
public class UpdateSkyBoxEvent : EventBase
{
    //[HideInInspector]
    public float blendFrom;
    public float blendTo;
    public float time;
    public Material skybox;

    public override IEvent CreateRunner()
    {
        UpdateSkyBoxRunner runner = new UpdateSkyBoxRunner();

        runner.blendFrom = blendFrom;
        runner.blendTo = blendTo;
        runner.time = time;
        runner.skybox = skybox;

        return runner;
    }

    string eventName = "Update SkyBox Event";
    public override string GetEventName()
    {
        return eventName;
    }
}

