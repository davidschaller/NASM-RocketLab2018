using UnityEngine;
using System.Collections;
using System;

class EnableNPCTalkRunner : IEvent
{
    //[HideInInspector]
    public NPCController npc;

    public void OnEnter()
    {
        if (npc == null)
        {
            Debug.LogWarning("No NPC has been specified to enable a talk event.");
        }
        else
        {
            // Oleg: NPC reconstruction
            //npc.canTalk = true;
        }
        eventFinished = true;
    }

    public void OnExecute()
    {
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
public class EnableNPCTalkEvent : EventBase
{
    //[HideInInspector]
    public NPCController npc;

    public override IEvent CreateRunner()
    {
        EnableNPCTalkRunner runner = new EnableNPCTalkRunner();

        runner.npc = npc;
        return runner;
    }


    string eventName = "Enable NPC Talk Event";
    public override string GetEventName()
    {
        return eventName;
    }
}