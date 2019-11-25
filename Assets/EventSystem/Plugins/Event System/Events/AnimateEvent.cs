using UnityEngine;
using System.Collections;
using System;

class AnimateRunner : IEvent
{
	//[HideInInspector]
	public Animation animation;
	public AnimationClip clip;
    public string mixingTransformPath;
    public bool additive = false;
    public int layer = 0;

	float animateTime;
	static int times = 0;
	public void OnEnter ()
	{
        animateTime = 0;

        if (animation && clip)
        {
            if (animation.gameObject.activeSelf && animation.gameObject.activeInHierarchy)
            {
                if (animation[clip.name] == null)
                    animation.AddClip(clip, clip.name);

                if (!string.IsNullOrEmpty(mixingTransformPath))
                {
                    Transform trMixing = animation.transform.Find(mixingTransformPath);

                    if (trMixing)
                    {
                        animation[clip.name].AddMixingTransform(trMixing);
                        animation[clip.name].layer = layer;
                    }
                }

                animation.CrossFade(clip.name, .5f);
            }
            else
                eventFinished = true;
        }
        else
        {
            eventFinished = true;
            if (animation)
            {
                Debug.LogError("Clip is not assigned for " + animation.name, animation.transform);
            }
            else
            {
                Debug.LogError("Animation component or clip is not assigned");
            }
        }
	}
	
	public void OnExecute ()
	{
		animateTime += Time.deltaTime;
        if (animateTime >= clip.length || !animation || !animation.enabled)
        {
            eventFinished = true;
        }
	}
	public void OnExit ()
	{
		times++;
	}

	bool eventFinished = false;
	public bool EventIsFinished ()
	{
		return eventFinished;
	}
	
    public void OnReset()
    {
        if (animation && animation[clip.name] != null)
        {
            bool deactivateAtTheEnd = false;
            if (!animation.gameObject.activeSelf)
            {
                animation.gameObject.SetActive(true);
                deactivateAtTheEnd = true;
            }

            animation[clip.name].time = 0;
            animation.Sample();
            animation.Stop();

            if (deactivateAtTheEnd)
            {
                animation.gameObject.SetActive(false);
            }
        }

        animateTime = 0;
        times = 0;
    }


    public void OnTerminate()
    {
        if (animation && clip)
        {
            if (animation.gameObject.activeSelf && animation.IsPlaying(clip.name))
            {
                animation.Stop(clip.name);
            }
        }
    }
}

[System.Serializable]
public class AnimateEvent : EventBase
{
	//[HideInInspector]
	public Animation animation;
	public AnimationClip clip;
    public string mixingTransformPath;
    public bool additive = false;
    public int layer = 0;

	public override IEvent CreateRunner ()
	{
		AnimateRunner runner = new AnimateRunner();
		runner.animation = animation;
		runner.clip = clip;
        runner.mixingTransformPath = mixingTransformPath;
        runner.additive = additive;
        runner.layer = layer;

		return runner;
	}
	
	
	string eventName = "Animate";
	public override string GetEventName ()
	{
		return eventName;
	}
}