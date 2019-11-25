using UnityEngine;
using System.Collections;
using System;

class FaceAnimateRunner : IEvent
{
    public NPCController target;
    public string clipName;
    public AudioClip audio;
    public TextAsset xmlSubtitles;

    float timer = 0;

    public void OnEnter()
    {
        FaceFXController faceFX = target.GetComponentInChildren<FaceFXController>();

        if (faceFX)
        {
            if (string.IsNullOrEmpty(clipName))
            {
                Debug.LogError("FaceAnimate failed due to clipName is NULL");
                eventFinished = true;
                return;
            }

            if (!audio)
            {
                Debug.LogError("FaceAnimate failed due to audioClip is NULL");
                eventFinished = true;
                return;
            }

            if (SubtitlesManager.Main && xmlSubtitles)
            {
                SubtitlesManager.Main.Show(xmlSubtitles);
                xmlSubtitles = null;
            }

            faceFX.PlayAnim(clipName, audio);
        }
        else
        {
            Debug.LogError("Can't play Face Animation, because FaceFXController is not found");
            eventFinished = true;
        }
    }

    public void OnExecute()
    {
        timer += Time.deltaTime;

        if (timer > audio.length)
            eventFinished = true;
    }

    public void OnExit()
    {
    }

    string GetNickname()
    {
        return "FaceAnimateRunner";
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
public class FaceAnimateEvent : EventBase
{
    public NPCController target;
    public string clipName;
    public AudioClip audio;
    public TextAsset xmlSubtitles;

    public override IEvent CreateRunner()
    {
        FaceAnimateRunner runner = new FaceAnimateRunner();
        runner.target = target;
        runner.clipName = clipName;
        runner.audio = audio;
        runner.xmlSubtitles = xmlSubtitles;
        xmlSubtitles = null;

        return runner;
    }

    string eventName = "Face Animate Event";
    public override string GetEventName()
    {
        return eventName;
    }
}