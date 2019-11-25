using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class AnimateNPCRunner : IEvent
{
    public NPCController[] npcs;
    public AnimationClip clip;
    public int timesToPlay = 1;
    public float eventCutoffTime = 0;
    public EventBase ev;
    public bool clampOnExit = false;
    public float maxDesync = 0;
    public Transform lookAt;
    public string animationToBodyPart;
    public WrapMode wrapModeToUse = WrapMode.Default;

    bool timedEvent = false;
    int[] playedCount;
    string[] savedClipNames;

    Quaternion[] savedRotations;

    float timeLeft = 0;
    float savedSpeed = 0;

    List<float> rotateTimers;

    public void OnEnter()
    {
        if (npcs == null || clip == null)
        {
            if (npcs == null)
                Debug.LogError("Animate NPC is terminated. There are no NPCs to animate");
            else
                Debug.LogError("Animate NPC is terminated. Animation clip is missing");

            eventFinished = true;
            return;
        }

        List<NPCController> notNullNpcs = new List<NPCController>();
        rotateTimers = new List<float>();

        for (int i = 0; i < npcs.Length; i++)
            if (npcs[i] != null)
                notNullNpcs.Add(npcs[i]);

        npcs = notNullNpcs.ToArray();


        for (int i = 0; i < npcs.Length; i++)
        {
            npcs[i].ActivateNPC();
        }

        InitCache();

        for (int i = 0; i < npcs.Length; i++)
        {
			/*
            if (!npcs[i].enabled)
            {
                Debug.LogError("(AnimateNPC Event) NPC Controller is not enabled.  Most likely this is because it had no prefab associated with it...highlight it by clicking this log entry", npcs[i].gameObject);
                eventFinished = true;
                return;
            }*/

            Animation anim = npcs[i].GetAnimation();
            if (!anim)
            {
                if (Application.isEditor)
                    Debug.Log("Couldn't get animation from NPC");

                eventFinished = true;
                return;
            }

            savedClipNames[i] = anim.clip.name;

            if (anim[clip.name] == null)
                anim.AddClip(clip, clip.name);

            if (!string.IsNullOrEmpty(animationToBodyPart))
            {
                Transform back = anim.transform.Find(animationToBodyPart);
                if (back)
                    anim[clip.name].AddMixingTransform(back);
            }

            maxDesync = Mathf.Clamp(maxDesync, 0, 1);
            anim[clip.name].normalizedTime = Random.Range(0, maxDesync);
            savedSpeed = anim[clip.name].speed;

            if (wrapModeToUse != WrapMode.Default)
            {
                anim[clip.name].wrapMode = wrapModeToUse;
            }

            rotateTimers.Add(0);

            if (lookAt && npcs[i].canRotateForSalute)
            {
                rotateTimers[i] = LookAt(anim.transform, Quaternion.LookRotation((lookAt.position - anim.transform.position)), 0);
            }

            if (eventCutoffTime > 0)
            {
                timedEvent = true;
                timeLeft = eventCutoffTime;
            }
        }
    }

    float LookAt(Transform tr, Quaternion quaternion, float lookAtTimer)
    {
        lookAtTimer += Time.deltaTime;
		tr.rotation = Quaternion.Lerp(tr.rotation, quaternion, lookAtTimer);
		tr.eulerAngles = new Vector3(0, tr.eulerAngles.y, 0);

        return lookAtTimer;
    }

    void InitCache()
    {
        playedCount = new int[npcs.Length];
        for (int i = 0; i < playedCount.Length; i++)
            playedCount[i] = 0;

        savedClipNames = new string[npcs.Length];
        savedRotations = new Quaternion[npcs.Length];

        for (int i = 0; i < npcs.Length; i++)
            savedRotations[i] = npcs[i].transform.rotation;
    }

    public override string ToString()
    {
        return "Animate NPC";
    }

    int lastFrame = 0;
    public void OnExecute()
    {
        if (Time.frameCount == lastFrame)
            return;

        for (int i = 0; i < npcs.Length; i++)
        {
            Animation anim = npcs[i].GetAnimation();

            if (!anim)
            {
                //Debug.Log(npcs[i].transform.name + " doesn't have animation ", npcs[i].transform);
                continue;
            }

            if (!anim.IsPlaying(clip.name) && playedCount[i] < timesToPlay && LookAt(npcs[i], lookAt, i))
            {
                playedCount[i]++;
                anim.CrossFade(clip.name, .5f);
                
            }
            else if ((anim[clip.name].normalizedTime >= 0.9f && !anim.IsPlaying(savedClipNames[i]) && playedCount[i] == timesToPlay) ||
                     (!anim.IsPlaying(clip.name) && !anim.IsPlaying(savedClipNames[i]) && playedCount[i] == timesToPlay))
            {
                if (!string.IsNullOrEmpty(animationToBodyPart))
                {
                    Transform back = anim.transform.Find(animationToBodyPart);
                    if (!back)
                    {
                        //Debug.Log("revert clip (animationToBodyPart no back) " + clip.name + " to " + savedClipNames[i]);
                        npcs[i].RevertClip(savedClipNames[i]);
                    }
                    else
                    {
                        if (anim["crew_idle"] != null)
                        {
                            anim.CrossFade("crew_idle", .5f);
                        }
                        else if (!clampOnExit)
                        {
                            //Debug.Log("revert clip (animationToBodyPart with back) " + clip.name + " to " + savedClipNames[i]);
                            npcs[i].RevertClip(savedClipNames[i]);
                        }
                    }
                }
                else if (!clampOnExit)
                {
                    //Debug.Log("revert clip " + clip.name + " to " + savedClipNames[i]);
                    npcs[i].RevertClip(savedClipNames[i]);
                }

                playedCount[i]++;
            }
        }

        bool finishAnimation = true;

        foreach (int item in playedCount)
        {
            if (item <= timesToPlay)
            {
                finishAnimation = false;
                break;
            }
        }

        if (finishAnimation)
            eventFinished = true;

        if (timedEvent && timeLeft <= 0)
            eventFinished = true;

        timeLeft -= Time.deltaTime;
        lastFrame = Time.frameCount;
    }

    bool LookAt(NPCController npcController, Transform lookAt, int index)
    {
        bool lookAtTarget = true;

        if (lookAt && npcController.canRotateForSalute && npcController.rotateBackAfterSalute)
        {
            rotateTimers[index] = LookAt(npcController.transform, Quaternion.LookRotation((lookAt.position - npcController.transform.position)), rotateTimers[index]);

            if (rotateTimers[index] < 1)
                lookAtTarget = false;
        }

        return lookAtTarget;
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
public class AnimateNPCEvent : EventBase
{
    public NPCController[] npcs;
    public AnimationClip clip;
    public int timesToPlay = 1;
    public float eventCutoffTime = 0;
    public bool clampOnExit = false;
    public float maxDesync = 0;
    public Transform lookAt;
    public string animationToBodyPart;
    public WrapMode wrapModeToUse = WrapMode.Default;

    public override IEvent CreateRunner()
    {
        AnimateNPCRunner runner = new AnimateNPCRunner();
        runner.npcs = npcs;
        runner.clip = clip;
        runner.timesToPlay = timesToPlay;
        runner.eventCutoffTime = eventCutoffTime;
        runner.clampOnExit = clampOnExit;
        runner.ev = this;
        runner.maxDesync = maxDesync;
        runner.lookAt = lookAt;
        runner.animationToBodyPart = animationToBodyPart;
        runner.wrapModeToUse = wrapModeToUse;

        return runner;
    }

    string eventName = "Animate NPC";
    public override string GetEventName()
    {
        return eventName;
    }

    public override string ToString()
    {
        if (npcs != null && npcs.Length > 0 && clip != null)
            return "Animate NPC Event, npc count: " + npcs.Length.ToString() + ", clip: " + clip.name;
        else
            return "Animate NPC Event, some components are null";
    }
}
