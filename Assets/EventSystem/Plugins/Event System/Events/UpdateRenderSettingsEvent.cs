using UnityEngine;
using System.Collections;
using System;


class UpdateRenderSettingsRunner : IEvent
{
    //[HideInInspector]
    public bool fog;
    public Color fogColor;
    public float fogDensity;
    public Color ambientLight;
    public float haloStrength;
    public float flareStrength;
    public Material skybox;

    public void OnEnter()
    {
    }

    public void OnExecute()
    {
        if (RenderSettings.fog == fog &&
                RenderSettings.fogColor == fogColor &&
                RenderSettings.fogDensity == fogDensity &&
                //RenderSettings.ambientLight == ambientLight &&
                RenderSettings.haloStrength == haloStrength &&
                RenderSettings.flareStrength == flareStrength &&
                RenderSettings.skybox == skybox)
           
            eventFinished = true;
        else
        {

            RenderSettings.fog = fog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            //RenderSettings.ambientLight = ambientLight;
            RenderSettings.haloStrength = haloStrength;
            RenderSettings.flareStrength = flareStrength;
            RenderSettings.skybox = skybox;
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
public class UpdateRenderSettingsEvent : EventBase
{
    //[HideInInspector]
    public bool fog;
    public Color fogColor;
    public float fogDensity;
    public Color ambientLight;
    public float haloStrength;
    public float flareStrength;
    public Material skybox;

    public override IEvent CreateRunner()
    {
        UpdateRenderSettingsRunner runner = new UpdateRenderSettingsRunner();

        runner.fog = fog;
        runner.fogColor = fogColor;
        runner.fogDensity = fogDensity;
        runner.ambientLight = ambientLight;
        runner.haloStrength = haloStrength;
        runner.flareStrength = flareStrength;
        runner.skybox = skybox;

        return runner;
    }

    string eventName = "Update Render Settings";
    public override string GetEventName()
    {
        return eventName;
    }
}

