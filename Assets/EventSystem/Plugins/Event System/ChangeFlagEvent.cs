using UnityEngine;
using System.Collections;
using System;

/*
    Use this event as a test bed for making sure 
    all variable types get saved and used correctly 
    by the event editor/system.
*/

class ChangeFlagRunner : IEvent
{
    public GameObject newFlag;

    public void OnEnter()
    {
    }

    public void OnExecute()
    {
        if (FlagManager.Main)
        {
            FlagManager.Main.SetFlagByGameobject(newFlag); 
            eventFinished = true;
        }
    }
    public void OnExit()
    {
    }

    string GetNickname()
    {
        return "ChangeFlagRunner";
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
public class ChangeFlagEvent : EventBase
{
    public GameObject newFlag;

    public override IEvent CreateRunner()
    {
        ChangeFlagRunner runner = new ChangeFlagRunner();
        runner.newFlag = newFlag;
        return runner;
    }

    string eventName = "Change Flag Event";
    public override string GetEventName()
    {
        return eventName;
    }
}