using UnityEngine;
using System.Collections;

public class EventPlayer : EventPlayerBase
{
    public delegate void TriggerDelegate(EventPlayer ep);

    TriggerDelegate triggerCallback,
                    finishCallback,
                    resetCallback;

    public void SubscribeForTrigger(TriggerDelegate callback)
    {
        triggerCallback += callback;
    }

    public void SubscribeForFinish(TriggerDelegate callback)
    {
        finishCallback += callback;
    }


    public EventScenario scenario;
    public bool playOnceOnly = false;
    public bool debugPlayer = false;
    public bool deepEventDebug = false;
    public bool respectLOD = false;
    public float lodMaxDistance = 30;
    public float behindLodMaxDistance = 30;
    public EventPlayerBase playEventOnExit;
    public bool canBeTriggeredByCollider;


    bool hasTriggered = false;

    public bool HasTriggered
    {
        get
        {
            return hasTriggered;
        }
    }

    public bool CompletelyFinished
    {
        get
        {
            EventPlayer lastEP = this;

            while (lastEP.playEventOnExit)
                lastEP = (EventPlayer)lastEP.playEventOnExit;

            return lastEP.Finished;
        }
    }

    private bool needRestart = false;
    public bool NeedRestart
    {
        get
        {
            return needRestart;
        }
        private set
        {
        }
    }

    void Awake()
    {
        // TODO Load substitution variables here and make their connections

    }

    public override void StartOnSceneStart()
    {
        StartScenario();
    }

    void ExecuteEvent(EventBase ev)
    {
        if (ev)
        {
            needRestart = true; // if it's already executing need to restart after F1 pressed

            scenario.MapSubstitutions(ev, this);

            StartCoroutine(ev.ExecuteEvent(this, ExecuteEvent, false));
        }
        else
        {
            if (debugPlayer) print("Cann't execute null event");
        }
    }

    public void ForceTerminate(EventBase caller)
    {
        foreach (EventBase ev in scenario.GetEvents())
        {
            if (caller != ev)
            {
                ev.ExitEventEarly(this);
            }
        }

        ThreadCount = 0;
        //ScenarioFinished();
    }

    public bool finished = false;
    public override bool Finished
    {
        get
        {
            return finished;
        }
    }

    int threads = 0;
    public int ThreadCount
    {
        get
        {
            return threads;
        }
        set
        {
            threads = value < 0 ? 0 : value;
            if (debugPlayer)
            {
                if (GUIManager.disablePlayerWarnings)
                    Debug.Log(name + " threadCount = " + threads, gameObject);
            }
        }
    }

    public void ScenarioFinished()
    {
        ThreadCount--;

        if (debugPlayer) Debug.LogWarning(ThreadCount + " EP threads left for " + gameObject.name, gameObject);

        bool unFinished = false;
        if (ThreadCount > 0)
        {
            if (debugPlayer)
            {
                foreach (EventBase ev in scenario.GetEvents())
                {
                    if (ev)
                    {
                        if (!ev.IsFinished())
                        {
                            Debug.LogWarning("Unfinished: " + ev.GetEventName() + " for " + gameObject.name);
                            unFinished = true;
                        }
                        else
                        {
                            Debug.LogWarning("Finished: " + ev.GetEventName() + " for " + gameObject.name);
                        }
                    }
                    else
                    {
                        if (debugPlayer) Debug.LogWarning("ev is null" + " for " + gameObject.name);
                    }
                }
            }

            if (unFinished)
            {
                if (debugPlayer) Debug.LogWarning("<<<< Not ending scenario '" + name + "' yet");

                return;
            }
            else
            {
                // RAL: this breaks Jade dialog 2: pictorial event exits before simple dialog event’s button clicked
                if (debugPlayer) Debug.LogWarning("---- ending scenario '" + name + "', event though its ThreadCount is not zero");
            }
        }
        else
        {
            EventPlayerFinish(true);
        }
    }

    public bool pauseOnExit = false;
    void EventPlayerFinish(bool playEventsOnExit)
    {
        if (pauseOnExit)
            Debug.Break();

        needRestart = false;
        finished = true;

        if (debugPlayer)
        {
            Debug.LogWarning("Writing scenario '" + name + "' to event registry (as completed)");
        }

        if (playEventsOnExit)
        {
            if (finishCallback != null)
            {
                finishCallback(this);
                finishCallback = null;
            }

            if (playEventOnExit)
            {
                if (debugPlayer)
                {
                    Debug.LogWarning("----- Ending scenario " + name + ", playing event player " + playEventOnExit.name + " on exit", gameObject);
                }

                playEventOnExit.PlayerTriggered();
            }
            else if (debugPlayer)
            {
                Debug.LogWarning("----- Event player " + name + " is finished with all events");
            }
        }
    }

    public override void PlayerTriggered()
    {
        if (hasTriggered && playOnceOnly)
            return;

        if (startType == EventStartupType.Trigger || startType == EventStartupType.Both)
        {
            finished = false;
            if (debugPlayer) print(gameObject.name + " being triggered by player");
            StartScenario();
        }
        else
        {
            Debug.LogWarning("This eventPlayer has type 'SceneStart' but someone tries to Trigger it again", gameObject);
        }
    }

    void StartScenario()
    {
        if (hasTriggered && playOnceOnly)
            return;

        hasTriggered = true;

        if (debugPlayer) print("Starting " + name + " scenario with " + scenario.EventCount() + " events");

        //StartCoroutine(scenario.PlayEvents());
        if (scenario)
        {
            EventBase ev0 = scenario.GetEvents()[0];
            //ev0.AddExitEvent(scenario.GetEvents()[1]);
            //ev0.AddExitNotification(ExecuteEvent);
            ExecuteEvent(ev0);
        }
    }

    public override void Pause()
    {
        if (scenario)
            foreach (EventBase ev in scenario.GetEvents())
                if (!ev.EventPlayer.CompletelyFinished)
                    ev.Pause();
    }

    public override void Resume()
    {
        if (scenario)
            foreach (EventBase ev in scenario.GetEvents())
                if (ev.Paused)
                    ev.Resume();
    }

    public void ResetRecursively()
    {
        StopAllCoroutines();

        finished = false;
        hasTriggered = false;

        if (scenario)
            foreach (EventBase ev in scenario.GetEvents())
            {
                // Still a question
                //if (ev.EventPlayer.ThreadCount > 0) 
                if (ev != null)
                    ev.Reset(this);
            }

        if (playEventOnExit != null)
        {
            ((EventPlayer)playEventOnExit).ResetRecursively();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player" && canBeTriggeredByCollider)
        {
            if (!Finished)
                PlayerTriggered();
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Player" && canBeTriggeredByCollider)
        {
            if (!playOnceOnly)
                ResetRecursively();
        }
    }

    public void Stop()
    {
        if (scenario)
            foreach (EventBase ev in scenario.GetEvents())
                if (!ev.EventPlayer.CompletelyFinished)
                    ev.Stop();
    }
}
